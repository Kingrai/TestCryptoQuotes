﻿using Kucoin.Net.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCryptoQuotes.Interfaces;
using TestCryptoQuotes.ViewModels;

namespace TestCryptoQuotes.RequestSymbolManager
{
    public class KucoinRequestClient: IRequestClient
    {
        private readonly KucoinRestClient _restClient;

        public KucoinRequestClient()
        {
            _restClient = new KucoinRestClient();
        }

        public async Task<List<NameSymbolModel>> GetTraidingSymbols(string _exchangeName)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetSymbolsAsync();
            if (!result.Success)
                throw new Exception("Ошибка запроса информации по токенам с биржи. Информация по ошибке:\n" + result.Error);
            List<NameSymbolModel> symbols = result.Data.Where(r => r.EnableTrading == true)
                                    .Select(s => new NameSymbolModel(_exchangeName) { BaseAsset = s.BaseAsset, QuoteAsset = s.QuoteAsset })
                                    .OrderBy(s => s.DisplaySymbol).ToList();
            return symbols;

        }

        public async Task<decimal> GetTickSizeForSymbol(string symbol, string pair)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetSymbolsAsync();
            if (!result.Success)
                throw new Exception($"Ошибка запроса шага цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal tickSize = result.Data.FirstOrDefault(r => r.BaseAsset + r.QuoteAsset == symbol + pair).PriceIncrement;
            return tickSize;
        }

        public async Task<decimal> GetLastPriceForSymbol(string symbol, string pair)
        {
            var result = await _restClient.SpotApi.ExchangeData.GetTickerAsync(symbol + "-" + pair);
            if (!result.Success)
                throw new Exception($"Ошибка запроса последней цены по токену {symbol} с биржи. Информация по ошибке:\n" + result.Error);
            decimal lastPrice = result.Data.LastPrice ?? 0;
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
