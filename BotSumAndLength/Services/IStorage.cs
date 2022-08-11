using System;
using System.Collections.Generic;
using System.Linq;
using BotSumAndLength.Models;
using BotSumAndLength.Controllers;

namespace BotSumAndLength.Servises
{
    public interface IStorage
    {
        /// <summary>
        /// Получение сессии пользователя по идентификатору
        /// </summary>
        Session GetSession(long chatId);
    }
}
