using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP4Encoder
{
    public delegate void InfoOutputDele(object sendProcess, DataReceivedEventArgs output);
    public delegate void ExecuteProcessDele(Process process);

}
