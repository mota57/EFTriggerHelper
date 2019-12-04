using Microsoft.EntityFrameworkCore;
using System;

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

    }


}
