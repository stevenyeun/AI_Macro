using AI_Macro.ViewModel;
using System;
using System.Collections.Generic;
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

namespace AI_Macro.View
{
    /// <summary>
    /// CaptureView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CaptureView : UserControl
    {

        private CaptureViewModel viewModel = new CaptureViewModel();
        public CaptureView()
        {
            InitializeComponent();

            this.DataContext = viewModel;

            new Thread(() =>
            {
                while (!GlobalData.ExitAllThread)
                {
                    if (GlobalData.WindowCapture.Count() > 0)
                    {
                        this.Dispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            this.viewModel.BitmapSource = BitmapConversion.BitmapToBitmapSource(GlobalData.WindowCapture.Dequeue());
                        }));
                    }
                    Thread.Sleep(200);
                }
            }
            ).Start();

        }
    }
}
