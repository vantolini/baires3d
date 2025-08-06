using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;

namespace Builder{
    public static class CompressionHelper {

        public static void CompressFile(string fileName) {

            using (ZipFile zip = new ZipFile()) {
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                ZipEntry e = zip.AddFile(fileName, "");
                e.Comment = "Baires 3D - Victor Antolini";
                zip.Comment = String.Format("Baires 3D - Victor Antolini '{0}'", "lalala" /*System.Net.Dns.GetHostName()*/);

                string filePath2 = Path.GetDirectoryName(fileName);
                string name2 = Path.GetFileNameWithoutExtension(fileName);
                zip.Save(filePath2 + "/" + name2 + ".zip");
            }
            File.Delete(fileName);
        }

    }
}