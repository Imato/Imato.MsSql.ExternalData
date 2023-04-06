
use msdb
go

create proc [dbo].[execmd] 
	@cmd varchar(4000),
	@ignore_error bit = 0
as 
begin
	set nocount on;

	declare @out table (s varchar(max));
	declare @result bit,
					@error varchar(max) = '';

	insert into @out
	exec @result = master.dbo.xp_cmdshell @cmd;

	if (@result != 0 and @ignore_error = 0)
	begin 
	select @error += s + '
'
		from @out
		where s is not null

		set @error = 'Error executing ''' + @cmd + ''': ' + @error;
		throw 60060, @error, 1;
	end
end
go