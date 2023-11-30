using HotelAppLibrary.Database;
using HotelAppLibrary.Models;

namespace HotelAppLibrary.Data
{
	public class SqlData
	{
		private readonly ISqlDataAccess _db;
		private const string _connectionStringName = "SqlDb";

		public SqlData(ISqlDataAccess db)
		{
			_db = db;
		}

		public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
		{
			return _db.LoadData<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetAvailableTypes",
				new { startDate, endDate }, _connectionStringName, true);
		}
	}
}
