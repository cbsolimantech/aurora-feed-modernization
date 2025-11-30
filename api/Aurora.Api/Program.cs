var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

//Health endpoints
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

var feedItems = new List<FeedItem>();

app.MapGet("/api/v2/feed", () =>
{
    return Results.Ok("All good in v2!");
});

app.MapGet("/api/feed", () =>
{
    return Results.Ok(feedItems);
});

app.MapGet("/api/feed/{id}", (int id) =>
{
    var item = feedItems.FirstOrDefault(f => f.Id == id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapPost("/api/feed", (FeedItem newItem) =>
{
    newItem.Id = feedItems.Count + 1;
    feedItems.Add(newItem);
    return Results.Created($"/api/feed/{newItem.Id}", newItem);
});

app.MapPut("/api/feed/{id}", (int id, FeedItem updated) =>
{
    var item = feedItems.FirstOrDefault(f => f.Id == id);
    if (item is null) return Results.NotFound();

    item.Title = updated.Title;
    item.Content = updated.Content;
    return Results.Ok(item);
});

app.MapDelete("/api/feed/{id}", (int id) =>
{
    var item = feedItems.FirstOrDefault(f => f.Id == id);
    if (item is null) return Results.NotFound();

    feedItems.Remove(item);
    return Results.NoContent();
});

app.Run();

record FeedItem
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
}
