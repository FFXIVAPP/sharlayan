// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Constants.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using Newtonsoft.Json;

    internal static class Constants {
        public const string DEFAULT_API_BASE_URL = "https://raw.githubusercontent.com/FFXIVAPP/sharlayan-resources/master";

        public static readonly string UNKNOWN_LOCALIZED_NAME = "???";

        public static readonly string[] ChatAlliance = {
            "000F",
        };

        public static readonly string[] ChatCWLS = {
            "0025",
            "0026",
            "0027",
            "0028",
            "0029",
            "002A",
            "002B",
            "002C",
        };

        public static readonly string[] ChatFC = {
            "0018",
        };

        public static readonly string[] ChatLS = {
            "0010",
            "0011",
            "0012",
            "0013",
            "0014",
            "0015",
            "0016",
            "0017",
        };

        public static readonly string[] ChatNovice = {
            "001B",
        };

        public static readonly string[] ChatParty = {
            "000E",
        };

        public static readonly string[] ChatPublic = {
            "000A",
            "000B",
            "000C",
            "000D",
            "000E",
            "000F",
            "0010",
            "0011",
            "0012",
            "0013",
            "0014",
            "0015",
            "0016",
            "0017",
            "0018",
            "001B",
            "001E",
            "0025",
            "0026",
            "0027",
            "0028",
            "0029",
            "002A",
            "002B",
            "002C",
        };

        public static readonly string[] ChatSay = {
            "000A",
        };

        public static readonly string[] ChatShout = {
            "000B",
        };

        public static readonly string[] ChatTell = {
            "000C",
            "000D",
        };

        public static readonly string[] ChatYell = {
            "001E",
        };

        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
        };
    }
}