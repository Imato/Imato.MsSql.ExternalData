using Imato.MsSql.ExternalData.Example;

public static class Programm
{
    public static async Task Main(string[] args)
    {
        await new DaysProcess(args).RunAsync();
    }
}