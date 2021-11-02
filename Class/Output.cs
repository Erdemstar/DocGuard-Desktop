using System;
using System.Collections.Generic;
using System.Text;

namespace DocGuard_Desktop.Class
{
    class Output
    {
        public string appName { get; set; }
        public string type { get; set; }
        public string sourceFile { get; set; }
        public string sourceFolder { get; set; }
        public string Url { get; set; }
        public string outputPath { get; set; }
        public int threadSleepCount { get; set; }

        public List<successOutput> succesOutput { get; set; }
        public List<errorOutput> errorOutput { get; set; }

        public int fileCount { get; set; }
        public int successCount { get; set; }
        public int errorCount { get; set; }
        public string finish { get; set; }
    }


    class successOutput
    {
        public string fileName { get; set; }
        public string errorMessage { get; set; }
        public string filePath { get; set; }
        public string fileType { get; set; }
        public string Verdict { get; set; }
        public string MD5 { get; set; }
    }

    class errorOutput
    {
        public string errorMessage { get; set; }
        
    }


}
