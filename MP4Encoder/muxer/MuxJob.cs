using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class MuxJob : IJob
    {
        private MuxSetting _setting;

        public MuxJob(MuxSetting setting) {
            this._setting = setting;
        }

        public string EncoderFileFullName {
            get { return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "mp4box/mp4box.exe"); }
        }

        public string GetCommandLine() {
            CultureInfo ci = new CultureInfo("en-us");
            StringBuilder sb = new StringBuilder();

            if (_setting.VideoInput.Length > 0) {
                sb.Append("-add \"" + _setting.VideoInput);
                if (_setting.VideoInput.ToLower().EndsWith(".mp4")) {
                    int trackID = 1;
                    sb.Append("#trackID=" + trackID);
                }
                if (_setting.Framerate.HasValue) {
                    string fpsString = _setting.Framerate.Value.ToString(ci);
                    sb.Append(":fps=" + fpsString);
                }
                if (!_setting.VideoName.Equals(""))
                    sb.Append(":name=" + _setting.VideoName);
                sb.Append("\"");
            }
            if (_setting.MuxedInput.Length > 0) {
                sb.Append(" -add \"" + _setting.MuxedInput);
                if (_setting.MuxedInput.ToLower().EndsWith(".mp4")) {
                    int trackID = 1;
                    sb.Append("#trackID=" + trackID);
                }
                if (_setting.Framerate.HasValue) {
                    string fpsString = _setting.Framerate.Value.ToString(ci);
                    sb.Append(":fps=" + fpsString);
                }
                if (!_setting.VideoName.Equals(""))
                    sb.Append(":name=" + _setting.VideoName);
                sb.Append("\"");
            }
            if (_setting.AudioInput.Length > 0) {
                sb.Append(" -add \"" + _setting.AudioInput);
                if (_setting.AudioInput.ToLower().EndsWith(".mp4") || _setting.AudioInput.ToLower().EndsWith(".m4a")) {
                    int trackID = 1;
                    sb.Append("#trackID=" + trackID);
                }
                if (_setting.Framerate.HasValue) {
                    string fpsString = _setting.Framerate.Value.ToString(ci);
                    sb.Append(":fps=" + fpsString);
                }
                if (!_setting.VideoName.Equals(""))
                    sb.Append(":name=" + _setting.VideoName);
                sb.Append("\"");
            }

            //foreach (object o in settings.AudioStreams) {
            //    MuxStream stream = (MuxStream)o;
            //    sb.Append(" -add \"" + stream.path);
            //    if (stream.path.ToLower().EndsWith(".mp4") || stream.path.ToLower().EndsWith(".m4a")) {
            //        int trackID = VideoUtil.getIDFromAudioStream(stream.path, count);
            //        sb.Append("#trackID=" + trackID);
            //    }
            //    if (stream.language != null && !stream.language.Equals(""))
            //        sb.Append(":lang=" + stream.language);
            //    if (stream.name != null && !stream.name.Equals(""))
            //        sb.Append(":name=" + stream.name);
            //    if (stream.delay != 0)
            //        sb.AppendFormat(":delay={0}", stream.delay);
            //    sb.Append("\"");
            //    count++;
            //}
            //foreach (object o in settings.SubtitleStreams) {
            //    MuxStream stream = (MuxStream)o;
            //    sb.Append(" -add \"" + stream.path);
            //    if (!stream.language.Equals(""))
            //        sb.Append(":lang=" + stream.language);
            //    if (stream.name != null && !stream.name.Equals(""))
            //        sb.Append(":name=" + stream.name);
            //    sb.Append("\"");
            //}

            //if (!settings.ChapterFile.Equals("")) // a chapter file is defined
            //    sb.Append(" -chap \"" + settings.ChapterFile + "\"");

            //if (settings.SplitSize.HasValue)
            //    sb.Append(" -splits " + settings.SplitSize.Value.KB);

            // tmp directory
            // due to a bug from MP4Box, we need to test the path delimiter number
            if (CountStrings(_setting.MuxedOutput, '\\') > 1) {
                sb.AppendFormat(" -tmp \"{0}\"", Path.GetDirectoryName(_setting.MuxedOutput));
            } else {
                sb.AppendFormat(" -tmp {0}", Path.GetDirectoryName(_setting.MuxedOutput));
            }

            // force to create a new output file
            sb.Append(" -new \"" + _setting.MuxedOutput + "\"");
            return sb.ToString();
        }

        public ExecuteProcessDele ExecuteInProcess {
            get { return null; }
        }

        private int CountStrings(string src, char find) {
            int ret = 0;
            foreach (char s in src) {
                if (s == find) {
                    ++ret;
                }
            }
            return ret;
        }


    }
}
