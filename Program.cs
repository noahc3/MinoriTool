using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace MinoriTool {
    class Program {
        static string gimconv = "gimconv/GimConv.exe";
        static DirectoryInfo inputDirectory = new DirectoryInfo("input");
        static DirectoryInfo outputDirectory = new DirectoryInfo("output");

        public static void Main() {
            if (!CanBegin()) {
                Console.WriteLine("Requirements not met, cannot continue.");
                Environment.Exit(1);
            } else {
                Console.WriteLine("Ready to begin, press any key to continue.");
                Console.ReadKey();
                Convert();
                Environment.Exit(0);
            }
        }

        public static bool CanBegin() {
            if (!inputDirectory.Exists) inputDirectory.Create();

            bool gunzipFound = IsGunzipAvailable();
            bool gimConvFound = IsGimConvAvailable();
            int gimToConvert = Directory.EnumerateFiles("input").Where(x => x.ToLower().EndsWith(".gim")).Count();

            Console.WriteLine("gunzip: " + (gunzipFound ? "OK" : "NOT FOUND"));
            Console.WriteLine("GimConv: " + (gimConvFound ? "OK" : "NOT FOUND"));
            Console.WriteLine("GIM files to convert: " + gimToConvert + " found");

            if (!gunzipFound) {
                Console.WriteLine("[ERROR] gunzip was not found! Ensure bash is accessible from commandline (ex. have WSL installed) and that gunzip is available within the default environment (check with bash -c \"which gunzip\" from Windows command prompt).");
            }

            if (!gimConvFound) {
                FileInfo fi = new FileInfo("gimconv/GimConv.exe");
                Console.WriteLine($"[ERROR] GimConv.exe not found! Ensure Sony's official GimConv tool is located at ./gimconv/GimConv.exe ({fi.FullName})");
            }

            if (gimToConvert <= 0) {
                Console.WriteLine($"[ERROR] No .gim files found to convert! Ensure they are located ./input/ ({inputDirectory.FullName})");
            }

            return gunzipFound && gimConvFound && gimToConvert > 0;
        }

        public static bool IsGunzipAvailable() {
            ProcessStartInfo psi = new ProcessStartInfo() {
                FileName = "bash",
                Arguments = "-c \"which gunzip\"",
                RedirectStandardOutput = true
            };
            Process proc = new Process() { StartInfo = psi };
            proc.Start();
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd().Count() > 1;
        }

        public static bool IsGimConvAvailable() {
            return File.Exists(gimconv);
        }

        public static void Convert() {

            Console.WriteLine("\nExtracting and/or copying to output directory\n");

            if (outputDirectory.Exists) outputDirectory.Delete(true);
            outputDirectory.Create();

            foreach (FileInfo originalFile in inputDirectory.EnumerateFiles().Where(x => x.Extension.ToLower().Equals(".gim"))) {
                bool isCompressed;
                byte[] sig = new byte[3];
                FileInfo outputFile;

                Console.WriteLine(originalFile.Name);

                using (FileStream originalStream = originalFile.OpenRead()) {
                    originalStream.Read(sig, 0, 3);
                    isCompressed = sig.SequenceEqual(new byte[] { 0x1f, 0x8b, 0x08 });
                }

                outputFile = new FileInfo("output/" + originalFile.Name + (isCompressed ? ".gz" : ""));
                originalFile.CopyTo(outputFile.FullName);

                if (isCompressed) {
                    ProcessStartInfo psi = new ProcessStartInfo() {
                        FileName = "bash",
                        Arguments = $"-c \"gunzip -d {outputFile.Name}\"",
                        WorkingDirectory = outputDirectory.FullName
                    };
                    Process p = new Process() { StartInfo = psi };
                    p.Start();
                    p.WaitForExit();
                }
            }

            Console.WriteLine("\nConverting to PNG\n");

            foreach (FileInfo gimFile in outputDirectory.EnumerateFiles().Where(x => x.Extension.ToLower().Equals(".gim"))) {
                ProcessStartInfo psi = new ProcessStartInfo() {
                    FileName = gimconv,
                    Arguments = gimFile.Name + " -o " + gimFile.Name + ".png",
                    WorkingDirectory = outputDirectory.FullName
                };
                Process process = new Process() { StartInfo = psi };
                process.Start();
                process.WaitForExit();
                gimFile.Delete();
            }

            Console.WriteLine($"Finished! PNG files can be found at {outputDirectory.FullName}.");
        }
    }
}
