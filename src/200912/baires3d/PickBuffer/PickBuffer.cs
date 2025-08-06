//
//      coded by un
//            --------------
//                     mindshifter.com
//


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    // Describes a vertex transformation stage matrix
    public enum MatrixMode
    {
        World,
        View,
        Projection
    }

    // internal delegate to handle queued renderables
    internal delegate void PickRenderer();

    // public delegate to support custom draw calls for pick buffer clients
    public delegate void OverridePickRenderer(GraphicsDevice device);

    internal struct PickRenderable
    {
        public PickRenderer    Render;
        public Matrix          WVP;
        public Color           PickColor;

        internal PickRenderable(PickRenderer render, Matrix wvp, Color pick_color)
        {
            Render = render;
            WVP = wvp;
            PickColor = pick_color;
        }
    }

    public class PickBuffer
    {
        private IGraphicsDeviceService      mGraphics;

        private RenderTarget2D              mBuffer;
        private Texture2D                   mPickTexture;
        private DepthStencilBuffer          mDepthBuffer;

        private PickEffect                  mEffect;

        private Queue<PickRenderable>       mRenderables;

        private Dictionary<MatrixMode,
                        Stack<Matrix>>      mMatrices;

        private Stack<VertexDeclaration>    mVertexDeclaration;
        private Stack<VertexBuffer>         mVertexBuffer;
        private Stack<IndexBuffer>          mIndexBuffer;

        private Stack<uint>                 mPickID;

        /// <summary>
        /// Gets the size of the render queue for the current frame
        /// </summary>
        public int QueueSize
        {
            get { return mRenderables.Count; }
        }

        /// <summary>
        /// Gets the size of the view matrix stack
        /// </summary>
        public int ViewMatrixStackSize
        {
            get { return mMatrices[MatrixMode.View].Count; }
        }

        /// <summary>
        /// Gets the size of the projection matrix stack
        /// </summary>
        public int ProjectionMatrixStackSize
        {
            get { return mMatrices[MatrixMode.Projection].Count; }
        }

        /// <summary>
        /// Gets the size of the world matrix stack
        /// </summary>
        public int WorldMatrixStackSize
        {
            get { return mMatrices[MatrixMode.World].Count; }
        }

        /// <summary>
        /// Gets the size of the pick id stack
        /// </summary>
        public int PickIDStackSize
        {
            get { return mPickID.Count; }
        }

        /// <summary>
        /// Gets the size of the vertex buffer stack
        /// </summary>
        public int VertexBufferStackSize
        {
            get { return mVertexBuffer.Count; }
        }

        /// <summary>
        /// Gets the size of the index buffer stack
        /// </summary>
        public int IndexBufferStackSize
        {
            get { return mIndexBuffer.Count; }
        }

        /// <summary>
        /// Gets the size of the vertex declaration stack
        /// </summary>
        public int VertexDeclarationStackSize
        {
            get { return mVertexDeclaration.Count; }
        }

        /// <summary>
        /// Creates a new instance of PickBuffer
        /// </summary>
        /// <param name="graphics">The IGraphicsDeviceService to be used for rendering</param>
        public PickBuffer(IGraphicsDeviceService graphics)
        {
            // store graphics device service
            mGraphics = graphics;

            // Render queue
            mRenderables = new Queue<PickRenderable>();

            // Matrix stacks for each MatrixMode
            mMatrices = new Dictionary<MatrixMode, Stack<Matrix>>();
            for (MatrixMode m = MatrixMode.World; m <= MatrixMode.Projection; ++m)
                mMatrices[m] = new Stack<Matrix>();

            // Vertex and index data stacks
            mVertexDeclaration = new Stack<VertexDeclaration>();
            mVertexBuffer = new Stack<VertexBuffer>();
            mIndexBuffer = new Stack<IndexBuffer>();

            // Pick id stack
            mPickID = new Stack<uint>();

            // Hook into the device created and reset events so we can re-create our
            // render target buffer when necessary
            mGraphics.DeviceCreated += new EventHandler(CreateBuffer);
            mGraphics.DeviceReset += new EventHandler(CreateBuffer);

            // If the graphics device is already up and running we won't hit the above events,
            // so check manually for whether or not we need to create the render target
            // buffer right now
            if ((mGraphics.GraphicsDevice != null) && !mGraphics.GraphicsDevice.IsDisposed)
                CreateBuffer(this, null);

            mEffect = new PickEffect(mGraphics);
        }

        /// <summary>
        /// Creates the render target used to store pick id data
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void CreateBuffer(object sender, EventArgs e)
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            PresentationParameters prm = mGraphics.GraphicsDevice.PresentationParameters;

            int bbw = prm.BackBufferWidth;
            int bbh = prm.BackBufferHeight;

            mBuffer = new RenderTarget2D(dev, bbw, bbh, 1, SurfaceFormat.Color, 
                MultiSampleType.None, 0, RenderTargetUsage.PlatformContents);
            mDepthBuffer = new DepthStencilBuffer(dev, bbw, bbh, dev.DepthStencilBuffer.Format);
        }

        /// <summary>
        /// Pushes a matrix of the specified MatrixMode onto the matrix stack
        /// </summary>
        /// <param name="mode">The type of matrix to push onto the corresponding stack</param>
        /// <param name="mtx">The matrix to push onto the stack</param>
        public void PushMatrix(MatrixMode mode, Matrix mtx)
        {
            mMatrices[mode].Push(mtx);
        }

        /// <summary>
        /// Pops a matrix of the specified MatrixMode from the matrix stack
        /// </summary>
        /// <param name="mode">The type of matrix to pop from the corresponding stack</param>
        /// <returns>The matrix popped from the stack</returns>
        public Matrix PopMatrix(MatrixMode mode)
        {
            if (mMatrices[mode].Count == 0)
                throw new InvalidOperationException(string.Format("Attempt to pop a {0} Matrix from a stack of size 0", mode.ToString()));

            return mMatrices[mode].Pop();
        }

        /// <summary>
        /// Gets the topmost matrix from the specified MatrixMode stack
        /// </summary>
        /// <param name="mode">The type of matrix to retrieve</param>
        /// <returns>The topmost matrix on the specified MatrixMode stack</returns>
        public Matrix GetMatrix(MatrixMode mode)
        {
            if (mMatrices[mode].Count == 0)
                throw new InvalidOperationException(string.Format("Attempt to retrieve a {0} Matrix from a stack of size 0", mode.ToString()));

            return mMatrices[mode].Peek();
        }

        /// <summary>
        /// Pushes a VertexDeclaration onto the vertex declaration stack
        /// </summary>
        /// <param name="decl">The vertex declaration to push onto the stack</param>
        public void PushVertexDeclaration(VertexDeclaration decl)
        {
            mVertexDeclaration.Push(decl);
        }

        public VertexDeclaration PopVertexDeclaration()
        {
            if (mVertexDeclaration.Count == 0)
                throw new InvalidOperationException("Attempt to pop a VertexDeclaration from a stack of size 0");

            return mVertexDeclaration.Pop();
        }

        public VertexDeclaration GetVertexDeclaration()
        {
            if (mVertexDeclaration.Count == 0)
                throw new InvalidOperationException("Attempt to retrieve a VertexDeclaration from a stack of size 0");

            return mVertexDeclaration.Peek();
        }

        public void PushVertexBuffer(VertexBuffer vb)
        {
            mVertexBuffer.Push(vb);
        }

        public VertexBuffer PopVertexBuffer()
        {
            if(mVertexBuffer.Count == 0)
                throw new InvalidOperationException("Attempt to pop a VertexBuffer from a stack of size 0");

            return mVertexBuffer.Pop();
        }

        public VertexBuffer GetVertexBuffer()
        {
            if (mVertexBuffer.Count == 0)
                throw new InvalidOperationException("Attempt to retrieve a VertexBuffer from a stack of size 0");

            return mVertexBuffer.Peek();
        }

        public void PushIndexBuffer(IndexBuffer ib)
        {
            mIndexBuffer.Push(ib);
        }

        public IndexBuffer PopIndexBuffer()
        {
            if(mIndexBuffer.Count == 0)
                throw new InvalidOperationException("Attempt to pop an IndexBuffer from a stack of size 0");

            return mIndexBuffer.Pop();
        }

        public IndexBuffer GetIndexBuffer()
        {
            if (mIndexBuffer.Count == 0)
                throw new InvalidOperationException("Attempt to retrieve a VertexBuffer from a stack of size 0");

            return mIndexBuffer.Peek();
        }

        public void PushPickID(uint id)
        {
            mPickID.Push(id);
        }

        public uint PopPickID()
        {
            if (mPickID.Count == 0)
                throw new InvalidOperationException("Attempt to pop a pick id from a stack of size 0");

            return mPickID.Pop();
        }

        public uint GetPickID()
        {
            if (mPickID.Count == 0)
                throw new InvalidOperationException("Attempt to retrieve a pick id from a stack of size 0");

            return mPickID.Peek();
        }

        private Color CalculatePickColor(uint pick_id)
        {
            byte r = (byte)((pick_id >> 24) & 0xFF);
            byte g = (byte)((pick_id >> 16) & 0xFF);
            byte b = (byte)((pick_id >> 8) & 0xFF);
            byte a = (byte)(pick_id & 0xFF);

            Color col = new Color(r, g, b, a);

            return col;
        }

        private uint CalculatePickID(Color color)
        {
            uint id = 0;

            id |= ((uint)color.R << 24);
            id |= ((uint)color.G << 16);
            id |= ((uint)color.B << 8);
            id |= ((uint)color.A);

            return id;
        }

        private void QueueRenderable(PickRenderer handler)
        {
            Matrix view = GetMatrix(MatrixMode.View);
            Matrix proj = GetMatrix(MatrixMode.Projection);
            Matrix world = GetMatrix(MatrixMode.World);
            uint pick_id = GetPickID();

            Color col = CalculatePickColor(pick_id);
            Matrix wvp = world * view * proj;

            mRenderables.Enqueue(new PickRenderable(handler, wvp, col));
        }

        public void QueuePrimitives(PrimitiveType prim_type, int buffer_ofst, int start_vtx, int prim_count)
        {
            VertexDeclaration decl = GetVertexDeclaration();
            VertexBuffer vtx_buf = GetVertexBuffer();

            QueueRenderable(delegate()
            {
                mGraphics.GraphicsDevice.VertexDeclaration = decl;
                mGraphics.GraphicsDevice.Vertices[0].SetSource(vtx_buf, buffer_ofst, decl.GetVertexStrideSize(0));
                mGraphics.GraphicsDevice.DrawPrimitives(prim_type, start_vtx, prim_count);
            });
        }

        public void QueueIndexedPrimitives(PrimitiveType prim_type, int buffer_ofst, int base_vtx, int min_vtx_idx,
            int num_vtx, int start_idx, int prim_count)
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            VertexDeclaration decl = GetVertexDeclaration();
            VertexBuffer vtx_buf = GetVertexBuffer();
            IndexBuffer idx_buf = GetIndexBuffer();

            QueueRenderable(delegate()
            {
                dev.VertexDeclaration = decl;
                dev.Vertices[0].SetSource(vtx_buf, buffer_ofst, decl.GetVertexStrideSize(0));
                dev.Indices = idx_buf;
                dev.DrawIndexedPrimitives(prim_type, base_vtx, min_vtx_idx,
                    num_vtx, start_idx, prim_count);
            });
        }

        public void QueueUserPrimitives<T>(PrimitiveType prim_type, T[] vtx_data, int vtx_ofst, int prim_count) 
            where T : struct
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            VertexDeclaration decl = GetVertexDeclaration();

            QueueRenderable(delegate()
            {
                dev.VertexDeclaration = decl;
                dev.DrawUserPrimitives(prim_type, vtx_data, vtx_ofst, prim_count);
            });
        }

        public void QueueUserIndexedPrimitives<T>(PrimitiveType prim_type, T[] vtx_data, int vtx_ofst,
            int num_vtx, int[] idx_data, int idx_ofst, int prim_count) where T : struct
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            VertexDeclaration decl = GetVertexDeclaration();

            QueueRenderable(delegate()
            {
                dev.VertexDeclaration = decl;
                dev.DrawUserIndexedPrimitives(prim_type, vtx_data, vtx_ofst, num_vtx,
                    idx_data, idx_ofst, prim_count);
            });
        }

        public void QueueUserIndexedPrimitives<T>(PrimitiveType prim_type, T[] vtx_data, int vtx_ofst,
            int num_vtx, short[] idx_data, int idx_ofst, int prim_count) where T : struct
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            VertexDeclaration decl = GetVertexDeclaration();

            QueueRenderable(delegate()
            {
                dev.VertexDeclaration = decl;
                dev.DrawUserIndexedPrimitives(prim_type, vtx_data, vtx_ofst, num_vtx,
                    idx_data, idx_ofst, prim_count);
            });
        }

        public void QueueRenderFunction(OverridePickRenderer renderer)
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            VertexDeclaration decl = GetVertexDeclaration();

            QueueRenderable(delegate()
            {
                dev.VertexDeclaration = decl;
                renderer(mGraphics.GraphicsDevice);
            });
        }

        public void Render()
        {
            GraphicsDevice dev = mGraphics.GraphicsDevice;

            DepthStencilBuffer depth_buf = dev.DepthStencilBuffer;

            dev.SetRenderTarget(0, mBuffer);
            dev.DepthStencilBuffer = mDepthBuffer;
            dev.Clear(Color.TransparentBlack);

            mEffect.Render(mRenderables);

            dev.SetRenderTarget(0, null);
            dev.DepthStencilBuffer = depth_buf;
            mPickTexture = mBuffer.GetTexture();

            mRenderables.Clear();
        }

        public uint Pick(int x, int y)
        {
            if(mPickTexture == null)
                return 0;

            if ((x < 0) || (x >= mPickTexture.Width))
                return 0;

            if ((y < 0) || (y >= mPickTexture.Height))
                return 0;

            Color[] data = new Color[1];
            mPickTexture.GetData(0, new Rectangle(x, y, 1, 1), data, 0, 1);

            uint pick_id = CalculatePickID(data[0]);

            return pick_id;
        }
    }
}