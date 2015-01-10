using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace runtimeTRACE
{
    public partial class Form1 : Form
    {
        String name = "runtimeTRACE for OpenModelica";
        String version = "Version 0.1+dev";
        String author = "(c) 2014-2015, Lennart A. Ochel";

        String filename = "";
        System.IO.FileSystemWatcher fsWatcher = new System.IO.FileSystemWatcher();

        // This delegate enables asynchronous calls for setting
        // the text property on the RichTextBox control.
        delegate void SetTextCallback(string text);

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = name + " | " + version + " | " + author;
            fsWatcher.Changed += new System.IO.FileSystemEventHandler(TraceDump_Changed);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if(filename == "")
                toolStripStatusLabel1.Text = "loading...";
            else
                toolStripStatusLabel1.Text = "loading " + filename + " ...";

            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode("TRACE");
            Int32 level = 0;
            Hashtable functions = new Hashtable();

            foreach (string line in richTextBox1.Text.Split('\n'))
            {
                if (line.Contains("TRACE: push"))
                {
                    String fcn = line.Remove(0, 12);
                    TreeNode node = new TreeNode(fcn);
                    try
                    {
                        root.Nodes.Add(node);
                    }
                    catch (Exception)
                    {
                    }
                    root = node;
                    level++;

                    if (functions.Contains(fcn))
                    {
                        int count = (int)functions[fcn];
                        functions.Remove(fcn);
                        functions.Add(fcn, count + 1);
                    }
                    else
                        functions.Add(fcn, 1);
                }
                else if (line.Contains("TRACE: pop"))
                {
                    try
                    {
                        root = root.Parent;
                    }
                    catch (Exception)
                    {
                    }
                    level--;
                }
                else
                {
                    try
                    {
                        root.Nodes.Add(line);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if(level>=0)
                treeView1.Nodes.Add(root);

            dataGridView1.Rows.Clear();
            foreach (String fcn in functions.Keys)
            {
                object[] row0 = { true, fcn, (int)functions[fcn] };

                dataGridView1.Rows.Add(row0);
            }

            if (level == 0)
            {
                if (filename == "")
                    toolStripStatusLabel1.Text = "TRACE dump loaded";
                else
                    toolStripStatusLabel1.Text = filename + " loaded";
            }
            else
            {
                if (filename == "")
                    toolStripStatusLabel1.Text = "TRACE dump is corrupted: level=" + level;
                else
                    toolStripStatusLabel1.Text = filename + " is corrupted: level=" + level;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(name + "\n" + version + "\n\n" + author, "runtimeTRACE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.Enabled = false;
                filename = ofd.FileName;

                fsWatcher.Path = System.IO.Path.GetDirectoryName(filename);
                fsWatcher.Filter = System.IO.Path.GetFileName(filename);
                
                fsWatcher.EnableRaisingEvents = true;
                richTextBox1.Text = System.IO.File.ReadAllText(filename);
            }
        }

        private void TraceDump_Changed(object source, System.IO.FileSystemEventArgs e)
        {
            System.Threading.Thread demoThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProcSafe));
            demoThread.Start();
        }

        private void ThreadProcSafe()
        {
            try
            {
                String text = System.IO.File.ReadAllText(filename);
                this.SetText(text);
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "Couldn't read " + filename;
            }
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.Text = text;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fsWatcher.EnableRaisingEvents = false;
            richTextBox1.Enabled = true;
            filename = "";
            richTextBox1.Text = "<insert code here>";
        }
    }
}
