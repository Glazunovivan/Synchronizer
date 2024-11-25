using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System;
using Google.Apis.Auth.OAuth2;

namespace Synchronizer.Application
{
    public class GoogleSheet : IGetterData
    {
        public readonly string _linkUrl;
        public readonly string _range;
        public readonly string _spreadsheetId;
        public GoogleCredential? _credential;

        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly }; // Только на чтение таблицы

        public GoogleSheet(Settings settings, string linkUrl, string range)
        {
            _linkUrl = linkUrl;
            _range = range;
            _spreadsheetId = Extensions.ExtractSpreadsheetId(_linkUrl);
            _credential = GoogleCredential.FromFile(settings.Credential);
        }


        public List<string> GetData()
        {
            string range = $"Лист1!{_range}:{_range}";

            try
            {
                List<IList<object>> values = ReadPublicSheet(_spreadsheetId, range);

                var result = new List<string>();
                foreach (var res in values)
                {
                    result.Add(res.FirstOrDefault().ToString());
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex.Message}");
            }
        }

        public List<IList<object>> ReadPublicSheet(string spreadsheetId, string range)
        {
            // Авторизация с использованием сервисного аккаунта (не требуется экран согласия для публичных данных)
            if (_credential == null)
            {
                throw new Exception("Неверные учетные данные");
            }

            // Создаем сервис
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential
            });

            // Запрос данных
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();

            return (List<IList<object>>)response.Values;
        }
    }
}
