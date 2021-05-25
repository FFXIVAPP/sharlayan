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