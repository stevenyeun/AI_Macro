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
        FIND_TOP_PROFILE,
        FIND_FOLLOW_BLUE_TEXT,        
        FIND_PICTURE,
        FIND_MAIN_OBJECT,
        FIND_REPLY,
        CLICK_REPLY,
        INPUT_TEXT,
        FIND_POST,
        CLICK_POST,
        FIND_BACK_BTN,
        CLICK_BACK_BTN,
        FIND_HEART,
        CLICK_HEART,
        CLICK_FOLLOW_BTN,
        SET_WAIT_TIME,
        WAIT_TIME,
        PROC_FAIL,
    }
    static public class GlobalData
    {
        // lock문에 사용될 객체
        
        static public Process SelectedProcess;
        static public bool ExitAllThread = false;
        public static bool ExitSomeThread = false;
        static public SOME_MODE_STATUS Status = 0;

        static public void SetDrawBitmap(Bitmap bmp)
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

                    if(bmp != null)
                        return true;
                }
            }
            return false;
        }
        static private object lockObject = new object();
        static private Queue<Bitmap> WindowCapture = new Queue<Bitmap>();

    }
}
