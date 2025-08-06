using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Amib.Threading;
using Microsoft.Xna.Framework.Graphics;


namespace b3d
{
    public class ComponentManager
    {
        public List<RenderableComponent> RenderableComponents = new List<RenderableComponent>();

        public string DataPath = @"Data\";

        public void DrawTexts(GameTime gameTime)
        {
            /*return;
            for (int i = 0; i < InternalComponents.Count; i++) {
                if (InternalComponents[i].Type == ComponentType.Streets)
                {
                    CallesComponent call = ((CallesComponent)InternalComponents[i]);
                    float cameradist = 99999999999999999;
                    float calledist = 0;
                    
                    for (int e = 0; e < call.Calles.Count;e++ ){
                        calledist = Vector3.Distance(Constants.Camera.Position,
                                                             call.Calles[e].BoundingSphere.Center);

                        if (call.Calles[e].Name.Contains("Av ") || call.Calles[e].Name.Contains("Avda")) {
                            if(calledist >160000){
                                continue;
                            }
                        }else{
                            continue;
                            if (calledist >4000) {
                                continue;
                            }
                        }

                        float dist = 99999999999999999;
                        int tramoid = -1;
                        for(int t = 0 ; t < call.Calles[e].Puntos.Count;t++){
                            float curdist = Vector3.Distance(Constants.Camera.Position,
                                                             call.Calles[e].Puntos[t]);
                            if(curdist < dist){
                                dist = curdist;
                                tramoid = t;
                            }
                        }


                        Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                            //new Vector3(call.Calles[e].Tramos[0].Start.X, 0, call.Calles[e].Tramos[tramoid].Start.Z),
                        call.Calles[e].Puntos[tramoid],
                        Constants.Camera.Projection,
                        Constants.Camera.View,
                        Matrix.Identity);
                        if (Constants.Camera.Projection.Forward.Z < center.Z)
                        {
                            continue;
                        }

                        Vector2 vc = Constants.Get2DCoords(call.Calles[e].Puntos[tramoid]);
                        
                        Vector3 ang = AngleTo(call.Calles[e].Puntos[tramoid],call.Calles[e].Puntos[tramoid]);
                        //Vector3 vr = QuaternionToEulerAngleVector3(Baires3d.camera.Orientation);
                        //Vector3 fin = ang - vr;
                       // float perpDot = call.Calles[e].Tramos[tramoid].Start.X * call.Calles[e].Tramos[tramoid].End.Y - call.Calles[e].Tramos[tramoid].Start.Y * call.Calles[e].Tramos[tramoid].End.X;
                        float angle = 0;


                        if (call.Calles[e].Name.Contains("Av ") || call.Calles[e].Name.Contains("Avda")){
                            Constants.Render2D.DrawBox(
                                call.Calles[e].Name,// + "\nDist: " + dist + "\nangle: " + angle.ToString() + "\nQuat: " + vr.ToString(),
                                vc,
                                Color.Black,
                                1f,
                                angle);
                            Constants.Render2D.DrawAvenida(
                                call.Calles[e].Name,// + "\nDist: " + dist + "\nangle: " + angle.ToString() + "\nQuat: " + vr.ToString(),
                                vc,
                                Color.Black,
                                1f,
                                angle, 0.4f);

                            vc.X -= 0.9f;
                            vc.Y -= 0.9f;
                            Constants.Render2D.DrawAvenida(
                                call.Calles[e].Name,// + " | " + dist,
                                vc, 
                                Color.White,
                                1f,
                                angle, 0.5f);

                        }else{
                            Constants.Render2D.DrawCalle(call.Calles[e].Name + " | " + dist, vc, Color.Black, 0.9f);
                            vc.X -= 0.9f;
                            vc.Y -= 0.9f;
                            Constants.Render2D.DrawCalle(call.Calles[e].Name + " | " + dist, vc, Color.White, 0.9f);


                        }
                    }
                   break;
                }
            }
           */
        }

        public void DrawBoundingBoxes()
        {
            for (int i = 0; i < RenderableComponents.Count; i++) {
                if (RenderableComponents[i].Enabled && RenderableComponents[i].Visible && RenderableComponents[i].Type == ComponentType.Mesh)
                {
                    var rc = RenderableComponents[i] as MeshComponent;
                    //Constants.Render3D.RenderBoundingBox(rc.BoundingBox, rc.Color);
                    
                    for (int x = 0; x < rc.MeshParts.Count; x++)
                    {
                        if (rc.MeshParts[x].Visible)
                        {
                            Constants.Render3D.RenderBoundingBox(
                                rc.MeshParts[x].BoundingBox, rc.MeshParts[x].Color
                            );
                        }
                        
                    }
                }
            }
        }

