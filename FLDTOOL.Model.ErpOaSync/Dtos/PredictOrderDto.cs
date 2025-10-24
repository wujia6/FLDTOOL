using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class PredictOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("客户名称")]
        public string CustomerName { get; set; }

        [Description("客户等级")]
        public string? CustomerLevel { get; set; }

        [Description("单据类型")]
        public string? OrderType { get; set; }

        [Required]
        [Description("ERP预测单号")]
        public string ErpOrderNo { get; set; }

        [Description("计划出货日期")]
        public DateTime? DeliveryDate { get; set; }

        [Description("特殊要求")]
        public string? Requirements { get; set; }

        [Required]
        [Description("订单明细")]
        public List<PerdictOrderDetailDto> DetailDtos { get; set; }
    }

    public class PerdictOrderDetailDto
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
        public string? Spceification { get; set; }

        [Required]
        [Description("单位")]
        public string Unit { get; set; }

        //[Description("材料")]
        //public string? Material { get; set; }

        [Description("库存可用量")]
        public float? AvailableStock { get; set; }

        [Required]
        [Description("数量")]
        public float Quantity { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }
    }
}
