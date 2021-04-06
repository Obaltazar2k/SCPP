using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using SCPP.DataAcces;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ProyectTest
    {
        Proyecto testProyect;
        List<Proyecto> testProyectList;
        private readonly DateTime thisDay = DateTime.Today;


        [TestInitialize]
        public void TestInitialize()
        {
            testProyect = new Proyecto()
            {
                Actividades = "5 Actividades",
                Clave = 1,
                Descripcion = "Proyecto de hotel",
                Fecharegistro = thisDay,
                Noestudiantes = 20,
                Nombre = "Hotel Paradise",
                Resbonsablenombre = "Aldo Colorado Díaz"
            };

            testProyectList = new List<Proyecto>()
            {
                new Proyecto
                {
                    Actividades = "4 Actividades",
                    Clave = 0,
                    Descripcion = "Proyecto de CocaCola",
                    Fecharegistro = thisDay,
                    Noestudiantes = 20,
                    Nombre = "Web Store",
                    Resbonsablenombre = "Jon Snow"
                },

                new Proyecto
                {
                    Actividades = "4 Actividades",
                    Clave = 0,
                    Descripcion = "Proyecto de Call Of Duty",
                    Fecharegistro = thisDay,
                    Noestudiantes = 20,
                    Nombre = "Web Warzone",
                    Resbonsablenombre = "Arya Stark"
                }
            };
        }

        [TestMethod]
        public void AddNewProyect_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Proyecto.Add(testProyect);
                context.SaveChanges();
                var expected = context.Proyecto.Find(testProyect.Clave);
                Assert.AreEqual(expected.Clave, testProyect.Clave);
            }
        }

        [TestMethod]
        public void AddRangeProyect_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Proyecto.AddRange(testProyectList);
                context.SaveChanges();
                var expected = context.Proyecto.Find(2);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, "Web Store");
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(testProyect.Clave);
                Assert.IsNotNull(proyectRetrieved);

                context.Proyecto.Remove(proyectRetrieved);
                context.SaveChanges();
                var proyectRemoved = context.Proyecto.Find(testProyect.Clave);
                Assert.IsNull(proyectRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var proyectList = context.Proyecto.Where(p => p.Resbonsablenombre == "Jon Snow");
                context.Proyecto.RemoveRange(proyectList);
                context.SaveChanges();

                var proyectRemoved = context.Proyecto.Find(2);
                Assert.IsNull(proyectRemoved);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(3);
                Assert.IsNotNull(proyectRetrieved);
                Assert.AreEqual("Web Warzone", proyectRetrieved.Nombre);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.Find(1);
                Assert.IsNull(proyectRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullProyect_DoesNotAffectDatabase()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Proyecto.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeProyects_DoesNotAffectDatabase()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Proyecto.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAllProyects_Succes()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var proyectRetrieved = context.Proyecto.ToList();
                int expected = 5;
                int actual = proyectRetrieved.Count();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}

