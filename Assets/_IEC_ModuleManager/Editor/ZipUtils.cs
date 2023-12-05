using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace IEC.ModuleManager
{
    public class ZipUtils
    {
        public static void GetZipFileFromByteArray(byte[] zipFileData, Action<ZipFile> zipFileAction)
        {
            using (var fileStream = new MemoryStream())
            {
                fileStream.Write(zipFileData, 0, zipFileData.Length);
                fileStream.Flush();
                fileStream.Seek(0, SeekOrigin.Begin);

                zipFileAction(new ZipFile(fileStream));
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void ExtractZipFile(byte[] zipFileData, string targetDirectory, int bufferSize = 256 * 1024)
        {
            Directory.CreateDirectory(targetDirectory);

            GetZipFileFromByteArray(zipFileData, zipFile =>
            {
                foreach (ZipEntry entry in zipFile)
                {
                    string targetFile = Path.Combine(targetDirectory, entry.Name);

                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(targetFile);
                    }
                    else
                    {
                        using (FileStream outputFile = File.Create(targetFile))
                        {
                            if (entry.Size > 0)
                            {
                                Stream zippedStream = zipFile.GetInputStream(entry);
                                byte[] dataBuffer = new byte[bufferSize];

                                int readBytes;
                                while ((readBytes = zippedStream.Read(dataBuffer, 0, bufferSize)) > 0)
                                {
                                    outputFile.Write(dataBuffer, 0, readBytes);
                                    outputFile.Flush();
                                }
                            }
                        }
                    }
                }
            });

        }
    }
}