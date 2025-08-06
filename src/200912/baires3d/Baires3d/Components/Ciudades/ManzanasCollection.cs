using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class ManzanasCollection : IEnumerable<Manzana> {
        private List<Manzana> pManzanas;

        public MeshComponent Mesh;
        public IEnumerator<Manzana> GetEnumerator() {
            return (IEnumerator<Manzana>)this.pManzanas.GetEnumerator();
        }

        public void AddLotes(List<Lote> lotes) {
            if (pManzanas == null || pManzanas.Count ==0) {
                return;
            }
            for (int m = 0; m < pManzanas.Count;m++ ){
                for (int l = 0; l < lotes.Count; l++) {
                    if(lotes[l].Codigo == ""){
                        continue;
                    }
                    if(pManzanas[m].Codigo == lotes[l].Codigo){
                        if (pManzanas[m].Lotes == null) {
                            pManzanas[m].Lotes = new List<Lote>();
                        }
                        pManzanas[m].Lotes.Add(lotes[l]);
                    }
                }
            }

            for (int m = 0; m < pManzanas.Count; m++){
                if(pManzanas[m].Lotes == null || pManzanas[m].Lotes.Count == 0){
                    continue;
                }

            }

        }

        public void Add(Manzana manzana) {
            if (pManzanas == null) {
                pManzanas = new List<Manzana>();
            }
            pManzanas.Add(manzana);
        }

        // Properties
        public int Count {
            get {
                return this.pManzanas.Count;
            }
        }

        public Manzana this[int index] {
            get {
                if ((index >= 0) && (index < this.pManzanas.Count)) {
                    return this.pManzanas[index];
                }
                return null;
            }
        }

        public Manzana this[string name] {
            get {
                List<Manzana>.Enumerator enumerator = this.pManzanas.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        Manzana current = enumerator.Current;
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
            Mesh.Draw(time);
        }
    }
}