namespace SMiW.Lab.Models
{
    internal struct WavHeader
    {
        public byte[] RiffId;
        public uint Size;
        public byte[] WavId;
        public byte[] FmtId;
        public uint FmtSize;
        public ushort Format;
        public ushort Channels;
        public uint SampleRate;
        public uint BytePerSec;
        public ushort BlockSize;
        public ushort Bit;
        public byte[] DataId;
        public uint DataSize;
    }
}