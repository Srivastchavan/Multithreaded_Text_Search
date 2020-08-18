/*Written by Srivastchavan Rengarajan for CS6326.001 Multithreaded Text Search Assignment, starting April 10
Net ID: sxr190067

This is Form class that implements all the necessary methods for multithreaded text search.
It contains Text boxes for filepath and search string, buttons for browse, search/cancel, clear, list view for displaying results, 
readonly textboxes for displaying total time taken and total occurences of the search string, 
label for displaying feedback messages and status strip to display progress bar.

The Search button text is changed to “Cancel” once the search starts, and will cancel the operation when pressed.

The ListView control displays the entire line on which the text was found and the line number within the document.  

The search used is case insensitive search and each line of text is displayed on screen as it is read.

Special features added are progress bar to display the progress of the text search, total time taken , number of occurences of search string and message label with necessary feedback.

 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sxr190067Asg4
{
    // Form class with necessary methods to invode multithreading text search 
    public partial class Form1 : Form
    {
        int resultCount = 0;  // number of lines with searched text 
        BackgroundWorker bkgWorker; // background worker object for the invoking threads in the background
        TimeSpan timeTaken;  
        System.Diagnostics.Stopwatch totTimeElapsed = new System.Diagnostics.Stopwatch();  // stopwatch to get total time taken to search
        
        // Form1 constructor
        public Form1()
        {
            InitializeComponent();
            // set the form to center of the screen
            this.CenterToScreen();
            // set default values for all the fields
            resetFields();
            // create new background worker and adding events to it for searching string
            bkgWorker = new BackgroundWorker();
            bkgWorker.DoWork += new DoWorkEventHandler(bkgSearchText);
            bkgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkgCompleteSearch);
            bkgWorker.ProgressChanged += new ProgressChangedEventHandler(bkgProgressChange);
            bkgWorker.WorkerReportsProgress = true;
            bkgWorker.WorkerSupportsCancellation = true;

        }
        // Event Handler to update the progress bar text and value
        private void bkgProgressChange(object sender, ProgressChangedEventArgs e)
        {
            tslblProgress.Text = e.ProgressPercentage.ToString() + "%";
            tslblProgressBar.Value = e.ProgressPercentage;
        }
        // Event handler called when the text search is fully completed
        private void bkgCompleteSearch(object sender, RunWorkerCompletedEventArgs e)
        {
            // if error occurs display error message
            if (e.Error != null)
            {
                tslblMode.Text = "Error Occurred.";
                tslblMode.ForeColor = Color.Red;
                lblError.Visible = true;
                btnClear.Enabled = true;
                lblError.Text = "The text search was errored out. Error: "+e.Error.Message+" .Please click on Clear button on the right to reset all Fields.";
                txtTotalTime.Text = timeTaken.ToString(@"hh\:mm\:ss\:ff");
                
            }
            //if operation is cancelled display cancel message
            else if (e.Cancelled)
            {
                tslblMode.Text = "Search Cancelled.";
                tslblMode.ForeColor = Color.Blue;

                lblError.Visible = true;
                lblError.Text = "The text search was cancelled. The text ' " + txtSearchFor.Text + " ' occurs " + txtTotalLines.Text + " times till now. Please click on Clear button on the right to reset all Fields.";
                txtTotalTime.Text = timeTaken.ToString(@"hh\:mm\:ss\:ff");
                btnClear.Enabled = true;
            }
            // display result message
            else
            {
                totTimeElapsed.Stop();
                timeTaken = totTimeElapsed.Elapsed;
                if (resultCount == 0)
                {
                    txtTotalLines.Text = resultCount.ToString();
                    tslblMode.Text = "Requested text not found in the file. Search completed successfully.";
                }
                else
                {
                    tslblMode.Text = "Search completed successfully.";
                }
                lblError.Visible = true;
                lblError.Text = "The text ' " + txtSearchFor.Text + " ' occurs " + txtTotalLines.Text + " times in the selected file. Please click on Clear button on the right to reset all Fields.";
                tslblMode.ForeColor = Color.Green;
                tslblProgress.Text = "100%";
                tslblProgressBar.Value = 100;
                txtTotalTime.Text = timeTaken.ToString(@"hh\:mm\:ss\:ff");
                btnSearch.Text = "Search";
                resultCount = 0;
                totTimeElapsed.Reset();
                btnClear.Enabled = true;
                timeTaken = TimeSpan.Zero;
            }
        }
        // Event handler to search the text string in the file
        // Calls FileIO to read the file line by line
        private void bkgSearchText(object sender, DoWorkEventArgs e)
        {
            
            //Clear List View items
            if (lvResult.Items.Count > 0)
            {
                this.Invoke((MethodInvoker)(() => lvResult.Items.Clear()));
            }
            // file IO class is used to call search file method 
            FileIO fIO = new FileIO();
            fIO.searchFile(this, e, bkgWorker, resultCount);
            resultCount = Int32.Parse(txtTotalLines.Text);
           
        }

        
        //Calculate progress bar completed percentage according to number of lines searched in the file
        public int calculateProgress(int lineLength, int lineNo, int len)
        {
            int percComplete;
            percComplete = ((lineNo * 4834) / len);         
            Console.WriteLine("Inside :" + lineNo + " " + len + " " + percComplete);
            return percComplete;
        }

        // browse button click event handler which calls FileIO class
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileIO fIO = new FileIO();
            fIO.browse(this);
            
        }
        // search/cancel button click event handler which in turn calls background worker threads
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(btnSearch.Text=="Cancel" && bkgWorker.IsBusy)
            {
                bkgWorker.CancelAsync();
                btnSearch.Text = "Search";
                totTimeElapsed.Stop();
                timeTaken = totTimeElapsed.Elapsed;
                txtTotalTime.Text = timeTaken.ToString();
            }
            else
            {
                btnSearch.Text = "Cancel";
                totTimeElapsed.Reset();
                timeTaken = TimeSpan.Zero;
                totTimeElapsed.Start();
                tslblMode.ForeColor = Color.Black;
                tslblMode.Text = "Searching...";
                tslblProgress.Text = "0%";

                tslblProgressBar.Maximum = 100;
                tslblProgressBar.Step = 1;
                tslblProgressBar.Value = 0;

                this.txtTotalLines.Text = "0";
                this.txtTotalTime.Text = "0";

                bkgWorker.RunWorkerAsync();


            }
        }
        // enable search button when search for text box is not empty
        private void txtSearchFor_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchFor.Text))
            {
                btnSearch.Enabled = true;
            }
        }
        // set default values for all fields
        public void resetFields()
        {
            this.txtFileName.Text = null;
            this.txtSearchFor.Text = null;
            this.lvResult.Items.Clear();
            this.btnSearch.Enabled = false;
            this.txtTotalLines.Text = "0";
            this.txtTotalTime.Text = "0";
            this.tslblMode.ForeColor = Color.Black;
            this.tslblMode.Text = "Search";
            this.tslblProgress.Text = "0%";
            this.tslblProgressBar.Value = 0;
            this.lblError.Visible = false;
            this.btnClear.Enabled = false;
        }

        // reset all fields on click of clear button
        private void btnClear_Click(object sender, EventArgs e)
        {
            resetFields();
        }
        // enable search button when search for text box is not empty
        private void txtSearchFor_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchFor.Text))
            {
                btnSearch.Enabled = true;
            }
        }

       
    }   
}
