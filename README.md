# EFTriggerHelper

This is a simple trigger implementation in efcore that you can create for your entities.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Support](#support)

## Installation

```sh
 dotnet add package EFTriggerHelper
```

## Usage

In your db context add the field/property `DbContextTriggerHelper` and override the SaveChanges like so

```cs
    public class DummyContext : DbContext
    {
        public DbSet<PersonTbl> Person { get; set; }
       

        private DbContextTriggerHelper helper =  helper = new DbContextTriggerHelper(typeof(PersonTrigger).Assembly);

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

    
    }

```

Define a class that implements the following interfaces.
```cs
 public class PersonTrigger :
            IBeforeCreate<PersonTbl>,
            IAfterCreate<PersonTbl>,
            IBeforeUpdate<PersonTbl>,
            IAfterUpdate<PersonTbl>,
            IBeforeDelete<PersonTbl>,
            IAfterDelete<PersonTbl>
    {

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


## Support

Please [open an issue](https://github.com/mota57/EFTriggerHelper/issues/new) for support.

