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

            StartSomeMode();
        }

        private void StartSomeMode()
        {
            new Thread(() =>
            {
                while (!GlobalData.ExitAllThread)
                {
                    if (GlobalData.SelectedProcess != null)
                    {
                        bool res = CaptureProcScr();

                        switch (GlobalData.Status)
                        {
                            case 1:
                                break;
                            default:
                                break;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            ).Start();
        }

        private static bool CaptureProcScr()
        {
            bool ret = DisplayHelper.ProcessCapture(GlobalData.SelectedProcess, out Bitmap bmp, false);
            if (ret)
            {
                GlobalData.WindowCapture.Enqueue(bmp);
            }
            else
            {
                Bitmap _bmp = (Bitmap)Bitmap.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\nosignal.jpg", true);
                GlobalData.WindowCapture.Enqueue(_bmp);
            }
            return ret;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalData.ExitAllThread = true;
        }
    }
}
