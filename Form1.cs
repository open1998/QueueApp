using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace QueueApp;

public partial class Form1 : Form
{
    public Form1()
    {
        try 
        {
            Console.WriteLine("Form1 Constructor Starting...");
            InitializeComponent();
            Console.WriteLine("InitializeComponent Finished.");

            // Check if index.html exists
            string hostPage = "wwwroot/index.html";
            if (!File.Exists(hostPage))
            {
                Console.WriteLine($"ERROR: {hostPage} NOT FOUND at {Path.GetFullPath(hostPage)}");
                MessageBox.Show($"Critical Error: {hostPage} not found!");
            }
            else
            {
                Console.WriteLine($"Verified: {hostPage} exists.");
            }

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            
            #if DEBUG
            Console.WriteLine("Adding Developer Tools...");
            services.AddBlazorWebViewDeveloperTools();
            #endif

            Console.WriteLine("Building Service Provider...");
            blazorWebView1.HostPage = hostPage;
            blazorWebView1.Services = services.BuildServiceProvider();
            
            Console.WriteLine("Adding Root Component <App>...");
            blazorWebView1.RootComponents.Add<App>("#app");
            
            blazorWebView1.BlazorWebViewInitialized += (s, e) => {
                e.WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                e.WebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                Console.WriteLine("WebView2 Core Initialized.");
            };
            
            Console.WriteLine("Blazor WebView Setup Complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"C# INIT ERROR: {ex.Message}");
            MessageBox.Show($"Initialization Error: {ex.Message}");
        }
    }
}
