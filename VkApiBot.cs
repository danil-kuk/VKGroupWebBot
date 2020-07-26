using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;
using WebBot.Models;

namespace WebBot
{
    /// <summary>
    /// ВК бот
    /// </summary>
    class VkApiBot
    {
        static readonly VkApi vk = new VkApi();
        readonly string token;
        public ulong MyGroupId { get; private set; }

        public Dictionary<long, UserSession> AllUserSessions { get; set; }

        public List<long> AdminIds { get; private set; } = new List<long>();

        public List<OrderData> InProcessingOrders { get; set; } = new List<OrderData>();

        public SpreadSheetBot SpreadSheetBot { get; private set; }

        /// <summary>
        /// Создание бота для группы
        /// </summary>
        /// <param name="token">Ключ доступа API</param>
        /// <param name="groupId">ID группы, в которой будет работать бот</param>
        /// <param name="adminIds">ID администраторов группы</param>
        public VkApiBot(string token, ulong groupId, List<long> adminIds, SpreadSheetBot sheetBot)
        {
            MyGroupId = groupId;
            this.token = token;
            AdminIds = adminIds;
            AllUserSessions = new Dictionary<long, UserSession>();
            SpreadSheetBot = sheetBot;
            Authorize();
        }

        /// <summary>
        /// Авторизация бота
        /// </summary>
        private void Authorize()
        {
            vk.Authorize(new ApiAuthParams() { AccessToken = token });
        }

