using Dobby.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;




// Host Application Builder
var builder = new HostApplicationBuilder(args);



// Configuration builder for appsettings and user secrets

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets(typeof(Program).Assembly, true)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot config = configuration.Build();

string token = config["DISCORD_TOKEN"].ToString() ?? "";
string prefix = config["PREFIX"].ToString() ?? "";


// DSharpPlus
builder.Services.AddHostedService<ApplicationHost>();
builder.Services.AddSingleton<DiscordClient>();
builder.Services.AddSingleton(new DiscordConfiguration { Token = token, Intents = DiscordIntents.All,TokenType = TokenType.Bot}); // Put token here


builder.Services.AddSingleton<TextCommands>();
builder.Services.AddSingleton<VoiceCommands>();
builder.Services.AddLavalink();
builder.Services.ConfigureLavalink(config =>
{
    config.Label = "Lavalink";
    // config.BaseAddress = new Uri("https://f1ca39dd-9874-4eb6-95b4-87f98b207461-00-21h6gpzbthny.picard.replit.dev") ;
    config.Passphrase = "satanwantsmore";
    config.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(15));
    config.ReadyTimeout = TimeSpan.FromSeconds(15);
});


builder.Services.AddSingleton<SlashCommandsConfiguration>(new SlashCommandsConfiguration()
{
    Services = builder.Services.BuildServiceProvider()
});
builder.Services.AddSingleton<CommandsNextConfiguration>(new CommandsNextConfiguration()
{
    StringPrefixes = new[] { prefix },
    EnableMentionPrefix = true,
    EnableDms = false,
    CaseSensitive = false,
    Services = builder.Services.BuildServiceProvider()
});

// Logging
builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Build().Run();

file sealed class ApplicationHost : BackgroundService
{
    private readonly DiscordClient _discordClient;
    public ApplicationHost(DiscordClient discordClient, CommandsNextConfiguration commands,SlashCommandsConfiguration slashCommands)
    {
        ArgumentNullException.ThrowIfNull(discordClient);

        _discordClient = discordClient;
       
        
        var command = _discordClient.UseCommandsNext(commands);
        
        var slashCommand = _discordClient.UseSlashCommands(slashCommands);
        slashCommand.RegisterCommands<SlashCommandController>();
        command.RegisterCommands<TextCommandController>();
        command.RegisterCommands<VoiceCommandsController>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // connect to discord gateway and initialize node connection
        await _discordClient
            .ConnectAsync()
            .ConfigureAwait(false);

        var readyTaskCompletionSource = new TaskCompletionSource();

        Task SetResult(DiscordClient client, ReadyEventArgs eventArgs)
        {
            readyTaskCompletionSource.TrySetResult();
            return Task.CompletedTask;
        }

        _discordClient.Ready += SetResult;
        await readyTaskCompletionSource.Task.ConfigureAwait(false);
        _discordClient.Ready -= SetResult;
        
    }
}