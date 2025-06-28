namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class RestServiceSettingForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RestServiceSettingForm));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_URL = new DevExpress.XtraEditors.TextEdit();
            this.txt_PeriodNo = new DevExpress.XtraEditors.TextEdit();
            this.txt_Username = new DevExpress.XtraEditors.TextEdit();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_Password = new DevExpress.XtraEditors.TextEdit();
            this.txt_CountryLang = new DevExpress.XtraEditors.TextEdit();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_CompanyNo = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_URL.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PeriodNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Username.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CountryLang.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CompanyNo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.panel1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(839, 498);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Rest Servis Ayarları";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txt_URL);
            this.panel1.Controls.Add(this.txt_PeriodNo);
            this.panel1.Controls.Add(this.txt_Username);
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txt_Password);
            this.panel1.Controls.Add(this.txt_CountryLang);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txt_CompanyNo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(2, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(283, 463);
            this.panel1.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(17, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "J-Platform URL:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label7.Location = new System.Drawing.Point(17, 262);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 17);
            this.label7.TabIndex = 20;
            this.label7.Text = "Dönem No:";
            // 
            // txt_URL
            // 
            this.txt_URL.Location = new System.Drawing.Point(17, 45);
            this.txt_URL.Name = "txt_URL";
            this.txt_URL.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_URL.Properties.Appearance.Options.UseFont = true;
            this.txt_URL.Properties.MaxLength = 100;
            this.txt_URL.Size = new System.Drawing.Size(221, 24);
            this.txt_URL.TabIndex = 0;
            // 
            // txt_PeriodNo
            // 
            this.txt_PeriodNo.Location = new System.Drawing.Point(17, 289);
            this.txt_PeriodNo.Name = "txt_PeriodNo";
            this.txt_PeriodNo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_PeriodNo.Properties.Appearance.Options.UseFont = true;
            this.txt_PeriodNo.Properties.MaxLength = 10;
            this.txt_PeriodNo.Size = new System.Drawing.Size(221, 24);
            this.txt_PeriodNo.TabIndex = 4;
            this.txt_PeriodNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_PeriodNo_KeyPress);
            // 
            // txt_Username
            // 
            this.txt_Username.Location = new System.Drawing.Point(17, 106);
            this.txt_Username.Name = "txt_Username";
            this.txt_Username.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Username.Properties.Appearance.Options.UseFont = true;
            this.txt_Username.Properties.MaxLength = 50;
            this.txt_Username.Size = new System.Drawing.Size(221, 24);
            this.txt_Username.TabIndex = 1;
            // 
            // btn_Save
            // 
            this.btn_Save.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.btn_Save.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Save.Appearance.Options.UseBackColor = true;
            this.btn_Save.Appearance.Options.UseFont = true;
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Save.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Save.ImageOptions.Image")));
            this.btn_Save.Location = new System.Drawing.Point(0, 414);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(281, 47);
            this.btn_Save.TabIndex = 6;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(17, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "J-Platform Kullanıcı Adı:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label5.Location = new System.Drawing.Point(17, 323);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "J-Platform Dil:";
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(17, 167);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Password.Properties.Appearance.Options.UseFont = true;
            this.txt_Password.Properties.MaxLength = 65;
            this.txt_Password.Properties.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(221, 24);
            this.txt_Password.TabIndex = 2;
            // 
            // txt_CountryLang
            // 
            this.txt_CountryLang.Location = new System.Drawing.Point(17, 350);
            this.txt_CountryLang.Name = "txt_CountryLang";
            this.txt_CountryLang.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_CountryLang.Properties.Appearance.Options.UseFont = true;
            this.txt_CountryLang.Properties.MaxLength = 10;
            this.txt_CountryLang.Size = new System.Drawing.Size(221, 24);
            this.txt_CountryLang.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label3.Location = new System.Drawing.Point(17, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "J-Platform Kullanıcı Şifre:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label4.Location = new System.Drawing.Point(17, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "J-Platform Şirket No:";
            // 
            // txt_CompanyNo
            // 
            this.txt_CompanyNo.Location = new System.Drawing.Point(17, 228);
            this.txt_CompanyNo.Name = "txt_CompanyNo";
            this.txt_CompanyNo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_CompanyNo.Properties.Appearance.Options.UseFont = true;
            this.txt_CompanyNo.Properties.MaxLength = 10;
            this.txt_CompanyNo.Size = new System.Drawing.Size(221, 24);
            this.txt_CompanyNo.TabIndex = 3;
            this.txt_CompanyNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_CompanyNo_KeyPress);
            // 
            // RestServiceSettingForm
            // 
            this.AcceptButton = this.btn_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(839, 498);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("RestServiceSettingForm.IconOptions.LargeImage")));
            this.MaximizeBox = false;
            this.Name = "RestServiceSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rest Servis Ayarları";
            this.Load += new System.EventHandler(this.RestServiceSettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_URL.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_PeriodNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Username.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CountryLang.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CompanyNo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_URL;
        private System.Windows.Forms.Label label5;
        private DevExpress.XtraEditors.TextEdit txt_CountryLang;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraEditors.TextEdit txt_CompanyNo;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraEditors.TextEdit txt_Password;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit txt_Username;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private DevExpress.XtraEditors.TextEdit txt_PeriodNo;
    }
}