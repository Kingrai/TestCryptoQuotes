using CryptoExchange.Net.Objects.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.SubscribeManager
{

    public class SubscribeManagerClass
    {
        private ITickerClient _tickerClient;
        private CancellationTokenSource _cancellationTokenSource;
        private UpdateSubscription _currentSubscription;
        private string _exchangeName;

        public SubscribeManagerClass(string exchangeName, ITickerClient tickerClient)
        {
            _tickerClient = tickerClient;
            _exchangeName = exchangeName;
        }


        public async Task<InfoSymbolModel> SubscribeToTickerUpdatesAsync(string symbol, string pair)
        {
            var infoSymbol = new InfoSymbolModel
            {
                Exchange = _exchangeName,
            };
            await UnsubscribeAsync();
            _cancellationTokenSource = new CancellationTokenSource();

            _currentSubscription = await _tickerClient.SubscribeToTickerUpdatesAsync(symbol, pair, (price, timestamp) =>
            {
                infoSymbol.LastPrice = price;
                infoSymbol.UpdateDT = timestamp;
            }, _cancellationTokenSource.Token);

            return infoSymbol;
        }
        public async Task UnsubscribeAsync()
        {
            if (_currentSubscription != null)
                await _currentSubscription.CloseAsync();
            _cancellationTokenSource?.Cancel();
        }
    }
}
