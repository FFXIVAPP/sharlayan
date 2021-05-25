namespace BootstrappedWPF.Helpers {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using BootstrappedWPF.Models;
    using BootstrappedWPF.ViewModels;

    public static class SettingsHelper {
        public static void SaveChatCodes() {
            IEnumerable<XElement> xElements = AppViewModel.Instance.XChatCodes.Descendants().Elements("Code");
            XElement[] enumerable = xElements as XElement[] ?? xElements.ToArray();

            foreach (ChatCode chatCode in AppViewModel.Instance.ChatCodes) {
                XElement element = enumerable.FirstOrDefault(e => e.Attribute("Key")?.Value == chatCode.Code);

                string xKey = chatCode.Code;
                string xColor = chatCode.Color;
                string xDescription = chatCode.Description;

                List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

                keyValuePairs.Add(new KeyValuePair<string, string>(xKey, xColor));
                keyValuePairs.Add(new KeyValuePair<string, string>(xKey, xDescription));

                if (element is null) {
                    XMLHelper.SaveXMLNode(AppViewModel.Instance.XChatCodes, "Codes", "Code", xKey, keyValuePairs);
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

            AppViewModel.Instance.XChatCodes.Save(Path.Combine(AppViewModel.Instance.ConfigurationsPath, "ChatCodes.xml"));
        }
    }
}