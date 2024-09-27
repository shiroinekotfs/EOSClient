using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Principal;

namespace EOSClient;
public class frmAnnoucement : Form
{
	private IContainer components = null;

	private TextBox txtNoiQuy;

	private CheckBox chbRead;

	private Button btnNext;

	public frmAnnoucement()
	{
		InitializeComponent();
	}

	private void btnNext_Click(object sender, EventArgs e)
	{
		AuthenticateForm authenticateForm = new AuthenticateForm();
		authenticateForm.Show();
		EOSLogging.InitLogging();
        Hide();
	}

	private void chbRead_CheckedChanged(object sender, EventArgs e)
	{
		btnNext.Enabled = chbRead.Checked;
	}

	private bool IsAdministrator()
	{
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
		if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
		{
			return true;
		}
		return false;
	}

	private void frmAnnoucement_Load(object sender, EventArgs e)
	{
		if (!IsAdministrator())
		{
			MessageBox.Show("You must login Windows as System Administrator or Run [EOS Client] as Administrator.", "Run as Administrator!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}
	}

	private void txtNoiQuy_TextChanged(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EOSClient.frmAnnoucement));
		this.txtNoiQuy = new System.Windows.Forms.TextBox();
		this.chbRead = new System.Windows.Forms.CheckBox();
		this.btnNext = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.txtNoiQuy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtNoiQuy.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.txtNoiQuy.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtNoiQuy.Location = new System.Drawing.Point(12, 12);
		this.txtNoiQuy.Multiline = true;
		this.txtNoiQuy.Name = "txtNoiQuy";
		this.txtNoiQuy.ReadOnly = true;
		this.txtNoiQuy.Size = new System.Drawing.Size(1006, 389);
		this.txtNoiQuy.TabIndex = 2;
		this.txtNoiQuy.Text = resources.GetString("txtNoiQuy.Text");
		this.txtNoiQuy.TextChanged += new System.EventHandler(txtNoiQuy_TextChanged);
		this.chbRead.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.chbRead.AutoSize = true;
		this.chbRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.chbRead.ForeColor = System.Drawing.Color.Blue;
		this.chbRead.Location = new System.Drawing.Point(688, 409);
		this.chbRead.Name = "chbRead";
		this.chbRead.Size = new System.Drawing.Size(249, 19);
		this.chbRead.TabIndex = 1;
		this.chbRead.Text = "Tôi đã đọc và hiểu rõ Nội quy kỳ thi";
		this.chbRead.UseVisualStyleBackColor = true;
		this.chbRead.CheckedChanged += new System.EventHandler(chbRead_CheckedChanged);
		this.btnNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnNext.Enabled = false;
		this.btnNext.Location = new System.Drawing.Point(943, 407);
		this.btnNext.Name = "btnNext";
		this.btnNext.Size = new System.Drawing.Size(75, 23);
		this.btnNext.TabIndex = 1;
		this.btnNext.Text = "Next";
		this.btnNext.UseVisualStyleBackColor = true;
		this.btnNext.Click += new System.EventHandler(btnNext_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1028, 438);
		base.Controls.Add(this.btnNext);
		base.Controls.Add(this.chbRead);
		base.Controls.Add(this.txtNoiQuy);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.MaximizeBox = false;
		base.Name = "frmAnnoucement";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Nội quy kỳ thi";
		base.Load += new System.EventHandler(frmAnnoucement_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
