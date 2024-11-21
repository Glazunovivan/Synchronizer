using Microsoft.EntityFrameworkCore;
using Synchronizer.Database;
using Synchronizer.Models;

namespace Synchronizer.Application
{
    public class Synchronize
    {
        private readonly ApplicationContext _context;
        private readonly Settings _settings;
        private readonly Logger _logger;

        public Synchronize(Settings settings)
        {
            _context = new ApplicationContext(settings.DbConnectionString);
            _settings = settings;
            _logger = new Logger(_settings.LogFilePath);
        }

        public void Sync()
        {
            FistSync();
            ReSync();

            _context.SaveChanges();
        }

        /// <summary>
        /// Первичный залив стока
        /// </summary>
        private void FistSync()
        {
            var firstStockCreds = _context.StockCreds
                                  .Where(x => x.UpdateTime == null).ToList();

            foreach (var fStockCreds in firstStockCreds)
            {
                var user = _context.UserStocks.FirstOrDefault(x => x.Id == fStockCreds.UserId);

                IGetterData getter;
                if (fStockCreds.StockLink is not null)
                {
                    getter = new GoogleSheet(_settings, fStockCreds.StockLink, fStockCreds.ExelColumn);
                }
                else
                {
                    getter = new Excel(_settings.ExcelFilePath, fStockCreds.Id, fStockCreds.ExelColumn);
                }

                try
                {
                    var data = getter.GetData();
                    foreach (var result in data)
                    {
                        _context.UserStocks.Add(new Models.UserStock()
                        {
                            UserId = fStockCreds.UserId,
                            PartNumber = result,
                            UpdateTime = DateTime.UtcNow
                        });
                    }
                }
                catch (Exception ex)
                {
                    _context.ErrorsLog.Add(new Error_log
                    {
                        ScriptName = "Renew_stock",
                        ErrorMessage = ex.Message,
                        ErrorTime = DateTime.UtcNow,
                        UserId = fStockCreds.UserId,
                        StockId = fStockCreds.Id,
                        AdditionalData = string.Empty,
                        //JobId = null //<- что сюда передавать?
                    });
                }
            }
        }

        /// <summary>
        /// Повторная синхронизация стоков
        /// </summary>
        private void ReSync()
        {
            //данные для повторной синхронизации
            var stockCreds = _context.StockCreds
                             .Include(x => x.UserStock)
                             .Where(x => x.UpdateTime != null
                                      && x.UpdateTime.Value.AddSeconds(x.SincSwitch) >= DateTime.UtcNow)
                             .ToList();

            foreach (var stock in stockCreds)
            {
                int countUpdate = 0;
                int countDelete = 0;    
                
                var users = _context.UserStocks.Where(x => x.UserId == stock.UserId).ToList();

                string logString = string.Empty;    
                try
                {
                    var fileData = GetData(stock, stock.DocType);
                    foreach (var gd in fileData)
                    {
                        //в файле есть запись, но нет в бд
                        if (users.Where(x => x.PartNumber == gd).FirstOrDefault() is null)
                        {
                            _context.UserStocks.Add(new Models.UserStock()
                            {
                                UserId = stock.UserId,
                                PartNumber = gd,
                                UpdateTime = DateTime.UtcNow
                            });
                            countUpdate++;  
                        }
                    }
                    foreach (var dataInDb in users)
                    {
                        //есть запись в бд, но нет в файле, тогда удаляем
                        if (fileData.Where(a => a == dataInDb.PartNumber).FirstOrDefault() is null)
                        {
                            var valueInDb = _context.UserStocks.Where(x => x.PartNumber == dataInDb.PartNumber).FirstOrDefault();
                            _context.UserStocks.Remove(valueInDb);
                            countDelete++;
                        }
                    }

                    logString = Logger.GetStringForLog(DateTime.UtcNow, stock.UserId, stock.Id, "Ok", countUpdate, countDelete);
                    _logger.WriteToLog(logString);
                }
                catch (Exception ex)
                {
                    _context.ErrorsLog.Add(new Error_log
                    {
                        ScriptName = "Renew_stock",
                        ErrorMessage = ex.Message,
                        ErrorTime = DateTime.UtcNow,
                        UserId = stock.UserId,
                        StockId = stock.Id,
                        AdditionalData = string.Empty,
                        //JobId = null //<- что сюда передавать?
                    });

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
                var excel = new Excel(_settings.ExcelFilePath, stock.Id, stock.ExelColumn);
                return excel.GetData();
            }
            else
            {
                throw new Exception("Метод загрузки через Dropbox не реализован");
            }
        }
    }
}
