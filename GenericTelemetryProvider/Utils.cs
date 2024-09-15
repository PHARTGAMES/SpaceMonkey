using System;
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

namespace GenericTelemetryProvider
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

        public static Vector3 GetPYRFromQuaternion(Quaternion r)
        {
            float yaw = (float)Math.Atan2(2.0f * (r.Y * r.W + r.X * r.Z), 1.0f - 2.0f * (r.X * r.X + r.Y * r.Y));
            float pitch = (float)Math.Asin(2.0f * (r.X * r.W - r.Y * r.Z));
            float roll = (float)Math.Atan2(2.0f * (r.X * r.Y + r.Z * r.W), 1.0f - 2.0f * (r.X * r.X + r.Z * r.Z));

            return new Vector3(pitch, yaw, roll);
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


        public static IntPtr GetPointerAddress(Process mainProcess, IntPtr baseAddress, Int64[] offsets)
        {
            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            reader.OpenProcess();

            IntPtr curAdd = (IntPtr)ReadIntPtrAddress(reader, baseAddress);
            for (int i = 0; i < offsets.Length - 1; ++i)
            {
                curAdd = (IntPtr)ReadIntPtrAddress(reader, (IntPtr)((Int64)curAdd + offsets[i]));
            }
            curAdd = (IntPtr)((Int64)curAdd + offsets[offsets.Length - 1]);

            reader.CloseHandle();

            return curAdd;
        }

        public static Int64 ReadIntPtrAddress(ProcessMemoryReader reader, IntPtr addr)
        {
            byte[] results = new byte[8];

            Int64 byteReadSize;
            reader.ReadProcessMemory((IntPtr)addr, (ulong)results.Length, out byteReadSize, results);

            return BitConverter.ToInt64(results, 0);
        }


        /// <summary>TimeBeginPeriod(). See the Windows API documentation for details.</summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]

        public static extern uint TimeBeginPeriod(uint uMilliseconds);

        /// <summary>TimeEndPeriod(). See the Windows API documentation for details.</summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]

        public static extern uint TimeEndPeriod(uint uMilliseconds);

        public static void DebugLog(string message)
        {
            using (StreamWriter writer = new StreamWriter("SpaceMonkey.log", true))
            {
                // Write the current date and time along with the log message
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }

        // Cubic smoothing function that mimics half of a bell curve
        public static double CubicSmoothStep(double x)
        {

            return x * x * (3 - 2 * x);
        }


    }
}
