using Macro.Extensions;
using Macro.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utils;
using Utils.Extensions;
using Utils.Infrastructure;
using WindowsInput;
using MouseInput = Macro.Infrastructure.MouseInput;
using Rect = Utils.Infrastructure.Rect;

namespace AI_Macro.Infrastructure.Controller
{
    public class KeyBoardMouseController
    {
        static public bool KeyboardTextTriggerProcess(IntPtr hWnd, string text)
        {
            //Active Window
            NativeHelper.SetForegroundWindow(hWnd);
            Thread.Sleep(100);
            IntPtr foregroundHwnd = NativeHelper.GetForegroundWindow();
            if(foregroundHwnd == hWnd)
            {
                InputSimulator inputSIm = new InputSimulator();
                inputSIm.Keyboard.TextEntry(text);

                return true;
            }
            return false;
        }        

        static public void MouseTriggerProcess(IntPtr hWnd, Point location, MouseTriggerInfo mouseTriggerInfo)
        {   
            Tuple<float, float> factor = CalculateFactor(hWnd, true);

            var rect = new Rect();
            NativeHelper.GetWindowRect(hWnd, ref rect);
            var mousePosition = new Point()
            {
                X = Math.Abs(rect.Left + (mouseTriggerInfo.StartPoint.X + location.X) * -1) * factor.Item1,
                Y = Math.Abs(rect.Top + (mouseTriggerInfo.StartPoint.Y + location.Y) * -1) * factor.Item2
            };

            //var items = NativeHelper.GetChildHandles(hWnd);
            //foreach (var item in items)
            {
                if (mouseTriggerInfo.MouseInfoEventType == MouseEventType.LeftClick)
                {
                    LogHelper.Debug($">>>>LMouse Save Position X : {mouseTriggerInfo.StartPoint.X} Save Position Y : {mouseTriggerInfo.StartPoint.Y} Target X : { location.X } Target Y : { location.Y }");
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonDown, 1, location.ToLParam());
                    Task.Delay(100).Wait();
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonUp, 0, location.ToLParam());
                }
                else if (mouseTriggerInfo.MouseInfoEventType == MouseEventType.RightClick)
                {
                    LogHelper.Debug($">>>>RMouse Save Position X : {mouseTriggerInfo.StartPoint.X} Save Position Y : {mouseTriggerInfo.StartPoint.Y} Target X : { location.X } Target Y : { location.Y }");
                    NativeHelper.PostMessage(hWnd, WindowMessage.RButtonDown, 1, location.ToLParam());
                    Task.Delay(100).Wait();
                }
                else if (mouseTriggerInfo.MouseInfoEventType == MouseEventType.DragLong)
                {
                    Point dragLocation = new Point(20, 500);
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonDown,
                        1, dragLocation.ToLParam());
                    Task.Delay(10).Wait();
                    for (int i = 0; i < 10; ++i)
                    {
                        mousePosition = new Point()
                        {
                            X = dragLocation.X,
                            Y = dragLocation.Y -= 20
                        };
                        NativeHelper.PostMessage(hWnd, WindowMessage.MouseMove, 1, dragLocation.ToLParam());
                        Task.Delay(10).Wait();
                    }
                    mousePosition = new Point()
                    {
                        X = dragLocation.X,
                        Y = dragLocation.Y
                    };
                    NativeHelper.PostMessage(hWnd, WindowMessage.MouseMove, 1, dragLocation.ToLParam());
                    Task.Delay(10).Wait();
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonUp, 0, dragLocation.ToLParam());
                }
                else if (mouseTriggerInfo.MouseInfoEventType == MouseEventType.Drag)
                {

                    //LogHelper.Debug($">>>>Drag Mouse Save Position X : {MouseTriggerInfo.StartPoint.X} Save Position Y : {model.MouseTriggerInfo.StartPoint.Y} Target X : { mousePosition.X } Target Y : { mousePosition.Y }");

                    Point dragLocation = new Point(20, 500);
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonDown, 1, dragLocation.ToLParam());
                    Task.Delay(10).Wait();
                    for (int i = 0; i < 10; ++i)
                    {
                        mousePosition = new Point()
                        {
                            X = dragLocation.X,
                            Y = dragLocation.Y -= 2
                        };
                        NativeHelper.PostMessage(hWnd, WindowMessage.MouseMove, 1, dragLocation.ToLParam());
                        Task.Delay(10).Wait();
                    }
                    mousePosition = new Point()
                    {
                        X = dragLocation.X,
                        Y = dragLocation.Y
                    };
                    NativeHelper.PostMessage(hWnd, WindowMessage.MouseMove, 1, dragLocation.ToLParam());
                    Task.Delay(10).Wait();
                    NativeHelper.PostMessage(hWnd, WindowMessage.LButtonUp, 0, dragLocation.ToLParam());
                    //LogHelper.Debug($">>>>Drag Mouse Save Position X : {model.MouseTriggerInfo.EndPoint.X} Save Position Y : {model.MouseTriggerInfo.EndPoint.Y} Target X : { mousePosition.X } Target Y : { mousePosition.Y }");

                }
                else if (mouseTriggerInfo.MouseInfoEventType == MouseEventType.Wheel)
                {
                    LogHelper.Debug($">>>>Wheel Save Position X : {mouseTriggerInfo.StartPoint.X} Save Position Y : {mouseTriggerInfo.StartPoint.Y} Target X : { location.X } Target Y : { location.Y }");
                    //NativeHelper.PostMessage(hWnd, WindowMessage.LButtonDown, 1, mousePosition.ToLParam());
                    //Task.Delay(100).Wait();
                    //NativeHelper.PostMessage(hWnd, WindowMessage.LButtonUp, 0, mousePosition.ToLParam());
                    //NativeHelper.PostMessage(hWnd, WindowMessage.MouseWheel, ObjectExtensions.MakeWParam((uint)WindowMessage.MKControl, (uint)(model.MouseTriggerInfo.WheelData * -1)), 0);
                    //var hwnd = NativeHelper.FindWindowEx(NativeHelper.FindWindow(null, "Test.txt - 메모장"), IntPtr.Zero, "Edit", null);
                    //var p = new System.Drawing.Point(0, 0);
                    //int directionUp = 1;
                    //int directionDown = -1;

                    // Scrolls [Handle] down by 1/2 wheel rotation with Left Button pressed
                    //IntPtr wParam = MAKEWPARAM(directionDown, .5f, WinMsgMouseKey.MK_LBUTTON);
                    //IntPtr lParam = MAKELPARAM(Cursor.Position.X, Cursor.Position.Y);
                    //NativeHelper.PostMessage(hWnd, WindowMessage.MouseWheel, ObjectExtensions.MakeWParam((int)(-1 * .5f), 0x0001), location.ToLParam());
                    //NativeHelper.PostMessage(item.Item2, WindowMessage.MouseWheel, ObjectExtensions.MakeWParam(0, mouseTriggerInfo.WheelData * ConstHelper.WheelDelta), location.ToLParam());
                }
            }
        }

        static private Tuple<float, float> CalculateFactor(IntPtr hWnd, bool isDynamic)
        {
            var currentPosition = new Rect();
            NativeHelper.GetWindowRect(hWnd, ref currentPosition);
            var factor = NativeHelper.GetSystemDPI();
            var factorX = 1.0F;
            var factorY = 1.0F;
            var positionFactorX = 1.0F;
            var positionFactorY = 1.0F;
            if (isDynamic)
            {
                foreach (var monitor in DisplayHelper.MonitorInfo())
                {
                    if (monitor.Rect.IsContain(currentPosition))
                    {
                        factorX = factor.X * factorX / monitor.Dpi.X;
                        factorY = factor.Y * factorY / monitor.Dpi.Y;

                        //if (model.EventType == EventType.Mouse)
                        //{
                        //    positionFactorX = positionFactorX * monitor.Dpi.X / model.MonitorInfo.Dpi.X;
                        //    positionFactorY = positionFactorY * monitor.Dpi.Y / model.MonitorInfo.Dpi.Y;
                        //}
                        //else
                        //{
                        //    positionFactorX = positionFactorX * factor.X / monitor.Dpi.X;
                         //   positionFactorY = positionFactorY * factor.Y / monitor.Dpi.Y;
                        //}
                        break;
                    }
                }
            }
            return Tuple.Create(factorX, factorY);
        }
    }
}
