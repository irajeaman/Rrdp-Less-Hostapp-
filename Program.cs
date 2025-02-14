using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using CredentialManagement; // For handling credentials

class Program
{
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    public static extern bool BlockInput(bool block);
    private const int SW_HIDE = 0; // Hide window
    private const string logFilePath = @"C:\temp\irajeapp.txt"; // Path to the log file

    static void Main(string[] args)
    {
        // Hide the console window
        HideConsoleWindow();

        // Validate command-line arguments
        if (args.Length < 5)
        {
            Log("Usage: RdpFileGenerator <serverIP> <domain> <username> <password> <applicationName> [applicationArgs]");
            return;
        }

        string server = args[0];
        string domain = args[1];
        string username = args[2];
        string password = args[3];
        string applicationName = args[4];
        string applicationArgs = args.Length > 5 ? string.Join(" ", args, 5, args.Length - 5) : string.Empty;

        // Block user input before execution
        BlockInput(true);
        Log("User input has been blocked.");

        // Create output .rdp file path in C:\temp
        string outputFilePath = $@"C:\temp\{username}.rdp"; // Output .rdp file path

        // Define the .rdp file content for RemoteApp with keyboard shortcuts enabled
        string rdpContent = $@"
full address:s:{server}
alternate shell:s:||{applicationName}
remoteapplicationmode:i:1
remoteapplicationname:s:{applicationName}
remoteapplicationprogram:s:||{applicationName}
remoteapplicationcmdline:s:{applicationArgs}
screen mode id:i:2
use multimon:i:0
desktopwidth:i:1920
desktopheight:i:1080
session bpp:i:16
compression:i:1
keyboardhook:i:1
audiocapturemode:i:0
videoplaybackmode:i:1
connection type:i:2
displayconnectionbar:i:0
disable wallpaper:i:1
allow font smoothing:i:0
allow desktop composition:i:0
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:1
administrative session:i:0
redirectclipboard:i:1
redirectdrives:i:1
redirectposdevices:i:0
redirectdirectx:i:1
autoreconnection enabled:i:1
authentication level:i:2
prompt for credentials:i:0
negotiate security layer:i:1
promptcredentialonce:i:1
username:s:{domain}\{username}
networkautodetect:i:1
bandwidthautodetect:i:1
";

        try
        {
            // Write the .rdp file content
            File.WriteAllText(outputFilePath, rdpContent.Trim());
            Log($"Successfully created {outputFilePath}");

            // Save credentials using Credential Management API
            if (SaveCredentials(server, domain, username, password))
            {
                Log($"Saved credentials for {domain}\\{username} on server {server}.");
            }
            else
            {
                Log($"Failed to save credentials for {domain}\\{username} on server {server}.");
                return; // Exit if credentials saving fails
            }

            // Start the RDP session using rdp.exe or mstsc.exe
            Process rdpProcess = StartProcessWithFallback("rdp.exe", "mstsc.exe", outputFilePath);
            if (rdpProcess != null)
            {
                Log("RDP session started in the background.");

                // Wait for 2 seconds before performing any clean-up operations
                Thread.Sleep(2000);

                // Proceed to delete files and credentials
                Log("Proceeding to delete files and credentials...");

                // Delete the .rdp file after the RDP session has started
                DeleteFile(outputFilePath);
                DeleteCredentials(server);
            }
            else
            {
                Log("Failed to start RDP session.");
            }
        }
        catch (Exception ex)
        {
            Log($"Error: {ex.Message}");
        }
        finally
        {
            // Allow user input after execution
            BlockInput(false);
            Log("User input has been allowed.");
        }
    }

    private static bool SaveCredentials(string server, string domain, string username, string password)
    {
        try
        {
            using (var cred = new Credential())
            {
                cred.Target = $"TERMSRV/{server}";
                cred.Username = $"{domain}\\{username}";
                cred.Password = password;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
                return true;
            }
        }
        catch (Exception ex)
        {
            Log($"Error saving credentials: {ex.Message}");
            return false;
        }
    }

    private static void DeleteCredentials(string server)
    {
        try
        {
            var cred = new Credential { Target = $"TERMSRV/{server}" };
            cred.Delete();
            Log($"Deleted credentials for {server}");
        }
        catch (Exception ex)
        {
            Log($"Failed to delete credentials for {server}: {ex.Message}");
        }
    }

    private static void DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Log($"Deleted {filePath} after execution.");
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to delete {filePath}: {ex.Message}");
        }
    }

    private static Process StartProcessWithFallback(string primaryFileName, string fallbackFileName, string arguments)
    {
        Process process = null;
        try
        {
            process = StartProcess(primaryFileName, arguments);
            if (process == null)
            {
                Log($"{primaryFileName} not found. Trying {fallbackFileName}...");
                process = StartProcess(fallbackFileName, arguments);
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to start process with fallback: {ex.Message}");
        }
        return process;
    }

    private static Process StartProcess(string fileName, string arguments)
    {
        try
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            return process;
        }
        catch (Exception ex)
        {
            Log($"Failed to start process {fileName}: {ex.Message}");
            return null;
        }
    }

    private static void Log(string message)
    {
        try
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log message: {ex.Message}");
        }
    }

    private static void HideConsoleWindow()
    {
        IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
        ShowWindow(handle, SW_HIDE);
    }
}
