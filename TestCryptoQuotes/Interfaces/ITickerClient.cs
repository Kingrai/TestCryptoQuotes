using Binance.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestCryptoQuotes.Interfaces
{
    public interface ITickerClient 
    { 
        Task<UpdateSubscription> SubscribeToTickerUpdatesAsync(string symbol,
                                                        string pair,
                                                        Action<decimal, DateTime> onUpdate, 
                                                        CancellationToken cancellationToken);
        Task<bool> PingAsync();

    }
}
