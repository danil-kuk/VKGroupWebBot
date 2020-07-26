using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VkNet.Model;
using VkNet.Model.Keyboard;

namespace WebBot
{
    static class BotActions
    {
        public static Action<VkApiBot, Message> SendHelloMessage = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.HelloMessage, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> MakeOrder = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value,
                "Для совершения заказа выберите его в нашем магазине: https://vk.com/market-" + bot.MyGroupId + "\n" +
                "После зайдите на страницу товара и нажмите \"Написать продавцу\" для подтверждения выбора товара.",
                MessageKeyboardSchemes.DefaultButtons); // Изменить строку
        });

        public static Action<VkApiBot, Message> StartProductOrder = new Action<VkApiBot, Message>((bot, message) =>
        {
            var productData = message.Attachments[0].Instance as Market;
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.ProductSelection;
            bot.AllUserSessions[userId].OrderData.Products.Add(productData);
            bot.AllUserSessions[message.FromId.Value].ProductsAsAttachments.AddRange(message.Attachments.Where(at => at.Type.Name == "Market"));
            bot.WriteToSelectedUser(userId, ResponsePhrases.AddAnotherProduct + bot.GetProductsWithCosts(userId),
                MessageKeyboardSchemes.AddNewProductToOrder);
            Logger.Info($"Товар \"{productData.Title}\" добавлен к заказу пользователя  id{userId}");
        });

        public static Action<VkApiBot, Message> OrderModeSelection = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.OrderModeSelection;
            bot.WriteToSelectedUser(userId, ResponsePhrases.OrderModeSelection, MessageKeyboardSchemes.OrderModeSelectionButtons);
        });

        public static Action<VkApiBot, Message> ShowAllCommands = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.ShowAllCommands + ResponsePhrases.AllCommands, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> DeliveryInfo = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.ShowDeliveryInfo, bot.AllUserSessions[userId].PrevKeyboard);
        });

        public static Action<VkApiBot, Message> AskQuestion = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.AskQuestion, MessageKeyboardSchemes.AskQuestion);
        });

        public static Action<VkApiBot, Message> CheckOrderStatus = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            var userOrders = bot.GetUserOrders(userId);
            bot.WriteToSelectedUser(userId, "Ваши заказы: \n" + userOrders,
                MessageKeyboardSchemes.BackToMenuButtonOnly);
        });

        public static Action<VkApiBot, Message> AskAdmin = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.WaitForAdminResponse, 
                MessageKeyboardSchemes.DefaultButtons);
            bot.NotifyAdminsAboutNewQuestion(message.FromId.Value);
        });

        public static Action<VkApiBot, Message> BackToMenu = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.HelloMessage, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> ServicesInfo = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.ServicesInfo, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> ShowReviews = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.ReviewsInfo, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> UnknownCommand = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.UnknownCommand, MessageKeyboardSchemes.AllComandsButtonOnly);
        });

        public static Action<VkApiBot, Message> ShowTable = new Action<VkApiBot, Message>((bot, message) =>
        {
            var tableLink = "https://docs.google.com/spreadsheets/d/" + bot.SpreadSheetBot.SpreadsheetId.ToString();
            bot.WriteToSelectedUser(message.FromId.Value, "Ссылка на таблицу заказов: \n" + tableLink, MessageKeyboardSchemes.DefaultButtons);
        });

        public static Action<VkApiBot, Message> AcceptOrder = new Action<VkApiBot, Message>((bot, message) =>
        {
            var lastBotMessage = bot.AllUserSessions[message.FromId.Value].LastBotMessage;
            var product = bot.InProcessingOrders.Where(p => p.DeliveryAddress.Coordinates.Latitude == lastBotMessage.Lat &&
            p.DeliveryAddress.Coordinates.Longitude == lastBotMessage.Longitude).FirstOrDefault(); //сделать нахождение заказа через id заказа
            if (product == null)
                bot.WriteToSelectedUser(message.FromId.Value, "Нет заказа", MessageKeyboardSchemes.DefaultButtons);
            else
            {
                bot.ChangeOrderStatus(product, "Принят");
                bot.WriteToSelectedUser(message.FromId.Value, "Заказ принят", MessageKeyboardSchemes.DefaultButtons);
                bot.InProcessingOrders.Remove(product);
            }
        });
        //Нужен рефакторинг
        public static Action<VkApiBot, Message> DeclineOrder = new Action<VkApiBot, Message>((bot, message) =>
        {
            var lastBotMessage = bot.AllUserSessions[message.FromId.Value].LastBotMessage;
            var product = bot.InProcessingOrders.Where(p => p.DeliveryAddress.Coordinates.Latitude == lastBotMessage.Lat &&
            p.DeliveryAddress.Coordinates.Longitude == lastBotMessage.Longitude).FirstOrDefault(); //сделать нахождение заказа через id заказа
            if (product == null)
                bot.WriteToSelectedUser(message.FromId.Value, "Нет заказа", MessageKeyboardSchemes.DefaultButtons);
            else
            {
                bot.ChangeOrderStatus(product, "Отклонен");
                bot.WriteToSelectedUser(message.FromId.Value, "Заказ отклонен", MessageKeyboardSchemes.DefaultButtons);
                bot.InProcessingOrders.Remove(product);
            }
        });

        //Новый заказ
        public static Action<VkApiBot, Message> StartAutoOrderMode = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.OrderDeliveryAdress, MessageKeyboardSchemes.SetLocation);
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.DeliveryAddressSelection;
            Logger.Info($"Начато оформление заказа {bot.AllUserSessions[userId].OrderData.OrderId} " +
                $"пользователем id{userId}");
        });

        public static Action<VkApiBot, Message> WrongOrderMode = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.OrderModeSelection, MessageKeyboardSchemes.OrderModeSelectionButtons);
        });

        public static Action<VkApiBot, Message> EnterPromoCode = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.PromoCodeChecking;
            bot.WriteToSelectedUser(userId, ResponsePhrases.EnterPromocode, MessageKeyboardSchemes.PromoCodeChecking);
        });

        public static Action<VkApiBot, Message> PromoCodeCheking = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            var promoCode = bot.AllUserSessions[userId].OrderData.Promocode;
            bot.WriteToSelectedUser(userId, $"Введенный промокод: {promoCode.Code}\n{promoCode.Description}");
            //Изменение цены и прочего
            PromoCodesTypes.AllPromoCodes[promoCode.CodeType].Invoke(bot, message, promoCode);
            OrderConfirmationAfterPromocode(bot, message);
        });

        public static Action<VkApiBot, Message> WrongPromoCode = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.WrongPromocode,
                MessageKeyboardSchemes.PromoCodeChecking);
        });

        public static Action<VkApiBot, Message> WrongProductSelection = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, 
                ResponsePhrases.AddAnotherProduct + bot.GetProductsWithCosts(userId), 
                MessageKeyboardSchemes.AddNewProductToOrder);
        });

        public static Action<VkApiBot, Message> SelectDeliveryTime = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.DeliveryAddress = message.Geo;
            Logger.Info($"{userId}: Адрес доставки - Lat: {message.Geo.Coordinates.Latitude}; " +
                $"Long: {message.Geo.Coordinates.Longitude}");
            if (bot.AllUserSessions[userId].OrderData.FindDistanteToDeliveryPoint() > 10000)
            {
                bot.AllUserSessions[userId].OrderData.DeliveryAddress = null;
                bot.WriteToSelectedUser(userId, ResponsePhrases.FarAwayDelivery, MessageKeyboardSchemes.SetLocation);
                return;
            }
            bot.WriteToSelectedUser(userId, ResponsePhrases.OrderDateTime, MessageKeyboardSchemes.OrderDefaultButtons);
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.DeliveryDateTimeSelection;
        });

        public static Action<VkApiBot, Message> OrderConfirmation = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            if (bot.AllUserSessions[userId].OrderData.Promocode == null)
                OrderConfirmationBeforePromocode(bot, message);
            else
                OrderConfirmationAfterPromocode(bot, message);
        });

        private static Action<VkApiBot, Message> OrderConfirmationBeforePromocode = new Action<VkApiBot, Message>((bot, message) =>
        {
            Enum.TryParse(message.Text.ToLower(), out PaymentMethod paymentMethod);
            var userId = message.FromId.Value;
            bot.AllUserSessions[userId].OrderData.PaymentMethod = paymentMethod;
            bot.AllUserSessions[userId].OrderData.OrderStatus = "В обработке";
            bot.WriteToSelectedUserWithDeliveryAddress(userId,
                ResponsePhrases.OrderConfirmation + bot.GetOrderDetails(message.FromId.Value),
                MessageKeyboardSchemes.ConfirmOrderWithPromo);
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.DeliveryConfirmation;
        });

        private static Action<VkApiBot, Message> OrderConfirmationAfterPromocode = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUserWithDeliveryAddress(userId,
                ResponsePhrases.OrderConfirmation + bot.GetOrderDetails(message.FromId.Value),
                MessageKeyboardSchemes.ConfirmOrderWithDeliveryInfo);
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.DeliveryConfirmation;
        });

        public static Action<VkApiBot, Message> ShowPaymentMethodSelection = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.PaymentMethod, MessageKeyboardSchemes.PaymentMethodSelectionButtons);
            bot.AllUserSessions[userId].OrderData.OrderStage = OrderStage.PaymentMethodSelection;
        });

        public static Action<VkApiBot, Message> WrongDeliveryTimeFormat = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.WrongDeliveryTimeFormat, MessageKeyboardSchemes.OrderDefaultButtons);
        });

        public static Action<VkApiBot, Message> UnknownOrderCommand = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, "Неизвестная команда: ", //заменить строку
                MessageKeyboardSchemes.CancelOrderButtonOnly);
        });

        public static Action<VkApiBot, Message> WrongDeliveryAddress = new Action<VkApiBot, Message>((bot, message) =>
        {
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.SelectDeliveryAddress,
                MessageKeyboardSchemes.SetLocation);
        });

        public static Action<VkApiBot, Message> StartManualOrderMode = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.WaitForAdminResponse, MessageKeyboardSchemes.AllComandsButtonOnly);
            //Отправка данных администратору
            bot.ManualOrderSendOrderInfoToAdmin(userId);
            var prevKeyboard = bot.AllUserSessions[userId].PrevKeyboard;
            bot.AllUserSessions[userId].OrderData = new OrderData(userId);
        });

        public static Action<VkApiBot, Message> OrderFinished = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(userId, ResponsePhrases.OrderFinished, MessageKeyboardSchemes.DefaultButtons);
            bot.SendOrderDataToTable(userId); //Отправка данных в таблицу
            //Отправка уведомления админам
            foreach (var adminId in bot.AdminIds)
            {
                bot.WriteToAdminWithDeliveryInfo(adminId, userId,
                                ResponsePhrases.OrderConfirmationForAdmin + $"[id{userId}|{bot.GetFullUserName(userId)}] \n" +
                                bot.GetOrderDetails(userId),
                                MessageKeyboardSchemes.AdminOrderConfirmationButtons);
            }
            bot.InProcessingOrders.Add(bot.AllUserSessions[userId].OrderData);
            Logger.Info($"Заказ {bot.AllUserSessions[userId].OrderData.OrderId} " +
                $"от пользователя id{message.FromId.Value} принят в обработку");
            bot.AllUserSessions[userId].OrderData = new OrderData(userId);
        });

        public static Action<VkApiBot, Message> CancelOrder = new Action<VkApiBot, Message>((bot, message) =>
        {
            var userId = message.FromId.Value;
            bot.WriteToSelectedUser(message.FromId.Value, ResponsePhrases.OrderCanceled, 
                MessageKeyboardSchemes.DefaultButtons);
            Logger.Info($"Заказ {bot.AllUserSessions[userId].OrderData.OrderId} " +
                $"от пользователя id{userId} отменен");
            bot.AllUserSessions[userId].OrderData = new OrderData(userId);
        });
    }
}
