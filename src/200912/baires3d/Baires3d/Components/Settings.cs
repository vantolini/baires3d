using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace b3d
{
    [Serializable]
    public class SettingsCamera
    {
        public float Speed;
        public float Velocity;
        public Vector3 Position;

        public SettingsCamera()
        {
            Position = new Vector3(0, 0, 0);
            Velocity = 30;
            Speed = 30;
        }
    }

    [Serializable]
    public class SettingsFog
    {
        public bool Enabled;
        public float Near;
        public float Far;
        public Color Color;

        public SettingsFog()
        {
            Enabled = false;
            Color = Color.Gray;
            Far = 1000;
            Near = 700;
        }
    }

    [Serializable]
    public class SettingsLight
    {
        public bool Enabled;
        public bool PerPixel;

        public float SpecularPower;
        public Color SpecularColor;
        public Color AmbientColor;
        public Color DiffuseColor;

        public SettingsLight()
        {
            Enabled = true;

            AmbientColor = Color.LightSalmon;
            DiffuseColor = Color.LightBlue;
            SpecularColor = Color.LightGreen;

            SpecularPower = 14;
        }
    }

    [Serializable]
    public class SettingsLayer
    {
        public bool Enabled;
        public string FilePath;
        public string FileName;
        public string Nombre;
        public bool Internal = false;
        public ComponentType Type;

        public SettingsLayer()
        {
        }

        public SettingsLayer(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);
            Enabled = true;
        }

        public SettingsLayer(string filePath, bool enabled)
        {
            FilePath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);
            Enabled = enabled;
        }

        public SettingsLayer(string filePath, bool enabled, bool intern)
        {
            if (intern)
            {
                Nombre = filePath;
                Enabled = enabled;
                Internal = intern;
            }
            else
            {
                FilePath = filePath;
                FileName = Path.GetFileNameWithoutExtension(filePath);
                Nombre = FileName;
                Enabled = enabled;
                Internal = intern;
            }
        }

        public SettingsLayer(string filePath, bool enabled, bool intern, ComponentType componentType)
        {
            FilePath = filePath;
            FileName = Path.GetFileNameWithoutExtension(filePath);
            Nombre = FileName;
            Enabled = enabled;
            Internal = intern;
            Type = componentType;
        }
    }

    [Serializable]
    public class SettingsTerrain
    {
        public bool Enabled;

        public SettingsTerrain()
        {
            Enabled = false;
        }
    }

    [Serializable]
    public class SettingsSkybox
    {
        public bool Enabled;
        public float Speed;

        public SettingsSkybox()
        {
            Enabled = false;
            Speed = 20f;
        }
    }

    [Serializable]
    public class SettingsGraphics
    {
        public SettingsFog Fog;
        public SettingsLight Light;
        public bool Wireframe;

        public SettingsGraphics()
        {
            Wireframe = false;
            Fog = new SettingsFog();
            Light = new SettingsLight();
        }
    }

    [Serializable]
    public class SettingsComponents
    {
        public SettingsCamera Camera;
        public SettingsTerrain Terrain;
        public SettingsSkybox Skybox;

        public SettingsComponents()
        {
            Camera = new SettingsCamera();
            Terrain = new SettingsTerrain();
            Skybox = new SettingsSkybox();
        }
    }

    [Serializable]
    public class SettingsGeneral
    {
        public bool DrawBoundingBoxes;
        public bool DrawBoundingSpheres;
    }

    [Serializable]
    public class Settings
    {
        [NonSerialized] private b3d Baires3d;

        [NonSerialized] public bool Changed = false;

        public SettingsGeneral General;
        public SettingsGraphics Graphics;
        public SettingsComponents Components;

        public List<SettingsLayer> Layers = new List<SettingsLayer>();

        public Settings()
        {
            Components = new SettingsComponents();
            Graphics = new SettingsGraphics();
            General = new SettingsGeneral();
        }
    }
}