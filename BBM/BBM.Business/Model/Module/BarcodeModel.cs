using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Model.Module
{
    public class BarcodeModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Tmp { get; set; }
        public int MarginLeftPage { get; set; }
        public int MarginLeftItem { get; set; }
        public int HeightItem { get; set; }
        public int WidthItem { get; set; }
        public int HeightFlag { get; set; }
        public int WidthFlag { get; set; }
        public int PageSize { get; set; }
        public int ItemInLine { get; set; }
        public int FontSize { get; set; }
        public int PaddingItem { get; set; }
        public int PaddingTopPage { get; set; }
        public int PaddingBotPage { get; set; }
        public int PaddingLeftPage { get; set; }
        public int PaddingRightPage { get; set; }
        public int MarginBottomItem { get; set; }
        public int ItemInbreak { get; set; }
    }
}
