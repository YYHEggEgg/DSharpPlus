using DSharpPlus.Caching;
using DSharpPlus.Entities;

namespace DSharpPlus.EventArgs;

/// <summary>
/// Represents arguments for <see cref="DiscordClient.InviteDeleted"/>
/// </summary>
public sealed class InviteDeleteEventArgs : DiscordEventArgs
{
    /// <summary>
    /// Gets the guild that deleted the invite.
    /// </summary>
    public CachedEntity<ulong, DiscordGuild> Guild { get; internal set; }

    /// <summary>
    /// Gets the channel that the invite was for.
    /// </summary>
    public CachedEntity<ulong, DiscordChannel> Channel { get; internal set; }

    /// <summary>
    /// Gets the deleted invite.
    /// </summary>
    public DiscordInvite Invite { get; internal set; }

    internal InviteDeleteEventArgs() : base() { }
}
