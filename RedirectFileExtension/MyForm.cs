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
	public partial class MyForm : Form
	{
		public IDictionary<string, string> data;
		private IDictionary<string, string> _backup;
		private string _formName = "MyForm";
		public MyForm(IDictionary<string, string> data, string name)
		{
			_formName = name;
			InitializeComponent();
			this.data = data;
			_backup = data;
			ListGroupBoxes();
		}

		private void MyForm_Load(object sender, EventArgs e)
		{
			this.Text = _formName;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			data = _backup;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			data[tb.Name] = tb.Text;
			//Console.WriteLine("Whats happening?");
		}

		private GroupBox GetGroupBox(string title, string key, string text, GroupBox boxAbove = null)
		{
			GroupBox g = new GroupBox()
			{
				Size = new Size(flowLayoutPanel1.Width, 40),
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
				Size = new Size((int)(g.Width * 0.25), 20)
			};

			TextBox t = new TextBox()
			{
				AutoSize = true,
				Name = key,
				Text = text,
				Dock = DockStyle.Right,
				Size = new Size((int)(g.Width * 0.75), 20)
			};
			t.TextChanged += textBox_TextChanged;
			t.AcceptsTab = false;
			t.AcceptsReturn = false;

			g.Controls.Add(l);
			g.Controls.Add(t);

			return g;
		}

		private void ListGroupBoxes()
		{
			foreach (var entry in data)
			{
				flowLayoutPanel1.Controls.Add(GetGroupBox(entry.Key, entry.Key, entry.Value));
			}
		}
	}
}
