using DevExpress.XtraGrid.Localization;

namespace LogoJ_Platform_Rest_Test.Helper
{
    internal class MyGridLocalizer:GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id)
        {
            switch (id)
            {
                case GridStringId.FindControlFindButton: return "Bul";
                case GridStringId.FindControlClearButton: return "Temizle";
                case GridStringId.MenuColumnClearFilter: return "Filtreyi Temizle";
                case GridStringId.MenuColumnFilterEditor: return "Filtrele...";
                case GridStringId.MenuColumnFilter: return "Filtre";
                case GridStringId.MenuColumnSortAscending: return "Artan Sırala";
                case GridStringId.MenuColumnSortDescending: return "Azalan Sırala";
                case GridStringId.MenuColumnBestFit: return "Sütunu Uydur";
                case GridStringId.MenuColumnAutoFilterRowShow: return "Otomatik Filtre Satırını Göster";
                case GridStringId.MenuColumnAutoFilterRowHide: return "Otomatik Filtre Satırını Gizle";
                case GridStringId.MenuColumnClearSorting: return "Sıralamayı Temizle";
                case GridStringId.CustomFilterDialogCaption: return "Özel Filtre";
                case GridStringId.CustomFilterDialogRadioAnd: return "Ve";
                case GridStringId.CustomFilterDialogRadioOr: return "Veya";
                case GridStringId.CustomFilterDialogOkButton: return "Tamam";
                case GridStringId.CustomFilterDialogCancelButton: return "İptal";
                case GridStringId.CustomFilterDialog2FieldCheck: return "İkinci Alanı Kullan";
            }
            return base.GetLocalizedString(id);
        }
    }
}