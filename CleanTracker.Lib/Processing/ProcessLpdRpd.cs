using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTracker.Lib.Models;
using static CleanTracker.Lib.Reader.ReadCsv;
using static CleanTracker.Lib.Writer.WriterUtils;

namespace CleanTracker.Lib.Processing
{
    /// <summary>
    /// Stuff for processing LPD/RPD data
    /// </summary>
    public static class ProcessLpdRpd
    {
        /// <summary>
        /// Filter where CS == 1 || 2, then get all clicks (LPD, RPD)
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private static List<LpdRpd> getPdAtClicksForMediaId(List<Row> records)
        {
            return records.Where(r => { return (r.CS == 1 || r.CS == 2); }).Select(g => { return new LpdRpd(g.LPD, g.RPD, g.CS, Convert.ToDouble(g.time), g.CNT); }).ToList();
        }



        /// <summary>
        /// get average for whole time on screen
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private static AvgLpdRpd getAvgPdForMediaId(List<Row> records)
        {
            var lpd = records.Select(r => r.LPD).Average();
            var rpd = records.Select(r => r.RPD).Average();
            return new AvgLpdRpd(lpd, rpd);
        }

        public static List<AveragesWithClickData> ReadAndProcessFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var firstUnderscore = fileName.IndexOf('_');
            var participantId = fileName.Substring(0, firstUnderscore);
            var records = ReadAllData(filePath, "reading file");
            // records.Dump();
            var groups = records
            .Select(x => x)
            .GroupBy(x => { return x.MEDIA_ID; });

            var results = new List<AveragesWithClickData>();
            foreach (var grp in groups)
            {
                var clicks = getPdAtClicksForMediaId(grp.ToList());
                // exclude stimuli without any click data
                if (clicks.Count > 0)
                {
                    var averages = getAvgPdForMediaId(grp.ToList());
                    results.Add(new AveragesWithClickData(averages.avgLPD, averages.avgRPD, clicks, grp.Key, participantId));
                }
                else
                {
                    continue;
                }
            }
            return results;
        }

        /// <summary>
        /// Iterate over files, roll up LPD/RPD data, and write result
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="outputBasePath"></param>
        /// <param name="outputFileName"></param>
        public static void ReadAndProcessFiles(IEnumerable<string> filePaths, string outputBasePath, string outputFileName)
        {
            List<AveragesWithClickData> toWrite = new List<AveragesWithClickData>();
            foreach(var fp in filePaths)
            {
                try
                {
                    var processed = ReadAndProcessFile(fp);
                    if (processed.Count() > 0)
                    {
                        toWrite.AddRange(processed);
                        Console.WriteLine("Loaded "+processed.Count().ToString()+" records for "+fp);
                    }
                    else
                    {
                        Console.WriteLine("No data to save for"+ fp);
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred reading " + fp+ " : " + ex.Message.ToString());
                }
            }
            if (toWrite.Count() > 0)
            {
                try
                {
                    WriteCsvFile(Path.Combine(outputBasePath, outputFileName), toWrite);
                    Console.WriteLine("Saved to : " + outputBasePath + outputFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred saving the output: "+ ex.Message.ToString());
                }

            }
        }
    }
}
