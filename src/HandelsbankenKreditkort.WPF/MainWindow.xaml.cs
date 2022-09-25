using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HandelsbankenKreditkort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CheckCommandLine();
            
            FocusManager.SetFocusedElement(this, GridTrans);
        }

        private void CheckCommandLine()
        {
            var path = new Uri(GetType().Assembly.Location).LocalPath;
            var cmd = Environment.CommandLine;

            if (!string.IsNullOrEmpty(cmd))
            {
                cmd = cmd.Trim(' ', '"');
                var args = cmd.Substring(path.Length).Trim(' ', '"');

                if (cmd.StartsWith(path, StringComparison.CurrentCultureIgnoreCase) && cmd.Length > path.Length)
                {
                    try
                    {
                        var fi = new FileInfo(args);
                        var vm = ((ViewModel)DataContext);
                        vm.FilePath = fi.FullName;
                        vm.ParseCmd.Execute(null);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), args, MessageBoxButton.OK, MessageBoxImage.Error);
                    } 
                }
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If space we toggle is checked
            var dg = (DataGrid)sender;
            var idx = dg.SelectedIndex;

            if (idx > -1)
            {
                var row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(idx);

                if (!row.IsEditing)
                {
                    // Mark the row as checked
                    if (e.Key == Key.Space)
                    {
                        var vm = (TransactionViewModel)row.DataContext;
                        vm.IsShared = !vm.IsShared;
                        e.Handled = true;
                    }
                }
            }
        }

        private void GridTrans_Loaded(object sender, RoutedEventArgs e)
        {
            var dg = (DataGrid)sender;
            dg.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
