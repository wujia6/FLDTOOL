using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class PurchaseOrderChangeDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Required]
        [Description("ERP变更单号")]
        public string ErpOrderNo { get; set; }

        [Description("采购类型")]
        public string? Type { get; set; }

        [Description("变更说明")]
        public string? Explan { get; set; }
        
        [Description("变更明细")]
        public List<PurchaseOrderChangeDtialDto>? DetailDtos { get; set; }
    }

    public class PurchaseOrderChangeDtialDto
    {
        //[Required]
        //[Description("变更")]
        //public string ChangeFrom { get; set; }

        [Description("商品编码")]
        public string? ProductNo { get; set; }

        [Description("商品名称")]
        public string? ProductName { get; set; }

        [Description("规格型号")]
        public string? Specification { get; set; }

        //[Description("厂商名称")]
        //public string? Manufacturer { get; set; }

        [Description("客户品名")]
        public string? CustomerProductName { get; set; }

        [Description("单位")]
        public string? Unit { get; set; }

        //[Description("材质")]
        //public string? Material { get; set;}

        [Description("数量")]
        public float? Quantity { get; set; }

        [Description("单价")]
        public decimal? Price { get; set; }

        [Description("税率")]
        public int? TaxRate { get; set; }

        [Description("总价")]
        public decimal? Total { get; set; }

        //[Description("用途")]
        //public string? Effect { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }
    }
}
