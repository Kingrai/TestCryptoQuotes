using System;
using System.Threading;
using System.Threading.Tasks;
using Bybit.Net.Clients;
using CryptoExchange.Net.Objects.Sockets;
using TestCryptoQuotes.Interfaces;


namespace TestCryptoQuotes.SubscribeManager
{
    public class BybitTickerClient : ITickerClient
    {
        private readonly BybitSocketClient _socketClient;

        public BybitTickerClient()
        {
            _socketClient = new BybitSocketClient();
        }

        public async Task<UpdateSubscription> SubscribeToTickerUpdatesAsync(string symbol, string pair, Action<decimal, DateTime> onUpdate, CancellationToken cancellationToken)
        {
            var result = await _socketClient.V5SpotApi.SubscribeToTickerUpdatesAsync(symbol + pair, update =>
            {
                onUpdate(update.Data.LastPrice, DateTime.Now);
            });

            if (!result.Success)
                throw new Exception($"Ошибка подписки на токен {symbol + pair}:\n{result.Error}");

            return result.Data;
        }

        public Task<bool> PingAsync()
        {
            throw new NotImplementedException();
        }
    }
}
