using SignalFilter.Models;

namespace SignalFilter.Services;

public interface ISignalSource
{
    IAsyncEnumerable<RawSignal> ReadSignalsAsync(CancellationToken cancellationToken);
}
