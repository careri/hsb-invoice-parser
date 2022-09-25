using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandelsbankenKreditkort
{
    internal static class WellKnownData
    {
        //2017-08-25 WWW.ALIEXPRESS.COM LONDON -18,86
        //2017-08-28 AliExpress San Mateo -7,25 USD 8,153103 -59,11
        public const string TransactionTableKey = "Köpdatum;;";
        public static readonly Regex Regex_TransactionItem = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2});;(?<shop>[^;]+);;(?<city>[^;]+).*;;(?<amount>[^;]+)$");
    }
}
