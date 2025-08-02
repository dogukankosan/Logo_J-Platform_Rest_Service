using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace LogoJ_Platform_Rest_Test.Forms
{
    public partial class WaitForm1 : SplashScreen
    {
        public enum SplashScreenCommand
        {
            SetCaption
        }
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public WaitForm1()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 2000-" + DateTime.Now.Year.ToString();
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;
            this.peImage.MouseDown += WaitForm1_MouseDown;
            this.peImage.MouseMove += WaitForm1_MouseMove;
            this.peImage.MouseUp += WaitForm1_MouseUp;
            this.peLogo.MouseDown += WaitForm1_MouseDown;
            this.peLogo.MouseMove += WaitForm1_MouseMove;
            this.peLogo.MouseUp += WaitForm1_MouseUp;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            if (cmd is SplashScreenCommand command)
            {
                switch (command)
                {
                    case SplashScreenCommand.SetCaption:
                        labelStatus.Text = arg?.ToString();
                        break;
                }
            }
        }
        private void WaitForm1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
            Cursor.Current = Cursors.SizeAll;
        }
        private void WaitForm1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }
        private void WaitForm1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            Cursor.Current = Cursors.Default;
        }
        private void WaitForm1_Load(object sender, EventArgs e)
        {

        }
    }
}