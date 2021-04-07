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
        Expediente testExpediente;
        Inscripción testInscription;
        List<Expediente> testExpedienteList;
        private readonly DateTime thisDay = DateTime.Today;

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
                context.Expediente.Add(testExpediente);
                context.SaveChanges();
                var expected = context.Expediente.First(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                Assert.AreEqual(expected.InscripciónID, testInscription.InscripciónID);
            }
        }

        [TestMethod]
        public void AddRangeExpedientes_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Expediente.AddRange(testExpedienteList);
                context.SaveChanges();
                var expected = context.Expediente.FirstOrDefault(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.InscripciónID, testInscription.InscripciónID);
            }
        }

        [TestMethod]
        public void Find_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.FirstOrDefault(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                Assert.IsNotNull(expedienteRetrieved);
                Assert.AreEqual(testInscription.InscripciónID, expedienteRetrieved.ExpedienteID);
            }
        }

        [TestMethod]
        public void GetAllExpedientes_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.ToList();
                int expected = 2;
                int actual = expedienteRetrieved.Count();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var expedienteRetrieved = context.Expediente.FirstOrDefault(e => e.InscripciónID.Equals(testInscription.InscripciónID));
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
                var expedienteList = context.Expediente.Where(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                context.Expediente.RemoveRange(expedienteList);
                context.SaveChanges();
                DeleteInscription();
                var expedienteRemoved = context.Inscripción.FirstOrDefault(e => e.InscripciónID.Equals(testInscription.InscripciónID));
                Assert.IsNull(expedienteRemoved);
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