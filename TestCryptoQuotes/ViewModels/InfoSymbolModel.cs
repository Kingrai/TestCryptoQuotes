using System;
using System.ComponentModel;

namespace TestCryptoQuotes.ViewModels
{
    public class InfoSymbolModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private decimal _lastPrice;
        public decimal LastPrice
        {
            get
            {
                return _lastPrice;
            }
            set
            {
                if (_lastPrice == value) return;
                _lastPrice = value;
                NotifyPropertyChanged("LastPrice");
            }
        }

        private string _exchange;
        public string Exchange
        {
            get
            {
                return _exchange;
            }
            set
            {
                if (_exchange == value) return;
                _exchange = value;
                NotifyPropertyChanged("Exchange");
            }
        }

        private DateTime _updateDT;
        public DateTime UpdateDT
        {
            get
            {
                return _updateDT;
            }
            set
            {
                if (_updateDT == value) return;
                _updateDT = value;
                NotifyPropertyChanged("UpdateDT");
            }
        }

        private DateTime _lastPingDT;
        public DateTime LastPingDT
        {
            get
            {
                return _lastPingDT;
            }
            set
            {
                if (_lastPingDT == value) return;
                _lastPingDT = value;
                NotifyPropertyChanged("LastPingDT");
            }
        }


        private bool _isTraiding;
        public bool IsTraiding
        {
            get
            {
                return _isTraiding;
            }
            set
            {
                if (_isTraiding == value) return;
                _isTraiding = value;
                NotifyPropertyChanged("IsTraiding");
            }
        }

        private bool _isConnect;
        public bool IsConnect
        {
            get
            {
                return _isConnect;
            }
            set
            {
                if (_isConnect == value) return;
                _isConnect = value;
                NotifyPropertyChanged("IsConnect");
            }
        }

        private string _precisionFormat;
        public string PrecisionFormat
        {
            get
            {
                return _precisionFormat;
            }
            private set
            {
                if (_precisionFormat == value) return;
                _precisionFormat = value;
                NotifyPropertyChanged("PrecisionFormat");
            }
        }

        private string _info;
        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                if (_info == value) return;
                _info = value;
                NotifyPropertyChanged("Info");
            }
        }

        private decimal _tickSize;
        public decimal TickSize
        {
            get
            {
                return _tickSize;
            }
            set
            {
                if (_tickSize == value) return;
                _tickSize = value;
                _precisionFormat = GetPrecisionFormat();
                NotifyPropertyChanged("PrecisionFormat");
                NotifyPropertyChanged("TickSize");
            }
        }

        private int CalculateBasePrecisionForSymbol()
        {
            string tickSizeString = TickSize.ToString().TrimEnd('0').Replace(",", ".");
            int decimalPointIndex = tickSizeString.IndexOf('.');
            if (decimalPointIndex == -1)
                return 0;
            return tickSizeString.Length - decimalPointIndex - 1;
        }

        public string GetPrecisionFormat()
        {
            int numberOfDigitsAfterPoint = CalculateBasePrecisionForSymbol();
            if (numberOfDigitsAfterPoint > 0)
                return "0.".PadRight(2 + numberOfDigitsAfterPoint, '0');
            else
                return "0";
        }

        public void Copy(InfoSymbolModel infoSymbol)
        {
            LastPrice = infoSymbol.LastPrice;
            Exchange = infoSymbol.Exchange;
            UpdateDT = infoSymbol.UpdateDT;
        }

    }
}
