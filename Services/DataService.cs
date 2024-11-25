using Microsoft.EntityFrameworkCore;
using Synchronizer.Database;
using Synchronizer.Models;

namespace Synchronizer.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationContext _context;

        public DataService(string dbconnection)
        {
            _context = new ApplicationContext(dbconnection);
        }

        /// <summary>
        /// Получить стоки, которые нужно обновить
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public List<StockCreds> GetStocksCreds()
        {
            return _context.StockCreds
                            .Include(x => x.UserStock)
                            .Where(x => x.UpdateTime != null
                                     && x.UpdateTime.Value.AddSeconds(x.SincSwitch) >= DateTime.UtcNow)
                            .ToList();
        }

        /// <summary>
        /// Получить стоки, которые ни разу не заливались
        /// </summary>
        /// <returns></returns>
        public List<StockCreds> GetStockCredsFirstly()
        {
            return _context.StockCreds.Where(x => x.UpdateTime == null).ToList();
        }

        
        /// <summary>
        /// Получить пользователей
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public List<UserStock> GetUserStocks(int idUser)
        {
            return _context.UserStocks.Where(x => x.UserId == idUser).ToList();
        }

        public void AddDataUserStock(int userId, string partNumber, DateTime dateTime)
        {
            _context.UserStocks.Add(new Models.UserStock()
            {
                UserId = userId,
                PartNumber = partNumber,
                UpdateTime = dateTime
            });
        }

        public void RemoveDataUserStock(string partNumber)
        {
            var valueInDb = _context.UserStocks.Where(x => x.PartNumber == partNumber).FirstOrDefault();
            _context.UserStocks.Remove(valueInDb);
        }

        public void AddErrorLog(string message, int userId, int stockId)
        {
            _context.ErrorsLog.Add(new Error_log
            {
                ScriptName = "Renew_stock",
                ErrorMessage = message,
                ErrorTime = DateTime.UtcNow,
                UserId = userId,
                StockId = stockId,
                AdditionalData = string.Empty,
                //JobId = null //<- что сюда передавать?
            });
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
