using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EFTriggerHelper.Extensions
{

    public static class DbContextTriggerHelperExtensions
    {
        public static int SaveChangesHandler(this DbContext ctx, Func<int> handler, DbContextTriggerHelper helper)
        {
            helper.BeforeCreate(ctx);
            helper.BeforeUpdate(ctx);
            helper.BeforeDelete(ctx);
            var result = handler();
            helper.AfterCreate(ctx);
            helper.AfterUpdate(ctx);
            helper.AfterDelete(ctx);
            return result;
        }


        public static async Task<int> SaveChangesHandlerAsync(this DbContext ctx, Func<int> handler, DbContextTriggerHelper helper)
        {
            int result = 0;
            await Task.Run(async () =>
            {
                await helper.BeforeCreateAsync(ctx);
                await helper.BeforeUpdateAsync(ctx);
                await helper.BeforeDeleteAsync(ctx);
            }).ContinueWith((action) => {
                result = handler();
            }).ContinueWith(async (action) =>
            {
                await helper.AfterCreateAsync(ctx);
                await helper.AfterUpdateAsync(ctx);
                await helper.AfterDeleteAsync(ctx);
            });
            return result;
        }

    }


}
