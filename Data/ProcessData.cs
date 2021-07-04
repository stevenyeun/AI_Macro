using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Macro.Data
{
    public class ProcessData
    {
        public ProcessData(string procName, string windowTitle, Process proc)
        {
            _ProcName = procName;
            _WindowTitle = windowTitle;
            _Proc = proc;
        }
        private string _ProcName;
        public string ProcName
        {
            get { return _ProcName; }
            set { _ProcName = value; }
        }


        private string _WindowTitle;
        public string WindowTitle
        {
            get { return _WindowTitle; }
            set { _WindowTitle = value; }
        }

        private Process _Proc;
        public Process Proc
        {
            get { return _Proc; }
            set { _Proc = value; }
        }
    }
}
