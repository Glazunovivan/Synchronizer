namespace Synchronizer.Application
{
    public class Logger
    {
        private string _filePath;

        public Logger(string filePath)
        {
                _filePath = filePath;
        }

        public static string GetStringForLog(DateTime dateTime, int userId, int stockId, string status, int countUpdated, int countDeleted)
        {
            return $"Time={dateTime.TimeOfDay};Script_name=Renew_Stock;UserId={userId};StockId={stockId};Status={status};Updated={countUpdated};Deleted={countDeleted}";
        }

        public void WriteToLog(string message)
        {
            using (StreamWriter sw = new StreamWriter(_filePath, true))
            {
                sw.WriteLine(message);
            }
        }
    }
}
