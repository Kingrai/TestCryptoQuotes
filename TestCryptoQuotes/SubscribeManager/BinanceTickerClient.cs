using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Clients;
using CryptoExchange.Net.Objects.Sockets;
using TestCryptoQuotes.Interfaces;


namespace TestCryptoQuotes.SubscribeManager
{
    public class BinanceTickerClient : ITickerClient
    {
        private readonly BinanceSocketClient _socketClient;

        public BinanceTickerClient()
        {
            _socketClient = new BinanceSocketClient();
        }

        public async Task<UpdateSubscription> SubscribeToTickerUpdatesAsync(string symbol, string pair, Action<decimal, DateTime> onUpdate, CancellationToken cancellationToken)
        {
            var result = await _socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync(symbol + pair, update =>
            {
                onUpdate(update.Data.LastPrice, DateTime.Now);
            });

            if (!result.Success)
                throw new Exception($"Ошибка подписки на токен {symbol + pair}:\n{result.Error}");

            return result.Data;
        }

        public async Task<bool> PingAsync()
        {
            var result = await _socketClient.SpotApi.ExchangeData.PingAsync();
            if (!result.Success)
                throw new Exception("Не удалось выполнить команду ping:\n" + result.Error);
            if (result.Data.Status != 200)
                return false;
            return true;
        }
    }
}
