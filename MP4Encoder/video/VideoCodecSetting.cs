using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public abstract class VideoCodecSetting
    {
        public VideoCodecSetting() {
            logfile = ".stats";
            customEncoderOptions = "";
            videoName = "";
            fourCC = 0;
            nbThreads = 1;
        }

        public virtual void setAdjustedNbThreads(int nbThreads) {
            NbThreads = nbThreads;
        }

        public override int GetHashCode() {
            // DO NOT CALL BASE.GETHASHCODE!
            return 0;
        }

        public enum Mode : int { CBR = 0, CQ, twopass1, twopass2, twopassAutomated, threepass1, threepass2, threepass3, threepassAutomated, quality };
        int encodingMode, bitrateQuantizer, keyframeInterval, nbBframes, minQuantizer, maxQuantizer, fourCC,
            maxNumberOfPasses, nbThreads;
        bool turbo, v4mv, qpel, trellis;
        decimal creditsQuantizer;
        private string logfile, customEncoderOptions, videoName;
        private string[] fourCCs;

        public abstract bool UsesSAR {
            get;
        }

        public int EncodingMode {
            get { return encodingMode; }
            set { encodingMode = value; }
        }
        public int BitrateQuantizer {
            get { return bitrateQuantizer; }
            set { bitrateQuantizer = value; }
        }
        public int KeyframeInterval {
            get { return keyframeInterval; }
            set { keyframeInterval = value; }
        }
        public int NbBframes {
            get { return nbBframes; }
            set { nbBframes = value; }
        }
        public int MinQuantizer {
            get { return minQuantizer; }
            set { minQuantizer = value; }
        }
        public int MaxQuantizer {
            get { return maxQuantizer; }
            set { maxQuantizer = value; }
        }
        public bool Turbo {
            get { return turbo; }
            set { turbo = value; }
        }
        public bool V4MV {
            get { return v4mv; }
            set { v4mv = value; }
        }
        public bool QPel {
            get { return qpel; }
            set { qpel = value; }
        }
        public bool Trellis {
            get { return trellis; }
            set { trellis = value; }
        }
        public decimal CreditsQuantizer {
            get { return creditsQuantizer; }
            set { creditsQuantizer = value; }
        }
        /// <summary>
        /// returns the available FourCCs for the codec
        /// </summary> 
        public string[] FourCCs {
            get { return fourCCs; }
            set { fourCCs = value; }
        }
        /// <summary>
        /// gets / sets the logfile
        /// </summary>
        public string Logfile {
            get { return logfile; }
            set { logfile = value; }
        }
        /// <summary>
        /// gets / sets Video Tracks Name (used with the muxers)
        /// </summary>
        public string VideoName {
            get { return videoName; }
            set { videoName = value; }
        }
        /// <summary>
        /// gets / set custom commandline options for the encoder
        /// </summary>
        public string CustomEncoderOptions {
            get { return customEncoderOptions; }
            set { customEncoderOptions = value; }
        }
        /// <summary>
        /// gets / sets which fourcc from the FourCCs array is to be used
        /// </summary>
        public int FourCC {
            get { return fourCC; }
            set { fourCC = value; }
        }
        /// <summary>
        ///  gets / sets the maximum number of passes that can be performed with the current codec
        /// </summary>
        public int MaxNumberOfPasses {
            get { return maxNumberOfPasses; }
            set { maxNumberOfPasses = value; }
        }
        /// <summary>
        /// gets / sets the number of encoder threads to be used
        /// </summary>
        public int NbThreads {
            get { return nbThreads; }
            set { nbThreads = value; }
        }

        public VideoCodecSetting Clone() {
            // This method is sutable for all known descendants!
            return this.MemberwiseClone() as VideoCodecSetting;
        }

        #region GenericSettings Members
        public virtual void FixFileNames(System.Collections.Generic.Dictionary<string, string> _) { }

        public virtual string[] RequiredFiles {
            get { return new string[0]; }
        }

        #endregion
    }
}
