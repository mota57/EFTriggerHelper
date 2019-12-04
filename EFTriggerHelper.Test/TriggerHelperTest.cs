using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;


namespace EFTriggerHelper.Test
{

    [TestClass]
    public class TriggerHelperTest
    {
        [TestMethod]
        public void TestCanGetConcreteTypes()
        {

            var assembly = typeof(PersonTrigger).Assembly;

            DbContextTriggerHelper helper = new DbContextTriggerHelper(assembly);

            IEnumerable<MetaGenericType> types = helper.GetTypesWithInferfaceOfType(typeof(IBeforeCreate<>));

            Assert.IsTrue(types.Any(p => p.Implementor == typeof(PersonTrigger) && p.EntityTypeArg == typeof(PersonTbl)));
        }

        [TestMethod]
        public void TestBeforeAfterDeleteTrigger()
        {
            TestHandler.Handle<DummyContext>((options) =>
            {
                using (var context = new DummyContext(options))
                {
                    var p = new PersonTbl();
                    context.Add(p);
                    context.SaveChanges();
                }
                using (var context = new DummyContext(options))
                {
                    var p = context.Person.First();
                    context.Person.Remove(p);
                    context.SaveChanges();
                    Assert.AreEqual(p.PersonTblId, PersonTrigger.EntityBeforeDelete.PersonTblId);
                    Assert.AreEqual(p.PersonTblId, PersonTrigger.EntityAfterDelete.PersonTblId);
                }
            });

        }


        [TestMethod]
        public void TestBeforeAfterCreateTrigger()
        {
            TestHandler.Handle<DummyContext>((options) =>
            {
                using (var context = new DummyContext(options))
                {
                    var p = new PersonTbl();
                    context.Add(p);
                    context.SaveChanges();
                }
                using (var context = new DummyContext(options))
                {
                    var p = context.Person.First();
                    var log = context.LogTbl.First();

                    Assert.IsTrue(log.Description.StartsWith("new entity added"));
                    Assert.AreEqual("TEST", p.Name);
                    Assert.AreEqual(DateTime.Now.Year, p.CreatedDate.Value.Year);
                }
            });

        }

        [TestMethod]
        public void TestBeforeAfterUpdateTrigger()
        {
            TestHandler.Handle<DummyContext>((options) =>
            {
                using (var context = new DummyContext(options))
                {
                    var p = new PersonTbl();
                    context.Add(p);
                    context.SaveChanges();
                }
                using (var context = new DummyContext(options))
                {
                    var p = context.Person.First();
                    p.Name = "change";
                    context.SaveChanges();
                    Assert.AreEqual("TEST", p.ModifiyBy);
                    Assert.IsTrue(PersonTrigger.EntityAfterUpdate.PersonTblId == p.PersonTblId);
                }

            });

        }




    }
}
