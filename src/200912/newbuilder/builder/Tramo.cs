using System;
using System.Collections.Generic;

namespace Builder{
    public class Calle {
        public string Nombre;
        public List<CalleSegmento> Segmentos = new List<CalleSegmento>();


        public List<CalleTramo> TramosOriginales = new List<CalleTramo>();
        public List<Vector3> Puntos = new List<Vector3>();
        public Calle(string nombre) {
            Nombre = nombre;
        }

        public CalleTramo FindConnected(CalleTramo except, out bool forward) {
            for (int i = 0; i < TramosOriginales.Count; i++) {
                bool found = false;

                //Console.WriteLine("dist1: " + Vector3.Distance(TramosOriginales[i].Puntos[0], except.Puntos[except.Puntos.Count - 1]).ToString());

                //Console.WriteLine("dist2: " + Vector3.Distance(TramosOriginales[i].Puntos[TramosOriginales[i].Puntos.Count - 1], except.Puntos[0]).ToString());

                if (Vector3.Distance(TramosOriginales[i].Puntos[0], except.Puntos[except.Puntos.Count - 1]) < 0.000002) {
                    forward = true;
                    return TramosOriginales[i];
                }
                else {
                    if (Vector3.Distance(TramosOriginales[i].Puntos[TramosOriginales[i].Puntos.Count - 1], except.Puntos[0]) < 0.000002) {
                        forward = false;
                        return TramosOriginales[i];
                    }

                }
            }
            forward = false;
            return null;

        }
        public void Debug(string str){
            Console.Write(str);
        }

        public void BuildSegments(){
            if(TramosOriginales.Count == 1){
                Segmentos.Add(new CalleSegmento(TramosOriginales));
                return;
            }
            Vector3 first = TramosOriginales[0].Puntos[0];

            bool finished = false;
            CalleTramo currentTramo = TramosOriginales[0];

            int except = 0;
            bool goingForward = false;

            CalleSegmento segmento = new CalleSegmento();
            bool firstRun = true;
            if(this.Nombre.Contains("ESCALADA")){
                Console.Write("");
            }

            while(!finished){
                
                segmento = new CalleSegmento();
                currentTramo = TramosOriginales[0];
                TramosOriginales.Remove(currentTramo);


                segmento.Tramos.Add(currentTramo);

                if(firstRun){
                    //segmento.Tramos.Add(currentTramo);
                    firstRun = false;
                }else{}

                bool foundConnections = false;
                
                while(!foundConnections){
                    bool forward = false;
                    CalleTramo foundTramo = FindConnected(currentTramo, out forward);

                    if(foundTramo == null){
                        foundConnections = true;

                        //CalleTramo foundTramo = FindConnected(currentTramo, out forward);
                    }else{
                        currentTramo = foundTramo;
                        //segmento.Tramos.Add(currentTramo);
                        if(!forward){
                           List<CalleTramo> trams = segmento.Tramos;
                            List<CalleTramo> tmp = new List<CalleTramo>();
                            tmp.Add(currentTramo);
                            for (int xx = 0; xx < trams.Count; xx++) {
                                tmp.Add(trams[xx]);
                            }

                            segmento.Tramos = tmp;
                        }else{
                            segmento.Tramos.Add(currentTramo);

                        }

                        TramosOriginales.Remove(currentTramo);
                    }
                }


                Segmentos.Add(segmento);
                if (TramosOriginales.Count == 0) {
                    finished = true;
                }
            }
        }

        private bool mustRemove;
        private List<CalleTramo> ToRemove = new List<CalleTramo>();
        private CalleSegmento findSegmento(int except, Vector3 first) {
            CalleSegmento retSegmento = new CalleSegmento();
            Vector3 current = Vector3.Zero;
            for(int i = 0 ; i < TramosOriginales.Count;i++){
                if(i != except){
                    if (TramosOriginales[i].Puntos[0] == first){
                        retSegmento.Tramos.Add(TramosOriginales[i]);
                    }
                }
            }
            return retSegmento;

        }
    }
    [System.Diagnostics.DebuggerDisplay("{Tramos[0].AlturaInicialPar} {Tramos[Tramos.Count - 1].AlturaFinalPar}")]
    public class CalleSegmento {
        public List<CalleTramo> Tramos = new List<CalleTramo>();

        public CalleSegmento(List<CalleTramo> tramos) {
            Tramos = tramos;
        }
        public CalleSegmento() {
        }
    }

    [System.Diagnostics.DebuggerDisplay("{AlturaInicialPar} {AlturaFinalPar}  | {Puntos[0]} {Puntos[Puntos.Count - 1]} |")]
    public class CalleTramo {
        public string AlturaInicialPar;
        public string AlturaInicialImpar;
        public string AlturaFinalPar;
        public string AlturaFinalImpar;

        public List<Vector3> Puntos = new List<Vector3>();
        public string Sentido;
        public string Tipo;
    }


    [System.Diagnostics.DebuggerDisplay("{Source} {Target}  | {Data[2]} {Data[3]} | {OrigLine[0].X}##{OrigLine[0].Z} |  {OrigLine[OrigLine.Count - 1].X}##{OrigLine[OrigLine.Count - 1].Z}")]
    public class Tramo
    {
        public Tramo Next;
        public Tramo Previous;

        public List<string> Data = new List<string>();
        public List<Vector3> OrigLine = new List<Vector3>();
        public bool Disabled = false;

        public int Source;
        public int Target;

        public Tramo()
        {
        }
        public Tramo(List<Vector3> origLine)
        {
            OrigLine = origLine;
        }
        public Tramo(List<Vector3> origLine, List<string> data) {
            OrigLine = origLine;
            Data = data;
        }

        public Tramo(List<Vector3> origLine, List<string> data, int source, int target)
        {
            OrigLine = origLine;
            Data = data;
            Source = source;
            Target = target;

        }


    }
}