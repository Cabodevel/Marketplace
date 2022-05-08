using Marketplace.Api;
using Marketplace.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton(new ClassifiedAdsCommandsApi());
builder.Services.AddSingleton<IEntityStore, RavenDbEntityStore>();
builder.Services.AddScoped<IHandleCommand<ClassifiedAds.V1.Create>,CreateClassifiedAdHandler>();
builder.Services.AddScoped<IHandleCommand<ClassifiedAds.V1.Create>>(c =>
    new RetryingCommandHandler<ClassifiedAds.V1.Create>(
        new CreateClassifiedAdHandler(c.GetService<RavenDbEntityStore>())));

builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "ClassifiedAds",
                        Version = "v1"
                    }));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
       "/swagger/v1/swagger.json",
       "ClassifiedAds v1");
    c.RoutePrefix = "";
});



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

