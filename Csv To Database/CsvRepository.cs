using Csv_Enumerable;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Csv_To_Database
{
    class CsvRepository : IRepository<CsvRecord>, IDisposable
    {
        static SqliteConnection connection;
        static CsvRepository instance;
        bool disposed = false;

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
            if (instance == null)
            {
                instance = new CsvRepository(connectionString);
            }
            return instance;
        }

        string CreateInsertStr(IEnumerable<string> values)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO Records (");
            var n = 0;
            foreach (var item in values)
            {
                sb.Append($"Field{n}");
                sb.Append(",");
                n++;
            }
            if (n > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(") VALUES (");
            n = 0;
            foreach (var item in values)
            {
                sb.Append($"'{item}'");
                sb.Append(",");
                n++;
            }
            if (n > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(");\r\n");
            return sb.ToString();
        }

        public void Add(CsvRecord entity)
        {
            var colCount = GetColumnsCount();
            var sqlExpression = $"pragma table_info(Records)";
            if (colCount < entity.Fields.Count)
            {
                AddColumns(colCount, entity.Fields.Count);
            }
            sqlExpression = CreateInsertStr(entity.Fields);
            using (var command = new SqliteCommand(sqlExpression, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        int GetColumnsCount()
        {
            var sqlExpression = $"pragma table_info('Records')";
            var colCount = 0;
            using (var command = new SqliteCommand(sqlExpression, connection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    colCount++;
                }
            }
            return colCount;
        }

        public void AddRange(IEnumerable<CsvRecord> range)
        {
            var colCount = GetColumnsCount();
            foreach (var item in range)
            {
                if (colCount < item.Fields.Count)
                {
                    AddColumns(colCount, item.Fields.Count);
                    colCount = item.Fields.Count;
                }
            }
            var sb = new StringBuilder();
            foreach (var item in range)
            {
                sb.Append(CreateInsertStr(item.Fields));
            }
            var sqlExpression = sb.ToString();
            using (var command = new SqliteCommand(sqlExpression, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        void AddColumns(int colCount, int fieldCount)
        {
            var sb = new StringBuilder();
            for (int i = colCount - 1; i < fieldCount; i++)
            {
                sb.Append($"ALTER TABLE Records ADD Field{i} TEXT;");
                sb.Append("\r\n");
            }
            var sqlExpression = sb.ToString();
            using (var command1 = new SqliteCommand(sqlExpression, connection))
            {
                command1.ExecuteNonQuery();
            }
        }

        public void Delete(CsvRecord entity)
        {
            var colCount = GetColumnsCount();
            var sb = new StringBuilder();
            sb.Append("DELETE FROM Records WHERE ");
            var i = 0;
            foreach (var item in entity.Fields)
            {
                sb.Append($"Field{i}='{item}' AND ");
                i++;
            }
            if (i > 0)
            {
                sb.Remove(sb.Length - 1 - 4, 4);
            }
            var sqlExpression = sb.ToString();
            using (var command1 = new SqliteCommand(sqlExpression, connection))
            {
                command1.ExecuteNonQuery();
            }
        }

        public void Edit(CsvRecord entity)
        {
            int colCount = GetColumnsCount();
            var sb = new StringBuilder();
            sb.Append($"UPDATE Records SET ");
            int i = 0;
            foreach (var item in entity.Fields)
            {
                sb.Append($"Field{i}='{item}',");
                i++;
            }
            if (i > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append($"WHERE Id={entity.Id}");
            var sqlExpression = sb.ToString();
            using (var command1 = new SqliteCommand(sqlExpression, connection))
            {
                command1.ExecuteNonQuery();
            }
        }

        public IEnumerable<CsvRecord> GetAll()
        {
            var sqlExpression = "SELECT * FROM Records ORDER BY Id";
            var list = new List<CsvRecord>();
            using (var command = new SqliteCommand(sqlExpression, connection))
            {
                var reader = command.ExecuteReader();
                var sb = new StringBuilder();
                var colCount = GetColumnsCount();
                while (reader.Read())
                {
                    var Id = (long)reader.GetValue(0);
                    int i = 1;
                    for (; i < colCount; i++)
                    {
                        sb.Append(reader.GetString(i));
                        sb.Append(",");
                    }
                    if (i > 1)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        list.Add(new CsvRecord(sb.ToString(), Id));
                        sb.Clear();
                    }
                }
            }
            return list;
        }

        public CsvRecord GetById(int id)
        {
            var sqlExpression = $"SELECT * FROM Records WHERE Id={id}";
            using (var command = new SqliteCommand(sqlExpression, connection))
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var sb = new StringBuilder();
                    var colCount = GetColumnsCount();

                    var Id = (long)reader.GetValue(0);
                    int i = 1;
                    for (; i < colCount; i++)
                    {
                        sb.Append(reader.GetString(i));
                        sb.Append(",");
                    }
                    if (i > 1)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        return new CsvRecord(sb.ToString(), Id);
                    }
                }
            }
            return null;
        }

        public IEnumerable<CsvRecord> GetList(Expression<Func<CsvRecord, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                instance = null;
            }
            // Free any unmanaged objects here.
            //
            connection?.Close();
            connection?.Dispose();
            disposed = true;
        }

        ~CsvRepository()
        {
            Dispose(false);
        }
    }
}