        /// <summary>
        /// Проверка авторизации. 
        /// При неверных данных бот не сможет получить ответ от Long Poll сервера
        /// </summary>
        public void AuthCheck()
        {
            try
            {
                vk.Groups.GetLongPollServer(MyGroupId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Получение LongPollHistory
        /// </summary>
        private BotsLongPollHistoryResponse GetLongPollHistory()
        {
            var server = vk.Groups.GetLongPollServer(MyGroupId);
            return vk.Groups.GetBotsLongPollHistory(
               new BotsLongPollHistoryParams()
               { Server = server.Server, Ts = server.Ts, Key = server.Key, Wait = 25 });
        }

        /// <summary>
        /// Получить список всех диалогов бота
        /// </summary>
        public List<Peer> GetAllUsers()
        {
            var allConversations = vk.Messages.GetConversations(new GetConversationsParams() { GroupId = MyGroupId });
            var usersToWrite = new List<Peer>();
            foreach (var conversation in allConversations.Items.Select(c => c.Conversation))
            {
                usersToWrite.Add(conversation.Peer);
            }
            return usersToWrite;
        }

        /// <summary>
        /// Отслеживание ботом всех событий сообщества
        /// </summary>
        public Task Listen()
        {
            var task = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var poll = GetLongPollHistory();
                        if (poll?.Updates == null) continue; // Проверка на новые события
                        foreach (var update in poll.Updates)
                        {
                            if (update.Type == GroupUpdateType.MessageNew)
                            {
                                var userId = update.Message.FromId.Value;
                                if (update.Message.Text != "")
                                    Logger.Info($"id{update.Message.FromId.Value}: {update.Message.Text}");
                                if (AllUserSessions.ContainsKey(userId) && AllUserSessions[userId].OrderData.OrderStage != OrderStage.NoOrder)
                                    CollectOrderData(update.Message);
                                else
                                    RegularResponse(update.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    PermanentValues.BotStatus = BotStatus.ReadyForRestart;
                    Logger.Info("Бот остановлен");
                }
            });
            return task;
        }

        /// <summary>
        /// Обычный ответ бота на сообщение пользователя
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        private void RegularResponse(Message message)
        {
            if (message.Attachments.Count > 0 && message.Attachments[0].Type.Name == "Market" &&
                (!AllUserSessions.ContainsKey(message.FromId.Value) ||
                AllUserSessions[message.FromId.Value].OrderData.OrderStage == OrderStage.NoOrder)) //проверить на повторное начинание заказа
                NewOrderResponse(message);
            else
                MessageResponse(message);
        }

        /// <summary>
        /// Сбор всех данных о заказе в зависимости от сообщения пользователя
        /// </summary>
        /// <param name="message">Сообщение пользователя с информацией о заказе</param>
        private void CollectOrderData(Message message)
        {
            if (AllUserSessions[message.FromId.Value].OrderData.OrderStage == OrderStage.ProductSelection &&
                message.Attachments.Count > 0 && message.Attachments[0].Type.Name == "Market")
            {
                BotActions.StartProductOrder(this, message);
                return;
            }
            var command = message.Text.ToLower();
            var userId = message.FromId.Value;
            if (command != "условия доставки" && command != "отменить заказ")
                switch (AllUserSessions[userId].OrderData.OrderStage)
                {
                    case OrderStage.ProductSelection:
                        {
                            if (command != "продолжить" && command != "отменить")
                                command = "product selection";
                            break;
                        }
                    case OrderStage.OrderModeSelection:
                        {
                            if (command != "автоматически" && command != "написать продавцу")
                                command = "mode selection";
                            break;
                        }
                    case OrderStage.DeliveryAddressSelection:
                        {
                            if (message.Geo != null)
                                command = "выбрать время";
                            else
                                command = "wrong geo";
                            break;
                        }
                    case OrderStage.DeliveryDateTimeSelection:
                        {
                            if (TrySetDeliveryTime(message))
                                command = "pm selection"; //переделать команды
                            else
                                command = "неверный формат";
                            break;
                        }
                    case OrderStage.PaymentMethodSelection:
                        {
                            if (command != "наличными" && command != "картой")
                                command = "pm selection";
                            else
                                command = "готово";
                            break;
                        }
                    case OrderStage.DeliveryConfirmation:
                        {
                            if (command == "подтвердить")
                                command = "confirm";
                            else if (command == "промокод" && AllUserSessions[userId].OrderData.Promocode == null)
                                command = "promo";
                            else
                                command = "repeat confirm";
                            break;
                        }
                    case OrderStage.PromoCodeChecking:
                        {
                            if (command == "назад")
                                command = "готово";
                            else if (PromoCodeChecking(userId, message.Text.ToUpper()))
                                command = "correct promo";
                            else
                                command = "wrong promo";
                            break;
                        }
                }
            if (!CommandsPhrases.OrderActions.ContainsKey(command))
                command = "unknown";
            CommandsPhrases.OrderActions[command].Invoke(this, message);
        }

        /// <summary>
        /// Проверка введенного промокода, а также его применение к заказу
        /// </summary>
        /// <param name="userId">Id пользователя, который совершает заказ</param>
        /// <param name="code">Введенный промокод</param>
        /// <returns>
        /// True - верный промокод; False - неверный промокод
        /// </returns>
        public bool PromoCodeChecking(long userId, string code)
        {
            var data = SpreadSheetBot.ReadAllEntries("Лист2");
            var rowIndex = 0;
            foreach (var row in data)
            {
                rowIndex++;
                if (row[0].ToString() == code)
                {
                    if ((row[3].ToString() != "Всех" && row[3].ToString() != userId.ToString()) ||
                        row[6].ToString() == "Да")
                        break;
                    AllUserSessions[userId].OrderData.Promocode = new PromoCode
                    {
                        Code = code,
                        CodeType = row[1].ToString().ToLower(),
                        Discount = int.Parse(row[2].ToString()),
                        UserId = row[3].ToString() == "Всех" ? -1 : userId,
                        Product = row[4].ToString(),
                        Description = row[5].ToString(),
                        IsLimited = row[6].ToString() != "-" ? true : false,
                        IsUsed = (row[6].ToString() == "-" || row[6].ToString() == "Нет") ? false : true,
                        TableRowIndex = rowIndex
                    };
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Написать пользователю сообщение
        /// </summary>
        /// <param name="userId">Id получателя</param>
        /// <param name="message">Сообщение</param>
        /// <param name="keyboard">Клавиатура бота (null - сообщение без клавиатуры)</param>
        public void WriteToSelectedUser(long userId, string message, MessageKeyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = MessageKeyboardSchemes.SetEmptyKeyboard;
            }
            var messageSendParams = new MessagesSendParams
            {
                UserId = userId,
                Message = message,
                RandomId = new Random().Next(999999),
                Keyboard = keyboard
            };
            vk.Messages.Send(messageSendParams);
            if (!AllUserSessions.ContainsKey(userId))
                AllUserSessions.Add(userId, new UserSession(userId, this));
            AllUserSessions[userId].PrevKeyboard = keyboard;
            AllUserSessions[userId].LastBotMessage = messageSendParams;
        }

        /// <summary>
        /// Установить время доставки заказа пользователя
        /// </summary>
        /// <param name="message">Сообщение, в котором пользователь указал время доставки</param>
        /// <returns>
        /// True, если удалось установить время заказа; 
        /// False, если из сообщения пользователя нельзя установить время заказа
        /// </returns>
        public bool TrySetDeliveryTime(Message message)
        {
            var time = message.Text;
            var userId = message.FromId.Value;
            string format = @"dd.MM.yy HH':'mm";
            DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime info);
            if (info < DateTime.Now.AddHours(2) || info > DateTime.Now.AddDays(5))
                return false;
            AllUserSessions[userId].OrderData.DeliveryDateTime = info;
            return true;
        }

        /// <summary>
        /// Получить подробную информацию о заказе в виде строки
        /// </summary>
        /// <param name="userId">Id пользователя, сделавшего заказ</param>
        /// <returns></returns>
        public string GetOrderDetails(long userId)
        {
            var order = AllUserSessions[userId].OrderData;
            if (order.DeliveryCost < 0)
                order.FindDeliveryCost();
            var productsWithCost = GetProductsWithCosts(userId);
            return $"Номер заказа: {order.OrderId}\n" +
                $"Выбранные товары: \n{productsWithCost}" +
                $"Стоимость доставки: {order.DeliveryCost} руб.\n" +
                $"Итоговая стоимость: {order.TotalCost} руб.\n" +
                $"Выбрана оплата {order.PaymentMethod} \n" +
                $"Адрес доставки: {order.DeliveryAddress.Place.City} (см. карту ниже) \n" +
                $"Дата и время доставки: {order.DeliveryDateTime.ToString("dd.MM.yy")} " +
                $"{order.DeliveryDateTime.ToString("HH:mm")}";
        }

        /// <summary>
        /// Получить имя и фамилию пользователя по его id
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Имя и фамилия в формате: "Имя Фамилия"</returns>
        public string GetFullUserName(long userId)
        {
            var userInfo = vk.Users.Get(new List<long> { userId }).First();
            return userInfo.FirstName + " " + userInfo.LastName;
        }

        /// <summary>
        /// Ответ на начало оформления нового заказа
        /// </summary>
        /// <param name="message">Сообщение пользователя с товаром для начала заказа</param>
        private void NewOrderResponse(Message message)
        {
            message.Text = "оформить";
            var userId = message.FromId.Value;
            if (!AllUserSessions.ContainsKey(userId))
                AllUserSessions.Add(userId, new UserSession(userId, this));
            AllUserSessions[message.FromId.Value].ProductsAsAttachments.AddRange(message.Attachments.Where(at => at.Type.Name == "Market"));//
            MessageResponse(message);
        }

        /// <summary>
        /// Ответ бота на сообщение пользователя
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        private void MessageResponse(Message message)
        {
            var command = message.Text.ToLower();
            var userId = message.FromId.Value;
            if (!AllUserSessions.ContainsKey(userId))
                AllUserSessions.Add(userId, new UserSession(userId, this));
            if ((command == "таблица" || command == "принять" || command == "отклонить")
                && !AdminIds.Contains(userId))
                command = "unknown";
            if (!CommandsPhrases.Commands.ContainsKey(command))
                command = "unknown";
            CommandsPhrases.CommandsActions[CommandsPhrases.Commands[command]].Invoke(this, message);
        }

        /// <summary>
        /// Написать пользователю сообщение с адресом доставки
        /// </summary>
        /// <param name="userId">Id получателя</param>
        /// <param name="message">Сообщение</param>
        /// <param name="keyboard">Клавиатура бота</param>
        public void WriteToSelectedUserWithDeliveryAddress(long userId, string message,
            MessageKeyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = MessageKeyboardSchemes.SetEmptyKeyboard;
            }
            var messageSendParams = new MessagesSendParams
            {
                UserId = userId,
                Message = message,
                RandomId = new Random().Next(999999),
                Keyboard = keyboard,
                Lat = AllUserSessions[userId].OrderData.DeliveryAddress.Coordinates.Latitude,
                Longitude = AllUserSessions[userId].OrderData.DeliveryAddress.Coordinates.Longitude
            };
            vk.Messages.Send(messageSendParams);
            AllUserSessions[userId].PrevKeyboard = keyboard;
            AllUserSessions[userId].LastBotMessage = messageSendParams;
        }

        /// <summary>
        /// Отправить администратору сообщение о поступлении нового заказа
        /// </summary>
        /// <param name="adminId">Id администратору, которому нужно отправить сообщение</param>
        /// <param name="customerId">Id пользователя, который сделал заказ</param>
        /// <param name="message">Сообщение</param>
        /// <param name="keyboard">Клавиатура бота</param>
        public void WriteToAdminWithDeliveryInfo(long adminId, long customerId,
            string message, MessageKeyboard keyboard = null)
        {
            if (keyboard == null)
            {
                keyboard = MessageKeyboardSchemes.SetEmptyKeyboard;
            }
            var messageSendParams = new MessagesSendParams
            {
                UserId = adminId,
                Message = message,
                RandomId = new Random().Next(999999),
                Keyboard = keyboard,
                Lat = AllUserSessions[customerId].OrderData.DeliveryAddress.Coordinates.Latitude,
                Longitude = AllUserSessions[customerId].OrderData.DeliveryAddress.Coordinates.Longitude
            };
            vk.Messages.Send(messageSendParams);
            if (!AllUserSessions.ContainsKey(adminId))
                AllUserSessions.Add(adminId, new UserSession(adminId, this));
            AllUserSessions[adminId].PrevKeyboard = keyboard;
            AllUserSessions[adminId].LastBotMessage = messageSendParams;
        }

        /// <summary>
        /// Оформление заказа в "ручном" режиме, то есть с помощью администратора
        /// </summary>
        /// <param name="userId">Id пользователя, который делает заказ</param>
        public void ManualOrderSendOrderInfoToAdmin(long userId)
        {
            var productsWithCost = "Выбранные товары: \n";
            productsWithCost += GetProductsWithCosts(userId);
            var message = ResponsePhrases.OrderConfirmationForAdmin + $"[id{userId}|{GetFullUserName(userId)}] \n" + productsWithCost;
            WriteToSelectedUser(userId, message, null);
        }

        /// <summary>
        /// Оповещение всех администраторов о поступлении нового заказа от пользователя с указанным id
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        public void NotifyAdminsAboutNewQuestion(long userId)
        {
            foreach (var adminId in AdminIds)
            {
                var message = $"Новый вопрос от [id{userId}|{GetFullUserName(userId)}]";
                WriteToSelectedUser(adminId, message, null);
            }
        }

        /// <summary>
        /// Отправка всех данных о заказе пользователя в таблицу заказов
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        public void SendOrderDataToTable(long userId)
        {
            var productsWithCost = "";
            var allProducts = AllUserSessions[userId].OrderData.Products;
            for (int i = 0; i < allProducts.Count; i++)
            {
                productsWithCost += $"{allProducts[i].Title} ({allProducts[i].Price.Text})";
                if (i + 1 < allProducts.Count)
                    productsWithCost += "\n";
            }
            var orderData = new List<IList<object>>
            {
                new List<object>
                {
                    AllUserSessions[userId].OrderData.OrderId,
                    DateTime.Now.ToString("dd.MM.yy") + " " + DateTime.Now.ToString("HH:mm"),
                    productsWithCost,
                    userId,
                    "=HYPERLINK(\"https://vk.com/id" + userId + $" \"; \"{ GetFullUserName(userId)}\")",
                    AllUserSessions[userId].OrderData.TotalCost + " руб.",
                    "Оплата " + AllUserSessions[userId].OrderData.PaymentMethod,
                    AllUserSessions[userId].OrderData.DeliveryDateTime.ToString("dd.MM.yy") + " " + AllUserSessions[userId].OrderData.DeliveryDateTime.ToString("HH:mm"),
                    "=HYPERLINK(\"https://www.google.com/maps/search/?api=1&query=" +
                    AllUserSessions[userId].OrderData.DeliveryAddress.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture) + "," +
                    AllUserSessions[userId].OrderData.DeliveryAddress.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture) +
                    "\"; \"Ссылка на карту\")",
                    AllUserSessions[userId].OrderData.OrderStatus
                }
            };
            SpreadSheetBot.AppendRangeValue(orderData);
        }

        /// <summary>
        /// Получить все товары пользователя со стоимостью
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        public string GetProductsWithCosts(long userId)
        {
            var productsWithCost = "";
            foreach (var product in AllUserSessions[userId].OrderData.Products)
            {
                productsWithCost += $"— {product.Title} ({product.Price.Amount / 100} руб.)\n";
            }
            return productsWithCost;
        }

        /// <summary>
        /// Изменить статус заказа
        /// </summary>
        /// <param name="order">Заказ, статус которого нужно изменить</param>
        /// <param name="newStatus">Новый статус заказа</param>
        public void ChangeOrderStatus(OrderData order, string newStatus)
        {
            var table = SpreadSheetBot.ReadAllEntries();
            var rowIndex = 0;
            foreach (var row in table)
            {
                rowIndex++;
                if (row[0].ToString() == order.OrderId)
                    break;
            }
            SpreadSheetBot.ChangeCellValue(10, rowIndex, newStatus);
        }

        /// <summary>
        /// Получить список всех заказов пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        public string GetUserOrders(long userId)
        {
            var table = SpreadSheetBot.ReadAllEntries();
            var rowIndex = 0;
            var ordersWithCosts = "";
            foreach (var row in table)
            {
                rowIndex++;
                if (row[3].ToString() == userId.ToString() && row[9].ToString() != "Выполнен")
                    ordersWithCosts += $"Номер заказа: {row[0].ToString()} \n" +
                        $"Список товаров: \n{row[2].ToString()} \n" +
                        $"Общая стоимость: {row[5].ToString()}\n" +
                        $"{row[6].ToString()} \n" +
                        $"Дата и время доставки: {row[7].ToString()}\n" +
                        $"Статус заказа: {row[9].ToString()}\n\n";
            }
            return ordersWithCosts;
        }
    }
}
