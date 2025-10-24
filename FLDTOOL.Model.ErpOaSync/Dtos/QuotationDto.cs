using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FLDTOOL.Model.ErpOaSync.Dtos
{
    public class QuotationDto
    {
        [Required]
        [Description("客户名称")]
        public string CustomerName { get; set; }

        [Required]
        [Description("客户编号")]
        public string CustomerCode { get; set; }

        [Required]
        [Description("选定流程")]
        public string SelectProcess { get; set; }

        [Required]
        [Description("业务单号")]
        public string BusinessNo { get; set; }

        [Required]
        [Description("明细")]
        public List<QuotationDetailDto> DetailDtos { get; set; }
    }

    public class QuotationDetailDto
    {
        [Description("品号")]
        public string? ProductNo { get; set; }

        [Description("品名")]
        public string? ProductName { get; set; }

        [Description("货品规格")]
        public string? Specification { get; set; }

        [Description("客户品名")]
        public string? CustomerProductName { get; set; }

        [Description("客户规格")]
        public string? CustomerSpecification { get; set; }

        [Description("月估用量")]
        public int? Quantity { get; set; }

        [Description("材料牌号")]
        public string? Grade { get; set; }

        [Description("开刃工时")]
        public double? WorkHour { get; set; }

        [Description("涂层类型")]
        public string? Coating { get; set; }

        [Description("钝化")]
        public string? Passivate { get; set; }

        [Description("避空")]
        public string? AvoidEmpty { get; set; }

        [Description("成本单价")]
        public decimal? CostPrice { get; set; }

        [Description("不含税售价")]
        public decimal? BeforeTaxPrice { get; set; }

        [Description("报价毛利率")]
        public string? QuotationGrossMargin { get; set; }
    }
}
