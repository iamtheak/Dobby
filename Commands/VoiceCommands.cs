using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Options;

namespace Dobby.Commands;

public class VoiceCommands
{
    public IAudioService _audioService { private get; set; }
    public DiscordClient _discordClient { private get; set; }

    public VoiceCommands(IAudioService audioService, DiscordClient client)
    {
        _audioService = audioService;
        _discordClient = client;
        DiscordActivity activity = new DiscordActivity("Satan Xo Maria <3", ActivityType.Playing);
        _discordClient.UpdateStatusAsync(activity, UserStatus.Online);
        
        _discordClient.ConnectAsync().ConfigureAwait(false);

        _audioService.StartAsync().ConfigureAwait(false);
    }


    private async Task<LavalinkTrack> SearchTrackFromYoutube(string query)
    {
        var track = await _audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube)
            .ConfigureAwait(false);

        return track;
    }

    private async Task<DiscordEmbed> PLayMusic(LavalinkTrack track, QueuedLavalinkPlayer player)
    {
        await player.PlayAsync(new TrackReference(track));
        var queue = player.Queue;
        
        var embed = new DiscordEmbedBuilder()
        {
            Description = $"🔈 Playing: {track.Title} By: {track.Author} on queue {(queue.Count) + 1}",
            Color = DiscordColor.SapGreen
        };

        return embed;
    }

    public async Task<DiscordEmbed> Play(DiscordMember member, string query)
    {

        var embed = new DiscordEmbedBuilder();

        if (member.VoiceState?.Channel == null)
        {
            embed.Description = "You must be in a voice channel to use this command.";
            embed.Color = DiscordColor.Red;
            return embed;
        }

        var response = await GetPlayerAsync(member.Guild.Id, member.VoiceState.Channel.Id,connectToVoiceChannel: true).ConfigureAwait(false);

        if (response.Item1 is null)
        {
            embed.Description = response.Item2;
            embed.Color = DiscordColor.Red;
            
            return embed;
        }

        var player = response.Item1;

        var track = await SearchTrackFromYoutube(query: query) ?? null;


        if (track != null)
        {
           var musicResp =  await PLayMusic(track: track, player: player);
           return musicResp;

        }
        
        embed.Description = "😖 No results.";
        embed.Color = DiscordColor.Red;
        return embed;
    }

   
    public async Task<DiscordEmbed> Skip(DiscordMember member)
    {
        var response = await GetPlayerAsync(member.Guild.Id, member.VoiceState.Channel.Id, connectToVoiceChannel: false);

        var embed = new DiscordEmbedBuilder();
        if (response.Item1 is null)
        {
            embed.Description = response.Item2;
            embed.Color = DiscordColor.Red;
            
            return embed;
        }

        var player = response.Item1;

        if (player is null)
        {
            embed.Description = "The bot is not connected to a voice channel.";
            embed.Color = DiscordColor.Red;
            return embed;
        }

        if (player.CurrentTrack is null)
        {
            embed.Description = "Nothing playing right now!";
            embed.Color = DiscordColor.Red;
            return embed;
        }

        await player.SkipAsync().ConfigureAwait(false);

        var track = player.CurrentTrack;

        if (track is not null)
        {
            embed.Description = $"Skipped. Now playing: {track.Uri}";
            embed.Color = DiscordColor.SapGreen;

            return embed;
        }
        
        embed.Description = "Nothing playing!";
        embed.Color = DiscordColor.Red;
        return embed;
    }
    
    public async Task Join(DiscordMember  member)
    {
        var voiceState = member.VoiceState;
        await _audioService.Players.JoinAsync(member.Guild.Id, voiceState.Channel.Id);
    }
    

    public async Task Leave(DiscordMember member)
    {
        var voiceState = member.VoiceState;

        var response = await GetPlayerAsync(member.Guild.Id,member.VoiceState.Channel.Id, connectToVoiceChannel: false);

        var player = response.Item1;

        if (player != null)
        {
            await player.DisconnectAsync();
        }

    }
    
    public async Task<DiscordEmbed> Queue(DiscordMember member)
    {
        var embed = new DiscordEmbedBuilder();
        var response = GetPlayerAsync(member.Guild.Id, member.VoiceState.Channel.Id, connectToVoiceChannel: false).Result;

        if (response.Item1 is null)
        {
            embed.Description = response.Item2;
            embed.Color = DiscordColor.Red;
            return embed;
        }

        var player = response.Item1;

        if (player.Queue.Count == 0)
        {
            embed.Description = "The queue is empty.";
            embed.Color = DiscordColor.Red;
            return embed;
        }

        var tracks = player.Queue.Select(x => x.Track.Title).ToList();
        var queue = string.Join("\n", tracks);

        embed.Description = queue;
        embed.Color = DiscordColor.SapGreen;

        return embed;
    }
    private async ValueTask<(QueuedLavalinkPlayer?, string)> GetPlayerAsync(ulong guildId, ulong channelId,
        bool connectToVoiceChannel = true)
    {
        var channelBehavior = connectToVoiceChannel
            ? PlayerChannelBehavior.Join
            : PlayerChannelBehavior.None;
        var options = new QueuedLavalinkPlayerOptions
        {
            DisconnectOnStop = false,
            ClearQueueOnStop = false,
            SelfDeaf = true
        };
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

        var result = await _audioService.Players
            .RetrieveAsync(guildId, channelId, playerFactory: PlayerFactory.Queued, Options.Create(options),
                retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };
            return (null, errorMessage);
        }

        return (result.Player, "Success");
    }
}
