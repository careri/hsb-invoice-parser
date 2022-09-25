using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandelsbankenKreditkort
{
    class TransactionBlock : IEnumerable<string>
    {
        private readonly IEnumerable<string> m_lines;

        public TransactionBlock(List<string> lines)
        {
            m_lines = lines.ToArray();
        }


        public IEnumerator<string> GetEnumerator()
        {
            return m_lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_lines.GetEnumerator();
        }

        internal static bool TryRead(IEnumerator<string> reader, out TransactionBlock block)
        {
            if (FindStart(reader))
            {
                block = ReadBlock(reader);
            }
            else
            {
                block = null;
            }
            return block != null;
        }

        private static TransactionBlock ReadBlock(IEnumerator<string> reader)
        {
            var lines = new List<string>();

            while (reader.MoveNext())
            {
                var line = reader.Current;
                if (WellKnownData.Regex_TransactionItem.IsMatch(line))
                {
                    lines.Add(line);
                }
            }
            if (lines.Count > 0)
            {
                return new TransactionBlock(lines);
            }
            return null;
        }

        private static bool FindStart(IEnumerator<string> reader)
        {
            while (reader.MoveNext())
            {
                var line = reader.Current;

                if (line.StartsWith(WellKnownData.TransactionTableKey))
                {
                    // The next row should be the data rows
                    return true;
                }
            }
            return false;
        }
    }
}
