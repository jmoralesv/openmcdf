﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using OpenMcdf.Benchmarks;

namespace OpenMcdf.Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
[HideColumns(Column.AllocRatio)]
[MarkdownExporter]
public class MemoryStreamTransactedWrite : IDisposable
{
    private MemoryStream? writeStream;
    private byte[] buffer = Array.Empty<byte>();

    [Params(Version.V3, Version.V4)]
    public Version Version { get; set; }

    [Params(512, 1024 * 1024)]
    public int BufferSize { get; set; }

    [Params(1024 * 1024)]
    public int StreamLength { get; set; }

    public void Dispose()
    {
        writeStream?.Dispose();
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        buffer = new byte[BufferSize];
        writeStream = new MemoryStream(2 * StreamLength);
    }

    [GlobalCleanup]
    public void GlobalCleanup() => Dispose();

    [Benchmark]
    public void WriteTransacted() => OpenMcdfBenchmarks.WriteStream(writeStream!, Version, StorageModeFlags.LeaveOpen | StorageModeFlags.Transacted, buffer, StreamLength);
}
