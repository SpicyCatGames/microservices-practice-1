using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroRabbit.Infra.IoC;
public class DependencyContainer
{
    public static void RegisterServices(IHostApplicationBuilder builder)
    {
        // Domain Bus
        builder.Services.AddTransient<IEventBus, RabbitMQBus>();

        // Application Services
        builder.Services.AddTransient<IAccountService, AccountService>();

        // Data
        builder.Services.AddTransient<IAccountRepository, AccountRepository>();
        //services.AddTransient<BankingDbContext>();
        builder.Services.AddDbContext<BankingDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
        });
    }
}
