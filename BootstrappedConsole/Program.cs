// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Program.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedConsole {
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    using NLog;
    using NLog.Config;

    class Program {
        static void Main(string[] args) {
            XElement nLogConfig = XElement.Load("./BootstrappedConsole.exe.nlog");
            StringReader stringReader = new StringReader(nLogConfig.ToString());

            using (XmlReader xmlReader = XmlReader.Create(stringReader)) {
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }

            AppContext.Instance.Initialize();

            Console.WriteLine("To exit this application press \"Enter\".");
            Console.ReadLine();
        }
    }
}