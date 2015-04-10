using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class AudioCodecSetting
    {
        public int Bitrate { get; set; }
        public BitrateMode BitrateMode { get; set; }
        public bool ImproveAccuracy { get; set; }
        public bool AutoGain { get; set; }
        public bool DirectShow { get; set; }
        public decimal Quality { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public AudioCodecSetting(string input, string output, decimal quality, BitrateMode mode = BitrateMode.VBR) {
            this.Quality = quality;
            this.BitrateMode = mode;
            this.Input = input;
            this.Output = output;
        }
    }
}
