using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Macro
{
    static public class GlobalData
    {
        static public KeyValuePair<string, Process>[] ProcessList;
        static public KeyValuePair<string, Process>? SelectedProcess;
    }
}
