using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvEnumerable
{
    public class CsvRecord : EntityBase
    {
        public List<string> Fields { get; }

        public CsvRecord(string strRecord, long id = 0)
        {
            Id = id;
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
            stringBuilder.Append($"Id={Id} ");
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
}
