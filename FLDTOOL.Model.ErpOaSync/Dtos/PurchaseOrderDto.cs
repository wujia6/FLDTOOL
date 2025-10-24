using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class PurchaseOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("ERP采购单")]
        public string ErpOrderNo { get; set; }

        [Description("ERP申购单")]
        public string? ErpOrderNo2 { get; set; }

        [Description("采购类型")]
        public string? Type { get; set; }

        [Required]
        [Description("厂商")]
        public string Manufacturer { get; set; }

        [Description("大类")]
        public string? Category { get; set; }

        [Required]
        [Description("采购单明细")]
        public List<PurchaseOrderDetailDto> DetailDtos { get; set; }
    }

    public class PurchaseOrderDetailDto
    {
        [Required]
        [Description("数量")]
        public float Quantity { get; set; }

        [Required]
        [Description("单价")]
        public decimal Price { get; set; }

        [Description("受订价格")]
        public decimal? ReservePrice { get; set; }

        [Required]
        [Description("总价")]
        public decimal TotalPrice { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        [Required]
        [Description("商品编码")]
        public string ProductNo { get; set; }

        [Description("规格型号")]
        public string? Specification { get; set; }

        [Required]
        [Description("商品名称")]
        public string ProductName { get; set; }

        [Required]
        [Description("单位")]
        public string Unit { get; set; }

        [Description("材质")]
        public string? Material { get; set; }

        [Description("用途")]
        public string? ApplyTo { get; set; }

        [Description("客户品名")]
        public string? CustomerProductName { get; set; }
    }
}
