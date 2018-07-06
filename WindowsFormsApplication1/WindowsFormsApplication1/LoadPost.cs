using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostProcessApplication
{
    public partial class LoadPost : Form
    {
        
        ProcessPost PPWork = new ProcessPost();
        public LoadPost()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            // use dialog box to get file name to load
            openFileDialog1.ShowDialog();
            // if file exists start processing
            if (openFileDialog1.CheckFileExists)
            {
                // load post records into list
                if (PPWork.LoadPost(openFileDialog1.FileName))
                {
                    // if record load was sucessful show controls for next step
                    groupBox1.Visible = true;
                    groupBox2.Visible = true;
                    button2.Visible = true;
                    // hide load button
                    button1.Visible = false;
                    label1.Text = "File Sucessfully Upload";
                }
                else
                {
                    label1.Text = "File Upload Failed";
                } 
            }
            else
            {
                label1.Text = "file Does Not Exist";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // button text will be set to Exit after files are processed. click on it to exit program
            if (button2.Text == "Exit")
            {
                Application.Exit();
                return;
            }
            // use dialog box to get output directory
            folderBrowserDialog1.ShowDialog();
            // build list structures to create output files
            PPWork.BuildTopAndOther();
            PPWork.BuildTopDaily();
            // if csv output 
            if (radioButton3.Checked)
            {
                //if only number id is needed
                if (radioButton1.Checked)
                {
                    PPWork.CreateSmallOutputFiles(folderBrowserDialog1.SelectedPath);
                    // show done message
                    label1.Text = "CSV output Files TopPost.csv, OtherPost.csv and TopDayPost.csv Have been created in folder " + folderBrowserDialog1.SelectedPath;
                }
                //if full record is needed
                if (radioButton2.Checked)
                {
                    PPWork.CreateOutputFiles(folderBrowserDialog1.SelectedPath);
                    // show done message
                    label1.Text = "CSV output Files TopPostF.csv, OtherPostF.csv and TopDayPostF.csv Have been created in folder " + folderBrowserDialog1.SelectedPath;
                }
               
            }
            //JSON output
            if (radioButton4.Checked)
            {
                //if only number id is needed
                if (radioButton1.Checked)
                {
                    PPWork.CreateSmallOutputFilesJ(folderBrowserDialog1.SelectedPath);
                    // show done message
                    label1.Text = "JSON output Files TopPost.txt, OtherPost.txt and TopDayPost.txt Have been created in folder " + folderBrowserDialog1.SelectedPath;
                }
                //if full record is needed
                if (radioButton2.Checked)
                {
                    PPWork.CreateOutputFilesJ(folderBrowserDialog1.SelectedPath);
                    // show done message
                    label1.Text = "JSON output Files TopPostF.txt, OtherPostF.txt and TopDayPostF.txt Have been created in folder " + folderBrowserDialog1.SelectedPath;
                }
               
            }
           
            // change button text to trigger exit next time it is pushed
            button2.Text = "Exit";
        }

        
    }
}
