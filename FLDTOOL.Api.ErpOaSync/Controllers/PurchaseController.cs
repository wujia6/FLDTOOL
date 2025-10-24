using System.ComponentModel;
using FLDTOOL.Model.ErpOaSync.Dtos;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FLDTOOL.Api.ErpOaSync.Controllers
{
    [Tags("采购接口")]
    [ApiController]
    [Route("fld/purchase")]
    public class PurchaseController(IOptionsMonitor<OaOptions> options, HttpWebClient httpWebClient) : ControllerBase
    {
        private readonly OaOptions oa = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));
        private readonly HttpWebClient webClient = httpWebClient ?? throw new ArgumentNullException(nameof(httpWebClient));

        [HttpPost("order")]
        [EndpointDescription("采购单")]
        public async Task<dynamic> OrderAsync([Description("OA账号")] string loginName, [FromBody] PurchaseOrderDto dto)
        {
            //string formMain = "formmain_0078", formSon = "formson_0079", template = "CGD";
            string formMain = "formmain_0124", formSon = "formson_0125", template = "CGD1";
            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            //构建请求数据
            var purchaseOrder = new Dictionary<string, object>
            {
                { "主体公司", "name|" + dto.Company },
                { "ERP采购单号", dto.ErpOrderNo },
                { "关联申购单", dto.ErpOrderNo2 ?? string.Empty },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "厂商", dto.Manufacturer },
                { "采购类型", "name|" + dto.Type ?? string.Empty },
                { "大类", dto.Category ?? string.Empty },
                { "数量合计", dto.DetailDtos.Sum(src => src.Quantity) },
                { "金额合计", dto.DetailDtos.Sum(src => src.TotalPrice) }
            };
            var purchaseOrderDetails = new List<Dictionary<string, object>>();
            foreach (var detail in dto.DetailDtos)
            {
                purchaseOrderDetails.Add(new Dictionary<string, object>
                {
                    { "商品编码", detail.ProductNo },
                    { "商品名称", detail.ProductName },
                    { "规格型号", detail.Specification ?? string.Empty },
                    { "客户品名", detail.CustomerProductName ?? string.Empty },
                    { "单位", detail.Unit },
                    { "材质", detail.Material ?? string.Empty },
                    { "数量", detail.Quantity },
                    { "单价", detail.Price },
                    { "受订价格", detail.ReservePrice ?? 0 },
                    { "总价", detail.TotalPrice },
                    { "备注", detail.Remark ?? string.Empty }
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
                        { "data", new Dictionary<string, object> { { formMain, purchaseOrder }, { formSon, purchaseOrderDetails } } },
                        { "subject", "采购单" }
                    }
                }
            };

            //提交请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }

        [HttpPost("orderChange")]
        [EndpointDescription("采购变更单")]
        public async Task<dynamic> OrderChangeAsync([Description("OA账号")] string loginName, [FromBody] PurchaseOrderChangeDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");
            
            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            //构建请求数据
            //string formMain = "formmain_0334", formSon = "formson_0336", template = "CGBG";
            string formMain = "formmain_0128", formSon = "formson_0129", template = "CGBGD";
            var changeMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "ERP变更单号", dto.ErpOrderNo },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "采购类型", $"name|{dto.Type ?? string.Empty}" },
                { "变更原因", dto.Explan ?? string.Empty },
                //{ "变更前合计数量", dto.DetailDtos.Where(det => det.ChangeFrom.Equals("变更前")).Sum(det => det.Quantity) },
                //{ "变更前合计金额", dto.DetailDtos.Where(det => det.ChangeFrom.Equals("变更前")).Sum(det => det.Total) },
                { "变更后合计数量", dto.DetailDtos.Sum(det => det.Quantity) ?? 0 },
                { "变更后合计金额", dto.DetailDtos.Sum(det => det.Total) ?? 0 }
            };
            var changeDetails = new List<Dictionary<string, object>>();
            if (dto.DetailDtos != null && dto.DetailDtos.Count > 0)
            {
                dto.DetailDtos.ForEach(det =>
                {
                    changeDetails.Add(new Dictionary<string, object>
                    {
                        //{ "变更", det.ChangeFrom },
                        { "商品编码", det.ProductNo ?? string.Empty },
                        { "商品名称", det.ProductName ?? string.Empty },
                        { "规格型号", det.Specification ?? string.Empty },
                        //{ "厂商名称", det.Manufacturer ?? string.Empty },
                        { "客户品名", det.CustomerProductName ?? string.Empty },
                        { "单位", det.Unit ?? string.Empty },
                        //{ "材质", det.Material ?? string.Empty },
                        { "数量", det.Quantity ?? 0 },
                        { "单价", det.Price ?? 0 },
                        { "税率", det.TaxRate ?? 0 },
                        { "总价", det.Total ?? 0 },
                        //{ "用途", det.Effect ?? string.Empty },
                        { "备注", det.Remark ?? string.Empty }
                    });
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
                        { "data", new Dictionary<string, object> { { formMain, changeMain }, { formSon, changeDetails } } },
                        { "subject", "采购变更单" }
                    }
                }
            };
            //提交请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }

        [HttpPost("orderApply")]
        [EndpointDescription("申购单")]
        public async Task<dynamic> OrderApplyAsync([Description("OA账号")] string loginName, [FromBody] PurchaseRequestOrderDto dto)
        {
            //string formMain = "formmain_0328", formSon = "fromson_0329", template = "SGD";
            string formMain = "formmain_0122", formSon = "formson_0123", template = "SGD1";
            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            if (dto == null || dto.DetailDtos == null)
                return ApiResponse<string>.Fail("必填参数不能为空");

            //构建请求数据
            var pro = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请理由", dto.Reason?? string.Empty },
                { "金额合计", dto.DetailDtos.Sum(src => src.Amount) },
                { "数量合计", dto.DetailDtos.Sum(src => src.Quantity) },
                { "ERP申购单号", dto.ErpOrderNo },
                { "单据类型", $"name|{dto.Type}" },
                { "申请日期", DateTime.Now }
            };
            var details = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                details.Add(new Dictionary<string, object>
                {
                    { "商品编码", det.ProductNo },
                    { "商品名称", det.ProductName },
                    { "规格型号", det.Specification ?? string.Empty },
                    { "单位", det.Unit },
                    { "申购量", det.Quantity },
                    { "材质", det.Material ?? string.Empty },
                    { "历史价", det.PriceHistory ?? 0 },
                    { "预估价", det.PriceExpected ?? 0 },
                    { "现存量", det.ReservesNow ?? 0 },
                    { "需求日期", det.RequireDate.HasValue ? det.RequireDate.Value : string.Empty },
                    { "明细备注", det.Remark ?? string.Empty },
                    { "每列合计金额", det.Amount }
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
                        { "data", new Dictionary<string, object> { { formMain, pro }, { formSon, details } } },
                        { "subject", "申购单" }
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
