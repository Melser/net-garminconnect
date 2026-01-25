namespace GarminConnect.Api;

/// <summary>
/// A stream wrapper that ensures the underlying HttpResponseMessage is disposed
/// when the stream is disposed.
/// </summary>
internal sealed class HttpResponseStream : Stream
{
    private readonly HttpResponseMessage _response;
    private readonly Stream _innerStream;
    private bool _disposed;

    public HttpResponseStream(HttpResponseMessage response, Stream innerStream)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override void Flush() => _innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) =>
        _innerStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) =>
        _innerStream.Seek(offset, origin);

    public override void SetLength(long value) =>
        _innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) =>
        _innerStream.Write(buffer, offset, count);

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerStream.ReadAsync(buffer, cancellationToken);

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerStream.WriteAsync(buffer, offset, count, cancellationToken);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerStream.WriteAsync(buffer, cancellationToken);

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _innerStream.FlushAsync(cancellationToken);

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _innerStream.Dispose();
            _response.Dispose();
        }

        _disposed = true;
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await _innerStream.DisposeAsync().ConfigureAwait(false);
        _response.Dispose();

        _disposed = true;
        await base.DisposeAsync().ConfigureAwait(false);
    }
}
