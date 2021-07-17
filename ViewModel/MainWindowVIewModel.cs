
using AI_Macro;
using AI_Macro.Data;
using MyDelegateCommand;
using MyViewModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;



public class MainWindowVIewModel : ViewModelBase
{

    public MainWindowVIewModel()
    {

    }
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



    private ICommand _OkCommand;
    public ICommand OkCommand
    {
        get
        {
            return (this._OkCommand) ?? (this._OkCommand = new DelegateCommand(Ok_Execute));
        }
    }
    private void Ok_Execute()
    {

    }
}