// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChatLogItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
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

        bool JP { get; set; }

        string Line { get; set; }

        string Raw { get; set; }

        DateTime TimeStamp { get; set; }
    }
}