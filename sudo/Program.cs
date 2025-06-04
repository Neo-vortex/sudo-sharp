using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial class Program
{
    [LibraryImport("libc", SetLastError = true)]
    private static partial uint getuid();

    [LibraryImport("libc", SetLastError = true)]
    private static partial uint geteuid();

    [LibraryImport("libc", SetLastError = true)]
    private static partial int setuid(uint uid);

    static async Task Main(string[] args)
    {
        var getuidResult = getuid();
        var geteuidResult = geteuid();

        if (geteuidResult != 0)
        {
            var setuidResult = setuid(0);
            if (setuidResult != 0 || geteuid() != 0)
            {
                await Console.Error.WriteLineAsync("❌ Failed to escalate privileges to root.");
                await Console.Error.WriteLineAsync($"   UID: {getuidResult}, EUID: {geteuidResult}, setuid(0) returned {setuidResult}");
                Environment.Exit(1);
            }
        }
        setuid(0);
        var fileToExecute = args.Length > 0 ? args[0] : "/bin/bash";
        var arguments = args.Length > 1 ? string.Join(" ", args[1..]) : "";
        var startInfo = new ProcessStartInfo
        {
            FileName = fileToExecute,
            UseShellExecute = false,
            Arguments = arguments,
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
}