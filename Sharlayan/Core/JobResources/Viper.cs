// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Viper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources {
    using System;
    using Sharlayan.Core.Interfaces;
    using Sharlayan.Core.JobResources.Enums;

    public sealed class ViperResources : IJobResource {
        public byte RattlingCoilStacks { get; set; }

        public byte SerpentOffering { get; set; }

        public byte AnguineTribute { get; set; }

        public DreadCombo DreadCombo { get; set; }
      
        public TimeSpan Timer { get; set; }
    }
}