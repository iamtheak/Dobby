using Dobby.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;


namespace Dobby.Commands
{
    public class TextCommandController : BaseCommandModule
    {
        public TextCommands _TextCommands = new TextCommands();
        [Command("ping")]
        public async Task Ping(CommandContext ctx,DiscordUser user, int n = 1)
        {

            try
            {
                if (user.Username == null)
                {

                    await TextHelper.SendMessage(ctx, "No user mentioned a user");
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    await TextHelper.SendMessage(ctx, $"<@{user.Id}>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        [Command("nick")]
        public async Task Nick(CommandContext ctx, DiscordUser user, string nick)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            await member.ModifyAsync(x => x.Nickname = nick);
        }

        [Command("kiss")]

        public async Task Kiss(CommandContext ctx, DiscordUser user)
        {
            var message = _TextCommands.Kiss(ctx.User, user);
            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("hug")]

        public async Task Hug(CommandContext ctx, DiscordUser user)
        {
            var message = new DiscordEmbedBuilder()
            {
                Title = "Hugged",
                Description = $"<@{ctx.User.Id}> hugged <@{user.Id}> <3 ",
                ImageUrl = "https://media1.tenor.com/m/eAKshP8ZYWAAAAAC/cat-love.gif",
                Color = DiscordColor.SapGreen,

            };
            message.WithAuthor(ctx.User.Username, null,ctx.User.AvatarUrl);
            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("av")]

        public async Task Av(CommandContext ctx, DiscordUser user)
        {

            var message = new DiscordEmbedBuilder()
            {
                Title = "Avatar",
                Description = $"<@{user.Id}>",
                ImageUrl = user.AvatarUrl,
                Color = DiscordColor.SapGreen,
            };
            message.WithAuthor(ctx.User.Username, null,ctx.User.AvatarUrl);

            await ctx.Channel.SendMessageAsync(embed: message);
        }

        [Command("av")]

        public async Task Av(CommandContext ctx)
        {

            var message = new DiscordEmbedBuilder()
            {
                Title = "Avatar",
                Description = $"<@{ctx.User.Id}> looking good",
                ImageUrl = ctx.User.AvatarUrl,
                Color = DiscordColor.SapGreen,

            };
            message.WithAuthor(ctx.User.Username,null, ctx.User.AvatarUrl);
            await ctx.Channel.SendMessageAsync(embed: message);
        }
    }
}
