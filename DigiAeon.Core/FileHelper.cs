using System.IO;

namespace DigiAeon.Core
{
    public static class FileHelper
    {
        public static void CreateFile(string filePath, Stream fileStream)
        {
            File.WriteAllBytes(filePath, ToBytes(fileStream));
        }

        // http://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
        private static byte[] ToBytes(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}