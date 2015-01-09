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
        Hashtable functions = new Hashtable();

        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode("TRACE");

            foreach (string line in richTextBox1.Text.Split('\n'))
            {
                if (line.Contains("TRACE: push"))
                {
                    String fcn = line.Remove(0, 12);
                    TreeNode node = new TreeNode(fcn);
                    root.Nodes.Add(node);
                    root = node;

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
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("runtimeTRACE for OpenModelica\nVersion 0.1\n\n(c) 2014-2015, Lennart A. Ochel", "runtimeTRACE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
