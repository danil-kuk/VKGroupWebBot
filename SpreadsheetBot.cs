using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebBot
{
    /// <summary>
    /// Бот Google Таблиц
    /// </summary>
    class SpreadSheetBot
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static SheetsService service;

        public readonly string SpreadsheetId; //Id таблицы, с которой работаем
        public string Sheet { get; set; } //Название страницы, например "Лист 1"
        public string Range { get; private set; } //Диапазон
        /// <summary>
        /// Создание бота для Google Таблиц
        /// </summary>
        /// <param name="spreadsheetId">Id таблицы с которой нужно работать</param>
        public SpreadSheetBot(string credentialPath, string spreadsheetId)
            : this(credentialPath, spreadsheetId, "Лист1") { }
        public SpreadSheetBot(string credentialPath, string spreadsheetId, string sheetPage)
            : this(credentialPath, spreadsheetId, sheetPage, "A:Z") { }
        public SpreadSheetBot(string credentialPath, string spreadsheetId, string sheetPage, string range)
        {
            SpreadsheetId = spreadsheetId;
            Sheet = sheetPage;
            Range = $"{Sheet}!{range}";
            Connect(credentialPath);
        }

        /// <summary>
        /// Подключение к таблице
        /// </summary>
        private void Connect(string credentialPath)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Создание Google Sheets API сервиса.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SpreadsheetBot",
            });
        }

        /// <summary>
        /// Получить значения всех полей таблицы
        /// </summary>
        /// <param name="sheet">Лист таблицы, например "Лист1"</param>
        /// <returns></returns>
        public IEnumerable<IList<object>> ReadAllEntries(string sheet = null)
        {
            var data = sheet == null ? GetDataRequest(Range) : GetDataRequest($"{sheet}!A:Z");
            foreach (var order in data)
            {
                yield return order;
            }
        }

        /// <summary>
        /// Получить значение конкретной клетки таблицы
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <param name="row">Строка</param>
        /// <returns></returns>
        public string GetCellValue(int column, int row)
        {
            var col = TranslateColumn(column);
            var range = $"{Sheet}!{col}{row}";
            var response = GetDataRequest(range);
            return response?[0][0].ToString();
        }

        /// <summary>
        /// Создание запроса на получение данных из таблицы
        /// </summary>
        /// <param name="range">Диапазон, откуда требуется получить данные</param>
        /// <returns></returns>
        private IList<IList<object>> GetDataRequest(string range)
        {
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            try
            {
                return request.Execute().Values;
            }
            catch (Exception)
            {
                //Если введен неверный id таблицы
                throw new ArgumentException("Не удается получить доступ к таблице с указанным id");
            }
        }

        /// <summary>
        /// Изменить значение указанной клетки
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <param name="row">Строка</param>
        /// <param name="newText">Новое значение</param>
        /// <param name="sheet">Лист таблицы (по-умолчанию первый лист таблицы)</param>
        public void ChangeCellValue(int column, int row, string newText, string sheet = null)
        {
            var col = TranslateColumn(column);
            var range = sheet == null ? $"{Sheet}!{col}{row}" : $"{sheet}!{col}{row}";
            var valueRange = new ValueRange();
            var objectsList = new List<object> { newText };
            valueRange.Values = new List<IList<object>>() { objectsList };
            UpdateRequest(range, valueRange);
        }

        /// <summary>
        /// Изменить значение диапазона, начиная с указанной ячейки.
        /// Необходимо указать только адрес левого верхнего угла диапазона
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <param name="row">Строка</param>
        /// <param name="values">Новые значения</param>
        public void ChangeRangeValue(int column, int row, List<IList<object>> values)
        {
            var col = TranslateColumn(column);
            var endRow = row + values.Count;
            var endCol = TranslateColumn(column + values[0].Count);
            var range = $"{Sheet}!{col}{row}:{endCol}{endRow}";
            var valueRange = new ValueRange
            {
                Values = values
            };
            UpdateRequest(range, valueRange);
        }

        /// <summary>
        /// Создание запроса на обновление значений в таблице
        /// </summary>
        /// <param name="range">Диапазон, в котором будут изменены значения</param>
        /// <param name="valueRange">Новые значения</param>
        private void UpdateRequest(string range, ValueRange valueRange)
        {
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                updateRequest.Execute();
            }
            catch (Exception)
            {
                //Если введен неверный id таблицы
                throw new ArgumentException("Не удается получить доступ к таблице с указанным id");
            }
        }

        /// <summary>
        /// Добавление новых значений в конец таблицы
        /// </summary>
        /// <param name="values">Новые значение, которые нужно добавить</param>
        public void AppendRangeValue(List<IList<object>> values)
        {
            var valueRange = new ValueRange
            {
                Values = values
            };
            AppendRequest(valueRange);
        }

        /// <summary>
        /// Запрос на добавление новых данных в конец таблицы
        /// </summary>
        /// <param name="range"></param>
        /// <param name="valueRange"></param>
        private void AppendRequest(ValueRange valueRange)
        {
            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, Range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                appendRequest.Execute();
            }
            catch (Exception)
            {
                //Если введен неверный id таблицы
                throw new ArgumentException("Не удается получить доступ к таблице с указанным id");
            }
        }

        /// <summary>
        /// Перевод столбца в букву, например 2 -> B
        /// </summary>
        /// <param name="value">Числовое значение столбца</param>
        /// <returns></returns>
        private char TranslateColumn(int value) => (char)(65 + (value - 1));
    }
}
