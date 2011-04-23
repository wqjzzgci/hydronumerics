using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  public partial class ShowChanges : Form
  {
    public ShowChanges()
    {
      InitializeComponent();
    }

    public void ShowThis(string text)
    {
      textBox1.Text = text;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }
}
