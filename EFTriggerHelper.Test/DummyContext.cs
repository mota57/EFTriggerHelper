using EFTriggerHelper.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

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
            return this.SaveChangesHandler(handler,  helper);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Func<int> handler = () => base.SaveChanges(acceptAllChangesOnSuccess);
            return this.SaveChangesHandler(handler, helper);
        }

        //public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    await helper.BeforeCreateAsync(this);
        //    return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    await helper.BeforeCreateAsync(this);
        //    return await base.SaveChangesAsync(cancellationToken);
        //}

    }
}
