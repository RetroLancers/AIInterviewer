using System.IO;
using System.Text;

namespace AIInterviewer.ServiceInterface.Utilities;

public static class WavUtils
{
    public static byte[] AddWavHeader(byte[] pcmData, int sampleRate, short channels = 1, short bitsPerSample = 16)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + pcmData.Length);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);
        writer.Write((short)1); // AudioFormat 1 = PCM
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * bitsPerSample / 8); // ByteRate
        writer.Write((short)(channels * bitsPerSample / 8)); // BlockAlign
        writer.Write(bitsPerSample);
        writer.Write("data".ToCharArray());
        writer.Write(pcmData.Length);
        writer.Write(pcmData);

        return stream.ToArray();
    }
}
