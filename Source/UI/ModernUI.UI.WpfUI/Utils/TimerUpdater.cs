using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public class TimerUpdater
    {
        private DispatcherTimer dispatcherTimer;
        private Action updateAction;
        
        public TimerUpdater(TimeSpan interval, Action updateAction)
        {
            this.updateAction = updateAction;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = interval;
            dispatcherTimer.Start();
        }

        public void UpdateInterval(TimeSpan interval)
        {
            dispatcherTimer.Interval = interval;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(updateAction!=null)
            {
                updateAction();
            }
        }
    }
}
