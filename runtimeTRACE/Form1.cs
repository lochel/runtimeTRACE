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

        Hashtable functions = new Hashtable();

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = name + " | " + version + " | " + author;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "loading...";

            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode("TRACE");
            Int32 level = 0;

            foreach (string line in richTextBox1.Text.Split('\n'))
            {
                if (line.Contains("TRACE: push"))
                {
                    String fcn = line.Remove(0, 12);
                    TreeNode node = new TreeNode(fcn);
                    root.Nodes.Add(node);
                    root = node;
                    level++;

                    if(functions.Contains(fcn))
                    {
                        int count = (int)functions[fcn];
                        functions.Remove(fcn);
                        functions.Add(fcn, count+1);
                    }
                    else
                        functions.Add(fcn, 1);
                }
                else if (line.Contains("TRACE: pop"))
                {
                    root = root.Parent;
                    level--;
                }
                else
                    root.Nodes.Add(line);
            }


            treeView1.Nodes.Add(root);

            dataGridView1.Rows.Clear();
            foreach (String fcn in functions.Keys)
            {
                object[] row0 = { true, fcn, (int)functions[fcn] };

                dataGridView1.Rows.Add(row0);
            }

            if (level == 0)
                toolStripStatusLabel1.Text = "TRACE dump loaded";
            else
                toolStripStatusLabel1.Text = "TRACE dump corrupted: level=" + level;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(name + "\n" + version + "\n\n" + author, "runtimeTRACE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
