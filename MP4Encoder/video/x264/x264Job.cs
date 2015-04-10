using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
   public  class x264Job : IJob
    {
        private x264Setting _setting;
        public x264Job( x264Setting setting) {
            this._setting = setting;
        }

        public  string EncoderFileFullName {
            get { return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "x264/x264.exe"); }
        }

        public  string GetCommandLine() {
            int qp;
            StringBuilder sb = new StringBuilder();
            CultureInfo ci = new CultureInfo("en-us");
            if (_setting.EncodingMode == 4 || _setting.EncodingMode == 7)
                _setting.Turbo = false; // turn off turbo to prevent inconsistent commandline preview
            switch (_setting.EncodingMode) {
                case 0: // ABR
                    sb.Append("--bitrate " + _setting.BitrateQuantizer + " ");
                    break;
                case 1: // CQ
                    if (_setting.Lossless)
                        sb.Append("--qp 0 ");
                    else {
                        qp = (int)_setting.QuantizerCRF;
                        sb.Append("--qp " + qp.ToString(ci) + " ");
                    }
                    break;
                case 2: // 2 pass first pass
                    sb.Append("--pass 1 --bitrate " + _setting.BitrateQuantizer + " --stats " + "\"" + _setting.Logfile + "\" ");
                    break;
                case 3: // 2 pass second pass
                case 4: // automated twopass
                    sb.Append("--pass 2 --bitrate " + _setting.BitrateQuantizer + " --stats " + "\"" + _setting.Logfile + "\" ");
                    break;
                case 5: // 3 pass first pass
                    sb.Append("--pass 1 --bitrate " + _setting.BitrateQuantizer + " --stats " + "\"" + _setting.Logfile + "\" ");
                    break;
                case 6: // 3 pass 2nd pass
                    sb.Append("--pass 3 --bitrate " + _setting.BitrateQuantizer + " --stats " + "\"" + _setting.Logfile + "\" ");
                    break;
                case 7: // 3 pass 3rd pass
                case 8: // automated threepass, show third pass options
                    sb.Append("--pass 3 --bitrate " + _setting.BitrateQuantizer + " --stats " + "\"" + _setting.Logfile + "\" ");
                    break;
                case 9: // constant quality
                    sb.Append("--crf " + _setting.QuantizerCRF.ToString(ci) + " ");
                    break;
            } // now add the rest of the x264 encoder options

            //// AVC Level
            //if (_setting.Level != 15) // unrestricted (x264.exe default)
            //    sb.Append("--level " + AVCLevels.getCLILevelNames()[_setting.Level] + " ");
            if (_setting.KeyframeInterval != 250) // gop size of 250 is default
                sb.Append("--keyint " + _setting.KeyframeInterval + " ");
            if (_setting.MinGOPSize != 25)
                sb.Append("--min-keyint " + _setting.MinGOPSize + " ");
            if (_setting.Turbo) {
                _setting.NbRefFrames = 1;
                _setting.SubPelRefinement = 1; // Q-Pel 2 iterations
                _setting.METype = 0; // diamond search
                _setting.I4x4mv = false;
                _setting.P4x4mv = false;
                _setting.I8x8mv = false;
                _setting.P8x8mv = false;
                _setting.B8x8mv = false;
                _setting.AdaptiveDCT = false;
                _setting.MixedRefs = false;
                _setting.X264Trellis = 0; // disable trellis
                _setting.noFastPSkip = false;
            }
            if (_setting.DeadZoneInter != 21)
                sb.Append("--deadzone-inter " + _setting.DeadZoneInter + " ");
            if (_setting.DeadZoneIntra != 11)
                sb.Append("--deadzone-intra " + _setting.DeadZoneIntra + " ");
            if (_setting.NbRefFrames != 1) // 1 ref frame is default
                sb.Append("--ref " + _setting.NbRefFrames + " ");
            if (_setting.MixedRefs)
                sb.Append("--mixed-refs ");
            if (_setting.noFastPSkip)
                sb.Append("--no-fast-pskip ");
            if (_setting.NbBframes != 0) // 0 is default value, adaptive and pyramid are conditional on b frames being enabled
            {
                sb.Append("--bframes " + _setting.NbBframes + " ");
                if (_setting.NewAdaptiveBFrames != 1)
                    sb.Append("--b-adapt " + _setting.NewAdaptiveBFrames + " ");
                if (_setting.NbBframes > 1 && _setting.BFramePyramid) // pyramid needs a minimum of 2 b frames
                    sb.Append("--b-pyramid ");
                if (_setting.WeightedBPrediction)
                    sb.Append("--weightb ");
                if (_setting.BframePredictionMode != 1) {
                    sb.Append("--direct ");
                    if (_setting.BframePredictionMode == 0)
                        sb.Append("none ");
                    else if (_setting.BframePredictionMode == 2)
                        sb.Append("temporal ");
                    else if (_setting.BframePredictionMode == 3)
                        sb.Append("auto ");
                }
            }
            if (_setting.Deblock) // deblocker active, add options
            {
                if (_setting.AlphaDeblock != 0 || _setting.BetaDeblock != 0) // 0 is default value
                    sb.Append("--deblock " + _setting.AlphaDeblock + ":" + _setting.BetaDeblock + " ");
            } else // no deblocking
                sb.Append("--nf ");
            if (!_setting.Cabac) // no cabac
                sb.Append("--no-cabac ");
            if (_setting.SubPelRefinement + 1 != 6) // non default subpel refinement
            {
                int subq = _setting.SubPelRefinement + 1;
                sb.Append("--subme " + subq + " ");
            }
            if (!_setting.ChromaME)
                sb.Append("--no-chroma-me ");
            if (_setting.X264Trellis > 0)
                sb.Append("--trellis " + _setting.X264Trellis + " ");
            if ((_setting.PsyRDO != new decimal(1.0) || _setting.PsyTrellis != new decimal(0.0)) && _setting.SubPelRefinement + 1 > 5)
                sb.Append("--psy-rd " + _setting.PsyRDO.ToString(ci) + ":" + _setting.PsyTrellis.ToString(ci) + " ");
            // now it's time for the macroblock types
            if (_setting.P8x8mv || _setting.B8x8mv || _setting.I4x4mv || _setting.I8x8mv || _setting.P4x4mv || _setting.AdaptiveDCT) {
                sb.Append("--partitions ");
                if (_setting.I4x4mv && _setting.P4x4mv && _setting.I8x8mv && _setting.P8x8mv && _setting.B8x8mv)
                    sb.Append("all ");
                else {
                    if (_setting.P8x8mv) // default is checked
                        sb.Append("p8x8,");
                    if (_setting.B8x8mv) // default is checked
                        sb.Append("b8x8,");
                    if (_setting.I4x4mv) // default is checked
                        sb.Append("i4x4,");
                    if (_setting.P4x4mv) // default is unchecked
                        sb.Append("p4x4,");
                    if (_setting.I8x8mv) // default is checked
                        sb.Append("i8x8");
                    if (sb.ToString().EndsWith(","))
                        sb.Remove(sb.Length - 1, 1);
                }
                if (_setting.AdaptiveDCT) // default is unchecked
                    sb.Append(" --8x8dct ");
                if (!sb.ToString().EndsWith(" "))
                    sb.Append(" ");
            } else {
                sb.Append("--partitions none ");
            }
            if (_setting.EncodingMode != 1) // doesn't apply to CQ mode
            {
                if (_setting.MinQuantizer != 10) // default min quantizer is 10
                    sb.Append("--qpmin " + _setting.MinQuantizer + " ");
                if (_setting.MaxQuantizer != 51) // 51 is the default max quanitzer
                    sb.Append("--qpmax " + _setting.MaxQuantizer + " ");
                if (_setting.MaxQuantDelta != 4) // 4 is the default value
                    sb.Append("--qpstep " + _setting.MaxQuantDelta + " ");
                if (_setting.IPFactor != new decimal(1.4)) // 1.4 is the default value
                    sb.Append("--ipratio " + _setting.IPFactor.ToString(ci) + " ");
                if (_setting.PBFactor != new decimal(1.3)) // 1.3 is the default value here
                    sb.Append("--pbratio " + _setting.PBFactor.ToString(ci) + " ");
                if (_setting.ChromaQPOffset != new decimal(0.0))
                    sb.Append("--chroma-qp-offset " + _setting.ChromaQPOffset.ToString(ci) + " ");
                if (_setting.VBVBufferSize > 0)
                    sb.Append("--vbv-bufsize " + _setting.VBVBufferSize + " ");
                if (_setting.VBVMaxBitrate > 0)
                    sb.Append("--vbv-maxrate " + _setting.VBVMaxBitrate + " ");
                if (_setting.VBVInitialBuffer != new decimal(0.9))
                    sb.Append("--vbv-init " + _setting.VBVInitialBuffer.ToString(ci) + " ");
                if (_setting.BitrateVariance != 1)
                    sb.Append("--ratetol " + _setting.BitrateVariance.ToString(ci) + " ");
                if (_setting.QuantCompression != new decimal(0.6))
                    sb.Append("--qcomp " + _setting.QuantCompression.ToString(ci) + " ");
                if (_setting.EncodingMode > 1) // applies only to twopass
                {
                    if (_setting.TempComplexityBlur != 20)
                        sb.Append("--cplxblur " + _setting.TempComplexityBlur.ToString(ci) + " ");
                    if (_setting.TempQuanBlurCC != new decimal(0.5))
                        sb.Append("--qblur " + _setting.TempQuanBlurCC.ToString(ci) + " ");
                }
            }
            if (_setting.SCDSensitivity != new decimal(40))
                sb.Append("--scenecut " + _setting.SCDSensitivity.ToString(ci) + " ");
            if (_setting.BframeBias != new decimal(0))
                sb.Append("--b-bias " + _setting.BframeBias.ToString(ci) + " ");
            if (_setting.METype + 1 != 2) {
                sb.Append("--me ");
                if (_setting.METype + 1 == 1)
                    sb.Append("dia ");
                if (_setting.METype + 1 == 3)
                    sb.Append("umh ");
                if (_setting.METype + 1 == 4)
                    sb.Append("esa ");
                if (_setting.METype + 1 == 5)
                    sb.Append("tesa ");
            }
            if (_setting.MERange != 16)
                sb.Append("--merange " + _setting.MERange + " ");
            if (_setting.NbThreads > 1)
                sb.Append("--threads " + _setting.NbThreads + " ");
            if (_setting.NbThreads == 0)
                sb.Append("--threads auto ");
            sb.Append("--thread-input ");

            if (_setting.AQmode == 0) {
                sb.Append("--aq-mode 0 ");
            }
            if (_setting.AQmode > 0) {
                if (_setting.AQstrength != new decimal(1.0))
                    sb.Append("--aq-strength " + _setting.AQstrength.ToString(ci) + " ");
            }
            //if (_zone != null && _zone.Length > 0 && _setting.CreditsQuantizer >= new decimal(1)) {
            //    sb.Append("--_zone ");
            //    foreach (Zone zone in _zone) {
            //        sb.Append(zone.startFrame + "," + zone.endFrame + ",");
            //        if (zone.mode == ZONEMODE.QUANTIZER) {
            //            sb.Append("q=");
            //            sb.Append(zone.modifier + "/");
            //        }
            //        if (zone.mode == ZONEMODE.WEIGHT) {
            //            sb.Append("b=");
            //            double mod = (double)zone.modifier / 100.0;
            //            sb.Append(mod.ToString(ci) + "/");
            //        }
            //    }
            //    sb.Remove(sb.Length - 1, 1);
            //    sb.Append(" ");
            //}

            if (_setting.QuantizerMatrixType > 0) // custom matrices enabled
            {
                if (_setting.QuantizerMatrixType == 1)
                    sb.Append("--cqm \"jvt\" ");
                if (_setting.QuantizerMatrixType == 2)
                    sb.Append("--cqmfile \"" + _setting.QuantizerMatrix + "\" ");
            }
            // sb.Append("--progress "); // ensure that the progress is shown
            if (_setting.NoDCTDecimate)
                sb.Append("--no-dct-decimate ");
            //if (!_setting.PSNRCalculation)
            //    sb.Append("--no-psnr ");
            //if (!_setting.SSIMCalculation)
            //    sb.Append("--no-ssim ");
            if (_setting.EncodeInterlaced)
                sb.Append("--interlaced ");
            if (_setting.NoiseReduction > 0)
                sb.Append("--nr " + _setting.NoiseReduction + " ");
            //add the rest of the mencoder commandline regarding the output
            if (_setting.EncodingMode == 2 || _setting.EncodingMode == 5)
                sb.Append("--output NUL ");
            else
                sb.Append("--output " + "\"" + _setting.Output + "\" ");
            sb.Append("\"" + _setting.Input + "\" ");
            if (!_setting.CustomEncoderOptions.Equals("")) // add custom encoder options
                sb.Append(_setting.CustomEncoderOptions);
            return sb.ToString();
        }

        public ExecuteProcessDele ExecuteInProcess {
            get { return null; }
        }
    }
}
