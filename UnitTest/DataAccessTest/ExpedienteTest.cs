using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ExpedienteTest
    {
        private Expediente testExpediente;
        private Inscripción testInscription;
        private List<Expediente> testExpedienteList;
        private readonly DateTime thisDay = DateTime.Today;
        private static int expectedInDB;
        private static int soloID;
        private static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            CreateTestInscription();

            testExpediente = new Expediente()
            {
                Fechafinpp = null,
                Fechainiciopp = thisDay,
                Horasacumuladas = 84101,
                Numreportesentregados = 1000,
                InscripciónID = testInscription.InscripciónID
            };

            testExpedienteList = new List<Expediente>()
            {
                new Expediente
                {
                    Fechafinpp = null,
                    Fechainiciopp = thisDay.AddDays(2),
                    Horasacumuladas = 84101,
                    Numreportesentregados = 2000,
                    InscripciónID = testInscription.InscripciónID
        },

                new Expediente
                {
                    Fechafinpp = null,
                    Fechainiciopp = thisDay.AddDays(3),
                    Horasacumuladas = 84101,
                    Numreportesentregados = 2000,
                    InscripciónID = testInscription.InscripciónID
        }
            };
        }

        [TestMethod]
        public void AddNewExpediente_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Expediente.ToList().Count();
                context.Expediente.Add(testExpediente);
                context.SaveChanges();
                soloID = testExpediente.ExpedienteID;
                var expected = context.Expediente.Find(testExpediente.ExpedienteID);
                Assert.AreEqual(expected.ExpedienteID, testExpediente.ExpedienteID);
            }
        }

        [TestMethod]
        public void AddRangeExpedientes_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Expediente.AddRange(testExpedienteList);
                context.SaveChanges();
                duoID[0] = testExpedienteList[0].ExpedienteID;
                duoID[1] = testExpedienteList[1].ExpedienteID;
                var expected = context.Expediente.Find(testExpedienteList[1].ExpedienteID);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.ExpedienteID, testExpedienteList[1].ExpedienteID);
            }
        }

        [TestMethod]
        public void Find_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.Find(soloID);
                Assert.IsNotNull(expedienteRetrieved);
                Assert.AreEqual(expedienteRetrieved.Numreportesentregados, testExpediente.Numreportesentregados);
            }
        }

        [TestMethod]
        public void GetAllExpedientes_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Expediente.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.Find(0);
                Assert.IsNull(expedienteRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullInscription_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Expediente.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeInscriptions_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Expediente.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.FirstOrDefault(e => e.Numreportesentregados == (1000));
                Assert.IsNotNull(expedienteRetrieved);

                context.Expediente.Remove(expedienteRetrieved);
                context.SaveChanges();
                DeleteInscription();
                var expedienteRemoved = context.Expediente.FirstOrDefault(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                Assert.IsNull(expedienteRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var expedienteList = context.Expediente.Where(e => e.Numreportesentregados == (2000));
                context.Expediente.RemoveRange(expedienteList);
                context.SaveChanges();
                //DeleteInscription();
                var expedienteRemoved = context.Expediente.Find(duoID[0]);
                Assert.IsNull(expedienteRemoved);
            }
        }

        public void CreateTestInscription()
        {
            using (SCPPContext context = new SCPPContext())
            {
                testInscription = context.Inscripción.FirstOrDefault(i => i.Estatus.Equals("TestExpediente"));
                if (testInscription == null)
                {
                    Inscripción inscripcion = new Inscripción()
                    {
                        Estatus = "TestExpediente",
                        Fecha = thisDay,
                        Periodo = "FEB2021 - JUL2021",
                        Tipo = "Example 1"
                    };
                    context.Inscripción.Add(inscripcion);
                    context.SaveChanges();
                    testInscription = context.Inscripción.FirstOrDefault(i => i.Estatus.Equals("TestExpediente"));
                }
            }
        }

        public void DeleteInscription()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var inscriptionRemoved = context.Inscripción.FirstOrDefault(i => i.Estatus.Equals("TestExpediente"));
                context.Inscripción.Remove(inscriptionRemoved);
                context.SaveChanges();
            }
        }
    }
}