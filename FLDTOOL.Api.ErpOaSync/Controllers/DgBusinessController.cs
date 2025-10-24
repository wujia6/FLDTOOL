using System.ComponentModel;
using FLDTOOL.Model.ErpOaSync.Dtos;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FLDTOOL.Api.ErpOaSync.Controllers
{
    [Tags("东莞业务")]
    [ApiController]
    [Route("fld/business")]
    public class DgBusinessController(IOptionsMonitor<OaOptions> oa, HttpWebClient httpWebClient) : ControllerBase
    {
        private readonly OaOptions oa = oa.CurrentValue ?? throw new ArgumentNullException(nameof(oa));
        private readonly HttpWebClient httpWebClient = httpWebClient ?? throw new ArgumentNullException(nameof(httpWebClient));

        [EndpointDescription("快速报价")]
        [HttpPost("quickQuote")]
        public async Task<dynamic> QuickQuoteAsync([Description("OA账号")] string loginName, [FromBody] QuotationDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            string formMain = "formmain_0374", formSon = "formmain_0375", template = "KFBJ";
            //构建请求数据
            var quoteMain = new Dictionary<string, object>
            {
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "客户名称", dto.CustomerName },
                { "客户编号", dto.CustomerCode },
                { "选定流程", $"name|{dto.SelectProcess}" },
                { "关联单号", dto.BusinessNo }
            };
            var quoteDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                quoteDetails.Add(new Dictionary<string, object>
                {
                    { "品号", det.ProductNo ?? string.Empty },
                    { "品名", det.ProductName ?? string.Empty },
                    { "货品规格", det.Specification ?? string.Empty },
                    { "客户品名", det.CustomerProductName ?? string.Empty },
                    { "客户规格", det.CustomerSpecification ?? string.Empty },
                    { "月估用量", det.Quantity ?? 0 },
                    { "材料牌号", det.Grade ?? string.Empty },
                    { "开刃工时", det.WorkHour ?? 0 },
                    { "涂层类型", det.Coating ?? string.Empty },
                    { "钝化", det.Passivate ?? string.Empty },
                    { "避空", det.AvoidEmpty ?? string.Empty },
                    { "成本单价", det.CostPrice ?? 0 },
                    { "不含税售价", det.BeforeTaxPrice ?? 0 },
                    { "报价毛利率", det.QuotationGrossMargin ?? string.Empty }
                });
            });
            //构建请求
            string reqUrl = oa.OaUrl + oa.FlowUrl;
            var requestHeader = new Dictionary<string, string> { { "token", oaUser.id.ToString() } };
            var requestData = new Dictionary<string, object>
            {
                { "appName", "collaboration" },
                {
                    "data", new Dictionary<string, object>
                    {
                        { "templateCode", template },
                        { "draft", "0" },
                        { "data", new Dictionary<string, object> { { formMain, quoteMain }, { formSon, quoteDetails } } },
                        { "subject", "客户报价单" }
                    }
                }
            };
            //发送请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }

        private async Task<dynamic> GetOaUserAsync(string loginName)
        {
            var reqUrl = oa.OaUrl + oa.TokenUrl.Replace("@restName", oa.RestName).Replace("@password", oa.Password).Replace("@loginName", loginName);
            return await httpWebClient.GetAsync<dynamic>(reqUrl, new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            });
        }
    }
}
