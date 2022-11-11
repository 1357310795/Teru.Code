using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Teru.Code.Services
{
    public class LoopWorker
    {
        #region Task Work
        public TaskState State { get; set; }

        private BackgroundWorker bgw;

        public delegate void OnGoAnimationHandler();
        public event OnGoAnimationHandler OnGoAnimation;

        public delegate TaskState OnGoHandler();
        public event OnGoHandler Go;

        public delegate bool BoolHandler();
        public event BoolHandler CanRun;

        public int Interval { get; set; }

        public void StartRun()
        {
            if (State == TaskState.Started) return;
            if (!CanRun.Invoke())
            {
                //
                return;
            }
            State = TaskState.Started;
            initbgw();
            bgw.RunWorkerAsync();//Todo：优化
            //OnGoAnimation.Invoke();
        }

        public void StopRun()
        {
            State = TaskState.None;
        }

        public void ToggleRun()
        {
            if (State != TaskState.Started)
            {
                StartRun();
            }
            else
            {
                StopRun();
            }
        }

        private void initbgw()
        {
            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            }
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Monitor.Enter(this);
            if ((bool)e.Result)
            {
                bgw.RunWorkerAsync();
            }
            Monitor.Exit(this);
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var res = Go.Invoke();//true继续
            State = res;

            e.Result = true;
            if (State != TaskState.Started)
            {
                e.Result = false;
                return;
            }
            OnGoAnimation.Invoke();
            Thread.Sleep(Interval);
            if (State != TaskState.Started)
            {
                e.Result = false;
                return;
            }
        }
        #endregion
    }

    public enum TaskState
    {
        None, Started, Done, Error//, Waiting
    }
}
