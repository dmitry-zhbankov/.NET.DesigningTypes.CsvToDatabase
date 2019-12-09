using Csv_Enumerable;
using Logging_Proxy;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Csv_To_Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var csvFile = configuration["FilePath"];
            var conStr = configuration["ConnectionString"];

            IEnumerable<CsvRecord> csvEnumerable = new CsvEnumerable(csvFile);
            IRepository<CsvRecord> repository = CsvRepository.GetInstance(conStr);

            var proxy = new LoggingProxy<IRepository<CsvRecord>>();
            repository = proxy.CreateInstance(repository);

            repository.AddRange(csvEnumerable);

            var records = repository.GetAll();
            WriteAll(records);

            Console.WriteLine("Delete first record");
            repository.Delete(records.First());

            records = repository.GetAll();
            WriteAll(records);

            Console.WriteLine("Edit last record");
            CsvRecord record = records.Last();
            record.Fields[0] = "2019";
            repository.Edit(record);

            records = repository.GetAll();
            WriteAll(records);

            Console.WriteLine("Get record with Id=2");
            record = repository.GetById(2);
            Console.WriteLine(record);

            Console.ReadKey();
        }

        private static void WriteAll(IEnumerable<CsvRecord> records)
        {
            Console.WriteLine("All records in db:");
            foreach (var item in records)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }
    }
}
