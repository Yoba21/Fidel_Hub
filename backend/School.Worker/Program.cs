using Quartz;

var builder = Host.CreateApplicationBuilder(args);

// Configure Quartz.NET background job scheduler
builder.Services.AddQuartz(q =>
{
    // Outbox dispatcher and periodic sync jobs configured here
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var host = builder.Build();
host.Run();
