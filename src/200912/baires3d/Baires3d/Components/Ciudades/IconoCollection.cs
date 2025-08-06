using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d{
    public class IconoCollection : IEnumerable<Icono> {
        private List<Icono> pIconos;

        public IEnumerator<Icono> GetEnumerator() {
            return (IEnumerator<Icono>)this.pIconos.GetEnumerator();
        }

        public void Add(Icono ciudad) {
            if (pIconos == null) {
                pIconos = new List<Icono>();
            }
            pIconos.Add(ciudad);
        }

        // Properties
        public int Count {
            get {
                return this.pIconos.Count;
            }
        }

        public Icono this[int index] {
            get {
                if ((index >= 0) && (index < this.pIconos.Count)) {
                    return this.pIconos[index];
                }
                return null;
            }
        }
        public bool Exists(string name) {
            for(int i = 0; i < pIconos.Count;i++){
                if (pIconos[i].Name == name) {
                    return true;
                }
            }
            return false;
        }

        public Icono this[string name] {
            get {
                List<Icono>.Enumerator enumerator = this.pIconos.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        Icono current = enumerator.Current;
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

        public void Draw(GameTime time)
        {
            
        }
    }
}