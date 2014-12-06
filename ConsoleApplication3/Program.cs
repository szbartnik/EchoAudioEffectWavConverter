using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ConsoleApplication3
{
    class Program
    {
        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ProcessCpp")]
        private static extern unsafe void ProcessCpp(short* lChannelPtr, short* rChannelPtr , int dataSize);

        struct WavHeader
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

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invoke using [inputWavFilePath] [outputWavFilePath]!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine(args[0] + " reading...");

            var header = new WavHeader();

            short[] lDataList;
            short[] rDataList;
            int sizeOfData;

            using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                try
                {
                    header.RiffId = br.ReadBytes(4);
                    header.Size = br.ReadUInt32();
                    header.WavId = br.ReadBytes(4);
                    header.FmtId = br.ReadBytes(4);
                    header.FmtSize = br.ReadUInt32();
                    header.Format = br.ReadUInt16();
                    header.Channels = br.ReadUInt16();
                    header.SampleRate = br.ReadUInt32();
                    header.BytePerSec = br.ReadUInt32();
                    header.BlockSize = br.ReadUInt16();
                    header.Bit = br.ReadUInt16();
                    header.DataId = br.ReadBytes(4);
                    header.DataSize = br.ReadUInt32();

                    sizeOfData = (int) (header.DataSize/header.BlockSize);
                    lDataList = new short[sizeOfData];
                    rDataList = new short[sizeOfData];

                    for (int i = 0; i < sizeOfData; i++)
                    {
                        lDataList[i] = (short)br.ReadUInt16();
                        rDataList[i] = (short)br.ReadUInt16();
                    }
                }
                finally
                {
                    br.Close();
                    fs.Close();
                }
            }

            Console.WriteLine("channels processing...");
            StartCppProcessing(lDataList, rDataList, sizeOfData);


            Console.WriteLine(args[1] + " saving...");

            var lNewDataList = lDataList;
            var rNewDataList = rDataList;

            header.DataSize = (uint)sizeOfData * 4;

            using (var fs = new FileStream(args[1], FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                try
                {
                    bw.Write(header.RiffId);
                    bw.Write(header.Size);
                    bw.Write(header.WavId);
                    bw.Write(header.FmtId);
                    bw.Write(header.FmtSize);
                    bw.Write(header.Format);
                    bw.Write(header.Channels);
                    bw.Write(header.SampleRate);
                    bw.Write(header.BytePerSec);
                    bw.Write(header.BlockSize);
                    bw.Write(header.Bit);
                    bw.Write(header.DataId);
                    bw.Write(header.DataSize);

                    for (int i = 0; i < sizeOfData; i++)
                    {
                        bw.Write((ushort)lNewDataList[i]);
                        bw.Write((ushort)rNewDataList[i]);
                    }
                }
                finally
                {
                    bw.Close();
                    fs.Close();
                }
            }

            Console.ReadKey();
        }

        private static unsafe void StartCppProcessing(short[] lDataList, short[] rDataList, int sizeOfData)
        {
            fixed(short* lDataListPtr = lDataList)
            fixed(short* rDataListPtr = rDataList)
            {
                ProcessCpp(lDataListPtr, rDataListPtr, sizeOfData);
            }
        }
    }
}