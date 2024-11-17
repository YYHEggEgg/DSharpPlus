// This Source Code form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DSharpPlus.Internal.Abstractions.Rest.Payloads;

namespace DSharpPlus.Internal.Rest.Payloads;

/// <inheritdoc cref="ICreateGuildSoundboardSoundPayload" />
public sealed record CreateGuildSoundboardSoundPayload : ICreateGuildSoundboardSoundPayload
{
    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public required InlineMediaData Sound { get; init; }

    /// <inheritdoc/>
    public Optional<double?> Volume { get; init; }

    /// <inheritdoc/>
    public Optional<Snowflake?> EmojiId { get; init; }

    /// <inheritdoc/>
    public Optional<string?> EmojiName { get; init; }
}