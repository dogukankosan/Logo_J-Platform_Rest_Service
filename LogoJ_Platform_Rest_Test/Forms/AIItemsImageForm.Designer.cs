namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class AIItemsImageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIItemsImageForm));
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.rgDomain = new DevExpress.XtraEditors.RadioGroup();
            this.btn_Clear = new DevExpress.XtraEditors.SimpleButton();
            this.btn_ImageAdd = new DevExpress.XtraEditors.SimpleButton();
            this.btn_UnGroup = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
            this.btn_List = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Group = new DevExpress.XtraEditors.SimpleButton();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ramProductToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelAlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyErrrorProductToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.lbl_unpicture = new System.Windows.Forms.Label();
            this.lbl_picture = new System.Windows.Forms.Label();
            this.lbl_ProductCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.btn_ImageUpdate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgDomain.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
            this.groupControl4.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl3
            // 
            this.groupControl3.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl3.CaptionImageOptions.Image")));
            this.groupControl3.Controls.Add(this.rgDomain);
            this.groupControl3.Location = new System.Drawing.Point(1035, 11);
            this.groupControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(182, 128);
            this.groupControl3.TabIndex = 23;
            this.groupControl3.Text = "Tür";
            // 
            // rgDomain
            // 
            this.rgDomain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgDomain.Location = new System.Drawing.Point(2, 33);
            this.rgDomain.Name = "rgDomain";
            this.rgDomain.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Varlıklar"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Malzemeler")});
            this.rgDomain.Size = new System.Drawing.Size(178, 93);
            this.rgDomain.TabIndex = 15;
            this.rgDomain.SelectedIndexChanged += new System.EventHandler(this.rgDomain_SelectedIndexChanged);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;
            this.btn_Clear.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_Clear.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_Clear.Appearance.Options.UseBackColor = true;
            this.btn_Clear.Appearance.Options.UseFont = true;
            this.btn_Clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Clear.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Clear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Clear.ImageOptions.Image")));
            this.btn_Clear.Location = new System.Drawing.Point(2, 149);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(220, 36);
            this.btn_Clear.TabIndex = 3;
            this.btn_Clear.Text = "Temizle";
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // btn_ImageAdd
            // 
            this.btn_ImageAdd.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.btn_ImageAdd.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_ImageAdd.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_ImageAdd.Appearance.Options.UseBackColor = true;
            this.btn_ImageAdd.Appearance.Options.UseFont = true;
            this.btn_ImageAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ImageAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_ImageAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_ImageAdd.ImageOptions.Image")));
            this.btn_ImageAdd.Location = new System.Drawing.Point(2, 73);
            this.btn_ImageAdd.Name = "btn_ImageAdd";
            this.btn_ImageAdd.Size = new System.Drawing.Size(220, 40);
            this.btn_ImageAdd.TabIndex = 1;
            this.btn_ImageAdd.Text = "Toplu Görsel Ekle";
            this.btn_ImageAdd.Click += new System.EventHandler(this.btn_ImageAdd_Click);
            // 
            // btn_UnGroup
            // 
            this.btn_UnGroup.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_UnGroup.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_UnGroup.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_UnGroup.Appearance.Options.UseBackColor = true;
            this.btn_UnGroup.Appearance.Options.UseFont = true;
            this.btn_UnGroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_UnGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_UnGroup.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_UnGroup.ImageOptions.Image")));
            this.btn_UnGroup.Location = new System.Drawing.Point(2, 185);
            this.btn_UnGroup.Name = "btn_UnGroup";
            this.btn_UnGroup.Size = new System.Drawing.Size(220, 35);
            this.btn_UnGroup.TabIndex = 4;
            this.btn_UnGroup.Text = "Grup Çöz";
            this.btn_UnGroup.Click += new System.EventHandler(this.btn_UnGroup_Click);
            // 
            // groupControl4
            // 
            this.groupControl4.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl4.CaptionImageOptions.Image")));
            this.groupControl4.Controls.Add(this.btn_ImageUpdate);
            this.groupControl4.Controls.Add(this.btn_Clear);
            this.groupControl4.Controls.Add(this.btn_ImageAdd);
            this.groupControl4.Controls.Add(this.btn_UnGroup);
            this.groupControl4.Controls.Add(this.btn_List);
            this.groupControl4.Controls.Add(this.btn_Group);
            this.groupControl4.Location = new System.Drawing.Point(732, 330);
            this.groupControl4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupControl4.Name = "groupControl4";
            this.groupControl4.Size = new System.Drawing.Size(224, 257);
            this.groupControl4.TabIndex = 22;
            this.groupControl4.Text = "İşlem";
            // 
            // btn_List
            // 
            this.btn_List.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Question;
            this.btn_List.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_List.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_List.Appearance.Options.UseBackColor = true;
            this.btn_List.Appearance.Options.UseFont = true;
            this.btn_List.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_List.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_List.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_List.ImageOptions.Image")));
            this.btn_List.Location = new System.Drawing.Point(2, 33);
            this.btn_List.Name = "btn_List";
            this.btn_List.Size = new System.Drawing.Size(220, 40);
            this.btn_List.TabIndex = 0;
            this.btn_List.Text = "Listeyi Yenile";
            this.btn_List.Click += new System.EventHandler(this.btn_List_Click);
            // 
            // btn_Group
            // 
            this.btn_Group.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btn_Group.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_Group.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_Group.Appearance.Options.UseBackColor = true;
            this.btn_Group.Appearance.Options.UseFont = true;
            this.btn_Group.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Group.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btn_Group.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Group.ImageOptions.Image")));
            this.btn_Group.Location = new System.Drawing.Point(2, 220);
            this.btn_Group.Name = "btn_Group";
            this.btn_Group.Size = new System.Drawing.Size(220, 35);
            this.btn_Group.TabIndex = 5;
            this.btn_Group.Text = "Gruplandır";
            this.btn_Group.Click += new System.EventHandler(this.btn_Group_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeToolStripMenuItem.Image")));
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.removeToolStripMenuItem.Text = "Seçili Görselleri Kaldır";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // exportImageToolStripMenuItem
            // 
            this.exportImageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportImageToolStripMenuItem.Image")));
            this.exportImageToolStripMenuItem.Name = "exportImageToolStripMenuItem";
            this.exportImageToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.exportImageToolStripMenuItem.Text = "Seçili Görselleri Dışarı Al";
            this.exportImageToolStripMenuItem.Click += new System.EventHandler(this.exportImageToolStripMenuItem_Click);
            // 
            // imageExportToolStripMenuItem
            // 
            this.imageExportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("imageExportToolStripMenuItem.Image")));
            this.imageExportToolStripMenuItem.Name = "imageExportToolStripMenuItem";
            this.imageExportToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.imageExportToolStripMenuItem.Text = "Tüm Görselleri Dışarı Al";
            this.imageExportToolStripMenuItem.Click += new System.EventHandler(this.imageExportToolStripMenuItem_Click);
            // 
            // ramProductToolStripMenuItem
            // 
            this.ramProductToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ramProductToolStripMenuItem.Image")));
            this.ramProductToolStripMenuItem.Name = "ramProductToolStripMenuItem";
            this.ramProductToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.ramProductToolStripMenuItem.Text = "Seçili Malzeme Kodunu Kopyala";
            this.ramProductToolStripMenuItem.Click += new System.EventHandler(this.ramProductToolStripMenuItem_Click);
            // 
            // excelAlToolStripMenuItem
            // 
            this.excelAlToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("excelAlToolStripMenuItem.Image")));
            this.excelAlToolStripMenuItem.Name = "excelAlToolStripMenuItem";
            this.excelAlToolStripMenuItem.Size = new System.Drawing.Size(246, 26);
            this.excelAlToolStripMenuItem.Text = "Excel Al";
            this.excelAlToolStripMenuItem.Click += new System.EventHandler(this.excelAlToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelAlToolStripMenuItem,
            this.ramProductToolStripMenuItem,
            this.imageExportToolStripMenuItem,
            this.exportImageToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(247, 156);
            // 
            // groupControl2
            // 
            this.groupControl2.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl2.CaptionImageOptions.Image")));
            this.groupControl2.Controls.Add(this.listBoxControl1);
            this.groupControl2.Location = new System.Drawing.Point(730, 143);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(487, 183);
            this.groupControl2.TabIndex = 21;
            this.groupControl2.Text = "Görsel Aktarım Log";
            // 
            // listBoxControl1
            // 
            this.listBoxControl1.ContextMenuStrip = this.contextMenuStrip2;
            this.listBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxControl1.HorizontalScrollbar = true;
            this.listBoxControl1.Location = new System.Drawing.Point(2, 33);
            this.listBoxControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listBoxControl1.Name = "listBoxControl1";
            this.listBoxControl1.Size = new System.Drawing.Size(483, 148);
            this.listBoxControl1.TabIndex = 8;
            this.listBoxControl1.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.listBoxControl1_DrawItem);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyErrrorProductToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(158, 30);
            // 
            // copyErrrorProductToolStripMenuItem
            // 
            this.copyErrrorProductToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyErrrorProductToolStripMenuItem.Image")));
            this.copyErrrorProductToolStripMenuItem.Name = "copyErrrorProductToolStripMenuItem";
            this.copyErrrorProductToolStripMenuItem.Size = new System.Drawing.Size(157, 26);
            this.copyErrrorProductToolStripMenuItem.Text = "Hatayı Kopyala";
            this.copyErrrorProductToolStripMenuItem.Click += new System.EventHandler(this.copyErrrorProductToolStripMenuItem_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.CaptionImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("groupControl1.CaptionImageOptions.Image")));
            this.groupControl1.Controls.Add(this.lbl_unpicture);
            this.groupControl1.Controls.Add(this.lbl_picture);
            this.groupControl1.Controls.Add(this.lbl_ProductCount);
            this.groupControl1.Controls.Add(this.label3);
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Location = new System.Drawing.Point(730, 11);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(301, 128);
            this.groupControl1.TabIndex = 20;
            this.groupControl1.Text = "Malzeme Bilgisi";
            // 
            // lbl_unpicture
            // 
            this.lbl_unpicture.AutoSize = true;
            this.lbl_unpicture.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.lbl_unpicture.Location = new System.Drawing.Point(210, 89);
            this.lbl_unpicture.Name = "lbl_unpicture";
            this.lbl_unpicture.Size = new System.Drawing.Size(16, 17);
            this.lbl_unpicture.TabIndex = 9;
            this.lbl_unpicture.Text = "0";
            // 
            // lbl_picture
            // 
            this.lbl_picture.AutoSize = true;
            this.lbl_picture.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.lbl_picture.Location = new System.Drawing.Point(182, 62);
            this.lbl_picture.Name = "lbl_picture";
            this.lbl_picture.Size = new System.Drawing.Size(16, 17);
            this.lbl_picture.TabIndex = 9;
            this.lbl_picture.Text = "0";
            // 
            // lbl_ProductCount
            // 
            this.lbl_ProductCount.AutoSize = true;
            this.lbl_ProductCount.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.lbl_ProductCount.Location = new System.Drawing.Point(110, 38);
            this.lbl_ProductCount.Name = "lbl_ProductCount";
            this.lbl_ProductCount.Size = new System.Drawing.Size(16, 17);
            this.lbl_ProductCount.TabIndex = 8;
            this.lbl_ProductCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label3.Location = new System.Drawing.Point(8, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Görseli Olmayan Malzeme Sayısı:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label2.Location = new System.Drawing.Point(8, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Görsel Olan Malzeme Sayısı:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.25F);
            this.label1.Location = new System.Drawing.Point(8, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Malzeme Sayısı:";
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // gridControl1
            // 
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(724, 610);
            this.gridControl1.TabIndex = 19;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // btn_ImageUpdate
            // 
            this.btn_ImageUpdate.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Warning;
            this.btn_ImageUpdate.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F, System.Drawing.FontStyle.Bold);
            this.btn_ImageUpdate.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.btn_ImageUpdate.Appearance.Options.UseBackColor = true;
            this.btn_ImageUpdate.Appearance.Options.UseFont = true;
            this.btn_ImageUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ImageUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ImageUpdate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btn_ImageUpdate.Location = new System.Drawing.Point(2, 113);
            this.btn_ImageUpdate.Name = "btn_ImageUpdate";
            this.btn_ImageUpdate.Size = new System.Drawing.Size(220, 36);
            this.btn_ImageUpdate.TabIndex = 2;
            this.btn_ImageUpdate.Text = "Toplu Görsel Güncelle";
            this.btn_ImageUpdate.Click += new System.EventHandler(this.btn_ImageUpdate_Click);
            // 
            // AIItemsImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1259, 610);
            this.Controls.Add(this.groupControl3);
            this.Controls.Add(this.groupControl4);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.gridControl1);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("AIItemsImageForm.IconOptions.LargeImage")));
            this.Name = "AIItemsImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AI Malzeme Görsel";
            this.Load += new System.EventHandler(this.AIItemsImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgDomain.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
            this.groupControl4.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl3;
        private DevExpress.XtraEditors.RadioGroup rgDomain;
        private DevExpress.XtraEditors.SimpleButton btn_Clear;
        private DevExpress.XtraEditors.SimpleButton btn_ImageAdd;
        private DevExpress.XtraEditors.SimpleButton btn_UnGroup;
        private DevExpress.XtraEditors.GroupControl groupControl4;
        private DevExpress.XtraEditors.SimpleButton btn_List;
        private DevExpress.XtraEditors.SimpleButton btn_Group;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ramProductToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelAlToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.ListBoxControl listBoxControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem copyErrrorProductToolStripMenuItem;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.Label lbl_unpicture;
        private System.Windows.Forms.Label lbl_picture;
        private System.Windows.Forms.Label lbl_ProductCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraEditors.SimpleButton btn_ImageUpdate;
    }
}