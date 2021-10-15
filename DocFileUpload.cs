using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DocGuard_Desktop
{
    class DocFileUpload
    {
        public string FilePath { get; set; }
        public string URL { get; set; }
        public int maxFileSize = 26214400;
        public int fileCount = 0;


        public string[] AllowedExtension = { ".hta", ".pdf", ".slk", ".csv", ".doc", ".dot", ".docx", ".docm", ".dotx", ".dotm",
                ".wll", ".xls", ".xll", ".xlw", ".xlt", ".xlsx", ".xlsm", ".xlsb", ".xlam", ".xltx", ".xltm", ".ppt", ".pps",
                ".pptx", ".pptm", ".ppsx", ".ppam", "ppa", ".rtf", ".bin", ".pub" };

        public DocFileUpload(string Path)
        {
            FilePath = Path;
            URL = "https://localhost:44351/api/FileAnalyzing/AnalyzeFile/";
        }

        public void FolderList()
        {
            IEnumerable<string> files = new List<string>();
            int fileCounter = 0;
            try
            {
                files = Directory.EnumerateFiles(FilePath, "*.*", SearchOption.AllDirectories)
                .Where(file => AllowedExtension.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList();
                fileCount = files.Count();
                
            }
            catch (Exception Ex)
            {
                Console.WriteLine("There is error about sourceFolder path. Please try control it");
                return;
            }


            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                JObject response = null;
                try
                {
                    response = JObject.Parse(FileUpload(file, fileName));
                }
                catch (Exception)
                {
                    continue;
                }
                
                fileCounter = fileCounter + 1;

                if (!(response is null))
                {

                    if (response.ContainsKey("Error"))
                    {

                        Console.WriteLine("----------------------------");
                        Console.WriteLine(fileName);
                        Console.WriteLine(response["Error"].ToString());
                        Console.WriteLine("----------------------------");

                    }
                    else
                    {
                        Console.WriteLine("----------------------------");
                        Console.WriteLine("Remain files to wait for analyze: {0}", fileCount - fileCounter);
                        Console.WriteLine("----------------------------");
                        Console.WriteLine(fileName);
                        Console.WriteLine(response["FileType"]);
                        Console.WriteLine(response["Verdict"]);
                        Console.WriteLine("----------------------------");
                    }
                }
            }
        }

        public string FileUpload(string file, string FileName)
        {
            int fileCounter = 0;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            content.Add(new StringContent("false"), "sanitization");
                            content.Add(new StringContent("true"), "isPublic");

                            var fileContent = new StreamContent(fileStream);
                            content.Add(fileContent, "file", FileName);



                            var response = client.PostAsync(URL, content).GetAwaiter().GetResult();
                            if (response.IsSuccessStatusCode)
                            {
                                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            }
                        }

                    }
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine("[*] There is a error while file uploading. File name is " + FileName);
                return null;
            }


            return null;
        }



    }
}
