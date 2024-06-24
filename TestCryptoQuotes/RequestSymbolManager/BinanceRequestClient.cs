using Binance.Net.Clients;
using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.RequestSymbolManager
{
    public class BinanceRequestClient: IRequestClient
    {
        private readonly BinanceRestClient _restClient;

        public BinanceRequestClient()
        {
            _restClient = new BinanceRestClient();
        }

        public async Task<List<NameSymbolModel>> GetTraidingSymbols(string _exchangeName)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetExchangeInfoAsync();
            if (!result.Success)
                throw new Exception("Ошибка запроса информации по токенам с биржи. Информация по ошибке:\n" + result.Error);
            List<NameSymbolModel> symbols = result.Data.Symbols.Where(r=> r.Status == Binance.Net.Enums.SymbolStatus.Trading)
                    .Select(s => new NameSymbolModel(_exchangeName) { BaseAsset = s.BaseAsset, QuoteAsset = s.QuoteAsset})
                    .OrderBy(s => s.DisplaySymbol).ToList();
            return symbols;
        }

        public async Task<decimal> GetTickSizeForSymbol(string symbol, string pair)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetExchangeInfoAsync(symbol + pair);
            if (!result.Success || result.Data.Symbols.Count() == 0)
                throw new Exception($"Ошибка запроса шага цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal tickSize = result.Data.Symbols.ElementAt(0).PriceFilter.TickSize;
            return tickSize;
        }

        public async Task<decimal> GetLastPriceForSymbol(string symbol, string pair)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetPriceAsync(symbol + pair);
            if (!result.Success)
                throw new Exception($"Ошибка запроса последней цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal lastPrice = result.Data.Price;
            return lastPrice;
        }

        public async Task<bool> CheckConnectWithServer()
        {
            var result = await _restClient.SpotApi.ExchangeData.GetServerTimeAsync();
            if (!result.Success)
                return false;                
            return true;
        }
    }
}
