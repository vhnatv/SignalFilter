namespace SignalFilter.Models;

public record RawSignal(
    ulong Timestamp,   // milliseconds
    ulong Frequency,   // Hz
    uint  Bandwidth,   // Hz
    double Snr         // dB
);
