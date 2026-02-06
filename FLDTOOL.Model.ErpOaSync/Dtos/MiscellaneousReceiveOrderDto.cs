using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class MiscellaneousReceiveOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("ERP单号")]
        public string ErpOrderNo { get; set; }

        [Description("单据类型")]
        public string Type { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        [Required]
        [Description("明细")]
        public List<MiscellaneousReceiveOrderDetailDto> DetailDtos { get; set; }
    }

    public class MiscellaneousReceiveOrderDetailDto
    {
        [Required]
        [Description("商品编码")]
        public string ProductNo { get; set; }

        [Required]
        [Description("商品名称")]
        public string ProductName { get; set; }

        [Description("规格型号")]
        public string? Specification { get; set; }

        [Required]
        [Description("单位")]
        public string Unit { get; set; }

        [Description("仓库")]
        public string? WareHouse { get; set; }

        [Required]
        [Description("数量")]
        public float Quantity { get; set; }

        [Required]
        [Description("单价")]
        public decimal Price { get; set; }

        [Description("总价")]
        public decimal? Amount { get; set; }

        [Description("用途")]
        public string? ApplyTo { get; set; }

        [Description("受益部门")]
        public string? Department { get; set; }
    }
}
