using MyViewModelBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Utils;

namespace AI_Macro.ViewModel
{   
    public class CaptureViewModel : ViewModelBase
    {
        public CaptureViewModel()
        {            
        }
        private BitmapSource _BitmapSource;
        public BitmapSource BitmapSource
        {            
            get { return _BitmapSource; }
            set
            {
                _BitmapSource = value;
                OnPropertyChanged("BitmapSource");
            }
        }




        //public MainWindowViewModel()
        //{
        //    Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"c:\dump\bulb.png", true);
        //    _bitmapSource = BitmapConversion.BitmapToBitmapSource(bitmap);
        //}

        /*
            byte[] imgb = service_client.getImage();
            //Convert it to BitmapImage
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(imgb);
            image.EndInit();
            //Set the image as Source of the control
            image1.Source = image;
         */

        /*
        while (DisplayHelper.ProcessCapture(processConfigModel.Processes[i], out Bitmap bmp, applciationData.IsDynamic) && count-- > 0)
                    {
                        var targetBmp = model.Image.Resize((int)Math.Truncate(model.Image.Width * factor.Item1.Item1), (int)Math.Truncate(model.Image.Height * factor.Item1.Item1));
                        var similarity = OpenCVHelper.Search(bmp, targetBmp, out Point location, processConfigModel.SearchImageResultDisplay);
                        Lo
                        
        
        
        
        
        
        
        
        
        
        
        gHelper.Debug($"RepeatType[Search : {count}] : >>>> Similarity : {similarity} % max Loc : X : {location.X} Y: {location.Y}");
                        this.baseContentView.CaptureImage(bmp);
                        if (!await TaskHelper.TokenCheckDelayAsync(model.AfterDelay, processConfigModel.Token) || similarity > processConfigModel.Similarity)
                            break;
                        for (int ii = 0; ii < model.SubEventTriggers.Count; ++ii)
                        {
                            await TriggerProcess(model.SubEventTriggers[ii], processConfigModel);
                            if (processConfigModel.Token.IsCancellationRequested)
                                break;
                        }
                        factor = CalculateFactor(processConfigModel.Processes[i].MainWindowHandle, model, applciationData.IsDynamic);
                    }
         */

    }
    public static class BitmapConversion
    {
        public static BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
