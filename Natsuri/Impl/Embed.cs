using System;
using System.Collections.Generic;
using Discord;

namespace Natsuri.Impl
{
    [Serializable]
    public class Embed
    {
        public static Embed InitEmbed(IEmbed embed)
        {
            Embed NewEmbed = new Embed
            {
                Description = embed.Description,
                Url = embed.Url,
                Color = embed.Color?.RawValue.ToString(),
                ImageUrl = embed.Image.HasValue ? embed.Image.Value.Url : null,
                Author = embed.Author.HasValue ? embed.Author.Value.Name : null,
                Title = embed.Title,
                Footer = embed.Footer.HasValue ? embed.Footer.Value.Text : null
            };

            foreach (var field in embed.Fields)
            {
                NewEmbed.Fields.Add((field.Name, field.Value));
            }

            return NewEmbed;
        }

        public List<(string, string)> Fields = new List<(string, string)>();
        public string Description;
        public string Url;
        public string Color;
        public string Author;
        public string ImageUrl;
        public string Title;
        public string Footer;
    }
}
