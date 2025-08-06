using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class CallesCollection : IEnumerable<CalleComponent> {
        private List<CalleComponent> pCalles;
        public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;
        private bool BigBufferBuilt = false;
        private bool enabled;
        public bool Enabled {
            get { return enabled; }
            set {

                enabled = value;
                for (int i = 0; i < pCalles.Count; i++) {
                    pCalles[i].Enabled = value;
                }
            }
        }
        
        public IEnumerator<CalleComponent> GetEnumerator()
        {
            return (IEnumerator<CalleComponent>)this.pCalles.GetEnumerator();
        }

        public void Add(CalleComponent barrio) {
            if (pCalles == null) {
                pCalles = new List<CalleComponent>();
            }
            pCalles.Add(barrio);
        }

        public int Count {
            get {
                return this.pCalles.Count;
            }
        }

        public CalleComponent this[int index] {
            get {
                if ((index >= 0) && (index < this.pCalles.Count)) {
                    return this.pCalles[index];
                }
                return null;
            }
        }

        public CalleComponent this[string name] {
            get {
                List<CalleComponent>.Enumerator enumerator = this.pCalles.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        CalleComponent current = enumerator.Current;
                        if (current.Name == name) {
                            return current;
                        }
                    }
                    while (enumerator.MoveNext());
                }
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Draw(GameTime time) {
            if (!Enabled) {
                return;
            }

            Constants.LineManager.SetDeclaration();

            bool drawAll = false;

            if(drawAll){
                float tim = (float)time.TotalRealTime.TotalSeconds;
                Constants.LineManager.Draw(
                    ref Lines,
                   0.0002f, 
                    Color.Black,
                    Constants.Camera.View * Constants.Camera.Projection,
                    tim, 
                    "NoBlur");

                return;
            }

            for (int i = 0; i < pCalles.Count; i++) {
                if (!CollisionHelper.isVisible(pCalles[i].BoundingSphere)) {
                    pCalles[i].Visible = false;
                    continue;
                }
                //Constants.Logger.AddLog("visible: " + pCalles[i].Name);
                pCalles[i].Visible = true;
                pCalles[i].Draw(time);
            }
        }
        public void CreateBounds() {
            for (int i = 0; i < pCalles.Count; i++) {
                pCalles[i].CreateBounds();
            }

            BoundingSphere = pCalles[0].BoundingSphere;

            for (int i = 1; i < pCalles.Count; i++) {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere, pCalles[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }

        public void Draw2d(GameTime time) {
            if (!Enabled) {
                return;
            }

            for (int i = 0; i < pCalles.Count; i++) {
                pCalles[i].Draw2d(time);
            }
        }

        List<RoundLine> Lines = new List<RoundLine>();

        public void Init(){
            for (int i = 0; i < pCalles.Count; i++) {
                Lines.AddRange(pCalles[i].Lines);
            }
        }
    }
}