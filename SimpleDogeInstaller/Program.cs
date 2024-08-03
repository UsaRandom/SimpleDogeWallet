using SimpleDogeInstaller;
using System.Diagnostics;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(@"
Simple Doge Wallet
v0.2.0 - Beta

A decentralized Dogecoin wallet.

Press [Enter] to Install...
");
        Console.ReadLine();
        Console.WriteLine("Installing VC++ 2015 Redist...");
        VcRedist2015Installer.Install();

        Console.WriteLine("Installing Simple Doge Wallet...");
        AppInstaller.Install();

        Console.WriteLine("Done!\n");


        string appExeLoc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet\SimpleDogeWallet.exe");
        
        ProcessStartInfo startInfo = new ProcessStartInfo(appExeLoc);
       
        startInfo.WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SimpleDogeWallet");

        Process.Start(startInfo);

        Console.WriteLine("Press [Enter] to Exit Installer...");
        Console.ReadLine();
    }
}