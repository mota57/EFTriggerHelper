using Microsoft.EntityFrameworkCore;
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

        public async Task BeforeCreateAsync(DbContext context)
        {
            var methodName = nameof(BeforeCreateAsync);
            await ExecuteBeforeTriggerMethodAsync(context, typeof(IBeforeCreateAsync<>), methodName, EntityState.Added);
        }


        public async Task BeforeUpdateAsync(DbContext context)
        {
            var methodName = nameof(BeforeUpdateAsync);
            await ExecuteBeforeTriggerMethodAsync(context, (typeof(IBeforeUpdateAsync<>)), methodName, EntityState.Modified);
        }


        public async Task BeforeDeleteAsync(DbContext context)
        {
            var methodName = nameof(BeforeDeleteAsync);
            await ExecuteBeforeTriggerMethodAsync(context, (typeof(IBeforeDeleteAsync<>)), methodName, EntityState.Deleted);
        }


        public async Task AfterCreateAsync(DbContext context)
        {
            var methodName = nameof(AfterCreateAsync);
            await ExecuteAfterTriggerMethodAsync(context, typeof(IAfterCreateAsync<>), methodName, TempEntriesAdded);
        }

        public async Task AfterUpdateAsync(DbContext context)
        {
            var methodName = nameof(AfterUpdateAsync);
            await ExecuteAfterTriggerMethodAsync(context, typeof(IAfterUpdateAsync<>), methodName, TempEntriesModified);
        }

        public async Task AfterDeleteAsync(DbContext context)
        {
            var methodName = nameof(AfterDeleteAsync);
            await ExecuteAfterTriggerMethodAsync(context, typeof(IAfterDeleteAsync<>), methodName, TempEntriesDeleted);
        }



        public async Task ExecuteBeforeTriggerMethodAsync(DbContext context, Type typeInstanceHandler, string methodName, EntityState entityState)
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
                var result = (Task)info.Invoke(instance, new object[] { context, entities });
                await result;
            }
        }


        public async Task ExecuteAfterTriggerMethodAsync(DbContext context, Type typeInstanceHandler, string methodName, Dictionary<Type, HashSet<object>> tempTentries)
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
                var result = (Task)info.Invoke(instance, new object[] { context, entities });
                await result;
            }
        }

    }


}
