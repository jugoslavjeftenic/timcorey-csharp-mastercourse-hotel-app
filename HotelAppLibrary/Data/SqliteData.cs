using HotelAppLibrary.Database;
using HotelAppLibrary.Models;

namespace HotelAppLibrary.Data
{
	public class SqliteData : IDatabaseData
	{
		private readonly ISqlDataAccess _db;
		private const string _connectionStringName = "SQLiteDb";

		public SqliteData(ISqlDataAccess db)
		{
			_db = db;
		}

		public void BookGuest(string firstName, string lastName, DateTime startDate, DateTime endDate,
			int roomTypeId)
		{
			string sql = @"select 1 from Guests where FirstName = @firstName and LastName = @lastName";
			int results = _db.LoadData<dynamic, dynamic>(sql,
				new { firstName, lastName },
				_connectionStringName).Count();
			if (results > 0)
			{
				sql = @"insert into Guests (FirstName, LastName) values (@firstName, @lastName);";

				_db.SaveData(sql, new { firstName, lastName }, _connectionStringName);
			}

			sql = @"select [Id], [FirstName], [LastName]
					from Guests
					where FirstName = @firstName and LastName = @lastName
					limit 1;";
			GuestModel guest = _db.LoadData<GuestModel, dynamic>(sql,
				new { firstName, lastName },
				_connectionStringName).First();

			sql = @"select * from RoomTypes where Id = @Id";
			RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>(sql,
				new { roomTypeId },
				_connectionStringName).First();

			TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

			sql = @"select r.*
					from Rooms r
					inner join RoomTypes t on t.Id = r.RoomTypeId
					where r.RoomTypeId = @roomTypeId
					and r.Id not in (
						select b.RoomId
						from Bookings b
						where (@startDate < b.StartDate and @endDate > b.EndDate)
							or (b.StartDate <= @endDate and @endDate < b.EndDate)
							or (b.StartDate <= @startDate and @startDate < b.EndDate)
					)";
			List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>(sql,
				new { startDate, endDate, roomTypeId },
				_connectionStringName);

			sql = @"insert into Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
					values (@roomId, @guestId, @startDate, @endDate, @totalCost);";
			_db.SaveData(sql,
				new
				{
					roomId = availableRooms.First().Id,
					guestId = guest.Id,
					startDate,
					endDate,
					totalCost = timeStaying.Days * roomType.Price
				},
				_connectionStringName);
		}

		public void CheckInGuest(int bookingId)
		{
			throw new NotImplementedException();
		}

		public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
		{
			string sql = @"select t.Id, t.Title, t.Description, t.Price
						   from Rooms r
						   inner join RoomTypes t on t.Id = r.RoomTypeId
						   where r.Id not in (
							   select b.RoomId
							   from Bookings b
							   where (@startDate < b.StartDate and @endDate > b.EndDate)
								   or (b.StartDate <= @endDate and @endDate < b.EndDate)
								   or (b.StartDate <= @startDate and @startDate < b.EndDate)
						   )
						   group by t.Id, t.Title, t.Description, t.Price;";
			var output = _db.LoadData<RoomTypeModel, dynamic>(sql,
				new { startDate, endDate },
				_connectionStringName);

			output.ForEach(x => x.Price /= 100);

			return output;
		}

		public RoomTypeModel GetRoomTypeById(int roomTypeId)
		{
			string sql = @"select [Id], [Title], [Description], [Price]
						   from RoomTypes
						   where Id = @roomTypeId;";
			return _db.LoadData<RoomTypeModel, dynamic>(sql,
				new { roomTypeId },
				_connectionStringName).First();
		}

		public List<BookingFullModel> SearchBookings(string lastName)
		{
			throw new NotImplementedException();
		}
	}
}
