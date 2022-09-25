using HandelsbankenKreditkort;
internal class Program
{
    private static void Main(string[] args)
    {
        var path = string.Join(" ", args);
        Console.WriteLine(path);
        ViewModel vm = new(new FileInfo(path));
        Console.WriteLine("Enter the row number to toggle is shared. -1 to quit");
        var done = false;

        while (!done)
        {
            Show(vm);
            Console.Write("Line: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var rowNo))
            {
                done = rowNo == -1;

                if (!done && rowNo > -1 && rowNo < vm.Transactions.Count)
                {
                    var item = vm.Transactions.GetItem(rowNo);
                    item.IsShared = !item.IsShared;
                }
            }
        }

        static void Show(ViewModel vm)
        {
            var widest = vm.Transactions.Items.Max(i => i.Shop.Length);
            foreach (var t in vm.Transactions.Items.Select((trans, idx) => (trans, idx)))
            {
                Console.WriteLine($"{t.idx:00#}. {t.trans.Shop.PadRight(widest)} = {t.trans.Amount}{(t.trans.IsShared ? "*" : string.Empty)}");
            }

            Console.WriteLine($"Sum: {vm.Sum:#####.00}");
            Console.WriteLine($"SharedSum: {vm.SharedSum::#####.00}");
        }
    }
}