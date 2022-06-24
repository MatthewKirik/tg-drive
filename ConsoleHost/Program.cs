// See https://aka.ms/new-console-template for more information

using ConsoleHost.Exceptions;
using DriveServices;
using DriveServices.Implementations;
using EfRepositories;
using EfRepositories.Repositories;
using LiteDB;
using MappingConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Telegram.Bot;
using TgChatsStorage;
using TgFrontend;
using TgFrontend.Abstractions;
using TgFrontend.Menus;
using TgGateway.Abstractions;
using TgGateway.Implementations;

var mySqlConnectionStr =
    Environment.GetEnvironmentVariable("TGDRIVE_MYSQL_CONNECTION_STRING");
if (mySqlConnectionStr == null)
{
    throw new EnvironmentException("connection string for MySql",
        "TGDRIVE_MYSQL_CONNECTION_STRING");
}

var liteDbConnectionStr =
    Environment.GetEnvironmentVariable("TGDRIVE_LITEDB_CONNECTION_STRING");
if (mySqlConnectionStr == null)
{
    throw new EnvironmentException("connection string for LiteDb",
        "TGDRIVE_LITEDB_CONNECTION_STRING");
}

var tgBotToken =
    Environment.GetEnvironmentVariable("TGDRIVE_BOT_TOKEN");
if (tgBotToken == null)
{
    throw new EnvironmentException("telegram bot token",
        "TGDRIVE_BOT_TOKEN");
}

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services
            .AddDbContextPool<TgDriveContext>(options => options.UseMySql(
                mySqlConnectionStr,
                ServerVersion.AutoDetect(mySqlConnectionStr),
                options => options.MigrationsAssembly("MySqlMigrations")
            ));
        services
            .AddSingleton(AutoMapperConfigurator.GetMapper());
        services
            .AddScoped<IDirectoryRepository, DirectoryRepository>()
            .AddScoped<IFileRepository, FileRepository>()
            .AddScoped<IUserRepository, UserRepository>();
        services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IFileService, FileService>()
            .AddScoped<ITgFileService, TgFileService>()
            .AddScoped<IDirectoryService, DirectoryService>();

        services
            .AddScoped<ILiteDatabase>(_ => new LiteDatabase(liteDbConnectionStr))
            .AddScoped<IMessageStorage, LiteDbMessageStorage>();
        var bot = new TelegramBotClient(tgBotToken);
        services
            .AddSingleton<ITelegramBotClient>(bot)
            .AddSingleton<IBotClient, TgBotClient>();

        services.AddSingleton<IRedirectHandler, RedirectHandler>();
        services
            .AddSingleton<RootMenu>()
            .AddSingleton<DirectoryMenu>()
            .AddSingleton<FileMenu>();
        services.AddSingleton<BotFrontend>();
    })
    .Build();

using (var db = host.Services.GetRequiredService<TgDriveContext>())
{
    db.Database.Migrate();
}

var front = host.Services.GetRequiredService<BotFrontend>();
front.Start();
Thread.Sleep(Timeout.Infinite);
