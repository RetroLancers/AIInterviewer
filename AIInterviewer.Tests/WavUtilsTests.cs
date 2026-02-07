using NUnit.Framework;
using AIInterviewer.ServiceInterface.Utilities;
using System.IO;
using System.Text;

namespace AIInterviewer.Tests;

[TestFixture]
public class WavUtilsTests
{
    [Test]
    public void AddWavHeader_ShouldCreateCorrectHeader_WithDefaultValues()
    {
        // Arrange
        var pcmData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        int sampleRate = 44100;

        // Act
        var wavData = WavUtils.AddWavHeader(pcmData, sampleRate);

        // Assert
        Assert.That(wavData.Length, Is.EqualTo(44 + pcmData.Length));

        using var stream = new MemoryStream(wavData);
        using var reader = new BinaryReader(stream);

        // Check RIFF header
        var riffChunkId = reader.ReadChars(4);
        Assert.That(new string(riffChunkId), Is.EqualTo("RIFF"));

        var chunkSize = reader.ReadInt32();
        Assert.That(chunkSize, Is.EqualTo(36 + pcmData.Length));

        var format = reader.ReadChars(4);
        Assert.That(new string(format), Is.EqualTo("WAVE"));

        // Check fmt sub-chunk
        var subChunk1Id = reader.ReadChars(4);
        Assert.That(new string(subChunk1Id), Is.EqualTo("fmt "));

        var subChunk1Size = reader.ReadInt32();
        Assert.That(subChunk1Size, Is.EqualTo(16));

        var audioFormat = reader.ReadInt16();
        Assert.That(audioFormat, Is.EqualTo(1)); // PCM

        var numChannels = reader.ReadInt16();
        Assert.That(numChannels, Is.EqualTo(1)); // Default

        var sampleRateRead = reader.ReadInt32();
        Assert.That(sampleRateRead, Is.EqualTo(sampleRate));

        var byteRate = reader.ReadInt32();
        Assert.That(byteRate, Is.EqualTo(sampleRate * 1 * 16 / 8)); // sampleRate * NumChannels * BitsPerSample/8

        var blockAlign = reader.ReadInt16();
        Assert.That(blockAlign, Is.EqualTo(1 * 16 / 8)); // NumChannels * BitsPerSample/8

        var bitsPerSample = reader.ReadInt16();
        Assert.That(bitsPerSample, Is.EqualTo(16)); // Default

        // Check data sub-chunk
        var subChunk2Id = reader.ReadChars(4);
        Assert.That(new string(subChunk2Id), Is.EqualTo("data"));

        var subChunk2Size = reader.ReadInt32();
        Assert.That(subChunk2Size, Is.EqualTo(pcmData.Length));

        var data = reader.ReadBytes(pcmData.Length);
        Assert.That(data, Is.EqualTo(pcmData));
    }

    [Test]
    public void AddWavHeader_ShouldCreateCorrectHeader_WithCustomValues()
    {
        // Arrange
        var pcmData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        int sampleRate = 48000;
        short channels = 2;
        short bitsPerSample = 24;

        // Act
        var wavData = WavUtils.AddWavHeader(pcmData, sampleRate, channels, bitsPerSample);

        // Assert
        Assert.That(wavData.Length, Is.EqualTo(44 + pcmData.Length));

        using var stream = new MemoryStream(wavData);
        using var reader = new BinaryReader(stream);

        // Skip basic RIFF/WAVE header checks, focus on format details
        stream.Seek(22, SeekOrigin.Begin); // Skip RIFF(4) + Size(4) + WAVE(4) + fmt (4) + Size(4) + AudioFormat(2)

        var numChannels = reader.ReadInt16();
        Assert.That(numChannels, Is.EqualTo(channels));

        var sampleRateRead = reader.ReadInt32();
        Assert.That(sampleRateRead, Is.EqualTo(sampleRate));

        var byteRate = reader.ReadInt32();
        Assert.That(byteRate, Is.EqualTo(sampleRate * channels * bitsPerSample / 8));

        var blockAlign = reader.ReadInt16();
        Assert.That(blockAlign, Is.EqualTo(channels * bitsPerSample / 8));

        var bitsPerSampleRead = reader.ReadInt16();
        Assert.That(bitsPerSampleRead, Is.EqualTo(bitsPerSample));
    }
}
