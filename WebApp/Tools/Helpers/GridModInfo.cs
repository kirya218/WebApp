namespace WebApp.Tools.Helpers
{
    internal class GridModInfo
    {
        public bool InfiniteScroll { get; set; }

        public bool PageInfo { get; set; }

        public bool PageSize { get; set; }

        public bool ColumnsSelector { get; set; }

        public bool AutoMiniPager { get; set; }

        public bool InlineEdit { get; set; }

        public string CreateUrl { get; set; }

        public string EditUrl { get; set; }

        public bool ColumnsAutohide { get; set; }

        public bool MiniPager { get; set; }

        public bool OneRow { get; set; }

        public IList<string> CustomMods { get; set; }

        public bool Loading { get; set; }

        public bool MovableRows { get; set; }

        public string[] GridIds { get; set; }

        public bool RelOnSav { get; set; }

        public bool KeyNav { get; set; }

        public bool RowClickEdit { get; set; }

        public bool LdngDisb { get; set; }

        public bool NoEmpMsg { get; set; }

        public string modFunc { get; set; }

        public bool CustomRender { get; set; }

        public string EmptyMessage { get; set; }

        public GridFilterOpt FilterOpt { get; set; }

        public int FzLeft { get; set; }

        public int FzRight { get; set; }

        public int Minsw { get; set; }

        public bool Batch { get; set; }

        public string ClientSave { get; set; }

        public bool MoreBtn { get; set; }
    }
}