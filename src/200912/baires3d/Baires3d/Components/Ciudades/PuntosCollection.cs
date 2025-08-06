using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d{
    public class PuntosCollection : IEnumerable<PointsComponent> {
        private List<PointsComponent> pPuntos;
        public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;
        private bool BigBufferBuilt = false;
        private bool enabled;
        public bool Enabled {
            get { return enabled; }
            set {

                enabled = value;
                for (int i = 0; i < pPuntos.Count; i++) {
                    pPuntos[i].Enabled = value;
                }
            }
        }

        public IEnumerator<PointsComponent> GetEnumerator() {
            return (IEnumerator<PointsComponent>)this.pPuntos.GetEnumerator();
        }

        public void Add(PointsComponent barrio) {
            if (pPuntos == null) {
                pPuntos = new List<PointsComponent>();
            }
            pPuntos.Add(barrio);
        }

        public int Count {
            get {
                return this.pPuntos.Count;
            }
        }

        public PointsComponent this[int index] {
            get {
                if ((index >= 0) && (index < this.pPuntos.Count)) {
                    return this.pPuntos[index];
                }
                return null;
            }
        }

        public PointsComponent this[string name] {
            get {
                List<PointsComponent>.Enumerator enumerator = this.pPuntos.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        PointsComponent current = enumerator.Current;
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
        }
        public void CreateBounds() {
            for (int i = 0; i < pPuntos.Count; i++) {
                pPuntos[i].CreateBounds();
            }

            BoundingSphere = pPuntos[0].BoundingSphere;

            for (int i = 1; i < pPuntos.Count; i++) {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere, pPuntos[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }

        public void Draw2d(GameTime time) {
            for (int i = 0; i < pPuntos.Count; i++) {
                if (pPuntos[i].Visible) {
                    pPuntos[i].Draw2d(time);
                }
            }
        }
    }
}