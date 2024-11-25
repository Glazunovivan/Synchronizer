using Synchronizer.Database;
using Synchronizer.Models;
using Synchronizer.Services;

namespace Synchronizer.Application
{
    public class Synchronize
    {
        private readonly DataService _dataService;
        private readonly Settings _settings;
        private readonly Logger _logger;

        public Synchronize(Settings settings)
        {
            _settings = settings;
            _logger = new Logger(_settings.LogFilePath);
            _dataService = new DataService(_settings.DbConnectionString);
        }

        public void Sync()
        {
            FistSync();
            ReSync();

            _dataService.SaveChanges();
        }

        /// <summary>
        /// Первичный залив стока
        /// </summary>
        private void FistSync()
        {
            var firstStockCreds = _dataService.GetStockCredsFirstly();

            foreach (var fStockCreds in firstStockCreds)
            {
                var user = _dataService.GetUserStocks(fStockCreds.UserId);

                IGetterData getter = fStockCreds.StockLink is not null ? 
                                     new GoogleSheet(_settings, fStockCreds.StockLink, fStockCreds.ExelColumn) :
                                     new Excel(_settings.ExcelFilePath, fStockCreds.StockLink, fStockCreds.Id, fStockCreds.ExelColumn);
              
                try
                {
                    var partNumbers = getter.GetData();
                    foreach (var partNumber in partNumbers)
                    {
                        _dataService.AddDataUserStock(fStockCreds.UserId, partNumber, DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _dataService.AddErrorLog(ex.Message, fStockCreds.UserId, fStockCreds.Id);
                }
            }
        }

        /// <summary>
        /// Повторная синхронизация стоков
        /// </summary>
        private void ReSync()
        {
            //данные для повторной синхронизации
            var stockCreds = _dataService.GetStocksCreds();

            foreach (var stock in stockCreds)
            {
                int countUpdate = 0;
                int countDelete = 0;    
                
                var users = _dataService.GetUserStocks(stock.UserId);

                string logString = string.Empty;    
                try
                {
                    var fileData = GetData(stock, stock.DocType);
                    foreach (var gd in fileData)
                    {
                        //в файле есть запись, но нет в бд
                        if (users.Where(x => x.PartNumber == gd).FirstOrDefault() is null)
                        {
                            _dataService.AddDataUserStock(stock.UserId, gd, DateTime.UtcNow);
                            countUpdate++;  
                        }
                    }
                    foreach (var dataInDb in users)
                    {
                        //есть запись в бд, но нет в файле, тогда удаляем
                        if (fileData.Where(a => a == dataInDb.PartNumber).FirstOrDefault() is null)
                        {
                            _dataService.RemoveDataUserStock(dataInDb.PartNumber);
                            countDelete++;
                        }
                    }

                    logString = Logger.GetStringForLog(DateTime.UtcNow, stock.UserId, stock.Id, "Ok", countUpdate, countDelete);
                    _logger.WriteToLog(logString);
                }
                catch (Exception ex)
                {
                    _dataService.AddErrorLog(ex.Message, stock.UserId, stock.Id);

                    logString = Logger.GetStringForLog(DateTime.UtcNow, stock.UserId, stock.Id, "Error", countUpdate, countDelete);
                    _logger.WriteToLog(logString);
                }
            }
        }

        /// <summary>
        /// Получаем данные из источника по DocType
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<string> GetData(StockCreds stock, int docType)
        {
            if (stock.DocType == 2)
            {
                var google = new GoogleSheet(_settings, stock.StockLink, stock.ExelColumn);
                return google.GetData();
            }
            else if (stock.DocType == 3)
            {
                var excel = new Excel(_settings.ExcelFilePath, stock.StockLink, stock.Id, stock.ExelColumn);
                return excel.GetData();
            }
            else
            {
                throw new Exception("Метод загрузки через Dropbox не реализован");
            }
        }
    }
}
