using SignalFilter.Models;

namespace SignalFilter.Services;

public class SignalAggregator
{
    private readonly List<SignalRecord> _records = new();

    public IReadOnlyList<SignalRecord> Records => _records;

    public void Process(RawSignal signal)
    {
        foreach (var record in _records)
        {
            if (record.Matches(signal.Frequency))
            {
                record.AddSignal(signal);
                return;
            }
        }
        _records.Add(new SignalRecord(signal));
    }
}
