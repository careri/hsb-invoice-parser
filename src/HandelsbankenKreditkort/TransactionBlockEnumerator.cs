using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System.IO;

namespace HandelsbankenKreditkort
{
    /// <summary>
    /// Parses the output of the PDF reader and tries to locate a block of transaction lines
    /// </summary>
    class TransactionBlockEnumerator : IEnumerator<TransactionBlock>
    {
        private readonly PdfDocument m_doc;
        private readonly Lazy<TransactionBlock[]> m_blocks;
        private TransactionBlock m_current;
        private int m_index = -1;


        public TransactionBlockEnumerator(PdfDocument doc)
        {
            this.m_doc = doc;
            m_blocks = new Lazy<TransactionBlock[]>(InitBlocks, true);
        }

        private TransactionBlock[] InitBlocks()
        {
            var pageCount = m_doc.GetNumberOfPages();
            var list = new List<TransactionBlock>();
            Console.WriteLine("#Pages: {0}", pageCount);
            var sb = new StringBuilder();

            for (int i = 1; i <= pageCount; i++)
            {
                var page = m_doc.GetPage(i);
                //var strategy = new SimpleTextExtractionStrategy();
                var strategy = new LocationTextExtractionStrategy2();
                sb.AppendLine(PdfTextExtractor.GetTextFromPage(page, strategy));                
            }
            var fi = new FileInfo(@"logs\kreditkort.txt");

            using (var reader = new LoggingEnumerator(fi, new LineEnumerator(sb.ToString())))
            {
                TransactionBlock block;

                while (TransactionBlock.TryRead(reader, out block))
                {
                    list.Add(block);
                }
            }

            return list.ToArray();
        }

        public TransactionBlock Current => m_current;

        object IEnumerator.Current => m_current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (m_index < m_blocks.Value.Length -1)
            {
                m_current = m_blocks.Value[++m_index];
                return true;
            }
            return false;
        }

        public void Reset()
        {
            m_index = -1;
            m_current = null;
        }
    }
}
