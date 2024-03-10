using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CycloneGames.Utility
{
    public class FileUtility
    {
        private static string DEBUG_FLAG = "[FileUtility]";
        public static bool FilesAreEqual(string filePath1, string filePath2)
        {
            byte[] file1Hash = ComputeFileHash(filePath1);
            byte[] file2Hash = ComputeFileHash(filePath2);
            return StructuralComparisons.StructuralEqualityComparer.Equals(file1Hash, file2Hash);
        }
        private static byte[] ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return sha256.ComputeHash(stream);
                }
            }
        }
        public static void CopyFileWithComparison(string sourceFilePath, string destinationFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Source file does not exist.");
            }

            // Ensure the destination directory exists
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(destinationDirectory); // If already exists, this does nothing

            if (File.Exists(destinationFilePath))
            {
                if (FilesAreEqual(sourceFilePath, destinationFilePath))
                {
                    // Files are the same, so skip the copy
                    UnityEngine.Debug.Log($"{DEBUG_FLAG} The files are identical. No copy needed.");
                    return;
                }
                else
                {
                    // Files are different, delete the existing file
                    File.Delete(destinationFilePath);
                }
            }

            // Copy the file to the destination path
            File.Copy(sourceFilePath, destinationFilePath);
            UnityEngine.Debug.Log($"{DEBUG_FLAG} File copied successfully.");
        }
        public static bool LargeFilesAreEqual(string filePath1, string filePath2)
        {
            using (var sha256 = SHA256.Create())
            {
                using (FileStream fileStream1 = new FileStream(filePath1, FileMode.Open),
                       fileStream2 = new FileStream(filePath2, FileMode.Open))
                {
                    if (fileStream1.Length != fileStream2.Length)
                    {
                        return false;
                    }

                    byte[] buffer1 = new byte[4096]; // 4KB buffer
                    byte[] buffer2 = new byte[4096];
                    int bytesRead1, bytesRead2;

                    while ((bytesRead1 = fileStream1.Read(buffer1, 0, buffer1.Length)) > 0)
                    {
                        bytesRead2 = fileStream2.Read(buffer2, 0, buffer2.Length);
                        if (bytesRead1 != bytesRead2 ||
                            !buffer1.Take(bytesRead1).SequenceEqual(buffer2.Take(bytesRead1)))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        public static void CopyLargeFileWithComparison(string sourceFilePath, string destinationFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Source file does not exist.");
            }

            // Ensure the directory of the destination file exists
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(destinationDirectory); // If already exists, this does nothing

            if (File.Exists(destinationFilePath))
            {
                if (LargeFilesAreEqual(sourceFilePath, destinationFilePath))
                {
                    // Files are the same, so skip the copy
                    UnityEngine.Debug.Log($"{DEBUG_FLAG} The files are identical. No copy needed.");
                    return;
                }
                else
                {
                    // Files are different, delete the existing file
                    File.Delete(destinationFilePath);
                }
            }

            // Copy the file to the destination path
            File.Copy(sourceFilePath, destinationFilePath);
            UnityEngine.Debug.Log($"{DEBUG_FLAG} File copied successfully.");
        }
    }
}