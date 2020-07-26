using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Model;

namespace WebBot
{
    /// <summary>
    /// Команды бота
    /// </summary>
    static class CommandsPhrases
    {
        static Guid helloGuid = Guid.NewGuid();
        static Guid showAllCommandsGuid = Guid.NewGuid();
        static Guid makeOrderGuid = Guid.NewGuid();
        static Guid startProductOrderGuid = Guid.NewGuid();
        static Guid deliveryInfoGuid = Guid.NewGuid();
        static Guid showFAQTopicGuid = Guid.NewGuid();
        static Guid reviewsGuid = Guid.NewGuid();
        static Guid servicesGuid = Guid.NewGuid();
        static Guid unknownCommandGuid = Guid.NewGuid();
        static Guid cancelOrderGuid = Guid.NewGuid();
        static Guid showTableGuid = Guid.NewGuid();
        static Guid acceptOrderGuid = Guid.NewGuid();
        static Guid declineOrderGuid = Guid.NewGuid();
        static Guid askAdminGuid = Guid.NewGuid();
        static Guid backToMenuGuid = Guid.NewGuid();
        static Guid checkOrderStatusGuid = Guid.NewGuid();
        public static Dictionary<string, Guid> Commands = new Dictionary<string, Guid>
        {
            { "unknown", unknownCommandGuid}, { "таблица", showTableGuid}, { "принять", acceptOrderGuid }, { "отклонить", declineOrderGuid},
            { "привет", helloGuid }, {"эй", helloGuid }, {"здравствуйте", helloGuid }, {"здравствуй", helloGuid }, {"йоу", helloGuid },
            {"все команды", showAllCommandsGuid }, {"команды", showAllCommandsGuid }, {"что ты умеешь", showAllCommandsGuid },
            { "помощь", showAllCommandsGuid }, {"покажи команды", showAllCommandsGuid }, { "оформить", startProductOrderGuid },
            { "заказ", makeOrderGuid }, {"сделать заказ", makeOrderGuid }, {"купить", makeOrderGuid },
            { "доставка", deliveryInfoGuid }, {"условия доставки", deliveryInfoGuid }, {"про доставку", deliveryInfoGuid },
            { "вопрос", showFAQTopicGuid }, {"задать вопрос", showFAQTopicGuid }, {"faq", showFAQTopicGuid },
            { "отзывы", reviewsGuid }, {"отзыв", reviewsGuid }, {"спросить", askAdminGuid}, {"меню", backToMenuGuid},
            { "наши услуги", servicesGuid }, {"услуги", servicesGuid }, {"отменить заказ", cancelOrderGuid },
            { "заказы", checkOrderStatusGuid }
        };

        public static Dictionary<Guid, Action<VkApiBot, Message>> CommandsActions = new Dictionary<Guid, Action<VkApiBot, Message>>
        {
            {helloGuid, BotActions.SendHelloMessage },
            {showAllCommandsGuid, BotActions.ShowAllCommands },
            {unknownCommandGuid, BotActions.UnknownCommand },
            {startProductOrderGuid, BotActions.StartProductOrder },
            {makeOrderGuid, BotActions.MakeOrder },
            {deliveryInfoGuid, BotActions.DeliveryInfo },
            {showFAQTopicGuid, BotActions.AskQuestion },
            {reviewsGuid, BotActions.ShowReviews },
            {servicesGuid, BotActions.ServicesInfo },
            {cancelOrderGuid, BotActions.CancelOrder },
            {showTableGuid, BotActions.ShowTable },
            {acceptOrderGuid, BotActions.AcceptOrder },
            {declineOrderGuid, BotActions.DeclineOrder },
            {askAdminGuid, BotActions.AskAdmin },
            {backToMenuGuid, BotActions.BackToMenu },
            {checkOrderStatusGuid, BotActions.CheckOrderStatus}
        };

        public static Dictionary<string, Action<VkApiBot, Message>> OrderActions = new Dictionary<string, Action<VkApiBot, Message>>
        {
            {"автоматически", BotActions.StartAutoOrderMode }, {"написать продавцу", BotActions.StartManualOrderMode }, //изменить назначение ключа на автоматическое
            { "unknown", BotActions.UnknownOrderCommand }, { "отменить заказ", BotActions.CancelOrder }, { "условия доставки", BotActions.DeliveryInfo },
            { "выбрать время", BotActions.SelectDeliveryTime }, { "готово", BotActions.OrderConfirmation }, { "wrong geo", BotActions.WrongDeliveryAddress},
            { "неверный формат", BotActions.WrongDeliveryTimeFormat }, { "confirm", BotActions.OrderFinished },
            { "repeat confirm", BotActions.OrderConfirmation }, {"mode selection", BotActions.WrongOrderMode },
            { "картой", BotActions.OrderConfirmation }, {"наличными", BotActions.OrderConfirmation}, {"pm selection", BotActions.ShowPaymentMethodSelection },
            { "product selection", BotActions.WrongProductSelection }, { "продолжить", BotActions.OrderModeSelection },
            { "promo", BotActions.EnterPromoCode }, { "correct promo", BotActions.PromoCodeCheking }, { "wrong promo", BotActions.WrongPromoCode }
        };
    }
}
