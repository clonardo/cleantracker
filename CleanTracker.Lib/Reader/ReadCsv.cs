using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTracker.Lib.Models;
using CsvHelper;
using CsvHelper.Configuration;
using static CleanTracker.Lib.Writer.WriterUtils;

namespace CleanTracker.Lib.Reader
{
    public static class ReadCsv
    {
        /// <summary>
        /// Reads CSV file, deduplicates data. Filter on CS = 1||2
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="cleanDirName"></param>
        /// <param name="rejectedDirName"></param>
        /// <param name="msg"></param>
        public static void Read(string filepath, string cleanDirName, string rejectedDirName, string msg)
        {
            
            var filenameWithExt= Path.GetFileName(filepath);

            using (var sr = new StreamReader(filepath))
            {
                var cleanRows = new List<Row>();
                var rejectedRows = new List<Row>();
                Console.WriteLine("--"+msg+" - reading "+filepath);

                var csvr = new CsvReader(sr);
                
                
                csvr.Configuration.HasHeaderRecord = true;
                csvr.Configuration.HeaderValidated = null;
                csvr.Configuration.MissingFieldFound = null;
                
                // make note of the last media ID to scrub changed values
                int lastMediaId = -1;
                
                while (csvr.Read())
                {
                    var row = csvr.GetRecord<Row>();
                    if (row != null) {
                        var time = Convert.ToDouble(csvr.GetField(3));
                        var timetick = (long)Convert.ToDouble(csvr.GetField(4));
                        if (row != null && (row.CS == 1 || row.CS == 2))
                        {
                            row.time = time;
                            row.timetick = timetick;
                            if (cleanRows.Count > 0 && lastMediaId == row.MEDIA_ID)
                            {
                                var prevRow = cleanRows[cleanRows.Count - 1];
                                if (prevRow.MEDIA_ID == row.MEDIA_ID)
                                {
                                    rejectedRows.Add(prevRow);
                                    cleanRows.Remove(prevRow);
                                }
                            }
                            lastMediaId = row.MEDIA_ID;
                            cleanRows.Add(row);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("-- WARNING: "+ filenameWithExt+" is empty or invalid");
                        Console.ResetColor();
                        continue;
                    }
                    
                }

                if(cleanRows.Count > 0)
                {
                    WriteCsvFile(Path.Combine(cleanDirName, filenameWithExt), cleanRows);
                }
                if(rejectedRows.Count > 0)
                {
                    var rejectedFileName = Path.GetFileNameWithoutExtension(filepath)+"_rejected.csv";
                    WriteCsvFile(Path.Combine(rejectedDirName, rejectedFileName), rejectedRows);

                }
                Console.WriteLine("read complete");
            }
        }

        /// <summary>
        /// Read a single file at the specified path. 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="msg"></param>
        public static List<Row> ReadAllData(string filepath, string msg)
        {

            var filenameWithExt = Path.GetFileName(filepath);
            var allRows = new List<Row>();

            Console.WriteLine("--" + msg + " - reading " + filepath);

            using (var sr = new StreamReader(filepath))
            {
                

                var csvr = new CsvReader(sr);


                csvr.Configuration.HasHeaderRecord = true;
                csvr.Configuration.HeaderValidated = null;
                csvr.Configuration.MissingFieldFound = null;

                // make note of the last media ID to scrub changed values
                int lastMediaId = -1;

                while (csvr.Read())
                {
                    var row = csvr.GetRecord<Row>();
                    if (row != null)
                    {
                        var time = Convert.ToDouble(csvr.GetField(3));
                        var timetick = (long)Convert.ToDouble(csvr.GetField(4));
                        if (row != null )
                        {
                            row.time = time;
                            row.timetick = timetick;
                                 
                            allRows.Add(row);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("-- WARNING: " + filenameWithExt + " is empty or invalid");
                        Console.ResetColor();
                        continue;
                    }

                }

                

            }
            Console.WriteLine("read complete. data length (rows):", allRows.Count.ToString());
            return allRows;
        }
    
}
}
