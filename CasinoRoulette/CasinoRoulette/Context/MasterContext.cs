using System;
using System.Collections.Generic;
using CasinoRoulette.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoRoulette.Context;

public partial class MasterContext : DbContext
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }

    public virtual DbSet<Bet> Bets { get; set; }

    public virtual DbSet<BetNumber> BetNumbers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Spin> Spins { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=127.0.0.1,1771; Database=master; User=sa; Password=mati1234!; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("AccountTransaction_pk");

            entity.ToTable("AccountTransaction");

            entity.Property(e => e.Amount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Player).WithMany(p => p.AccountTransactions)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transaction_Player");
        });

        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(e => e.BetId).HasName("Bet_pk");

            entity.ToTable("Bet");

            entity.Property(e => e.BetAmount).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.BetType).HasMaxLength(20);

            entity.HasOne(d => d.Game).WithMany(p => p.Bets)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bet_Game");

            entity.HasOne(d => d.Player).WithMany(p => p.Bets)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bet_Player");

            entity.HasOne(d => d.Spin).WithMany(p => p.Bets)
                .HasForeignKey(d => d.SpinId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bet_Spin");
        });

        modelBuilder.Entity<BetNumber>(entity =>
        {
            entity.HasKey(e => e.BetNumberId).HasName("BetNumber_pk");

            entity.ToTable("BetNumber");

            entity.HasOne(d => d.Bet).WithMany(p => p.BetNumbers)
                .HasForeignKey(d => d.BetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BetNumber_Bet");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("Game_pk");

            entity.ToTable("Game");

            entity.Property(e => e.GameName).HasMaxLength(50);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("Player_pk");

            entity.ToTable("Player");

            entity.Property(e => e.AccountBalance).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Email).HasMaxLength(40);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(40);
            entity.Property(e => e.RefreshTokenExp).HasColumnType("datetime");
            entity.Property(e => e.Telephone).HasMaxLength(9);
        });

        modelBuilder.Entity<Spin>(entity =>
        {
            entity.HasKey(e => e.SpinId).HasName("Spin_pk");

            entity.ToTable("Spin");

            entity.HasOne(d => d.Game).WithMany(p => p.Spins)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Spin_Game");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Uaid).HasName("UserActivity_pk");

            entity.ToTable("UserActivity");

            entity.Property(e => e.Uaid).HasColumnName("UAId");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Player).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Session_Player");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
