using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.ObjectPool;

namespace DSharpPlus.Net.Ratelimits;

/// <summary>
/// Represents a rate limit bucket.
/// </summary>
internal sealed class RatelimitBucket : IResettable
{
    private bool live;

    private int maximum;
    private int remaining;
    private DateTimeOffset resetsAt;

    private string hash = "UNSET";
    private readonly List<string> routes = [];

    private TransportRequestContainer activeRequests;
    private readonly Channel<TransportRequest> requestQueue;
    private readonly IRatelimiter ratelimiter;
    private readonly RestClient restClient;
    private readonly ObjectPool<RatelimitBucket> pool;

    public bool TryReset()
    {
        Debug.Assert(this.requestQueue.Reader.Count == 0);

        this.live = false;
        this.hash = "UNSET";
        this.routes.Clear();

        return true;
    }

    // this logic assumes that this is being called after a first request was made. from there, there are two outcomes:
    // another request is being made or we expire.
    // from thereon, if we get a request we will try our utmost to dispatch it - if we have a request, we will wait for
    // a slot to be free and then check whether we're allowed to do that. one annoying side effect is that this request
    // may be dispatched to a dead bucket, but we'll deal with that.
    public async Task LoopAsync()
    {
        TransportRequest request;

        while (true)
        {
            try
            {
                CancellationTokenSource cts = new(this.resetsAt - DateTimeOffset.UtcNow);
                request = await requestQueue.Reader.ReadAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                if (this.activeRequests.GetActiveRequestCount() == 0)
                {
                    break;
                }

                await Task.Delay(this.resetsAt - DateTimeOffset.UtcNow);
                continue;
            }

            await this.activeRequests.WaitForFreeSlotAsync();

            while (this.activeRequests.GetActiveRequestCount() + this.remaining > this.maximum)
            {
                await Task.Delay(this.resetsAt - DateTimeOffset.UtcNow);
            }

            this.activeRequests.RegisterRequest(request);
            _ = request.ExecuteRequestAsync(request.RequestObject).ContinueWith
            (
                (task, state) =>
                {
                    if (state is not TransportRequest request)
                    {
                        return;
                    }

                    if (!task.IsCompletedSuccessfully)
                    {
                        return;
                    }

                    this.UpdateRatelimits(task.Result);
                    request.ResultSource.SetResult(task.Result);
                }
            );
        }

        this.pool.Return(this);
    }
}
