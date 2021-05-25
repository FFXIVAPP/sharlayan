// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SettingsHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedElectron.Helpers {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using BootstrappedElectron.Models;

    public static class SettingsHelper {
        public static void SaveChatCodes() {
            IEnumerable<XElement> xElements = AppConfig.Instance.XChatCodes.Descendants().Elements("Code");
            XElement[] enumerable = xElements as XElement[] ?? xElements.ToArray();

            foreach (ChatCode chatCode in AppConfig.Instance.ChatCodes) {
                XElement element = enumerable.FirstOrDefault(e => e.Attribute("Key")?.Value == chatCode.Code);

                string xKey = chatCode.Code;
                string xColor = chatCode.Color;
                string xDescription = chatCode.Description;

                List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

                keyValuePairs.Add(new KeyValuePair<string, string>(xKey, xColor));
                keyValuePairs.Add(new KeyValuePair<string, string>(xKey, xDescription));

                if (element is null) {
                    SaveXMLNode(AppConfig.Instance.XChatCodes, "Codes", "Code", xKey, keyValuePairs);
                }
                else {
                    XElement xColorElement = element.Element("Color");
                    if (xColorElement != null) {
                        xColorElement.Value = xColor;
                    }
                    else {
                        element.Add(new XElement("Color", xColor));
                    }

                    XElement xDescriptionElement = element.Element("Description");
                    if (xDescriptionElement != null) {
                        xDescriptionElement.Value = xDescription;
                    }
                    else {
                        element.Add(new XElement("Description", xDescription));
                    }
                }
            }

            AppConfig.Instance.XChatCodes.Save(Path.Combine(AppConfig.Instance.ConfigurationsPath, "ChatCodes.xml"));
        }

        private static void SaveXMLNode(XDocument xDoc, string xRoot, string xNode, string xKey, IEnumerable<KeyValuePair<string, string>> xValuePairs) {
            XElement element = xDoc.Element(xRoot);
            if (element == null) {
                return;
            }

            XElement newElement = new XElement(xNode, new XAttribute("Key", xKey));
            foreach (KeyValuePair<string, string> s in xValuePairs) {
                newElement.Add(new XElement(s.Key, s.Value));
            }

            element.Add(newElement);
        }
    }
}