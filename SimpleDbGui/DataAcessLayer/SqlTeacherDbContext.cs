using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleDbGui.DataAcessLayer.Domain;

namespace SimpleDbGui.DataAcessLayer;

public partial class SqlTeacherDbContext : DbContext
{
    public SqlTeacherDbContext()
    {
    }

    public SqlTeacherDbContext(DbContextOptions<SqlTeacherDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abteilung> Abteilungs { get; set; }

    public virtual DbSet<Artikel> Artikels { get; set; }

    public virtual DbSet<Bestellung> Bestellungs { get; set; }

    public virtual DbSet<Hersteller> Herstellers { get; set; }

    public virtual DbSet<Jobticket> Jobtickets { get; set; }

    public virtual DbSet<Kategorie> Kategories { get; set; }

    public virtual DbSet<Kunde> Kundes { get; set; }

    public virtual DbSet<Mitarbeiter> Mitarbeiters { get; set; }

    public virtual DbSet<Mwstsatz> Mwstsatzs { get; set; }

    public virtual DbSet<Posten> Postens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=192.168.64.128; user id=mysqladmin;password=Riethuesli>12345;database=SqlTeacherDb");
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abteilung>(entity =>
        {
            entity.HasKey(e => e.Abteilungsnr).HasName("PRIMARY");

            entity.ToTable("abteilung");

            entity.Property(e => e.Abteilungsnr).HasColumnName("ABTEILUNGSNR");
            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(50)
                .HasColumnName("BEZEICHNUNG");
        });

        modelBuilder.Entity<Artikel>(entity =>
        {
            entity.HasKey(e => e.Artikelnr).HasName("PRIMARY");

            entity.ToTable("artikel");

            entity.HasIndex(e => e.Hersteller, "HERSTELLER");

            entity.HasIndex(e => e.Kategorie, "KATEGORIE");

            entity.HasIndex(e => e.Mwst, "MWST");

            entity.Property(e => e.Artikelnr).HasColumnName("ARTIKELNR");
            entity.Property(e => e.Bestand).HasColumnName("BESTAND");
            entity.Property(e => e.Bestellvorschlag)
                .HasMaxLength(1)
                .HasDefaultValueSql("'0'")
                .IsFixedLength()
                .HasColumnName("BESTELLVORSCHLAG");
            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(50)
                .HasColumnName("BEZEICHNUNG");
            entity.Property(e => e.Hersteller).HasColumnName("HERSTELLER");
            entity.Property(e => e.Kategorie).HasColumnName("KATEGORIE");
            entity.Property(e => e.Mindestbestand).HasColumnName("MINDESTBESTAND");
            entity.Property(e => e.Mwst).HasColumnName("MWST");
            entity.Property(e => e.Nettopreis)
                .HasPrecision(10)
                .HasColumnName("NETTOPREIS");

            entity.HasOne(d => d.HerstellerNavigation).WithMany(p => p.Artikels)
                .HasForeignKey(d => d.Hersteller)
                .HasConstraintName("artikel_ibfk_2");

            entity.HasOne(d => d.KategorieNavigation).WithMany(p => p.Artikels)
                .HasForeignKey(d => d.Kategorie)
                .HasConstraintName("artikel_ibfk_3");

            entity.HasOne(d => d.MwstNavigation).WithMany(p => p.Artikels)
                .HasForeignKey(d => d.Mwst)
                .HasConstraintName("artikel_ibfk_1");
        });

        modelBuilder.Entity<Bestellung>(entity =>
        {
            entity.HasKey(e => e.Bestellnr).HasName("PRIMARY");

            entity.ToTable("bestellung");

            entity.HasIndex(e => e.Kundennr, "KUNDENNR");

            entity.Property(e => e.Bestellnr).HasColumnName("BESTELLNR");
            entity.Property(e => e.Bestelldatum)
                .HasColumnType("date")
                .HasColumnName("BESTELLDATUM");
            entity.Property(e => e.Kundennr).HasColumnName("KUNDENNR");
            entity.Property(e => e.Lieferdatum)
                .HasColumnType("date")
                .HasColumnName("LIEFERDATUM");
            entity.Property(e => e.Rechnungsbetrag)
                .HasPrecision(10)
                .HasColumnName("RECHNUNGSBETRAG");

            entity.HasOne(d => d.KundennrNavigation).WithMany(p => p.Bestellungs)
                .HasForeignKey(d => d.Kundennr)
                .HasConstraintName("bestellung_ibfk_1");
        });

        modelBuilder.Entity<Hersteller>(entity =>
        {
            entity.HasKey(e => e.Herstellernr).HasName("PRIMARY");

            entity.ToTable("hersteller");

            entity.Property(e => e.Herstellernr).HasColumnName("HERSTELLERNR");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Jobticket>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("jobticket");

            entity.HasIndex(e => e.Mitarbeiternr, "MITARBEITERNR");

            entity.Property(e => e.GueltigBis)
                .HasColumnType("date")
                .HasColumnName("GUELTIG_BIS");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Mitarbeiternr).HasColumnName("MITARBEITERNR");

            entity.HasOne(d => d.MitarbeiternrNavigation).WithMany()
                .HasForeignKey(d => d.Mitarbeiternr)
                .HasConstraintName("jobticket_ibfk_1");
        });

