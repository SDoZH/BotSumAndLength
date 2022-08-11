using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotSumAndLength.Servises;

namespace BotSumAndLength.Controllers
{
    public class InlineKeyboardController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;
        private static CallbackQuery callback;
        public InlineKeyboardController(ITelegramBotClient telegramClient, IStorage memoryStorage)
        {
            _telegramClient = telegramClient;
            _memoryStorage = memoryStorage;
            callback = new CallbackQuery();
        }
        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            string Function = callbackQuery.Data switch
            {
                "считаем" => " Подсчёт количества символов в тексте",
                "складываем" => " Вычисление суммы чисел",
                _ => String.Empty
            };
            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>Режим - {Function}.{Environment.NewLine}</b>" +
                $"{Environment.NewLine}Можно поменять в главном меню.", cancellationToken: ct, parseMode: ParseMode.Html);
            if (callbackQuery?.Data == null)
                return;

            else if (callbackQuery.Data == "считаем")
            {
                callback = callbackQuery;
                // Обновление пользовательской сессии новыми данными
                // _memoryStorage.GetSession(callbackQuery.From.Id).Choice = callbackQuery.Data;
                callback = callbackQuery;
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, $"Посчитаем колличество символов в предложении \nВведите предложение: ", cancellationToken: ct, parseMode: ParseMode.Html);
            }
            else if (callbackQuery.Data == "складываем")
            {
                callback = callbackQuery;
                // Обновление пользовательской сессии новыми данными
                _memoryStorage.GetSession(callbackQuery.From.Id).Config = callbackQuery.Data;
                await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id, $"Вычислим сумму ваших чисел \nВведите числа через пробел: ", cancellationToken: ct, parseMode: ParseMode.Html);
            }
        }
        public static CallbackQuery GetCallbackQuery()
        {
            return callback;
        }

    }
}
