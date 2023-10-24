﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security;

namespace XInputFFB
{
    public class Utils
    {

        private delegate void SafeCallIntDelegate(int value);
        private delegate void SafeCallBoolDelegate(bool value);
        private delegate void SafeCallStringDelegate(string value);

        public static void SetTextBoxThreadSafe(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {

                textBox.BeginInvoke(new Action<string>((s) => { textBox.Text = s; }), text);
            }
            else
            {
                textBox.Text = text;
                textBox.Enabled = true;
            }
        }

        public static void SetLabelThreadSafe(Label label, string text)
        {
            if (label.InvokeRequired)
            {

                label.BeginInvoke(new Action<string>((s) => { label.Text = s; }), text);
            }
            else
            {
                label.Text = text;
                label.Enabled = true;
            }
        }

        public static void AddComboBoxEntryThreadSafe(ComboBox comboBox, object entry)
        {
            if (comboBox.InvokeRequired)
            {
                comboBox.BeginInvoke(new Action<object>((s) => { comboBox.Items.Add(s); }), entry);
            }
            else
                comboBox.Items.Add(entry);
        }


        public static void SetComboBoxSelectedIndexThreadSafe(ComboBox comboBox, int index)
        {
            if (comboBox.InvokeRequired)
            {
                comboBox.BeginInvoke(new Action<int>((s) => { comboBox.SelectedIndex = s; }), index);
            }
            else
                comboBox.SelectedIndex = index;
        }

        public static void SetRichTextBoxThreadSafe(RichTextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                textBox.BeginInvoke(new Action<string>((s) => { textBox.Text = s; }), text);
            }
            else
                textBox.Text = text;
        }

        public static void EnableButtonThreadSafe(Button button, bool value)
        {
            if (button.InvokeRequired)
            {
                button.BeginInvoke(new Action<bool>((s) => { button.Enabled = s; }), value);
            }
            else
                button.Enabled = value;

        }

        public static void SetProgressThreadSafe(ProgressBar progressBar, int progress)
        {
            if (progressBar.InvokeRequired)
            {
                SafeCallIntDelegate d = new SafeCallIntDelegate((x) => { progressBar.Value = x; });
                progressBar.Invoke(d, new object[] { progress });
            }
            else
                progressBar.Value = progress;

        }

        public static int TextBoxSafeParseInt(TextBox textBox, int safeValue)
        {
            int value = safeValue;
            if (int.TryParse(textBox.Text, out value))
            {
                return value;
            }

            return safeValue;
        }

        public static float TextBoxSafeParseFloat(TextBox textBox, float safeValue)
        {
            float value = safeValue;
            if (float.TryParse(textBox.Text, out value))
            {
                return value;
            }

            return safeValue;
        }


        public static float CalculateAngularChange(float sourceA, float targetA)
        {
            sourceA *= (180.0f / (float)Math.PI);
            targetA *= (180.0f / (float)Math.PI);

            float a = targetA - sourceA;
            float sign = Math.Sign(a);

            a = ((Math.Abs(a) + 180) % 360 - 180) * sign;
            
            return a * ((float)Math.PI / 180.0f);
        }


        public static float LoopAngleDeg(float angle, float minMag)
        {

            float absAngle = Math.Abs(angle);

            if (absAngle <= minMag)
            {
                return angle;
            }

            float direction = angle / absAngle;

            //(180.0f * 1) - 135 = 45
            //(180.0f *-1) - -135 = -45
            float loopedAngle = (180.0f * direction) - angle;

            return loopedAngle;
        }

        public static float LoopAngleRad(float angle, float minMag)
        {
            float absAngle = Math.Abs(angle);

            if (absAngle <= minMag)
            {
                return angle;
            }

            float direction = angle / absAngle;

            //(180.0f * 1) - 135 = 45
            //(180.0f *-1) - -135 = -45
            float loopedAngle = ((float)Math.PI * direction) - angle;

            return loopedAngle;
        }

        public static float FlipAngleRad(float angle)
        {
            if (angle > (float)Math.PI)
            {
                angle = (-(float)Math.PI + (angle - (float)Math.PI));
            }

            return angle;
        }


        /// <summary>TimeBeginPeriod(). See the Windows API documentation for details.</summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]

        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        /// <summary>TimeEndPeriod(). See the Windows API documentation for details.</summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]

        public static extern uint TimeEndPeriod(uint uMilliseconds);

    }
}
