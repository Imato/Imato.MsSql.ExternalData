
use msdb
go

create proc dbo.execmd
@cmd varchar(4000)
as
begin
	set nocount on;

	declare @result table 
		(str varchar(max));

	insert into @result
	exec xp_cmdshell @command_string = @cmd;

	if exists (select top 1 1 from @result where str is not null)
	begin
		declare @error varchar(max) = '';

		select @error += isnull(str, '
') + '
'
			from @result;

		throw 67000, @error, 1;
	end
end;
go