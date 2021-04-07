using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ProfesorTest
    {
        private Profesor testProfesor;
        private List<Profesor> testProfesorsList;
        private int expectedInDB;
        private static string soloID;
        private static string[] duoID = new string[2];

        [TestInitialize]
        public void TestInitialize()
        {
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
                expectedInDB = context.Profesor.ToList().Count();
                context.Profesor.Add(testProfesor);
                context.SaveChanges();
                soloID = testProfesor.Rfc;
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
                duoID[0] = testProfesorsList[0].Rfc;
                duoID[1] = testProfesorsList[1].Rfc;
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
                var profesorRetrieved = context.Profesor.Find(soloID);
                Assert.IsNotNull(profesorRetrieved);
                Assert.AreEqual(profesorRetrieved.Apellidopaterno, testProfesor.Apellidopaterno);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var profesorRetrieved = context.Profesor.Find("");
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
                var profesorRetrieved = context.Profesor.Find(soloID);
                Assert.IsNotNull(profesorRetrieved);

                context.Profesor.Remove(profesorRetrieved);
                context.SaveChanges();
                var profesorRemoved = context.Profesor.Find(soloID);
                Assert.IsNull(profesorRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var profesorsList = context.Profesor.Where(p => p.Correopersonal.Equals("test@gmail.com"));
                context.Profesor.RemoveRange(profesorsList);
                context.SaveChanges();

                var studentRemoved = context.Profesor.Find(duoID[0]);
                Assert.IsNull(studentRemoved);
            }
        }
    }
}