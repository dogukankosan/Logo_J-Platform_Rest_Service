namespace LogoJ_Platform_Rest_Test.Forms
{
    partial class HomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.userName_Company = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator2 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.accordionControlElement1 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_restServiceSettings = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_SQLSettings = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_Logs = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_SQLiteCommand = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator1 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.accordionControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_SlipForm = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_DayGLSlip = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlElement3 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.btn_Thema = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.popupMenu2 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.skinBarSubItem2 = new DevExpress.XtraBars.SkinBarSubItem();
            this.fluentFormDefaultManager1 = new DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager(this.components);
            this.skinPaletteDropDownButtonItem1 = new DevExpress.XtraBars.SkinPaletteDropDownButtonItem();
            this.skinBarSubItem1 = new DevExpress.XtraBars.SkinBarSubItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentFormDefaultManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // accordionControl1
            // 
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.userName_Company,
            this.accordionControlSeparator2,
            this.accordionControlElement1,
            this.accordionControlSeparator1,
            this.accordionControlElement2,
            this.accordionControlElement3});
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Fluent;
            this.accordionControl1.Size = new System.Drawing.Size(245, 606);
            this.accordionControl1.TabIndex = 0;
            this.accordionControl1.Text = "accordionControl1";
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // userName_Company
            // 
            this.userName_Company.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image, DevExpress.XtraBars.Navigation.HeaderElementAlignment.Right)});
            this.userName_Company.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
            this.userName_Company.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("userName_Company.ImageOptions.Image")));
            this.userName_Company.ImageOptions.ImageIndex = 0;
            this.userName_Company.Name = "userName_Company";
            this.userName_Company.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.userName_Company.Text = "Hoşgeldiniz.";
            this.userName_Company.Click += new System.EventHandler(this.userName_Company_Click);
            // 
            // accordionControlSeparator2
            // 
            this.accordionControlSeparator2.Name = "accordionControlSeparator2";
            // 
            // accordionControlElement1
            // 
            this.accordionControlElement1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.btn_restServiceSettings,
            this.btn_SQLSettings,
            this.btn_Logs,
            this.btn_SQLiteCommand});
            this.accordionControlElement1.Expanded = true;
            this.accordionControlElement1.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons)});
            this.accordionControlElement1.Name = "accordionControlElement1";
            this.accordionControlElement1.Text = "Admin Ayarlar";
            // 
            // btn_restServiceSettings
            // 
            this.btn_restServiceSettings.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_restServiceSettings.ImageOptions.Image")));
            this.btn_restServiceSettings.Name = "btn_restServiceSettings";
            this.btn_restServiceSettings.Text = "Rest Servis Ayarları";
            this.btn_restServiceSettings.Click += new System.EventHandler(this.btn_restServiceSettings_Click);
            // 
            // btn_SQLSettings
            // 
            this.btn_SQLSettings.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_SQLSettings.ImageOptions.Image")));
            this.btn_SQLSettings.Name = "btn_SQLSettings";
            this.btn_SQLSettings.Text = "SQL Ayarları";
            this.btn_SQLSettings.Click += new System.EventHandler(this.btn_SQLSettings_Click);
            // 
            // btn_Logs
            // 
            this.btn_Logs.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Logs.ImageOptions.Image")));
            this.btn_Logs.Name = "btn_Logs";
            this.btn_Logs.Text = "Hata Kayıtları";
            this.btn_Logs.Click += new System.EventHandler(this.btn_Logs_Click);
            // 
            // btn_SQLiteCommand
            // 
            this.btn_SQLiteCommand.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_SQLiteCommand.ImageOptions.Image")));
            this.btn_SQLiteCommand.Name = "btn_SQLiteCommand";
            this.btn_SQLiteCommand.Text = "SQLite Komut";
            this.btn_SQLiteCommand.Click += new System.EventHandler(this.btn_SQLiteCommand_Click);
            // 
            // accordionControlSeparator1
            // 
            this.accordionControlSeparator1.Name = "accordionControlSeparator1";
            // 
            // accordionControlElement2
            // 
            this.accordionControlElement2.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.btn_SlipForm,
            this.btn_DayGLSlip});
            this.accordionControlElement2.Expanded = true;
            this.accordionControlElement2.Name = "accordionControlElement2";
            this.accordionControlElement2.Text = "Formlar";
            // 
            // btn_SlipForm
            // 
            this.btn_SlipForm.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_SlipForm.ImageOptions.Image")));
            this.btn_SlipForm.Name = "btn_SlipForm";
            this.btn_SlipForm.Text = "Muhasebe Mahsup Fişi  Aktar";
            this.btn_SlipForm.Click += new System.EventHandler(this.btn_SlipForm_Click);
            // 
            // btn_DayGLSlip
            // 
            this.btn_DayGLSlip.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_DayGLSlip.ImageOptions.SvgImage")));
            this.btn_DayGLSlip.Name = "btn_DayGLSlip";
            this.btn_DayGLSlip.Text = "Muhasebe Günlük Fişi Aktar";
            this.btn_DayGLSlip.Click += new System.EventHandler(this.btn_DayGLSlip_Click);
            // 
            // accordionControlElement3
            // 
            this.accordionControlElement3.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.btn_Thema});
            this.accordionControlElement3.Expanded = true;
            this.accordionControlElement3.Name = "accordionControlElement3";
            this.accordionControlElement3.Text = "Kullanıcı Ayarları";
            // 
            // btn_Thema
            // 
            this.btn_Thema.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btn_Thema.ImageOptions.Image")));
            this.btn_Thema.Name = "btn_Thema";
            this.btn_Thema.Text = "Temalar";
            this.btn_Thema.Click += new System.EventHandler(this.btn_Thema_Click_1);
            // 
            // popupMenu2
            // 
            this.popupMenu2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.skinBarSubItem2)});
            this.popupMenu2.Manager = this.fluentFormDefaultManager1;
            this.popupMenu2.Name = "popupMenu2";
            // 
            // skinBarSubItem2
            // 
            this.skinBarSubItem2.Caption = "Temalar";
            this.skinBarSubItem2.Id = 2;
            this.skinBarSubItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("skinBarSubItem2.ImageOptions.Image")));
            this.skinBarSubItem2.Name = "skinBarSubItem2";
            // 
            // fluentFormDefaultManager1
            // 
            this.fluentFormDefaultManager1.DockingEnabled = false;
            this.fluentFormDefaultManager1.Form = this;
            this.fluentFormDefaultManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.skinPaletteDropDownButtonItem1,
            this.skinBarSubItem1,
            this.skinBarSubItem2});
            this.fluentFormDefaultManager1.MaxItemId = 3;
            // 
            // skinPaletteDropDownButtonItem1
            // 
            this.skinPaletteDropDownButtonItem1.Id = 0;
            this.skinPaletteDropDownButtonItem1.Name = "skinPaletteDropDownButtonItem1";
            // 
            // skinBarSubItem1
            // 
            this.skinBarSubItem1.Caption = "Temalaer";
            this.skinBarSubItem1.Id = 1;
            this.skinBarSubItem1.Name = "skinBarSubItem1";
            // 
            // panelControl1
            // 
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(245, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1172, 606);
            this.panelControl1.TabIndex = 1;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1417, 606);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.accordionControl1);
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("HomeForm.IconOptions.Image")));
            this.MaximizeBox = false;
            this.Name = "HomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asyen Yazılım J-Platform Rest Aktarım";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HomeForm_FormClosing);
            this.Load += new System.EventHandler(this.HomeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentFormDefaultManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_restServiceSettings;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_SQLSettings;
        private DevExpress.XtraBars.PopupMenu popupMenu2;
        private DevExpress.XtraBars.SkinBarSubItem skinBarSubItem2;
        private DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager fluentFormDefaultManager1;
        private DevExpress.XtraBars.SkinPaletteDropDownButtonItem skinPaletteDropDownButtonItem1;
        private DevExpress.XtraBars.SkinBarSubItem skinBarSubItem1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_Logs;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_SQLiteCommand;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_SlipForm;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_DayGLSlip;
        private DevExpress.XtraBars.Navigation.AccordionControlElement userName_Company;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement3;
        private DevExpress.XtraBars.Navigation.AccordionControlElement btn_Thema;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator2;
    }
}