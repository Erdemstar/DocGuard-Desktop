using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocGuard_Desktop
{
    class Options
    {
        [Option("credentials", Default = "false", HelpText = "--credentials true / --credentials false")]
        public string credentials { get; set; }

        [Option("email", Required = false, HelpText = "--email erdemstar@streamer.com")]
        public string email { get; set; }

        [Option("password", Required = false, HelpText = "--password streAmer")]
        public string password { get; set; }

        [Option("sourceFile", Required = false, HelpText = @"--sourceFile C:\User\Download\test.pdf")]
        public string sourceFile { get; set; }

        [Option("sourceFolder", Required = false, HelpText = @"--sourceFolder C:\User\Download")]
        public string sourceFolder { get; set; }

        [Option("destinationUrl", Required = true, HelpText = "--destinationUrl https://api.docguard.net:8443/")]
        public string destinationUrl { get; set; }

        [Option("outputPath", Required = true, HelpText = @"--outputPath C:\User\Download")]
        public string outputPath { get; set; }

        [Option("threadSleep", Required = true, HelpText = "--threadSleep 2")]
        public int threadSleep { get; set; }

        [Option("verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
