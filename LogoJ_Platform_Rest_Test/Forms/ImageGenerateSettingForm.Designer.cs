namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class ImageGenerateSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageGenerateSettingForm));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.label3 = new System.Windows.Forms.Label();
            this.rch_imagePromt = new System.Windows.Forms.RichTextBox();
            this.txt_googleGeminiKey = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_imageKey = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_googleGeminiKey.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_imageKey.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.label3);
            this.groupControl1.Controls.Add(this.rch_imagePromt);
            this.groupControl1.Controls.Add(this.txt_googleGeminiKey);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.btn_Save);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Controls.Add(this.txt_imageKey);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(287, 529);
            this.groupControl1.TabIndex = 1;
            this.groupControl1.Text = "Resim Yapay Zeka Ayarı";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label3.Location = new System.Drawing.Point(12, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(215, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Şirkette Bulunan Malzemeler Bilgisi:";
            // 
            // rch_imagePromt
            // 
            this.rch_imagePromt.Location = new System.Drawing.Point(12, 181);
            this.rch_imagePromt.Name = "rch_imagePromt";
            this.rch_imagePromt.Size = new System.Drawing.Size(221, 77);
            this.rch_imagePromt.TabIndex = 2;
            this.rch_imagePromt.Text = "";
            // 
            // txt_googleGeminiKey
            // 
            this.txt_googleGeminiKey.Location = new System.Drawing.Point(12, 122);
            this.txt_googleGeminiKey.Name = "txt_googleGeminiKey";
            this.txt_googleGeminiKey.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_googleGeminiKey.Properties.Appearance.Options.UseFont = true;
            this.txt_googleGeminiKey.Properties.MaxLength = 250;
            this.txt_googleGeminiKey.Properties.PasswordChar = '*';
            this.txt_googleGeminiKey.Size = new System.Drawing.Size(221, 24);
            this.txt_googleGeminiKey.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Google Gemini API Key:";
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
            this.btn_Save.Location = new System.Drawing.Point(2, 480);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(283, 47);
            this.btn_Save.TabIndex = 3;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Resim API Key:";
            // 
            // txt_imageKey
            // 
            this.txt_imageKey.Location = new System.Drawing.Point(12, 65);
            this.txt_imageKey.Name = "txt_imageKey";
            this.txt_imageKey.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_imageKey.Properties.Appearance.Options.UseFont = true;
            this.txt_imageKey.Properties.MaxLength = 250;
            this.txt_imageKey.Properties.PasswordChar = '*';
            this.txt_imageKey.Size = new System.Drawing.Size(221, 24);
            this.txt_imageKey.TabIndex = 0;
            // 
            // ImageGenerateSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(912, 529);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ImageGenerateSettingForm.IconOptions.LargeImage")));
            this.Name = "ImageGenerateSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Resim AI Ayarları";
            this.Load += new System.EventHandler(this.ImageGenerateSettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_googleGeminiKey.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_imageKey.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rch_imagePromt;
        private DevExpress.XtraEditors.TextEdit txt_googleGeminiKey;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txt_imageKey;
    }
}