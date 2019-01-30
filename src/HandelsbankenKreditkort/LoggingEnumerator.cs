using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandelsbankenKreditkort
{
    class LoggingEnumerator : IEnumerator<string>
    {
        private readonly IEnumerator<string> m_decorated;
        private readonly StreamWriter m_writer;

        public LoggingEnumerator(FileInfo fi, IEnumerator<string> decorated)
        {
            m_decorated = decorated;
            fi.Directory.Create();
            m_writer = new StreamWriter(fi.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read));
        }

        public string Current => m_decorated.Current;

        object IEnumerator.Current => m_decorated.Current;

        public void Dispose()
        {
            m_decorated.Dispose();
            m_writer.Dispose();
        }

        public bool MoveNext()
        {
            if (m_decorated.MoveNext())
            {
                m_writer.WriteLine(m_decorated.Current);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            m_decorated.Reset();
        }
    }
}
