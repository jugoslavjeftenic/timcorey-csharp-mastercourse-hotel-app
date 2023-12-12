using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace HotelAppLibrary.Database
{
	public class SqliteDataAccess : ISqliteDataAccess
	{
		private readonly IConfiguration _config;

		public SqliteDataAccess(IConfiguration config)
		{
			_config = config;
		}

		public List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName)
		{
			string connectionString = _config.GetConnectionString(connectionStringName)!;

			using IDbConnection connection = new SQLiteConnection(connectionString);
			List<T> rows = connection.Query<T>(sqlStatement, parameters).ToList();
			return rows;
		}

		public void SaveData<T>(string sqlStatement, T parameters, string connectionStringName)
		{
			string connectionString = _config.GetConnectionString(connectionStringName)!;

			using IDbConnection connection = new SQLiteConnection(connectionString);
			connection.Execute(sqlStatement, parameters);
		}
	}
}
