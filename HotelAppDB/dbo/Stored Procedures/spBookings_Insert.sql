CREATE PROCEDURE [dbo].[spBookings_Insert]
	@roomId int,
	@guestId int,
	@starDate date,
	@endDate date,
	@totalCost money
AS
begin
	set nocount on;

	insert into dbo.Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
	values (@roomId, @guestId, @starDate, @endDate, @totalCost);
end