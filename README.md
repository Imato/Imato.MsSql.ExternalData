# Imato.MsSql.ExternalData

How to put data from external cmd process to SQL server

### Using 

#### Programm for produse data

1. Create your own process to produce new data. 
Override CreateData in DataProcess
```csharp
internal class DaysProcess : DataProcess<MonthDay>
{
    public DaysProcess(string[] args) : base(args)
    {
    }

    protected override IEnumerable<MonthDay> CreateData(
        IDictionary<string, string> parameters)
    {
        ....
    }
}
```

2. Create table for DTO class MonthDay in DB
```sql
create table ##test
(
Id int,
Name varchar(30),
Date datetime,
IsDayOf bit
)
```

3. Run process
```csharp
public static async Task Main(string[] args)
{
    await new DaysProcess(args).RunAsync();
}
```

4. Start your executable for producing data with parameters
```
>Imato.MsSql.ExternalData.Example.exe Year=2022 Month=10
```

Add special parameter for destination table
```
>Imato.MsSql.ExternalData.Example.exe Year=2022 Month=10 Table=##test
```

Override connection string if it isn't local
```
>Imato.MsSql.ExternalData.Example.exe Year=2022 Month=10 Table=##test ConnectionString=yourConnection
```

5. Execute your cmd util in t-sql script  
Create procedure execmd.sql in msdb.  
Start 
```sql
exec dbo.execmd 'Imato.MsSql.ExternalData.Example.exe Year=2022 Month=10 Table=##test'
```
Use data from ##test in t-sql script.

#### Programm for execute same command without result

1. Create your own command executor 
Override StartCommand in Process

```csharp
internal class SameProcess : CommandProcess
{
    public SameProcess(string[] args) : base(args)
    {
    }

    protected override Task StartCommandAsync()
    {
        switch Command:
        .....
    }
}
```

2. Run process
```csharp
public static async Task Main(string[] args)
{
    await new SameProcess(args).RunAsync();
}
```

3. Start your executable for execute command
```
>Imato.MsSql.ExternalData.Example.exe Command=Test
```