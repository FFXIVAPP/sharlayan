// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Program.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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