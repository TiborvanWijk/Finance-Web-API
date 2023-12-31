
using FinanceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinanceApi.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }


        public DbSet<Income> Incomes { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<GoalCategory> GoalCategories { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<BudgetCategory>()
                .HasKey(bk => new { bk.BudgetId, bk.CategoryId });
            builder.Entity<BudgetCategory>()
                .HasOne(b => b.Budget)
                .WithMany(bc => bc.BudgetCategories)
                .HasForeignKey(b => b.BudgetId);
            builder.Entity<BudgetCategory>()
                .HasOne(b => b.Category)
                .WithMany(bc => bc.BudgetCategories)
                .HasForeignKey(b => b.CategoryId);


            builder.Entity<ExpenseCategory>()
                .HasKey(ec => new { ec.ExpenseId, ec.CategoryId });
            builder.Entity<ExpenseCategory>()
                .HasOne(b => b.Expense)
                .WithMany(bc => bc.ExpenseCategories)
                .HasForeignKey(b => b.ExpenseId);
            builder.Entity<ExpenseCategory>()
                .HasOne(b => b.Category)
                .WithMany(bc => bc.ExpenseCategories)
                .HasForeignKey(b => b.CategoryId);


            builder.Entity<GoalCategory>()
                .HasKey(gc => new { gc.GoalId, gc.CategoryId });
            builder.Entity<GoalCategory>()
                .HasOne(b => b.Goal)
                .WithMany(bc => bc.GoalCategories)
                .HasForeignKey(b => b.GoalId);
            builder.Entity<GoalCategory>()
                .HasOne(b => b.Category)
                .WithMany(bc => bc.GoalCategories)
                .HasForeignKey(b => b.CategoryId);


            builder.Entity<IncomeCategory>()
                .HasKey(gc => new { gc.IncomeId, gc.CategoryId });
            builder.Entity<IncomeCategory>()
                .HasOne(b => b.Income)
                .WithMany(bc => bc.IncomeCategories)
                .HasForeignKey(b => b.IncomeId);
            builder.Entity<IncomeCategory>()
                .HasOne(b => b.Category)
                .WithMany(bc => bc.IncomeCategories)
                .HasForeignKey(b => b.CategoryId);

        }
    }
}
