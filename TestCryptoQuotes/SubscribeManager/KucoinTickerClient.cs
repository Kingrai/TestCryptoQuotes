using System;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects.Sockets;
using Kucoin.Net.Clients;
using TestCryptoQuotes.Interfaces;

namespace TestCryptoQuotes.SubscribeManager
{
    public class KucoinTickerClient : ITickerClient
    {
        private readonly KucoinSocketClient _socketClient;

        public KucoinTickerClient()
        {
            _socketClient = new KucoinSocketClient();
        }

        public async Task<UpdateSubscription> SubscribeToTickerUpdatesAsync(string symbol, string pair, Action<decimal, DateTime> onUpdate, CancellationToken cancellationToken)
        {
            var result = await _socketClient.SpotApi.SubscribeToTickerUpdatesAsync(symbol + "-" + pair, update =>
            {
                onUpdate(update.Data.LastPrice ?? 0, DateTime.Now);
            });

            if (!result.Success)
                throw new Exception($"Ошибка подписки на токен {symbol+pair}:\n{result.Error}");

            return result.Data;
        }

        public Task<bool> PingAsync()
        {
            throw new NotImplementedException();
        }
    }
}
