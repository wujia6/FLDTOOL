using System.ComponentModel;
using FLDTOOL.Model.ErpOaSync.Dtos;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FLDTOOL.Api.ErpOaSync.Controllers
{
    [Tags("销售接口")]
    [ApiController]
    [Route("fld/sale")]
    public class SaleController(IOptionsMonitor<OaOptions> oa, HttpWebClient httpWebClient) : ControllerBase
    {
        private readonly OaOptions oa = oa.CurrentValue ?? throw new ArgumentNullException(nameof(oa));
        private readonly HttpWebClient httpWebClient = httpWebClient ?? throw new ArgumentNullException(nameof(httpWebClient));

        [HttpPost("order")]
        [EndpointDescription("销售订单")]
        public async Task<dynamic> OrderAsync([Description("OA账号")] string loginName, [FromBody] SaleOrderDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            string formMain = "formmain_0121", formSon = "formson_0122", template = "XSDD";
            //string formMain = "formmain_0132", formSon = "formson_0133", template = "XSDD";
            //构建请求数据
            var saleMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "客户名称", dto.CustomerName },
                { "客户等级", dto.CustomerLevel ?? string.Empty },
                { "订单类型", dto.OrderType ?? string.Empty },
                { "ERP销售单号", dto.ErpOrderNo },
                { "交期", dto.DeliveryDate },
                { "特殊要求", dto.Requirements ?? string.Empty },
                { "库存可用量合计", dto.DetailDtos.Sum(src => src.AvailableStock ?? 0) },
                { "本次受订量合计", dto.DetailDtos.Sum(src => src.CurrentReceiveQuantity) },
                { "售订未交量合计", dto.DetailDtos.Sum(src => src.NotDeliveredQuantity ?? 0) }
            };
            var saleDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                saleDetails.Add(new Dictionary<string, object>
                {
                    { "商品编码", det.ProductNo },
                    { "商品名称", det.ProductName },
                    { "客户品号", det.CustomerProductNo ?? string.Empty },
                    { "客户规格", det.CustomerSpecification ?? string.Empty },
                    { "规格", det.Specification ?? string.Empty },
                    { "单位", det.Unit ?? string.Empty },
                    { "材料", det.Material ?? string.Empty },
                    { "库存可用量", det.AvailableStock ?? 0 },
                    { "本次受订量", det.CurrentReceiveQuantity },
                    { "售订未交量", det.NotDeliveredQuantity ?? 0 },
                    { "备注", det.Remark ?? string.Empty },
                    { "单价", det.Price ?? 0 },
                    { "税率", string.IsNullOrEmpty(det.TaxRate) ? string.Empty : $"name|{det.TaxRate}" },
                    { "税额", det.TaxPrice ?? 0 },
                    { "未税金额", det.NotTaxAmount ?? 0 },
                    { "价税合计", det.TaxPriceTotal ?? 0 },
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
                        { "data", new Dictionary<string, object> { { formMain, saleMain }, { formSon, saleDetails } } },
                        { "subject", "销售订单" }
                    }
                }
            };
            //发送请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }
        
        [HttpPost("orderChange")]
        [EndpointDescription("销售变更单")]
        public async Task<dynamic> OrderChangeAsync([Description("OA账号")] string loginName, [FromBody] SaleOrderChangeDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            string formMain = "formmain_0345", formSon = "formmain_0346", template = "XSBG";
            //string formMain = "formmain_0140", formSon = "formson_0141", template = "XSBG1";
            //构建请求数据
            var changeMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now.Date },
                { "客户名称", dto.CustomerName ?? string.Empty },
                { "订单类型", dto.Type ?? string.Empty },
                { "ERP变更单号", dto.ErpOrderNo },
                { "交期", dto.DeliveryDate.HasValue ? dto.DeliveryDate : string.Empty },
                { "变更原因", dto.ReasonChange ?? string.Empty },
                { "本次受订量合计", dto.DetailDtos.Sum(src => src.CurrentReceiveQuantity ?? 0) },
                { "库存可用量合计", dto.DetailDtos.Sum(src => src.StockQuantity ?? 0) },
                { "售订未交量合计", dto.DetailDtos.Sum(src => src.NotDeliveredQuantity ?? 0) }
            };
            var changeDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                changeDetails.Add(new Dictionary<string, object>
                {
                    //{ "变更", det.ChangeFrom },
                    { "商品编码", det.ProductNo ?? string.Empty },
                    { "客户品号", det.CustomerProductNo ?? string.Empty },
                    { "客户规格", det.CustomerSpecification ?? string.Empty },
                    { "商品名称", det.ProductName ?? string.Empty },
                    { "规格", det.Specification ?? string.Empty },
                    //{ "单位", det.Unit ?? string.Empty },
                    //{ "材料", det.Material ?? string.Empty },
                    { "库存可用量", det.StockQuantity ?? 0 },
                    { "本次受订量", det.CurrentReceiveQuantity ?? 0 },
                    { "售订未交量", det.NotDeliveredQuantity ?? 0 },
                    { "备注", det.Remark ?? string.Empty },
                    { "单价", det.Price ?? 0 },
                    { "税率", string.IsNullOrEmpty(det.TaxRate) ? string.Empty : $"name|{det.TaxRate}" },
                    { "税额", det.TaxPrice ?? 0 },
                    { "未税金额", det.NotTaxAmount ?? 0 },
                    { "价税合计", det.TaxPriceTotal ?? 0 }
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
                        { "data", new Dictionary<string, object> { { formMain, changeMain }, { formSon, changeDetails } } },
                        { "subject", "销售变更单" }
                    }
                }
            };
            //发送请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }

        [HttpPost("orderRefund")]
        [EndpointDescription("销售退回单")]
        public async Task<dynamic> OrderRefundAsync([Description("OA账号")] string loginName, [FromBody] SaleOrderRefundDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            string formMain = "formmain_0343", formSon = "formson_0344", template = "XSTH";
            //string formMain = "formmain_0136", formSon = "formson_0137", template = "XSTH";
            //构建请求数据
            var perdictMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "客户名称", dto.CustomerName },
                { "单据类型", dto.OrderType ?? string.Empty },
                { "ERP退回单号", dto.ErpOrderNo },
                { "出货日期", dto.DeliveryDate.HasValue ? dto.DeliveryDate.Value : string.Empty },
                { "特殊要求", dto.Requirements ?? string.Empty },
                { "数量合计", dto.DetailDtos.Sum(src => src.Quantity ?? 0) },
                { "金额合计", dto.DetailDtos.Sum(src => src.Amount ?? 0) }
            };
            var perdictDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                perdictDetails.Add(new Dictionary<string, object>
                {
                    { "订购单号", det.OrderNo ?? string.Empty },
                    { "商品编码", det.ProductNo ?? string.Empty },
                    { "商品名称", det.ProductName ?? string.Empty },
                    { "规格", det.Specification ?? string.Empty },
                    { "单位", det.Unit ?? string.Empty },
                    { "数量", det.Quantity ?? 0 },
                    { "单价", det.Price ?? 0 },
                    { "金额", det.Amount ?? 0 },
                    { "备注", det.Remark ?? string.Empty },
                    { "税率", string.IsNullOrEmpty(det.TaxRate) ? string.Empty : $"name|{det.TaxRate}" }
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
                        { "data", new Dictionary<string, object> { { formMain, perdictMain }, { formSon, perdictDetails } } },
                        { "subject", "销售退回单" }
                    }
                }
            };
            //发送请求
            var result = await httpWebClient.PostAsync<dynamic>(reqUrl, requestData, requestHeader);
            return result.code.ToString() == "0" ? ApiResponse<string>.Success() : ApiResponse<string>.Fail(result.message.ToString(), int.Parse(result.code.ToString()));
        }

        [HttpPost("orderPredict")]
        [EndpointDescription("预测订单")]
        public async Task<dynamic> OrderPredictAsync([Description("OA账号")] string loginName, [FromBody] PredictOrderDto dto)
        {
            if (dto == null || dto.DetailDtos == null || dto.DetailDtos.Count == 0)
                return ApiResponse<string>.Fail("必填参数不能为空");

            dynamic oaUser = await GetOaUserAsync(loginName);

            if (oaUser.bindingUser == null)
                return ApiResponse<string>.Fail("OA账号无效");

            string formMain = "formmain_0339", formSon = "formson_0340", template = "YCDD";
            //string formMain = "formmain_0134", formSon = "formson_0135", template = "YCDD";
            //构建请求数据
            var perdictMain = new Dictionary<string, object>
            {
                { "主体公司", $"name|{dto.Company}" },
                { "申请人", oaUser.bindingUser.id.ToString() },
                { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                { "申请时间", DateTime.Now },
                { "客户名称", dto.CustomerName },
                { "客户等级", dto.CustomerLevel ?? string.Empty },
                { "单据类型", dto.OrderType ?? string.Empty },
                { "ERP预测单号", dto.ErpOrderNo },
                { "交期", dto.DeliveryDate.HasValue ? dto.DeliveryDate.Value : string.Empty },
                { "特殊要求", dto.Requirements ?? string.Empty }
            };
            var perdictDetails = new List<Dictionary<string, object>>();
            dto.DetailDtos.ForEach(det =>
            {
                perdictDetails.Add(new Dictionary<string, object>
                {
                    { "商品编码", det.ProductNo },
                    { "客户品号", det.CustomerProductNo ?? string.Empty },
                    { "客户规格", det.CustomerSpecification ?? string.Empty },
                    { "商品名称", det.ProductName },
                    { "规格", det.Spceification ?? string.Empty },
                    { "单位", det.Unit ?? string.Empty },
                    //{ "材料", det.Material ?? string.Empty },
                    { "库存可用量", det.AvailableStock ?? 0 },
                    { "数量", det.Quantity },
                    { "备注", det.Remark ?? string.Empty }
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
                        { "data", new Dictionary<string, object> { { formMain, perdictMain }, { formSon, perdictDetails } } },
                        { "subject", "预测订单" }
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
