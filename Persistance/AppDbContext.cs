using Microsoft.EntityFrameworkCore;
using Domain.Entites;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Persistance;

public partial class AppDbContext : DbContext,IAppDbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

    public virtual DbSet<Deposite> Deposites { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Pricing> Pricings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SenderType> SenderTypes { get; set; }

    public virtual DbSet<ServiceModel> ServiceModels { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRequest> UserRequests { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Withdrawal> Withdrawals { get; set; }

    public DatabaseFacade datbase => Database;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("Attachment");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.FileType)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Message).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachment_Messages");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversation");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
  
            entity.HasOne(d => d.ServiceModel).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.ServiceModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversation_Service");

            entity.HasOne(d => d.User).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversation_User");
        });

        modelBuilder.Entity<CurrencyExchangeRate>(entity =>
        {
            entity.ToTable("CurrencyExchangeRate");

            entity.Property(e => e.CurrencyExchangeRate1)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("CurrencyExchangeRate");
        });

        modelBuilder.Entity<Deposite>(entity =>
        {
            entity.ToTable("Deposite");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DepositeAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DepositeDate).HasColumnType("datetime");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Deposites)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposite_Wallet");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Conversation");

            entity.HasOne(d => d.SenderTypeNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_SenderType");
        });

        modelBuilder.Entity<Pricing>(entity =>
        {
            entity.ToTable("Pricing");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CurrencyExchangeRate).WithMany(p => p.Pricings)
                .HasForeignKey(d => d.CurrencyExchangeRateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pricing_CurrencyExchange");

            entity.HasOne(d => d.ServiceModel).WithMany(p => p.Pricings)
                .HasForeignKey(d => d.ServiceModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pricing_ServiceModel");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SenderType>(entity =>
        {
            entity.ToTable("SenderType");

            entity.Property(e => e.SenderType1)
                .HasMaxLength(50)
                .HasColumnName("SenderType");
        });

        modelBuilder.Entity<ServiceModel>(entity =>
        {
            entity.ToTable("ServiceModel");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SeviceName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Mobile)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRole_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK_User_Role");
                        j.ToTable("UserRole");
                    });
        });

        modelBuilder.Entity<UserRequest>(entity =>
        {
            entity.ToTable("UserRequest");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RequestTime).HasColumnType("datetime");

            entity.HasOne(d => d.Conversation).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRequest_Conversation");

            entity.HasOne(d => d.ServiceModel).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.ServiceModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRequest_Service");

            entity.HasOne(d => d.User).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRequest_User");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BalanceAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Wallet)
                .HasForeignKey<Wallet>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_User");
        });

        modelBuilder.Entity<Withdrawal>(entity =>
        {
            entity.ToTable("Withdrawal");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.WithdrawalAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.WithdrawalDate).HasColumnType("datetime");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Withdrawals)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Withdrawal_Wallet");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
