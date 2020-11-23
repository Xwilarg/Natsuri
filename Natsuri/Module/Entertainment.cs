﻿using Discord.Commands;
using Natsuri.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natsuri.Module
{
    public class Entertainment : ModuleBase
    {
        private char[] _splitChar = new[]
        {
            '.', ' ', '/', '\\', '!', '?', ',', ';', '(', ')', '\n', ':', '_', '`', '*', '"'
        };

        [Command("Sentence all", RunMode = RunMode.Async)]
        public async Task SentenceAll()
        {
            await ReplyAsync(SentenceInternal(Program.Db.GetMessages(Context.Guild.Id)));
        }

        [Command("Sentence", RunMode = RunMode.Async)]
        public async Task Sentence()
        {
            await ReplyAsync(SentenceInternal(Program.Db.GetMessages(Context.Guild.Id, Context.User.Id)));
        }

        private string SentenceInternal(List<Message> messages)
        {
            messages = messages.Where(x => !x.Content.All(x => _splitChar.Contains(x)) && !x.Content.StartsWith("n.")).ToList();
            var start = messages[Program.Rand.Next(0, messages.Count)];
            var current = start;
            var currentText = current.Content.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0];
            string nextText = null;
            StringBuilder str = new StringBuilder(currentText);
            var validSentences = messages.Where(x => x.Content.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries).Contains(currentText) && x.MessageId != current.MessageId).ToArray();
            var errorIteration = 100;
            while (validSentences.Length > 0)
            {
                current = validSentences[Program.Rand.Next(0, validSentences.Length)];
                var allWords = current.Content.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries);
                var possible = allWords.Count(x => x == currentText);
                int random = Program.Rand.Next(0, possible);
                string currentProg = current.Content;
                string lastCharacs;
                int curr;
                while (true)
                {
                    lastCharacs = "";
                    while (currentProg.Length > 0 && _splitChar.Contains(currentProg[0]))
                    {
                        lastCharacs += currentProg[0];
                        currentProg = currentProg[1..];
                    }
                    if (currentProg.Length == 0)
                        goto end;
                    if (currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0] == currentText)
                    {
                        if (random == 0)
                        {
                            curr = currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0].Length;
                            currentProg = currentProg.Substring(curr, currentProg.Length - curr);
                            if (currentProg.Length == 0)
                                goto end;
                            lastCharacs = "";
                            while (currentProg.Length > 0 && _splitChar.Contains(currentProg[0]))
                            {
                                lastCharacs += currentProg[0];
                                currentProg = currentProg[1..];
                            }
                            if (currentProg.Length == 0)
                                goto end;
                            var firstWord = currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0];
                            if (firstWord == nextText || firstWord == currentText) // TODO: Quick fix
                                goto next;
                            currentText = firstWord;
                            nextText = null;
                            if (currentProg.Length > 0)
                            {
                                currentProg = currentProg.Substring(0, currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0].Length);
                                while (currentProg.Length > 0 && _splitChar.Contains(currentProg[0]))
                                    currentProg = currentProg[1..];
                                curr = currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0].Length;
                                nextText = currentProg.Length == 0 ? null : currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0];
                            }
                            break;
                        }
                        random--;
                    }
                    curr = currentProg.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries)[0].Length;
                    currentProg = currentProg.Substring(curr, currentProg.Length - curr);
                }
                if (currentText == null)
                    break;
                str.Append((lastCharacs == "" ? " " : lastCharacs) + currentText);
                next:
                validSentences = messages.Where(x => x.Content.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries).Contains(currentText) && x.MessageId != current.MessageId).ToArray();
                errorIteration--;
                if (errorIteration == 0)
                    break;
            }
            end:
            return str.ToString();
        }
    }
}
