using System.ComponentModel;
using FLDTOOL.Model.ErpOaSync.Dtos;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FLDTOOL.Api.ErpOaSync.Controllers
{
    [Tags("其他接口")]
    [ApiController]
    [Route("api/other")]
    public class OtherController(IOptionsMonitor<OaOptions> oa, HttpWebClient httpWebClient) : ControllerBase
    {
        private readonly OaOptions oa = oa.CurrentValue ?? throw new ArgumentNullException(nameof(oa));
        private readonly HttpWebClient httpWebClient = httpWebClient ?? throw new ArgumentNullException(nameof(httpWebClient));

        [HttpPost("miscellaneousReceive")]
        [EndpointDescription("杂发领料审批单")]
        public async Task<dynamic> MiscellaneousReceiveAsync([Description("OA账号")] string loginName, [FromBody] MiscellaneousReceiveOrderDto dto)
        {
            string formMain = "formmain_0359", formSon = "formson_0360", template = "ZFD";
            //string formMain = "formmain_0126", formSon = "formson_0127", template = "ZFD";
            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            //构建请求数据
            var orderMain = new Dictionary<string, object>
            {
                { "主体公司", "name|" + dto.Company },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "ERP单号", dto.ErpOrderNo },
                { "单据类型", dto.Type },
                { "备注", dto.Remark ?? string.Empty }
            };
            var orderDetails = new List<Dictionary<string, object>>();
            foreach (var detail in dto.DetailDtos)
            {
                orderDetails.Add(new Dictionary<string, object>
                {
                    { "商品编码", detail.ProductNo },
                    { "商品名称", detail.ProductName },
                    { "规格-型号", detail.Specification ?? string.Empty },
                    { "单位", detail.Unit },
                    { "仓库", detail.WareHouse ?? string.Empty },
                    { "数量", detail.Quantity },
                    { "单价", detail.Price },
                    { "总价", detail.Amount ?? 0 },
                    { "用途", detail.ApplyTo ?? string.Empty },
                    { "受益部门", detail.Department ?? string.Empty }
                });
            }

            //请求构建
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
                        { "data", new Dictionary<string, object> { { formMain, orderMain }, { formSon, orderDetails } } },
                        { "subject", "杂发领料审批单" }
                    }
                }
            };

            //提交请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            if (result.code.ToString() == "0")
                return ApiResponse<string>.Success();
            return ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
            //return result.code != "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
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
