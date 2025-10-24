using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter<PaymentOrderType>))]
    public enum PaymentOrderType
    {
        厂商货款 = 1,
        设备软件款 = 2,
        工程款 = 3,
        职工薪酬 = 4,
        服务费 = 5,
        集团调拨 = 6,
        其他 = 7
    }

    [JsonConverter(typeof(JsonStringEnumConverter<PaymentType>))]
    public enum PaymentType
    {
        预付 = 1,
        应付 = 2
    }

    public class PaymentOrderDto
    {
        [Required]
        [Description("主体公司")]
        public string Company{ get; set; }

        [Description("款项类别")]
        public string? Type { get; set; }

        [Required]
        [Description("付款类别")]
        public string PayType { get; set; }

        [Required]
        [Description("ERP请款单号")]
        public string ErpOrderNo { get; set; }

        [Required]
        [Description("请款单明细")]
        public List<PaymentOrderDetailDto> DetailDtos { get; set; }
    }

    public class PaymentOrderDetailDto
    {
        [Required]
        [Description("摘要")]
        public string Remark { get; set; }

        [Description("项目类别")]
        public string? ProjectType { get; set; }

        [Required]
        [Description("收款单位名称")]
        public string ReceiveUnitName { get; set; }

        [Description("收款账号")]
        public string? PayeeAccount { get; set; }

        [Description("银行行号")]
        public string? BankNo { get; set; }

        [Description("开户银行")]
        public string? BankOrigin { get; set; }

        [Required]
        [Description("含税金额")]
        public decimal TaxAmount { get; set; }

        [Description("付款事项说明")]
        public string? Explan { get; set; }
    }
}
