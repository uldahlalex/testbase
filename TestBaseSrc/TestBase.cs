using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace TestBaseSrc
{
    public abstract class TestBase<TContext, TService, TStartup>
        where TContext : DbContext
        where TService : class
        where TStartup : class
    {
        private readonly PostgreSqlContainer _postgres;
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;
        private readonly Action<IServiceCollection> _configureServices;
        protected WebApplicationFactory<TStartup> Factory { get; }
        protected HttpClient Client { get; private set; }

        protected TestBase(
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

        protected TestBase(
            WebApplicationFactory<TStartup> factory,
            string postgresImage = "postgres:16-alpine",
            Action<DbContextOptionsBuilder> configureDbContext = null,
            Action<IServiceCollection> configureServices = null)
            : this(postgresImage, configureDbContext, configureServices)
        {
            Factory = factory;
        }

        private Action<DbContextOptionsBuilder> DefaultDbContextConfiguration =>
            optionsBuilder =>
            {
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                optionsBuilder.UseNpgsql(_postgres.GetConnectionString());
            };

        public TContext DbContextInstance { get; private set; }
        public TService ServiceInstance { get; private set; }
        protected IServiceProvider ServiceProviderInstance { get; private set; }

        public async Task Setup()
        {
            await _postgres.StartAsync();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            _configureDbContext(optionsBuilder);

            DbContextInstance = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options);
            await DbContextInstance.Database.EnsureCreatedAsync();

            var services = new ServiceCollection();
            services.AddSingleton(DbContextInstance);

            services.AddScoped<TService>();

            _configureServices?.Invoke(services);

            if (Factory != null)
            {
                Client = Factory.CreateClient();
                services.AddSingleton(Client);
            }

            var serviceProvider = services.BuildServiceProvider();

            ServiceInstance = serviceProvider.GetRequiredService<TService>();
            ServiceProviderInstance = serviceProvider;
        }

        public async Task Teardown()
        {
            await DbContextInstance.DisposeAsync();
            await _postgres.DisposeAsync();
            Client?.Dispose();
            Factory?.Dispose();
        }

        public virtual async Task InitializeAsync()
        {
            await Setup();
        }

        public virtual async Task DisposeAsync()
        {
            await Teardown();
        }
    }
}