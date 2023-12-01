﻿CREATE PROCEDURE [dbo].[spRoomTypes_GetRoomTypeById]
	@roomTypeId int
AS
begin
	set nocount on;

	select [Id], [Title], [Description], [Price]
	from dbo.RoomTypes
	where Id = @roomTypeId;
end