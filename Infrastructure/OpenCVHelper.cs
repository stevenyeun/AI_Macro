﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using Utils;
using Point = OpenCvSharp.Point;
using Rect = Utils.Infrastructure.Rect;

namespace Macro.Infrastructure
{
    public class OpenCVHelper
    {
        public static bool SearchColor(Bitmap source, byte r, byte g, byte b, out System.Windows.Point location, bool isResultDisplay = false)
        {
            bool ret = false;
            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            var sourceMat = BitmapConverter.ToMat(source);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color clr = source.GetPixel(x, y);
                    
                    if(clr.R == r && clr.G == g && clr.B == b)
                    {
                        location = new System.Windows.Point()
                        {
                            X = x,
                            Y = y
                        };
                        ret = true;
                        y = source.Height;
                        x = source.Width;
                    }
                }
            }

            if (isResultDisplay && ret)
            {
                using (var gr = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        gr.DrawEllipse(pen, new Rectangle() { X = (int)location.X, Y = (int)location.Y, Width = 30, Height = 30 });
                    }
                }
            }
            return ret;
        }
        public static bool Search(Bitmap source, Bitmap target, out System.Windows.Point location, int thresh, bool isResultDisplay = false)
        {
            var _sourceMat = BitmapConverter.ToMat(source);
            var sourceMat = _sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);
            var _targetMat = BitmapConverter.ToMat(target);
            var targetMat = _targetMat.CvtColor(ColorConversionCodes.RGB2GRAY);
            if (sourceMat.Cols <= targetMat.Cols || sourceMat.Rows <= targetMat.Rows)
            {
                location = new System.Windows.Point();
                return false;
            }
            //Mat result = new Mat();
            //Cv2.MatchTemplate(sourceMat, targetMat, result, TemplateMatchModes.CCoeffNormed);
            
            var match = sourceMat.MatchTemplate(targetMat, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(match, out _, out double max, out _, out Point maxLoc);

            location = new System.Windows.Point()
            {
                X = maxLoc.X,
                Y = maxLoc.Y
            };

            int prob = Convert.ToInt32(max * 100);
            bool ret = false;

            if (prob > thresh)
            {
                ret = true;
            }

            if (isResultDisplay && ret)
            {
                using (var g = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        g.DrawRectangle(pen, new Rectangle() { X = (int)location.X, Y = (int)location.Y, Width = target.Width, Height = target.Height });
                    }
                }
            }

            return ret;

        }
        public static List<System.Windows.Point> MultipleSearch(Bitmap source, Bitmap target, int similarity, int maxSameRepeatCount, bool isResultDisplay = false)
        {
            var searchAndCopyImage = source.Clone() as Bitmap;

            var sourceMat = BitmapConverter.ToMat(searchAndCopyImage);
            var targetMat = BitmapConverter.ToMat(target);

            if (sourceMat.Cols <= targetMat.Cols || sourceMat.Rows <= targetMat.Rows)
            {
                return new List<System.Windows.Point>();
            }
            List<System.Windows.Point> locations = new List<System.Windows.Point>();
            while(maxSameRepeatCount-- > 0)
            {
                sourceMat = BitmapConverter.ToMat(searchAndCopyImage);
                var match = sourceMat.MatchTemplate(targetMat, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(match, out _, out double max, out _, out Point maxLoc);
                max *= 100;
                if (similarity < max)
                {
                    locations.Add(new System.Windows.Point()
                    {
                        X = maxLoc.X,
                        Y = maxLoc.Y
                    }); 
                    using (var g = Graphics.FromImage(searchAndCopyImage))
                    {
                        using (var brush = new SolidBrush(Color.Black))
                        {
                            var rect = new Rectangle() { X = (int)maxLoc.X, Y = (int)maxLoc.Y, Width = target.Width, Height = target.Height };
                            g.FillRectangle(brush, rect);
                        }
                    }
                    using (var g = Graphics.FromImage(source))
                    {
                        if (isResultDisplay)
                        {
                            using (var pen = new Pen(Color.Red, 2))
                            {
                                g.DrawRectangle(pen, new Rectangle() { X = (int)maxLoc.X, Y = (int)maxLoc.Y, Width = target.Width, Height = target.Height });
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return locations;
        }
        public static Bitmap MakeRoiImage(Bitmap source, Rect rect)
        {
            var sourceMat = BitmapConverter.ToMat(source);
            var roiMat = sourceMat.SubMat(new OpenCvSharp.Rect()
            {
                Left = rect.Left,
                Top = rect.Top,
                Height = rect.Height,
                Width = rect.Width
            });
            var destBitmap = BitmapConverter.ToBitmap(roiMat);
            return destBitmap;
        }

        public static int SearchImagePercentage(Bitmap source, Tuple<double, double ,double> lower, Tuple<double, double, double> upper)
        {
            var sourceMat = BitmapConverter.ToMat(source);
            var colorMat = sourceMat.CvtColor(ColorConversionCodes.RGB2HSV);
            var thresholded = new Mat();
            
            Cv2.InRange(colorMat,
                        new Scalar(lower.Item3, lower.Item1, lower.Item2),
                        new Scalar(upper.Item3, upper.Item1, upper.Item2),
                        thresholded);

            return Cv2.CountNonZero(thresholded) / (source.Width * source.Height);
        }
    }
}
