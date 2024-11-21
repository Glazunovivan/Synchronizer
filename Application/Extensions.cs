using System.Text.RegularExpressions;

namespace Synchronizer.Application
{
    public static class Extensions
    {
        /// <summary>
        /// Получения индекса столбца из имени столбца (A, B, AA, AB, ...)
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int ToColumnIndex(this string columnName)
        {
            int index = 0;
            foreach (char c in columnName.ToUpperInvariant())
            {
                index = index * 26 + (c - 'A' + 1);
            }
            return index - 1;
        }

        /// <summary>
        /// Получение Id таблицы для Google Sheets
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ExtractSpreadsheetId(string url)
        {
            // Регулярное выражение для извлечения spreadsheetId
            var match = Regex.Match(url, @"spreadsheets/d/([a-zA-Z0-9-_]+)/");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception("Не удалось получить Id Google таблицы.");
            }
        }
    }
}
