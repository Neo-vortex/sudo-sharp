using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sudo;

internal static partial class Program
{
    [LibraryImport("libc", SetLastError = true)]
    private static partial uint getuid();

    [LibraryImport("libc", SetLastError = true)]
    private static partial uint geteuid();

    [LibraryImport("libc", SetLastError = true)]
    private static partial int setuid(uint uid);
    static async Task Main(string[] args)
    {
        setuid(0);
        var getuidResult = getuid();
        var geteuidResult = geteuid();
        if (geteuidResult != 0)
        {
            Console.WriteLine($"⚠ Current UID: {getuidResult}, EUID: {geteuidResult}");
            Console.WriteLine("⚠ Attempting privilege escalation...");
            
            var setuidResult = setuid(0);
            if (setuidResult != 0 || geteuid() != 0)
            {
                await Console.Error.WriteLineAsync("❌ Failed to escalate privileges to root.");
                await Console.Error.WriteLineAsync($"UID: {getuidResult}, EUID: {geteuidResult}, setuid(0) returned {setuidResult}");
                Environment.Exit(1);
            }
            
            Console.WriteLine("✅ Successfully escalated privileges");
            Console.WriteLine($"New UID: {getuid()}, EUID: {geteuid()}");
        }
        var startInfo = new ProcessStartInfo
        {
            FileName = args.Length == 0  ?  "/bin/bash" : args[0],
            Arguments = args.Length == 0 ? string.Empty : string.Join(" ", args[1..]),
            UseShellExecute = false,
            RedirectStandardInput = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = false
        };

        var process = new Process { StartInfo = startInfo };
        
        try
        {
            process.Start();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string GetDefaultShell()
    {
        const string bashPath = "/bin/bash";
        const string shPath = "/bin/sh";
        
        if (File.Exists(bashPath))
            return bashPath;
        
        if (File.Exists(shPath))
            return shPath;
            
        throw new Exception("Neither /bin/bash nor /bin/sh were found");
    }

}