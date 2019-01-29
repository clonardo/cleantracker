using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTracker.Lib.Models;
using static CleanTracker.Lib.Reader.ReadCsv;
using static CleanTracker.Lib.Writer.WriterUtils;
using CleanTracker.Lib.Extensions;

namespace CleanTracker.Lib.Processing
{
    public class MediaNameAndIdComparer : IEqualityComparer<MediaNameAndId>
    {
        /// <summary>
        /// Has a good distribution.
        /// </summary>
        const int _multiplier = 89;

        /// <summary>
        /// Whether the two objects are equal
        /// </summary>
        public bool Equals(MediaNameAndId x, MediaNameAndId y)
        {
            return x.mediaId == y.mediaId && x.mediaName == y.mediaName;
        }

        /// <summary>
        /// Return the hash code for this string.
        /// </summary>
        public int GetHashCode(MediaNameAndId obj)
        {

            // Don't compute hash code on null object.
            if (obj == null)
            {
                return 0;
            }

            return obj.mediaName.GetHashCode() + obj.mediaId;;
        }
    }

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
        /// Get rows where CS = 1 or 2, and the stimulus is an image
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Row> ReadFileAndLoadImageStimuli(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var firstUnderscore = fileName.IndexOf('_');
            var participantId = fileName.Substring(0, firstUnderscore);
            var records = ReadAllData(filePath, "reading file");
            // records.Dump();
            return records
                .Where(row =>
                {
                    // pictures: 431, 433, 437
                    return ((row.CS == 1 || row.CS == 2) && (row.MEDIA_ID == 431 || row.MEDIA_ID == 433 || row.MEDIA_ID == 437));
                })
            .Select(x => x).ToList();
        }

        /// <summary>
        /// Get rows where the media ID is mismatched
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Row> ReadFileAndLoadMediaIdMismatches(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var firstUnderscore = fileName.IndexOf('_');
            var participantId = fileName.Substring(0, firstUnderscore);
            var records = ReadAllData(filePath, "reading file");
            // records.Dump();
            return records
                .Where(row =>
                {
                    return row.IsFubar();
                })
            .Select(x => x).ToList();
        }

        /// <summary>
        /// Get media name and media ID from the source files
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<MediaNameAndId> ReadFileAndLoadMediaIdAndNames(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var firstUnderscore = fileName.IndexOf('_');
            var participantId = fileName.Substring(0, firstUnderscore);
            var records = ReadAllData(filePath, "reading file");
            // records.Dump();
            return records
            .Select(x => new MediaNameAndId(x)).Distinct(new MediaNameAndIdComparer()).ToList();
        }

        /// <summary>
        /// Iterate over files, find all media IDs that have multiple media names, and write result to disk
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="outputBasePath"></param>
        /// <param name="outputFileName"></param>
        public static void ReadFilesAndGetMediaIdsWIthDuplicateMediaNames(IEnumerable<string> filePaths, string outputBasePath, string outputFileName)
        {
            List<MediaNameAndId> toWrite = new List<MediaNameAndId>();
            foreach (var fp in filePaths)
            {
                try
                {
                    var processed = ReadFileAndLoadMediaIdAndNames(fp);
                    if (processed.Count() > 0)
                    {
                        toWrite.AddRange(processed);
                        Console.WriteLine("Loaded " + processed.Count().ToString() + " records for " + fp);
                    }
                    else
                    {
                        Console.WriteLine("No data to save for" + fp);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred reading " + fp + " : " + ex.Message.ToString());
                }
            }
            if (toWrite.Count() > 0)
            {
                try
                {
                    var results = toWrite.Distinct().GroupBy(x => { return x.mediaId; }).Where(grp =>
                    {
                        var names = grp.Select(g => { return g.mediaName; }).Distinct().ToList();
                        return names.Count > 1;
                    }).SelectMany(res =>
                    {
                        return res;
                    }).Distinct(new MediaNameAndIdComparer()).ToList();
                    WriteCsvFile(Path.Combine(outputBasePath, outputFileName), toWrite);
                    Console.WriteLine("Saved to : " + outputBasePath + outputFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred saving the output: " + ex.Message.ToString());
                }

            }
        }

        /// <summary>
        /// Iterate over files, find all that have image stimuli, and write result to disk
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="outputBasePath"></param>
        /// <param name="outputFileName"></param>
        public static void ReadFilesAndAggregateImageData(IEnumerable<string> filePaths, string outputBasePath, string outputFileName)
        {
            List<Row> toWrite = new List<Row>();
            foreach (var fp in filePaths)
            {
                try
                {
                    var processed = ReadFileAndLoadImageStimuli(fp);
                    if (processed.Count() > 0)
                    {
                        toWrite.AddRange(processed);
                        Console.WriteLine("Loaded " + processed.Count().ToString() + " records for " + fp);
                    }
                    else
                    {
                        Console.WriteLine("No data to save for" + fp);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred reading " + fp + " : " + ex.Message.ToString());
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
                    Console.WriteLine("An error occurred saving the output: " + ex.Message.ToString());
                }

            }
        }

        /// <summary>
        /// Iterate over files, find all that have screwed-up media IDs, and write result to disk
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="outputBasePath"></param>
        /// <param name="outputFileName"></param>
        public static void ReadFilesAndAggregateMismatchedMediaIdData(IEnumerable<string> filePaths, string outputBasePath, string outputFileName)
        {
            List<Row> toWrite = new List<Row>();
            foreach (var fp in filePaths)
            {
                try
                {
                    var processed = ReadFileAndLoadMediaIdMismatches(fp);
                    if (processed.Count() > 0)
                    {
                        toWrite.AddRange(processed);
                        Console.WriteLine("Loaded " + processed.Count().ToString() + " records for " + fp);
                    }
                    else
                    {
                        Console.WriteLine("No data to save for" + fp);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred reading " + fp + " : " + ex.Message.ToString());
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
                    Console.WriteLine("An error occurred saving the output: " + ex.Message.ToString());
                }

            }
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
