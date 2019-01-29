using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CleanTracker.Lib.Models;

namespace CleanTracker.Lib.Writer
{
    public static class WriterUtils
    {
        /// <summary>
        /// Create directory path name from base path, timestamp, and leaf segment
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="timestamp"></param>
        /// <param name="leafSegment"></param>
        /// <returns></returns>
        public static string BuildDirectoryName(string basePath, string timestamp, string leafSegment)
        {

            return Path.Combine(basePath, timestamp, leafSegment);
        }

        static void CreateIfMissing(string path)
        {
          bool folderExists = Directory.Exists(path);
          if (!folderExists)
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Created directory at "+path);
            }
            
        }

        public static void CreateDirectories(string basePath, string timestamp, string cleanPath, string rejectedPath)
        {
            WriterUtils.CreateIfMissing(basePath);
            var timestampedPath = Path.Combine(basePath, timestamp);
            WriterUtils.CreateIfMissing(timestampedPath);
            WriterUtils.CreateIfMissing(cleanPath);
            WriterUtils.CreateIfMissing(rejectedPath);
        }

        
        public static void WriteCsvFile(string filename, IEnumerable<MediaNameAndId> rows)
        {

            try
            {
                TextWriter textWriter = File.CreateText(filename);

                var csvWriter = new CsvHelper.CsvWriter(textWriter);
                csvWriter.WriteRecords(rows);

                textWriter.Close();
                Console.WriteLine("Wrote file to " + filename);
            }
            catch
            {
                Console.WriteLine("Error writing to " + filename);
            }
        }

        public static void WriteCsvFile(string filename, IEnumerable<Row> rows)
        {
            
            try
            {
                TextWriter textWriter = File.CreateText(filename);

                var csvWriter = new CsvHelper.CsvWriter(textWriter);
                csvWriter.WriteRecords(rows);

                textWriter.Close();
                Console.WriteLine("Wrote file to "+filename);
            }
            catch
            {
                Console.WriteLine("Error writing to " + filename);
            }
        }

        public static void WriteCsvFile(string filename, IEnumerable<AveragesWithClickData> rows)
        {
            try
            {
                TextWriter textWriter = File.CreateText(filename);

                var csvWriter = new CsvHelper.CsvWriter(textWriter);
                csvWriter.WriteRecords(rows);

                textWriter.Close();
                Console.WriteLine("Wrote file to " + filename);
            }
            catch
            {
                Console.WriteLine("Error writing to " + filename);
            }
        }
    }
}
