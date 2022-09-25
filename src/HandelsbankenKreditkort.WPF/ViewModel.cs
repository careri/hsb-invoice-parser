using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandelsbankenKreditkort
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string m_file;
        private readonly RelayCommand m_parse;
        private TransactionCollectionViewModel m_collection;

        public ViewModel()
        {
            m_collection = new TransactionCollectionViewModel();
            m_parse = new RelayCommand(o => true, Parse);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Parse(object o)
        {
            var path = FilePath;
            Unbind();
            if (string.IsNullOrEmpty(path))
            {
                m_collection = new TransactionCollectionViewModel();
            }
            else
            {
                m_collection = TransactionCollectionViewModel.Parse(path);
            }
            Bind();
            TriggerPropertyChanged("Transactions");
            TriggerPropertyChanged("Sum");
            TriggerPropertyChanged("SharedSum");

        }

        private void Unbind()
        {
            foreach (var item in m_collection.Items)
            {
                item.PropertyChanged -= OnItemChanged;
            }
        }

        private void Bind()
        {
            foreach (var item in m_collection.Items)
            {
                item.PropertyChanged += OnItemChanged;
            }
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsShared".Equals(e.PropertyName))
            {
                TriggerPropertyChanged("Sum");
                TriggerPropertyChanged("SharedSum");
            }
        }

        public ICommand ParseCmd => m_parse;

        public TransactionCollectionViewModel Transactions => m_collection;

        public double Sum => m_collection.Items.Sum(i => i.Amount);

        public double SharedSum => m_collection.Items.Where(i => i.IsShared).Sum(i => i.Amount); 



        /// <summary>
        /// The text from a pdf
        /// </summary>
        public string FilePath
        {
            get => m_file;
            set
            {
                m_file = value;
                TriggerPropertyChanged("FilePath");
            }
        }

        private void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
