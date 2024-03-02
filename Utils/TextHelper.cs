using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dobby.Utils
{
    static class TextHelper
    {
        public static async Task SendMessage(CommandContext ctx, string message)
        {
            try
            {
                await ctx.Channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
