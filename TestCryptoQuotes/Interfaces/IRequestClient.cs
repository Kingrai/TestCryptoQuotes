using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.Interfaces
{
    public interface IRequestClient
    {
        Task<List<NameSymbolModel>> GetTraidingSymbols(string _exchangeName);

        Task<decimal> GetTickSizeForSymbol(string symbol, string pair);

        Task<decimal> GetLastPriceForSymbol(string symbol, string pair);

        Task<bool> CheckConnectWithServer();
    }

}
