using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronizer.Application
{
    public class Excel : IGetterData
    {
        private readonly string _filePath;
        private readonly string _cellName;
        private readonly int _stockId;

        public Excel(string filePath, int stock_id,  string cellName)
        {
            _filePath = filePath;
            _cellName = cellName;
            _stockId = stock_id;
        }

        public List<string> GetData()
        {
            IWorkbook workbook;
            var pathXls = $"{_filePath}/user_stocks/{_stockId}/stock.xls";
            var pathXlsx = $"{_filePath}/user_stocks/{_stockId}/stock.xlsx";

            if (File.Exists(pathXls) && File.Exists(pathXlsx))
            {
                throw new Exception($"Ошибка: в папке {_filePath}/user_stocks/{_stockId} находятся 2 файла.");
            }
            else if (File.Exists(pathXls) == false && File.Exists(pathXlsx) == false)
            {
                throw new Exception($"Ошибка: в папке {_filePath}/user_stocks/{_stockId} нет подходящего файла.");
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


        //private object GetCellValue(ICell cell)
        //{
        //    if (cell == null) return null;
        //    switch (cell.CellType)
        //    {
        //        case CellType.Numeric:
        //            if (DateUtil.IsCellDateFormatted(cell)) return cell.DateCellValue;
        //            return cell.NumericCellValue;
        //        case CellType.String: return cell.StringCellValue;
        //        case CellType.Boolean: return cell.BooleanCellValue;
        //        case CellType.Formula: return cell.CellFormula;
        //        case CellType.Blank: return null;
        //        default: return cell.ToString();
        //    }
        //}
    }
}
