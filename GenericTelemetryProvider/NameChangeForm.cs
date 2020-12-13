using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericTelemetryProvider
{
    public partial class NameChangeForm : Form
    {

        public Action<string> okCallback;
        public Action cancelCallback;
        string nameText;

        public NameChangeForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okCallback.Invoke(nameTextBox.Text);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelCallback.Invoke();
            Close();
        }

        private void NameChangeForm_Load(object sender, EventArgs e)
        {
            nameTextBox.Text = nameText;
        }

        public void SetNameText(string text)
        {
            nameText = text;
        }
    }
}
