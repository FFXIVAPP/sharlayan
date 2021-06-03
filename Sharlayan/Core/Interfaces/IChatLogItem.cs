// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChatLogItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IChatLogItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Interfaces {
    using System;

    public interface IChatLogItem {
        byte[] Bytes { get; set; }
        string Code { get; set; }
        string Combined { get; set; }
        bool IsInternational { get; set; }
        string Line { get; set; }
        string Message { get; set; }
        string PlayerName { get; set; }
        string Raw { get; set; }
        DateTime TimeStamp { get; set; }
    }
}