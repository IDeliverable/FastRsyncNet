using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using NUnit.Framework;

namespace Octodiff.Tests
{
    public class PackageGenerator
    {
        static Random r = new Random();

        public static void GeneratePackage(string fileName, int numberOfFiles = 10, int averageFileSize = 100*1024)
        {
            var fullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            using (var package = ZipPackage.Open(fullPath, FileMode.Create))
            {
                for (int i = 0; i < numberOfFiles; i++)
                {
                    var buffer = new byte[averageFileSize];
                    var part = package.CreatePart(new Uri("/" + Guid.NewGuid(), UriKind.Relative), "text/plain");
                    using (var partStream = part.GetStream(FileMode.Create))
                    {
                        r.NextBytes(buffer);
                        partStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public static void ModifyPackage(string fileName, string newFileName, int filesToAdd, int filesToRemove, int averageFileSize = 100*1024)
        {
            var oldFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            var fullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, newFileName);
            File.Copy(oldFilePath, fullPath, true);
            using (var package = ZipPackage.Open(fullPath, FileMode.Open))
            {
                for (int i = 0; i < filesToAdd; i++)
                {
                    var buffer = new byte[averageFileSize];
                    var part = package.CreatePart(new Uri("/" + Guid.NewGuid(), UriKind.Relative), "text/plain");
                    using (var partStream = part.GetStream(FileMode.Create))
                    {
                        r.NextBytes(buffer);
                        partStream.Write(buffer, 0, buffer.Length);
                    }
                }
                var parts = package.GetParts().OrderBy(o => r.Next(0, 10)).Take(filesToRemove).ToArray();
                foreach (var part in parts)
                {
                    package.DeletePart(part.Uri);
                }
            }
        }
    }
}