using CommandLine;
using System;
using DocGuard_Desktop.Class;

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

        }

    }
}
