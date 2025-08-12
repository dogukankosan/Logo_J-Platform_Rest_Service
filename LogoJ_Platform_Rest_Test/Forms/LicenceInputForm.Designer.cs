namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class LicenceInputForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenceInputForm));
            this.txt_Key = new DevExpress.XtraEditors.TextEdit();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_CompanyName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Key.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CompanyName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_Key
            // 
            this.txt_Key.EditValue = "";
            this.txt_Key.Location = new System.Drawing.Point(170, 60);
            this.txt_Key.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_Key.Name = "txt_Key";
            this.txt_Key.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_Key.Properties.Appearance.Options.UseFont = true;
            this.txt_Key.Properties.MaxLength = 500;
            this.txt_Key.Size = new System.Drawing.Size(258, 28);
            this.txt_Key.TabIndex = 1;
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
            this.btn_Save.Location = new System.Drawing.Point(0, 118);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(455, 58);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(29, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 22);
            this.label1.TabIndex = 8;
            this.label1.Text = "Lisans Anahtarı:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(29, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Firma Adı:";
            // 
            // txt_CompanyName
            // 
            this.txt_CompanyName.EditValue = "";
            this.txt_CompanyName.Location = new System.Drawing.Point(170, 8);
            this.txt_CompanyName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_CompanyName.Name = "txt_CompanyName";
            this.txt_CompanyName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_CompanyName.Properties.Appearance.Options.UseFont = true;
            this.txt_CompanyName.Properties.MaxLength = 250;
            this.txt_CompanyName.Size = new System.Drawing.Size(258, 28);
            this.txt_CompanyName.TabIndex = 0;
            // 
            // LicenceInputForm
            // 
            this.AcceptButton = this.btn_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(455, 176);
            this.Controls.Add(this.txt_CompanyName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.txt_Key);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("LicenceInputForm.IconOptions.LargeImage")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "LicenceInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lisans Bilgileri";
            this.Load += new System.EventHandler(this.LicenceInputForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txt_Key.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_CompanyName.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public DevExpress.XtraEditors.TextEdit txt_Key;
        public DevExpress.XtraEditors.TextEdit txt_CompanyName;
    }
}