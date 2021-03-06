﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SysModelBank.Data.DataSeeders;
using SysModelBank.Data.DataSeeders.Identity;
using SysModelBank.Data.DataSeeders.Settings;
using SysModelBank.Data.Models;
using SysModelBank.Data.Models.Identity;
using SysModelBank.Data.Models.Settings;

namespace SysModelBank.Data
{
    public class SysModelBankDbContext : IdentityDbContext<User, Role, int>
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Transaction> TransactionCodes { get; set; }

        public SysModelBankDbContext(DbContextOptions<SysModelBankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(SysModelBankDbContext).Assembly);

            new CurrencyDataSeeder().Add(builder);
            new UserDataSeeder().Add(builder);
            new RoleDataSeeder().Add(builder);
            new UserRoleDataSeeder().Add(builder);
            new AccountDataSeeder().Add(builder);

            builder.Entity<Transaction>().HasOne(u => u.SenderAccount).WithMany(u => u.SentTransactions);
            builder.Entity<Transaction>().HasOne(u => u.RecipientAccount).WithMany(u => u.RecievedTransactions);
        }
    }
}
