using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Players;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.VisualBasic.CompilerServices;


namespace Dobby.Commands
{
    // Inside your TextCommands class or wherever appropriate
    public class VoiceCommandsController : BaseCommandModule
    {
        public VoiceCommands _commands;
        public VoiceCommandsController(VoiceCommands _voiceCommands)
        {
            _commands = _voiceCommands;
        }

        [Command("p")]
        public async Task Play(CommandContext ctx, [RemainingText] string query)
        {
            var embed = await _commands.Play(ctx.Member, query);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("s")]
        public async Task Skip(CommandContext ctx)
        {
            var embed = await _commands.Skip(ctx.Member);
            await ctx.RespondAsync(embed: embed);
        }

        [Command("join")]
        [Description("Joins the voice channel you are currently in.")]
        public async Task Join(CommandContext ctx)
        {
            var voiceState = ctx.Member?.VoiceState;

            var embed = new DiscordEmbedBuilder();
            if (voiceState?.Channel == null)
            {

                embed.Description = "You must be in a voice channel to use this command.";
                embed.Color = DiscordColor.Red;
                await ctx.RespondAsync(embed);
                return;
            }

            await _commands.Join(ctx.Member);
        }

        [Command("leave")]
        [Description("Leaves the voice channel.")]

        public async Task Leave(CommandContext ctx)
        {
            var voiceState = ctx.Member?.VoiceState;

            if (voiceState?.Channel == null)
            {
                await ctx.RespondAsync("You must be in a voice channel to use this command.");
                return;
            }

            await _commands.Leave(ctx.Member);
        }
        
        [Command("q")]
        public async Task Queue(CommandContext ctx)
        {
            var embed = await _commands.ViewQueue(ctx.Member);
            await ctx.RespondAsync(embed: embed);
        }
        
        [Command("rm")]
        public async Task Remove(CommandContext ctx, int position)
        {
            var embed = await _commands.RemoveFromQueue(ctx.Member, position);
            await ctx.RespondAsync(embed: embed);
        }
        
        [Command("insert")]
        public async Task Insert(CommandContext ctx, [RemainingText] string query,int position = 1)
        {
            var embed = await _commands.Play(ctx.Member, query,position);
            await ctx.RespondAsync(embed: embed);
        }

    }
}

