using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronizer.Application
{
    public class SettingLoader
    {
        public static Settings GetSettings()
        {
            var settings = new Settings();

            foreach (string x in File.ReadAllLines("Settings.ini"))
            {
                string[] data = x.Split('=');
                switch (data[0])
                {
                    case "DbConnection":
                        settings.DbConnectionString = data[1];
                        break;
                    case "CredentialPath":
                        settings.Credential = data[1];
                        break;
                    case "ExcelFilePath":
                        settings.ExcelFilePath = data[1];
                        break;
                    case "TimeSync":
                        settings.TimeSync = int.Parse(data[1]);
                        break;
                    case "LoggerFile":
                        settings.LogFilePath = data[1];
                        break;
                }
            }
            return settings;
        }
    }
}
