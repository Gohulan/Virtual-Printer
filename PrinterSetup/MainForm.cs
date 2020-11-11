using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.Remoting.Messaging;

namespace PrinterSetup
{
	public class MainForm : Form
	{
		private IContainer components = null;
		private Button button1;
		private Label label1;
		private ComboBox comboBox1;
		private TextBox textBox1;
		private Label label2;
		private FolderBrowserDialog folderBrowserDialog1;
		private Button button2;
		private TextBox textBox2;
		private Label label3;
		private Button button3;
		private Button InstallButton;
		public string installationpath;
		public string path;
		//public MainForm();

		public MainForm()
		{
			this.InitializeComponent();
			

		}

		private void InstallButton_Click(object sender, EventArgs e)
		{
			

			try
			{
				

				bool flag = !File.Exists("C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PSCRIPT5.DLL");
				if (flag)
				{
					File.Copy("PSCRIPT5.DLL", "C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PSCRIPT5.DLL");
				}
				bool flag2 = !File.Exists("C:\\WINDOWS\\system32\\spool\\drivers\\x64\\OperaVirtual.PPD");
				if (flag2)
				{
					File.Copy("OperaVirtual.PPD", "C:\\WINDOWS\\system32\\spool\\drivers\\x64\\OperaVirtual.PPD");
				}
				bool flag3 = !File.Exists("C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PS5UI.DLL");
				if (flag3)
				{
					File.Copy("PS5UI.DLL", "C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PS5UI.DLL");
				}
				bool flag4 = !File.Exists("C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PSCRIPT.HLP");
				if (flag4)
				{
					File.Copy("PSCRIPT.HLP", "C:\\WINDOWS\\system32\\spool\\drivers\\x64\\PSCRIPT.HLP");
				}
				bool flag5 = !File.Exists("C:\\WINDOWS\\system32\\mfilemon.DLL");
				if (flag5)
				{
					File.Copy("mfilemon.DLL", "C:\\WINDOWS\\system32\\mfilemon.DLL");
				}
				bool flag6 = !File.Exists("C:\\WINDOWS\\system32\\mfilemonUI.DLL");
				if (flag6)
				{
					File.Copy("mfilemonUI.DLL", "C:\\WINDOWS\\system32\\mfilemonUI.DLL");
				}

				SpoolerHelper sh = new SpoolerHelper();
				SpoolerHelper.GenericResult result = sh.AddVPrinter(textBox1.Text, textBox1.Text);

				

				
				
			}
			catch (Exception ex)
			{

				string message = ex.Message;
				string title = "Error while adding Virtual Printer " + textBox1.Text;
				MessageBoxButtons buttons = MessageBoxButtons.RetryCancel;
				DialogResult result1 = MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
				//MessageBox.Show(ex.Message);
			}

			

			
		}

		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.InstallButton = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// InstallButton
			// 
			this.InstallButton.Location = new System.Drawing.Point(405, 226);
			this.InstallButton.Margin = new System.Windows.Forms.Padding(2);
			this.InstallButton.Name = "InstallButton";
			this.InstallButton.Size = new System.Drawing.Size(135, 62);
			this.InstallButton.TabIndex = 6;
			this.InstallButton.Text = "Install Printer";
			this.InstallButton.UseVisualStyleBackColor = true;
			this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(406, 62);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(134, 61);
			this.button1.TabIndex = 1;
			this.button1.Text = "Get all printers";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "All Available Printers";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(173, 83);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(199, 21);
			this.comboBox1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(173, 248);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(199, 20);
			this.textBox1.TabIndex = 5;
			this.textBox1.Text = "Opera Virtual Printer";
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(32, 251);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(125, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "New Virtual Printer Name";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(405, 139);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(134, 61);
			this.button2.TabIndex = 4;
			this.button2.Text = "...Browse";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(173, 160);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(199, 20);
			this.textBox2.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(32, 163);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(126, 13);
			this.label3.TabIndex = 18;
			this.label3.Text = "Opera Virtual Printer Path";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(335, 308);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 19;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click_1);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 366);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.InstallButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Opera Virtual Printer";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void button1_Click(object sender, EventArgs e)
		{
			PrintDocument prtdoc = new PrintDocument();
			string strDefaultPrinter = prtdoc.PrinterSettings.PrinterName;

			//PrintDocument prtdoc = new PrintDocument();
			//string strDefaultPrinter = prtdoc.PrinterSettings.PrinterName;
			foreach (String strPrinter in PrinterSettings.InstalledPrinters)
			{
				comboBox1.Items.Add(strPrinter);
				if (strPrinter == strDefaultPrinter)
				{
					comboBox1.SelectedIndex = comboBox1.Items.IndexOf(strPrinter);
				}


			}
		}

		private void button3_Click(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderDlg = new FolderBrowserDialog();
			folderDlg.ShowNewFolderButton = true;
			// Show the FolderBrowserDialog.  
			DialogResult result = folderDlg.ShowDialog();
			if (result == DialogResult.OK)
			{
				textBox2.Text = folderDlg.SelectedPath;
				Environment.SpecialFolder root = folderDlg.RootFolder;

				path = textBox2.Text.Replace(@"\", @"\\");
				installationpath = "\"" + path + "\"";
			}

			
		}

		private void button3_Click_1(object sender, EventArgs e)
		{
			

			MessageBox.Show(installationpath);
			
		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}
	}
}
