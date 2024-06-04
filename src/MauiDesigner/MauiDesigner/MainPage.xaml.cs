using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using System.Reflection;
using Microsoft.Maui.Layouts;

namespace MauiDesigner;

public partial class MainPage : ContentPage
{
    public string CURRENT_VERSION = "unset";
    private IDispatcherTimer timer;
    private Dock dock;

    public MainPage()
    {
        InitializeComponent();
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += (sender, e) => Refresh();

        CheckForUpdates();
        dock = new Dock();
        
        dock.OnCompile(timer.Stop);
        dock.OnGoBack(timer.Start);
    }

    private async void CheckForUpdates()
    {
        HttpClient updateChecker = new HttpClient();
        
        var version = new StreamReader(await FileSystem.OpenAppPackageFileAsync("version.txt"));
        CURRENT_VERSION = version.ReadToEnd();
        
        Label status = (Label)FindByName("UpdateStatus");

        try
        {
            var newestVersion =
                await updateChecker.GetStringAsync(new Uri("https://kapstok.github.io/MAUI-Designer/version.txt"));
            Console.WriteLine(newestVersion);
            Console.WriteLine(CURRENT_VERSION);

            if (newestVersion == CURRENT_VERSION)
            {
                status.Text = "De designer is up-to-date.";
                return;
            }
            
            status.Text = "Update beschikbaar. Herstart de designer voor de popup.";
            status.TextColor = Colors.Red;
            bool update = await DisplayAlert("Updates", "Er is een nieuwe update beschikbaar.\nWil je updaten?", "Ja",
                "Nee");

            if (!update) return;

            // Download update from localhost:8000
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:8000")
            };

            OpenUrl("http://localhost:8000");
        }
        catch (HttpRequestException e)
        {
            status.Text = "Fout: Niet gelukt om te updaten.";
            status.TextColor = Colors.Red;
            Console.WriteLine(e);
        }
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
        if (node.Name == "Button")
        {
            node.Attributes.Remove(node.Attributes["Clicked"]);
        }

        for (int i = 0; i < node.ChildNodes.Count; i++)
        {
            getAllNodes(node.ChildNodes[i]);
        }
    }

    private void Refresh()
    {
        string path = ((Editor)FindByName("Editor")).Text;
        string xaml = File.ReadAllText(path);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xaml);

        for (int i = 0; i < doc.ChildNodes.Count; i++)
        {
            getAllNodes(doc.ChildNodes[i]);
        }

        try
        {
            ContentPage toBeDisplayed = new ContentPage().LoadFromXaml(doc.OuterXml);
            AppShell.Current.CurrentItem = dock.Draw(toBeDisplayed);
        }
        catch (XamlParseException ex)
        {
            ((ContentPage)Shell.Current.CurrentPage).Title = "Error loading XAML. See terminal output for details.";
            Shell.SetBackgroundColor(this, Color.FromRgb(0xff, 0x0, 0x0));
            Console.WriteLine(ex);
        }
        timer.Start();        
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

        try
        {
            ContentPage toBeDisplayed = new ContentPage().LoadFromXaml(doc.OuterXml);
            Navigation.PushAsync(dock.Draw(toBeDisplayed));
        }
        catch (XamlParseException ex)
        {
            ((ContentPage)Shell.Current.CurrentPage).Title = "Error loading XAML. See terminal output for details.";
            Shell.SetBackgroundColor(this, Color.FromRgb(0xff, 0x0, 0x0));
            Console.WriteLine(ex);
        }
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
            Console.WriteLine(ex);
        }
    }
}