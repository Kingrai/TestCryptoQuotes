using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;

namespace TestCryptoQuotes.ViewModels
{
    public class CryptoClients
    {
        public ITickerClient TickerClient { get; private set; }
        public IRequestClient RequestClient { get; private set; }

        public CryptoClients(ITickerClient tickerClient, IRequestClient requestClient)
        {
            TickerClient = tickerClient;
            RequestClient = requestClient;
        }

    }
}
