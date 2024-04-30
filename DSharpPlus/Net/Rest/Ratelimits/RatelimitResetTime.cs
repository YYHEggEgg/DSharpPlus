using System;

namespace DSharpPlus.Net.Ratelimits;

/// <summary>
/// Represents the reset time, as well as whether that reset time is synthetic or specified by Discord.
/// </summary>
/// <remarks>
/// This structure is mutable and designed to update itself.
/// </remarks>

/*************************************************************************************************
* we can save 8 bytes here by doing the following:
*
* [StructLayout(StructLayoutKind.Explicit)]
* struct RatelimitResetTime
* {
*     [FieldOffset(0)]
*     DateTimeOffset time;
*
*     [FieldOffset(10)]
*     bool synthetic;
* }
*
* which is equivalent except for using padding bytes better, however, that's also unhinged. a bit
* too much for this version of this library. but anyway: the reason this works is because
* DateTimeOffset is 10 bytes large, padded to 16. then the bool field is added, taking the 17th
* byte and padding to 24 bytes total. we can instead align the bool directly after the time, which
* is always legal (but relies on runtime implementation details, namely the field order and size),
* which gets rid of the extra 7 bytes of padding from the bool plus moves the bool into previously
* padded memory, therefore saving 8 bytes total.
***************************************************************************************************/

internal struct RatelimitResetTime(DateTimeOffset nextReset)
{
    private static readonly TimeSpan syntheticIncrement = TimeSpan.FromSeconds(1);

    public DateTimeOffset NextResetTime { get; private set; } = nextReset;
    public bool Synthetic { get; private set; } = false;

    public readonly TimeSpan UntilReset => DateTimeOffset.UtcNow - this.NextResetTime;

    /// <summary>
    /// Sets the next reset time as obtained from Discord.
    /// </summary>
    public void SetReset(DateTimeOffset nextReset)
    {
        this.NextResetTime = nextReset;
        this.Synthetic = false;
    }

    /// <summary>
    /// Checks whether this ratelimit is expired, and if so, sets the next expiry one second into the future.
    /// </summary>
    public bool CheckExpiry()
    {
        if (this.NextResetTime < DateTimeOffset.UtcNow)
        {
            this.NextResetTime = DateTimeOffset.UtcNow + syntheticIncrement;
            this.Synthetic = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
