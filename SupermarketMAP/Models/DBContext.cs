using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketMAP.Models
{
    internal class DBContext : DbContext
    {
        public DBContext() : base("name=mapDB")
        {
            Database.Log = sql => Debug.WriteLine(sql); 
        }


        public DbSet<Product> products { get; set; }
        public DbSet<Producer> producers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Stock> stocks { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Bill> bills { get; set; }
        public DbSet<ProductBill> productBills { get; set; }
        public DbSet<Offer> offers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.category)
                .WithMany(c => c.products)
                .HasForeignKey(p => p.categoryId);

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.producer)
                .WithMany(pr => pr.products)
                .HasForeignKey(p => p.producerId);

            modelBuilder.Entity<Stock>()
                .HasRequired(s => s.product)
                .WithMany(p => p.stocks)
                .HasForeignKey(s => s.productId);

            modelBuilder.Entity<Bill>()
                .HasRequired(b => b.cashier)
                .WithMany(u => u.bills)
                .HasForeignKey(b => b.cashierId);

            modelBuilder.Entity<ProductBill>()
                .HasRequired(pb => pb.bill)
                .WithMany(b => b.productBills)
                .HasForeignKey(pb => pb.billId);

            modelBuilder.Entity<ProductBill>()
                .HasRequired(pb => pb.product)
                .WithMany(p => p.productBills)
                .HasForeignKey(pb => pb.productId);

            modelBuilder.Entity<Offer>()
                .HasRequired(o => o.product)
                .WithMany(p => p.offers)
                .HasForeignKey(o => o.productId);

        }
    }
}
    