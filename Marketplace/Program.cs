using Marketplace;
using Marketplace.Api;
using Marketplace.Domain;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using MarketPlace.Infrastructure;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);
var store = new DocumentStore
{
    Urls = new[] { "http://localhost:8080" },
    Database = "Marketplace_Chapter8",
    Conventions =
        {
            FindIdentityProperty = m => m.Name == "_databaseId"
        }
};
store.Initialize();

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
builder.Services.AddScoped(c => store.OpenAsyncSession());
builder.Services.AddScoped<IUnitOfWork, RavenDbUnitOfWork>();
builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();

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

