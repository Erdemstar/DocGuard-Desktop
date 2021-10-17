using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;

namespace DocGuard_Desktop
{
    class DocFileUpload
    {
        //Variable for initiliaze
        public string filePath { get; set; }
        public string Url { get; set; } //https://localhost:44351/api/FileAnalyzing/AnalyzeFile/
        public string outputPath { get; set; }
        public int _threadSleep;

        //Variable for file status
        public int fileCount = 0;
        public int readFileCount = 0;
        public int errorCount = 0;
        public int verdictCount = 0;

        //Whilelist for file
        public string[] AllowedExtension = { ".hta", ".pdf", ".slk", ".csv", ".doc", ".dot", ".docx", ".docm", ".dotx", ".dotm",
                ".wll", ".xls", ".xll", ".xlw", ".xlt", ".xlsx", ".xlsm", ".xlsb", ".xlam", ".xltx", ".xltm", ".ppt", ".pps",
                ".pptx", ".pptm", ".ppsx", ".ppam", "ppa", ".rtf", ".bin", ".pub" };

        public DocFileUpload(string filePath, string Url, string outputPath, string threadSleep)
        {
            this.filePath = filePath;
            this.Url = Url;
            this.outputPath = outputPath;

            try
            {
                if(String.IsNullOrEmpty(threadSleep) || String.IsNullOrWhiteSpace(threadSleep))
                {
                    _threadSleep = 0;
                }
                else if(!Int32.TryParse(threadSleep, out _threadSleep))
                {
                    throw new ApplicationException("ID wasn't an integer");
                }
                if(!(_threadSleep >= 0) || !(_threadSleep <= 61))
                {
                    throw new ApplicationException("threadSleep value cannot be less than 60 seconds");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            //Before write someting to output.txt, delete output.txt which already created
            deleteAlreadyLogFile();

            //Write App name with date
            writeFile("[*] Docguard File Sender - " + DateTime.Now + "\n");

            //Write parametre which take from user
            writeFile("[*] Source folder : " + filePath);
            writeFile("[*] URL : " + Url);
            writeFile("[*] Output path : " + outputPath + "output.txt");
            writeFile("[*] Thread will sleep : " + threadSleep + " seconds\n");

        }

        //List file and call FileUpload
        public void FolderList()
        {
            IEnumerable<string> files = new List<string>();

            //Read files destinationPath or finish program
            try
            {
                files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories)
                .Where(file => AllowedExtension.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList();
                fileCount = files.Count();

                writeFile("[*] There is/are : " + fileCount + " file/files to send\n");

            }
            catch (Exception Ex)
            {
                writeFile("\n[!] There is error about sourceFolder path. Please try control it\n");
                return;
            }

            //iterate all file and send it for upload
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                readFileCount += 1;
                Thread.Sleep(TimeSpan.FromSeconds(_threadSleep));

                writeFile("[*] " + fileCount + "/" + readFileCount);

                JObject response = null;
                try
                {
                    response = JObject.Parse(FileUpload(file, fileName));
                }
                catch (Exception)
                {
                    writeFile("\t[!] There is an error while parsing Json response name file : " + fileName + "\n");
                    errorCount += 1;
                    continue;
                }
                
                if (!(response is null))
                {
                    if (response.ContainsKey("Error"))
                    {
                        writeFile("\t[-] File Name : " + fileName);
                        writeFile("\t[-] Error message : " + response["Error"].ToString() + "\n");
                        errorCount += 1;
                    }
                    else if (response.ContainsKey("Verdict"))
                    {
                        writeFile("\t[+] File Name : " + fileName);
                        writeFile("\t[+] Full Path : " + file);
                        writeFile("\t[+] File type : " + response["FileType"].ToString());
                        writeFile("\t[+] Verdict : " + response["Verdict"].ToString());
                        writeFile("\t[+] MD5 : " + response["FileMD5Hash"].ToString() + "\n");
                        verdictCount += 1;
                    }
                }
            }

            //Sending process is finish write brief about sending
            writeFile("[*] All file is sent - " + DateTime.Now);
            writeFile("[*] Success Count : " + verdictCount);
            writeFile("[*] Error Count : " + errorCount);
        }

        //FileUpload
        public string FileUpload(string file, string FileName)
        {
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

                            var response = client.PostAsync(Url, content).GetAwaiter().GetResult();
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
                writeFile("\t[!] There is a error while file uploading. File name is : " + FileName + "\n");
                errorCount += 1;
                return null;
            }

            return null;
        }

        //Take data and write for output.txt
        public void writeFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(outputPath + "output.txt", true))
            {
                writer.WriteLine(message);
            }
        }

        //Avoid append same text to output.txt so that delete output.txt
        public void deleteAlreadyLogFile()
        {
            if (File.Exists(outputPath + "output.txt"))
            {
                File.Delete(outputPath + "output.txt");
            }

        }

    }
}
