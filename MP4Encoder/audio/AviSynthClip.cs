using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class AviSynthClip : IDisposable
    {
        private IntPtr _avs;
        private AVSDLLVideoInfo _vi;
        private AviSynthColorspace _colorSpace;
        private AudioSampleType _sampleType;

        public AviSynthClip(string func, string arg, AviSynthColorspace colorSpace) {
            _vi = new AVSDLLVideoInfo();
            _avs = new IntPtr(0);
            _colorSpace = AviSynthColorspace.Unknown;
            _sampleType = AudioSampleType.Unknown;
            if (0 != dimzon_avs_init(ref _avs, func, arg, ref _vi, ref _colorSpace, ref _sampleType, colorSpace.ToString())) {
                var err = GetLastError();
                throw new ApplicationException(err);
            }
        }

        public int AudioSampleRate {
            get {
                return _vi.audio_samples_per_second;
            }
        }

        public AudioSampleType SampleType {
            get {
                return _vi.sample_type;
            }
        }

        public short ChannelsCount {
            get {
                return (short)_vi.nchannels;
            }
        }

        public short BitsPerSample {
            get {
                return (short)(BytesPerSample * 8);
            }
        }

        public long SamplesCount {
            get {
                return _vi.num_audio_samples;
            }
        }

        public short BytesPerSample {
            get {
                switch (SampleType) {
                    case AudioSampleType.INT8:
                        return 1;
                    case AudioSampleType.INT16:
                        return 2;
                    case AudioSampleType.INT24:
                        return 3;
                    case AudioSampleType.INT32:
                        return 4;
                    case AudioSampleType.FLOAT:
                        return 4;
                    default:
                        throw new ArgumentException(SampleType.ToString());

                }
            }
        }

        public void Dispose() {
            CleanUp(true);
        }

        private string GetLastError() {
            const int errLength = 1024;
            StringBuilder sb = new StringBuilder(errLength);
            sb.Length = dimzon_avs_getlasterror(_avs, sb, errLength);
            return sb.ToString();
        }

        public void CleanUp(bool disposing) {
            dimzon_avs_destroy(ref _avs);
            _avs = new IntPtr(0);
            if (disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~AviSynthClip() {
            CleanUp(false);
        }

        public void ReadAudio(IntPtr addr, long offset, int count) {
            if (0 != dimzon_avs_getaframe(_avs, addr, offset, count))
                throw new Exception(GetLastError());

        }

        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_init(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);

        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getlasterror(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);

        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_destroy(ref IntPtr avs);

        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int dimzon_avs_getaframe(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount);
    }
}
