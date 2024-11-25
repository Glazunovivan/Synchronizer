using Synchronizer.Models;

namespace Synchronizer.Services
{
    public interface IDataService
    {
        public List<StockCreds> GetStocksCreds();
        public List<StockCreds> GetStockCredsFirstly();
        public List<UserStock> GetUserStocks(int idUser);
        public void AddDataUserStock(int userId, string partNumber, DateTime dateTime);
        public void RemoveDataUserStock(string partNumber);

        public void AddErrorLog(string message, int userId, int stockId);
    }
}
