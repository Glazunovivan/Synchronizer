using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Synchronizer.Application
{
    public class Excel : IGetterData
    {
        private readonly string _filePath;
        private readonly string _cellName;
        private readonly int _stockId;
        private readonly string _folderPath;

        public Excel(string filePath, string folderPath, int stock_id,  string cellName)
        {
            _filePath = filePath;
            _cellName = cellName;
            _stockId = stock_id;
            _folderPath = folderPath;
        }

        public List<string> GetData()
        {
            IWorkbook workbook;
            var pathXls = $"{_filePath}/{_folderPath}/{_stockId}/stock.xls";
            var pathXlsx = $"{_filePath}/{_folderPath}/{_stockId}/stock.xlsx";

            if (File.Exists(pathXls) && File.Exists(pathXlsx))
            {
                throw new Exception($"Ошибка: в папке {_filePath}/{_folderPath}/{_stockId} находятся 2 файла.");
            }
            else if (File.Exists(pathXls) == false && File.Exists(pathXlsx) == false)
            {
                throw new Exception($"Ошибка: в папке {_filePath}/{_folderPath}/{_stockId} нет подходящего файла.");
            }

            //файл, который в итоге будем читать
            var filePathOk = File.Exists(pathXls) ? pathXls : pathXlsx;

            using (FileStream fileStream = new FileStream(filePathOk, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }

            ISheet sheet = workbook.GetSheetAt(0);

            var values = GetCellValueByName(workbook, sheet, _cellName);
            return values;
        }

        private List<string> GetCellValueByName(IWorkbook workbook, ISheet sheet, string cellName)
        {
            List<string> result = new List<string>();

            int columnIndex = GetColumnIndex(cellName);
            int rowIndex = GetRowIndex(cellName);

            if (rowIndex < 0 || rowIndex >= sheet.LastRowNum || columnIndex < 0 || columnIndex >= sheet.GetRow(rowIndex)?.LastCellNum)
            {
                return null;
            }

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null)
                {
                    ICell cell = row.GetCell(columnIndex);

                    result.Add(cell.ToString());
                }
                else
                {
                    result.Add(string.Empty);
                }
            }
            return result;
        }

        private int GetColumnIndex(string cellName)
        {
            string columnName = "";
            for (int i = 0; i < cellName.Length; i++)
            {
                if (char.IsLetter(cellName[i]))
                {
                    columnName += cellName[i];
                }
                else
                {
                    break;
                }
            }
            return columnName.ToUpperInvariant().ToColumnIndex();
        }

        private int GetRowIndex(string cellName)
        {
            string rowNumberStr = "";
            for (int i = 0; i < cellName.Length; i++)
            {
                if (char.IsDigit(cellName[i]))
                {
                    rowNumberStr += cellName[i];
                }
                else if (rowNumberStr.Length > 0)
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(rowNumberStr))
            {
                return -1;
            }

            return int.Parse(rowNumberStr) - 1;
        }

    }
}
