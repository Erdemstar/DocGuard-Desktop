using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;


namespace DocGuard_Desktop.Class
{
    class DocFileUpload
    {
        #region Variable for initiliaze
        public string type { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string token { get; set; } = null;
        public string filePath { get; set; }
        public string folderPath { get; set; }
        public string Url { get; set; }
        public string outputPath { get; set; }
        public int threadSleep { get; set; }

        public Output opt = new Output();


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

        public DocFileUpload()
        {
            opt.succesOutput = new List<successOutput>();
            opt.errorOutput = new List<errorOutput>();
        }

        //
        public void Header()
        {
            //Before write someting to output.txt, delete output.txt which already created
            deleteAlreadyLogFile();

            //Write App name with date
            opt.appName = "Docguard File Sender  - " + DateTime.Now;

            //Write parametre which take from user
            if (!(folderPath is null))
            {
                opt.sourceFolder = folderPath;

            }
            else
            {
                opt.sourceFile = filePath;
            }
            opt.Url = Url;
            opt.outputPath = outputPath + "output.json";
            opt.threadSleepCount = threadSleep;
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

                    opt.fileCount = fileCount;


                }
                catch (Exception Ex)
                {
                    addError("There is error about sourceFolder path. Please try control it");
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

                    opt.fileCount = fileCount;

                }
                catch (Exception Ex)
                {
                    addError("There is error about sourceFile path. Please try control it");

                    return;
                }
            }

            //iterate all file and send it for upload
            foreach (var file in files)
            {
                successOutput so = new successOutput();

                var fileName = Path.GetFileName(file);
                readFileCount += 1;

                Thread.Sleep(TimeSpan.FromSeconds(threadSleep));

                JObject response = null;
                try
                {
                    response = JObject.Parse(FileUpload(file, fileName));
                }
                catch (Exception)
                {
                    addError("There is an error while parsing Json response name file : " + fileName);
                    errorCount += 1;
                    continue;
                }

                if (!(response is null))
                {
                    if (response.ContainsKey("Error"))
                    {
                        so.fileName = fileName;
                        so.errorMessage = response["Error"].ToString();
                        so.filePath = "";
                        so.fileType = "";
                        so.Verdict = "";
                        so.MD5 = "";

                        opt.succesOutput.Add(so);

                        errorCount += 1;
                    }
                    else if (response.ContainsKey("Verdict"))
                    {
                        so.fileName = fileName;
                        so.errorMessage = "";
                        so.filePath = file;
                        so.fileType = response["FileType"].ToString();
                        so.Verdict = response["Verdict"].ToString();
                        so.MD5 = response["FileMD5Hash"].ToString();

                        opt.succesOutput.Add(so);

                        verdictCount += 1;
                    }
                }
            }

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
                addError("There is a error while file uploading. File name is : " + FileName);

                errorCount += 1;
                return null;
            }

            return null;
        }

        //Avoid append same text to output.txt so that delete output.txt
        public void deleteAlreadyLogFile()
        {
            if (File.Exists(outputPath + "output.json"))
            {
                File.Delete(outputPath + "output.json");
            }

        }

        //Control threadSleep value and assingee _threadSleep
        public void threadSleepControl()
        {
            try
            {
                if (!(threadSleep >= 0) || !(threadSleep <= 61))
                {
                    threadSleep = 2;
                }
            }
            catch (Exception ex)
            {
                addError("There is a error on threadSleep");
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        //Generate Token
        public void getToken()
        {
            //Control email if email is null user is a anonymous
            if (!(email is null))
            {
                string resp = null;

                //Send request for loging and take response
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {

                            content.Add(new StringContent(email), "Username");
                            content.Add(new StringContent(email), "Email");
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
                    addError("There is an error while getting email : " + email);
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
                            addError("There is an error while parsing email : " + email);
                        }
                    }

                }

            }

        }

        //Start control
        public void Start()
        {

            if (type == "credentials")
            {
                opt.type = "credentials";
                Header();
                getToken();
                threadSleepControl();

                //If token is null , all file which upload will be public
                if (token != null)
                {
                    List();
                }

                Footer();
                Write();
            }
            else
            {
                opt.type = "anonymous";
                Header();
                threadSleepControl();
                List();
                Footer();
                Write();
            }

        }

        //take last status
        public void Footer()
        {
            opt.finish = "Docguard File Sender  - " + DateTime.Now;
            opt.successCount = verdictCount;
            opt.errorCount = errorCount;
        }

        //take error mesagge for output
        public void addError(string errorMessage)
        {
            errorOutput eo = new errorOutput();
            eo.errorMessage = errorMessage;
            opt.errorOutput.Add(eo);
        }

        //write json output
        public void Write()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(opt);
            File.WriteAllText(outputPath + "output.json", json);
        }

    }
}


