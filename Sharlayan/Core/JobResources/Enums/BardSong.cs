// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BardSong.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   BardSong.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.JobResources.Enums {
    public enum SongFlags : byte {
        None = 0,
        MagesBallad = 5,
        ArmysPaeon = 10,
        WanderersMinuet = 15,
        MagesBalladLastPlayed = 1 << 2,
        ArmysPaeonLastPlayed = 1 << 3,
        WanderersMinuetLastPlayed = MagesBalladLastPlayed | ArmysPaeonLastPlayed,
        MagesBalladCoda = 1 << 4,
        ArmysPaeonCoda = 1 << 5,
        WanderersMinuetCoda = 1 << 6
    }
    /*
    public enum SongFlags : byte {
        None = 0,
        MagesBallad = 1 << 0,
        ArmysPaeon = 1 << 1,
        WanderersMinuet = MagesBallad | ArmysPaeon,
        MagesBalladLastPlayed = 1 << 2,
        ArmysPaeonLastPlayed = 1 << 3,
        WanderersMinuetLastPlayed = MagesBalladLastPlayed | ArmysPaeonLastPlayed,
        MagesBalladCoda = 1 << 4,
        ArmysPaeonCoda = 1 << 5,
        WanderersMinuetCoda = 1 << 6
    }
    */
}