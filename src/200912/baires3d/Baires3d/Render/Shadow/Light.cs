/*
 * Created by Javier Cantón Ferrero.
 * MVP Windows-DirectX 2007/2008
 * DotnetClub Sevilla
 * Date 17/02/2007
 * Web www.codeplex.com/XNACommunity
 * Email javiuniversidad@gmail.com
 * blog: mirageproject.blogspot.com
 */


using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace b3d
{
    public class Light
    {
        private Vector4 position;
        private Vector3 lookAt;
        private Vector4 ambientColor;
        private Vector4 diffuseColor;
        private Vector4 specularColor;
        private float lightPower;
        private GraphicsDevice device;

        public Shadow shadow;

        #region Properties
        public float LightPower
        {
            get { return lightPower; }
            set { lightPower = value; }
        }

        public Matrix LightViewProject
        {
            get { return LightView * LightProject; }
        }

        public Matrix LightView
        {
            get { return Matrix.CreateLookAt(Position3, new Vector3(lookAt.X, lookAt.Y, lookAt.Z), new Vector3(0, 1, 0)); }
        }

        public Matrix LightProject
        {
            get { return Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 3, 1.0f, 1f, 10000f); }
        }

        public Vector3 LookAt
        {
            get { return lookAt; }
            set { lookAt = value; }
        }

        public Vector4 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public Vector4 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Vector4 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; }
        }

        public Vector4 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position.X, position.Y, position.Z); }
        }

        public Effect ShadowMap
        {
            get { return this.shadow.shadowMap; }
        }
        #endregion

        public Light(Vector4 position)
        {
            this.position = position;
            this.lookAt = Vector3.Zero;
            this.ambientColor = new Vector4(0.1843f, 0.3098f, 0.3098f, 1);
            this.diffuseColor = new Vector4(0.3921f, 0.5843f, 0.9294f, 1);
            this.specularColor = new Vector4(1, 1, 1, 1);
            this.lightPower = 2f;
        }

        public void Initialize(GraphicsDevice device, ContentManager content)
        {
            this.device = device;

            shadow = new Shadow();
            shadow.Initialize(device, content, this);
        }
    }
}
