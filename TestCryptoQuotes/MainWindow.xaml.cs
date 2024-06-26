using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;

using TestCryptoQuotes.ViewModels;
using TestCryptoQuotes.SubscribeManager;
using TestCryptoQuotes.Interfaces;
using Microsoft.Extensions.Logging;

using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using Binance.Net.Clients;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using Kucoin.Net.Clients;
using Bybit.Net.Clients;
using Bitget.Net.Clients;
using TestCryptoQuotes.RequestSymbolManager;
using System.Threading;


namespace TestCryptoQuotes
{
    public partial class MainWindow : Window
    {

        MultiExchangeSubscriptionManager stackExchange = new MultiExchangeSubscriptionManager();
        ObservableCollection<InfoSymbolModel> listInfoSymbols = new ObservableCollection<InfoSymbolModel>();

        Dictionary<string, CryptoClients> dictExchange = new Dictionary<string, CryptoClients>()
        {
            { "Binance",    new CryptoClients(new BinanceTickerClient(),new BinanceRequestClient()) },
            { "Kucoin",     new CryptoClients(new KucoinTickerClient(), new KucoinRequestClient()) },
            { "Bitget",     new CryptoClients(new BitgetTickerClient(), new BitgetRequestClient()) },
            { "Bybit",      new CryptoClients(new BybitTickerClient(),  new BybitRequestClient())  }

        };
        List<NameSymbolModel> nameSymbolList;
        NameSymbolModel currentNameSymbol;
        bool glbFlgStartPingThread = false;

        Thread pingThread;

