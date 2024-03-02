using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dobby.ApiControl;
using Dobby.Utils;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using Lavalink4NET.Players;
using Lavalink4NET;
using Lavalink4NET.Rest;
using DSharpPlus.VoiceNext;
using Lavalink4NET.Players.Vote;
using Microsoft.Extensions.DependencyInjection;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;


namespace Dobby.Commands
{
    
        // Inside your TextCommands class or wherever appropriate
        public class VoiceCommands : BaseCommandModule
        {

            public IAudioService _audioService { private get; set; }
            public DiscordClient _discordClient { private get; set; }
            public VoiceCommands(IAudioService audioService,DiscordClient client)
            {
                _audioService = audioService;
                _discordClient = client;

                _discordClient.ConnectAsync().ConfigureAwait(false);

                var readyTaskCompletionSource = new TaskCompletionSource();

                Task SetResult(DiscordClient client, ReadyEventArgs eventArgs)
                {
                    readyTaskCompletionSource.TrySetResult();
                    return Task.CompletedTask;
                }

                _discordClient.Ready += SetResult;
                 readyTaskCompletionSource.Task.ConfigureAwait(false);
                _discordClient.Ready -= SetResult;
            }

            [Command("play")]
            public async Task Play(CommandContext ctx, string trackUrl)
            {
                var voiceState = ctx.Member?.VoiceState;

                if (voiceState?.Channel == null)
                {
                    await ctx.RespondAsync("You must be in a voice channel to use this command.");
                    return;
                }

       
                var playerOptions = new LavalinkPlayerOptions
                {
                    InitialTrack = new TrackQueueItem(new TrackReference(trackUrl)),
                    SelfDeaf = true
                };

                await _audioService.StartAsync().ConfigureAwait(false);
                await Task.Delay(3000);


                await _audioService.Players.JoinAsync(ctx.Guild.Id, voiceState.Channel.Id, playerOptions);

                await ctx.RespondAsync($"Now playing: {trackUrl}");
            }

            [Command("join")]
            [Description("Joins the voice channel you are currently in.")]
            public async Task Join(CommandContext ctx)
            {
                var voiceState = ctx.Member?.VoiceState;

                if (voiceState?.Channel == null)
                {
                    await ctx.RespondAsync("You must be in a voice channel to use this command.");
                    return;
                }

                var _audioService = ctx.Services.GetRequiredService<IAudioService>();

                

                await _audioService.Players.JoinAsync(ctx.Guild.Id, voiceState.Channel.Id);
                await ctx.RespondAsync($"Joined voice channel: {voiceState.Channel.Name}");
            }

        private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(CommandContext ctx, bool connectToVoiceChannel = true)
        {
            var channelBehavior = connectToVoiceChannel
                ? PlayerChannelBehavior.Join
                : PlayerChannelBehavior.None;

            var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: channelBehavior);

            var audioService = ctx.Services.GetRequiredService<IAudioService>();
            var result = await _audioService.Players.RetrieveAsync(ctx.Guild.Id, ctx.Member.VoiceState.Channel.Id, retrieveOptions).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                    _ => "Unknown error.",
                };

                await ctx.RespondAsync(errorMessage);
                return null;
            }

            return result.Player;
        }
    }


    }


