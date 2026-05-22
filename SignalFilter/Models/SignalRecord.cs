using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalFilter.Models;

public partial class SignalRecord : ObservableObject
{
    public ulong AnchorFrequency { get; }
    public uint  AnchorBandwidth { get; }

    private readonly List<ulong> _allFrequencies = new();

    [ObservableProperty] 
    private ulong  _timestamp;
    [ObservableProperty] 
    private double _frequencyMhz;
    [ObservableProperty] 
    private double _bandwidthKhz;
    [ObservableProperty] 
    private double _snr;
    [ObservableProperty] 
    private int    _count;

    public SignalRecord(RawSignal first)
    {
        AnchorFrequency = first.Frequency;
        AnchorBandwidth = first.Bandwidth;

        Timestamp    = first.Timestamp;
        FrequencyMhz = first.Frequency / 1000000.0;
        BandwidthKhz = first.Bandwidth / 1000.0;
        Snr          = first.Snr;
        Count        = 1;

        _allFrequencies.Add(first.Frequency);
    }

    public void AddSignal(RawSignal signal)
    {
        _allFrequencies.Add(signal.Frequency);
        Count++;
        UpdateMedianFrequency();
    }

    private void UpdateMedianFrequency()
    {
        var sorted = _allFrequencies.Order().ToList();
        int mid = sorted.Count / 2;
        double median = sorted.Count % 2 == 1
            ? sorted[mid]
            : (sorted[mid - 1] + sorted[mid]) / 2.0;
        FrequencyMhz = median / 1000000.0;
    }

    public bool Matches(ulong incomingFrequency)
    {
        long half   = (long)(AnchorBandwidth / 2);
        long freq   = (long)incomingFrequency;
        long anchor = (long)AnchorFrequency;
        return freq >= anchor - half && freq < anchor + half;
    }
}
