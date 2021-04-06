using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class InscriptionTest
    {
        Inscripción testInscription;
        List<Inscripción> testInscriptionList;
        private readonly DateTime thisDay = DateTime.Today;

        [TestInitialize]
        public void TestInitialize()
        {
            testInscription = new Inscripción
            {
                Estatus = "Test",
                Fecha = thisDay,
                Periodo = "FEB2021 - JUL2021",
                Tipo = "Example 1",
            };

            testInscriptionList = new List<Inscripción>()
            {
                new Inscripción
                {
                    Estatus = "Test",
                    Fecha = thisDay,
                    Periodo = "FEB2021 - JUL2021",
                    Tipo = "Example 2",
                },
                new Inscripción
                {
                    Estatus = "Test",
                    Fecha = thisDay,
                    Periodo = "FEB2021 - JUL2021",
                    Tipo = "Example 3",
                }
            };
        }

        [TestMethod]
        public void AddNewInscription_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Inscripción.Add(testInscription);
                context.SaveChanges();
                var expected = context.Inscripción.First(i => i.Tipo.Equals(testInscription.Tipo));
                Assert.AreEqual(expected.Tipo, testInscription.Tipo);
            }
        }

        [TestMethod]
        public void AddRangeInscriptions_Success()
        {
            using (SCPPContext context = new SCPPContext()) 
            {
                context.Inscripción.AddRange(testInscriptionList);
                context.SaveChanges();
                var expected = context.Inscripción.First(i => i.Tipo.Equals("Example 2"));
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Tipo, "Example 2");
            }
        }

        [TestMethod]
        public void Find_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.FirstOrDefault(i => i.Tipo.Equals("Example 1"));
                 Assert.IsNotNull(inscriptionRetrieved);
                Assert.AreEqual("Example 1", inscriptionRetrieved.Tipo);
            }
        }

        [TestMethod]
        public void GetAllInscriptions_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.ToList();
                int expected = 2;
                int actual = inscriptionRetrieved.Count();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.FirstOrDefault(i => i.Tipo.Equals("Example 1"));
                Assert.IsNotNull(inscriptionRetrieved);

                context.Inscripción.Remove(inscriptionRetrieved);
                context.SaveChanges();
                var inscriptionRemoved = context.Inscripción.FirstOrDefault(i => i.Tipo.Equals("Example 1"));
                Assert.IsNull(inscriptionRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionList = context.Inscripción.Where(i => i.Estatus.Equals("Test"));
                context.Inscripción.RemoveRange(inscriptionList);
                context.SaveChanges();

                var inscriptionRemoved = context.Inscripción.FirstOrDefault(i => i.Estatus.Equals("Test"));
                Assert.IsNull(inscriptionRemoved);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.Find(0);
                Assert.IsNull(inscriptionRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullInscription_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Inscripción.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeInscriptions_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Inscripción.AddRange(null);
                context.SaveChanges();
            }
        }
    }
}
