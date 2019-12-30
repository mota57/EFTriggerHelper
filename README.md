# EFTriggerHelper

This is a simple trigger implementation in efcore that you can create for your entities.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Support](#support)
- [MIT License](#License)

## Installation

```sh.
dotnet add package EFTriggerHelper --version 1.0.0
```

## Usage

- copy and paste in your db context the override of  savechanges  in the following example.

```cs
    public class DummyContext : DbContext
    {
        public DbSet<PersonTbl> Person { get; set; }
       

        private DbContextTriggerHelper helper =   new DbContextTriggerHelper(typeof(PersonTrigger).Assembly);

       
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
- Define a class that implements the following interfaces. (Every sync interface has a async interface. The IBeforeCreate is sync and the async interface is IBeforeCreateAsync, it will be the same for the rest of the interfaces)
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
            Console.WriteLine("before update executed");
            entities.First().ModifiedDate = DateTime.Now;
            entities.First().ModifiyBy = "TEST";
        }

      
        public void AfterUpdate(DbContext context,  IEnumerable<PersonTbl> entities) => Console.WriteLine("after update executed");
      
        public void BeforeDelete(DbContext context,  IEnumerable<PersonTbl> entities) =>  Console.WriteLine("before delete executed");

        public void AfterDelete(DbContext context,  IEnumerable<PersonTbl> entities) =>  Console.WriteLine("after delete executed");
    }

```
- Make sure to pass as parameter the assembly of one concrete trigger class. 
- Make sure to organize your trigger classes in one assembly.

```cs
        private DbContextTriggerHelper helper =  helper = new DbContextTriggerHelper(typeof(PersonTrigger).Assembly);
```

Execute triggers by calling SaveChanges or SaveChangesAsync() in the derived DbContext 
```.cs
public class PersonController : Controller

public IActionResult CreatePerson(PersonTbl person)
{ 
    yourDbContext.Person.Add(person);
    YourDbContext.SaveChanges() // or await yourDbContext.SaveChangesAsync() 
}
```

## Support

Please [open an issue](https://github.com/mota57/EFTriggerHelper/issues/new) for support.


## License

MIT License

Copyright (c) [2019] [Hector Mota]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

