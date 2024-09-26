using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace EOSClient;

public class frmCheckFont : Form
{
	private IContainer components = null;

	private Button btnClose;

	private TextBox txtFontGuide;

	private Label lblTestDisplay;

	public frmCheckFont()
	{
		InitializeComponent();
	}

	private bool checkFont(string fontName)
	{
		using Font font = new Font(fontName, 12f, FontStyle.Regular);
		if (font.Name.Equals(fontName))
		{
			return true;
		}
		return false;
	}

	private void frmCheckFont_Load(object sender, EventArgs e)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		InstalledFontCollection installedFontCollection = new InstalledFontCollection();
		FontFamily[] families = installedFontCollection.Families;
		FontFamily[] array = families;
		foreach (FontFamily fontFamily in array)
		{
			string text = fontFamily.GetName(0).Trim().ToUpper();
			if (text.StartsWith("KaiTi".ToUpper()))
			{
				flag = true;
			}
			if (text.StartsWith("Ms Mincho".ToUpper()))
			{
				flag2 = true;
			}
			if (text.StartsWith("HGSeikai".ToUpper()))
			{
				flag3 = true;
			}
			if (text.StartsWith("NtMotoya".ToUpper()))
			{
				flag4 = true;
			}
		}
		string text2 = "CHECK FONT RESULT:\r\n\r\n";
		string text3 = "KaiTi";
		text2 = ((!flag) ? (text2 + "Chinese font ('" + text3 + "') : NOT FOUND.\r\n\r\n") : (text2 + "Chinese font ('" + text3 + "') : OK.\r\n\r\n"));
		string text4 = "MS Mincho";
		text2 = ((!flag2) ? (text2 + "Japanese font 1 ('" + text4 + "') :  NOT FOUND.\r\n\r\n") : (text2 + "Japanese font 1 ('" + text4 + "') : OK.\r\n\r\n"));
		text4 = "HGSeikaishotaiPRO";
		text2 = ((!flag3) ? (text2 + "Japanese font 2 ('" + text4 + "') :  NOT FOUND.\r\n\r\n") : (text2 + "Japanese font 2 ('" + text4 + "') : OK.\r\n\r\n"));
		text4 = "NtMotoya Kyotai";
		text2 = ((!flag4) ? (text2 + "Japanese font 3 ('" + text4 + "') :  NOT FOUND.\r\n\r\n") : (text2 + "Japanese font 3 ('" + text4 + "') : OK.\r\n\r\n"));
		if (!flag2 || !flag || !flag3 || !flag4)
		{
			text2 += "\r\n\r\nINSTALLING FONTS ON Windows:\r\n\r\nThere are several ways to install fonts on Windows.\r\nKeep in mind that you must be an Administrator on the target machine to install fonts.\r\n\r\n - Download the font.\r\n - Double-click on a font file to open the font preview and select 'Install'.\r\n\r\nOR\r\n\r\n - Right-click on a font file, and then select 'Install'.";
		}
		txtFontGuide.Text = text2;
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
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
		this.btnClose = new System.Windows.Forms.Button();
		this.txtFontGuide = new System.Windows.Forms.TextBox();
		this.lblTestDisplay = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClose.Location = new System.Drawing.Point(628, 480);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(75, 23);
		this.btnClose.TabIndex = 0;
		this.btnClose.Text = "Close";
		this.btnClose.UseVisualStyleBackColor = true;
		this.btnClose.Click += new System.EventHandler(btnClose_Click);
		this.txtFontGuide.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtFontGuide.BackColor = System.Drawing.Color.White;
		this.txtFontGuide.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtFontGuide.Location = new System.Drawing.Point(12, 12);
		this.txtFontGuide.Multiline = true;
		this.txtFontGuide.Name = "txtFontGuide";
		this.txtFontGuide.ReadOnly = true;
		this.txtFontGuide.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.txtFontGuide.Size = new System.Drawing.Size(691, 448);
		this.txtFontGuide.TabIndex = 1;
		this.lblTestDisplay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblTestDisplay.AutoSize = true;
		this.lblTestDisplay.Font = new System.Drawing.Font("MS Mincho", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblTestDisplay.Location = new System.Drawing.Point(9, 480);
		this.lblTestDisplay.Name = "lblTestDisplay";
		this.lblTestDisplay.Size = new System.Drawing.Size(173, 39);
		this.lblTestDisplay.TabIndex = 2;
		this.lblTestDisplay.Text = "ベトナム (in Japanese)\r\n越南 (in Chinese)\r\nViệt Nam (in Vietnamese)\r\n";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(715, 532);
		base.Controls.Add(this.lblTestDisplay);
		base.Controls.Add(this.txtFontGuide);
		base.Controls.Add(this.btnClose);
		base.Name = "frmCheckFont";
		this.Text = "Check fonts for Japanese/Chinese exam.";
		base.Load += new System.EventHandler(frmCheckFont_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
