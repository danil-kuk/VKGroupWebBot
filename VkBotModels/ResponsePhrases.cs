using System;
using System.Collections.Generic;
using System.Text;

namespace WebBot
{
    /// <summary>
    /// Фразы для ответов бота
    /// </summary>
    static class ResponsePhrases
    {
        private static readonly List<string> helloMessage = new List<string>
        {
            "И тебе привет, нажми на кнопку, чтобы сделать заказ",
            "Привет! Для заказа нажми на кнопку ниже"
        };
        private static readonly List<string> showFAQTopic = new List<string>
        {
            "Если у вас появились вопросы, то ознакомьтесь с соответсвующией темой у нас в группе: \n" +
            "https://vk.com/topic-180174909_39786341 \n\n" +//ссылка на тему с вопросами
            "Если у Вас возникли другие вопросы или Вы не нашли ответ в нашей теме, " +
            "то нажмите кнопку \"Спросить\" и Вам ответит наш Администратор", 
        };
        private static readonly List<string> showDeliveryInfo = new List<string>
        {
            "Ознакомиться с нашими условиями достваки можно здесь: \n" +
            "https://vk.com/izh_coil?w=product-180174909_2310150%2Fquery", //ссылка
        };
        private static readonly List<string> servicesInfo = new List<string>
        {
            "Информацию обо всех наших услугах можно увидеть здесь: \n" +
            "https://vk.com/market-180174909?section=album_1", //ссылка
        };
        private static readonly List<string> reviewsInfo = new List<string>
        {
            "Отзывы наших клентов можно увидеть здесь: \n" +
            "https://vk.com/topic-180174909_39684154 \n" +//ссылка
            "А также, если вы уже пользовались нашим сервисом, то оставьте нам свой отзыв.", 
        };
        private static readonly List<string> showAllComands = new List<string>
        {
            "Cписок моих команд: \n",
            "Все мои команды: \n",
            "Используйте следующие команды: \n"
        };
        public static readonly string AllCommands = "- Сделать заказ [Заказ] \n- Задать вопрос[Вопрос] \n" +
            "- Посмотреть отзывы[Отзывы] \n- Наши услуги[Услуги] \n- Условия доставки[Доставка]"; //Изменить вывод команд
        private static readonly List<string> unknownCommands = new List<string>
        {
            "Не понимаю Вас, используйте одну из моих команд",
            "Не понимаю Вас, попробуйте использовать одну из моих команд",
            "Неизвестная команда, используйте команду из списка",
            "Неизвестная команда, воспользуйтесь командой из списка",
            "Неизвестная команда, проверьте правильность введенной команды"
        };
        private static readonly List<string> addAnotherProduct = new List<string>
        {
            "Хотите купить еще один товар? Просто добавьте его аналогичным образом. \n" +
            "Если Вы выбрали все товары, то нажмите \"Продолжить\" \n" +
            "Выбранные товары: \n"
        };
        private static readonly List<string> orderModeSelection = new List<string>
        {
            "Отлично! Теперь давайте выберем вариант оформления заказа: \n" +
            "Автоматически - заказ будет оформлен при помощи бота и сразу отправится в обработку. \n" +
            "Написать продавцу - вам ответит один из наших Администраторов, но тогда придется немного подождать."
        };
        private static readonly List<string> orderDeliveryAdress = new List<string>
        {
            "Вы выбрали автоматический режим оформления заказа. Давайте начнем: \n" +
            "Для начала укажите адрес доставки"
        };
        private static readonly List<string> farAwayDelivery = new List<string>
        {
            "К сожалению, мы не можем доставить ваш заказ в данное место. \n" +
            "Пожалуйста, укажите другой адрес доставки:"
        };
        private static readonly List<string> orderDateTime = new List<string>
        {
            "Адрес доставки выбран, осталось только указать дату и время доставки: \n" +
            $"Укажите дату и время одним сообщением в формате: ДД.ММ.ГГ ЧЧ:ММ \n" +
            $"Пример: {DateTime.Now.AddDays(1).ToString("dd.MM.yy")} {DateTime.Now.ToString("HH:mm")}\n" +
            $"\n [Заказать можно на время НЕ РАНЬШЕ, чем на 2 часа от текущего времени!]"
        };
        private static readonly List<string> wrongDeliveryTimeFormat = new List<string>
        {
            "Введенные данные имели неверный формат или не соответсвовали нашим условиям. " +
            "Проверьте корректность вашей информации. \n" +
            $"Пример: {DateTime.Now.AddDays(1).ToString("dd.MM.yy")} {DateTime.Now.ToString("HH:mm")} \n" +
            "Пожалуйста, введите дату и время доставки повторно: "
        };
        private static readonly List<string> paymentMethod = new List<string>
        {
            "Выберите способ оплаты:"
        };
        private static readonly List<string> orderConfirmation = new List<string>
        {
            "Готово! Осталось только проверить информацию о вашем заказе. Данные, которые вы указали: \n"
        };
        private static readonly List<string> orderFinished = new List<string>
        {
            "Списибо за покупку! Ваш заказ отправлен в обработку, если у нас не получиться доставить его в указанное вами время, с вами свяжется наш администратор"
        };
        private static readonly List<string> orderConfirmationForAdmin = new List<string>
        {
            "Поступил новый заказ! \nПокупатель: "
        };
        private static readonly List<string> waitForAdminResponse = new List<string>
        {
            "Ждите, с вами свяжется администратор"
        };
        private static readonly List<string> enterPromocode = new List<string>
        {
            "Введите промокод"
        };
        private static readonly List<string> wrongPromocode = new List<string>
        {
            "Неверный промокод!\nПроверьте свой промокод и введите его повторно"
        };
        private static readonly List<string> selectDeliveryAddress = new List<string>
        {
            "Укажите адрес доставки, нажав на соответсвующую кнопку"
        };
        private static readonly List<string> orderCanceled = new List<string>
        {
            "Покупка товара отменена"
        };
        public static string UnknownCommand { get => GetRandomResponsePhrase(unknownCommands); }
        public static string ShowAllCommands { get => GetRandomResponsePhrase(showAllComands); }
        public static string HelloMessage { get => GetRandomResponsePhrase(helloMessage); }
        public static string AskQuestion { get => GetRandomResponsePhrase(showFAQTopic); }
        public static string ReviewsInfo { get => GetRandomResponsePhrase(reviewsInfo); }
        public static string ServicesInfo { get => GetRandomResponsePhrase(servicesInfo); }
        public static string ShowDeliveryInfo { get => GetRandomResponsePhrase(showDeliveryInfo); }
        public static string OrderModeSelection { get => GetRandomResponsePhrase(orderModeSelection); }
        public static string OrderDeliveryAdress { get => GetRandomResponsePhrase(orderDeliveryAdress); }
        public static string FarAwayDelivery { get => GetRandomResponsePhrase(farAwayDelivery); }
        public static string OrderDateTime { get => GetRandomResponsePhrase(orderDateTime); }
        public static string WrongDeliveryTimeFormat { get => GetRandomResponsePhrase(wrongDeliveryTimeFormat); }
        public static string OrderConfirmation { get => GetRandomResponsePhrase(orderConfirmation); }
        public static string PaymentMethod { get => GetRandomResponsePhrase(paymentMethod); }
        public static string OrderFinished { get => GetRandomResponsePhrase(orderFinished); }
        public static string OrderConfirmationForAdmin { get => GetRandomResponsePhrase(orderConfirmationForAdmin); }
        public static string AddAnotherProduct { get => GetRandomResponsePhrase(addAnotherProduct); }
        public static string WaitForAdminResponse { get => GetRandomResponsePhrase(waitForAdminResponse); }
        public static string EnterPromocode { get => GetRandomResponsePhrase(enterPromocode); }
        public static string WrongPromocode { get => GetRandomResponsePhrase(wrongPromocode); }
        public static string SelectDeliveryAddress { get => GetRandomResponsePhrase(selectDeliveryAddress); }
        public static string OrderCanceled { get => GetRandomResponsePhrase(orderCanceled); }

        /// <summary>
        /// Получить случайную фразу из списка для ответа пользователю
        /// </summary>
        /// <param name="phrasesList">Список фраз</param>
        /// <returns>Одна фраза</returns>
        private static string GetRandomResponsePhrase(List<string> phrasesList)
        {
            var rnd = new Random();
            var listLength = phrasesList.Count;
            return phrasesList[rnd.Next(listLength)];
        }
    }
}
