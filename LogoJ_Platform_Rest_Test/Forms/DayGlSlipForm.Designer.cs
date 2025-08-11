namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class DayGlSlipForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DayGlSlipForm));
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btn_TempExcel = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Excel = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Transfer = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridControl1.Location = new System.Drawing.Point(0, 139);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(965, 388);
            this.gridControl1.TabIndex = 6;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btn_TempExcel);
            this.groupControl1.Controls.Add(this.btn_Excel);
            this.groupControl1.Controls.Add(this.btn_Transfer);
            this.groupControl1.Location = new System.Drawing.Point(12, 6);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(477, 92);
            this.groupControl1.TabIndex = 5;
            this.groupControl1.Text = "Aktarım Panel";
            // 
            // btn_TempExcel
            // 
            this.btn_TempExcel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Primary;
            this.btn_TempExcel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_TempExcel.Appearance.Options.UseBackColor = true;
            this.btn_TempExcel.Appearance.Options.UseFont = true;
            this.btn_TempExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_TempExcel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_TempExcel.ImageOptions.SvgImage")));
            this.btn_TempExcel.Location = new System.Drawing.Point(312, 38);
            this.btn_TempExcel.Name = "btn_TempExcel";
            this.btn_TempExcel.Size = new System.Drawing.Size(152, 38);
            this.btn_TempExcel.TabIndex = 3;
            this.btn_TempExcel.Text = "Örnek Excel";
            this.btn_TempExcel.Click += new System.EventHandler(this.btn_TempExcel_Click);
            // 
            // btn_Excel
            // 
            this.btn_Excel.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Warning;
            this.btn_Excel.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Excel.Appearance.Options.UseBackColor = true;
            this.btn_Excel.Appearance.Options.UseFont = true;
            this.btn_Excel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Excel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Excel.ImageOptions.Image")));
            this.btn_Excel.Location = new System.Drawing.Point(166, 36);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(140, 43);
            this.btn_Excel.TabIndex = 2;
            this.btn_Excel.Text = "Excel Getir";
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // btn_Transfer
            // 
            this.btn_Transfer.Appearance.BackColor = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
            this.btn_Transfer.Appearance.Font = new System.Drawing.Font("Tahoma", 15.25F);
            this.btn_Transfer.Appearance.Options.UseBackColor = true;
            this.btn_Transfer.Appearance.Options.UseFont = true;
            this.btn_Transfer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Transfer.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Transfer.ImageOptions.Image")));
            this.btn_Transfer.Location = new System.Drawing.Point(17, 36);
            this.btn_Transfer.Name = "btn_Transfer";
            this.btn_Transfer.Size = new System.Drawing.Size(140, 43);
            this.btn_Transfer.TabIndex = 1;
            this.btn_Transfer.Text = "Aktar";
            this.btn_Transfer.Click += new System.EventHandler(this.btn_Transfer_Click);
            // 
            // DayGlSlipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 527);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("DayGlSlipForm.IconOptions.SvgImage")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "DayGlSlipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Muhasebese Fişi Günlük Aktar";
            this.Load += new System.EventHandler(this.DayGlSlipForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btn_TempExcel;
        private DevExpress.XtraEditors.SimpleButton btn_Excel;
        private DevExpress.XtraEditors.SimpleButton btn_Transfer;
    }
}