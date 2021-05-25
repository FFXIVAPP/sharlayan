namespace BootstrappedElectron {
    using ElectronNET.API;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program {
        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                webBuilder => {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                });
        }

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }
    }
}