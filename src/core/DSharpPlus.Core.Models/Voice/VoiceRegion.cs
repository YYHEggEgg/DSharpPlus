// This Source Code form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using DSharpPlus.Core.Abstractions.Models;

namespace DSharpPlus.Core.Models;

/// <inheritdoc cref="IVoiceRegion" />
public sealed record VoiceRegion : IVoiceRegion
{
    /// <inheritdoc/>
    public required string Id { get; init; }

    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public required bool Optimal { get; init; }

    /// <inheritdoc/>
    public required bool Deprecated { get; init; }

    /// <inheritdoc/>
    public required bool Custom { get; init; }
}