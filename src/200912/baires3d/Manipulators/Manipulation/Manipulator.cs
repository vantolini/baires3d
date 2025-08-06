//
//      coded by un
//            --------------
//                     mindshifter.com
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mindshifter
{
    /// <summary>
    /// Defines the current axes on which a manipulator is operating
    /// </summary>
    [Flags]
    public enum AxisFlags : int
    {
        None = 0,

        X = (0x1 << 0),
        Y = (0x1 << 1),
        Z = (0x1 << 2),

        XY = X | Y,
        YX = Y | X,
        XZ = X | Z,
        ZX = Z | X,
        YZ = Y | Z,
        ZY = Z | Y,

        XYZ = X | Y | Z,

        All = XYZ,
    }

    /// <summary>
    /// Defines the directions of an axis
    /// </summary>
    [Flags]
    public enum AxisDirections
    {
        Positive = (0x1 << 0),      // Positive direction of an axis
        Negative = (0x1 << 1),      // Negative direction of an axis

        All = Positive | Negative,  // Both positive and negative directions
    }

    /// <summary>
    /// Defines the transformation mode of a manipulator
    /// </summary>
    [Flags]
    public enum TransformationMode
    {
        None,                               // No manipulation mode

        TranslationAxis = (0x1 << 0),        // Manipulating the position of an object along an axis
        TranslationPlane = (0x1 << 1),       // Manipulating the position of an object along a plane
        Rotation = (0x1 << 2),               // Manipulating the orientation of an object around an axis
        ScaleAxis = (0x1 << 3),              // Manipulating the scale of an object on an axis
        ScalePlane = (0x1 << 4),             // Manipulating the scale of an object on a plane (two-axes)
        ScaleUniform = (0x1 << 5),           // Manipulating the scale of an object uniformly
    }

    /// <summary>
    /// Defines the vector space in which a manipulator is operating
    /// </summary>
    public enum VectorSpace
    {
        World,              // Manipulating with world space basis vectors
        Local               // Manipulating with local space basis vectors
    }

    public sealed partial class Manipulator
    {
        // indices of the lines connecting a bounding box's corners
        private static short[] BoundIndices = new short[]
        {
            // x
            0, 1,   2, 3,   4, 5,   6, 7,

            // y
            0, 3,   1, 2,   4, 7,   5, 6,

            // z
            0, 4,   1, 5,   2, 6,   3, 7,
        };

        /// <summary>
        /// Helper class that caches mouse input data used by the manipulator
        /// </summary>
        private class Input
        {
            public Vector2 Start;
            public Vector2 End;
            public Vector2 Delta;

            public bool LeftButton;
            public bool LeftClick;
            public bool LeftRelease;

            public int X { get { return (int)End.X; } }
            public int Y { get { return (int)End.Y; } }

            public float Length
            {
                get { return Delta.Length(); }
            }

            public Input()
            {
                Reset();
            }

            public void Reset()
            {
                MouseState mouse = Mouse.GetState();
                Start = End = new Vector2(mouse.X, mouse.Y);
                Delta = Vector2.Zero;
                LeftButton = LeftClick = (mouse.LeftButton == ButtonState.Pressed);
                LeftRelease = !LeftButton;
            }

            public void Cycle()
            {
                Start = End;

                MouseState mouse = Mouse.GetState();
                End = new Vector2(mouse.X, mouse.Y);

                Delta = End - Start;

                LeftClick = (LeftButton) ? (false) : (mouse.LeftButton == ButtonState.Pressed);
                LeftRelease = (LeftButton) ? (mouse.LeftButton == ButtonState.Released) : (false);
                LeftButton = mouse.LeftButton == ButtonState.Pressed;
            }
        }

        private delegate void DrawFunction(AxisFlags axis);
        private delegate void ManipFunction();

        private class DrawFunctions : Dictionary<TransformationMode, Dictionary<AxisFlags, DrawFunction>> 
        {
            public DrawFunctions()
            {
                foreach(TransformationMode mode in Enum.GetValues(typeof(TransformationMode)))
                    this[mode] = new Dictionary<AxisFlags, DrawFunction>();
            }
        }

        private class ManipFunctions : Dictionary<TransformationMode, Dictionary<AxisFlags, ManipFunction>> 
        {
            public ManipFunctions()
            {
                foreach (TransformationMode mode in Enum.GetValues(typeof(TransformationMode)))
                    this[mode] = new Dictionary<AxisFlags, ManipFunction>();
            }
        }

        private IGraphicsDeviceService          mGraphics;
        private VertexDeclaration               mVertexDeclaration;
        private Effect                          mEffect;

        private PickBuffer                      mPickBuffer;
        private DepthStencilBuffer              mDepthBuffer;

        private DrawFunctions                   mDrawFunctions;
        private ManipFunctions                  mManipFunctions;

        private ManipulatorSettings             mSettings;

        private Input                           mInput;

        private ITransform                      mTransform;
        private ICamera                         mCamera;

        private TransformationMode              mActiveMode;
        private TransformationMode              mEnabledModes;
        private AxisFlags                       mSelectedAxes;
        private VectorSpace                     mVectorSpace;

        private bool                            mManipulating;

        private Stack<TransformState>           mUndoStack;
        private Stack<TransformState>           mRedoStack;

        private float                           mScale;

        /// <summary>
        /// Gets or sets the ITransform operated on by the manipulator
        /// </summary>
        public ITransform Transform
        {
            get { return mTransform; }
            set 
            {
                if (mTransform == value)
                    return;

                mTransform = value;
                mManipulating = false;

                mInput.Reset();

                mUndoStack.Clear();
                mRedoStack.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the ICameraProvider from which the manipulator gets the current view 
        /// and projection matrices
        /// </summary>
        public ICamera Camera
        {
            get { return mCamera; }
            set { mCamera = value; }
        }

        /// <summary>
        /// Shortcut accessor for retrieving the current view matrix
        /// </summary>
        private Matrix ViewMatrix
        {
            get 
            { 
                return (mCamera != null) 
                    ? (mCamera.ViewMatrix) 
                    : (Matrix.Identity);
            }
        }

        /// <summary>
        /// Shortcut accessor for retrieving the current projection matrix
        /// </summary>
        private Matrix ProjectionMatrix
        {
            get
            {
                return (mCamera != null)
                    ? (mCamera.ProjectionMatrix)
                    : (Matrix.Identity);
            }
        }

        /// <summary>
        /// Shortcut accessor for retrieving the manipulator's world matrix
        /// </summary>
        private Matrix WorldMatrix
        {
            get
            {
                if(mTransform == null)
                    return Matrix.Identity;

                switch(mVectorSpace)
                {
                    case VectorSpace.Local:
                        {
                            return Matrix.CreateScale(mScale)
                                * Matrix.CreateFromQuaternion(mTransform.Rotation)
                                * Matrix.CreateTranslation(mTransform.Translation);
                        }

                    case VectorSpace.World:
                        {
                            return Matrix.CreateScale(mScale)
                                * Matrix.CreateTranslation(mTransform.Translation);
                        }

                    default:
                        return Matrix.Identity;
                }
            }
        }

        /// <summary>
        /// Shortcut accessor for retrieving the current viewport
        /// </summary>
        private Viewport Viewport
        {
            get
            {
                return (mGraphics.GraphicsDevice == null)
                    ? (new Viewport())
                    : (mGraphics.GraphicsDevice.Viewport);
            }
        }

        /// <summary>
        /// Gets the manipulator's active transformation mode
        /// </summary>
        public TransformationMode ActiveMode
        {
            get { return mActiveMode; }
        }

        /// <summary>
        /// Gets or sets the transformation modes enabled on the manipulator
        /// </summary>
        public TransformationMode EnabledModes
        {
            get { return mEnabledModes; }
            set 
            {
                if (mEnabledModes == value)
                    return;

                if (mManipulating)
                    mRedoStack.Clear();

                mManipulating = false;
                mEnabledModes = value;
                mActiveMode = TransformationMode.None;
            }
        }

        /// <summary>
        /// Gets the axes currently being operated on by the manipulator
        /// </summary>
        public AxisFlags SelectedAxis
        {
            get { return mSelectedAxes; }
        }

        /// <summary>
        /// Gets or sets the vector space in which the manipulator will operate
        /// </summary>
        public VectorSpace VectorSpace
        {
            get { return mVectorSpace; }
            set 
            {
                if (mManipulating)
                    mRedoStack.Clear();

                mManipulating = false;
                mVectorSpace = value; 
            }
        }   

        /// <summary>
        /// Gets a flag indicating whether or not the manipulator is currently active
        /// </summary>
        public bool Active
        {
            get { return (mTransform != null); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the mouse is focused on the manipulator, 
        /// (currently hovering over one of the manipulator's axes)
        /// </summary>
        public bool Focused
        {
            get 
            { 
                return (Active && (mSelectedAxes != AxisFlags.None)); 
            }
        }

        /// <summary>
        /// Gets the size of the manipulator's undo stack
        /// </summary>
        public int UndoStackSize
        {
            get { return mUndoStack.Count; }
        }

        /// <summary>
        /// Gets the size of the manipulator's redo stack
        /// </summary>
        public int RedoStackSize
        {
            get { return mRedoStack.Count; }
        }

        /// <summary>
        /// Gets the manipulator's settings
        /// </summary>
        public ManipulatorSettings Settings
        {
            get { return mSettings; }
        }

        /// <summary>
        /// Creates a new instance of Manipulator
        /// </summary>
        /// <param name="graphics">The IGraphicsDeviceService with which drawing will be performed</param>
        /// <param name="camera">A provider for camera view and projection data</param>
        public Manipulator(IGraphicsDeviceService graphics, ICamera camera)
        {
            mGraphics = graphics;
            mCamera = camera;

            mInput = new Input();

            mSelectedAxes = AxisFlags.None;
            mActiveMode = TransformationMode.None;
            mEnabledModes = TransformationMode.None;
            mVectorSpace = VectorSpace.World;

            mPickBuffer = new PickBuffer(graphics);

            mSettings = new ManipulatorSettings();
            mSettings.RestoreDefaults();

            mGraphics.DeviceCreated += new EventHandler(OnDeviceCreated);
            mGraphics.DeviceDisposing += new EventHandler(OnDeviceDisposing);
            if ((mGraphics.GraphicsDevice != null) && !mGraphics.GraphicsDevice.IsDisposed)
                OnDeviceCreated(this, null);

            mUndoStack = new Stack<TransformState>();
            mRedoStack = new Stack<TransformState>();

            mDrawFunctions = new DrawFunctions();
            mManipFunctions = new ManipFunctions();

            mDrawFunctions[TransformationMode.None][AxisFlags.X]
                = mDrawFunctions[TransformationMode.None][AxisFlags.Y]
                = mDrawFunctions[TransformationMode.None][AxisFlags.Z]
                    = delegate(AxisFlags axis)
                    {
                        Vector3 unit = GetUnitAxis(axis);
                        Primitives.DrawLine(mGraphics.GraphicsDevice, Vector3.Zero, unit);
                    };

            InitTranslation();
            InitRotation();
            InitScale();
        }

        /// <summary>
        /// Event handler called when the graphics device is created
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void OnDeviceCreated(object sender, EventArgs e)
        {
            GraphicsDevice device = mGraphics.GraphicsDevice;

            CompiledEffect ce = Effect.CompileEffectFromSource(EffectSource, new CompilerMacro[0],
                null, CompilerOptions.None, TargetPlatform.Windows);

            mEffect = new Effect(mGraphics.GraphicsDevice, ce.GetEffectCode(), CompilerOptions.None, null);

            mVertexDeclaration = new VertexDeclaration(device,
                new VertexElement[] { new VertexElement(0, 0, VertexElementFormat.Vector3,
                        VertexElementMethod.Default, VertexElementUsage.Position, 0) });

            int bbw = mGraphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int bbh = mGraphics.GraphicsDevice.PresentationParameters.BackBufferHeight;

            mDepthBuffer = new DepthStencilBuffer(mGraphics.GraphicsDevice, bbw, bbh,
                mGraphics.GraphicsDevice.DepthStencilBuffer.Format);
        }

        /// <summary>
        /// Event handler called when the graphics device is disposing
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void OnDeviceDisposing(object sender, EventArgs e)
        {
            if ((mEffect != null) && !mEffect.IsDisposed)
                mEffect.Dispose();

            if ((mVertexDeclaration != null) && !mVertexDeclaration.IsDisposed)
                mVertexDeclaration.Dispose();

            mEffect = null;
            mVertexDeclaration = null;
        }

        /// <summary>
        /// Utility function that returns the unit axis in Vector3 format that corresponds to the 
        /// specified axes, oriented based on the vector space of the manipulator
        /// </summary>
        /// <param name="axis">The axes for which to retrieve the corresponding unit axis</param>
        /// <returns>The unit axis that corresponds to the specified axes</returns>
        private Vector3 GetUnitAxis(AxisFlags axes)
        {
            Vector3 unit = Vector3.Zero;

            if ((axes & AxisFlags.X) == AxisFlags.X)
                unit += Vector3.UnitX;
            if ((axes & AxisFlags.Y) == AxisFlags.Y)
                unit += Vector3.UnitY;
            if ((axes & AxisFlags.Z) == AxisFlags.Z)
                unit += Vector3.UnitZ;

            if (unit != Vector3.Zero)
                unit.Normalize();

            // in local vector space, rotate the axis with the transform's
            // rotation component, otherwise return the axis in its default
            // form for world vector space
            unit = ((mVectorSpace == VectorSpace.Local)
                        && (mTransform != null))
                ? (Vector3.TransformNormal(unit, 
                    Matrix.CreateFromQuaternion(mTransform.Rotation)))
                : (unit);

            return unit;
        }

        /// <summary>
        /// Utility function that returns the origin plane whose normal is perpendicular to the 
        /// vectors of the specified axes if multiple axes are specified, or the plane
        /// whose normal is the unit vector of the specified axis if a single axis is specified
        /// </summary>
        /// <param name="axis">The axes for which to retrieve the corresponding plane</param>
        /// <returns>The origin plane that corresponds to the specified axes</returns>
        private Plane GetPlane(AxisFlags axis)
        {
            Plane p = new Plane();

            switch (axis)
            {
                case AxisFlags.X:
                case AxisFlags.Y | AxisFlags.Z:
                    p.Normal = Vector3.UnitX;
                    break;

                case AxisFlags.Y:
                case AxisFlags.X | AxisFlags.Z:
                    p.Normal = Vector3.UnitY;
                    break;

                case AxisFlags.Z:
                case AxisFlags.X | AxisFlags.Y:
                    p.Normal = Vector3.UnitZ;
                    break;
            }

            if (mTransform == null)
                return p;

            p = (mVectorSpace == VectorSpace.Local)
                ? (Plane.Transform(p, Matrix.CreateFromQuaternion(mTransform.Rotation)
                    * Matrix.CreateTranslation(mTransform.Translation)))
                : (Plane.Transform(p, Matrix.CreateTranslation(mTransform.Translation)));

            return p;
        }

        /// <summary>
        /// Utility function that returns the display color of the specified world axis
        /// </summary>
        /// <param name="axis">The world axis for which to retrieve the corresponding display color</param>
        /// <returns>The color of the specified axis</returns>
        private Color GetAxisColor(AxisFlags axis)
        {
            Color color = ((mSelectedAxes & axis) == axis)
                ? (mSettings.SelectionColor)
                : (mSettings.GetAxisColor(axis));

            return color;
        }

        private Color GetAxisColor(TransformationMode mode, AxisFlags axis)
        {
            Color color = (mActiveMode == mode)
                ? (GetAxisColor(axis))
                : (mSettings.GetAxisColor(axis));

            return color;
        }

        /// <summary>
        /// Utility function that returns a pick ray originating from the near plane at the specified
        /// screen space position with a direction out toward the far plane
        /// </summary>
        /// <param name="position">The screen space position from which to project a ray</param>
        /// <returns>A pick ray cast into the scene from the specified screen position</returns>
        private Ray GetPickRay(Vector2 position)
        {
            // calculate manually projected start and end points at the near and far
            // clip planes where the mouse ray would intersect them
            Vector3 near = new Vector3(position, 0f);
            Vector3 far = new Vector3(position, 1f);

            // unproject the start and end points into world space using the current view
            // and projection matrices
            near = Viewport.Unproject(near, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            far = Viewport.Unproject(far, ProjectionMatrix, ViewMatrix, Matrix.Identity);

            // construct a pick ray with origin at the near world point and
            // normalized direction from near to far point
            Ray pickRay = new Ray(near, Vector3.Normalize(far - near));

            return pickRay;
        }

        /// <summary>
        /// Reverts the most recent change to the manipulator's transformation
        /// </summary>
        /// <returns>True if the operation was successful, false otherwise</returns>
        public bool Undo()
        {
            // no undos while we're inactive
            if (!Active)
                return false;

            // nothing to undo...
            if (mUndoStack.Count == 0)
                return false;

            // user requested an undo while manipulating. we must reset the 
            // manipulation flag, requiring that the user release and press the 
            // mouse button again in order to start a new operation. otherwise, 
            // the mouse position and manipulator position will be out of sync 
            // after the transform is reverted
            if (mManipulating)
            {
                mRedoStack.Clear();
                mManipulating = false;
            }

            // push the current transform state onto the redo stack
            mRedoStack.Push(new TransformState(mTransform));

            // pop the top transform state from the undo stack and apply
            // it to the current transform
            TransformState state = mUndoStack.Pop();
            state.Apply();

            // success
            return true;
        }

        /// <summary>
        /// Restores the most recent reversion to the manipulator's transformation
        /// </summary>
        /// <returns>True if the operation was successful, false otherwise</returns>
        public bool Redo()
        {
            // no redos while we're active or in manipulation mode
            if (!Active || mManipulating)
                return false;

            // nothing to redo...
            if (mRedoStack.Count == 0)
                return false;

            // push the current transform onto the undo stack
            mUndoStack.Push(new TransformState(mTransform));

            // pop the topmost transform state from the redo stack
            // and apply it to the current transform
            TransformState state = mRedoStack.Pop();
            state.Apply();

            // success
            return true;
        }

        /// <summary>
        /// Updates the manipulator and processes any input if the
        /// manipulator is active
        /// </summary>
        public void Update()
        {
            // we won't update if the manipulator is inactive
            if (!Active)
                return;

            mInput.Cycle();

            // current view (eye) position
            Vector3 viewpos = Matrix.Invert(ViewMatrix).Translation;

            // create a new view matrix that will allow us to calculate the necessary
            // scale of the manipulator based on how far the camera is from the world
            // position of the transform. this matrix looks down the negative z axis
            // toward the origin from a point on the positive z axis whose distance
            // from the origin is the same as the distance from the current camera's
            // eye position to the manipulator's world position.
            Matrix scaleview = Matrix.CreateLookAt(Vector3.UnitZ *
                Vector3.Distance(viewpos, mTransform.Translation),
                Vector3.Zero, Vector3.UnitY);

            // project two points onto the screen that represent the start and end of the unit x
            // axis from the above view matrix's point of view. we'll use the difference between
            // the projected points to measure the amount we need to scale the manipulator to make it
            // appear the same size regardless of the distance from the camera
            Vector3 projstart = Viewport.Project(Vector3.Zero, ProjectionMatrix, scaleview, Matrix.Identity);
            Vector3 projend = Viewport.Project(Vector3.UnitX, ProjectionMatrix, scaleview, Matrix.Identity);

            // using a base scale, we divide by the length of the projected difference
            // to get the relative scale
            mScale = 50.0f / Vector3.Subtract(projend, projstart).Length();

            // if we're not in the middle of a manipulation operation then 
            // do a pick buffer pass
            if (!mManipulating)
                Pick();

            // if the left mouse button is in the released state it is possible that
            // the user has just ended a manipulation operation
            if (!mInput.LeftButton)
            {
                // if the user was in manipulation mode we must clear the redo stack
                // since the current manipulation operation overwrites any forward
                // changes
                if(mManipulating)
                    mRedoStack.Clear();

                // if the user has released the mouse, then we're no longer manipulating
                mManipulating = false;
            }
            else if (mInput.LeftClick)
            {
                // the user has performed a mouse-down event, so if our selected
                // axis is valid then we'll flag the manipulator as actively in
                // manipulation mode
                if (mSelectedAxes != AxisFlags.None)
                {
                    mManipulating = true;
                    mUndoStack.Push(new TransformState(mTransform));
                }
            }

            // if the user is currently manipulating then pass along the input values to the 
            // appropriate transformation function where it will perform the necessary transformations 
            // and update the transform
            if (mManipulating && (mInput.Length > 0)
                && mManipFunctions[mActiveMode].ContainsKey(mSelectedAxes))
            {
                // execute the callback function defined for the context
                mManipFunctions[mActiveMode][mSelectedAxes]();
            }
        }

        private void Pick()
        {
            mPickBuffer.PushMatrix(MatrixMode.View, ViewMatrix);
            mPickBuffer.PushMatrix(MatrixMode.Projection, ProjectionMatrix);
            mPickBuffer.PushMatrix(MatrixMode.World, WorldMatrix);
            mPickBuffer.PushVertexDeclaration(mVertexDeclaration);

            foreach (TransformationMode mode in mDrawFunctions.Keys)
            {
                TransformationMode local_mode = mode;

                if ((mEnabledModes & local_mode) != local_mode)
                    continue;

                foreach (AxisFlags flags in mDrawFunctions[local_mode].Keys)
                {
                    AxisFlags local_flags = flags;

                    mPickBuffer.PushPickID(((uint)local_mode << 8) | (uint)flags);
                    mPickBuffer.QueueRenderFunction(delegate(GraphicsDevice device)
                    {
                        mDrawFunctions[local_mode][local_flags](local_flags);
                    });
                    mPickBuffer.PopPickID();
                }
            }

            mPickBuffer.PopVertexDeclaration();
            mPickBuffer.PopMatrix(MatrixMode.View);
            mPickBuffer.PopMatrix(MatrixMode.Projection);
            mPickBuffer.PopMatrix(MatrixMode.World);

            CullMode oldMode = mGraphics.GraphicsDevice.RenderState.CullMode;
            mGraphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            // render the pick buffer queue
            mPickBuffer.Render();

            mGraphics.GraphicsDevice.RenderState.CullMode = oldMode;

            // pick using current mouse position and convert picked
            // value back into an Axis value (safe, as the default pick buffer
            // id is 0 (AxisFlags.None) and only AxisFlags values are being rendered as
            // pick ids in the above render pass
            uint pick_id = mPickBuffer.Pick(mInput.X, mInput.Y);

            mSelectedAxes = (AxisFlags)(pick_id & 0xFF);
            mActiveMode = (TransformationMode)((pick_id >> 8) & 0xFF);
        }

        /// <summary>
        /// Draws the manipulator
        /// </summary>
        public void Draw()
        {
            // don't draw if we're inactive
            if (!Active)
                return;

            GraphicsDevice device = mGraphics.GraphicsDevice;

            // set the vertex declaration for Vector3 verts
            device.VertexDeclaration = mVertexDeclaration;

            // set the view and projection matrices
            mEffect.Parameters["View"].SetValue(ViewMatrix);
            mEffect.Parameters["Projection"].SetValue(ProjectionMatrix);

            // draw the transform's bounds if it is an IBoundedTransform
            // and draw bounds is enabled
            if (mSettings.DrawBoundsEnabled && (mTransform is IBoundedTransform))
                DrawBounds((IBoundedTransform)mTransform);

            // clear the depth buffer so the manipulator will draw on top of everything
            DepthStencilBuffer depth = device.DepthStencilBuffer;
            device.DepthStencilBuffer = mDepthBuffer;
            device.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            mEffect.Parameters["World"].SetValue(WorldMatrix);

            mEffect.Begin(SaveStateMode.SaveState);
            mEffect.CurrentTechnique.Passes[0].Begin();

            foreach (TransformationMode mode in mDrawFunctions.Keys)
            {
                if ((mEnabledModes & mode) != mode)
                    continue;

                foreach (AxisFlags flags in mDrawFunctions[mode].Keys)
                {
                    mEffect.Parameters["Color"].SetValue(GetAxisColor(mode, flags).ToVector3());
                    mEffect.CommitChanges();

                    mDrawFunctions[mode][flags](flags);
                }
            }

            mEffect.CurrentTechnique.Passes[0].End();
            mEffect.End();

            device.DepthStencilBuffer = depth;
        }

        /// <summary>
        /// Draws the transformed bounding box of the specified IBoundedTransform, using
        /// the world matrix derived from the IBoundedTransform's ITransform properties
        /// </summary>
        /// <param name="tform">The IBoundedTransform for which to draw the bounding box</param>
        private void DrawBounds(IBoundedTransform tform)
        {
            // vertices of the bounding box corners
            Vector3[] vtx = tform.Bounds.GetCorners();

            // colors of the axis lines
            Color[] colors = new Color[]
            {
                mSettings.XAxisColor,
                mSettings.YAxisColor,
                mSettings.ZAxisColor,
            };

            // set the IBoundedTransform's world matrix
            mEffect.Parameters["World"].SetValue(Matrix.CreateScale(tform.Scale)
                * Matrix.CreateFromQuaternion(tform.Rotation)
                * Matrix.CreateTranslation(tform.Translation));

            // begin effect
            mEffect.Begin();
            mEffect.CurrentTechnique.Passes[0].Begin();

            // draw each set of bounding lines
            for (int i = 0, col = 0; i < BoundIndices.Length; i += 8, col++)
            {
                // set appropriate axis color on the effect
                mEffect.Parameters["Color"].SetValue(colors[col].ToVector3());
                mEffect.CommitChanges();

                // draw the four bounding lines for this axis
                mGraphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList,
                    vtx, 0, 8, BoundIndices, i, 4);
            }

            // end effect
            mEffect.CurrentTechnique.Passes[0].End();
            mEffect.End();
        }

        private static string EffectSource =
    @"  float4x4 World;
        float4x4 View;
        float4x4 Projection;
        float3 Color;

        float4 VS(float4 Position : POSITION0) : POSITION0
        {
            return mul(Position, mul(World, mul(View, Projection)));
        }

        float4 PS(float4 Position : POSITION0) : COLOR0
        {
            return float4(Color, 1.0f);
        }

        Technique Color
        {
            Pass Color
            {
                CullMode = None;
                
                VertexShader = compile vs_1_1 VS();
                PixelShader = compile ps_1_1 PS();
            }
        }";
    }
}