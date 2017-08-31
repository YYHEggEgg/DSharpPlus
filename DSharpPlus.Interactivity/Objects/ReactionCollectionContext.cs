﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSharpPlus.Interactivity
{
    public class ReactionCollectionContext
    {
        public ConcurrentDictionary<DiscordEmoji, int> Reactions = new ConcurrentDictionary<DiscordEmoji, int>();

        internal List<ulong> _membersvoted = new List<ulong>();

        public InteractivityModule Interactivity;

        public DiscordClient Client => Interactivity.Client;

        internal void AddReaction(DiscordEmoji dr)
        {
            if (Reactions.ContainsKey(dr))
                Reactions[dr]++;
            else
                Reactions.TryAdd(dr, 1);
        }

        internal void AddReaction(DiscordEmoji dr, ulong m)
        {
            if (!_membersvoted.Contains(m))
            {
                if (Reactions.ContainsKey(dr))
                    Reactions[dr]++;
                else
                    Reactions.TryAdd(dr, 1);
                _membersvoted.Add(m);
            }
        }

        internal void RemoveReaction(DiscordEmoji dr)
        {
            if (Reactions.ContainsKey(dr))
            {
                Reactions[dr]--;
                if (Reactions[dr] == 0)
                    Reactions.TryRemove(dr, out int something);
            }
        }

        internal void RemoveReaction(DiscordEmoji dr, ulong m)
        {
            if (Reactions.ContainsKey(dr) && _membersvoted.Contains(m))
            {
                Reactions[dr]--;
                if (Reactions[dr] == 0)
                    Reactions.TryRemove(dr, out int something);
                // Just making sure no double member slipped in :^)
                _membersvoted.RemoveAll(x => x == m);
                // Though that should be impossible?
            }
        }

        internal void ClearReactions()
        {
            Reactions = new ConcurrentDictionary<DiscordEmoji, int>();
            _membersvoted = new List<ulong>();
        }
    }
}