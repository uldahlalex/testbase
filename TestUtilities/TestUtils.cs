using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace TestUtilities;

public class TestUtils<TContext> where TContext : DbContext
{
    public TContext DbContextInstance { get; private set; }
    public IServiceProvider ServiceProviderInstance { get; private set; }

    private readonly PostgreSqlContainer _postgres;
    private readonly Action<DbContextOptionsBuilder> _configureDbContext;
    private readonly Action<IServiceCollection> _configureServices;

    public TestUtils(
        string postgresImage = "postgres:16-alpine",
        Action<DbContextOptionsBuilder> configureDbContext = null,
        Action<IServiceCollection> configureServices = null)
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage(postgresImage)
            .Build();

        _configureDbContext = configureDbContext ?? DefaultDbContextConfiguration;
        _configureServices = configureServices;
    }


    private Action<DbContextOptionsBuilder> DefaultDbContextConfiguration =>
        optionsBuilder =>
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseNpgsql(_postgres.GetConnectionString());
        };

    public async Task Setup()
    {
        await _postgres.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        _configureDbContext?.Invoke(optionsBuilder);

        DbContextInstance = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
        await DbContextInstance.Database.EnsureCreatedAsync();

        var services = new ServiceCollection();
        services.AddSingleton(DbContextInstance);

        _configureServices?.Invoke(services);

        ServiceProviderInstance = services.BuildServiceProvider();
    }
    
    public async Task TearDown()
    {
        ServiceProviderInstance = null;

        await DbContextInstance.Database.EnsureDeletedAsync();
        DbContextInstance.Dispose();
        DbContextInstance = null;

        await _postgres.StopAsync();
    }
    
    
}

public static class AsyncHelper
{
    private static readonly TaskFactory MyTaskFactory = new
        TaskFactory(CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return MyTaskFactory
            .StartNew(func)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
    }

    public static void RunSync(Func<Task> func)
    {
        MyTaskFactory
            .StartNew(func)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
    }
}