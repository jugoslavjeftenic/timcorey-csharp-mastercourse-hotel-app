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

		public void BookGuest(string firstName, string lastName,
			DateTime startDate, DateTime endDate, int roomTypeId)
		{
			GuestModel guest = _db.LoadData<GuestModel, dynamic>("dbo.spGuests_Insert",
				new { firstName, lastName }, _connectionStringName, true).First();

			RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>("dbo.spRoomTypes_GetRoomTypeById",
				new { roomTypeId }, _connectionStringName, true).First();

			TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

			List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>("dbo.spRoomTypes_GetAvailableTypes",
				new { startDate, endDate, roomTypeId }, _connectionStringName, true);

			_db.SaveData("dbo.spBookings_Insert",
				new
				{
					roomId = availableRooms.First().Id,
					guestId = guest.Id,
					startDate,
					endDate,
					totalCost = timeStaying.Days * roomType.Price
				},
				_connectionStringName, true);
		}

		public List<BookingFullModel> SearchBookings(string lastName)
		{
			return _db.LoadData<BookingFullModel, dynamic>("dbo.spBookings_Search",
				new { lastName, startDate = DateTime.Now.Date }, _connectionStringName, true);
		}
	}
}
