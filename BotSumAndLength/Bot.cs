using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotSumAndLength.Controllers;
using BotSumAndLength.Models;
using BotSumAndLength.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BotSumAndLength
{
    public class Bot : BackgroundService
    {

        private ITelegramBotClient _telegramClient;
        private TextMessageController _textMessageController;
        private InlineKeyboardController _inlineKeyboardController;
        private Session _session;
        private MemoryStorage _memoryStorage;
        private CallbackQuery _callbackQuery;

        public Bot(ITelegramBotClient telegramClient, TextMessageController textMessageController, InlineKeyboardController inlineKeyboardController, Session session, MemoryStorage memoryStorage, CallbackQuery callbackQuery)
        {
            _telegramClient = telegramClient;
            _textMessageController = textMessageController;
            _inlineKeyboardController = inlineKeyboardController;
            _session = session;
            _memoryStorage = memoryStorage;
            _callbackQuery = callbackQuery;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, // Здесь выбираем, какие обновления хотим получать. В данном случае разрешены все
                cancellationToken: stoppingToken);

            Console.WriteLine("Бот запущен");
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.CallbackQuery)
            {
                _callbackQuery = update.CallbackQuery;
                await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                return;
            }

            
            if (update.Type == UpdateType.Message)
            {
                _session = _memoryStorage.GetSession(update.Message.Chat.Id);
                await _textMessageController.Handle(update.Message, _callbackQuery, cancellationToken);
                return;
            }

        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Выводим в консоль информацию об ошибке
            Console.WriteLine(errorMessage);

            // Задержка перед повторным подключением
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
            Thread.Sleep(10000);

            return Task.CompletedTask;
        }


    }
}