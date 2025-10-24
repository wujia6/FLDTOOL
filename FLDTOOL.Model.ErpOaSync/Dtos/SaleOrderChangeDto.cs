using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class SaleOrderChangeDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Description("客户名称")]
        public string? CustomerName { get; set; }

        [Description("订单类型")]
        public string? Type { get; set; }

        [Required]
        [Description("ERP变更单号")]
        public string ErpOrderNo { get; set; }

        [Description("交期")]
        public DateTime? DeliveryDate { get; set; }

        [Description("变更原因")]
        public string? ReasonChange { get; set; }

        //[Required]
        [Description("变更明细")]
        public List<SaleOrderChangeDetailDto> DetailDtos { get; set; }
    }

    public class SaleOrderChangeDetailDto
    {
        //[Required]
        //[Description("变更")]
        //public string ChangeFrom { get; set; }

        [Description("商品编码")]
        public string? ProductNo { get; set; }

        [Description("客户品号")]
        public string? CustomerProductNo { get; set; }

        [Description("客户规格")]
        public string? CustomerSpecification { get; set; }

        [Description("商品名称")]
        public string? ProductName { get; set; }

        [Description("规格型号")]
        public string? Specification { get; set; }

        //[Description("单位")]
        //public string? Unit { get; set; }

        //[Description("材料")]
        //public string? Material { get; set; }

        [Description("可用库存量")]
        public float? StockQuantity { get; set; }

        [Description("本次受订量")]
        public float? CurrentReceiveQuantity { get; set; }

        [Description("售订未交量")]
        public float? NotDeliveredQuantity { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        [Description("单价")]
        public decimal? Price { get; set; }

        [Description("税率")]
        public string? TaxRate { get; set; }

        [Description("税额")]
        public decimal? TaxPrice { get; set; }

        [Description("未税金额")]
        public decimal? NotTaxAmount { get; set; }

        [Description("税价合计")]
        public decimal? TaxPriceTotal { get; set; }
    }
}
