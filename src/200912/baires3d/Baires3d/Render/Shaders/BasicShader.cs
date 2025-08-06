using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class BasicShader {

        public Effect effect;
        public void Init(){
            effect = Constants.ar3d.Content.Load<Effect>("BasicEffect");
            this.CacheEffectParams(Constants.ar3d.GraphicsDevice);
            this.InitializeEffectParams();

        }

        private void CacheEffectParams(GraphicsDevice device)
        {
            ShaderProfile maxPixelShaderProfile = device.GraphicsDeviceCapabilities.MaxPixelShaderProfile;
            this.hasPS20 = (((maxPixelShaderProfile == ShaderProfile.PS_2_0) || (maxPixelShaderProfile == ShaderProfile.PS_2_A)) || ((maxPixelShaderProfile == ShaderProfile.PS_2_B) || (maxPixelShaderProfile == ShaderProfile.PS_3_0))) || (maxPixelShaderProfile == ShaderProfile.XPS_3_0);
            this.basicShaderTechnique = effect.Techniques["BasicEffect"];
            this.basicTextureParam = effect.Parameters["BasicTexture"];
            this.fogEnabledParam = effect.Parameters["FogEnabled"];
            this.fogStartParam = effect.Parameters["FogStart"];
            this.fogEndParam = effect.Parameters["FogEnd"];
            this.fogColorParam = effect.Parameters["FogColor"];
            this.worldParam = effect.Parameters["World"];
            this.viewParam = effect.Parameters["View"];
            this.projectionParam = effect.Parameters["Projection"];
            this.diffuseColorParam = effect.Parameters["DiffuseColor"];
            this.specularColorParam = effect.Parameters["SpecularColor"];
            this.emissiveColorParam = effect.Parameters["EmissiveColor"];
            this.specularPowerParam = effect.Parameters["SpecularPower"];
            this.alphaParam = effect.Parameters["Alpha"];
            this.ambientLightColorParam = effect.Parameters["AmbientLightColor"];
            this.eyePositionParam = effect.Parameters["EyePosition"];
            this.shaderIndexParam = effect.Parameters["ShaderIndex"];
            this.light0 = new BasicDirectionalLight(effect.Parameters["DirLight0Direction"], effect.Parameters["DirLight0DiffuseColor"], effect.Parameters["DirLight0SpecularColor"]);
            this.light1 = new BasicDirectionalLight(effect.Parameters["DirLight1Direction"], effect.Parameters["DirLight1DiffuseColor"], effect.Parameters["DirLight1SpecularColor"]);
            this.light2 = new BasicDirectionalLight(effect.Parameters["DirLight2Direction"], effect.Parameters["DirLight2DiffuseColor"], effect.Parameters["DirLight2SpecularColor"]);
        }


        public void Begin() {
            
            effect.CurrentTechnique = effect.Techniques["BasicEffect"];
            
            ApplySettings();
            effect.CommitChanges();
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

        }
        public void End() {
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }



        #region props
        // Properties
        public float Alpha {
            get {
                return this.alphaParam.GetValueSingle();
            }
            set {
                this.alphaParam.SetValue(value);
            }
        }

        public Vector3 AmbientLightColor {
            get {
                return this.ambientLightColorParam.GetValueVector3();
            }
            set {
                this.ambientLightColorParam.SetValue(value);
            }
        }

        public Vector3 DiffuseColor {
            get {
                return this.diffuseColorParam.GetValueVector3();
            }
            set {
                this.diffuseColorParam.SetValue(value);
            }
        }

        public BasicDirectionalLight DirectionalLight0 {
            get {
                return this.light0;
            }
        }

        public BasicDirectionalLight DirectionalLight1 {
            get {
                return this.light1;
            }
        }

        public BasicDirectionalLight DirectionalLight2 {
            get {
                return this.light2;
            }
        }

        public Vector3 EmissiveColor {
            get {
                return this.emissiveColorParam.GetValueVector3();
            }
            set {
                this.emissiveColorParam.SetValue(value);
            }
        }

        public Vector3 FogColor {
            get {
                return this.fogColorParam.GetValueVector3();
            }
            set {
                this.fogColorParam.SetValue(value);
            }
        }

        public bool FogEnabled {
            get {
                return this.fogEnabledParam.GetValueBoolean();
            }
            set {
                this.fogEnabledParam.SetValue(value);
            }
        }

        public float FogEnd {
            get {
                return this.fogEndParam.GetValueSingle();
            }
            set {
                this.fogEndParam.SetValue(value);
            }
        }

        public float FogStart {
            get {
                return this.fogStartParam.GetValueSingle();
            }
            set {
                this.fogStartParam.SetValue(value);
            }
        }

        public bool LightingEnabled {
            get {
                return this.lightingEnabled;
            }
            set {
                this.lightingEnabled = value;
                this.UpdateShaderIndex();
            }
        }

        public bool PreferPerPixelLighting {
            get {
                return this.preferPerPixelLighting;
            }
            set {
                this.preferPerPixelLighting = value;
                this.UpdateShaderIndex();
            }
        }

        public Matrix Projection {
            get {
                return this.projectionParam.GetValueMatrix();
            }
            set {
                this.projectionParam.SetValue(value);
            }
        }

        public Vector3 SpecularColor {
            get {
                return this.specularColorParam.GetValueVector3();
            }
            set {
                this.specularColorParam.SetValue(value);
            }
        }

        public float SpecularPower {
            get {
                return this.specularPowerParam.GetValueSingle();
            }
            set {
                this.specularPowerParam.SetValue(value);
            }
        }

        public Texture2D Texture {
            get {
                return this.basicTextureParam.GetValueTexture2D();
            }
            set {
                this.basicTextureParam.SetValue(value);
            }
        }

        public bool TextureEnabled {
            get {
                return this.textureEnabled;
            }
            set {
                this.textureEnabled = value;
                this.UpdateShaderIndex();
            }
        }

        public bool VertexColorEnabled {
            get {
                return this.vertexColorEnabled;
            }
            set {
                this.vertexColorEnabled = value;
                this.UpdateShaderIndex();
            }
        }

        public Matrix View {
            get {
                return this.viewParam.GetValueMatrix();
            }
            set {
                this.viewParam.SetValue(value);
                Matrix matrix = Matrix.Invert(value);
                this.eyePositionParam.SetValue(matrix.Translation);
            }
        }

        public Matrix World {
            get {
                return this.worldParam.GetValueMatrix();
            }
            set {
                this.worldParam.SetValue(value);
            }
        }
        #endregion



        private void InitializeEffectParams() {
            this.World = Matrix.Identity;
            this.View = Matrix.Identity;
            this.Projection = Matrix.Identity;
            this.FogEnabled = false;
            this.FogStart = 0f;
            this.FogEnd = 1f;
            this.FogColor = Vector3.Zero;
            this.DiffuseColor = Vector3.One;
            this.SpecularColor = Vector3.One;
            this.EmissiveColor = Vector3.Zero;
            this.Alpha = 1f;
            this.SpecularPower = 16f;
            this.preferPerPixelLighting = false;
            this.lightingEnabled = false;
            this.textureEnabled = false;
            this.vertexColorEnabled = false;
            this.UpdateShaderIndex();
            this.light0.Enabled = false;
            this.light0.Direction = Vector3.Up;
            this.light0.DiffuseColor = Vector3.Zero;
            this.light0.SpecularColor = Vector3.Zero;
            this.light1.Enabled = false;
            this.light1.Direction = Vector3.Up;
            this.light1.DiffuseColor = Vector3.Zero;
            this.light1.SpecularColor = Vector3.Zero;
            this.light2.Enabled = false;
            this.light2.Direction = Vector3.Up;
            this.light2.DiffuseColor = Vector3.Zero;
            this.light2.SpecularColor = Vector3.Zero;
            CurrentTechnique = this.basicShaderTechnique;
        }

        private EffectTechnique CurrentTechnique;

        // Fields
        private EffectParameter alphaParam;
        private EffectParameter ambientLightColorParam;
        private EffectTechnique basicShaderTechnique;
        private EffectParameter basicTextureParam;
        private EffectParameter diffuseColorParam;
        private EffectParameter emissiveColorParam;
        private EffectParameter eyePositionParam;
        private EffectParameter fogColorParam;
        private EffectParameter fogEnabledParam;
        private EffectParameter fogEndParam;
        private EffectParameter fogStartParam;
        private bool hasPS20;
        private BasicDirectionalLight light0;
        private BasicDirectionalLight light1;
        private BasicDirectionalLight light2;
        private bool lightingEnabled;
        private bool preferPerPixelLighting;
        private EffectParameter projectionParam;
        private EffectParameter shaderIndexParam;
        private EffectParameter specularColorParam;
        private EffectParameter specularPowerParam;
        private bool textureEnabled;
        private bool vertexColorEnabled;
        private EffectParameter viewParam;
        private EffectParameter worldParam;


        private void ApplySettings(){
            World = Matrix.Identity;
            View = Constants.Camera.View;
            Projection = Constants.Camera.Projection;
            if (Constants.Settings.Graphics.Wireframe &&
                Constants.ar3d.GraphicsDevice.RenderState.FillMode != FillMode.WireFrame){
                Constants.ar3d.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            }
            /*Constants.ar3d.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Constants.ar3d.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
            Constants.ar3d.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
            Constants.ar3d.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
            Constants.ar3d.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
            Constants.ar3d.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results
            */
            if (!Constants.Settings.Graphics.Wireframe &&
                Constants.ar3d.GraphicsDevice.RenderState.FillMode != FillMode.Solid){
                Constants.ar3d.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            }
            FogEnabled = Constants.Settings.Graphics.Fog.Enabled;
            FogStart = Constants.Settings.Graphics.Fog.Near;
            FogEnd = Constants.Settings.Graphics.Fog.Far;
            FogColor = Constants.Settings.Graphics.Fog.Color.ToVector3();


            if (Constants.Settings.Graphics.Light.Enabled) {
                if(LightingEnabled == false){
                    EnableDefaultLighting();
                }
                if (TextureEnabled) {
                    VertexColorEnabled = false;
                }
                else {
                    VertexColorEnabled = true;
                }
            }else{
                if (LightingEnabled == true){
                    LightingEnabled = false;
                }
                if (TextureEnabled) {
                    VertexColorEnabled = false;
                }
                else {
                    VertexColorEnabled = true;
                }
            }

            //effect.CommitChanges();
            // effect.Parameters["VertexColorEnabled"].SetValue(true);
        }


        public void EnableDefaultLighting() {
            this.LightingEnabled = true;
            this.AmbientLightColor = new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
            Vector3 vector = new Vector3(1f, 0.9607844f, 0.8078432f);
            this.DirectionalLight0.DiffuseColor = vector;
            this.DirectionalLight0.Direction = new Vector3(-0.5265408f, -0.5735765f, -0.6275069f);
            this.DirectionalLight0.SpecularColor = vector;
            this.DirectionalLight0.Enabled = true;
            this.DirectionalLight1.DiffuseColor = new Vector3(0.9647059f, 0.7607844f, 0.4078432f);
            this.DirectionalLight1.Direction = new Vector3(0.7198464f, 0.3420201f, 0.6040227f);
            this.DirectionalLight1.SpecularColor = Vector3.Zero;
            this.DirectionalLight1.Enabled = true;
            vector = new Vector3(0.3231373f, 0.3607844f, 0.3937255f);
            this.DirectionalLight2.DiffuseColor = vector;
            this.DirectionalLight2.Direction = new Vector3(0.4545195f, -0.7660444f, 0.4545195f);
            this.DirectionalLight2.SpecularColor = vector;
            this.DirectionalLight2.Enabled = true;
        }



        private void UpdateShaderIndex() {
            int num = ((this.vertexColorEnabled ? 1 : 0) | (this.textureEnabled ? 2 : 0)) | (this.lightingEnabled ? 4 : 0);
            num += ((this.lightingEnabled && this.preferPerPixelLighting) && this.hasPS20) ? 4 : 0;
            this.shaderIndexParam.SetValue(num);
        }




        internal void CommitChanges() {
            ApplySettings();
        }
    }
}
