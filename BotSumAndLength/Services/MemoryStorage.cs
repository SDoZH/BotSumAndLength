using System.Collections.Concurrent;
using BotSumAndLength.Models;
using BotSumAndLength.Servises;
using BotSumAndLength.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotSumAndLength.Services
{
    public class MemoryStorage : IStorage
    {
        /// <summary>
        /// Хранилище сессий
        /// </summary>
        private readonly ConcurrentDictionary<long, Session> _sessions;
        private static Session newSession;
        public MemoryStorage()
        {
            _sessions = new ConcurrentDictionary<long, Session>();

        }

        public Session GetSession(long chatId)
        {
            // Возвращаем сессию по ключу, если она существует
            if (_sessions.ContainsKey(chatId))
            {
                newSession.Config = InlineKeyboardController.GetCallbackQuery().Data;
                return _sessions[chatId];
            }


            // Создаем и возвращаем новую, если такой не было
            newSession = new Session();
            _sessions.TryAdd(chatId, newSession);
            return newSession;
        }

        public static string GetChoice()
        {
            return newSession.Config;
        }
    }
}
