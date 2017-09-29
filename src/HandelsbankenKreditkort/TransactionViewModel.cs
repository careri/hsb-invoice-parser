using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandelsbankenKreditkort
{
    public class TransactionViewModel : INotifyPropertyChanged
    {
        private bool m_isShared;

        public TransactionViewModel(DateTime date, string shop, string city, double amount, double amountForeign, double rate, string currency, bool isShared)
        {
            Date = date;
            Shop = shop;
            City = city;
            Amount = amount;
            AmountForeign = amountForeign;
            RateForeign = rate;
            CurrencyForeign = currency;
            IsShared = isShared;
        }

        public DateTime Date { get; }
        public string Shop { get; }
        public string City { get; }
        public double Amount { get; }
        public double AmountForeign { get; }
        public string CurrencyForeign { get; }
        public double RateForeign { get; }

        public bool IsShared
        {
            get { return m_isShared; }
            set
            {
                if (m_isShared != value)
                {
                    m_isShared = value;
                    TriggerPropertyChanged("IsShared");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
