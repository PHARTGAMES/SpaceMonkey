using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                SafeCallStringDelegate d = new SafeCallStringDelegate((x) => { textBox.Text = x; });
                textBox.Invoke(d, new object[] { text });
            }
            else
                textBox.Enabled = true;
        }
        public static void SetRichTextBoxThreadSafe(RichTextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                SafeCallStringDelegate d = new SafeCallStringDelegate((x) => { textBox.Text = x; });
                textBox.Invoke(d, new object[] { text });
            }
            else
                textBox.Enabled = true;
        }

        public static void EnableButtonThreadSafe(Button button, bool value)
        {
            if (button.InvokeRequired)
            {
                SafeCallBoolDelegate d = new SafeCallBoolDelegate((x) => { button.Enabled = x; });
                button.Invoke(d, new object[] { value });
            }
            else
                button.Enabled = true;

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

    }
}
