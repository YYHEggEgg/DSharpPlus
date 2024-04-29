using System;
using System.Threading.Tasks;

using DSharpPlus.Net.Ratelimits;

namespace DSharpPlus.Net;

internal sealed record TransportRequest
{
    public required Ulid Id { get; init; }

    // we're doing cursed async fuckery, this has to be a Task
    public required Func<IRestRequest, Task<InternalResponse>> ExecuteRequestAsync { get; init; }

    public required IRestRequest RequestObject { get; init; }

    public RatelimitBucket? ControllingBucket { get; set; }

    public ManualResetValueTaskSource<InternalResponse> ResultSource { get; init; }
}
