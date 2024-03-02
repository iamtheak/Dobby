using System;
using System.Threading;
using System.Threading.Tasks;
using Dobby.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.DSharpPlus;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder(args);

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



builder.Services.AddLavalink();
builder.Services.ConfigureLavalink(config =>
{
    config.Label = "Lavalink";
    config.Passphrase = "satanwantsmore";
    config.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(15));
    config.ReadyTimeout = TimeSpan.FromSeconds(15);
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
    private readonly IAudioService _audioService;

    public ApplicationHost(DiscordClient discordClient, IAudioService audioService,CommandsNextConfiguration commands)
    {
        ArgumentNullException.ThrowIfNull(discordClient);
        ArgumentNullException.ThrowIfNull(audioService);

        _discordClient = discordClient;
        _audioService = audioService;
        
        var command = _discordClient.UseCommandsNext(commands);

        command.RegisterCommands<TextCommands>();
        command.RegisterCommands<VoiceCommands>();
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


        var playerOptions = new LavalinkPlayerOptions
        {
            InitialTrack = new TrackQueueItem(new TrackReference("https://youtu.be/5TAko3RH0bk?si=FAEruVa4ibgPklmh")),
        };

        //await _audioService.StartAsync();
        //await Task.Delay(3000);

        //await _audioService.Players
        //    .JoinAsync(1072078275305811988, 1072078275305811992, playerOptions, stoppingToken) // Ids
        //    .ConfigureAwait(false);

    }
}