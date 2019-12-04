using EFTriggerHelper.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EFTriggerHelper.Test
{
    
    public class DummyContext : DbContext
    {
        public DbSet<PersonTbl> Person { get; set; }
        public DbSet<LogTbl> LogTbl { get; set; }

        public DummyContext(DbContextOptions<DummyContext> options)
            : base(options)
        {
            helper = new DbContextTriggerHelper(typeof(PersonTrigger).Assembly);
        }


        private DbContextTriggerHelper helper { get; set; }

        public override int SaveChanges()
        {
            Func<int> handler = ()  => base.SaveChanges();
            return this.EFTriggerHelperSaveChanges(handler,  helper);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Func<int> handler = () => base.SaveChanges(acceptAllChangesOnSuccess);
            return this.EFTriggerHelperSaveChanges(handler, helper);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<int>> handler =  async () => await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            return await this.EFTriggerHelperSaveChangesAsync(handler, helper);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Func<Task<int>> handler =  async () => await base.SaveChangesAsync(cancellationToken);
            return await this.EFTriggerHelperSaveChangesAsync(handler, helper);
        }

    }
}
