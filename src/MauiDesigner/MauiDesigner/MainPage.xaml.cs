using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace MauiDesigner;

public partial class MainPage : ContentPage
{
    private IDispatcherTimer timer;
    public const string CURRENT_VERSION = "0.1";
    
    public MainPage()
    {
        InitializeComponent();
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += (sender, e) => Navigation.PopAsync();

        CheckForUpdates();
    }

    private async void CheckForUpdates()
    {
        bool update = await DisplayAlert("Updates", "Er is een nieuwe update beschikbaar.\nWil je updaten?", "Ja", "Nee");

        if (!update) return;
        
        // Download update from localhost:8000
        HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8000")
        };

        OpenUrl("http://localhost:8000");
    }
    
    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            #if Windows
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            #elif Linux
                Process.Start("xdg-open", url);
            #elif MACCATALYST
                Process.Start("open", url);
            #else
                Console.WriteLine("Unsupported platform");
                throw;
            #endif
        }
    }

    private void getAllNodes(XmlNode node)
    {
        Console.WriteLine(node.Name);

        if (node.Name == "Button")
        {
            node.Attributes.Remove(node.Attributes["Clicked"]);
        }

        for (int i = 0; i < node.ChildNodes.Count; i++)
        {
            getAllNodes(node.ChildNodes[i]);
        }
    }

    private void Preview(object sender, EventArgs e)
    {
        string path = ((Editor)FindByName("Editor")).Text;
        string xaml = File.ReadAllText(path);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xaml);

        for (int i = 0; i < doc.ChildNodes.Count; i++)
        {
            getAllNodes(doc.ChildNodes[i]);
        }

        ContentPage btn = new ContentPage().LoadFromXaml(doc.OuterXml);
        Navigation.PushAsync(btn);
        
        timer.Start();
    }

    private async void BrowseFiles(object sender, EventArgs e)
    {
        try
        {
            var r = await FilePicker.PickAsync(PickOptions.Default);

            if (r != null)
            {
                var stream = r.FullPath;
                ((Editor)FindByName("Editor")).Text = stream;
            }
        }
        catch (Exception ex)
        {
            
        }
    }
}