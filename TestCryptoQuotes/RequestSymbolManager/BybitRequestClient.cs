using Bybit.Net.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.RequestSymbolManager
{
    internal class BybitRequestClient : IRequestClient
    {
        private readonly BybitRestClient _restClient;

        public BybitRequestClient()
        {
            _restClient = new BybitRestClient();
        }

        public async Task<List<NameSymbolModel>> GetTraidingSymbols(string _exchangeName)
        {
            var result = await _restClient.V5Api.ExchangeData.GetSpotSymbolsAsync();
            if (!result.Success)
                throw new Exception("Ошибка запроса информации по токенам с биржи. Информация по ошибке:\n" + result.Error);
            List<NameSymbolModel> symbols = result.Data.List.Where(r => r.Status == Bybit.Net.Enums.SymbolStatus.Trading)
                                    .Select(s => new NameSymbolModel(_exchangeName) { BaseAsset = s.BaseAsset, QuoteAsset = s.QuoteAsset })
                                    .OrderBy(s => s.DisplaySymbol).ToList();
            return symbols;
        }

        public async Task<decimal> GetTickSizeForSymbol(string symbol, string pair)
        {
            var result = await _restClient.V5Api.ExchangeData.GetSpotSymbolsAsync(symbol + pair);
            if (!result.Success || result.Data.List.Count() == 0)
                throw new Exception($"Ошибка запроса шага цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal tickSize = result.Data.List.ElementAt(0).PriceFilter.TickSize;
            return tickSize;
        }

        public async Task<decimal> GetLastPriceForSymbol(string symbol, string pair)
        {
            var result = await _restClient.V5Api.ExchangeData.GetSpotTickersAsync(symbol + pair);
            if (!result.Success)
                throw new Exception($"Ошибка запроса последней цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal lastPrice = result.Data.List.ElementAt(0).LastPrice;
            return lastPrice;
        }

        public async Task<bool> CheckConnectWithServer()
        {
            var result = await _restClient.V5Api.ExchangeData.GetServerTimeAsync();
            if (!result.Success)
                return false;
            return true;
        }
    }
}
