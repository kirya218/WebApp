namespace WebApp.Tools.Helpers
{
    internal class OdropdownTag
    {
        public string InLabel { get; set; }

        public string Caption { get; set; }

        public bool AutoSelectFirst { get; set; }

        public bool? NoSelClose { get; set; }

        public string MinWidth { get; set; }

        public string SearchFunc { get; set; }

        public string Url { get; set; }

        public string ItemFunc { get; set; }

        public string CaptionFunc { get; set; }

        public string Key { get; set; }

        public string PopupClass { get; set; }

        public bool OpenOnHover { get; set; }

        public int? Asmi { get; set; }

        public bool? ClearCacheOnLoad { get; set; }

        public int? PopupMaxHeight { get; set; }

        public bool CollapseNodes { get; set; }

        public bool NoCache { get; set; }

        public int PopupMinWidth { get; set; }

        public int PopupMaxWidth { get; set; }
        
        public object[] Favs { get; set; }

        public object[] FrozenFavs { get; set; }

        public int FavCount { get; set; }

        public bool ClearBtn { get; set; }

        public bool Submenu { get; set; }
    }
}