using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public enum BitrateMode { CBR, VBR, ABR };
    public enum ZONEMODE : int { QUANTIZER = 0, WEIGHT };
    public enum MuxerType { MP4BOX, MKVMERGE, AVC2AVI, AVIMUXGUI, DIVXMUX, FFMPEG, ATOMCHANGER };
    public enum StreamType : ushort { None = 0, Stderr = 1, Stdout = 2 };
    public enum AviSynthColorspace : int
    {
        Unknown = 0,
        YV12 = -1610612728,
        RGB24 = +1342177281,
        RGB32 = +1342177282,
        YUY2 = -1610612740,
        I420 = -1610612720,
        IYUV = I420
    }

    public enum AudioSampleType : int
    {
        Unknown = 0,
        INT8 = 1,
        INT16 = 2,
        INT24 = 4,    // Int24 is a very stupid thing to code, but it's supported by some hardware.
        INT32 = 8,
        FLOAT = 16
    };

    [StructLayout(LayoutKind.Sequential)]
    struct AVSDLLVideoInfo
    {
        public int width;
        public int height;
        public int raten;
        public int rated;
        public int aspectn;
        public int aspectd;
        public int interlaced_frame;
        public int top_field_first;
        public int num_frames;
        public AviSynthColorspace pixel_type;

        // Audio
        public int audio_samples_per_second;
        public AudioSampleType sample_type;
        public int nchannels;
        public int num_audio_frames;
            public long num_audio_samples;
    }
}
