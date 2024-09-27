using System;
using IRemote;
using System.IO;
using EncryptData;
using NAudio.Wave;
using QuestionLib;
using System.Drawing;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Principal;

namespace EOSClient;
public class AuthenticateForm : Form
{
	private Button btnLogin;

	private TextBox txtUser;

	private TextBox txtPassword;

	private TextBox txtDomain;

	private Label lblUser;

	private Label lblPass;

	private Button btnCancel;

	private Label lblDomain;

	private Container components = null;

	private Label lblExamCode;

	private Label lblMessage;

	private Label lblVersion;

	private LinkLabel linkLBLCheckFont;

	private LinkLabel lblLinkCheckSound;

	private Label label1;

	private TextBox txtExamCode;

	private string version = "C3AF3F4B-EA15-4EDA-9750-C0214649FEC8";

	private ServerInfo si = null;

	private frmCheckFont fcf = null;

	public AuthenticateForm()
	{
		InitializeComponent();
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
		System.Configuration.AppSettingsReader appSettingsReader = new System.Configuration.AppSettingsReader();
		this.btnLogin = new System.Windows.Forms.Button();
		this.txtUser = new System.Windows.Forms.TextBox();
		this.txtPassword = new System.Windows.Forms.TextBox();
		this.txtDomain = new System.Windows.Forms.TextBox();
		this.lblUser = new System.Windows.Forms.Label();
		this.lblPass = new System.Windows.Forms.Label();
		this.btnCancel = new System.Windows.Forms.Button();
		this.lblDomain = new System.Windows.Forms.Label();
		this.lblExamCode = new System.Windows.Forms.Label();
		this.txtExamCode = new System.Windows.Forms.TextBox();
		this.lblMessage = new System.Windows.Forms.Label();
		this.lblVersion = new System.Windows.Forms.Label();
		this.linkLBLCheckFont = new System.Windows.Forms.LinkLabel();
		this.lblLinkCheckSound = new System.Windows.Forms.LinkLabel();
		this.label1 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnLogin.Location = new System.Drawing.Point(95, 192);
		this.btnLogin.Name = "btnLogin";
		this.btnLogin.Size = new System.Drawing.Size(86, 23);
		this.btnLogin.TabIndex = 3;
		this.btnLogin.Text = "Login";
		this.btnLogin.Click += new System.EventHandler(btnLogin_Click);
		this.txtUser.Location = new System.Drawing.Point(96, 80);
		this.txtUser.Name = "txtUser";
		this.txtUser.Size = new System.Drawing.Size(272, 20);
		this.txtUser.TabIndex = 1;
		this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtPassword.Location = new System.Drawing.Point(96, 120);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.PasswordChar = '*';
		this.txtPassword.Size = new System.Drawing.Size(272, 22);
		this.txtPassword.TabIndex = 2;
		this.txtDomain.Enabled = false;
		this.txtDomain.Location = new System.Drawing.Point(96, 160);
		this.txtDomain.Name = "txtDomain";
		this.txtDomain.Size = new System.Drawing.Size(272, 20);
		this.txtDomain.TabIndex = 9;
		this.txtDomain.Text = (string)appSettingsReader.GetValue("domain", typeof(string));
		this.lblUser.AutoSize = true;
		this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblUser.Location = new System.Drawing.Point(15, 80);
		this.lblUser.Name = "lblUser";
		this.lblUser.Size = new System.Drawing.Size(80, 16);
		this.lblUser.TabIndex = 6;
		this.lblUser.Text = "User Name:";
		this.lblPass.AutoSize = true;
		this.lblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblPass.Location = new System.Drawing.Point(24, 120);
		this.lblPass.Name = "lblPass";
		this.lblPass.Size = new System.Drawing.Size(71, 16);
		this.lblPass.TabIndex = 7;
		this.lblPass.Text = "Password:";
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancel.Location = new System.Drawing.Point(230, 192);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(86, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Exit";
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		this.lblDomain.AutoSize = true;
		this.lblDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblDomain.Location = new System.Drawing.Point(37, 160);
		this.lblDomain.Name = "lblDomain";
		this.lblDomain.Size = new System.Drawing.Size(58, 16);
		this.lblDomain.TabIndex = 5;
		this.lblDomain.Text = "Domain:";
		this.lblExamCode.AutoSize = true;
		this.lblExamCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblExamCode.Location = new System.Drawing.Point(14, 38);
		this.lblExamCode.Name = "lblExamCode";
		this.lblExamCode.Size = new System.Drawing.Size(81, 16);
		this.lblExamCode.TabIndex = 10;
		this.lblExamCode.Text = "Exam Code:";
		this.txtExamCode.Location = new System.Drawing.Point(96, 38);
		this.txtExamCode.Name = "txtExamCode";
		this.txtExamCode.Size = new System.Drawing.Size(272, 20);
		this.txtExamCode.TabIndex = 0;
		this.lblMessage.AutoSize = true;
		this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblMessage.ForeColor = System.Drawing.Color.Blue;
		this.lblMessage.Location = new System.Drawing.Point(56, 252);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(294, 17);
		this.lblMessage.TabIndex = 11;
		this.lblMessage.Text = "Register the exam may take time, please wait!";
		this.lblVersion.AutoSize = true;
		this.lblVersion.ForeColor = System.Drawing.Color.Blue;
		this.lblVersion.Location = new System.Drawing.Point(12, 197);
		this.lblVersion.Name = "lblVersion";
		this.lblVersion.Size = new System.Drawing.Size(64, 13);
		this.lblVersion.TabIndex = 12;
		this.lblVersion.Text = "23.03.20.24";
		this.linkLBLCheckFont.AutoSize = true;
		this.linkLBLCheckFont.Location = new System.Drawing.Point(252, 228);
		this.linkLBLCheckFont.Name = "linkLBLCheckFont";
		this.linkLBLCheckFont.Size = new System.Drawing.Size(59, 13);
		this.linkLBLCheckFont.TabIndex = 13;
		this.linkLBLCheckFont.TabStop = true;
		this.linkLBLCheckFont.Text = "Check font";
		this.linkLBLCheckFont.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLBLCheckFont_LinkClicked);
		this.lblLinkCheckSound.AutoSize = true;
		this.lblLinkCheckSound.Location = new System.Drawing.Point(93, 228);
		this.lblLinkCheckSound.Name = "lblLinkCheckSound";
		this.lblLinkCheckSound.Size = new System.Drawing.Size(110, 13);
		this.lblLinkCheckSound.TabIndex = 14;
		this.lblLinkCheckSound.TabStop = true;
		this.lblLinkCheckSound.Text = "Check sound (7 secs)";
		this.lblLinkCheckSound.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lblLinkCheckSound_LinkClicked);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.ForeColor = System.Drawing.Color.Red;
		this.label1.Location = new System.Drawing.Point(3, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(404, 17);
		this.label1.TabIndex = 15;
		this.label1.Text = "You cannot take the exam if EOS runs under a virtual machine.";
		base.AcceptButton = this.btnLogin;
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(409, 278);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.lblLinkCheckSound);
		base.Controls.Add(this.linkLBLCheckFont);
		base.Controls.Add(this.lblVersion);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.lblExamCode);
		base.Controls.Add(this.txtExamCode);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.lblPass);
		base.Controls.Add(this.lblUser);
		base.Controls.Add(this.txtDomain);
		base.Controls.Add(this.txtPassword);
		base.Controls.Add(this.txtUser);
		base.Controls.Add(this.lblDomain);
		base.Controls.Add(this.btnLogin);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AuthenticateForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "EOS Login Form";
		base.Load += new System.EventHandler(AuthenticateForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void btnLogin_Click(object sender, EventArgs e)
	{
		if (txtExamCode.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide an Exam code", "Login");
			return;
		}
		if (txtUser.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide an username", "Login");
			return;
		}
		if (txtPassword.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide a password", "Login");
			return;
		}
		if (txtDomain.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide a domain address", "Login");
			return;
		}
		try
		{
			if (!si.Public_IP.Trim().Equals(""))
			{
				si.IP = si.Public_IP;
			}
			string url = "tcp://" + si.IP + ":" + si.Port + "/Server";
			EOSLogging.LoggingForURL(url); //%Logging for URL
			
			IRemoteServer remoteServer = (IRemoteServer)Activator.GetObject(typeof(IRemoteServer), url);
			RegisterData registerData = new RegisterData();
			EOSLogging.ExportRegisterData(registerData);  //%Logging for registered data
            
			registerData.Login = txtUser.Text;
			registerData.Password = txtPassword.Text;
			registerData.ExamCode = txtExamCode.Text;
			EOSLogging.LoggingForUserField(registerData.Login, registerData.ExamCode); //%Logging for user data

            registerData.Machine = Environment.MachineName.ToUpper();
			EOSLogging.LoggingMachineInfo(registerData.Machine); //%Logging for machine info

            EOSData eOSData = remoteServer.ConductExam(registerData);
			EOSLogging.ExportExamData("All", eOSData); //%Logging for Exam data

            EOSLogging.ExportServerInfo(si); //%Logging for Server Info

            if (eOSData.Status == RegisterStatus.EXAM_CODE_NOT_EXISTS)
			{
				MessageBox.Show("Exam code is not available!", "Start exam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (eOSData.Status == RegisterStatus.FINISHED)
			{
				MessageBox.Show("The exam is finished!", "Start exam", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (eOSData.Status == RegisterStatus.REGISTERED)
			{
				MessageBox.Show("This user [" + txtUser.Text + "] is already registered. Need re-assign to continue the exam.", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (eOSData.Status == RegisterStatus.REGISTER_ERROR)
			{
				MessageBox.Show("Register ERROR, try again", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (eOSData.Status == RegisterStatus.NOT_ALLOW_MACHINE)
			{
				MessageBox.Show("Your machine is not allowed to take the exam!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (eOSData.Status == RegisterStatus.NOT_ALLOW_STUDENT)
			{
				MessageBox.Show("The account is NOT allowed to take the exam!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (eOSData.Status == RegisterStatus.LOGIN_FAILED)
			{
				MessageBox.Show("Sorry, unable to verify your information. Check [User Name] and [Password]!", "Login failed");
			}
			if (eOSData.Status == RegisterStatus.NEW || eOSData.Status == RegisterStatus.RE_ASSIGN)
			{
				//Hide();
				EOSLogging.ExportGUIData(eOSData.GUI); //%Logging for GUI

                eOSData.GUI = GZipHelper.DeCompress(eOSData.GUI, eOSData.OriginSize);
				Assembly assembly = Assembly.Load(eOSData.GUI);
				Type type = assembly.GetType("ExamClient.frmEnglishExamClient");
				Form form = (Form)Activator.CreateInstance(type);
				IExamclient examclient = (IExamclient)form;
				eOSData.GUI = null;
				eOSData.ServerInfomation = si;
                eOSData.RegData = registerData;
				examclient.SetExamData(eOSData);
				form.Show();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString());
			EOSLogging.LoggingForError(ex.ToString()); //%Logging for exception
			MessageBox.Show("Start Exam Error:\nCannot connect to the EOS server!\n", "Connecting...", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void AuthenticateForm_Load(object sender, EventArgs e)
	{
		string text = "EOS_Server_Info.dat";
		if (File.Exists(Application.StartupPath + "\\" + text))
		{
			try
			{
				string key = "04021976";
				byte[] arrBytes = EncryptSupport.DecryptQuestions_FromFile(text, key);
				si = (ServerInfo)EncryptSupport.ByteArrayToObject(arrBytes);
				if (!version.Equals(si.Version))
				{
					MessageBox.Show("Wrong EOS client version! Please copy the right EOS client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					Application.Exit();
				}
				return;
			}
			catch
			{
				MessageBox.Show("Wrong [" + text + "] file format! Please copy the right EOS client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				Application.Exit();
				return;
			}
		}
		MessageBox.Show("File [" + text + "] not found! Please copy the right EOS client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		Application.Exit();
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

	private void linkLBLCheckFont_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (fcf == null || fcf.IsDisposed)
		{
			fcf = new frmCheckFont();
		}
		fcf.Show();
	}

	private void lblLinkCheckSound_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (!File.Exists("ghosts.mp3"))
		{
			MessageBox.Show("Audio file ghosts.mp3 cannot be found!", "Check sound");
		}
		else
		{
			PlayFromUrl("ghosts.mp3");
		}
	}

	public void PlayFromUrl(string url)
	{
		FileStream fileStream = File.OpenRead(url);
		byte[] buffer = new byte[fileStream.Length];
		fileStream.Read(buffer, 0, (int)fileStream.Length);
		fileStream.Close();
		Stream inputStream = new MemoryStream(buffer);
		Mp3FileReader waveProvider = new Mp3FileReader(inputStream);
		WaveOut waveOut = new WaveOut();
		waveOut.Init(waveProvider);
		waveOut.Volume = 1f;
		waveOut.Play();
	}
}
