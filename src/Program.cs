using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Diagnostics;

namespace TinyCratePackMgr
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.config"))) {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.config"), "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<configuration>\r\n  <appSettings>\r\n    <add key=\"installed_apps\" value=\"\" />\r\n  </appSettings>\r\n</configuration>");
            }
            if(!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Programs")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Programs"));
            }
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp"));
            }

            if (args.Length == 0) {
                Console.WriteLine("Requires command and target(s)");
                Console.WriteLine("Usage: TinyCratePackMgr <command> [targets]");
                Console.WriteLine("Commands:\r\n    install <ApplicationName>   Install the application.\r\n    remove <ApplicationName>    Remove the application.\r\n    run <ApplicationName>       Run the application.\r\n    help                        Show this help message.\r\n    version                     Show the version information.\r\n    exit                        Exit the package manager.\r\n    mkcab <source_dir> <cab_pack>   Create a .cab file from the source directory.\r\n    list                        List all installed applications.\r\n    info <ApplicationName>      Show information about the application.\r\n    sysinfo                     Show system information.\r\n    set_repo <repository_url>   Set the application repository URL.\r\n\r\n    Application package update not supported\r\n");
                return;
            }
            if (args.Length == 1) {
                string command = args[0].ToLower();
                switch (command) {
                    case "help":
                        Console.WriteLine("Welcome to TinyCrate Package Manager Help");
                        Console.WriteLine("Usage: TinyCratePackMgr <command> [targets]");
                        Console.WriteLine("Commands:\r\n    install <ApplicationName>   Install the application.\r\n    remove <ApplicationName>    Remove the application.\r\n    run <ApplicationName>       Run the application.\r\n    help                        Show this help message.\r\n    version                     Show the version information.\r\n    exit                        Exit the package manager.\r\n    mkcab <source_dir> <cab_pack>   Create a .cab file from the source directory.\r\n    list                        List all installed applications.\r\n    info <ApplicationName>      Show information about the application.\r\n    sysinfo                     Show system information.\r\n    set_repo <repository_url>   Set the application repository URL.\r\n\r\n    Application package update not supported\r\n");
                        return;
                    case "version":
                        Console.WriteLine("TinyCrate Package Manager Version 0.1.0 stable");
                        return;
                    case "list":
                        try {
                            var config = new ExeConfigurationFileMap();
                            config.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.config");
                            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(config, ConfigurationUserLevel.None);
                            string apps = configuration.AppSettings.Settings["installed_apps"]?.Value;
                            if(!string.IsNullOrEmpty(apps)) {
                                Console.WriteLine("Installed applications:"+apps);
                            } else {
                                Console.WriteLine("No applications installed.");
                            }
                        }
                        catch(Exception ex) {
                            Console.WriteLine("Error reading installed applications: " + ex.Message);

                        }
                        return;
                    case "sysinfo":
                        OperatingSystem os = Environment.OSVersion;
                        if(os.Platform == PlatformID.Win32NT) {
                            Console.WriteLine("Platform: Windows NT");
                            if (os.Version.Major == 10)
                            {
                                Console.WriteLine("Windows Version: Windows 10 / Windows 11");
                            }
                            else if (os.Version.Major == 6 && os.Version.Minor == 3)
                            {
                                Console.WriteLine("Windows Version: Windows 8.1");
                            }
                            else if (os.Version.Major == 6 && os.Version.Minor == 2)
                            {
                                Console.WriteLine("Windows Version: Windows 8");
                            }
                            else if (os.Version.Major == 6 && os.Version.Minor == 1)
                            {
                                Console.WriteLine("Windows Version: Windows 7");
                            }
                            else if (os.Version.Major == 6 && os.Version.Minor == 0)
                            {
                                Console.WriteLine("Windows Version: Windows Vista");
                            }
                            else if (os.Version.Major == 5 && os.Version.Minor == 1)
                            {
                                Console.WriteLine("Windows Version: Windows XP");
                            }
                            else if (os.Version.Major == 5 && os.Version.Minor == 0)
                            {
                                Console.WriteLine("Windows Version: Windows 2000");
                            }
                            else
                            {
                                Console.WriteLine("Windows Version: Unknown");
                            }
                            } else if(os.Platform == PlatformID.Unix) {
                            Console.WriteLine("Platform: Unix");
                        } else if(os.Platform == PlatformID.MacOSX) {
                            Console.WriteLine("Platform: Mac OS X");
                        } else {
                            Console.WriteLine("Platform: Unknown");
                        }
                        if (Environment.OSVersion != null) {
                            Console.WriteLine("Operating System: " + Environment.OSVersion.ToString());
                        } else {
                            Console.WriteLine("Operating System: Unknown");
                        }
                        Console.WriteLine("Machine Name: " + Environment.MachineName);
                        Console.WriteLine("User Name: " + Environment.UserName);
                        Console.WriteLine("Memory Size: " + (Environment.WorkingSet / (1024 * 1024)) + " MB");
                        Console.WriteLine("CPU Count: " + Environment.ProcessorCount);
                        return;
                    case "exit":
                        Environment.Exit(0);
                        return;
                    default:
                        Console.WriteLine("Requires command and target(s)");
                        Console.WriteLine("Usage: TinyCratePackMgr <command> [targets]");
                        Console.WriteLine("Commands:\r\n    install <ApplicationName>   Install the application.\r\n    remove <ApplicationName>    Remove the application.\r\n    run <ApplicationName>       Run the application.\r\n    help                        Show this help message.\r\n    version                     Show the version information.\r\n    exit                        Exit the package manager.\r\n    mkcab <source_dir> <cab_pack>   Create a .cab file from the source directory.\r\n    list                        List all installed applications.\r\n    info <ApplicationName>      Show information about the application.\r\n    sysinfo                     Show system information.\r\n    set_repo <repository_url>   Set the application repository URL.\r\n\r\n    Application package update not supported\r\n");
                        return;
                }
            }
            if(args.Length == 2)
            {
                string command = args[0].ToLower();
                string target = args[1];
                switch(command)
                {
                    case "install":
                        Console.WriteLine("Installing application: " + target);
                        //待会再说
                        return;
                    case "remove":
                        Console.WriteLine("Removing application: " + target);
                        //待会再说
                        return;
                    case "run":
                        Console.WriteLine("Running application: " + target);
                        string  appname = target;
                        string basedir = AppDomain.CurrentDomain.BaseDirectory;
                        string programdir = Path.Combine(basedir, "Programs");
                        string app_bat_p = Path.Combine(programdir, appname);
                        string app_bat_f = Path.Combine(app_bat_p, "start.bat");
                        //".\\Programs\\" + appname + "\\start.bat";
                        if (Directory.Exists(app_bat_p))
                        {
                            if (File.Exists(app_bat_f))
                            {
                                try
                                {
                                    ProcessStartInfo startInfo = new ProcessStartInfo(app_bat_f)
                                    {
                                        WorkingDirectory = app_bat_p,
                                        UseShellExecute = true
                                    };
                                    Process proc = Process.Start(startInfo);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error starting application: " + ex.Message);
                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Application start script not found: " + app_bat_f);

                            }
                        }
                        else
                        {
                            Console.WriteLine("Application not installed: " + appname);
                        }
                        return;
                    case "info":
                        Console.WriteLine("Showing information for application: " + target);
                        //待会再说
                        return;
                    case "set_repo":
                        Console.WriteLine("Setting repository URL to: " + target);
                        //待会再说
                        return;
                    default:
                        Console.WriteLine("Unknown command with provided argument.");
                        return;
                }
            }

            if (args.Length == 3)
            {
                string command = args[0].ToLower();
                string target1 = args[1];
                string target2 = args[2];
                switch(command)
                {
                    case "mkcab":
                        Console.WriteLine("Creating .cab file from directory: " + target1);
                        //待会再说
                        return;
                    default:
                        Console.WriteLine("Unknown command with provided arguments.");
                        return;
                }

            }
            if(args.Length > 3)
            {
                Console.WriteLine("Too many arguments provided.");
                return;
            }

        }
    }
}
