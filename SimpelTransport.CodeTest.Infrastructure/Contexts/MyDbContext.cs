using Microsoft.EntityFrameworkCore;
using SimpelTransport.CodeTest.Domain.Entities;

namespace SimpelTransport.CodeTest.Infrastructure.Contexts;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
}