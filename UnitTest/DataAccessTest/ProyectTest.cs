using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ProyectTest
    {
        private Proyecto testProyect;
        private List<Proyecto> testProyectList;
        private int expectedInDB;
        private readonly DateTime thisDay = DateTime.Today;
        private static int soloID;
        private static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testProyect = new Proyecto()
            {
                Actividades = "5 Actividades",
                Descripcion = "Proyecto de hotel",
                Fecharegistro = thisDay,
                Noestudiantes = 20,
                Nombre = "Hotel Paradise",
                ResponsableproyectoID = 1
        };

            testProyectList = new List<Proyecto>()
            {
                new Proyecto
                {
                    Actividades = "4 Actividades",
                    Descripcion = "Proyecto de CocaCola",
                    Fecharegistro = thisDay,
                    Noestudiantes = 20,
                    Nombre = "Web Store",
                    ResponsableproyectoID = 1
                },

                new Proyecto
                {
                    Actividades = "4 Actividades",
                    Descripcion = "Proyecto de Call Of Duty",
                    Fecharegistro = thisDay,
                    Noestudiantes = 20,
                    Nombre = "Web Warzone",
                    ResponsableproyectoID = 1
        }
            };
        }

        [TestMethod]
        public void AddNewProyect_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Proyecto.ToList().Count();
                context.Proyecto.Add(testProyect);
                context.SaveChanges();
                soloID = testProyect.Clave;
                var expected = context.Proyecto.Find(testProyect.Clave);
                Assert.AreEqual(expected.Clave, testProyect.Clave);
            }
        }

        [TestMethod]
        public void AddRangeProyects_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Proyecto.AddRange(testProyectList);
                context.SaveChanges();
                duoID[0] = testProyectList[0].Clave;
                duoID[1] = testProyectList[1].Clave;
                var expected = context.Proyecto.Find(testProyectList[1].Clave);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, testProyectList[1].Nombre);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(soloID);
                Assert.IsNotNull(proyectRetrieved);
                Assert.AreEqual(proyectRetrieved.Nombre, testProyect.Nombre);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(0);
                Assert.IsNull(proyectRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullProyect_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Proyecto.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeProyects_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Proyecto.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAllProyects_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Proyecto.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(soloID);
                Assert.IsNotNull(proyectRetrieved);

                context.Proyecto.Remove(proyectRetrieved);
                context.SaveChanges();
                var proyectRemoved = context.Proyecto.Find(soloID);
                Assert.IsNull(proyectRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var proyectList = context.Proyecto.Where(p => p.Clave >= (tmp));
                context.Proyecto.RemoveRange(proyectList);
                context.SaveChanges();

                var proyectRemoved = context.Proyecto.Find(duoID[0]);
                Assert.IsNull(proyectRemoved);
            }
        }
    }
}