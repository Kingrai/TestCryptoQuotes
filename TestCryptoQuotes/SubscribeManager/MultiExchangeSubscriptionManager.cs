using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;
using TestCryptoQuotes.SubscribeManager;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.SubscribeManager
{
    public class MultiExchangeSubscriptionManager
    {
        private readonly Dictionary<string, SubscribeManagerClass> _subscriptionManagers;

        public MultiExchangeSubscriptionManager()
        {
            _subscriptionManagers = new Dictionary<string, SubscribeManagerClass>();
        }

        public void AddClient(string exchangeName, ITickerClient tickerClient)
        {
            var manager = new SubscribeManagerClass(exchangeName, tickerClient);
            _subscriptionManagers[exchangeName] = manager;
        }

        public async Task<InfoSymbolModel> SubscribeToTickerUpdatesAsync(string exchangeName, string symbol, string pair)
        {
            if (_subscriptionManagers.TryGetValue(exchangeName, out var manager))
            {
                return await manager.SubscribeToTickerUpdatesAsync(symbol, pair);
            }
            throw new Exception($"Клиент биржи {exchangeName} не найден.");
        }

        public async Task UnsubscribeAsync(string exchangeName)
        {
            if (_subscriptionManagers.TryGetValue(exchangeName, out var manager))
            {
                await manager.UnsubscribeAsync();
            }
        }
    }
}
