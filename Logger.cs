using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBot
{
    /// <summary>
    /// Класс для ведения логов
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Строка логов
        /// </summary>
        private static StringBuilder logString = new StringBuilder();

        /// <summary>
        /// Создать запись в логах с тегом INFO
        /// </summary>
        /// <param name="message">Сообщение для записи в логи</param>
        public static void Info(string message)
        {
            logString.AppendLine($"[{DateTime.Now.ToString("HH:mm:ss")}] INFO | " +
                $"{message}");
        }

        /// <summary>
        /// Создать запись в логах с тегом ERROR
        /// </summary>
        /// <param name="exception">Исключение, которое вызвало ошибку</param>
        public static void Error(Exception exception)
        {
            logString.AppendLine($"[{DateTime.Now.ToString("HH:mm:ss")}] ERROR | " +
                $"{exception.Message}");
            logString.AppendLine(exception.StackTrace);
        }

        /// <summary>
        /// Очистить логи
        /// </summary>
        public static void Clear() => logString.Clear();

        /// <summary>
        /// Получить логи в виде строки
        /// </summary>
        /// <returns></returns>
        public static string GetLogString() => logString.ToString();

    }
}
