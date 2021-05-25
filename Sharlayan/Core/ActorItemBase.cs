// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorItemBase.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ActorItemBase.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core {
    using System;
    using System.Collections.Generic;

    using Sharlayan.Core.Enums;

    public class ActorItemBase {
        private string _name;

        public Coordinate Coordinate { get; set; }

        public short CPCurrent { get; set; }

        public short CPMax { get; set; }

        public double CPPercent => this.safeDivide(this.CPCurrent, this.CPMax);

        public string CPString => $"{this.CPCurrent}/{this.CPMax} [{this.CPPercent:P2}]";

        public List<EnmityItem> EnmityItems { get; protected set; } = new List<EnmityItem>();

        public short GPCurrent { get; set; }

        public short GPMax { get; set; }

        public double GPPercent => this.safeDivide(this.GPCurrent, this.GPMax);

        public string GPString => $"{this.GPCurrent}/{this.GPMax} [{this.GPPercent:P2}]";

        public float HitBoxRadius { get; set; }

        public int HPCurrent { get; set; }

        public int HPMax { get; set; }

        public double HPPercent => this.safeDivide(this.HPCurrent, this.HPMax);

        public string HPString => $"{this.HPCurrent}/{this.HPMax} [{this.HPPercent:P2}]";

        public uint ID { get; set; }

        public Actor.Job Job { get; set; }

        public byte JobID { get; set; }

        public byte Level { get; set; }

        public int MPCurrent { get; set; }

        public int MPMax { get; set; }

        public double MPPercent => this.safeDivide(this.MPCurrent, this.MPMax);

        public string MPString => $"{this.MPCurrent}/{this.MPMax} [{this.MPPercent:P2}]";

        public string Name {
            get => this._name ?? string.Empty;
            set => this._name = value;
        }

        public List<StatusItem> StatusItems { get; protected set; } = new List<StatusItem>();

        public string UUID { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public float GetCastingDistanceTo(ActorItem compare) {
            float distance = this.GetHorizontalDistanceTo(compare) - compare.HitBoxRadius - this.HitBoxRadius;
            return distance > 0
                       ? distance
                       : 0;
        }

        public float GetDistanceTo(ActorItem compare) {
            float distanceX = (float) Math.Abs(compare.X - this.X);
            float distanceY = (float) Math.Abs(compare.Y - this.Y);
            float distanceZ = (float) Math.Abs(compare.Z - this.Z);
            return (float) Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }

        public float GetHorizontalDistanceTo(ActorItem compare) {
            float distanceX = (float) Math.Abs(compare.X - this.X);
            float distanceY = (float) Math.Abs(compare.Y - this.Y);
            return (float) Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        private double safeDivide(double a, double b) {
            try {
                if (b == 0) {
                    return 0;
                }

                return a / b;
            }
            catch {
                // due to multithreading, sometimes b can be set to 0 between the check and the division
                return 0;
            }
        }
    }
}