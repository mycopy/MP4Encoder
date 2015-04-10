using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class MuxSetting
    {
         //  private List<MuxStream> audioStreams, subtitleStreams;
        private decimal? framerate;
        private string chapterFile, videoName;
        private string muxedInput, videoInput, audioInput, muxedOutput;

        //private FileSize? splitSize;

        public MuxSetting(string videoInput, string audioInput, string muxedOutput) {
            //audioStreams = new List<MuxStream>();
            //subtitleStreams = new List<MuxStream>();
            framerate = 0.0M;
            muxedInput = "";
            chapterFile = "";
            videoName = "";
            this.videoInput = videoInput;
            this.audioInput = audioInput;
            this.muxedOutput = muxedOutput;
            //splitSize = null;
        }


        public string MuxedInput {
            get { return muxedInput; }
            set { muxedInput = value; }
        }
        public string MuxedOutput {
            get { return muxedOutput; }
            set { muxedOutput = value; }
        }
        public string VideoInput {
            get { return videoInput; }
            set { videoInput = value; }
        }

        public string AudioInput {
            get { return audioInput; }
            set { audioInput = value; }
        }

        /// <summary>
        /// Array of all the audio streams to be muxed
        /// </summary>
        //public List<MuxStream> AudioStreams {
        //    get { return audioStreams; }
        //    set { audioStreams = value; }
        //}
        ///// <summary>
        ///// Array of subtitle tracks to be muxed
        ///// </summary>
        //public List<MuxStream> SubtitleStreams {
        //    get { return subtitleStreams; }
        //    set { subtitleStreams = value; }
        //}
        /// <summary>
        /// framerate of the video
        /// </summary>
        public decimal? Framerate {
            get { return framerate; }
            set { framerate = value; }
        }
        /// <summary>
        /// the file containing the chapter information
        /// </summary>
        public string ChapterFile {
            get { return chapterFile; }
            set { chapterFile = value; }
        }
        /// <summary>
        /// file size at which the output file is to be split
        /// </summary>
        //public FileSize? SplitSize {
        //    get { return splitSize; }
        //    set { splitSize = value; }
        //}

   
        /// <summary>
        /// gets / sets the name of the video track
        /// </summary>
        public string VideoName {
            get { return videoName; }
            set { videoName = value; }
        }

    }
}
