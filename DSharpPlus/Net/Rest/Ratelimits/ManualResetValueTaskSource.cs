using System;
using System.Threading.Tasks.Sources;

namespace DSharpPlus.Net.Ratelimits;

/// <summary>
/// A manual-reset IValueTaskSource
/// </summary>
internal sealed class ManualResetValueTaskSource : IValueTaskSource
{
    // dummy boolean
    private ManualResetValueTaskSourceCore<bool> core;

    public void GetResult(short token) => this.core.GetResult(token);
    public ValueTaskSourceStatus GetStatus(short token) => this.core.GetStatus(token);
    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        => this.core.OnCompleted(continuation, state, token, flags);

    public void SetException(Exception e) => this.core.SetException(e);
    public void Reset() => this.core.Reset();

    public short Version => core.Version;
}
