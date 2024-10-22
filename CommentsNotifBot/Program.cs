using Telegram.Bot;
using Telegram.Bot.Examples.Polling;
using Telegram.Bot.Polling;

namespace commentsNotifBot
{
    class Program
    {
        private static string token { get; set; } = "7693060914:AAFc25IVpvRwCZWxnFcdYSTdXYx4Ho2eu4I";
        private static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient(token);

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            Bot.StartReceiving(Handlers.HandleUpdateAsync,
                Handlers.HandleErrorAsync,
                receiverOptions,
                cts.Token);

            Console.WriteLine($"Бот запущен и ждет сообщения...");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}