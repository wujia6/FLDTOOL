using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class SaleOrderRefundDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("客户名称")]
        public string CustomerName { get; set; }

        [Description("单据类型")]
        public string? OrderType { get; set; }

        [Required]
        [Description("ERP退回单号")]
        public string ErpOrderNo { get; set; }

        [Description("出货日期")]
        public DateTime? DeliveryDate { get; set; }

        [Description("特殊要求")]
        public string? Requirements { get; set; }

        [Required]
        [Description("退回明细")]
        public List<SaleOrderRefundDetailDto> DetailDtos { get; set; }
    }

    public class SaleOrderRefundDetailDto
    {
        [Description("订购单号")]
        public string? OrderNo { get; set; }

        [Description("商品编码")]
        public string? ProductNo { get; set; }

        [Description("商品名称")]
        public string? ProductName { get; set; }

        [Description("规格型号")]
        public string? Specification { get; set; }

        [Description("单位")]
        public string? Unit { get; set; }

        [Description("数量")]
        public float? Quantity { get; set; }

        [Description("单价")]
        public decimal? Price { get; set; }

        [Description("金额")]
        public decimal? Amount { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        [Description("税率")]
        public string? TaxRate { get; set; }
    }
}
