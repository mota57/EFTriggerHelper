using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EFTriggerHelper
{

    public partial class DbContextTriggerHelper
    {
        private Assembly assembly;


        private Dictionary<Type, HashSet<object>> TempEntriesAdded =
                new Dictionary<Type, HashSet<object>>();

        private Dictionary<Type, HashSet<object>> TempEntriesModified =
             new Dictionary<Type, HashSet<object>>();

        private Dictionary<Type, HashSet<object>> TempEntriesDeleted =
          new Dictionary<Type, HashSet<object>>();

        public DbContextTriggerHelper(Assembly assembly)
        {
            this.assembly = assembly;
        }


        protected internal void BeforeCreate(DbContext context)
        {
            var methodName = nameof(BeforeCreate);
            ExecuteBeforeTriggerMethod(context, typeof(IBeforeCreate<>), methodName, EntityState.Added);
        }


        protected internal void BeforeUpdate(DbContext context)
        {
            var methodName = nameof(BeforeUpdate);
            ExecuteBeforeTriggerMethod(context, (typeof(IBeforeUpdate<>)), methodName, EntityState.Modified);
        }


        protected internal void BeforeDelete(DbContext context)
        {
            var methodName = nameof(BeforeDelete);
            ExecuteBeforeTriggerMethod(context, (typeof(IBeforeDelete<>)), methodName, EntityState.Deleted);
        }


        protected internal void AfterCreate(DbContext context)
        {
            var methodName = nameof(AfterCreate);
            ExecuteAfterTriggerMethod(context, typeof(IAfterCreate<>), methodName, TempEntriesAdded);
        }

        protected internal void AfterUpdate(DbContext context)
        {
            var methodName = nameof(AfterUpdate);
            ExecuteAfterTriggerMethod(context, typeof(IAfterUpdate<>), methodName, TempEntriesModified);
        }

        protected internal void AfterDelete(DbContext context)
        {
            var methodName = nameof(AfterDelete);
            ExecuteAfterTriggerMethod(context, typeof(IAfterDelete<>), methodName, TempEntriesDeleted);
        }


        protected internal void ExecuteBeforeTriggerMethod(DbContext context, Type typeInstanceHandler, string methodName, EntityState entityState)
        {
            var typeList = GetTypesWithInferfaceOfType(typeInstanceHandler);
            foreach (var typeMeta in typeList)
            {
                var type = typeMeta.Implementor;
                var entries = GetEntries(context, typeMeta.EntityTypeArg, entityState);
                if (entries.Count() == 0) continue;

                RecordInCachedEntries(typeMeta, entries, entityState);

                object instance = assembly.CreateInstance(type.FullName, false,
                     BindingFlags.ExactBinding,
                     null, new object[] { }, null, null);

                MethodInfo info = assembly.GetType(type.FullName).GetMethod(methodName);

                IList entities = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeMeta.EntityTypeArg));
                foreach (var entry in entries)
                    entities.Add(entry.Entity);

                if (entities.Count == 0) continue;
                info.Invoke(instance, new object[] { context, entities });
            }
        }


        protected internal void ExecuteAfterTriggerMethod(DbContext context, Type typeInstanceHandler, string methodName, Dictionary<Type, HashSet<object>> tempTentries)
        {
            var typeList = GetTypesWithInferfaceOfType(typeInstanceHandler);
            foreach (var typeMeta in typeList)
            {

                if (!tempTentries.ContainsKey(typeMeta.EntityTypeArg)) continue;

                var type = typeMeta.Implementor;
                var entries = tempTentries[typeMeta.EntityTypeArg].ToList();

                tempTentries[typeMeta.EntityTypeArg].Clear();

                object instance = assembly.CreateInstance(type.FullName, false,
                     BindingFlags.ExactBinding,
                     null, new object[] { }, null, null);

                MethodInfo info = assembly.GetType(type.FullName).GetMethod(methodName);


                IList entities = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeMeta.EntityTypeArg));
                foreach (var entry in entries)
                    entities.Add(entry);

                if (entities.Count == 0) continue;
                info.Invoke(instance, new object[] { context, entities });
            }
        }



        private void RecordInCachedEntries(MetaGenericType typeMeta, IEnumerable<EntityEntry> entries, EntityState entityState)
        {
            var entities = new HashSet<object>(entries.Select(p => p.Entity));

            if (entityState == EntityState.Added && !TempEntriesAdded.ContainsKey(typeMeta.EntityTypeArg))
                TempEntriesAdded.Add(typeMeta.EntityTypeArg, entities);


            if (entityState == EntityState.Modified && !TempEntriesModified.ContainsKey(typeMeta.EntityTypeArg))
                TempEntriesModified.Add(typeMeta.EntityTypeArg, entities);

            if (entityState == EntityState.Deleted && !TempEntriesDeleted.ContainsKey(typeMeta.EntityTypeArg))
                TempEntriesDeleted.Add(typeMeta.EntityTypeArg, entities);
        }




        public IEnumerable<MetaGenericType> GetTypesWithInferfaceOfType(Type typeInterface)
        {
            List<MetaGenericType> typeListMatch = new List<MetaGenericType>();
            var types  = assembly.GetTypes();
            
            foreach(var type in types)
            {
                foreach(var intf in type.GetInterfaces())
                {
                    if(intf.IsGenericType && intf.GetGenericTypeDefinition() == typeInterface)
                    {

                        typeListMatch.Add(new MetaGenericType()
                        {
                            EntityTypeArg = intf.GetGenericArguments().First(),
                            Implementor = type
                        });
                    }
                }
            }
            
            return typeListMatch;
        }

      
        public IEnumerable<EntityEntry> GetEntries(DbContext context, Type type, EntityState entityState)
            => context.ChangeTracker.Entries()
               .Where(p => p.Entity.GetType() == type
                && p.State == entityState);

    }


}
