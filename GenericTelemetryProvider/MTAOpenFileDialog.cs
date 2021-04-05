using System;
using System.Threading;
using System.Windows.Forms;

namespace GenericTelemetryProvider
{
    class MTAOpenFileDialog
    {
        public EventHandler OnComplete;

        string appendFilename = null;

        public void ShowDialog(string _appendFilename = null)
        {
            appendFilename = _appendFilename;

            try
            {
                Thread t = new Thread(() => GetFile(OnComplete));
                t.IsBackground = true;
                t.TrySetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
            }
            catch (Exception exc)
            {
            }
        }

        private void GetFile(EventHandler OnComplete)
        {

            if (appendFilename != null)
            {
                using (FolderBrowserDialog fileOpen = new FolderBrowserDialog())
                {
                    if (fileOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (OnComplete != null)
                            OnComplete.Invoke(this, new MTAOpenFileDialogEventArgs(fileOpen.SelectedPath + "\\" + appendFilename));
                    }
                }

            }
            else
            {

                using (OpenFileDialog fileOpen = new OpenFileDialog())
                {
                    if (fileOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (OnComplete != null)
                            OnComplete.Invoke(this, new MTAOpenFileDialogEventArgs(fileOpen.FileName));
                    }
                }
            }
        }
    }

    class MTAOpenFileDialogEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public MTAOpenFileDialogEventArgs(string fname)
        {
            FileName = fname;
        }
    }
}
