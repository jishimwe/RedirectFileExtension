using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RedirectFileExtension
{
	partial class UserControl1
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new System.Windows.Forms.Button();
			this.refuseButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.label = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel1.SuspendLayout();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(0, 300);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 25);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// refuseButton
			// 
			this.refuseButton.Location = new System.Drawing.Point(75, 300);
			this.refuseButton.Name = "refuseButton";
			this.refuseButton.Size = new System.Drawing.Size(75, 25);
			this.refuseButton.TabIndex = 1;
			this.refuseButton.Text = "Cancel";
			this.refuseButton.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.groupBox);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(35, 25);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(353, 269);
			this.flowLayoutPanel1.TabIndex = 2;
			this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.label);
			this.groupBox.Controls.Add(this.textBox);
			this.groupBox.Location = new System.Drawing.Point(3, 3);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(300, 45);
			this.groupBox.TabIndex = 0;
			this.groupBox.TabStop = false;
			this.groupBox.Enter += new System.EventHandler(this.groupBox_Enter);
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(6, 22);
			this.label.Name = "label";
			this.label.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.label.Size = new System.Drawing.Size(34, 13);
			this.label.TabIndex = 0;
			this.label.Text = "label";
			this.label.Click += new System.EventHandler(this.label_Click);
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(82, 19);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(234, 20);
			this.textBox.TabIndex = 1;
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.refuseButton);
			this.Controls.Add(this.okButton);
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(400, 350);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button refuseButton;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.GroupBox groupBox;
		private List<System.Windows.Forms.GroupBox> groupBoxes;
	}
}
