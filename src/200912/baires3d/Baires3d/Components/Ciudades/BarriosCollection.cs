using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class BarriosCollection :  IEnumerable<Barrio> {
        private List<Barrio> pBarrios;
        

        private int getAllVxCount() {
            int cnt = 0;
            for (int i = 0; i < pBarrios.Count; i++) {
                cnt += pBarrios[i].Mesh.Positions.Length;
            }
            return cnt;
        }

        private bool enabled;
        public bool Enabled {
            get { return enabled; }
            set{
                
                enabled = value;
                for(int i = 0; i < pBarrios.Count;i++){
                    pBarrios[i].Enabled = value;
                }
            }
        }

        public IEnumerator<Barrio> GetEnumerator() {
            return (IEnumerator<Barrio>)this.pBarrios.GetEnumerator();
        }

        public void Add(Barrio barrio) {
            if(pBarrios == null){
                pBarrios = new List<Barrio>();
            }
            pBarrios.Add(barrio);
        }
        public int Count {
            get {
                return this.pBarrios.Count;
            }
        }

        public Barrio this[int index] {
            get {
                if ((index >= 0) && (index < this.pBarrios.Count)) {
                    return this.pBarrios[index];
                }
                return null;
            }
        }

        public Barrio this[string name] {
            get {
                List<Barrio>.Enumerator enumerator = this.pBarrios.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        Barrio current = enumerator.Current;
                        if (current.Name == name) {
                            return current;
                        }
                    }
                    while (enumerator.MoveNext());
                }
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator(){
            return GetEnumerator();
        }
        private bool hasNear = false;
        private bool hasMiddle = false;
        private bool hasFar = false;
        private bool hasVisible = false;
        private bool DetermineVisibility(){
            hasVisible = false;
            hasFar = false;
            hasMiddle = false;
            hasNear = false;

            float lodNear = 1000f;
            float lodMiddle = 3000f;

            for (int i = 0; i < pBarrios.Count; i++) {
                if (!pBarrios[i].Enabled || !CollisionHelper.isVisible(pBarrios[i].Mesh.BoundingSphere)) {
                    pBarrios[i].Visible = false;
                    continue;
                }
                hasVisible = true;
                pBarrios[i].Visible = true;

                float Distance = Vector3.Distance(Constants.Camera.Position, pBarrios[i].BoundingSphere.Center);
                pBarrios[i].Distance = Distance;
                if (Distance < lodNear) {
                    hasNear = true;
                    pBarrios[i].LOD = LOD.Near;
                }
                else {
                    if (Distance < lodMiddle) {
                        pBarrios[i].LOD = LOD.Middle;
                        hasMiddle = true;
                    }
                    else {
                        pBarrios[i].LOD = LOD.Far;
                        hasFar = true;
                    }
                }

            }

            if (!hasVisible)
                return false;

            return true;
        }

        public void Draw(GameTime time){
            if(!Enabled){
                return;
            }

            if(!DetermineVisibility()){
                return;
            }

            if (hasNear){
                
                
                for (int i = 0; i < pBarrios.Count; i++){
                    if (!pBarrios[i].Visible || pBarrios[i].LOD != LOD.Near)
                        continue;

                    
                    pBarrios[i].Draw(time);
                }
            }

            if (hasMiddle) {
                for (int i = 0; i < pBarrios.Count; i++) {
                    if (!pBarrios[i].Visible || pBarrios[i].LOD != LOD.Middle)
                        continue;
                    Constants.GraphicsDevice.VertexDeclaration = pBarrios[i].Manzanas.Mesh.vertexDeclaration;
                    pBarrios[i].Draw(time);
                }
            }

            if (hasFar) {
                Constants.GraphicsDevice.VertexDeclaration = pBarrios[0].Mesh.vertexDeclaration;
                for (int i = 0; i < pBarrios.Count; i++) {
                    if (!pBarrios[i].Visible || pBarrios[i].LOD != LOD.Far)
                        continue;

                    float distt = Vector3.Distance(Constants.Camera.Position, pBarrios[i].BoundingSphere.Center) ;
                    float alpha = 0.5f + Math.Abs(distt / 20000 - 0.5f);

                    Constants.Render3D.BasicShader.Alpha = alpha;
                    Constants.Logger.AddLog("alpha: " + alpha);
                    Constants.Render3D.BasicShader.effect.CommitChanges();
                    pBarrios[i].Draw(time);
                }
            }

                Constants.Render3D.BasicShader.Alpha = 1;
                Constants.Render3D.BasicShader.effect.CommitChanges();
        }
        public void Draw2d(GameTime time){
            float dist = 0;
            for (int i = 0; i < pBarrios.Count; i++) {
                
                if (pBarrios[i].Visible){
                    if(pBarrios[i].Distance < 50000){
                        pBarrios[i].Draw2d(time);

                    }
                }
            }
        }
    }
}