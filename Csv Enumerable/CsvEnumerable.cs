using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Csv_Enumerable
{
    class CsvEnumerable : IEnumerable, IEnumerator
    {
        class CsvRecord
        {
            public List<string> Fields { get; private set; }

            public CsvRecord(string strRecord)
            {
                var regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                var mc = regex.Matches(strRecord);
                Fields = new List<string>();
                foreach (var item in mc)
                {
                    Fields.Add(item.ToString());
                }
            }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                foreach (var item in Fields)
                {
                    stringBuilder.Append(item);
                    stringBuilder.Append(";");
                }
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
                return stringBuilder.ToString();
            }
        }

        private bool disposed = false;
        int position = -1;
        List<CsvRecord> records;

        public CsvEnumerable(string fileName)
        {
            var fileStream = File.Open(fileName, FileMode.Open);
            using (var streamReader = new StreamReader(fileStream))
            {
                records = new List<CsvRecord>();
                while (!streamReader.EndOfStream)
                {
                    var strRecord = streamReader.ReadLine();
                    var csvRecord = new CsvRecord(strRecord);
                    records.Add(csvRecord);
                }
            }
        }

        public object Current
        {
            get
            {
                return records[position];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (position < records.Count - 1)
            {
                position++;
                return true;
            }
            Reset();
            return false;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
