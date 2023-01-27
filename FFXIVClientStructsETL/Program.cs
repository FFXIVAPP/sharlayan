// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - $CURRENT_YEAR$ Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Program.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;

using Sharlayan;
using Sharlayan.Utilities;

// Config up Sharlayan and grab latest current offsets
var configuration = new SharlayanConfiguration { UseLocalCache = false };
var structuresContainer = await APIHelper.GetStructures(configuration);

// ETL FFXIVClientStructs int Sharlayan format
var position = Marshal.OffsetOf(typeof(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject), "Position");
structuresContainer.ActorItem.X = (int)position;
structuresContainer.ActorItem.Y = (int)position + 8;
structuresContainer.ActorItem.Z = (int)position + 4;

// Write out JSON file
string region = configuration.GameRegion.ToString().ToLowerInvariant();
string patchVersion = configuration.PatchVersion;
string file = Path.Combine(configuration.JSONCacheDirectory, $"structures-{region}-{patchVersion}.json");
File.WriteAllText(file, JsonConvert.SerializeObject(structuresContainer, Formatting.Indented, new JsonSerializerSettings {
    NullValueHandling = NullValueHandling.Ignore,
    DefaultValueHandling = DefaultValueHandling.Populate,
}), Encoding.UTF8);