using FluentAssertions;
using SignalFilter.Models;
using SignalFilter.Services;

namespace SignalFilter.Tests;

public class SignalAggregatorTests
{
    private static RawSignal Signal(ulong freqHz, uint bwHz = 12500, double snr = 15.0, ulong ts = 1000)
        => new(ts, freqHz, bwHz, snr);

    [Fact]
    public void FirstSignal_CreatesNewRecord()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(103600000));

        agg.Records.Should().HaveCount(1);
        agg.Records[0].Count.Should().Be(1);
        agg.Records[0].AnchorFrequency.Should().Be(103600000);
    }

    [Fact]
    public void MatchingSignal_IncrementsCount()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(103600000, bwHz: 12500));
        agg.Process(Signal(103604000, bwHz: 12500));

        agg.Records.Should().HaveCount(1);
        agg.Records[0].Count.Should().Be(2);
    }

    [Fact]
    public void SignalAtLowerBoundary_Matches()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(103600000, bwHz: 12500)); // half = 6250; lower = 103593750
        agg.Process(Signal(103593750));                // exactly at lower bound - inclusive

        agg.Records[0].Count.Should().Be(2);
    }

    [Fact]
    public void SignalAtUpperBoundary_DoesNotMatch()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(103600000, bwHz: 12500)); // upper = 103_606_250 (exclusive)
        agg.Process(Signal(103606250));                // exactly at upper bound - new record

        agg.Records.Should().HaveCount(2);
    }

    [Fact]
    public void NonMatchingSignal_CreatesNewRecord()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(103600000));
        agg.Process(Signal(200000000));

        agg.Records.Should().HaveCount(2);
    }

    [Fact]
    public void MedianFrequency_OddCount_IsMiddleValue()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(100000000, bwHz: 100000000));
        agg.Process(Signal(102000000, bwHz: 100000000));
        agg.Process(Signal(104000000, bwHz: 100000000));

        agg.Records[0].FrequencyMhz.Should().BeApproximately(102.0, 0.001);
    }

    [Fact]
    public void MedianFrequency_EvenCount_IsAverageOfTwoMiddle()
    {
        var agg = new SignalAggregator();
        agg.Process(Signal(100000_000, bwHz: 100000000));
        agg.Process(Signal(102000_000, bwHz: 100000000));

        agg.Records[0].FrequencyMhz.Should().BeApproximately(101.0, 0.001);
    }
}
