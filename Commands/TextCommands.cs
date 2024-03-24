using DSharpPlus.Entities;
namespace Dobby.Commands;

public class TextCommands
{
    public DiscordEmbed Kiss(DiscordUser sender,DiscordUser receiver)
    {
        return new DiscordEmbedBuilder()
        {
            Description = $"<@{sender.Id}> kissed <@{receiver.Id}> muaaaaahh <3 ",
            ImageUrl = "https://media1.tenor.com/m/o_5RQarGvJ0AAAAC/kiss.gif",
            Color = DiscordColor.SapGreen,
            Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = sender.Username,
                IconUrl = sender.AvatarUrl
            }
        };
    }
    
    public DiscordEmbed Hug(DiscordUser sender,DiscordUser receiver)
    {
        return new DiscordEmbedBuilder()
        {
            Title = "Hugged",
            Description = $"<@{sender.Id}> hugged <@{receiver.Id}> <3 ",
            ImageUrl = "https://media1.tenor.com/m/eAKshP8ZYWAAAAAC/cat-love.gif",
            Color = DiscordColor.SapGreen,
            Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = sender.Username,
                IconUrl = sender.AvatarUrl
            }
        };
    }
    public DiscordEmbed Av(DiscordUser user)
    {
        return new DiscordEmbedBuilder()
        {
            Title = "Avatar",
            Description = $"<@{user.Id}>",
            ImageUrl = user.AvatarUrl,
            Color = DiscordColor.SapGreen,
            Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = user.Username,
                IconUrl = user.AvatarUrl
            }
        };
    }
}