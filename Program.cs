namespace QueueApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        Console.WriteLine("Starting QueueApp in Development Mode...");

        try 
        {
            Console.WriteLine("Initializing Database...");
            QueueApp.Data.DatabaseContext.Initialize();
            Console.WriteLine("Database Initialized.");

            ApplicationConfiguration.Initialize();
            Console.WriteLine("Application Configuration Initialized. Starting Window...");
            Application.Run(new Form1());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            MessageBox.Show($"Startup Error: {ex.Message}");
        }
    }
    
}