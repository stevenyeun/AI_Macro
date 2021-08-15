using AI_Macro.Infrastructure.Controller;
using Macro.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utils;

namespace AI_Macro
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVIewModel viewModel = null;
        public MainWindow()
        {
            InitializeComponent();

            MacroIni.Read();

            this.Closing += MainWindow_Closing;

            this.Topmost = true;

            viewModel = new MainWindowVIewModel(this);
            viewModel.MaxWaitTime = MacroIni.MaxWaitTime;
            viewModel.MinWaitTime = MacroIni.MinWaitTime;

            this.DataContext = viewModel;

            CaptureProcScr(out Bitmap bmp);
            GlobalData.SetDrawBitmap(bmp);
            //StartSomeMode();
        }

        //Reply Idea : MonthDay
        public void StartSomeMode()
        {
            new Thread(() =>
            {
                GlobalData.Status = SOME_MODE_STATUS.FIND_TOP_PROFILE;
                //GlobalData.Status = SOME_MODE_STATUS.FIND_POST;
                System.Windows.Point topProfileCenter = new System.Windows.Point();
                int topProfileRadius = 0;
                System.Windows.Point followTextPos = new System.Windows.Point();
                System.Drawing.Rectangle pictureRect = new System.Drawing.Rectangle();
                System.Windows.Point heartPos = new System.Windows.Point();
                System.Windows.Point replyPos = new System.Windows.Point();
                System.Windows.Point postTextPos = new System.Windows.Point();
                System.Windows.Point backBtnPos = new System.Windows.Point();
                Stopwatch stopwatch = new Stopwatch();

                while (!GlobalData.ExitSomeThread)
                {
                    this.viewModel.LogText = GlobalData.Status.ToString();
                    try
                    {
                        bool res = CaptureProcScr(out Bitmap bmp);


                        if (res == true)
                        {
                            switch (GlobalData.Status)
                            {
                                case SOME_MODE_STATUS.IDLE:
                                    {
                                       
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_TOP_PROFILE:
                                    //17 246 230
                                    {
                                        //region
                                        bool mastRet = OpenCVHelper.SetBlackMask(bmp,
                                            bmp.Width / 6, bmp.Width,
                                            bmp.Height / 5, bmp.Height);

                                        bool isSerchCircle = OpenCVHelper.SearchTopProfileCircle(bmp, out topProfileCenter, out topProfileRadius, true);
                                        if (isSerchCircle)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_FOLLOW_BLUE_TEXT;
                                        }
                                        else
                                        {
                                            //Drag
                                            MouseTriggerInfo info = new MouseTriggerInfo();
                                            info.MouseInfoEventType = MouseEventType.Drag;
                                            KeyBoardMouseController.MouseTriggerProcess(
                                                GlobalData.SelectedProcess.MainWindowHandle,
                                                new System.Windows.Point(), info);
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_FOLLOW_BLUE_TEXT:
                                    {
                                        bool isFind = OpenCVHelper.SearchBlueColor(bmp,
                                            (int)topProfileCenter.X + topProfileRadius,
                                            (int)topProfileCenter.Y - topProfileRadius,
                                            bmp.Width / 2,
                                            topProfileRadius * 2,
                                            out followTextPos,
                                            true
                                            );
                                        if (isFind)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_PICTURE;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_PICTURE:
                                    {
                                        //사진위치 파악
                                        bool isFind = OpenCVHelper.SearchFeedPictureRect(bmp, out pictureRect, true);
                                        if (isFind)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_MAIN_OBJECT;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_MAIN_OBJECT:
                                    {
                                        GlobalData.Status = SOME_MODE_STATUS.FIND_REPLY;
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_HEART:
                                    {
                                        bool ret = OpenCVHelper.FindOneContourNum(bmp,
                                            (int)topProfileCenter.X - topProfileRadius - 2, pictureRect.Bottom,
                                            topProfileRadius * 2, topProfileRadius * 2 + 5,
                                            out byte r, out byte g, out byte b,
                                            out heartPos, true);

                                        if (ret == true)
                                        {
                                            if (r == 255 && g == 255 && b == 255)//white
                                            {
                                                GlobalData.Status = SOME_MODE_STATUS.CLICK_HEART;
                                            }
                                            else
                                            {
                                                GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                            }
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.CLICK_HEART:
                                    {
                                        //click
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.LeftClick;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            heartPos, info);

                                        GlobalData.Status = SOME_MODE_STATUS.CLICK_FOLLOW_BTN;
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_REPLY:
                                    {
                                        bool ret = OpenCVHelper.FindOneContourNum(bmp,
                                            (int)topProfileCenter.X + topProfileRadius + 2, pictureRect.Bottom,
                                            topProfileRadius * 2, topProfileRadius * 2 + 2,
                                            out byte r, out byte g, out byte b,
                                            out replyPos,
                                            true);
                                        // Console.WriteLine("contourNum " + contourNum);
                                        if (ret)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.CLICK_REPLY;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.CLICK_REPLY:
                                    {
                                        //click
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.LeftClick;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            replyPos, info);

                                        GlobalData.Status = SOME_MODE_STATUS.INPUT_TEXT;
                                    }
                                    break;
                                case SOME_MODE_STATUS.INPUT_TEXT:
                                    {
                                        bool ret = KeyBoardMouseController.KeyboardTextTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            "맛있겠다~~다이어트 2주차....");
                                        if(ret)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_POST;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_BACK_BTN;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_POST:
                                    {
                                        int maskWidth = (bmp.Width / 6);
                                        int maskHeight = (bmp.Height / 6);

                                        bool isFind = OpenCVHelper.SearchBlueColor(bmp,
                                    bmp.Width - (bmp.Width / 6), bmp.Height - (bmp.Height / 6),
                                    maskWidth, maskHeight,
                                     out postTextPos,
                                    true
                                    );
                                        if (isFind)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.CLICK_POST;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.PROC_FAIL;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.CLICK_POST:
                                    {
                                        //
#if false
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.LeftClick;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            postTextPos, info);
#endif
                                        GlobalData.Status = SOME_MODE_STATUS.FIND_BACK_BTN;
                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_BACK_BTN:
                                    {
                                        int maskWidth = (bmp.Width / 9);
                                        int maskHeight = (bmp.Height / 8);

                                        bool ret = OpenCVHelper.FindBackArrow(bmp,
                                            0, 0, maskWidth, maskHeight,
                                            out backBtnPos,
                                            true);
                                        if(ret)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.CLICK_BACK_BTN;
                                        }
                                        else
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.IDLE;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.CLICK_BACK_BTN:
                                    {
                                        //click
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.LeftClick;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            backBtnPos, info);                                      

                                        GlobalData.Status = SOME_MODE_STATUS.FIND_HEART;
                                    }
                                    break;
                                case SOME_MODE_STATUS.CLICK_FOLLOW_BTN:
                                    {
                                        //click
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.LeftClick;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            followTextPos, info);

                                        //DragLong
                                        info.MouseInfoEventType = MouseEventType.DragLong;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            new System.Windows.Point(0, 0), info);

                                        GlobalData.Status = SOME_MODE_STATUS.SET_WAIT_TIME;
                                    }
                                    break;
                                case SOME_MODE_STATUS.SET_WAIT_TIME:
                                    {
                                        this.viewModel.SetWaitSec = new Random().Next(
                                            this.viewModel.MinWaitTime*60, this.viewModel.MaxWaitTime*60 );
                                        GlobalData.Status = SOME_MODE_STATUS.WAIT_TIME;
                                        stopwatch.Restart();
                                    }
                                    break;
                                case SOME_MODE_STATUS.WAIT_TIME:
                                    {
                                        this.viewModel.CurWaitSec = (int)stopwatch.Elapsed.TotalSeconds;
                                        if(this.viewModel.CurWaitSec >= this.viewModel.SetWaitSec)
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_TOP_PROFILE;
                                        }
                                    }
                                    break;
                                case SOME_MODE_STATUS.PROC_FAIL:
                                    {
                                        MouseTriggerInfo info = new MouseTriggerInfo();
                                        info.MouseInfoEventType = MouseEventType.DragLong;
                                        KeyBoardMouseController.MouseTriggerProcess(
                                            GlobalData.SelectedProcess.MainWindowHandle,
                                            new System.Windows.Point(0, 0), info);

                                        GlobalData.Status = SOME_MODE_STATUS.FIND_TOP_PROFILE;
                                    }
                                    break;


                                default:
                                    break;
                            }
                        }
                        
                        GlobalData.SetDrawBitmap(bmp);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Thread.Sleep(2000);
                }
            }
            ).Start();
        }

        public void StopSomeMode()
        {
            GlobalData.ExitSomeThread = true;
        }

        private bool BmpFromFile(out Bitmap bmp, string fileName)
        {
            try
            {
                bmp = (Bitmap)Bitmap.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Img\\" + fileName + ".jpg", true);
            }
            catch (Exception e)
            {
                bmp = null;
                return false;
            }

            return true;
        }

        public bool CaptureProcScr(out Bitmap bmp)
        {
            bool ret = false;

            if (GlobalData.SelectedProcess != null)
            {
                ret = DisplayHelper.ProcessCapture(GlobalData.SelectedProcess, out bmp, true);
            }
            else
            {
                bmp = (Bitmap)Bitmap.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Img\\nosignal.jpg", true);

            }
            return ret;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MacroIni.WaitPeriod = this.viewModel.WaitPeriod;
            MacroIni.Write();
            GlobalData.ExitAllThread = true;
        }

    }
}
