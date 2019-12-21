using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;


namespace EFTriggerHelper.Test
{
    /// <summary>
    /// create, update, delete
    /// </summary>
    public class PersonTrigger : IBeforeCreate<PersonTbl>, IAfterCreate<PersonTbl>, IBeforeUpdate<PersonTbl>, 
                                 IAfterUpdate<PersonTbl>, IBeforeDelete<PersonTbl>, IAfterDelete<PersonTbl>
    {
        
        public void AfterCreate(DbContext context, IEnumerable<PersonTbl> entities)
        {
            var ctx = (context as DummyContext);
            ctx.LogTbl.Add(new LogTbl() { Description = $"new entity added {entities.First().PersonTblId}" });
            ctx.SaveChanges();
        }


        public void BeforeCreate(DbContext context, IEnumerable<PersonTbl> entities)
        {
            entities.First().Name = "TEST";
            entities.First().CreatedDate = DateTime.Now;
        }


        public void AfterUpdate(DbContext context, IEnumerable<PersonTbl> entities)
        {
            EntityAfterUpdate = entities.First();
        }

        public void BeforeUpdate(DbContext context, IEnumerable<PersonTbl> entities)
        {
            entities.First().ModifiedDate = DateTime.Now;
            entities.First().ModifiyBy = "TEST";
        }

        public void AfterDelete(DbContext context, IEnumerable<PersonTbl> entities)
        {
            EntityBeforeDelete = entities.First();
        }

        public void BeforeDelete(DbContext context, IEnumerable<PersonTbl> entities)
        {
            EntityAfterDelete = entities.First();
        }

        public static PersonTbl EntityAfterUpdate = null;
        public static PersonTbl EntityBeforeDelete = null;
        public static PersonTbl EntityAfterDelete = null;

    }

}
