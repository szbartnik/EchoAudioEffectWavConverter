using System;
using System.IO;
using System.Runtime.InteropServices;
using SMiW.Lab.Models;

namespace SMiW.Lab
{
    public class DspTester
    {
        [DllImport("CppLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ProcessCpp")]
        private static extern unsafe void ProcessCpp(short* lChannelPtr, short* rChannelPtr, int dataSize);

        private WavHeader _wavHeader;
        private short[] _leftChannelData;
        private short[] _rightChannelData;
        private int _sizeOfData;

        public DspTester(string inputFilePath)
        {
            ReadInputFile(inputFilePath);
        }

        private void ReadInputFile(string inputFilePath)
        {
            using (var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                try
                {
                    _wavHeader.RiffId = br.ReadBytes(4);
                    _wavHeader.Size = br.ReadUInt32();
                    _wavHeader.WavId = br.ReadBytes(4);
                    _wavHeader.FmtId = br.ReadBytes(4);
                    _wavHeader.FmtSize = br.ReadUInt32();
                    _wavHeader.Format = br.ReadUInt16();
                    _wavHeader.Channels = br.ReadUInt16();
                    _wavHeader.SampleRate = br.ReadUInt32();
                    _wavHeader.BytePerSec = br.ReadUInt32();
                    _wavHeader.BlockSize = br.ReadUInt16();
                    _wavHeader.Bit = br.ReadUInt16();
                    _wavHeader.DataId = br.ReadBytes(4);
                    _wavHeader.DataSize = br.ReadUInt32();

                    _sizeOfData = (int) (_wavHeader.DataSize/_wavHeader.BlockSize);
                    _leftChannelData = new short[_sizeOfData];
                    _rightChannelData = new short[_sizeOfData];

                    for (int i = 0; i < _sizeOfData; i++)
                    {
                        _leftChannelData[i] = (short) br.ReadUInt16();
                        _rightChannelData[i] = (short) br.ReadUInt16();
                    }
                }
                catch { }
            }
        }

        public void SaveOutputFile(string outputFilePath)
        {
            _wavHeader.DataSize = (uint)_sizeOfData * 4;

            using (var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                try
                {
                    bw.Write(_wavHeader.RiffId);
                    bw.Write(_wavHeader.Size);
                    bw.Write(_wavHeader.WavId);
                    bw.Write(_wavHeader.FmtId);
                    bw.Write(_wavHeader.FmtSize);
                    bw.Write(_wavHeader.Format);
                    bw.Write(_wavHeader.Channels);
                    bw.Write(_wavHeader.SampleRate);
                    bw.Write(_wavHeader.BytePerSec);
                    bw.Write(_wavHeader.BlockSize);
                    bw.Write(_wavHeader.Bit);
                    bw.Write(_wavHeader.DataId);
                    bw.Write(_wavHeader.DataSize);

                    for (int i = 0; i < _sizeOfData; i++)
                    {
                        bw.Write((ushort) _leftChannelData[i]);
                        bw.Write((ushort) _rightChannelData[i]);
                    }
                }
                catch { }
            }
        }

        public unsafe void StartCppProcessing()
        {
            fixed (short* lDataListPtr = _leftChannelData)
            fixed (short* rDataListPtr = _rightChannelData)
            {
                ProcessCpp(lDataListPtr, rDataListPtr, _sizeOfData);
            }
        }
    }

    public static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invoke using [inputWavFilePath.wav] [outputWavFilePath.wav]!");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine("Reading input file '{0}'", args[0]);
                var dspTester = new DspTester(args[0]);

                Console.WriteLine("Sound processing...");
                dspTester.StartCppProcessing();

                Console.WriteLine("Saving output file '{0}'", args[1]);
                dspTester.SaveOutputFile(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Successfully saved!");
            Console.ReadKey();
        }
    }
}