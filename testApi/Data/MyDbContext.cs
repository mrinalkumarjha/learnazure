using System;
using Microsoft.EntityFrameworkCore;
using testApi.Models;

namespace testApi.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<Course> Products => Set<Course>();
}
