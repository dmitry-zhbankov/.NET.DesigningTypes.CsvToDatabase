using CsvEnumerable;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvToDatabase
{
    class CsvRepository : IAsyncRepository<CsvRecord>
    {
        static SqliteConnection connection;
        static CsvRepository instance;
        bool disposed;

        private CsvRepository(string connectionString)
        {
            connection = new SqliteConnection(connectionString);
            var sqlExpression =
                @"CREATE TABLE IF NOT EXISTS Records(
                Id INTEGER PRIMARY KEY,                
                Field0 TEXT
                );";
            connection?.Open();

            var command = new SqliteCommand(sqlExpression, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }
        public static CsvRepository GetInstance(string connectionString)
        {
            return instance ??= new CsvRepository(connectionString);
        }

        string CreateInsertSqlStr(CsvRecord entity)
        {
            var sb = new StringBuilder("INSERT INTO Records (");
            var items = entity.Fields.ToArray();
            sb.AppendJoin(" , ", items.Select((item, index) => $"Field{index}"));
            sb.Append(") VALUES (");
            sb.AppendJoin(" , ", items.Select((item, index) => $"'{item}'"));
            sb.Append(" );\r\n");
            return sb.ToString();
        }

        public async Task AddAsync(CsvRecord entity)
        {
            var colCount = await GetColumnsCount();
            if (colCount < entity.Fields.Count)
            {
                await AddColumns(colCount, entity.Fields.Count);
            }

            var sqlExpression = CreateInsertSqlStr(entity);
            await using (var command = new SqliteCommand(sqlExpression, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        async Task<int> GetColumnsCount()
        {
            var sqlExpression = "pragma table_info('Records')";
            var colCount = 0;
            await using (var command = new SqliteCommand(sqlExpression, connection))
            await using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    colCount++;
                }
            }
            return colCount;
        }

        public async Task AddRangeAsync(IEnumerable<CsvRecord> range)
        {
            var colCount = await GetColumnsCount();
            var csvRecords = range as CsvRecord[] ?? range.ToArray();
            foreach (var item in csvRecords)
            {
                if (colCount >= item.Fields.Count) continue;
                await AddColumns(colCount, item.Fields.Count);
                colCount = item.Fields.Count;
            }

            var sb = new StringBuilder();
            sb.AppendJoin("", csvRecords.Select((item, index) => CreateInsertSqlStr(item)));
            var sqlExpression = sb.ToString();
            await using (var command = new SqliteCommand(sqlExpression, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        async Task AddColumns(int colCount, int fieldCount)
        {
            var sb = new StringBuilder();
            for (var i = colCount - 1; i < fieldCount; i++)
            {
                sb.Append($"ALTER TABLE Records ADD Field{i} TEXT;");
                sb.Append("\r\n");
            }

            var sqlExpression = sb.ToString();
            await using (var command1 = new SqliteCommand(sqlExpression, connection))
            {
                await command1.ExecuteNonQueryAsync();
            }
        }

        string CreateDeleteSqlStr(CsvRecord entity)
        {
            var sb = new StringBuilder("DELETE FROM Records WHERE ");
            var items = entity.Fields.ToArray();
            sb.AppendJoin(" AND ", items.Select((item, index) => $"Field{index}='{item}'"));
            return sb.ToString();
        }

        public async Task DeleteAsync(CsvRecord entity)
        {
            var sqlExpression = CreateDeleteSqlStr(entity);
            await using (var command1 = new SqliteCommand(sqlExpression, connection))
            {
                await command1.ExecuteNonQueryAsync();
            }
        }

        string CreateUpdateSqlStr(CsvRecord entity)
        {
            var sb = new StringBuilder("UPDATE Records SET ");
            var items = entity.Fields.ToArray();
            sb.AppendJoin(" , ", items.Select((item, index) => $"Field{index}='{item}'"));
            sb.Append($"WHERE Id={entity.Id}");
            return sb.ToString();
        }

        public async Task EditAsync(CsvRecord entity)
        {
            var sqlExpression = CreateUpdateSqlStr(entity);
            await using (var command = new SqliteCommand(sqlExpression, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<CsvRecord>> GetAllAsync()
        {
            var sqlExpression = "SELECT * FROM Records ORDER BY Id";
            var list = new List<CsvRecord>();
            await using (var command = new SqliteCommand(sqlExpression, connection))
            await using (var reader = command.ExecuteReader())
            {
                var sb = new StringBuilder();
                var colCount = reader.FieldCount;
                while (await reader.ReadAsync())
                {
                    var id = (long)reader.GetValue(0);
                    var results = new object[colCount];
                    reader.GetValues(results);
                    var fields = results.Skip(1);
                    sb.AppendJoin(',', fields);
                    list.Add(new CsvRecord(sb.ToString(), id));
                    sb.Clear();
                }
            }
            return list;
        }

        public async Task<CsvRecord> GetByIdAsync(int id)
        {
            var sqlExpression = $"SELECT * FROM Records WHERE Id={id}";
            await using (var command = new SqliteCommand(sqlExpression, connection))
            await using (var reader = command.ExecuteReader())
            {
                if (!await reader.ReadAsync()) return null;
                var sb = new StringBuilder();
                var colCount = reader.FieldCount;
                var results = new object[colCount];
                reader.GetValues(results);
                var fields = results.Skip(1);
                sb.AppendJoin(',', fields);
                return new CsvRecord(sb.ToString(), id);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                connection?.Dispose();
            }
            disposed = true;
        }

        ~CsvRepository()
        {
            Dispose(false);
        }
    }
}