        public MainWindow()
        {
            InitializeComponent();
            currentNameSymbol = new NameSymbolModel(dictExchange.Keys.ToList()) { BaseAsset = "BTC", QuoteAsset = "USDT" };
            lblCurNameSymbol.DataContext = currentNameSymbol;
            AddMultipleClientTicker(dictExchange);
            SubscriptionToMultipleExchanges(currentNameSymbol);
            LoadSymbolsAsync(dictExchange);
            lbInfo.ItemsSource = listInfoSymbols;
            pingThread = new Thread(async () =>
            {
                while (true)
                {
                    if (listInfoSymbols.Count == dictExchange.Count && glbFlgStartPingThread)
                    {
                        var tempList =  new List<InfoSymbolModel>(listInfoSymbols);
                        var tempNameSymbol = currentNameSymbol;
                        foreach (var infoSymbol in tempList)
                        {
                            bool isConnect = true;
                            try
                            {
                                isConnect = await dictExchange[infoSymbol.Exchange].RequestClient.CheckConnectWithServer();
                            }
                            catch
                            {
                                isConnect = false;
                            }
                            UpdateInfoAfterConnect(infoSymbol.IsConnect, isConnect, infoSymbol.IsTraiding, infoSymbol.Exchange);
                            listInfoSymbols.First(x => x.Exchange == infoSymbol.Exchange).IsConnect = isConnect;
                            
                            if ((infoSymbol.IsTraiding) && infoSymbol.IsConnect)
                            {
                                bool pong = infoSymbol.IsConnect;
                                if (infoSymbol.LastPingDT != DateTime.MinValue && (DateTime.Now - infoSymbol.LastPingDT).TotalMinutes >= 10)
                                {
                                    try
                                    {
                                        pong = await dictExchange[infoSymbol.Exchange].TickerClient.PingAsync();
                                        listInfoSymbols.First(x => x.Exchange == infoSymbol.Exchange).LastPingDT = DateTime.Now;

                                    }
                                    catch (NotImplementedException)
                                    {
                                        listInfoSymbols.First(x => x.Exchange == infoSymbol.Exchange).LastPingDT = DateTime.Now;
                                    }
                                    catch
                                    {
                                        pong = false;
                                        listInfoSymbols.First(x => x.Exchange == infoSymbol.Exchange).Info = "Не пингуется";
                                    }
                                }
                                if ((infoSymbol.UpdateDT != DateTime.MinValue && (DateTime.Now - infoSymbol.UpdateDT).TotalSeconds > 5) || !pong)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        _ = stackExchange.UnsubscribeAsync(infoSymbol.Exchange);
                                        SubsciptionToExchangeAsync(infoSymbol.Exchange,
                                                                dictExchange[infoSymbol.Exchange].RequestClient,
                                                                tempNameSymbol);
                                    });
                                }
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            
            pingThread.Start();
        }

        private void UpdateInfoAfterConnect(bool lastConnect,bool curConnect, bool isTraiding, string nameExchange)
        {
            string info = null;
            if (lastConnect == curConnect)
                return;
            if (!curConnect)
                info = "Нет соединения";
            else if (!isTraiding)
                info = "Нет токена";
            listInfoSymbols.First(x => x.Exchange == nameExchange).Info = info;

        }

        public async void LoadSymbolsAsync(Dictionary<string, CryptoClients> _dictExchange)
        {
            List<NameSymbolModel> lastSymbolList = new List<NameSymbolModel>();
            foreach (var exchange in _dictExchange)
            {
                try
                {
                    var symbolList = await exchange.Value.RequestClient.GetTraidingSymbols(exchange.Key);
                    if (symbolList != null && symbolList.Count > 0)
                    {
                        if (lastSymbolList.Count == 0)
                            lastSymbolList = symbolList;
                        else
                        {
                            lastSymbolList = lastSymbolList.Concat(symbolList)
                                .GroupBy(x=> x.DisplaySymbol)
                                .Select(g=>
                                {
                                    var combined = g.First();
                                    foreach (var item in g.Skip(1))
                                    {
                                        combined.AddListExchange(item.GetExchange());
                                    }
                                    return combined;
                                }).ToList();
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            nameSymbolList = lastSymbolList.OrderBy(s => s.DisplaySymbol).ToList();
            if (nameSymbolList.Count != 0)
            {
                cmbNameSymbol.ItemsSource = nameSymbolList;
                cmbNameSymbol.SelectedIndex = nameSymbolList.FindIndex(x => x.DisplaySymbol == currentNameSymbol.DisplaySymbol);
                cmbNameSymbol.SelectionChanged += cmbNameSymbol_SelectionChanged;
                glbFlgStartPingThread = true;
            }
            else
            {
                MessageBox.Show("Список инструментов пуст. Перезапустите программу.");
                this.Close();
            }
        }

        private bool AddMultipleClientTicker(Dictionary<string, CryptoClients> _dictExchange)
        {
            try
            {
                foreach (var exchange in _dictExchange)
                {
                    stackExchange.AddClient(exchange.Key, exchange.Value.TickerClient);
                    listInfoSymbols.Add(new InfoSymbolModel() { Exchange = exchange.Key, UpdateDT = DateTime.Now, LastPrice = 0, IsTraiding = false, IsConnect = true });
                }
            }
            catch(Exception ex) { MessageBox.Show($"Невозможно добавить клиента из-за отсутсвтия подключения к бирже"); return false; }
            return true;
        }

        public void SubscriptionToMultipleExchanges(NameSymbolModel nameSymbol)
        {
            foreach (var exchange in dictExchange)
            {
                _ = stackExchange.UnsubscribeAsync(exchange.Key);
                SubsciptionToExchangeAsync(exchange.Key, exchange.Value.RequestClient, nameSymbol);
            }
        }

        public async void SubsciptionToExchangeAsync(string nameExchange, IRequestClient nameClient, NameSymbolModel nameSymbol)
        {
            int index = listInfoSymbols.ToList().FindIndex(x => x.Exchange == nameExchange);
            if (index == -1)
                return;
            if (!nameSymbol.GetExchange().Contains(nameExchange))
            {
                listInfoSymbols[index] = new InfoSymbolModel();
                listInfoSymbols[index].Exchange = nameExchange;
                listInfoSymbols[index].Info = "Нет токена";
                listInfoSymbols[index].LastPingDT = DateTime.Now;
                listInfoSymbols[index].IsTraiding = false;
                listInfoSymbols[index].TickSize = 0;
                listInfoSymbols[index].LastPrice = 0;
                return;
            }
            try
            {
                var subscription = await stackExchange.SubscribeToTickerUpdatesAsync(nameExchange, nameSymbol.BaseAsset, nameSymbol.QuoteAsset);
                listInfoSymbols[index] = subscription;
                listInfoSymbols[index].LastPrice = await nameClient.GetLastPriceForSymbol(nameSymbol.BaseAsset, nameSymbol.QuoteAsset);
                listInfoSymbols[index].TickSize = await nameClient.GetTickSizeForSymbol(nameSymbol.BaseAsset, nameSymbol.QuoteAsset);
                listInfoSymbols[index].IsTraiding = true;
                listInfoSymbols[index].LastPingDT = DateTime.Now;
                listInfoSymbols[index].Info = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подписки {nameExchange}" + ex.Message);
                listInfoSymbols[index].Info = "Ошибка подписки";
                listInfoSymbols[index].IsTraiding = false;
                listInfoSymbols[index].TickSize = 0;
                listInfoSymbols[index].LastPrice = 0;
            }
        }

        public void OnAutoCompleteComboBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = cmbNameSymbol.Text;
            if (string.IsNullOrEmpty(filter))
            {
                cmbNameSymbol.ItemsSource = nameSymbolList;
            }
            else
            {
                cmbNameSymbol.ItemsSource = nameSymbolList
                    .Where(x => x.DisplaySymbol.StartsWith(filter, StringComparison.OrdinalIgnoreCase)).OrderBy(x=> x.DisplaySymbol)
                    .ToList();
            }
            cmbNameSymbol.IsDropDownOpen = true;
            
        }

        private void cmbNameSymbol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameSymbolModel selectedSymbol = cmbNameSymbol.SelectedItem as NameSymbolModel;
            if (selectedSymbol != null && selectedSymbol != currentNameSymbol) 
            {
                currentNameSymbol.Copy(selectedSymbol);
                SubscriptionToMultipleExchanges(currentNameSymbol);
                lbInfo.ItemsSource = listInfoSymbols;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            pingThread.Abort();
            pingThread = null;

            foreach (var exchange in dictExchange)
            {
                _ = stackExchange.UnsubscribeAsync(exchange.Key);
            }
        }

    }


}
