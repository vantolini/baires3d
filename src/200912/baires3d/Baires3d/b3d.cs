using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys=Microsoft.Xna.Framework.Input.Keys;
using zCamera;
using zCamera.Cameras;

namespace b3d
{
    public class b3d : Game
    {
        public GraphicsDeviceManager graphics;
        public bool canMove = true;
        public MouseState currentMouseState;
        private TimeSpan elapsedTime = TimeSpan.Zero;


        public Main form;
        private int frameCounter;
        public int frameRate;
        public bool isLoading = true;
        private KeyboardState keyboard;
        public MouseState lastMouseState;

        public ComponentManager ComponentManager;

        private float camera_speed_run = 0.4f;
        private float camera_velocity_run = 0f;
        private float statsTimer;
        private int lastScrollWheelValue = 0;

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isHoldingLeft = (e.Button == MouseButtons.Left);
        }

        private void Panel2_MouseLeave(object sender, EventArgs e)
        {
            canMove = false;
            isHoldingLeft = false;
        }

        private void Panel2_MouseEnter(object sender, EventArgs e)
        {
            canMove = true;
        }

        public b3d(Main form){
            Content.RootDirectory = "Content";
            this.form = form;
            form.splitContainer2.Panel1.MouseEnter += Panel2_MouseEnter;
            form.splitContainer2.Panel1.MouseLeave += Panel2_MouseLeave;
            form.splitContainer2.Panel1.MouseDown += new MouseEventHandler(Panel1_MouseDown);
            form.splitContainer2.Panel1.MouseUp += new MouseEventHandler(Panel1_MouseUp);
            form.splitContainer2.Panel1.MouseClick += new MouseEventHandler(Panel1_MouseClick);
            var xnaWindow = (Form) Control.FromHandle((Window.Handle));
            xnaWindow.GotFocus += xnaWindow_GotFocus;
            form.Panel.Resize += Panel_Resize;
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;

            LayerCreator lc = new LayerCreator();
            lc.BuildLayers();
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isHoldingLeft = false;
        }


        protected override void Initialize()
        {
           
            this.IsFixedTimeStep = false;
            graphics.GraphicsDevice.RenderState.MultiSampleAntiAlias = true;
            graphics.PreferMultiSampling = true;
            graphics.GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.TwoSamples;
            graphics.GraphicsDevice.PresentationParameters.MultiSampleQuality =2;
            graphics.GraphicsDevice.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionColor.VertexElements);
            
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            Constants.GraphicsDevice = graphics.GraphicsDevice;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Constants.Window = Window;
            Constants.TextureManager = new TextureManager();
            Constants.TextureManager.LoadTextures();
            Constants.Render2D = new Render2D();
            Constants.Render3D = new Render3D();

            Constants.Logger = new Logger2d();
            Mouse.WindowHandle = form.splitContainer2.Panel1.Handle;
            Constants.LoadSettings();
            Constants.Camera = new CameraManager();

            base.LoadContent();
            ComponentManager = new ComponentManager();
            ComponentManager.LoadLayers();


            Constants.LineManager = new RoundLineManager();
            Constants.LineManager.Init(this.GraphicsDevice, this.Content);
            //ShortestPath = new ShortestPath();
            //ShortestPath.Init(@"..\Work\shp\Mesh\Calles\Capital.shp");
            //camera.ViewDirection = opciones.CameraViewDirection;
            //pp = new PostProcess();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            light0 = new Light(new Vector4(-286, 228, 144, 1));
            light0.Initialize(GraphicsDevice, Content);

            InitGUI();
            form.Show();
            //camera.Position = LayerMgr.Layers[0].Features[0].Vertices[0].Position;