        public void DrawBoundingSpheres()
        {
            for (int i = 0; i < RenderableComponents.Count; i++)
            {
                if (RenderableComponents[i].Enabled && RenderableComponents[i].Visible &&  RenderableComponents[i].Type == ComponentType.Mesh)
                {

                    var rc = RenderableComponents[i] as MeshComponent;

                    for (int p = 0; p < rc.MeshParts.Count; p++)
                    {
                        if (rc.MeshParts[p].Visible)
                        {
                            Constants.Render3D.RenderBoundingSphere(ref rc.MeshParts[p].BoundingSphere, ref rc.MeshParts[p].Color);
                        }
                    }
                }
            }
        }

        public void Draw2d(GameTime gameTime)
        {
            Capital.Draw2d(gameTime);
        }

        internal void Draw(GameTime gameTime)
        {
            Capital.Draw(gameTime);
        }

        internal void Update(GameTime gameTime)
        {
            for (int i = 0; i < RenderableComponents.Count; i++)
            {
                if (RenderableComponents[i].Enabled)
                    RenderableComponents[i].Update(gameTime);
            }
        }

        public ComponentManager()
        {
        }

        public void RemoveComponent(int id)
        {
            for (int i = 0; i < RenderableComponents.Count; i++)
            {
                if (RenderableComponents[i].ID == id)
                {
                    RenderableComponents[i].Dispose();
                    RenderableComponents.RemoveAt(i);
                    GC.Collect();
                    return;
                }
            }
        }

        public void ToggleComponentState(int id, bool state)
        {
            for (int i = 0; i < RenderableComponents.Count; i++)
            {
                if (RenderableComponents[i].ID == id)
                    RenderableComponents[i].Enabled = state;
            }
        }

        private RenderableComponent FindComponent(int id)
        {
            for (int i = 0; i < RenderableComponents.Count; i++)
            {
                if (RenderableComponents[i].ID == id)
                    return RenderableComponents[i];
            }
            return null;
        }

        public RenderableComponent GetComponent(int id)
        {
            return FindComponent(id);
        }

        public void AddComponent(RenderableComponent cmp)
        {
            string category = "";

            cmp.Color = LayerHelper.GetRandomColor();
            switch (cmp.Type) {
                case ComponentType.Mesh:
                    RenderableComponents.Add(cmp);
                    break;
                case ComponentType.Streets:
                    category = "NodeInternal";
                    RenderableComponents.Add(cmp);
                    break;
                case ComponentType.Point:
                    category = "NodePuntos";
                    if (Capital.Puntos == null) {
                        Capital.Puntos = new PuntosCollection();
                    }
                    Capital.Puntos.Add(cmp as PointsComponent);
                    break;

                default:
                    category = "NodeInternal";
                    RenderableComponents.Add(cmp);
                    break;
            }
        }

        public void AddLabels(CallesComponent calleLayer)
        {
            // LabelsComponent lblsc = new LabelsComponent();
            // lblsc.Name = calleLayer.Name + " labels";
            for (int i = 0; i < calleLayer.Calles.Count; i++)
            {
                for (int t = 0; t < calleLayer.Calles[i].Tramos.Count; t++)
                {
                    if (calleLayer.Calles[i].Name == "") continue;

                    //calleLayer.Calles[i].
                    LabelComponent lc = new LabelComponent();
                    lc.Label = calleLayer.Calles[i].Name;

                    int len = calleLayer.Calles[i].Tramos[t].Puntos.Count/2;
                    lc.Position = calleLayer.Calles[i].Tramos[t].Puntos[len];

                    lc.Angle =
                        (float)
                        Math.Atan2(
                            (calleLayer.Calles[i].Tramos[t].Puntos[len].Z -
                             calleLayer.Calles[i].Tramos[t].Puntos[len - 1].Z),
                            (calleLayer.Calles[i].Tramos[t].Puntos[len].X -
                             calleLayer.Calles[i].Tramos[t].Puntos[len - 1].X)) -
                        MathHelper.ToRadians(90);
                }
            }
        }

        public object LoadLayer(object LayerId)
        {
            return null;
        }
        public CapitalFederal Capital;
        public void LoadLayers()
        {
            Capital = new CapitalFederal("Capital Federal", DataPath + @"Ciudades\\Capital Federal.bin");
        }
    }
}