using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter<ProType>))]
    public enum ProType
    {
        外购品申购单 = 10,
        物料申购单 = 11,
        原材料申购单 = 12,
        固定资产申购单 = 13
    }

    public class PurchaseRequestOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company { get; set; }

        [Description("申请理由")]
        public string? Reason { get; set; }

        //[Required]
        //[Description("金额合计")]
        //public decimal TotalAmount { get; set; }

        //[Required]
        //[Description("数量合计")]
        //public int TotalQuantity { get; set; }

        [Required]
        [Description("ERP申购单号")]
        public string ErpOrderNo { get; set; }

        [Required]
        [Description("单据类型")]
        public string Type { get; set; }

        //[Description("申请日期")]
        //public static DateOnly Date { get => DateOnly.FromDateTime(DateTime.Now); }

        [Required]
        [Description("申购明细")]
        public List<PurchaseRequestOrderDetailDto> DetailDtos { get; set; }
    }

    public class PurchaseRequestOrderDetailDto
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

        [Required]
        [Description("申购量")]
        public float Quantity { get; set; }

        [Description("材质")]
        public string? Material { get; set; }

        [Description("历史价")]
        public decimal? PriceHistory { get; set; }

        [Description("预估价")]
        public decimal? PriceExpected { get; set; }

        [Description("现存量")]
        public float? ReservesNow { get; set; }

        [Description("需求日期")]
        public DateTime? RequireDate { get; set; }

        [Description("明细备注")]
        public string? Remark { get; set; }

        [Description("每列合计金额")]
        public decimal Amount { get; set; }
    }
}
