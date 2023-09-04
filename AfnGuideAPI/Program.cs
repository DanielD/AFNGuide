using AfnGuideAPI.Data;
using AfnGuideAPI.HostedServices;
using AfnGuideAPI.QueueService;
using AfnGuideAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();

builder.Services.AddLogging();

builder.Services.AddSingleton<IMemoryCache, AfnGuideMemoryCache>();
builder.Services.AddSingleton<OCRSpaceAPIService>();

builder.Services.AddDbContext<AfnGuideDbContext>(options 
       => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.InitializeDatabase();

builder.Services.AddHostedService<ChannelTimeZonesIngestionService>();
builder.Services.AddHostedService<RssIngestionService>();
builder.Services.AddHostedService<PromoIngestionService>();
builder.Services.AddHostedService<BulletinIngestionService>();

builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => new DefaultBackgroundTaskQueue(1000));
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddHostedService<PromoImageOCRService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(opt => opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
    }));
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
