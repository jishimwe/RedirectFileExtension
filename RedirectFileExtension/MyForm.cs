using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RedirectFileExtension
{
	public partial class MyForm : Form
	{
		public IDictionary<string, string> data;
		public IDictionary<string, string> config;
		private readonly IDictionary<string, string> _backup;
		private readonly string _formName;
		private readonly bool _redir;
		public MyForm(IDictionary<string, string> data, string name, IDictionary<string, string> config = null, bool redir = false)
		{
			_formName = name;
			InitializeComponent();
			this.data = data;
			_backup = data;
			ListGroupBoxes();
			this.config = config;
			_redir = redir;
		}

		private void MyForm_Load(object sender, EventArgs e)
		{
			this.Text = _formName;
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			data = _backup;
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void TextBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tb = sender as TextBox;
			data[tb.Name] = tb.Text;
		}

		private FlowLayoutPanel GetGroupBox(string title, string key, string text)
		{
			flowLayoutPanel1.Size = new Size(this.Width, flowLayoutPanel1.Height);
			FlowLayoutPanel g = new FlowLayoutPanel()
			{
				Size = new Size(flowLayoutPanel1.Width, 40),
				Name = title,
				Text = "",
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(3),
				AutoSize = true,
				WrapContents = false
			};

			Label l = new Label()
			{
				AutoSize = false,
				Name = key,
				Text = key,
				Size = new Size((int)(g.Width * 0.18), 20),
				TextAlign = ContentAlignment.MiddleRight,
			};

			TextBox t = new TextBox()
			{
				AutoSize = false,
				TextAlign = HorizontalAlignment.Left,
				Name = key,
				Text = text,
				Size = new Size((int)(g.Width * 0.68), 20),
			};
			t.TextChanged += TextBox_TextChanged;
			t.AcceptsTab = false;
			t.AcceptsReturn = false;

			string s = string.Empty;
			bool isFile = true;
			if (key.Contains("Path"))
			{
				s = "Open Folder";
				isFile = false;
			}
			else if (key.Contains("File"))
			{
				s = "Open File";
			}
			else if (key == "MergeOptions")
			{
				List<string> mergeOptions = new List<string>()
				{
					"Union Local-Remote",
					"Local Changes",
					"Remote Changes",
					"Merge formatted file"
				};
				g.Size = new Size(g.Width, g.Height * 2);
				g.FlowDirection = FlowDirection.TopDown;
				g.Controls.Add(l);
				g.Controls.Add(RadioButtonCreator(mergeOptions[0], g, "0"));
				g.Controls.Add(RadioButtonCreator(mergeOptions[1], g, "1"));
				g.Controls.Add(RadioButtonCreator(mergeOptions[2], g, "2"));
				g.Controls.Add(RadioButtonCreator(mergeOptions[3], g, "3", true));
				return g;
			}

			Button b = new Button()
			{
				Name = s,
				Text = s,
				Size = new Size((int)(g.Width * 0.1), 20),
			};
			b.Click += (sender, args) =>
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.ValidateNames = isFile;
				ofd.CheckFileExists = isFile;
				ofd.CheckPathExists = true;
				ofd.Multiselect = false;
				if (!isFile)
					ofd.FileName = "Folder Selection";
				if (config != null)
					ofd.InitialDirectory = _redir? config[RedirectProjectConfig.RedirectDirectoryPath] : config[RedirectProjectConfig.RealRepositoryPath];

				if (ofd.ShowDialog() == DialogResult.OK)
					t.Text = ofd.FileName;
			};

			g.Controls.Add(l);
			g.Controls.Add(t);
			if (!string.IsNullOrEmpty(s))
				g.Controls.Add(b);

			return g;
		}

		private RadioButton RadioButtonCreator(string name, FlowLayoutPanel f, string index, bool defaultRB = false)
		{
			RadioButton rb = new RadioButton()
			{
				AutoSize = false,
				Name = index,
				Text = name,
				AutoCheck = true,
				Checked = defaultRB,
				Size = new Size((int)(f.Width * 0.5), f.Height / 4),
				
			};
			rb.CheckedChanged += Rb_CheckedChanged;
			return rb;
		}

		private void Rb_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			//data[RedirectProjectConfig.MergeOptions] = rb.Name;
			if (rb.Checked)
				data[RedirectProjectConfig.MergeOptions] = rb.Name;
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
