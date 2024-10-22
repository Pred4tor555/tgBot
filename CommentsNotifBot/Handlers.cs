using System.Text.RegularExpressions;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Examples.Polling
{

    public class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            
            string urlPattern = @"(http|https)://[^\s]+|www\.[^\s]+";
            bool containsUrl = false;
            
            if (message.Type == MessageType.Text)
            {
                containsUrl = System.Text.RegularExpressions.Regex.IsMatch(message.Text, urlPattern, RegexOptions.IgnoreCase);
            }
            else if (message.Type == MessageType.Photo)
            {
                containsUrl = System.Text.RegularExpressions.Regex.IsMatch(message.Caption, urlPattern, RegexOptions.IgnoreCase);
            }
            else if (message.Type == MessageType.Contact)
            {
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            }
            else
            {
                return;
            }
            
            
            if (containsUrl)
                await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
    }
}