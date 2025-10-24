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
        return Task.CompletedTask;
    });
});

//cors
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseAuthorization();
app.MapControllers();
app.Run();