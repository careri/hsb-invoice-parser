﻿using System.Collections;

namespace HandelsbankenKreditkort
{
    public class LineEnumerator : IEnumerator<string>
    {
        private readonly string m_str;
        private StringReader? m_reader;
        private string? m_line;
        

        public LineEnumerator(string text)
        {
            m_str = text ?? string.Empty;
            Reset();
        }

        public string Current => m_line ?? throw new InvalidOperationException("You must call MoveNext first");

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            m_reader?.Dispose();
        }

        public bool MoveNext()
        {
            m_line = m_reader!.ReadLine();
            return m_line != null;
        }

        public void Reset()
        {
            if (m_reader != null)
            {
                m_reader.Dispose();
            }

            m_reader = new StringReader(m_str);
        }
    }
}
