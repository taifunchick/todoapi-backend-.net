using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseSqlite("Data Source=todos.db"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "✅ Todo API is running! Go to /swagger");

// 📋 GET all todos (with filters)
app.MapGet("/todos", async (TodoDb db, 
    [FromQuery] string? category,
    [FromQuery] string? priority,
    [FromQuery] bool? completed,
    [FromQuery] string? search) =>
{
    var query = db.Todos.AsQueryable();

    if (!string.IsNullOrEmpty(category))
        query = query.Where(t => t.Category == category);
    
    if (!string.IsNullOrEmpty(priority))
        query = query.Where(t => t.Priority == priority);
    
    if (completed.HasValue)
        query = query.Where(t => t.IsCompleted == completed.Value);
    
    if (!string.IsNullOrEmpty(search))
        query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));

    return Results.Ok(await query.OrderByDescending(t => t.CreatedAt).ToListAsync());
});

// 📋 GET single todo
app.MapGet("/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

// ➕ POST create todo
app.MapPost("/todos", async (Todo todo, TodoDb db) =>
{
    if (string.IsNullOrWhiteSpace(todo.Title))
        return Results.BadRequest(new { error = "Title is required" });

    todo.CreatedAt = DateTime.Now;
    todo.UpdatedAt = DateTime.Now;
    
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    
    return Results.Created($"/todos/{todo.Id}", todo);
});

// ✏️ PUT update todo
app.MapPut("/todos/{id}", async (int id, Todo input, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.Description = input.Description;
    todo.Priority = input.Priority;
    todo.Category = input.Category;
    todo.DueDate = input.DueDate;
    todo.IsCompleted = input.IsCompleted;
    todo.UpdatedAt = DateTime.Now;

    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

// ✅ PATCH toggle complete
app.MapPatch("/todos/{id}/toggle", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.IsCompleted = !todo.IsCompleted;
    todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;
    todo.UpdatedAt = DateTime.Now;
    
    await db.SaveChangesAsync();
    
    var status = todo.IsCompleted ? "✅ Completed" : "🔄 Reopened";
    return Results.Ok(new { message = status, todo });
});

// 🗑️ DELETE todo
app.MapDelete("/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Todo deleted" });
});

// 📊 GET stats
app.MapGet("/stats", async (TodoDb db) =>
{
    var total = await db.Todos.CountAsync();
    var completed = await db.Todos.CountAsync(t => t.IsCompleted);
    var pending = total - completed;
    var highPriority = await db.Todos.CountAsync(t => t.Priority == "High" && !t.IsCompleted);
    var overdue = await db.Todos.CountAsync(t => t.DueDate < DateTime.Now && !t.IsCompleted);

    return Results.Ok(new
    {
        total,
        completed,
        pending,
        highPriority,
        overdue,
        completionRate = total == 0 ? 0 : Math.Round((double)completed / total * 100, 1)
    });
});

// 🏷️ GET categories
app.MapGet("/categories", async (TodoDb db) =>
{
    var categories = await db.Todos
        .Where(t => t.Category != null && t.Category != "")
        .Select(t => t.Category)
        .Distinct()
        .ToListAsync();
    
    return Results.Ok(categories);
});

app.Run();

// Models
public class Todo
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public string Priority { get; set; } = "Medium"; // High, Medium, Low
    public string Category { get; set; } = "General";
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
    public DbSet<Todo> Todos { get; set; }
}