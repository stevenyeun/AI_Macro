using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public static bool GetBinBmp(Bitmap source, out Bitmap binBmp)
        {
            try
            {
                var _sourceMat = BitmapConverter.ToMat(source);
                var sourceMat = _sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);
                var thresholded = new Mat();
                Cv2.Threshold(sourceMat, thresholded, 0, 255, ThresholdTypes.Binary);

                binBmp = BitmapConverter.ToBitmap(thresholded);
            }
            catch(Exception e)
            {
                binBmp = null;
                return false;
            }
            
            return true;
        }

        //세 점이 주어질 때 사이 각도 구하기
        //http://stackoverflow.com/a/3487062
        public static int GetAngleABC(Point a, Point b, Point c)
        {
            Point ab = new Point( b.X - a.X, b.Y - a.Y );
            Point cb = new Point( b.X - c.X, b.Y - c.Y );
            double dot = (ab.X * cb.X + ab.Y * cb.Y); // dot product
            double cross = (ab.X * cb.Y - ab.Y * cb.X); // cross product
            double alpha = Math.Atan2(cross, dot);
            return (int)Math.Floor(alpha * 180.0 / Cv2.PI + 0.5);
        }
        public static float DistanceToPoint(Point a, Point b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public static bool SearchCircle(Bitmap source, out System.Windows.Point location, int thresh, bool isResultDisplay = false)
        {
            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };

            //to gray
            // to bin
            // to contour
            var oriMat = BitmapConverter.ToMat(source);

            var sourceMat = BitmapConverter.ToMat(source);
            sourceMat = sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();
                     
            Cv2.Threshold(sourceMat, sourceMat, 0, 255, ThresholdTypes.Binary);

            Cv2.FindContours(sourceMat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            //Filter
            //contour를 근사화한다.
            
            List<Point[]> new_contours = new List<Point[]>();

            for (int i = 0; i < contours.Length; i++)
            {
                //외곽선 길이를 반환합니다.
                double arcLen = Cv2.ArcLength((contours[i]), true);

                //외곽선을 단순화
                //Point [] approx = Cv2.ApproxPolyDP(contours[i], arcLen * 0.02/*근사정확도*/, true);
                Point[] approx = Cv2.ApproxPolyDP(contours[i], arcLen * 0.02/*근사정확도*/, true);
                //int circleWidth = sourceMat.Width / 12;
                //int circleRadius = (sourceMat.Width / 13) / 2;
                //int circleRadius = (sourceMat.Width / 14) / 2;
                int circleRadius = (sourceMat.Width / 15) / 2;
                int expectedCircleArea = (int)(circleRadius * circleRadius * Math.PI);
                int area = (int)Math.Abs(Cv2.ContourArea(approx));
                //Console.WriteLine(area);
                if (area > expectedCircleArea)  //면적이 일정크기 이상이어야 한다. 
                {
                    int size = approx.Length;

#if false
                    //모든 코너의 각도를 구해서 더한다.
                    //int ang_sum = 0;
                    int[] angles = new int[size];
                    int[] length = new int[size];
                    for (int k = 0; k < size; k++)
                    {
                        int ang = OpenCVHelper.GetAngleABC(approx[k], approx[(k + 1) % size], approx[(k + 2) % size]);
                        //ang_sum += ang;
                        //Console.WriteLine(ang);
                     
                        angles[k] = ang;
                        length[k] = (int)DistanceToPoint(approx[k], approx[(k + 1) % size]);
                    }
                    int ang_avg = (int)angles.Average();
                    int ang_sum = (int)angles.Sum();
                    int angleMin = angles.Min();
                    int angleMax = angles.Max();
                    float angleRatio = (float)angleMax / (float)angleMin;

                    int lengthAvg = (int)length.Average();
                    int lengthMin = length.Min();
                    int lengthMax = length.Max();
                    float lengthRatio = (float)lengthMax / (float)lengthMin;



                    //bool angleCheck = ang_avg * size == ang_sum; 

                    //int ang_threshold = 8;
                    //int ang_sum_min = (180 - ang_threshold) * (size - 2);
                    //int ang_sum_max = (180 + ang_threshold) * (size - 2);

                    //도형을 판정한다.
                    //IsContourConvex : contour에 오목한 부분이 있는지 체크하고 있다면 False 없다면 True를 반환한다. 
                    //bool angleCheck = (ang_sum >= ang_sum_min) && (ang_sum <= ang_sum_max);
#endif
                    if (size == 8 && Cv2.IsContourConvex(approx))
                    {
#if false
                        Console.WriteLine("start");
                        Console.WriteLine(angleRatio);
                        Console.WriteLine(lengthRatio);
                        Console.WriteLine("end");
                        if (angleRatio <= 1.2)
                        {
                            if(lengthRatio <= 2.6)
                            {
                                new_contours.Add(approx);
                            }
                            //Console.WriteLine("start " + angles.Length);
                            //foreach (var angle in angles)
                                //Console.Write(angle + " ");
                            //Console.WriteLine("");
                            //Console.WriteLine("end");
                            //Console.WriteLine(ang_avg + " " + ang_sum);
                           
                            //Console.WriteLine(size);

                           
                        }
#else
                        new_contours.Add(approx);
#endif
                    }

                    ////위 조건에 해당 안되는 경우는 찾아낸 꼭지점 갯수를 표시
                    //else setLabel(img_result, to_string(approx.size()), contours[i]);
                }
            }

            bool ret = false;

            if (new_contours.Count > 0)
            {
                ret = true;                
            }

            if (isResultDisplay && ret)
            {
                //Cv2.DrawContours(oriMat, new_contours, -1, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, null, 1);
                using (var g = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        foreach(var contour in new_contours)
                        {
                            Cv2.MinEnclosingCircle(contour, out Point2f center, out float radius); 
                            g.DrawEllipse(pen, new Rectangle() {
                                X = (int)center.X- (int)radius,
                                Y = (int)center.Y- (int)radius,
                                Width = (int)radius*2, Height = (int)radius*2
                            });
                        }
                        

                    }
                }

                

            }
            //Cv2.ImShow("oriMat", oriMat);
            //Cv2.ImShow("sourceMat", sourceMat);
            //Cv2.WaitKey(1);

            return true;

        }
        public static bool SearchByBinImg(Bitmap source, Bitmap target, out System.Windows.Point location, int thresh, bool isResultDisplay = false)

        {

            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            var _sourceMat = BitmapConverter.ToMat(source);
            var __sourceMat = _sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();
                                                                                 //black = 0, white = 255
            var sourceMat = new Mat();
            Cv2.Threshold(__sourceMat, sourceMat, 0, 255, ThresholdTypes.Binary);
            //var ___sourceMat= new Mat();
            Cv2.FastNlMeansDenoising(sourceMat, sourceMat, 50);

            var _targetMat = BitmapConverter.ToMat(target);
            var __targetMat = _targetMat.CvtColor(ColorConversionCodes.RGB2GRAY);
            var targetMat = new Mat();
            Cv2.Threshold(__targetMat, targetMat, 0, 255, ThresholdTypes.Binary);
            //var ___targetMat = new Mat();
            Cv2.FastNlMeansDenoising(targetMat, targetMat, 50);



            Cv2.ImShow("src", sourceMat);
            Cv2.ImShow("tar", targetMat);
            Cv2.WaitKey(1);
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
                Console.WriteLine(prob);
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

        public static bool SetBlackMask(Bitmap source, int x, int width)
        {
            try
            {
                using (var g = Graphics.FromImage(source))
                {

                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        g.FillRectangle(brush, new Rectangle()
                        {
                            X = x,
                            Y = 0,
                            Width = width,
                            Height = source.Height
                        });

                    }
               
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
