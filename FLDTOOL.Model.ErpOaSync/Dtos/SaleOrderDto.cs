using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class SaleOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("客户名称")]
        public string CustomerName { get; set; }

        [Description("客户等级")]
        public string? CustomerLevel { get; set; }

        [Description("订单类型")]
        public string? OrderType { get; set; }

        [Required]
        [Description("ERP销售单号")]
        public string ErpOrderNo { get; set; }

        [Required]
        [Description("交期")]
        public DateTime DeliveryDate { get; set; }

        [Description("特殊要求")]
        public string? Requirements { get; set; }

        [Required]
        [Description("销售明细")]
        public List<SaleOrderDetailDto> DetailDtos { get; set; }
    }

    public class SaleOrderDetailDto
    {
        [Required]
        [Description("商品编码")]
        public string ProductNo { get; set; }

        [Required]
        [Description("商品名称")]
        public string ProductName { get; set; }

        [Description("客户品号")]
        public string? CustomerProductNo { get; set; }

        [Description("客户规格")]
        public string? CustomerSpecification { get; set; }

        [Description("规格")]
        public string? Specification { get; set; }

        [Required]
        [Description("单位")]
        public string Unit { get; set; }

        [Description("材料")]
        public string? Material { get; set; }

        [Description("库存可用量")]
        public int? AvailableStock { get; set; }

        [Required]
        [Description("本次受订量")]
        public float CurrentReceiveQuantity { get; set; }

        [Description("售订未交量")]
        public int? NotDeliveredQuantity { get; set; }

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
