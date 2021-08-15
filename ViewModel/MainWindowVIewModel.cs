
using AI_Macro;
using AI_Macro.Data;
using MyDelegateCommand;
using MyViewModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Input;



public class MainWindowVIewModel : ViewModelBase
{

    public MainWindowVIewModel(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }
    private MainWindow mainWindow;
    //ComboBox Items
    private ObservableCollection<ProcessData> _ProcDataList;
    public ObservableCollection<ProcessData> ProcDataList
    {
        get { return _ProcDataList; }
        set
        {
            _ProcDataList = value;
            OnPropertyChanged("ProcDataList");
        }
    }

    //Selected Item
    private ProcessData _SelectedProcData;
    public ProcessData SelectedProcData
    {
        get { return _SelectedProcData; }
        set
        {

            _SelectedProcData = value;
            if (value != null)
            {
                GlobalData.SelectedProcess = value.Proc;
                if(this.mainWindow.CaptureProcScr(out Bitmap bmp))
                {
                    GlobalData.SetDrawBitmap(bmp);
                }
                OnPropertyChanged("SelectedProcData");
            }
        }
    }

    //Log
    private string _LogText;
    public string LogText
    {
        get { return _LogText; }
        set
        {
            string now = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
            _LogText += now + value;
            _LogText += "\n";
            if (value != null)
            {
                OnPropertyChanged("LogText");
            }
        }
    }


    private int _MinWaitTime;
    public int MinWaitTime
    {
        get { return _MinWaitTime; }
        set
        {
            _MinWaitTime = value;
            OnPropertyChanged("MinWaitTime");
        }
    }
    private int _MaxWaitTime;
    public int MaxWaitTime
    {
        get { return _MaxWaitTime; }
        set
        {
            _MaxWaitTime = value;
            OnPropertyChanged("MaxWaitTime");
        }
    }

    private double _CurWaitSec;
    public double CurWaitSec//sec
    {
        get
        {
            return _CurWaitSec;
        }
        set
        {
            //_CurWaitTime = value;
            //sec to min
            _CurWaitSec = value;
            int min = (int)value / 60;
            int sec = (int)value % 60;
            CurWaitTimeFormat = min.ToString() + "m" + sec.ToString() + "s";
        }
    }
    private string _CurWaitTimeFormat= "0m0s";//mm:ss
    public string CurWaitTimeFormat//mm:ss
    {
        get
        {
            return _CurWaitTimeFormat;
        }
        set
        {
            _CurWaitTimeFormat = value;
            OnPropertyChanged("CurWaitTimeFormat");
        }
    }


    private int _SetWaitSec;
    public int SetWaitSec//sec
    {
        get
        {
            return _SetWaitSec;
        }
        set
        {
            //_SetWaitTime = value;
            //sec to min
            _SetWaitSec = value;
            int min = value / 60;
            int sec = value % 60;
            SetWaitTimeFormat = min.ToString() + "m" + sec.ToString() + "s";
        }
    }
    private string _SetWaitTimeFormat="0m0s";//mm:ss
    public string SetWaitTimeFormat//mm:ss
    {
        get
        {
            return _SetWaitTimeFormat;
        }
        set
        {
            _SetWaitTimeFormat = value;
            OnPropertyChanged("SetWaitTimeFormat");
        }
    }
    //private string _ChInfo;
    //public string ChInfo
    //{
    //    get
    //    {
    //        return _ChInfo;
    //    }
    //    set
    //    {
    //        _ChInfo = value;
    //        OnPropertyChanged("ChInfo");
    //    }
    //}

    //private string _Objects;
    //public string Objects
    //{
    //    get
    //    {
    //        return _Objects;
    //    }
    //    set
    //    {
    //        _Objects = value;
    //        OnPropertyChanged("Objects");
    //    }
    //}

    //private string _Time;
    //public string Time
    //{
    //    get
    //    {
    //        return _Time;
    //    }
    //    set
    //    {
    //        _Time = value;
    //        OnPropertyChanged("Time");
    //    }
    //}

    private ICommand _DropDownOpenedCommand;
    public ICommand DropDownOpenedCommand
    {
        get
        {
            return (this._DropDownOpenedCommand) ?? (this._DropDownOpenedCommand = new DelegateCommand(DropDownOpened));
        }
    }
    private void DropDownOpened()
    {
        Console.WriteLine("DropDownOpened()");
        new Thread(() =>
        {
            ProcessData[] arr = Process.GetProcesses().Where(r => r.MainWindowHandle != IntPtr.Zero)
                                                .Select(r => new ProcessData(r.ProcessName, r.MainWindowTitle, r))
                                                .OrderBy(r => r.ProcName).ToArray();

            this.ProcDataList = new ObservableCollection<ProcessData>(arr);

        }).Start();
    }

    private ICommand _StartCommand;
    public ICommand StartCommand
    {
        get
        {
            return (this._StartCommand) ?? (this._StartCommand = new DelegateCommand(Start_Execute));
        }
    }
    private void Start_Execute()
    {
        this.mainWindow.StartSomeMode();
    }

    private ICommand _StopCommand;
    public ICommand StopCommand
    {
        get
        {
            return (this._StopCommand) ?? (this._StopCommand = new DelegateCommand(Stop_Execute));
        }
    }
    private void Stop_Execute()
    {
        this.mainWindow.StopSomeMode();
    }
}