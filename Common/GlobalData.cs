using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Macro
{
    static public class GlobalData
    {
        static public Process SelectedProcess;
        static public bool ExitAllThread = false;
        static public Queue<Bitmap> WindowCapture = new Queue<Bitmap>();
        static public int Status = 0;
    }
}
