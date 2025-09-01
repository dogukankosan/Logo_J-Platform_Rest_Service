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
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.lbl_credit = new System.Windows.Forms.Label();
            this.lbl_debit = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gaugeControl1 = new DevExpress.XtraGauges.Win.GaugeControl();
            this.digitalGauge5 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge();
            this.digitalBackgroundLayerComponent1 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.gaugeControl4 = new DevExpress.XtraGauges.Win.GaugeControl();
            this.digitalGauge6 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge();
            this.digitalBackgroundLayerComponent7 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.digitalBackgroundLayerComponent2 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.digitalGauge1 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge();
            this.digitalBackgroundLayerComponent3 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.digitalGauge2 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge();
            this.digitalBackgroundLayerComponent4 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btn_Clear = new DevExpress.XtraEditors.SimpleButton();
            this.btn_ungroup = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Group = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControl1.Location = new System.Drawing.Point(-1, 270);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1454, 471);
            this.gridControl1.TabIndex = 6;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.DetailHeight = 431;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btn_TempExcel);
            this.groupControl1.Controls.Add(this.btn_Excel);
            this.groupControl1.Controls.Add(this.btn_Transfer);
            this.groupControl1.Location = new System.Drawing.Point(14, 7);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(556, 100);
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
            this.btn_TempExcel.Location = new System.Drawing.Point(364, 44);
            this.btn_TempExcel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_TempExcel.Name = "btn_TempExcel";
            this.btn_TempExcel.Size = new System.Drawing.Size(177, 38);
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
            this.btn_Excel.Location = new System.Drawing.Point(194, 44);
            this.btn_Excel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(163, 38);
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
            this.btn_Transfer.Location = new System.Drawing.Point(20, 44);
            this.btn_Transfer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Transfer.Name = "btn_Transfer";
            this.btn_Transfer.Size = new System.Drawing.Size(163, 38);
            this.btn_Transfer.TabIndex = 1;
            this.btn_Transfer.Text = "Aktar";
            this.btn_Transfer.Click += new System.EventHandler(this.btn_Transfer_Click);
            // 
            // groupControl3
            // 
            this.groupControl3.Controls.Add(this.lbl_credit);
            this.groupControl3.Controls.Add(this.lbl_debit);
            this.groupControl3.Controls.Add(this.label5);
            this.groupControl3.Controls.Add(this.gaugeControl1);
            this.groupControl3.Controls.Add(this.gaugeControl4);
            this.groupControl3.Controls.Add(this.label4);
            this.groupControl3.Controls.Add(this.label3);
            this.groupControl3.Controls.Add(this.label2);
            this.groupControl3.Location = new System.Drawing.Point(14, 114);
            this.groupControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.Size = new System.Drawing.Size(556, 129);
            this.groupControl3.TabIndex = 10;
            this.groupControl3.Text = "Aktarım Bilgisi";
            // 
            // lbl_credit
            // 
            this.lbl_credit.AutoSize = true;
            this.lbl_credit.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.lbl_credit.Location = new System.Drawing.Point(402, 65);
            this.lbl_credit.Name = "lbl_credit";
            this.lbl_credit.Size = new System.Drawing.Size(51, 25);
            this.lbl_credit.TabIndex = 24;
            this.lbl_credit.Text = "0,00";
            // 
            // lbl_debit
            // 
            this.lbl_debit.AutoSize = true;
            this.lbl_debit.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.lbl_debit.Location = new System.Drawing.Point(419, 33);
            this.lbl_debit.Name = "lbl_debit";
            this.lbl_debit.Size = new System.Drawing.Size(51, 25);
            this.lbl_debit.TabIndex = 23;
            this.lbl_debit.Text = "0,00";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label5.Location = new System.Drawing.Point(17, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 25);
            this.label5.TabIndex = 21;
            this.label5.Text = "Fiş Sayısı:";
            // 
            // gaugeControl1
            // 
            this.gaugeControl1.AutoLayout = false;
            this.gaugeControl1.Gauges.AddRange(new DevExpress.XtraGauges.Base.IGauge[] {
            this.digitalGauge5});
            this.gaugeControl1.Location = new System.Drawing.Point(22, 62);
            this.gaugeControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gaugeControl1.Name = "gaugeControl1";
            this.gaugeControl1.Size = new System.Drawing.Size(104, 55);
            this.gaugeControl1.TabIndex = 20;
            // 
            // digitalGauge5
            // 
            this.digitalGauge5.AppearanceOff.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#0FD4F2FF");
            this.digitalGauge5.AppearanceOn.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#D4F2FF");
            this.digitalGauge5.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent[] {
            this.digitalBackgroundLayerComponent1});
            this.digitalGauge5.Bounds = new System.Drawing.Rectangle(-7, -1, 94, 53);
            this.digitalGauge5.DigitCount = 5;
            this.digitalGauge5.DisplayMode = DevExpress.XtraGauges.Core.Model.DigitalGaugeDisplayMode.SevenSegment;
            this.digitalGauge5.Name = "digitalGauge5";
            this.digitalGauge5.ProportionalStretch = false;
            // 
            // digitalBackgroundLayerComponent1
            // 
            this.digitalBackgroundLayerComponent1.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(251.375F, 106.075F);
            this.digitalBackgroundLayerComponent1.Name = "bg1";
            this.digitalBackgroundLayerComponent1.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style3;
            this.digitalBackgroundLayerComponent1.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent1.ZOrder = 1000;
            // 
            // gaugeControl4
            // 
            this.gaugeControl4.AutoLayout = false;
            this.gaugeControl4.Gauges.AddRange(new DevExpress.XtraGauges.Base.IGauge[] {
            this.digitalGauge6});
            this.gaugeControl4.Location = new System.Drawing.Point(146, 60);
            this.gaugeControl4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gaugeControl4.Name = "gaugeControl4";
            this.gaugeControl4.Size = new System.Drawing.Size(99, 57);
            this.gaugeControl4.TabIndex = 22;
            // 
            // digitalGauge6
            // 
            this.digitalGauge6.AppearanceOff.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#0FD4F2FF");
            this.digitalGauge6.AppearanceOn.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#D4F2FF");
            this.digitalGauge6.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent[] {
            this.digitalBackgroundLayerComponent7});
            this.digitalGauge6.Bounds = new System.Drawing.Rectangle(-7, -1, 92, 53);
            this.digitalGauge6.DigitCount = 5;
            this.digitalGauge6.DisplayMode = DevExpress.XtraGauges.Core.Model.DigitalGaugeDisplayMode.SevenSegment;
            this.digitalGauge6.Name = "digitalGauge6";
            this.digitalGauge6.ProportionalStretch = false;
            // 
            // digitalBackgroundLayerComponent7
            // 
            this.digitalBackgroundLayerComponent7.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(251.375F, 106.075F);
            this.digitalBackgroundLayerComponent7.Name = "bg1";
            this.digitalBackgroundLayerComponent7.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style3;
            this.digitalBackgroundLayerComponent7.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent7.ZOrder = 1000;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label4.Location = new System.Drawing.Point(274, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 25);
            this.label4.TabIndex = 15;
            this.label4.Text = "Alacak Toplam:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label3.Location = new System.Drawing.Point(274, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 25);
            this.label3.TabIndex = 14;
            this.label3.Text = "Borç Toplam:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label2.Location = new System.Drawing.Point(141, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 25);
            this.label2.TabIndex = 13;
            this.label2.Text = "Satır Sayısı:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 25);
            this.label1.TabIndex = 17;
            this.label1.Text = "Aktarım Log:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 60);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(243, 163);
            this.richTextBox1.TabIndex = 16;
            this.richTextBox1.Text = "";
            // 
            // digitalBackgroundLayerComponent2
            // 
            this.digitalBackgroundLayerComponent2.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(251.375F, 106.075F);
            this.digitalBackgroundLayerComponent2.Name = "bg1";
            this.digitalBackgroundLayerComponent2.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style3;
            this.digitalBackgroundLayerComponent2.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent2.ZOrder = 1000;
            // 
            // digitalGauge1
            // 
            this.digitalGauge1.AppearanceOff.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#0FFF5000");
            this.digitalGauge1.AppearanceOn.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#FF5000");
            this.digitalGauge1.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent[] {
            this.digitalBackgroundLayerComponent3});
            this.digitalGauge1.Bounds = new System.Drawing.Rectangle(-7, -1, 93, 53);
            this.digitalGauge1.DigitCount = 5;
            this.digitalGauge1.DisplayMode = DevExpress.XtraGauges.Core.Model.DigitalGaugeDisplayMode.SevenSegment;
            this.digitalGauge1.Name = "digitalGauge1";
            this.digitalGauge1.ProportionalStretch = false;
            // 
            // digitalBackgroundLayerComponent3
            // 
            this.digitalBackgroundLayerComponent3.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(251.375F, 106.075F);
            this.digitalBackgroundLayerComponent3.Name = "bg1";
            this.digitalBackgroundLayerComponent3.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style3;
            this.digitalBackgroundLayerComponent3.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent3.ZOrder = 1000;
            // 
            // digitalGauge2
            // 
            this.digitalGauge2.AppearanceOff.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#0FFF5000");
            this.digitalGauge2.AppearanceOn.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#FF5000");
            this.digitalGauge2.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent[] {
            this.digitalBackgroundLayerComponent4});
            this.digitalGauge2.Bounds = new System.Drawing.Rectangle(-7, -1, 93, 53);
            this.digitalGauge2.DigitCount = 5;
            this.digitalGauge2.DisplayMode = DevExpress.XtraGauges.Core.Model.DigitalGaugeDisplayMode.SevenSegment;
            this.digitalGauge2.Name = "digitalGauge2";
            this.digitalGauge2.ProportionalStretch = false;
            // 
            // digitalBackgroundLayerComponent4
            // 
            this.digitalBackgroundLayerComponent4.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(251.375F, 106.075F);
            this.digitalBackgroundLayerComponent4.Name = "bg1";
            this.digitalBackgroundLayerComponent4.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style3;
            this.digitalBackgroundLayerComponent4.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent4.ZOrder = 1000;
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.btn_Clear);
            this.groupControl2.Controls.Add(this.btn_ungroup);
            this.groupControl2.Controls.Add(this.btn_Group);
            this.groupControl2.Controls.Add(this.label1);
            this.groupControl2.Controls.Add(this.richTextBox1);
            this.groupControl2.Location = new System.Drawing.Point(600, 7);
            this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(494, 236);
            this.groupControl2.TabIndex = 18;
            this.groupControl2.Text = "Aktarım Yönetim";
            // 
            // btn_Clear
            // 
            this.btn_Clear.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_Clear.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.btn_Clear.Appearance.ForeColor = System.Drawing.Color.White;
            this.btn_Clear.Appearance.Options.UseBackColor = true;
            this.btn_Clear.Appearance.Options.UseFont = true;
            this.btn_Clear.Appearance.Options.UseForeColor = true;
            this.btn_Clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Clear.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_Clear.ImageOptions.SvgImage")));
            this.btn_Clear.Location = new System.Drawing.Point(257, 127);
            this.btn_Clear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(217, 38);
            this.btn_Clear.TabIndex = 20;
            this.btn_Clear.Text = "Temizle";
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // btn_ungroup
            // 
            this.btn_ungroup.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btn_ungroup.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.btn_ungroup.Appearance.Options.UseBackColor = true;
            this.btn_ungroup.Appearance.Options.UseFont = true;
            this.btn_ungroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ungroup.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_ungroup.ImageOptions.SvgImage")));
            this.btn_ungroup.Location = new System.Drawing.Point(257, 81);
            this.btn_ungroup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_ungroup.Name = "btn_ungroup";
            this.btn_ungroup.Size = new System.Drawing.Size(217, 38);
            this.btn_ungroup.TabIndex = 19;
            this.btn_ungroup.Text = "Fişlerin Grubu Çöz";
            this.btn_ungroup.Click += new System.EventHandler(this.btn_ungroup_Click);
            // 
            // btn_Group
            // 
            this.btn_Group.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btn_Group.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.btn_Group.Appearance.Options.UseBackColor = true;
            this.btn_Group.Appearance.Options.UseFont = true;
            this.btn_Group.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Group.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_Group.ImageOptions.SvgImage")));
            this.btn_Group.Location = new System.Drawing.Point(257, 36);
            this.btn_Group.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Group.Name = "btn_Group";
            this.btn_Group.Size = new System.Drawing.Size(217, 38);
            this.btn_Group.TabIndex = 18;
            this.btn_Group.Text = "Fişleri Grupla";
            this.btn_Group.Click += new System.EventHandler(this.btn_Group_Click);
            // 
            // DayGlSlipForm
            // 
            this.Appearance.ForeColor = System.Drawing.Color.White;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1451, 740);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl3);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.groupControl1);
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("DayGlSlipForm.IconOptions.SvgImage")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "DayGlSlipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Günlük / Mahsup Aktar";
            this.Load += new System.EventHandler(this.DayGlSlipForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            this.groupControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            this.groupControl2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btn_TempExcel;
        private DevExpress.XtraEditors.SimpleButton btn_Excel;
        private DevExpress.XtraEditors.SimpleButton btn_Transfer;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent2;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge digitalGauge1;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent3;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge digitalGauge2;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent4;
        private DevExpress.XtraGauges.Win.GaugeControl gaugeControl1;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge digitalGauge5;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent1;
        private DevExpress.XtraGauges.Win.GaugeControl gaugeControl4;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge digitalGauge6;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbl_credit;
        private System.Windows.Forms.Label lbl_debit;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.SimpleButton btn_ungroup;
        private DevExpress.XtraEditors.SimpleButton btn_Group;
        private DevExpress.XtraEditors.SimpleButton btn_Clear;
    }
}