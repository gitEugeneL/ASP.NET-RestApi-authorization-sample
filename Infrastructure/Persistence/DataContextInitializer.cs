namespace Infrastructure.Persistence;

public static class DataContextInitializer
{
    public static void Init(DataContext context)
    {
        context.Database.EnsureCreated();
    }
}