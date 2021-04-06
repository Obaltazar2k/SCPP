using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using SCPP.DataAcces;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ProfesorTest
    {
        Profesor testProfesor;
        List<Profesor> testProfesorsList;
        int expectedInDB;

        [TestInitialize]
        public void TestInitialize()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Profesor.ToList().Count();
            }
            testProfesor = new Profesor()
            {
                Correopersonal = "adrian@gmail.com",
                Apellidopaterno = "Hernández",
                Apellidomaterno = "Gutiérrez",
                Nombre = "Adrian",
                Rfc = "HEGA201093H01",
                Contraseña = "Jinchuriki2k"
            };

            testProfesorsList = new List<Profesor>()
            {
                new Profesor
                {
                    Correopersonal = "test@gmail.com",
                    Apellidopaterno = "Jímenez",
                    Apellidomaterno = "Galván",
                    Nombre = "Erick",
                    Rfc = "JIGE201093H01",
                    Contraseña = "KakashiHatake"
                },
                new Profesor
                {
                    Correopersonal = "test@gmail.com",
                    Apellidopaterno = "Lagunes",
                    Apellidomaterno = "Martínez",
                    Nombre = "Joel",
                    Rfc = "LAMJ201093H01",
                    Contraseña = "KakashiHatake"
                }
            };
        }

        [TestMethod]
        public void AddNewProfesor_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Profesor.Add(testProfesor);
                context.SaveChanges();
                var expected = context.Profesor.Find(testProfesor.Rfc);
                Assert.AreEqual(expected.Rfc, testProfesor.Rfc);
            }
        }

        [TestMethod]
        public void AddRangeProfesors_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Profesor.AddRange(testProfesorsList);
                context.SaveChanges();
                var expected = context.Profesor.Find(testProfesorsList[0].Rfc);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, testProfesorsList[0].Nombre);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesorRetrieved = context.Profesor.Find(testProfesorsList[1].Rfc);
                Assert.IsNotNull(profesorRetrieved);
                Assert.AreEqual(profesorRetrieved.Apellidopaterno, testProfesorsList[1].Apellidopaterno);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesorRetrieved = context.Profesor.Find(testProfesorsList[1].Rfc + 1);
                Assert.IsNull(profesorRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullProfesor_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Profesor.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeProfesors_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Profesor.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAllProfesors_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Profesor.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesorRetrieved = context.Profesor.Find(testProfesor.Rfc);
                Assert.IsNotNull(profesorRetrieved);

                context.Profesor.Remove(profesorRetrieved);
                context.SaveChanges();
                var profesorRemoved = context.Profesor.Find(testProfesor.Rfc);
                Assert.IsNull(profesorRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesorsList = context.Profesor.Where(p => p.Correopersonal.Equals("test@gmail.com"));
                context.Profesor.RemoveRange(profesorsList);
                context.SaveChanges();

                var studentRemoved = context.Profesor.Find("LAMJ201093H01");
                Assert.IsNull(studentRemoved);
            }
        }


    }
}
