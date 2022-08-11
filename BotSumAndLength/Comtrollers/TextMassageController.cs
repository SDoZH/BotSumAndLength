using Telegram.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BotSumAndLength.Configuration;
using BotSumAndLength.Models;
using BotSumAndLength.Services;

namespace BotSumAndLength.Controllers
{
    public class TextMessageController : MemoryStorage
    {
        private readonly ITelegramBotClient _telegramClient;
        private InlineKeyboardController _inlineKeyboard;
        private CallbackQuery callbackQuery;
        private Session _session;

        public TextMessageController(ITelegramBotClient telegramBotClient, InlineKeyboardController inlineKeyboard)
        {
            _telegramClient = telegramBotClient;
            _inlineKeyboard = inlineKeyboard;
            callbackQuery = new CallbackQuery();
        }
        public async Task Handle(Message message, CallbackQuery callbackQuery, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"Считаем колличество символов" , $"считаем"),
                        InlineKeyboardButton.WithCallbackData($"Суммируем цифры" , $"складываем")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b> BotSumAndLength может выполнять две функции:</b> {Environment.NewLine}" +
                        $"{Environment.NewLine}Подсчёт количества символов в тексте и вычисление суммы чисел, которые вы ему отправляете (одним сообщением через пробел).Что будем делать?{Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                    break;

                default:
                    callbackQuery.Data = GetChoice();
                    switch (callbackQuery.Data)
                    {
                        case "считаем":
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Колличество символов в вашем предложении: {message.Text.Length}");
                            return;
                        case "складываем":
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел равна: {GetSum(message)}");
                            return;
                    }
                    await _inlineKeyboard.Handle(callbackQuery, ct);
                    break;
            }
        }
        private int GetSum(Message e)
        {
            string tempmessage = e.Text;
            char separator = ' ';
            string[] tempArr = tempmessage.Split(separator);
            int result = 0;
            foreach (var item in tempArr)
            {
                int temp = Convert.ToInt32(item);
                result += temp;
            }
            return result;
        }
    }
}