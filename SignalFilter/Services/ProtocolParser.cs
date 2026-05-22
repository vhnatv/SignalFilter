using SignalFilter.Models;

namespace SignalFilter.Services;

public static class ProtocolParser
{
    /// <summary>
    /// Parses 2-byte NetSDR header (LSB first).
    /// Byte 0 = Length_LSB, Byte 1 = [Type(3 bits)][Length_MSB(5 bits)].
    /// </summary>
    public static MessageHeader ParseHeader(byte byte0, byte byte1)
    {
        byte type = (byte)((byte1 >> 5) & 0x07);
        int dataLength = ((byte1 & 0x1F) << 8) | byte0;
        return new MessageHeader(type, dataLength);
    }

    /// <summary>
    /// Parses 28-byte signal payload: uint64 timestamp, uint64 frequency, uint32 bandwidth, double SNR.
    /// </summary>
    public static RawSignal ParsePayload(ReadOnlySpan<byte> data)
    {
        ulong  timestamp = BitConverter.ToUInt64(data[0..8]);
        ulong  frequency = BitConverter.ToUInt64(data[8..16]);
        uint   bandwidth = BitConverter.ToUInt32(data[16..20]);
        double snr       = BitConverter.ToDouble(data[20..28]);
        return new RawSignal(timestamp, frequency, bandwidth, snr);
    }
}
