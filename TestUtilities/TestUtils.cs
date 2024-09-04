using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace TestUtilities
{
    public class TestUtils<TContext> where TContext : DbContext
    {
        public TContext DbContextInstance { get; private set; }
        public IServiceProvider ServiceProviderInstance { get; }

        private readonly PostgreSqlContainer _postgres;

        public TestUtils(
            string postgresImage = "postgres:16-alpine",
            Action<DbContextOptionsBuilder> configureDbContext = null,
            Action<IServiceCollection> configureServices = null)
        {
            _postgres = new PostgreSqlBuilder()
                .WithImage(postgresImage)
                .Build();

            var configureDbContext1 = configureDbContext;
            configureDbContext1 ??= optionsBuilder =>
            {
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                optionsBuilder.UseNpgsql(_postgres.GetConnectionString());
            };

            AsyncHelper.RunSync(() => _postgres.StartAsync());

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            configureDbContext1.Invoke(optionsBuilder);

            DbContextInstance = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);

            AsyncHelper.RunSync(() => DbContextInstance.Database.EnsureCreatedAsync());

            var services = new ServiceCollection();
            services.AddSingleton(DbContextInstance ?? throw new InvalidOperationException("DbContextInstance is null"));

            configureServices?.Invoke(services);

            ServiceProviderInstance = services.BuildServiceProvider();
        }

        public async Task TearDown()
        {
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
}