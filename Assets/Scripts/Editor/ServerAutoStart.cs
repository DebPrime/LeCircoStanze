using UnityEditor;
using System.Diagnostics;
using System.IO;

[InitializeOnLoad] // 1. Tells Unity to run this script as soon as the Editor opens
public class ServerAutoStart
{
    private static Process serverProcess = null;

    static ServerAutoStart()
    {
        // 2. Listen for when the user presses Play or Stop
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 3. Trigger actions based on entering or exiting Play mode
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            StartLocalServer();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            StopLocalServer();
        }
    }

    private static void StartLocalServer()
    {
        // 4. Find your 'Server' folder automatically from the project root
        string serverPath = Path.Combine(Directory.GetCurrentDirectory(), "Server");

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = "server.js",
            WorkingDirectory = serverPath,
            UseShellExecute = true, // 5. Opens a real terminal window so you can see logs
            CreateNoWindow = false
        };

        ProcessStartInfo startInfo2 = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = "seed.js",
            WorkingDirectory = serverPath,
            UseShellExecute = true, // 5. Opens a real terminal window so you can see logs
            CreateNoWindow = false
        };

        try
        {
            serverProcess = Process.Start(startInfo);
            serverProcess = Process.Start(startInfo2);
            UnityEngine.Debug.Log("Auto-Start: Node.js server opened successfully.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Auto-Start Failed: Ensure Node.js is installed. Error: " + e.Message);
        }
    }

    private static void StopLocalServer()
    {
        // 6. Automatically close the terminal window when you stop playing
        if (serverProcess != null && !serverProcess.HasExited)
        {
            try
            {
                serverProcess.Kill();
                serverProcess.Dispose();
                UnityEngine.Debug.Log("Auto-Stop: Node.js server shut down cleanly.");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning("Could not auto-close server process: " + e.Message);
            }
        }
    }
}
