using CsvEnumerable;
using LoggingProxy;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvToDatabase
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var csvFile = configuration["FilePath"];
            var conStr = configuration["ConnectionString"];

            IEnumerable<CsvRecord> csvEnumerable = new CsvEnumerable.CsvEnumerable(csvFile);
            IAsyncRepository<CsvRecord> repository = CsvRepository.GetInstance(conStr);

            var proxy = new LoggingProxy<IAsyncRepository<CsvRecord>>();
            repository = proxy.CreateInstance(repository);

            await repository.AddRangeAsync(csvEnumerable);

            var records = await repository.GetAllAsync();
            WriteAll(records);

            Console.WriteLine("Delete first record");
            await repository.DeleteAsync(records.First());

            records = await repository.GetAllAsync();
            WriteAll(records);

            Console.WriteLine("Edit last record");
            var record = records.Last();
            record.Fields[0] = "2019";
            await repository.EditAsync(record);

            records = await repository.GetAllAsync();
            WriteAll(records);

            Console.WriteLine("Get record with Id=2");
            record = await repository.GetByIdAsync(2);
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
