using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using Microsoft.Win32;
using IWshRuntimeLibrary;

namespace SimpleDogeInstaller
{
    internal class AppInstaller
    {
        public static void Install()
        {
            // Get the embedded resource
            Assembly assembly = Assembly.GetExecutingAssembly();

            string resourceName = "SimpleDogeInstaller.dist.sdw-release.zip";
            Stream resourceStream = assembly.GetManifestResourceStream(resourceName);


            // Assume 'zipStream' is your file stream
            using (var zip = new ZipArchive(resourceStream, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet", entry.FullName);
                    string directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (entry.Name == "")
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    else
                    {
                        Console.WriteLine("Installing... " + Path.GetFileName(filePath));
                        entry.ExtractToFile(filePath, true);
                    }
                }
            }


            Console.WriteLine("Creating StartMenu and Startup links...");
            // Create the shortcut
            WshShell shell = new WshShell();
            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SimpleDogeWallet\\Simple Ðoge Wallet.lnk";
            IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);

            // Set the shortcut properties
            shortcut.TargetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet\SimpleDogeWallet.exe");
            shortcut.WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet\");
            shortcut.Description = "Simple Doge Wallet";
            shortcut.IconLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet\Icon.ico");
            shortcut.Save();

            System.IO.File.Copy(shortcutAddress, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Simple Ðoge Wallet.lnk"), true);

            shortcut.Arguments = "-h";
            shortcut.Save();

            System.IO.File.Copy(shortcutAddress, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Simple Ðoge Wallet.lnk"), true);

        }
    }
}
