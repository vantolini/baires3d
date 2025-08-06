using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class CiudadesCollection : IEnumerable<Ciudad> {
        private List<Ciudad> pCiudades;

        public IEnumerator<Ciudad> GetEnumerator() {
            return (IEnumerator<Ciudad>)this.pCiudades.GetEnumerator();
        }

        public void Add(Ciudad ciudad) {
            if(pCiudades == null){
                pCiudades = new List<Ciudad>();
            }
            pCiudades.Add(ciudad);
        }

        public int Count {
            get {
                return this.pCiudades.Count;
            }
        }

        public Ciudad this[int index] {
            get {
                if ((index >= 0) && (index < this.pCiudades.Count)) {
                    return this.pCiudades[index];
                }
                return null;
            }
        }

        public Ciudad this[string name] {
            get {
                List<Ciudad>.Enumerator enumerator = this.pCiudades.GetEnumerator();
                if (enumerator.MoveNext()) {
                    do {
                        Ciudad current = enumerator.Current;
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

        public void Draw(GameTime time){
            for(int i =0;i < pCiudades.Count;i++){
                pCiudades[i].Draw(time);
            }
        }

        public void Draw(GameTime gameTime,  Effect effect, bool b){
            for (int i = 0; i < pCiudades.Count; i++) {
                pCiudades[i].Draw(gameTime, effect, b);
            }
        }
    }
}