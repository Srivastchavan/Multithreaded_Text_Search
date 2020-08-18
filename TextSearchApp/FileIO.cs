/*Written by Srivastchavan Rengarajan for CS6326.001 Multithreaded Text Search Assignment, starting April 10
Net ID: sxr190067

This is FileIO class for browsing the filepath and reading file line by line.

 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sxr190067Asg4
{
    // FileIO class with necessary methods for opening file dialog box as well as searching string line by line
    class FileIO
    {
        // browse method is called on click of browse button to open a file dialog box
        public void browse(Form1 form)
        {
            // open file dialog box and show text files for selection
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "SELECT ANY .txt File";
            openFileDialog1.Filter = "Text Files|*.txt";
            openFileDialog1.FilterIndex = 2;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName1 = openFileDialog1.FileName;
                    form.txtFileName.Text = fileName1;

                    form.txtSearchFor.Text = String.Empty;
                    form.tslblMode.Text = String.Empty;
                    form.tslblProgress.Text = 0 + "%";
                    form.txtTotalLines.Text = "0";
                    form.lvResult.Items.Clear();
                    form.txtTotalTime.Text = "0";
                    form.tslblProgressBar.Value = 0;

                }
            }
            catch (Exception ex)
            {
                form.lblError.Visible = true;
                form.lblError.Text = "Error Occurred. Could not read the selected file. Error Message :" + ex.Message;
            }
        }

        // search file method is called to read the selected file line by line using stream reader
        public void searchFile(Form1 form, DoWorkEventArgs e, BackgroundWorker bkgWorker, int resultCount)
        {
            string fileName2 = form.txtFileName.Text;
            string searchText = form.txtSearchFor.Text;
            using (StreamReader strReader1 = File.OpenText(fileName2))
            {
                string textLine;
                int progressPercent = 0;
                int LineNo = 1; // to keep track of line no in the file which is displayed in list view

                int len = (int)new FileInfo(fileName2).Length;
                try
                {// read each line
                    while ((textLine = strReader1.ReadLine()) != null)
                    {
                        // search if each line contains the required string and update listview
                        if (!string.IsNullOrWhiteSpace(textLine) && textLine.ToLower().Contains(searchText.ToLower()))
                        {
                            resultCount++;
                            form.Invoke((MethodInvoker)(() => form.lvResult.Items.Add(new ListViewItem(new[] { LineNo.ToString(), textLine }))));
                            form.Invoke((MethodInvoker)(() => form.txtTotalLines.Text = resultCount.ToString()));
                        }
                        // set cancel to true so that RunWorkerCompletedEventHandler knows that the search is cancelled
                        if (bkgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        // call calculateProgress method to calculate progress and then send it to report progress thread
                        if (textLine.Length != 0 && (LineNo % 100 == 0))
                        {
                            progressPercent = form.calculateProgress(textLine.Length, LineNo, len);
                            bkgWorker.ReportProgress(progressPercent);
                        }
                        Thread.Sleep(1); // pause of 1 millisecond added every time a line is read
                        LineNo++;
                    }
                }
                catch (Exception ex)
                {
                    form.lblError.Visible = true;
                    form.lblError.Text = "Error Occurred. Could not read the selected file. Error Message :" + ex.Message;
                }
            }
        }

    }
}
