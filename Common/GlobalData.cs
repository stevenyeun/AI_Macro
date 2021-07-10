using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Macro
{
    public enum SOME_MODE_STATUS
    {
        IDLE,
        CAPTURE,
        FIND_FOLLOW_TEXT,
        FIND_BOTTOM_MENU,
    }
    static public class GlobalData
    {
        // lock문에 사용될 객체
        
        static public Process SelectedProcess;
        static public bool ExitAllThread = false;
        static public SOME_MODE_STATUS Status = 0;

        static public void SetBitmap(Bitmap bmp)
        {
            lock (lockObject)
            {
                WindowCapture.Enqueue(bmp);
            }
        }
        static public bool GetBitmap(out Bitmap bmp)
        {
            bmp = null;
            lock (lockObject)
            {
                if (WindowCapture.Count() > 0)
                {
                    bmp = WindowCapture.Dequeue();
                    return true;
                }
            }
            return false;
        }
        static private object lockObject = new object();
        static private Queue<Bitmap> WindowCapture = new Queue<Bitmap>();
    }
}
