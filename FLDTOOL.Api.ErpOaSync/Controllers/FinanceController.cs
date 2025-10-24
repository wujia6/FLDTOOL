using System.ComponentModel;
using System.Net;
using FLDTOOL.Model.ErpOaSync.Dtos;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FLDTOOL.Api.ErpOaSync.Controllers
{
    [Tags("财务接口")]
    [ApiController]
    [Route("fld/finance")]
    public class FinanceController(IOptionsMonitor<OaOptions> oa, HttpWebClient httpWebClient) : ControllerBase
    {
        private readonly OaOptions oa = oa.CurrentValue ?? throw new ArgumentNullException(nameof(oa));
        private readonly HttpWebClient httpWebClient = httpWebClient ?? throw new ArgumentNullException(nameof(httpWebClient));
        
        [HttpPost("orderPay")]
        [EndpointDescription("请款单")]
        public async Task<dynamic> OrderPayAsync([Description("OA账号")] string loginName, [FromBody] PaymentOrderDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);
            
            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            //string formMain = "formmain_0330", formSon = "formson_0331", template = "QKD";
            string formMain = "formmain_0130", formSon = "formson_0131", template = "QKD";
            //构建请求数据
            var payMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请日期", DateTime.Now },
                { "款项类别", string.IsNullOrEmpty(dto.Type) ? string.Empty : $"name|{dto.Type}" },
                { "付款类别", $"name|{dto.PayType}" },
                { "ERP请款单号", dto.ErpOrderNo },
                { "含税金额合计", dto.DetailDtos.Sum(src => src.TaxAmount) }
            };
            var payDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det => 
            {
                payDetails.Add(new Dictionary<string, object>
                {
                    { "摘要", det.Remark },
                    { "项目类别", det.ProjectType ?? string.Empty },
                    { "收款单位名称", det.ReceiveUnitName },
                    { "收款账号", det.PayeeAccount ?? string.Empty },
                    { "银行行号", det.BankNo ?? string.Empty },
                    { "开户银行", det.BankOrigin ?? string.Empty },
                    { "含税金额", det.TaxAmount },
                    { "付款事项说明", det.Explan ?? string.Empty }
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
                        { "data", new Dictionary<string, object> { { formMain, payMain }, { formSon, payDetails } } },
                        { "subject", "请购单" }
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
