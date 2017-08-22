using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace ReplaySeeker.Core
{
    public static class Threading
    {
        // Extension method.
        public static void SynchronizedInvoke(this Control control, Action handler)
        {
            // If the invoke is not required, then invoke here and get out.
            if (control.InvokeRequired)
            {
                control.Invoke(handler);
            }
            else
            {
                handler();
            }
        }

        public static int getValueSafe(this TrackBar varControl)
        {
            if (varControl.InvokeRequired)
            {
                return (int)varControl.Invoke(
                  new Func<int>(() => getValueSafe(varControl))
                );
            }
            else
            {
                int varText = varControl.Value;
                return varText;
            }
        }
    }
}
