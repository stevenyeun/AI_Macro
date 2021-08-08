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
        public static bool FindOneContourNum(Bitmap source, int x, int y, int width, int height,
            out byte r, out byte g, out byte b,
            out System.Windows.Point location,
            bool isResultDisplay = false)
        {
            r = 0;
            g = 0;
            b = 0;

            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            bool ret = false;

            var sourceMat = BitmapConverter.ToMat(source);
            var partialMat = sourceMat[new OpenCvSharp.Rect(x, y, width, height)];
            //Cv2.ImShow("partialMat", partialMat);
            //Cv2.WaitKey(1);

            var grayMat = partialMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();
            
            /* 0 black 255 white */
            Cv2.Threshold(grayMat, grayMat, 254, 255, ThresholdTypes.Tozero/*black*/);
            //Cv2.ImShow("grayMat", grayMat);
            //Cv2.WaitKey(1);
            Cv2.FindContours(grayMat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            
            List<Point[]> new_contours = new List<Point[]>();
            List<Point> new_contoursCenter = new List<Point>();
            for (int i = 0; i < contours.Length; i++)
            {
                //외곽선 길이를 반환합니다.
                double arcLen = Cv2.ArcLength((contours[i]), true);

                //외곽선을 단순화                
                Point[] approx = Cv2.ApproxPolyDP(contours[i], arcLen * 0.02/*근사정확도*/, true);
                int area = (int)Math.Abs(Cv2.ContourArea(approx));
                
                if (area > 0)
                {

                    var mmt = Cv2.Moments(approx);
                    double cx = mmt.M10 / mmt.M00,
                        cy = mmt.M01 / mmt.M00;

                    #region
                    /*
                    var mmt = Cv2.Moments(approx);
                    double cx = mmt.M10 / mmt.M00,
                        cy = mmt.M01 / mmt.M00;

                   // Console.WriteLine(cx + " " + cy);
                    Cv2.Circle(partialMat, new Point(cx, cy), 1, Scalar.Red, -1, LineTypes.AntiAlias);
                    Cv2.ImShow("after", partialMat);
                    Cv2.WaitKey(1);
                    */
                    #endregion
                    //IsContourConvex : contour에 오목한 부분이 있는지 체크하고 있다면 False 없다면 True를 반환한다. 
                    if (/*Cv2.IsContourConvex(approx) && */approx.Length != 4)
                    {
                        new_contoursCenter.Add(new Point(cx, cy));
                        new_contours.Add(approx);
                    }
                }
            }
            //Cv2.DrawContours(partialMat, contours, -1, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, null, 1);
            //Cv2.ImShow("DrawContours", partialMat);
            //Cv2.WaitKey(1);

            if (new_contours.Count() > 0 && new_contours.Count() <= 2)
            {
                if (new_contours.Count() == 2)
                {
                    var distance = Math.Sqrt(
                        (Math.Pow(new_contoursCenter[0].X - new_contoursCenter[1].X, 2)
                        + Math.Pow(new_contoursCenter[0].Y - new_contoursCenter[1].Y, 2))
                        );

                    if (distance < 2)
                    {
                        new_contoursCenter.RemoveAt(1);
                        new_contours.RemoveAt(1);
                    }
                }
                if(new_contours.Count() == 1)
                {
                    ret = true;

                    //remove
                    Vec3b bgr = partialMat.At<Vec3b>((int)new_contoursCenter[0].Y, (int)new_contoursCenter[0].X);
                    b = bgr[0];
                    g = bgr[1];
                    r = bgr[2];

                    //rel to abs
                    location.X = new_contoursCenter[0].X + x;
                    location.Y = new_contoursCenter[0].Y + y;
                }

            }

            if (isResultDisplay && ret)
            {
                using (var gr = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        //double arcLen = Cv2.ArcLength(contour, true);

                        //외곽선을 단순화
                        //Point[] approx = Cv2.ApproxPolyDP(contour, arcLen * 0.02/*근사정확도*/, true);
                        var _rect = Cv2.BoundingRect(new_contours[0]);
                        var rect = new Rectangle()
                        { X = x + _rect.X, Y = y + _rect.Y, Width = _rect.Width, Height = _rect.Height };

                        gr.DrawRectangle(pen, rect);
                    }
                }
            }
            //Cv2.DrawContours(partialMat, new_contours, -1, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, null, 1);            
            //Cv2.ImShow("after", partialMat);
            //Cv2.WaitKey(1);
    
            return ret;
        }

        public static bool FindBackArrow(Bitmap source, int x, int y, int width, int height,
            out System.Windows.Point location,
            bool isResultDisplay = false)
        { 
            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            bool ret = false;

            var sourceMat = BitmapConverter.ToMat(source);
            var partialMat = sourceMat[new OpenCvSharp.Rect(x, y, width, height)];
            //Cv2.ImShow("partialMat", partialMat);
            //Cv2.WaitKey(1);

            var grayMat = partialMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();

            /* 0 black 255 white */
            Cv2.Threshold(grayMat, grayMat, 254, 255, ThresholdTypes.Tozero/*black*/);
            //Cv2.ImShow("grayMat", grayMat);
            //Cv2.WaitKey(1);
            Cv2.FindContours(grayMat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            List<Point[]> new_contours = new List<Point[]>();
            List<Point> new_contoursCenter = new List<Point>();
            List<int> new_contoursCenterY = new List<int>();
            for (int i = 0; i < contours.Length; i++)
            {
                //외곽선 길이를 반환합니다.
                double arcLen = Cv2.ArcLength((contours[i]), true);

                //외곽선을 단순화                
                Point[] approx = Cv2.ApproxPolyDP(contours[i], arcLen * 0.02/*근사정확도*/, true);
                int area = (int)Math.Abs(Cv2.ContourArea(approx));

                if (area > 0)
                {
                    //var mmt = Cv2.Moments(approx);
                    //double cx = mmt.M10 / mmt.M00,
                    //cy = mmt.M01 / mmt.M00;

                    #region
                  
                    var mmt = Cv2.Moments(approx);
                    double cx = mmt.M10 / mmt.M00,
                        cy = mmt.M01 / mmt.M00;
                                        
                    //Cv2.Circle(partialMat, new Point(cx, cy), 1, Scalar.Red, -1, LineTypes.AntiAlias);
                    //Cv2.ImShow("after", partialMat);
                    //Cv2.WaitKey(1);
                    
                    #endregion

                    //IsContourConvex : contour에 오목한 부분이 있는지 체크하고 있다면 False 없다면 True를 반환한다. 
                    //if (/*Cv2.IsContourConvex(approx) && */approx.Length != 4)
                    {
                        new_contoursCenterY.Add((int)cy);
                        new_contoursCenter.Add(new Point(cx, cy));
                        new_contours.Add(approx);
                    }
                }
            }
            //Cv2.DrawContours(partialMat, new_contours, -1, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, null, 1);
            //Cv2.ImShow("DrawContours", partialMat);
            //Cv2.WaitKey(1);

            Point[] finalContour = null;
            if (new_contours.Count() > 0)
            {
                ret = true;

                int indexLowerMostY = new_contoursCenterY.IndexOf(new_contoursCenterY.Max());
                finalContour = new_contours[indexLowerMostY];

                //rel to abs
                location.X = new_contoursCenter[indexLowerMostY].X + x;
                location.Y = new_contoursCenter[indexLowerMostY].Y + y;
            }

            if (isResultDisplay && ret)
            {
                using (var gr = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        //double arcLen = Cv2.ArcLength(contour, true);

                        //외곽선을 단순화
                        //Point[] approx = Cv2.ApproxPolyDP(contour, arcLen * 0.02/*근사정확도*/, true);
                        var _rect = Cv2.BoundingRect(finalContour);
                        var rect = new Rectangle()
                        { X = x + _rect.X, Y = y + _rect.Y, Width = _rect.Width, Height = _rect.Height };

                        gr.DrawRectangle(pen, rect);
                    }
                }
            }

            //Cv2.DrawContours(partialMat, new_contours, -1, new Scalar(0, 255, 0), 1, LineTypes.AntiAlias, null, 1);            
            //Cv2.ImShow("after", partialMat);
            //Cv2.WaitKey(1);

            return ret;
        }


        public static bool SearchBlueColor(Bitmap source, int x, int y, int width, int height,
            out System.Windows.Point location, bool isResultDisplay = false)
        {
            bool ret = false;
            location = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            var sourceMat = BitmapConverter.ToMat(source);
            sourceMat = sourceMat[new OpenCvSharp.Rect(x, y, width, height)];
            //Cv2.ImShow("sourceMat", sourceMat);
            //Cv2.WaitKey(1);
            sourceMat = sourceMat.CvtColor(ColorConversionCodes.BGR2HSV);
            Mat[] splitMat = Cv2.Split(sourceMat);
            sourceMat = sourceMat.CvtColor(ColorConversionCodes.HSV2BGR);

            Mat mask = new Mat();
        

            Cv2.InRange(splitMat[0], new Scalar(94)/* light blue */, new Scalar(126)/*dark blue*/, mask);
            //Cv2.ImShow("mask", mask);
            //Cv2.WaitKey(1);
            Cv2.FindContours(mask, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            if(contours.Length > 0)
            {
                ret = true;
            }
            
            if (isResultDisplay && ret)
            {
                using (var gr = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        //foreach(var contour in contours)
                        {
                            //double arcLen = Cv2.ArcLength(contour, true);

                            //외곽선을 단순화
                            //Point[] approx = Cv2.ApproxPolyDP(contour, arcLen * 0.02/*근사정확도*/, true);
                            var _rect = Cv2.BoundingRect(contours[0]);
                            var rect = new Rectangle()
                            { X = x + _rect.X, Y = y + _rect.Y, Width = _rect.Width, Height = _rect.Height };

                            gr.DrawRectangle(pen, rect);

                            location.X = rect.X;
                            location.Y = rect.Y;
                        }
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
                Cv2.Threshold(sourceMat, thresholded, 0, 255, ThresholdTypes.BinaryInv);

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
        public static bool SearchTopProfileCircle(Bitmap source, out System.Windows.Point centerLocation, out int radius, bool isResultDisplay = false)
        {
            centerLocation = new System.Windows.Point()
            {
                X = 0,
                Y = 0
            };
            radius = 0;
            //to gray
            // to bin
            // to contour
            //var oriMat = BitmapConverter.ToMat(source);

            var sourceMat = BitmapConverter.ToMat(source);
            sourceMat = sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();

            //Cv2.ImShow("before", sourceMat);
            /* 0 black 255 white */
            
            Cv2.Threshold(sourceMat, sourceMat, 254, 255,ThresholdTypes.Tozero/*black*/);
            //Cv2.ImShow("after", sourceMat);
            //Cv2.WaitKey(1);

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
                int circleRadius = (sourceMat.Width / 17) / 2;
                int expectedCircleArea = (int)(circleRadius * circleRadius * Math.PI);
                int area = (int)Math.Abs(Cv2.ContourArea(approx));
                //Console.WriteLine(area);
                if (area > expectedCircleArea)  //면적이 일정크기 이상이어야 한다. 
                {
                    int size = approx.Length;
                    //Console.WriteLine(size);
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

            if (new_contours.Count == 1)
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
                            Cv2.MinEnclosingCircle(contour, out Point2f center, out float _radius); 
                            g.DrawEllipse(pen, new Rectangle() {
                                X = (int)center.X- (int)_radius,
                                Y = (int)center.Y- (int)_radius,
                                Width = (int)_radius * 2,
                                Height = (int)_radius * 2
                            });

                            centerLocation.X = center.X;
                            centerLocation.Y = center.Y;
                            radius = (int)_radius;
                        }
                    }
                }
            }
           // Cv2.ImShow("oriMat", oriMat);
            
            return ret;

        }

        public static int sideBar = 15;
        public static bool SearchFeedPictureRect(Bitmap source, out Rectangle pictureRect,
            bool isResultDisplay = false)
        {
            pictureRect = new Rectangle()
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0
            };
            var orgMat = BitmapConverter.ToMat(source);
            var sourceMat = BitmapConverter.ToMat(source);

            //사진 위치를 찾기위해 좌우 더미라인 그리기
            Cv2.Line(sourceMat, new Point(0, 0), new Point(0, sourceMat.Height), new Scalar(255, 255, 255), sideBar);
            Cv2.Line(sourceMat, new Point(sourceMat.Width- sideBar, 0), new Point(sourceMat.Width - sideBar, sourceMat.Height), new Scalar(255, 255, 255), sideBar);

            sourceMat = sourceMat.CvtColor(ColorConversionCodes.RGB2GRAY);//  new Mat();
            Cv2.Threshold(sourceMat, sourceMat, 254, 255, ThresholdTypes.Tozero/*black*/);

            //Cv2.ImShow("Threshold", sourceMat);
            //Cv2.WaitKey(1);
            

            Cv2.FindContours(sourceMat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

          
            //contour를 근사화한다.
            List<double> avgColor = new List<double>();
            List<Point[]> filter1Contours = new List<Point[]>();

            for (int i = 0; i < contours.Length; i++)
            {
                //외곽선 길이를 반환합니다.
                double arcLen = Cv2.ArcLength((contours[i]), true);

                //외곽선을 단순화
                Point[] approx = Cv2.ApproxPolyDP(contours[i], arcLen * 0.02/*근사정확도*/, true);
           
                int expectedRectArea = sourceMat.Width * 2;
                int area = (int)Math.Abs(Cv2.ContourArea(approx));
                //Console.WriteLine(area);
                if (area > expectedRectArea)  //면적이 일정크기 이상이어야 한다. 
                {
                    int size = approx.Length;
                    if (size == 4 && Cv2.IsContourConvex(approx))
                    {
                        var rect = Cv2.BoundingRect(approx);
                        avgColor.Add( Cv2.Mean(sourceMat[rect]).Val0 );
                        filter1Contours.Add(approx);
                    }
                }
            }
            bool ret = false;

            Point[] finalContour = null;
            if (filter1Contours.Count > 0)
            {
                //second filter
                List<double> newAvgColor = new List<double>();
                List<Point[]> secondFilterContours = new List<Point[]>();
                List<int> secondFilterTop = new List<int>();
                for (int i=0; i<avgColor.Count(); i++)
                {
                    if(avgColor[i] < avgColor.Average())
                    {
                        newAvgColor.Add(avgColor[i]);
                        secondFilterContours.Add(filter1Contours[i]);

                        var rect = Cv2.BoundingRect(filter1Contours[i]);
                        secondFilterTop.Add(rect.Top);
                    }
                }
                //가장위에있는 rect
                int indexUpperMostY = secondFilterTop.IndexOf( secondFilterTop.Min());
                finalContour = secondFilterContours[indexUpperMostY];

                ret = true;
            }

            if (isResultDisplay && ret)
            {
                using (var g = Graphics.FromImage(source))
                {
                    using (var pen = new Pen(Color.Red, 5))
                    {
                        //Cv2.DrawContours(orgMat, filter1Contours, -1, new Scalar(0, 255, 0), 10, LineTypes.AntiAlias, null, 1);
                        //Cv2.ImShow("DrawContours", orgMat);

                        var rect = Cv2.BoundingRect(finalContour);
                        pictureRect = new Rectangle()
                        {
                            X = rect.X,
                            Y = rect.Y,
                            Width = rect.Width,
                            Height = rect.Height
                        };
                        g.DrawRectangle(pen, pictureRect);
                        //}
                    }
                }
            }
            // Cv2.ImShow("oriMat", oriMat);

            return ret;

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
            Cv2.Threshold(__sourceMat, sourceMat, 0, 255, ThresholdTypes.BinaryInv);
            //var ___sourceMat= new Mat();
            Cv2.FastNlMeansDenoising(sourceMat, sourceMat, 50);

            var _targetMat = BitmapConverter.ToMat(target);
            var __targetMat = _targetMat.CvtColor(ColorConversionCodes.RGB2GRAY);
            var targetMat = new Mat();
            Cv2.Threshold(__targetMat, targetMat, 0, 255, ThresholdTypes.BinaryInv);
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

        public static bool SetBlackMask(Bitmap source, int x, int width, int y, int height)
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

                        g.FillRectangle(brush, new Rectangle()
                        {
                            X = 0,
                            Y = y,
                            Width = source.Width,
                            Height = height
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
