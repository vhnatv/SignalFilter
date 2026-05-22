namespace SignalFilter.Models;

public struct MessageHeader
{
    public byte Type { get; private set; }
    public int DataLength { get; private set; }

    public MessageHeader(byte type, int dataLength)
    {
        Type = type;
        DataLength = dataLength;
    }
}
