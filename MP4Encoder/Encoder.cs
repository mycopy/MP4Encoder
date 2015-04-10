using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public class Encoder : IEncoder<IJob>
    {
        private IJob _job;
        public InfoOutputDele OnInfoOutput;

        public Encoder(IJob job) {
            this._job = job;
        }

        public Thread Start() {
            Thread _encoderThread;
            _encoderThread = new Thread(CreateEncoderProcess);
            _encoderThread.Priority = ThreadPriority.AboveNormal;
            _encoderThread.Start();
            return _encoderThread;
        }

        public void Stop() {
            throw new NotImplementedException();
        }

        public void Pause() {
            throw new NotImplementedException();
        }

        public void Resume() {
            throw new NotImplementedException();
        }

        private void processOutputInfo(object sendProcess, DataReceivedEventArgs output) {
            if (OnInfoOutput != null)
                OnInfoOutput(sendProcess, output);
        }

        private void CreateEncoderProcess() {
            using (var encoderProcess = new Process()) {
                ProcessStartInfo info = new ProcessStartInfo();
                info.Arguments = _job.GetCommandLine();
                info.FileName = _job.EncoderFileFullName;
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.CreateNoWindow = true;
                encoderProcess.StartInfo = info;
                encoderProcess.Start();
                encoderProcess.BeginErrorReadLine();
                encoderProcess.BeginOutputReadLine();
                encoderProcess.EnableRaisingEvents = true;
                encoderProcess.OutputDataReceived += new DataReceivedEventHandler(processOutputInfo);
                encoderProcess.ErrorDataReceived += new DataReceivedEventHandler(processOutputInfo);

                if (_job.ExecuteInProcess != null) {
                    _job.ExecuteInProcess(encoderProcess);
                }

                encoderProcess.WaitForExit();
                encoderProcess.Close();
                encoderProcess.Dispose();
            }
        }

    }
}
