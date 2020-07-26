using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace WebBot
{
    /// <summary>
    /// Пользовательская сессия
    /// </summary>
    class UserSession
    {
        public OrderData OrderData { get; set; }

        public List<Attachment> ProductsAsAttachments { get; set; } = new List<Attachment>();
        public MessageKeyboard PrevKeyboard { get; set; }
        public MessagesSendParams LastBotMessage { get; set; }
        public long SessionOwner { get; set; }

        public Timer SessionTime { get; private set; }
        public VkApiBot VkBot { get; }

        /// <summary>
        /// Создание новой сессии пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="bot">ВК бот</param>
        public UserSession(long userId, VkApiBot bot)
        {
            SessionOwner = userId;
            OrderData = new OrderData(userId);
            VkBot = bot;
            PrevKeyboard = MessageKeyboardSchemes.DefaultButtons;
            SetTimer();
            Logger.Info($"Начата сессия с пользователем id{userId}");
        }

        /// <summary>
        /// Установка таймера сессии
        /// </summary>
        private void SetTimer()
        {
            SessionTime = new Timer(10 * 60 * 1000); //Длина сессии 10 минут
            SessionTime.Elapsed += SessionEnd;
            SessionTime.AutoReset = true;
            SessionTime.Enabled = true;
        }

        /// <summary>
        /// Окончание сессии
        /// </summary>
        private void SessionEnd(object source, ElapsedEventArgs e)
        {
            SessionTime.Stop();
            //Лучше не писать об окончании сессии, потому что бот будет спамить людей
            //VkBot.WriteToSelectedUser(SessionOwner, "Сессия окончена", MessageKeyboardSchemes.DefaultButtons);
            VkBot.AllUserSessions.Remove(SessionOwner);
            Logger.Info($"Закончена сессия пользователя id{SessionOwner}");
        }
    }
}
