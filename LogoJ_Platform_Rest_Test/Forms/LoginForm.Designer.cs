namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.btn_Eyes = new DevExpress.XtraEditors.SimpleButton();
            this.btn_NotEye = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Login = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Password = new DevExpress.XtraEditors.TextEdit();
            this.txt_UserName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UserName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Eyes
            // 
            this.btn_Eyes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Eyes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Eyes.ImageOptions.Image")));
            this.btn_Eyes.Location = new System.Drawing.Point(104, 59);
            this.btn_Eyes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Eyes.Name = "btn_Eyes";
            this.btn_Eyes.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_Eyes.Size = new System.Drawing.Size(42, 36);
            this.btn_Eyes.TabIndex = 3;
            this.btn_Eyes.Click += new System.EventHandler(this.btn_Eyes_Click);
            // 
            // btn_NotEye
            // 
            this.btn_NotEye.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_NotEye.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_NotEye.ImageOptions.Image")));
            this.btn_NotEye.Location = new System.Drawing.Point(104, 58);
            this.btn_NotEye.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_NotEye.Name = "btn_NotEye";
            this.btn_NotEye.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_NotEye.Size = new System.Drawing.Size(42, 37);
            this.btn_NotEye.TabIndex = 4;
            this.btn_NotEye.Click += new System.EventHandler(this.btn_NotEye_Click);
            // 
            // btn_Login
            // 
            this.btn_Login.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;
            this.btn_Login.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Login.Appearance.Options.UseBackColor = true;
            this.btn_Login.Appearance.Options.UseFont = true;
            this.btn_Login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Login.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Login.ImageOptions.Image")));
            this.btn_Login.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
            this.btn_Login.Location = new System.Drawing.Point(162, 108);
            this.btn_Login.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(260, 54);
            this.btn_Login.TabIndex = 2;
            this.btn_Login.Text = "Ateşle";
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label2.Location = new System.Drawing.Point(13, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 25);
            this.label2.TabIndex = 12;
            this.label2.Text = "Şifre:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 25);
            this.label1.TabIndex = 11;
            this.label1.Text = "Kullanıcı Adı:";
            // 
            // txt_Password
            // 
            this.txt_Password.EditValue = "";
            this.txt_Password.Location = new System.Drawing.Point(162, 60);
            this.txt_Password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Password.Properties.Appearance.Options.UseFont = true;
            this.txt_Password.Properties.MaxLength = 50;
            this.txt_Password.Properties.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(260, 28);
            this.txt_Password.TabIndex = 1;
            // 
            // txt_UserName
            // 
            this.txt_UserName.EditValue = "";
            this.txt_UserName.Location = new System.Drawing.Point(162, 22);
            this.txt_UserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_UserName.Properties.Appearance.Options.UseFont = true;
            this.txt_UserName.Properties.MaxLength = 50;
            this.txt_UserName.Size = new System.Drawing.Size(260, 28);
            this.txt_UserName.TabIndex = 0;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btn_Login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(471, 190);
            this.Controls.Add(this.btn_Eyes);
            this.Controls.Add(this.btn_NotEye);
            this.Controls.Add(this.btn_Login);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_Password);
            this.Controls.Add(this.txt_UserName);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("LoginForm.IconOptions.LargeImage")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asyen Bilişim Logo J-Platform Aktarım V1.0";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_Password.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_UserName.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btn_Eyes;
        private DevExpress.XtraEditors.SimpleButton btn_NotEye;
        private DevExpress.XtraEditors.SimpleButton btn_Login;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_Password;
        private DevExpress.XtraEditors.TextEdit txt_UserName;
    }
}