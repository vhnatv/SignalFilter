using System.Net.Sockets;
using SignalFilter.Models;
using System.Runtime.CompilerServices;

namespace SignalFilter.Services;

public class TcpSignalService : ISignalSource
{
    private readonly string _host;
    private readonly int _port;

    public TcpSignalService(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async IAsyncEnumerable<RawSignal> ReadSignalsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_host, _port, cancellationToken).ConfigureAwait(false);
        await using var stream = client.GetStream();

        var headerBuf = new byte[2];
        while (!cancellationToken.IsCancellationRequested)
        {
            await stream.ReadExactlyAsync(headerBuf, cancellationToken).ConfigureAwait(false);
            MessageHeader header = ProtocolParser.ParseHeader(headerBuf[0], headerBuf[1]);

            var payload = new byte[header.DataLength];
            await stream.ReadExactlyAsync(payload, cancellationToken).ConfigureAwait(false);

            yield return ProtocolParser.ParsePayload(payload);
        }
    }
}