            //form.TopMost = true;
            isLoading = false;

        }

        public void InitGUI()
        {
            form.chkPerPixelLighting.Checked = Constants.Settings.Graphics.Light.PerPixel;
            form.chkLuces.Checked = Constants.Settings.Graphics.Light.Enabled;
            form.chkWireframe.Checked = Constants.Settings.Graphics.Wireframe;
            form.chkFog.Checked = Constants.Settings.Graphics.Fog.Enabled;


            //olvColumn2.CheckBoxes = true;
            if (Constants.ar3d.ComponentManager.Capital.StreetManager != null)
            {
                form.cmbCalleInicial.Items.AddRange(Constants.ar3d.ComponentManager.Capital.StreetManager.CalleNames.ToArray());
                form.cmbCalleDestino.Items.AddRange(Constants.ar3d.ComponentManager.Capital.StreetManager.CalleNames.ToArray());
                form.cmbEsquinaInicial.Items.AddRange(Constants.ar3d.ComponentManager.Capital.StreetManager.CalleNames.ToArray());
                form.cmbEsquinaDestino.Items.AddRange(Constants.ar3d.ComponentManager.Capital.StreetManager.CalleNames.ToArray());
                form.cmbCalleInicial.SelectedIndex = 20;
                form.cmbCalleDestino.SelectedIndex = 200;
            }
   


            form.cmbCull.SelectedIndex = 0;

            form.tvCapas.Nodes["NodeCalles"].Checked = ComponentManager.Capital.Calles.Enabled;
            form.tvCapas.Nodes["NodePuntos"].Checked = ComponentManager.Capital.Puntos.Enabled;
            form.tvCapas.Nodes["NodeBarrios"].Checked = ComponentManager.Capital.Barrios.Enabled;


            for(int i = 0 ; i < ComponentManager.Capital.Barrios.Count;i++){
                TreeNode node = new TreeNode(ComponentManager.Capital.Barrios[i].Name);
                node.Tag = i;
                node.Checked = true;
                node.EnsureVisible();
                form.tvCapas.Nodes["NodeBarrios"].Nodes.Add(node);
            }


            for (int i = 0; i < ComponentManager.Capital.Puntos.Count; i++) {
                TreeNode parentNode = new TreeNode(ComponentManager.Capital.Puntos[i].Name);
                parentNode.Tag = i;
                parentNode.Checked = true;
                parentNode.EnsureVisible();
                form.tvCapas.Nodes["NodePuntos"].Nodes.Add(parentNode);


                for (int p = 0; p < ComponentManager.Capital.Puntos[i].Points.Count; p++) {

                    TreeNode node = new TreeNode(ComponentManager.Capital.Puntos[i].Points[p].Name);
                    node.Tag = p;
                    node.Checked = true;
                    node.EnsureVisible();
                    parentNode.Nodes.Add(node);
                }
            }

            for (int i = 0; i < ComponentManager.Capital.Calles.Count; i++) {
                TreeNode node = new TreeNode(ComponentManager.Capital.Calles[i].Name);
                node.Tag = i;
                node.Checked = true;
                node.EnsureVisible();
                form.tvCapas.Nodes["NodeCalles"].Nodes.Add(node);
            }

            string[] colectivos = {
                "1", "2", "4", "5", "6", "7", "9", "10", "12", "15", "17", "19", "20", "21",
                "22", "23", "24", "25", "26", "28", "29", "32", "33", "34", "36", "37", "39",
                "41", "42", "44", "45", "46", "47", "49", "50", "51", "52", "53", "54", "55",
                "56", "57", "59", "60", "61", "62", "63", "64", "65", "67", "68", "70", "71",
                "74", "75", "76", "78", "79", "80", "84", "85", "86", "87", "88", "91", "92",
                "93", "95", "96", "97", "98", "99", "100", "101", "102", "103", "104", "105",
                "106", "107", "108", "109", "110", "111", "112", "113", "114", "115", "117",
                "118", "123", "124", "126", "127", "128", "129", "130", "132", "133", "134",
                "135", "136", "140", "141", "142", "143", "146", "148", "150", "151", "152",
                "153", "154", "158", "159", "160", "161", "162", "163", "165", "166", "168",
                "169", "170", "172", "174", "175", "176", "177", "178", "179", "180", "181",
                "182", "184", "185", "186", "188", "190", "193", "204", "430"
            };

            form.cmbColectivos.Items.AddRange(colectivos);
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = form.Panel.Handle;
        }

        private void xnaWindow_GotFocus(object sender, EventArgs e)
        {
            ((Form) sender).Visible = false;
            form.TopMost = false;
        }


        private void Panel_Resize(object sender, EventArgs e)
        {
            if (form.Panel.Height > 0 && form.Panel.Width > 0)
            {
                graphics.PreferredBackBufferWidth = form.Panel.Width;
                graphics.PreferredBackBufferHeight = form.Panel.Height;
                //aspectRatio = (float) form.Panel.Width/form.Panel.Height;
                graphics.ApplyChanges();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Constants.Settings.Components.Camera.Position = Constants.Camera.Position;
                Constants.SaveSettings();
                Dispose(true);
                Exit();
                return;
            }
            light0.Position = new Vector4(-286, 228, ((float)Math.Sin(gameTime.TotalGameTime.TotalSeconds / 10) * 200), 0);

            int newScrollWheelValue = Mouse.GetState().ScrollWheelValue;
            if (lastScrollWheelValue != newScrollWheelValue)
            {
                lastScrollWheelValue = newScrollWheelValue;
            }

            statsTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (statsTimer >= 500.0f)
            {
                currentfpsline = "fps: " + frameRate + ", " +
                                 string.Format("mem: {0}kb", (GC.GetTotalMemory(false)/1024));
                statsTimer -= 500.0f;
            }


            currentMouseState = Mouse.GetState();

            camera_speed_run = 44f;

            camera_velocity_run = Constants.Camera.Position.Y;
            if (camera_velocity_run < 0.000001)
            {
                camera_velocity_run = 1;
            }
            camera_speed_run = camera_velocity_run;

            Constants.Camera.Velocity = new Vector3(
                camera_velocity_run,
                camera_velocity_run ,
                camera_velocity_run 
                );
            Constants.Camera.Acceleration = new Vector3(
                camera_velocity_run / 2,
                camera_velocity_run / 2,
                camera_velocity_run / 2
                );
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                camera_velocity_run = camera_velocity_run * 2;
                Constants.Camera.Velocity = new Vector3(
                camera_velocity_run,
                camera_velocity_run,
                camera_velocity_run
                    );
                Constants.Camera.Acceleration = new Vector3(
                camera_velocity_run * 2,
                camera_velocity_run * 2,
                camera_velocity_run * 2
                    );
            }
            else
            {
                Constants.Camera.Velocity = new Vector3(
                    camera_velocity_run,
                    camera_velocity_run,
                    camera_velocity_run
                    );
                Constants.Camera.Acceleration = new Vector3(
                    camera_velocity_run/4,
                    camera_velocity_run/4,
                    camera_velocity_run/4
                    );
            }

            lastMouseState = currentMouseState;


            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
            ComponentManager.Update(gameTime);
            Constants.Camera.Update(gameTime);

            if(Constants.Camera.Position.Y < 0.00006f){
                Constants.Camera.Position = new Vector3(
                    Constants.Camera.Position.X,
                    0.00006f,
                    Constants.Camera.Position.Z
                    
                    );
            }
            base.Update(gameTime);
        }

        private string currentfpsline;

        public CullMode cullMode = CullMode.CullClockwiseFace;
        public FillMode fillMode = FillMode.Solid;


        private SpriteBatch sprite;

        private DepthStencilBuffer rTargDepth;
        private DepthStencilBuffer bloomDepth;
        private Effect bloomatic;
        private RenderTarget2D rTarg;
        private RenderTarget2D bloomTarg;

        private bool Additive;
        private bool HiRange;

        private DepthStencilBuffer oldBuffer;
        public int windowWidth = 0;
        public int windowHeight = 0;
        public int aRatio = 0;
        private bool bloom = false;
        private float alphaFrame;
        public float cameraNear = 0;
        public float cameraFar = 0;
        private float oldCameraHeight = 0;

        private Light light0;

        protected override void Draw(GameTime gameTime)
        {
            bloom = false;
            Constants.Camera.Frustum.Matrix = Constants.Camera.View * Constants.Camera.Projection;


            if (windowWidth != Constants.GraphicsDevice.DisplayMode.Width ||
                windowHeight != Constants.GraphicsDevice.DisplayMode.Height)
            {
                windowHeight = Constants.GraphicsDevice.DisplayMode.Height;
                windowWidth = Constants.GraphicsDevice.DisplayMode.Width;
                aRatio = windowWidth/windowHeight;
            }
            float cameraPos = Constants.Camera.Position.Y;
            float far = cameraPos;

            cameraNear = 1;
            cameraFar =  600000;
            //cameraNear =float.Parse(form.nmrNear.Value.ToString());
            //cameraFar = float.Parse(form.nmrFar.Value.ToString());

            Constants.Camera.camera.Perspective(90.0f, aRatio, cameraNear, cameraFar);
            //Constants.Logger.AddLog("selected street: " + Constants.ar3d.ComponentManager.Capital.StreetManager.StreetToDraw);
            Constants.Logger.AddLog("camera: " + Constants.Camera.Position.ToString());
            Constants.Logger.AddLog("camera near: " + cameraNear);
            Constants.Logger.AddLog("camera far: " + cameraFar);

            Constants.Logger.AddLog(currentfpsline);
            //Constants.Logger.AddLog("Holding left: " + isHoldingLeft);
            //Constants.Logger.AddLog("far: " + far);

            Constants.Logger.AddLog("Picked: " + Constants.Camera.Picked);
            Constants.Logger.AddLog("Picking: " + Constants.Camera.Picking);

            GraphicsDevice.RenderState.CullMode = cullMode;

            //Constants.ar3d.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            bool custom = false;
            bool blur2 = false;
            if(custom){
                Constants.Render3D.StartPostprocess();
                Constants.Render3D.StartDraw();
                ComponentManager.Draw(gameTime);
                Constants.Render3D.EndDraw();
                Constants.Render3D.EndPostprocess();
            }else{
                Constants.ar3d.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                ComponentManager.Capital.Calles.Draw(gameTime);
                Constants.Render3D.Begin();
                ComponentManager.Draw(gameTime);
                Constants.Render3D.End();
            }


            Constants.Render2D.Begin();


            ComponentManager.DrawTexts(gameTime);
            ComponentManager.Draw2d(gameTime);
            Constants.Logger.DrawLog();
            Constants.Render2D.End();


           GraphicsDevice.RenderState.DepthBufferEnable = true;
            base.Draw(gameTime);
            frameCounter++;
        }

        public Rectangle rect = new Rectangle();

        [STAThread]
        private static void Main()
        {
            var form = new Main();
            form.Disposed += form_Disposed;
            Constants.ar3d = new b3d(form);
            Constants.ar3d.Run();
        }

        public bool isHoldingLeft = false;

        private static void Panel2_MouseWheel(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (!Constants.ar3d.form.splitContainer2.Panel1.Focused) return;
            //game.log("mouse delta: " + e.Delta.ToString());

            if (e.Delta > 0)
            {
                //game.camera.Move(0, (e.Delta ), 0);
            }
            else
            {
                //game.camera.Move(0, (e.Delta ), 0);
            }
        }

        private static void Panel1_MouseLeave(object sender, EventArgs e)
        {
            //game.form.splitContainer2.Panel1.Focus();
            Constants.ar3d.canMove = true;
            //game.log("Panel2_MouseLeave");
        }

        private static void Panel1_MouseEnter(object sender, EventArgs e)
        {
            Constants.ar3d.canMove = false;
            //game.form.splitContainer2.Panel1.Focus();
            //game.log("Panel2_MouseEnter");
        }

        private static void Panel2_GotFocus(object sender, EventArgs e)
        {
            Constants.ar3d.canMove = true;
        }

        private static void Panel2_LostFocus(object sender, EventArgs e)
        {
            Constants.ar3d.canMove = false;
        }

        private static void form_Disposed(object sender, EventArgs e)
        {
            Constants.ar3d.Exit();
        }
    }
}