// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResources.Dancer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResources.Dancer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.Structures {
    public partial class JobResources {
        public sealed class DancerResources {
            public int Esprit { get; set; }
            public int FourFoldFeathers { get; set; }
            public int Step1 { get; set; }
            public int Step2 { get; set; }
            public int Step3 { get; set; }
            public int Step4 { get; set; }
            public int StepIndex { get; set; }
        }
    }
}