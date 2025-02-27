using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Database;

public partial class PizzaShopDbContext : DbContext
{
    private readonly IConfiguration _config;
    public PizzaShopDbContext(IConfiguration config)
    {
        _config = config;
    }

    public PizzaShopDbContext(DbContextOptions<PizzaShopDbContext> options, IConfiguration config): base(options)
    {
        _config = config;
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Permissionlist> Permissionlists { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userdetail> Userdetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("city_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("country_pkey");

            entity.ToTable("country");

            entity.HasIndex(e => e.CountryCode, "country_country_code_key").IsUnique();

            entity.HasIndex(e => e.CountryName, "country_country_name_key").IsUnique();

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .HasColumnName("country_code");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => new { e.Roleid, e.Permissionid }, "permissions_roleid_permissionid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Canaddedit).HasColumnName("canaddedit");
            entity.Property(e => e.Candelete).HasColumnName("candelete");
            entity.Property(e => e.Canview).HasColumnName("canview");
            entity.Property(e => e.Isenable).HasColumnName("isenable");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("permissions_modifiedby_fkey");

            entity.HasOne(d => d.PermissionNavigation).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Permissionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_permissionid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_roleid_fkey");
        });

        modelBuilder.Entity<Permissionlist>(entity =>
        {
            entity.HasKey(e => e.Permissionid).HasName("permissionlist_pkey");

            entity.ToTable("permissionlist");

            entity.HasIndex(e => e.Permissionname, "permissionlist_permissionname_key").IsUnique();

            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Permissionname)
                .HasMaxLength(100)
                .HasColumnName("permissionname");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Rolename, "role_rolename_key").IsUnique();

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(32)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StateName)
                .HasMaxLength(100)
                .HasColumnName("state_name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("state_country_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Hashpassword)
                .HasMaxLength(64)
                .HasColumnName("hashpassword");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasColumnName("password");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
        });

        modelBuilder.Entity<Userdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userdetails_pkey");

            entity.ToTable("userdetails");

            entity.HasIndex(e => e.Email, "uc_email").IsUnique();

            entity.HasIndex(e => e.Phone, "uc_phone").IsUnique();

            entity.HasIndex(e => e.Username, "uc_username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address1)
                .HasMaxLength(100)
                .HasColumnName("address1");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.Photourl)
                .HasMaxLength(250)
                .HasColumnName("photourl");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("status");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .HasColumnName("username");
            entity.Property(e => e.Zipcode).HasColumnName("zipcode");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.UserdetailCreatedbyNavigations)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userdetails_createdby_fkey");

            entity.HasOne(d => d.ModifiedbyNavigation).WithMany(p => p.UserdetailModifiedbyNavigations)
                .HasForeignKey(d => d.Modifiedby)
                .HasConstraintName("userdetails_modifiedby_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserdetailUsers)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userdetails_userid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
