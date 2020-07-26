using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBot;
using WebBot.Models;

namespace WebBot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Refresh()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult StartBot(ConfigModel config, IFormFile botfile)
        {
            var vkBot = TryBotCreation(config, botfile);
            if (vkBot != null)
                RunBot(vkBot);
            return RedirectToAction("Index", "Home");
        }

        private VkApiBot TryBotCreation(ConfigModel config, IFormFile botfile)
        {
            var sheetBot = TryCreateSpreadSheetBot(botfile, config.SpreadsheetId);
            if (sheetBot == null)
                return null;
            var vkBot = TryCreateVkBot(config, sheetBot);
            if (vkBot == null)
                return null;
            PermanentValues.Config = config;
            return vkBot;
        }

        public IActionResult ClearLogs()
        {
            Logger.Clear();
            Logger.Info("Старые логи удалены");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ResetBot()
        {
            Logger.Clear();
            PermanentValues.ClearPermanentValues();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RestartBot()
        {
            var sheetBot = new SpreadSheetBot(PermanentValues.GoogleSheetsCredentialsPath,
                PermanentValues.Config.SpreadsheetId);
            var vkBot = TryCreateVkBot(PermanentValues.Config, sheetBot);
            if (vkBot != null)
            {
                Logger.Clear();
                RunBot(vkBot);
            }
            return RedirectToAction("Index", "Home");
        }

        private void RunBot(VkApiBot vkBot)
        {
            vkBot.Listen();
            PermanentValues.BotStatus = BotStatus.Working;
            Logger.Info("Бот запущен");
        }

        private SpreadSheetBot TryCreateSpreadSheetBot(IFormFile file, string spreadsheetId)
        {
            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(stream);
                }
                PermanentValues.GoogleSheetsCredentialsPath = filePath;
                return new SpreadSheetBot(filePath, spreadsheetId);
            }
            catch (Exception ex)
            {
                TempDataMessage("message", "primary", $"Не удалось создать бота таблиц: {ex.Message}");
                return null;
            }

        }

        private VkApiBot TryCreateVkBot(ConfigModel config, SpreadSheetBot sheetBot)
        {
            try
            {
                var adminIds = new List<long>();
                foreach (var id in config.AdminIds.Split(','))
                {
                    adminIds.Add(long.Parse(id));
                }
                var bot = new VkApiBot(
                    config.Token,
                    ulong.Parse(config.GroupId),
                    adminIds, sheetBot);
                bot.AuthCheck();
                return bot;
            }
            catch (Exception ex)
            {
                TempDataMessage("message", "primary", $"Ошибка авторизации: {ex.Message}");
                return null;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void TempDataMessage(string key, string alert, string value)
        {
            TempData.Remove(key);
            TempData.Add(key, value);
            TempData.Add("alertType", alert);
        }
    }
}
