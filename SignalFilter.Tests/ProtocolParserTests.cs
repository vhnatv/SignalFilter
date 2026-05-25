using FluentAssertions;
using SignalFilter.Models;
using SignalFilter.Services;

namespace SignalFilter.Tests;

public class ProtocolParserTests
{
    private static byte[] BuildFrame(ulong timestamp, ulong freq, uint bw, double snr, byte type = 0)
    {
        const int dataLen = 28; // 28 byte of signal size
        byte lengthLsb = (byte)(dataLen & 0xFF);
        byte lengthMsb = (byte)((dataLen >> 8) & 0x1F);
        byte headerByte1 = (byte)((type << 5) | lengthMsb);

        var frame = new byte[2 + dataLen];
        frame[0] = lengthLsb;
        frame[1] = headerByte1;
        BitConverter.GetBytes(timestamp).CopyTo(frame, 2);
        BitConverter.GetBytes(freq).CopyTo(frame, 10);
        BitConverter.GetBytes(bw).CopyTo(frame, 18);
        BitConverter.GetBytes(snr).CopyTo(frame, 22);
        return frame;
    }

    [Fact]
    public void ParseHeader_ReturnsCorrectTypeAndLength()
    {
        var frame = BuildFrame(0, 0, 0, 0, type: 3);
        MessageHeader header = ProtocolParser.ParseHeader(frame[0], frame[1]);
        header.Type.Should().Be(3);
        header.DataLength.Should().Be(28);
    }

    [Fact]
    public void ParsePayload_ReturnsCorrectValues()
    {
        ulong ts   = 1740329520000UL; // 1 January 1970
        ulong freq = 103600000UL; // 103.6 MHz
        uint bw   = 12500U; // 12.5 kHz
        double snr  = 15.235; // dB

        var frame  = BuildFrame(ts, freq, bw, snr);
        var signal = ProtocolParser.ParsePayload(frame[2..]);

        signal.Timestamp.Should().Be(ts);
        signal.Frequency.Should().Be(freq);
        signal.Bandwidth.Should().Be(bw);
        signal.Snr.Should().BeApproximately(snr, 0.0001);
    }

    [Fact]
    public void ParsePayload_ZeroValues_ReturnsZeroSignal()
    {
        var frame  = BuildFrame(0, 0, 0, 0.0);
        var signal = ProtocolParser.ParsePayload(frame[2..]);
        signal.Timestamp.Should().Be(0);
        signal.Frequency.Should().Be(0);
        signal.Bandwidth.Should().Be(0);
        signal.Snr.Should().Be(0.0);
    }
}
