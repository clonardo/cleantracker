using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using static CleanTracker.Lib.Reader.ReadCsv;
using static CleanTracker.Lib.Reader.FileUtils;
using static CleanTracker.Lib.Writer.WriterUtils;
using static CleanTracker.Lib.Processing.ProcessLpdRpd;

namespace CleanTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("You must put all csv files in a folder called C:\\gazepoint_data");
            Console.WriteLine("The program will use only file names containing the text \"all_gaze\"");
            Console.WriteLine("Cleaned csv files will be written to a folder called C:\\gazepoint_cleaned");
            Console.WriteLine("Make sure that you have closed all of the relevant CSV files before continuing!");
            Console.WriteLine("Press any key to get started");
            Console.ReadKey();
            Console.WriteLine("Let's do this.");
            string targetPath = "C:\\gazepoint_data";
            string filterStr = "*all_gaze*.csv";
            // string filterStr = "User 103_all_gaze.csv";

            string timestamp = DateTime.Now.ToString("T").Replace(':', '-');
            string outputBasePath = "C:\\gazepoint_cleaned";
            string outputCleanPathSegment = "cleaned";
            string outputRejectedPathSegment = "rejected";

            string cleanDirName = BuildDirectoryName(outputBasePath, timestamp, outputCleanPathSegment);
            string rejectedDirName = BuildDirectoryName(outputBasePath, timestamp, outputRejectedPathSegment);

            var targetFiles = GetFileNames(targetPath, filterStr);
            var progressIdx = 0;
            if(targetFiles.Count > 0)
            {
                Console.WriteLine("Found "+targetFiles.Count.ToString()+" target files");
                CreateDirectories(outputBasePath, timestamp, cleanDirName, rejectedDirName);

                // get all targeted stimuli
                // ReadAndProcessFiles(targetFiles, Path.Combine(outputBasePath, timestamp), "aggregated.csv");

                // get all with images
                //ReadFilesAndAggregateImageData(targetFiles, Path.Combine(outputBasePath, timestamp), "aggregated-images.csv");

                // solve illiteracy by finding media ID mismatches
                ReadFilesAndAggregateMismatchedMediaIdData(targetFiles, Path.Combine(outputBasePath, timestamp), "mismatched-media-ids.csv");

                // get media IDs with dupe media names. doesn't work 100%, more pivoting needed
                // ReadFilesAndGetMediaIdsWIthDuplicateMediaNames(targetFiles, Path.Combine(outputBasePath, timestamp), "dupe-media-names.csv");

                //Read("C:\\gazepoint_data\\ESCR0033_all_gaze.csv");
                /*
                foreach (var tgt in targetFiles)
                {
                    progressIdx++;
                    var msg = progressIdx.ToString()+"/"+targetFiles.Count.ToString();

                    try
                    {
                        //Read(tgt, cleanDirName, rejectedDirName, msg);
                        //var rows = ReadAllData(tgt, msg);
                        var processed = ReadAndProcessFile(tgt);
                        if (processed.Count() > 0)
                        {

                        }
                        else
                        {
                            
                        }
                        // WriteCsvFile(Path.Combine(rejectedDirName, rejectedFileName), rejectedRows);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred reading "+tgt+" : "+ex.Message.ToString());
                    }
                    
                }
                */
                Console.WriteLine("Completed! Press any key to exit");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("No valid target files found.");
            }
            
        }
    }
}
