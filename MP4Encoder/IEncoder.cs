using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public interface IEncoder<IJob>
    {
        Thread Start();
        void Stop();
        void Pause();
        void Resume();
    }
}
