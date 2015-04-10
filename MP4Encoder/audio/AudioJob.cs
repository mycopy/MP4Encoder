using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class AudioJob : IJob
    {
        public AudioCodecSetting Setting;
        public AviSynthClip AviSynth;

        public AudioJob(AudioCodecSetting setting) {
            this.Setting = setting;
        }

        public string EncoderFileFullName {
            get { return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "faac/faac.exe"); }
        }

        public string GetCommandLine() {
            StringBuilder script = new StringBuilder();
            var directShow = this.Setting.DirectShow;
            if (!directShow && Path.GetExtension(this.Setting.Input).ToLower() == ".avs") {
                script.AppendFormat("Import(\"{0}\"){1}", this.Setting.Input, Environment.NewLine);
            } else {
                script.AppendFormat("DirectShowSource(\"{0}\"){1}", this.Setting.Input, Environment.NewLine);
                script.AppendFormat("EnsureVBRMP3Sync(){0}", Environment.NewLine);
            }
            if (this.Setting.ImproveAccuracy || this.Setting.AutoGain)
                script.AppendFormat("ConvertAudioToFloat(){0}", Environment.NewLine);
            if (this.Setting.AutoGain)
                script.AppendFormat("Normalize(){0}", Environment.NewLine);

            script.AppendFormat("ConvertAudioTo16bit(){0}", Environment.NewLine);
            script.AppendLine(@"return last");

            this.AviSynth = new AviSynthClip("Eval", script.ToString(), AviSynthColorspace.RGB24);
            var commandLine = "-b " + this.Setting.Bitrate + " -o \"{0}\" -P -X -R {1} -B {2} -C {3} --mpeg-vers 4 -";
            if (this.Setting.BitrateMode == BitrateMode.VBR) {
                commandLine = "-q " + this.Setting.Quality + " -o \"{0}\" -P -X -R {1} -B {2} -C {3} --mpeg-vers 4 -";
            }

            return string.Format(commandLine, this.Setting.Output, this.AviSynth.AudioSampleRate, this.AviSynth.BitsPerSample, this.AviSynth.ChannelsCount, 0, 0);

        }

        public ExecuteProcessDele ExecuteInProcess {
            get {
                return new ExecuteProcessDele((_encoderProcess) => {
                    using (Stream target = _encoderProcess.StandardInput.BaseStream) {
                        ManualResetEvent _mre = new System.Threading.ManualResetEvent(true); // lock used to pause encoding
                        bool hasStartedEncoding = false;
                        const int MAX_SAMPLES_PER_ONCE = 4096;
                        int frameSample = 0;
                        int lastUpdateSample = 0;
                        int frameBufferTotalSize = MAX_SAMPLES_PER_ONCE * this.AviSynth.ChannelsCount * this.AviSynth.BytesPerSample;
                        byte[] frameBuffer = new byte[frameBufferTotalSize];

                        GCHandle h = GCHandle.Alloc(frameBuffer, GCHandleType.Pinned);
                        IntPtr address = h.AddrOfPinnedObject();
                        try {
                            while (frameSample < this.AviSynth.SamplesCount) {
                                _mre.WaitOne();

                                if (_encoderProcess != null)
                                    if (_encoderProcess.HasExited)
                                        throw new ApplicationException("Abnormal encoder termination " + _encoderProcess.ExitCode.ToString());
                                int nHowMany = Math.Min((int)(this.AviSynth.SamplesCount - frameSample), MAX_SAMPLES_PER_ONCE);
                                this.AviSynth.ReadAudio(address, frameSample, nHowMany);

                                _mre.WaitOne();
                                if (!hasStartedEncoding) {
                                    hasStartedEncoding = true;
                                }


                                target.Write(frameBuffer, 0, nHowMany * this.AviSynth.ChannelsCount * this.AviSynth.BytesPerSample);
                                target.Flush();
                                frameSample += nHowMany;
                                if (frameSample - lastUpdateSample > 0) {
                                    lastUpdateSample = frameSample;
                                }
                                Thread.Sleep(0);
                            }
                        } finally {
                            h.Free();
                        }

                    }
                });
            }
        }
    }
}
