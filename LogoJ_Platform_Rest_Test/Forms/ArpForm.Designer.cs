namespace LogoJ_Platform_Rest_Test
{
    partial class ArpForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArpForm));
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txt_arpCode = new DevExpress.XtraEditors.TextEdit();
            this.txt_arpDesc = new DevExpress.XtraEditors.TextEdit();
            this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.excelAktarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Clear = new DevExpress.XtraEditors.SimpleButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_arpCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_arpDesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip2;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.gridControl1.Location = new System.Drawing.Point(502, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(500, 548);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // txt_arpCode
            // 
            this.txt_arpCode.Location = new System.Drawing.Point(15, 68);
            this.txt_arpCode.Name = "txt_arpCode";
            this.txt_arpCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_arpCode.Properties.Appearance.Options.UseFont = true;
            this.txt_arpCode.Properties.MaxLength = 30;
            this.txt_arpCode.Size = new System.Drawing.Size(221, 24);
            this.txt_arpCode.TabIndex = 0;
            // 
            // txt_arpDesc
            // 
            this.txt_arpDesc.Location = new System.Drawing.Point(15, 124);
            this.txt_arpDesc.Name = "txt_arpDesc";
            this.txt_arpDesc.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.txt_arpDesc.Properties.Appearance.Options.UseFont = true;
            this.txt_arpDesc.Properties.MaxLength = 200;
            this.txt_arpDesc.Size = new System.Drawing.Size(221, 24);
            this.txt_arpDesc.TabIndex = 1;
            // 
            // btn_Save
            // 
            this.btn_Save.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.btn_Save.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Save.Appearance.Options.UseBackColor = true;
            this.btn_Save.Appearance.Options.UseFont = true;
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Save.ImageOptions.Image")));
            this.btn_Save.Location = new System.Drawing.Point(15, 162);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(221, 47);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "Kaydet";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cari Kodu:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(14, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cari Açıklama:";
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.btn_Clear);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.txt_arpCode);
            this.groupControl1.Controls.Add(this.txt_arpDesc);
            this.groupControl1.Controls.Add(this.btn_Save);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(502, 548);
            this.groupControl1.TabIndex = 6;
            this.groupControl1.Text = "Cari Hesap";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelAktarToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(137, 30);
            // 
            // excelAktarToolStripMenuItem
            // 
            this.excelAktarToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("excelAktarToolStripMenuItem.Image")));
            this.excelAktarToolStripMenuItem.Name = "excelAktarToolStripMenuItem";
            this.excelAktarToolStripMenuItem.Size = new System.Drawing.Size(136, 26);
            this.excelAktarToolStripMenuItem.Text = "Excel Aktar";
            this.excelAktarToolStripMenuItem.Click += new System.EventHandler(this.excelAktarToolStripMenuItem_Click);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.btn_Clear.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Clear.Appearance.Options.UseBackColor = true;
            this.btn_Clear.Appearance.Options.UseFont = true;
            this.btn_Clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Clear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btn_Clear.Location = new System.Drawing.Point(116, 0);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btn_Clear.Size = new System.Drawing.Size(45, 29);
            this.btn_Clear.TabIndex = 6;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            this.btn_Clear.MouseHover += new System.EventHandler(this.btn_Clear_MouseHover);
            // 
            // ArpForm
            // 
            this.AcceptButton = this.btn_Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1002, 548);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.gridControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ArpForm.IconOptions.LargeImage")));
            this.MaximizeBox = false;
            this.Name = "ArpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cari Formu";
            this.Load += new System.EventHandler(this.ArpForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_arpCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_arpDesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.TextEdit txt_arpCode;
        private DevExpress.XtraEditors.TextEdit txt_arpDesc;
        private DevExpress.XtraEditors.SimpleButton btn_Save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem excelAktarToolStripMenuItem;
        private DevExpress.XtraEditors.SimpleButton btn_Clear;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

