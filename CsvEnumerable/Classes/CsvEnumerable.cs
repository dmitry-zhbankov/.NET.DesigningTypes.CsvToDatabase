using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CsvEnumerable
{
    public class CsvEnumerable : IEnumerable<CsvRecord>, IEnumerator<CsvRecord>
    {
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

        public object Current => records[position];
        
        CsvRecord IEnumerator<CsvRecord>.Current => records[position];

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
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

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator()
        {
            return this;
        }
    }
}
