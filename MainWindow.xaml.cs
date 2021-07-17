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
        private MainWindowVIewModel viewModel = new MainWindowVIewModel();
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;

            this.Topmost = true;
            this.DataContext = viewModel;

            CaptureProcScr(out Bitmap bmp);
            StartSomeMode();
        }

        private void StartSomeMode()
        {
            new Thread(() =>
            {
                GlobalData.Status = SOME_MODE_STATUS.FIND_FOLLOW_TEXT;
                
                while (!GlobalData.ExitAllThread)
                {
                    this.viewModel.LogText = GlobalData.Status.ToString();
                    try
                    {
                        bool res = CaptureProcScr(out Bitmap bmp);
                        if (res == true)
                        {
                            switch (GlobalData.Status)
                            {
                                case SOME_MODE_STATUS.FIND_FOLLOW_TEXT:
                                    //17 246 230
                                    {
                                        if (OpenCVHelper.SearchColor(bmp, 22, 143, 224, out System.Windows.Point loc, true))
                                        {
                                            /*
                                            MouseTriggerInfo info = new MouseTriggerInfo();
                                            info.MouseInfoEventType = MouseEventType.LeftClick;
                                            KeyBoardMouseController.MouseTriggerProcess(GlobalData.SelectedProcess.MainWindowHandle, loc, info);
                                            */
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_BOTTOM_MENU;
                                        }
                                        else
                                        {
                                            /*
                                            KeyBoardMouseController.KeyboardTextTriggerProcess(
                                                GlobalData.SelectedProcess.MainWindowHandle, "^^;;");
                                                */
                                            //KeyBoardMouseController.KeyboardTriggerProcess(
                                              //  GlobalData.SelectedProcess.MainWindowHandle, "123456789");

                                            //Move Wheel                                            
                                            MouseTriggerInfo info = new MouseTriggerInfo();
                                            info.MouseInfoEventType = MouseEventType.Wheel;
                                            info.WheelData = -1;
                                            KeyBoardMouseController.MouseTriggerProcess(
                                                GlobalData.SelectedProcess.Handle, loc, info);                                                
                                        }

                                    }
                                    break;
                                case SOME_MODE_STATUS.FIND_BOTTOM_MENU:
                                    if(BmpFromFile(out Bitmap findBmp, "BottomMenu"))
                                    {
                                        if( OpenCVHelper.Search(bmp, findBmp, out System.Windows.Point loc, 99, true) )
                                        {
                                            GlobalData.Status = SOME_MODE_STATUS.FIND_PICTURE; 
                                        }

                                    }                                    
                                    break;
                                default:
                                    break;
                            }
                            GlobalData.SetBitmap(bmp);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    Thread.Sleep(1000);
                }
            }
            ).Start();
        }

        private bool BmpFromFile(out Bitmap bmp, string fileName)
        {
            try
            {
                bmp = (Bitmap)Bitmap.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Img\\" + fileName +".jpg", true);
            }
            catch(Exception e)
            {
                bmp = null;
                return false;
            }

            return true;
        }
        
        private bool CaptureProcScr(out Bitmap bmp)
        {
            bool ret = DisplayHelper.ProcessCapture(GlobalData.SelectedProcess, out bmp, true);
            if (ret)
            {
                GlobalData.SetBitmap(new Bitmap(bmp));
                
            }
            else
            {
                Bitmap _bmp = (Bitmap)Bitmap.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Img\\nosignal.jpg", true);
                GlobalData.SetBitmap(new Bitmap(_bmp));
            }
            return ret;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalData.ExitAllThread = true;
        }
    }
}
