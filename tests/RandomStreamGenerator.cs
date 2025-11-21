using CommunityToolkit.HighPerformance;

namespace Toolkit.Minio.Tests;

internal sealed class RandomStreamGenerator
{
    private readonly Random _random = new();
    private readonly Memory<byte> _seedBuffer;

    public RandomStreamGenerator(int maxBufferSize)
    {
        _seedBuffer = new byte[maxBufferSize];
#if NETFRAMEWORK
        _random.NextBytes(_seedBuffer.Span.ToArray());
#else
        _random.NextBytes(_seedBuffer.Span);
#endif
    }

    public Stream GenerateStreamFromSeed(int size)
    {
        var randomWindow = _random.Next(0, size);

        Memory<byte> buffer = new byte[size];

        _seedBuffer[randomWindow..size].CopyTo(buffer[..(size - randomWindow)]);
        _seedBuffer[..randomWindow].CopyTo(buffer.Slice(size - randomWindow, randomWindow));

        return buffer.AsStream();
    }
}