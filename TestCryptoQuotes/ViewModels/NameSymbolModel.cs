using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace TestCryptoQuotes.ViewModels
{
    public class NameSymbolModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public NameSymbolModel() { }
        public NameSymbolModel(string _nameExchange) 
        {
            AddExchange(_nameExchange);
        }

        public NameSymbolModel(List<string> _listNameExchange)
        {
            NameExchangeList = _listNameExchange;
        }

        private List<string> NameExchangeList;
        
        public void AddExchange(string _nameExchange)
        {
            if (_nameExchange == null)
                throw new Exception("Нулевой входной аргумент _nameExchange.");
            if (NameExchangeList == null)
                NameExchangeList = new List<string>();
            if(!NameExchangeList.Contains(_nameExchange))
                NameExchangeList.Add(_nameExchange);
        }

        public void AddListExchange(List<string> _listNameExchange)
        {
            if (_listNameExchange == null)
                throw new Exception("Нулевой входной список _listNameExchange.");
            foreach (var _nameExchange in _listNameExchange)
                AddExchange(_nameExchange);
        }

        public List<string> GetExchange()
        {
            return NameExchangeList;
        }

        public void ClearExchange()
        {
            NameExchangeList.Clear();
        }


        private string _quoteAsset;
        public string QuoteAsset
        {
            get
            {
                return _quoteAsset;
            }
            set
            {
                if (_quoteAsset == value) return;
                _quoteAsset = value;
                NotifyPropertyChanged("QuoteAsset");
                NotifyPropertyChanged(nameof(DisplaySymbol));
            }
        }

        private string _baseAsset;
        public string BaseAsset
        {
            get
            {
                return _baseAsset;
            }
            set
            {
                if (_baseAsset == value) return;
                _baseAsset = value;
                NotifyPropertyChanged("BaseAsset");
                NotifyPropertyChanged(nameof(DisplaySymbol));
            }
        }

        public string DisplaySymbol => $"{BaseAsset}{QuoteAsset}";

        public void Copy(NameSymbolModel temp)
        {
            BaseAsset = temp.BaseAsset;
            QuoteAsset = temp.QuoteAsset;
            ClearExchange();
            AddListExchange(temp.GetExchange());
        }

        public override bool Equals(object obj)
        {
            if (obj is NameSymbolModel namesymbol) 
                return DisplaySymbol == namesymbol.DisplaySymbol;
            return false;
        }

        public override int GetHashCode() => DisplaySymbol.GetHashCode();


    }
}
