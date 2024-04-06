// This Source Code form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DSharpPlus.Internal.Abstractions.Models;

namespace DSharpPlus.Internal.Models;

/// <inheritdoc cref="IApplicationIntegrationTypeConfiguration" />
public sealed record ApplicationIntegrationTypeConfiguration : IApplicationIntegrationTypeConfiguration
{
    /// <inheritdoc/>
    public Optional<IInstallParameters> Oauth2InstallParams { get; init; }
}
