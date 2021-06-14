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
        private Inscripción testInscription;
        private List<Inscripción> testInscriptionList;
        private readonly DateTime thisDay = DateTime.Today;
        private static int expectedInDB;
        private static int soloID;
        private static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testInscription = new Inscripción
            {
                Estatus = "TestSolo",
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
                expectedInDB = context.Inscripción.ToList().Count();
                context.Inscripción.Add(testInscription);
                context.SaveChanges();
                soloID = testInscription.InscripciónID;
                var expected = context.Inscripción.Find(testInscription.InscripciónID);
                Assert.AreEqual(expected.InscripciónID, testInscription.InscripciónID);
            }
        }

        [TestMethod]
        public void AddRangeInscriptions_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Inscripción.AddRange(testInscriptionList);
                context.SaveChanges();
                duoID[0] = testInscriptionList[0].InscripciónID;
                duoID[1] = testInscriptionList[1].InscripciónID;
                var expected = context.Inscripción.Find(testInscriptionList[1].InscripciónID);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Tipo, testInscriptionList[1].Tipo);
            }
        }

        [TestMethod]
        public void Find_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.Find(soloID);
                Assert.IsNotNull(inscriptionRetrieved);
                Assert.AreEqual(inscriptionRetrieved.Tipo, testInscription.Tipo);
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

        [TestMethod]
        public void GetAllInscriptions_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Inscripción.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRetrieved = context.Inscripción.Find(soloID);
                Assert.IsNotNull(inscriptionRetrieved);

                context.Inscripción.Remove(inscriptionRetrieved);
                context.SaveChanges();
                var inscriptionRemoved = context.Inscripción.Find(soloID);
                Assert.IsNull(inscriptionRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var inscriptionList = context.Inscripción.Where(i => i.Estatus.Equals("Test"));
                context.Inscripción.RemoveRange(inscriptionList);
                context.SaveChanges();

                var inscriptionRemoved = context.Inscripción.Find(duoID[0]);
                Assert.IsNull(inscriptionRemoved);
            }
        }
    }
}