using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Dobby.Commands;

public class SlashCommandController : ApplicationCommandModule
{
    public TextCommands _TextCommands;
    public VoiceCommands _VoiceCommands;
    
    public SlashCommandController(TextCommands textCommands, VoiceCommands voiceCommands)
    {
        _TextCommands = textCommands;
        _VoiceCommands = voiceCommands;
    }

    [SlashCommand("Kiss", "Kiss someone")]
    public async Task Kiss(InteractionContext ctx, [Option("User", "User to kiss")] DiscordUser user)
    {
        await ctx.DeferAsync();
        var message = _TextCommands.Kiss(ctx.User, user);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(message));
    }

    [SlashCommand("Hug", "Hug someone")]
    public async Task Hug(InteractionContext ctx, [Option("User", "User to hug")] DiscordUser user)
    {
        await ctx.DeferAsync();
        var message = _TextCommands.Hug(ctx.User, user);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(message));
    }
    
    [SlashCommand("Avatar", "Get user avatar")]
    
    public async Task Av(InteractionContext ctx, [Option("User", "User to get avatar")] DiscordUser user)
    {
        await ctx.DeferAsync();
        var message = _TextCommands.Av(user);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(message));
    }
    
    [SlashCommand("Play", "Play music")]
    
    public async Task Play(InteractionContext ctx, [Option("Query", "Query to search for music")][RemainingText] string query)
    {
        await ctx.DeferAsync();
        var message = await _VoiceCommands.Play(ctx.Member, query);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(message));
    }

}