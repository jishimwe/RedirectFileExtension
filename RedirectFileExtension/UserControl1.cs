using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedirectFileExtension
{
	public partial class UserControl1 : UserControl
	{
		private IDictionary<string, string> config;

		public UserControl1(IDictionary<string, string> config)
		{
			InitializeComponent();
			this.config = config;
			ListGroupBoxes();
		}

		private string accept = "OK";
		private string refuse = "Cancel";
		private IDictionary<Label, TextBox> labelsAndVals;

		private IDictionary<string, string> acceptButton(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void cancelButton(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void label_Click(object sender, EventArgs e)
		{

		}

		private void groupBox_Enter(object sender, EventArgs e)
		{

		}

		private GroupBox GetGroupBox(string title, string key, string text, GroupBox boxAbove = null)
		{
			GroupBox g = new GroupBox()
			{
				Size = new Size(300, 40),
				Name = title,
				Text = "",
				Padding = new Padding(3)
			};

			Label l = new Label()
			{
				AutoSize = true,
				Name = key,
				Text = key,
				Dock = DockStyle.Left,
				Size = new Size(50, 20)
			};

			TextBox t = new TextBox()
			{
				AutoSize = true,
				Name = key,
				Text = text,
				Dock = DockStyle.Right,
				Size = new Size(225, 20),
			};
			t.TextChanged += textBox_TextChanged;

			g.Controls.Add(l);
			g.Controls.Add(t);

			return g;
		}

		private void ListGroupBoxes()
		{
			foreach (var entry in config)
			{
				flowLayoutPanel1.Controls.Add(GetGroupBox(entry.Key, entry.Key, entry.Value));
			}
		}
	}
}
