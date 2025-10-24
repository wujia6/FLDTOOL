using FLDTOOL.Api.ErpOaSync.Middleware;
using FLDTOOL.Model.ErpOaSync.Options;
using FLDTOOL.Utils.ErpOaSync;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //json小驼峰
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    //避免循环引用
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    //格式化时间
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "富兰地数据同步服务",
            Version = "v1",
            Description = "用友ERP-致远OA"
        };
        //设置对外展示的 server 地址（替换服务器地址或域名，本机调试时注释document.Servers代码块）
        document.Servers =
        [
            //new Microsoft.OpenApi.Models.OpenApiServer
            //{
            //    Url = "http://10.39.0.170:8377",
            //    Description = "内网地址"
            //},
            new Microsoft.OpenApi.Models.OpenApiServer
            {
                Url = "http://110.52.91.35:8733",   //实际部署的服务器外网地址
                Description = "外网地址"
            }
        ];
        return Task.CompletedTask;
    });
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        //找出枚举类型
        if (context.JsonTypeInfo.Type.BaseType == typeof(Enum))
        {
            var list = new List<IOpenApiAny>();
            //获取枚举项
            foreach (var enumValue in schema.Enum.OfType<OpenApiString>())
            {
                //把枚举项转为枚举类型
                if (Enum.TryParse(context.JsonTypeInfo.Type, enumValue.Value, out var result))
                {
                    //通过枚举扩展方法获取枚举描述
                    //var description = ((Enum)result).ToDescription();
                    //重新组织枚举值展示结构
                    list.Add(new OpenApiString(enumValue.Value));
                }
                else
                {
                    list.Add(enumValue);
                }
            }
            schema.Enum = list;
        }
        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())); //cors

builder.Services.Configure<OaOptions>(builder.Configuration.GetSection("OaOptions"));

builder.Services.AddHttpClient().AddScoped<HttpWebClient>();

//日志
//builder.Services.AddTransient<LoggingHandler>();
//builder.Services.AddHttpClient("LoggedClient").AddHttpMessageHandler<LoggingHandler>();

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Run();