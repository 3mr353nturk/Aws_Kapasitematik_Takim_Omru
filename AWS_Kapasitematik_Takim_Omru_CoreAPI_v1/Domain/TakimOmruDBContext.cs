using System;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication16.Domain
{
    public partial class TakimOmruDBContext : DbContext
    {
        public TakimOmruDBContext()
        {
        }

        public TakimOmruDBContext(DbContextOptions<TakimOmruDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Detail> Detail { get; set; }
        public virtual DbSet<Piece> Piece { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<SubPiece> SubPiece { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Note> Note { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
