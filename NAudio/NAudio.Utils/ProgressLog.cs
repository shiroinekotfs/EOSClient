using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NAudio.Utils;

public class ProgressLog : UserControl
{
	private delegate void LogMessageDelegate(Color color, string message);

	private delegate void ClearLogDelegate();

	private IContainer components;

	private RichTextBox richTextBoxLog;

	public new string Text => richTextBoxLog.Text;

	public ProgressLog()
	{
		InitializeComponent();
	}

	public void LogMessage(Color color, string message)
	{
		if (richTextBoxLog.InvokeRequired)
		{
			Invoke(new LogMessageDelegate(LogMessage), color, message);
		}
		else
		{
			richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
			richTextBoxLog.SelectionColor = color;
			richTextBoxLog.AppendText(message);
			richTextBoxLog.AppendText(Environment.NewLine);
		}
	}

	public void ClearLog()
	{
		if (richTextBoxLog.InvokeRequired)
		{
			Invoke(new ClearLogDelegate(ClearLog), new object[0]);
		}
		else
		{
			richTextBoxLog.Clear();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBoxLog.Location = new System.Drawing.Point(1, 1);
		this.richTextBoxLog.Name = "richTextBoxLog";
		this.richTextBoxLog.ReadOnly = true;
		this.richTextBoxLog.Size = new System.Drawing.Size(311, 129);
		this.richTextBoxLog.TabIndex = 0;
		this.richTextBoxLog.Text = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
		base.Controls.Add(this.richTextBoxLog);
		base.Name = "ProgressLog";
		base.Padding = new System.Windows.Forms.Padding(1);
		base.Size = new System.Drawing.Size(313, 131);
		base.ResumeLayout(false);
	}
}
