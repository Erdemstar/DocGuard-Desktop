using CommandLine;
using System;

namespace DocGuard_Desktop
{
    class Program
    {
        static void Main(string[] args)
        {

            DocFileUpload DFU = new DocFileUpload();

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.credentials == "true")
                       {
                           DFU.type = "credentials";

                           if (!(o.sourceFile is null))
                           {
                               DFU.email = o.email;
                               DFU.password = o.password;
                               DFU.filePath = o.sourceFile;
                               DFU.Url = o.destinationUrl;
                               DFU.outputPath = o.outputPath;
                               DFU.threadSleep = o.threadSleep;

                               DFU.Start();

                           }
                           else if (!(o.sourceFolder is null))
                           {
                               DFU.email = o.email;
                               DFU.password = o.password;
                               DFU.folderPath = o.sourceFolder;
                               DFU.Url = o.destinationUrl;
                               DFU.outputPath = o.outputPath;
                               DFU.threadSleep = o.threadSleep;

                               DFU.Start();
                           }
                           else
                           {
                               Console.Write("Bunlardan birtanesi girmek zorundasın");
                           }
                       }
                       else if (o.credentials == "false")
                       {
                           DFU.type = "anonymous";

                           if (!(o.sourceFile is null))
                           {
                               DFU.filePath = o.sourceFile;
                               DFU.Url = o.destinationUrl;
                               DFU.outputPath = o.outputPath;
                               DFU.threadSleep = o.threadSleep;

                               DFU.Start();

                           }
                           else if (!(o.sourceFolder is null))
                           {
                               DFU.folderPath = o.sourceFolder;
                               DFU.Url = o.destinationUrl;
                               DFU.outputPath = o.outputPath;
                               DFU.threadSleep = o.threadSleep;

                               DFU.Start();
                           }
                           else
                           {
                               Console.Write("Bunlardan birtanesi girmek zorundasın");
                           }
                       }
                   });



            #region test
            /*
            
            //Eger credentials kullanarak public olmadan göndermek isterse
            if (Array.Exists(args, arg => arg == "--credentials"))
            {
                DFU.type = "credentials";
                if (Array.Exists(args, arg => arg == "--sourceFolder"))
                {
                    DFU.username = args[2];
                    DFU.password = args[4];
                    DFU.folderPath = args[6];
                    DFU.Url = args[8];
                    DFU.outputPath = args[10];
                    DFU.threadSleep = args[12];

                    DFU.Start();

                }
                else if (Array.Exists(args, arg => arg == "--sourceFile"))
                {
                    DFU.username = args[2];
                    DFU.password = args[4];
                    DFU.filePath = args[6];
                    DFU.Url = args[8];
                    DFU.outputPath = args[10];
                    DFU.threadSleep = args[12];

                    DFU.Start();

                }
                else
                {
                    parametreHelp("credentials");
                }
            }
            //Eğer anonoymous bir şekilde dosya göndermek isterse
            else
            {
                DFU.type = "anonymous";

                if (Array.Exists(args, arg => arg == "--sourceFolder"))
                {
                    DFU.folderPath = args[2];
                    DFU.Url = args[4];
                    DFU.outputPath = args[6];
                    DFU.threadSleep = args[8];

                    DFU.Start();

                }
                else if (Array.Exists(args, arg => arg == "--sourceFile"))
                {
                    DFU.filePath = args[2];
                    DFU.Url = args[4];
                    DFU.outputPath = args[6];
                    DFU.threadSleep = args[8];

                    DFU.Start();

                }
                else
                {
                    parametreHelp("anonymous");
                }
            }
            
             */

            #endregion
        }

        static void baseHelp()
        {
            Console.WriteLine("[!] There is an error while giving arguman. We are waiting like bellow");
            Console.WriteLine(@"DocGuard_Desktop.exe --anonymous  / DocGuard_Desktop.exe --credentials ");
        }

        static void parametreHelp(string parametre)
        {
            if (parametre == "anonymous")
            {
                Console.WriteLine("[!] There is an error while giving arguman. We are waiting like bellow");
                Console.WriteLine(@"DocGuard_Desktop.exe --{0} --sourceFolder C:\TestPC\Mix --destinationUrl https://api.docguard.net:8443/api/FileAnalyzing/AnalyzeFile/ --outputPath C:\Users\TestPC\\DocGuard-Desktop\ --threadSleep 2", parametre);
                Console.WriteLine(@"DocGuard_Desktop.exe --{0} --sourceFile C:\TestPC\Mix\test.docx --destinationUrl https://api.docguard.net:8443/api/FileAnalyzing/AnalyzeFile/ --outputPath C:\Users\TestPC\\DocGuard-Desktop\ --threadSleep 2", parametre);
            }
            else
            {
                Console.WriteLine("[!] There is an error while giving arguman. We are waiting like bellow");
                Console.WriteLine(@"DocGuard_Desktop.exe --{0} --username --password --sourceFolder C:\TestPC\Mix --destinationUrl https://api.docguard.net:8443/api/FileAnalyzing/AnalyzeFile/ --outputPath C:\Users\TestPC\\DocGuard-Desktop\ --threadSleep 2", parametre);
                Console.WriteLine(@"DocGuard_Desktop.exe --{0} --username --password --sourceFile C:\TestPC\Mix\test.docx --destinationUrl https://api.docguard.net:8443/api/FileAnalyzing/AnalyzeFile/ --outputPath C:\Users\TestPC\\DocGuard-Desktop\ --threadSleep 2", parametre);
            }


        }
    }
}