        modelBuilder.Entity<Kategorie>(entity =>
        {
            entity.HasKey(e => e.Kategorienr).HasName("PRIMARY");

            entity.ToTable("kategorie");

            entity.Property(e => e.Kategorienr).HasColumnName("KATEGORIENR");
            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(50)
                .HasColumnName("BEZEICHNUNG");
        });

        modelBuilder.Entity<Kunde>(entity =>
        {
            entity.HasKey(e => e.Kundennr).HasName("PRIMARY");

            entity.ToTable("kunde");

            entity.Property(e => e.Kundennr).HasColumnName("KUNDENNR");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Ort)
                .HasMaxLength(50)
                .HasColumnName("ORT");
            entity.Property(e => e.Plz)
                .HasMaxLength(14)
                .IsFixedLength()
                .HasColumnName("PLZ");
            entity.Property(e => e.Strasse)
                .HasMaxLength(50)
                .HasColumnName("STRASSE");
            entity.Property(e => e.TelefonGesch)
                .HasMaxLength(25)
                .HasColumnName("TELEFON_GESCH");
            entity.Property(e => e.TelefonPrivat)
                .HasMaxLength(25)
                .HasColumnName("TELEFON_PRIVAT");
            entity.Property(e => e.Vorname)
                .HasMaxLength(50)
                .HasColumnName("VORNAME");
            entity.Property(e => e.Zahlungsart)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("ZAHLUNGSART");
        });

        modelBuilder.Entity<Mitarbeiter>(entity =>
        {
            entity.HasKey(e => e.Mitarbeiternr).HasName("PRIMARY");

            entity.ToTable("mitarbeiter");

            entity.HasIndex(e => e.Abteilung, "ABTEILUNG");

            entity.Property(e => e.Mitarbeiternr).HasColumnName("MITARBEITERNR");
            entity.Property(e => e.Abteilung).HasColumnName("ABTEILUNG");
            entity.Property(e => e.Eintrittsdatum)
                .HasColumnType("date")
                .HasColumnName("EINTRITTSDATUM");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gehalt)
                .HasPrecision(10)
                .HasColumnName("GEHALT");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Ort)
                .HasMaxLength(50)
                .HasColumnName("ORT");
            entity.Property(e => e.Plz)
                .HasMaxLength(14)
                .IsFixedLength()
                .HasColumnName("PLZ");
            entity.Property(e => e.Strasse)
                .HasMaxLength(50)
                .HasColumnName("STRASSE");
            entity.Property(e => e.Telefonnummer)
                .HasMaxLength(25)
                .HasColumnName("TELEFONNUMMER");
            entity.Property(e => e.Vorname)
                .HasMaxLength(50)
                .HasColumnName("VORNAME");

            entity.HasOne(d => d.AbteilungNavigation).WithMany(p => p.Mitarbeiters)
                .HasForeignKey(d => d.Abteilung)
                .HasConstraintName("mitarbeiter_ibfk_1");
        });

        modelBuilder.Entity<Mwstsatz>(entity =>
        {
            entity.HasKey(e => e.Mwstnr).HasName("PRIMARY");

            entity.ToTable("mwstsatz");

            entity.Property(e => e.Mwstnr).HasColumnName("MWSTNR");
            entity.Property(e => e.Prozent)
                .HasPrecision(4)
                .HasColumnName("PROZENT");
        });

        modelBuilder.Entity<Posten>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("posten");

            entity.HasIndex(e => e.Artikelnr, "ARTIKELNR");

            entity.HasIndex(e => e.Bestellnr, "BESTELLNR");

            entity.Property(e => e.Artikelnr).HasColumnName("ARTIKELNR");
            entity.Property(e => e.Bestellmenge).HasColumnName("BESTELLMENGE");
            entity.Property(e => e.Bestellnr).HasColumnName("BESTELLNR");
            entity.Property(e => e.Liefermenge).HasColumnName("LIEFERMENGE");

            entity.HasOne(d => d.ArtikelnrNavigation).WithMany()
                .HasForeignKey(d => d.Artikelnr)
                .HasConstraintName("posten_ibfk_2");

            entity.HasOne(d => d.BestellnrNavigation).WithMany()
                .HasForeignKey(d => d.Bestellnr)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("posten_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
