# EFTriggerHelper

This is a simple trigger implementation in efcore that you can create for your entities.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Support](#support)

## Installation

```sh
Download the project and add the reference of DbContextTriggerHelper to your project.
```

## Usage

In your db context add the field/property `DbContextTriggerHelper` and override the SaveChanges like so (copy and paste in your db context the savechanges)

```cs
    public class DummyContext : DbContext
    {
        public DbSet<PersonTbl> Person { get; set; }
       

        private DbContextTriggerHelper helper =  helper = new DbContextTriggerHelper(typeof(PersonTrigger).Assembly);

       
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

```

Define a class that implements the following interfaces.
```cs
 public class PersonTrigger :
            IBeforeCreateAsync<PersonTbl>,
            IBeforeCreate<PersonTbl>,
            IAfterCreate<PersonTbl>,
            IBeforeUpdate<PersonTbl>,
            IAfterUpdate<PersonTbl>,
            IBeforeDelete<PersonTbl>,
            IAfterDelete<PersonTbl>
    {
        public async Task BeforeCreateAsync(DbContext context,  IEnumerable<PersonTbl> entities) => Console.WriteLine("before create async"); 

        public void BeforeCreate(DbContext context,  IEnumerable<PersonTbl> entities)
        {
             Console.WriteLine("before create executed");
            entities.First().CreatedDate = DateTime.Now;
        }
        
        public void AfterCreate(DbContext context,  IEnumerable<PersonTbl> entities) =>  Console.WriteLine("after create executed");


        public void BeforeUpdate(DbContext context,  IEnumerable<PersonTbl> entities)
        {
            Console.WriteLine("before update executed);
            entities.First().ModifiedDate = DateTime.Now;
            entities.First().ModifiyBy = "TEST";
        }

      
        public void AfterUpdate(DbContext context,  IEnumerable<PersonTbl> entities) => Console.WriteLine("after update executed);
      
        public void BeforeDelete(DbContext context,  IEnumerable<PersonTbl> entities) =>  Console.WriteLine("before delete executed");

        public void AfterDelete(DbContext context,  IEnumerable<PersonTbl> entities) =>  Console.WriteLine("after delete executed");
    }

```


Execute triggers
```
YourDbContext.SaveChanges() or YourDbContext.SaveChangesAsync() 
```

## Support

Please [open an issue](https://github.com/mota57/EFTriggerHelper/issues/new) for support.

