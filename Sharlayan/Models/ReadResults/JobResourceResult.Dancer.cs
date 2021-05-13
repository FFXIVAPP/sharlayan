// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResourceResult.Dancer.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   JobResourceResult.Dancer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    public partial class JobResourceResult {
        public sealed class DancerResources {
            private readonly DanceStep[] _steps;

            internal DancerResources(JobResourceResult result) {
                this.FourFoldFeathers = result.Data[result.Offsets.Dancer.FourFoldFeathers];
                this.Esprit = result.Data[result.Offsets.Dancer.Esprit];
                this.StepIndex = result.Data[result.Offsets.Dancer.StepIndex];
                this._steps = new[] {
                    (DanceStep) result.Data[result.Offsets.Dancer.Step1],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step2],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step3],
                    (DanceStep) result.Data[result.Offsets.Dancer.Step4],
                };
            }

            public enum DanceStep : byte {
                Finish,

                Emboite,

                Entrechat,

                Jete,

                Pirouette,
            }

            public DanceStep CurrentStep => this.Steps[this.StepIndex];
            public int Esprit { get; }
            public int FourFoldFeathers { get; }
            public int StepIndex { get; }

            public DanceStep[] Steps {
                get {
                    if (this._steps[2] > 0) {
                        return new[] {
                            this._steps[0],
                            this._steps[1],
                            this._steps[2],
                            this._steps[3],
                            (DanceStep) 0,
                        };
                    }

                    return new[] {
                        this._steps[0],
                        this._steps[1],
                        (DanceStep) 0,
                    };
                }
            }
        }
    }
}