using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDesigner;

public partial class CompilePage : ContentPage
{
    private Editor output;
    
    public CompilePage()
    {
        InitializeComponent();
        output = (Editor) FindByName("Output");
    }

    public void CompileTarget()
    {
        output.Text = "";
        string csProjPath = Singleton.projPath + "/MauiDesigner.csproj";
        Console.WriteLine(csProjPath);

        #if WINDOWS
            _ = BuildAndRunWin(csProjPath);
        #elif MACCATALYST
            _ = BuildAndRunOSX(csProjPath);
        #elif Linux
        #else
            Console.WriteLine("Unsupported platform");
            Application.Current.Quit();
        #endif
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        CompileTarget();
    }

    private async Task BuildAndRunOSX(string csProjPath)
    {
        var buildInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build " + csProjPath + " -f net7.0-maccatalyst -o /tmp/",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await Execute(buildInfo);
        
        Dispatcher.Dispatch(() => output.Text += "Running App ..." + Environment.NewLine);
        
        var runInfo = new ProcessStartInfo
        {
            FileName = "/Users/werk/Projects/maui-demo/MAUI DEMO/MAUI DEMO/bin/Debug/net7.0-maccatalyst/maccatalyst-arm64/MAUI DEMO.app/Contents/MacOS/MAUI DEMO",
            Arguments = "",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await Execute(runInfo);
    }

    private async Task BuildAndRunWin(string csProjPath)
    {
        string tempPath = Path.GetTempPath();
        var buildInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build " + csProjPath + " -f net7.0-windows10.0.19041.0 -o " + tempPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await Execute(buildInfo);

        Dispatcher.Dispatch(() => output.Text += "Running App ..." + Environment.NewLine);

        var runInfo = new ProcessStartInfo
        {
            FileName = "C:\\Users\\jan\\source\\repos\\MauiApp1\\MauiApp1\\bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\MauiApp1.exe",
            Arguments = "",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await Execute(runInfo);
    }

    private async Task Execute(ProcessStartInfo processStartInfo)
    {
        var process = new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                outputBuilder.AppendLine(args.Data);
                Dispatcher.Dispatch(() =>
                {
                    output.Text += args.Data + Environment.NewLine;
                });
            }
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                errorBuilder.AppendLine(args.Data);
                Dispatcher.Dispatch(() =>
                {
                    output.Text += args.Data + Environment.NewLine;
                });
            }
        };

        bool processExited = false;

        process.Exited += (sender, args) =>
        {
            processExited = true;
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    
        while (!processExited)
        {
            await Task.Delay(100); // Wait for the process to exit
        }
        
        process.WaitForExit();
    }
}
