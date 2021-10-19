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
        #region Variable for initiliaze
        public string type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; } = null;
        public string filePath { get; set; }
        public string folderPath { get; set; }
        public string Url { get; set; }
        public string outputPath { get; set; }
        public string threadSleep { get; set; }
        public int _threadSleep;
        #endregion

        #region Variable for file status
        public int fileCount = 0;
        public int readFileCount = 0;
        public int errorCount = 0;
        public int verdictCount = 0;
        #endregion

        #region Whilelist for file
        public string[] AllowedExtension = { ".hta", ".pdf", ".slk", ".csv", ".doc", ".dot", ".docx", ".docm", ".dotx", ".dotm",
                ".wll", ".xls", ".xll", ".xlw", ".xlt", ".xlsx", ".xlsm", ".xlsb", ".xlam", ".xltx", ".xltm", ".ppt", ".pps",
                ".pptx", ".pptm", ".ppsx", ".ppam", "ppa", ".rtf", ".bin", ".pub" };
        #endregion

        //
        public void Header()
        {
            //Before write someting to output.txt, delete output.txt which already created
            deleteAlreadyLogFile();

            //Write App name with date
            writeFile("[*] Docguard File Sender - " + DateTime.Now + "\n");

            //Write parametre which take from user
            if (!(folderPath is null))
            {
                writeFile("[*] Source folder : " + folderPath);

            }
            else
            {
                writeFile("[*] Source file : " + filePath);
            }
            writeFile("[*] URL : " + Url);
            writeFile("[*] Output path : " + outputPath + "output.txt");
            writeFile("[*] Thread will sleep : " + threadSleep + " seconds\n");
        }

        //List file and call FileUpload
        public void List()
        {
            IEnumerable<string> files = new List<string>();

            if (!(folderPath is null))
            {
                try
                {
                    files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => AllowedExtension.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList();

                    fileCount = files.Count();

                    writeFile("[*] There is/are : " + fileCount + " file/files to send\n");

                }
                catch (Exception Ex)
                {
                    writeFile("\n[!] There is error about sourceFolder path. Please try control it\n");
                    return;
                }
            }
            else if (!(filePath is null))
            {
                try
                {
                    IEnumerable<string> tmp = new List<string>();

                    files = tmp.Append(filePath)
                    .Where(file => AllowedExtension.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList();
                    fileCount = files.Count();

                    writeFile("[*] There is/are : " + fileCount + " file/files to send\n");

                }
                catch (Exception Ex)
                {
                    writeFile("\n[!] There is error about sourceFile path. Please try control it\n");
                    return;
                }
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

        }

        //
        public void Footer()
        {
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

                            if (!(token is null))
                            {
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                            }

                            var response = client.PostAsync(Url + "api/FileAnalyzing/AnalyzeFile/", content).GetAwaiter().GetResult();
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

        //Control threadSleep value and assingee _threadSleep
        public void threadSleepControl()
        {
            try
            {
                if (String.IsNullOrEmpty(threadSleep) || String.IsNullOrWhiteSpace(threadSleep))
                {
                    _threadSleep = 0;
                }
                else if (!Int32.TryParse(threadSleep, out _threadSleep))
                {
                    throw new ApplicationException("ID wasn't an integer");
                }
                if (!(_threadSleep >= 0) || !(_threadSleep <= 61))
                {
                    throw new ApplicationException("threadSleep value cannot be less than 60 seconds");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        //Generate Token
        public void getToken()
        {
            //Control username if username is null user is a anonymous
            if (!(username is null))
            {
                string resp = null;

                //Send request for loging and take response
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {

                            content.Add(new StringContent(username), "Username");
                            content.Add(new StringContent(username), "Email");
                            content.Add(new StringContent(password), "Password");
                            content.Add(new StringContent("false"), "RememberMe");

                            var response = client.PostAsync(Url + "Account/Login", content).GetAwaiter().GetResult();
                            if (response.IsSuccessStatusCode)
                            {
                                resp = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    writeFile("\t[!] There is an error while getting username : " + username + "\n");
                }

                //Control resp for token
                if (!(resp is null))
                {
                    if (resp.Contains("Token"))
                    {
                        try
                        {
                            token = JObject.Parse(resp)["Token"].ToString();
                        }
                        catch (Exception)
                        {
                            writeFile("\t[!] There is an error while parsing username : " + username + "\n");
                        }
                    }
                    else
                    {
                        writeFile("\t[!] There is an error username or password\n");
                    }
                }

            }

        }


        public void Start()
        {

            if(type == "credentials")
            {
                Header();
                getToken();
                threadSleepControl();

                // Burada null olana izin verirsem adamın tüm dosyaları public olarak upload edilecektir.
                if (token != null)
                {
                    List();
                }

                Footer();
            }
            else
            {
                Header();
                threadSleepControl();
                List();
                Footer();
            }

            


        }

    }
}


