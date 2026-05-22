using System.Runtime.CompilerServices;
using SignalFilter.Models;

namespace SignalFilter.Services;

public sealed class MockSignalService : ISignalSource
{
    private static readonly ulong[] KnownFrequencies =
    [
        103600000, 103600000, 104200000, 99800000, 105500000
    ];

    private static readonly uint[] KnownBandwidths =
    [
        12500, 12500, 108230, 25000, 12500
    ];

    private readonly Random _rng = new();

    public async IAsyncEnumerable<RawSignal> ReadSignalsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(300, cancellationToken).ConfigureAwait(false);

            int i = _rng.Next(KnownFrequencies.Length);

            ulong freq = KnownFrequencies[i] + (ulong)_rng.Next(-2000, 2000);
            uint  bw   = KnownBandwidths[i];
            ulong ts   = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            double snr = 12.0 + _rng.NextDouble() * 5.0;

            yield return new RawSignal(ts, freq, bw, snr);
        }
    }
}
