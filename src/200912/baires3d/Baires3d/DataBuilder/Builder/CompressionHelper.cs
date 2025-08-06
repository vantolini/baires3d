using System.IO;
using SevenZip;

namespace b3d{
    public static class CompressionHelper{
        public static MemoryStream DecompressFile(string fileName) {
            MemoryStream ms = new MemoryStream();
            SevenZipExtractor decompressor = new SevenZipExtractor(fileName);
            decompressor.ExtractFile(0, ms);
            ms.Position = 0;
            return ms;
        }

        public static void CompressFile(string fileName){
                        
            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
            compressor.CompressionLevel = CompressionLevel.Ultra;
            compressor.CompressionMethod = CompressionMethod.Lzma2;
            
            string[] filez = new string[1];
            filez[0] = fileName;

            string filePath = Path.GetDirectoryName(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);
            compressor.CompressionMode = CompressionMode.Create;
            compressor.DirectoryStructure = false;
            compressor.CompressFiles(filePath + "\\" + name + ".7z", filez);
            File.Delete(fileName);

        }

    }
}