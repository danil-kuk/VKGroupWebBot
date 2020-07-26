using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkNet.Model;

namespace WebBot
{
    /// <summary>
    /// Промокод
    /// </summary>
    class PromoCode
    {
        public string Code { get; set; }
        public int Discount { get; set; }
        public string CodeType { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public bool IsLimited { get; set; }
        public bool IsUsed { get; set; }
        public long UserId { get; set; }
        public int TableRowIndex { get; set; }
    }

    static class PromoCodesTypes
    {
        public static Dictionary<string, Action<VkApiBot, Message, PromoCode>> AllPromoCodes = new Dictionary<string, Action<VkApiBot, Message, PromoCode>>
        {
            { "доставка", PromoCodesActions.DeliveryDiscount }, {"товар", PromoCodesActions.ProductDiscount},
            { "заказ", PromoCodesActions.FullOrderDiscount}
        };
    }

    static class PromoCodesActions
    {
        public static Action<VkApiBot, Message, PromoCode> DeliveryDiscount = new Action<VkApiBot, Message, PromoCode>((bot, message, code) =>
        {
            var userId = message.FromId.Value;
            var currentCost = bot.AllUserSessions[userId].OrderData.DeliveryCost;
            var newCost = currentCost - currentCost * code.Discount / 100.0;
            bot.AllUserSessions[userId].OrderData.DeliveryCost = (int)newCost;
            if (code.IsLimited)
                bot.SpreadSheetBot.ChangeCellValue(7, code.TableRowIndex, "Да", "Лист2");
        });

        public static Action<VkApiBot, Message, PromoCode> ProductDiscount = new Action<VkApiBot, Message, PromoCode>((bot, message, code) =>
        {
            var userId = message.FromId.Value;
            var productsForDiscount = bot.AllUserSessions[userId].OrderData.Products.Where(p => p.Title == code.Product);
            foreach (var product in productsForDiscount)
            {
                var currentPrice = product.Price.Amount;
                var newPrice = currentPrice - currentPrice * code.Discount / 100.0;
                product.Price.Amount = (int)newPrice;
            }
            if (code.IsLimited)
                bot.SpreadSheetBot.ChangeCellValue(7, code.TableRowIndex, "Да", "Лист2");
        });

        public static Action<VkApiBot, Message, PromoCode> FullOrderDiscount = new Action<VkApiBot, Message, PromoCode>((bot, message, code) =>
        {
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.OrderDiscont = code.Discount;
            if (code.IsLimited)
                bot.SpreadSheetBot.ChangeCellValue(7, code.TableRowIndex, "Да", "Лист2");
        });
    }
}
