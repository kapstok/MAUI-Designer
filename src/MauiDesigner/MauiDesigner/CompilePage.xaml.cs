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
        #if Windows
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        #elif MACCATALYST
            _ = BuildAndRunOSX();
        #elif Linux
        #else
            Console.WriteLine("Unsupported platform");
            throw;
        #endif
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        CompileTarget();
    }

    private async Task BuildAndRunOSX()
    {
        var buildInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build /Users/werk/projecten/maui-designer/src/MauiDesigner/MauiDesigner/MauiDesigner.csproj -f net7.0-maccatalyst",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        await ExecuteOSX(buildInfo);
        
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
        await ExecuteOSX(runInfo);
    }

    private async Task ExecuteOSX(ProcessStartInfo processStartInfo)
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
