using System;

namespace Builder{
    public static class CoordinateConverter {
        public static string[] vals = new string[2];
        public static float orig = -1;




        public static float[] OriginalToConverted(double x, double y, double z) {
            string[] vals = new string[2];
            vals[0] = x.ToString().Substring(4);
            vals[1] = z.ToString().Substring(4);



            if (vals[0].Length < 8) {
                vals[0] = vals[0].PadRight(7, '0');
                vals[0] = vals[0].PadRight(8, '1');
            }
            else {
                vals[0] = vals[0].Substring(0, 8);
            }

            if (vals[1].Length < 8) {
                vals[1] = vals[1].PadRight(7, '0');
                vals[1] = vals[1].PadRight(8, '1');
            }
            else {
                vals[1] = vals[1].Substring(0, 8);
            }

            float dX = float.Parse(vals[0]) / 10000;
            float dY = (((float)y) + 0.1f) * 50;// + 0.0000001f;
            dY = 0;// + 0.0000001f;
            float dZ = float.Parse(vals[1]) / 10000;
            if(CoordinateConverter.orig == -1)
            {
                CoordinateConverter.orig = dX;
            }
            dX -= CoordinateConverter.orig;
            dZ -= CoordinateConverter.orig;

            return new float[] { dX, dY, dZ };
        }

        public static float restar = 9865.165f;


    }
}