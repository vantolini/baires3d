using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Npgsql;

namespace Builder
{
    public class BarrioBuilder
    {
        private const string SERVER_HOST = "127.0.0.1";
        private const string SERVER_PORT = "5432";
        private const string SERVER_USERNAME = "postgres";
        private const string SERVER_PASSWORD = "***********";
        public bool Done = false;
        public bool AllDone;

        public void Start(string barrio)
        {
            NpgsqlConnection Connection = new NpgsqlConnection(
                "Server=" + SERVER_HOST +
                ";Port=" + SERVER_PORT +
                ";User Id=" + SERVER_USERNAME +
                ";Password=" + SERVER_PASSWORD +
                ";Database=" + "gis" + ";" +
                //";Sslmode=Allow;" +
                "Preload Reader=True;" +
                "Encoding=True;"
                );
            Connection.Open();

            Constants.Connection = Connection;
            PolygonBuilder polygonBuilder = new PolygonBuilder();


            polygonBuilder.Altura = 3.3f;
            polygonBuilder.TableName = "manzanas";
            polygonBuilder.LayerType = "Manzanas";
            AddLog("Building " + barrio + "... ");
            polygonBuilder.ProcessCiudad(barrio);



            PointBuilder pointBuilder = new PointBuilder(Connection);
            pointBuilder.Process(barrio);




            Connection.Close();

        }
        public List<Feature> Siluetas = new List<Feature>();
        private void bw_RunWorkerCompleted(object sender,
                                     RunWorkerCompletedEventArgs e)
        {
            Siluetas = (List<Feature>)e.Result;
            Done = true;
        }


        static void AddLog(string log) {
            Console.Write(log + "\n");
        }
    }
}
