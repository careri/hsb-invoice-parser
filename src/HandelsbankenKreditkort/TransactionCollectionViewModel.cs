using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace HandelsbankenKreditkort
{
    public class TransactionCollectionViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TransactionViewModel> m_items;

        public event PropertyChangedEventHandler PropertyChanged;

        public TransactionCollectionViewModel()
        {
            m_items = new ObservableCollection<TransactionViewModel>();
        }

        public string Error { get; private set; }

        internal static TransactionCollectionViewModel Parse(string path)
        {
            TransactionCollectionViewModel vm = new TransactionCollectionViewModel();
            try
            {
                var fi = new FileInfo(path);

                using (var reader = new PdfReader(fi))
                using (var doc = new PdfDocument(reader))
                using (var blockE = new TransactionBlockEnumerator(doc))
                {
                    while (blockE.MoveNext())
                    {
                        var block = blockE.Current;

                        foreach (var item in block)
                        {
                            // Get the parts
                            var match = WellKnownData.Regex_TransactionItem.Match(item);
                            int year = int.Parse(match.Groups["year"].Value);
                            int month = int.Parse(match.Groups["month"].Value);
                            int day = int.Parse(match.Groups["day"].Value);
                            var date = new DateTime(year, month, day);
                            var amount = double.Parse(match.Groups["amount"].Value);
                            var shop = match.Groups["shop"].Value;
                            var city= match.Groups["city"].Value;

                            vm.Add(new TransactionViewModel(date, shop, city, amount, 0, 0, null, false));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                vm.Error = ex.ToString();
            }
            return vm;
        }

        public int Count => m_items.Count; 

        private void Add(TransactionViewModel transactionViewModel)
        {
            m_items.Add(transactionViewModel);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        public IEnumerable<TransactionViewModel> Items => m_items; 
    }
}
