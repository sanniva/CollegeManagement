using CollegeManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Module = CollegeManagementSystem.Data.Entities.Module;

namespace CollegeManagementSystem.Data;

public class AppDbContext : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<ModuleInstructor> ModuleInstructors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables with custom names
        modelBuilder.Entity<IdentityUser<long>>().ToTable("Users");
        modelBuilder.Entity<IdentityRole<long>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<long>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<long>>().ToTable("UserTokens");

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(200);
            entity.Property(s => s.DateOfBirth).HasColumnType("timestamptz").IsRequired();
            entity.HasIndex(s => s.Email).IsUnique();
        });

        // Configure Instructor entity
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.LastName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.Email).IsRequired().HasMaxLength(200);
            entity.Property(i => i.HireDate).HasColumnType("timestamptz").IsRequired();
            entity.HasIndex(i => i.Email).IsUnique();
        });

        // Configure Course entity
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(c => c.Name).IsUnique();
        });

        // Configure Module entity
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Credits).IsRequired();
            entity.HasIndex(m => new { m.CourseId, m.Title }).IsUnique();
            
            entity.HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Enrollment entity
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);
            
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();
        });

        // Configure ModuleInstructor entity
        modelBuilder.Entity<ModuleInstructor>(entity =>
        {
            entity.HasKey(mi => mi.ModuleInstructorId);
            
            entity.HasOne(mi => mi.Module)
                .WithMany()
                .HasForeignKey(mi => mi.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(mi => mi.Instructor)
                .WithMany()
                .HasForeignKey(mi => mi.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(mi => new { mi.ModuleId, mi.InstructorId })
                .IsUnique();
        });
    }

    // Seed all roles at once
    public async Task SeedRolesAsync()
    {
        string[] roleNames = { "Admin", "Instructor", "Student" };
        
        foreach (var roleName in roleNames)
        {
            if (!Roles.Any(r => r.Name == roleName))
            {
                await Roles.AddAsync(new IdentityRole<long>
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }
        await SaveChangesAsync();
    }
}