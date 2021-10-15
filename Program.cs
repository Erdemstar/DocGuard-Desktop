using System;

namespace DocGuard_Desktop
{
    class Program
    {
        static void Main(string[] args)
        {

            
            if (args.Length != 2 || args[0] != "--sourceFolder")
            {
                Help();
                return;
            }
            
            DocFileUpload DFU = new DocFileUpload(args[1]);
            DFU.FolderList();

            //DocFileUpload DFU = new DocFileUpload(@"C:\Erdemstar\Arge\Office\Mix");
            //DFU.FolderList();

        }

        static void Help()
        {
            Console.WriteLine("[!] There is an error while giving arguman. Where are waiting like bellow");
            Console.WriteLine(@"cmd.exe : DocGuard_Desktop.exe --sourceFolder C:\Administrator\Desktop");

        }
    }
}
