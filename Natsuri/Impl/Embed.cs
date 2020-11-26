using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Natsuri.Impl
{
    [Serializable]
    public class Embed
    {
        public static Embed CreateFromEmbed(IEmbed embed)
        {
            return new Embed()
            {
                Description = embed.Description,
                Url = embed.Url,
                Color = embed.Color?.RawValue.ToString(),
                ImageUrl = embed.Image?.Url,
                Author = embed.Author?.Name,
                Title = embed.Title,
                Footer = embed.Footer?.Text,
                Fields = embed.Fields.Select(x => (x.Name, x.Value)).ToArray()
            };
        }

        public string Description;
        public string Url;
        public string Color;
        public string Author;
        public string ImageUrl;
        public string Title;
        public string Footer;
        public (string, string)[] Fields;
    }
}
