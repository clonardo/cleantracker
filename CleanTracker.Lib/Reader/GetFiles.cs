using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTracker.Lib.Reader
{
    public static class FileUtils
    {
        public static List<string> GetFileNames(string targetDir, string filterStr)
        {
            return Directory.GetFiles(targetDir, filterStr, SearchOption.AllDirectories).ToList();   
        }
    }
}
