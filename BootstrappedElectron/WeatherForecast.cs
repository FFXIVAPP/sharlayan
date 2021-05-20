// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherForecast.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   WeatherForecast.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron {
    using System;

    public class WeatherForecast {
        public DateTime Date { get; set; }

        public string Summary { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int) (this.TemperatureC / 0.5556);
    }
}