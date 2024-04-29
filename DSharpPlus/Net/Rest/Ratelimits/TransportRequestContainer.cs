using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace DSharpPlus.Net.Ratelimits;

internal struct TransportRequestContainer(int maximum)
{
    private readonly ManualResetValueTaskSource slotFreeValueTaskSource = new();
    private readonly TransportRequest?[] entries = new TransportRequest?[maximum];

    private SpinLock accessLock = new();
    private volatile int token = 0;

    public void CompleteRequest(Ulid id)
    {

    }

    public ValueTask WaitForFreeSlotAsync()
    {
        int token = Interlocked.Increment(ref this.token);

        // the mere idea of this being hit in legitimately working code is ridiculous, though not impossible.
        // in those pathological cases it's probably fine to just wrap back around to -32768 - one request surviving
        // longer than 65535 other requests is absurd.
        Debug.Assert(token < short.MaxValue);

        // important: do not change this to (short)this.token! that may hit a race condition.
        return new(this.slotFreeValueTaskSource, (short)token);
    }

    public readonly void RegisterRequest(TransportRequest request)
    {
        for (int i = 0; i < maximum; i++)
        {
            if (this.entries[i] is null)
            {
                this.entries[i] = request;
            }
        }
    }

    public readonly int GetActiveRequestCount()
    {
        int counter = 0;

        foreach(TransportRequest? entry in entries)
        {
            if (entry is not null)
            {
                counter++;
            }
        }

        return counter;
    }
}
