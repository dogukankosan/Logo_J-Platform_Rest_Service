using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class GridViewDesigner
    {
        internal static void CustomizeGrid(GridView view)
        {
            view.OptionsFind.FindNullPrompt = "Aramak için buraya yazın...";
            view.OptionsBehavior.Editable = false;
            view.OptionsBehavior.ReadOnly = true;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.OptionsSelection.MultiSelect = false;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ShowHorizontalLines = DefaultBoolean.True;
            view.OptionsView.ShowVerticalLines = DefaultBoolean.True;
            view.OptionsView.EnableAppearanceEvenRow = true;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            view.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            view.Appearance.HeaderPanel.BackColor = Color.MidnightBlue;
            view.Appearance.HeaderPanel.ForeColor = Color.Black;
            view.Appearance.Row.Font = new Font("Segoe UI", 9.5F);
            view.Appearance.EvenRow.BackColor = Color.Beige;
            view.Appearance.EvenRow.BackColor2 = Color.LightGoldenrodYellow;
            view.Appearance.EvenRow.GradientMode = LinearGradientMode.Horizontal;
            view.Appearance.OddRow.BackColor = Color.WhiteSmoke;
            view.Appearance.OddRow.BackColor2 = Color.BlanchedAlmond;
            view.Appearance.OddRow.GradientMode = LinearGradientMode.Horizontal;
            view.Appearance.FocusedRow.BackColor = Color.LightSkyBlue;
            view.Appearance.FocusedRow.BackColor2 = Color.AliceBlue;
            view.Appearance.FocusedRow.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            view.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            view.BestFitColumns();
        }
    }
}