namespace Synchronizer.Application
{
    public class Settings
    {
        public string DbConnectionString { get; set; }  
        public string Credential { get; set; }
        public string ExcelFilePath { get; set; }
        public int TimeSync { get; set; }

        public string LogFilePath { get; set; }
    }
}
