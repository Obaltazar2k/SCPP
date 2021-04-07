using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class StudentTest
    {
        private Estudiante testStudent;
        private List<Estudiante> testStudentsList;
        private int expectedInDB;
        private static string soloID;
        private static string[] duoID = new string[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testStudent = new Estudiante()
            {
                Correopersonal = "omarini2k@gmail.com",
                Apellidopaterno = "Hernández",
                Apellidomaterno = "Gutiérrez",
                Nombre = "Omar",
                Matricula = "S18012184",
                Estado = "Inscrito",
                Promedio = 9.5,
                Telefono = "2281186105",
                Contraseña = "Jinchuriki2k"
            };

            testStudentsList = new List<Estudiante>()
            {
                new Estudiante
                {
                    Correopersonal = "test@gmail.com",
                    Apellidopaterno = "Hernández",
                    Apellidomaterno = "Juárez",
                    Nombre = "Ernesto",
                    Matricula = "S18012185",
                    Estado = "Inscrito",
                    Promedio = 7.0,
                    Telefono = "2282187155",
                    Contraseña = "Jinchuriki2k"
                },
                new Estudiante
                {
                    Correopersonal = "test@gmail.com",
                    Apellidopaterno = "Jímenez",
                    Apellidomaterno = "Gutiérrez",
                    Nombre = "Adrian",
                    Matricula = "S18012186",
                    Estado = "Inscrito",
                    Promedio = 7.5,
                    Telefono = "2283196105",
                    Contraseña = "Jinchuriki2k"
                }
            };
        }

        [TestMethod]
        public void AddNewStudent_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Estudiante.ToList().Count();
                context.Estudiante.Add(testStudent);
                context.SaveChanges();
                soloID = testStudent.Matricula;
                var expected = context.Estudiante.Find(testStudent.Matricula);
                Assert.AreEqual(expected.Matricula, testStudent.Matricula);
            }
        }

        [TestMethod]
        public void AddRangeStudents_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Estudiante.AddRange(testStudentsList);
                context.SaveChanges();
                duoID[0] = testStudentsList[0].Matricula;
                duoID[1] = testStudentsList[1].Matricula;
                var expected = context.Estudiante.Find(testStudentsList[0].Matricula);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, testStudentsList[0].Nombre);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var studentRetrieved = context.Estudiante.Find(soloID);
                Assert.IsNotNull(studentRetrieved);
                Assert.AreEqual(studentRetrieved.Apellidopaterno, testStudentsList[0].Apellidopaterno);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var studentRetrieved = context.Estudiante.Find("");
                Assert.IsNull(studentRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullStudent_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Estudiante.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeStudents_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Estudiante.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAllStudents_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Estudiante.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var studentRetrieved = context.Estudiante.Find(soloID);
                Assert.IsNotNull(studentRetrieved);

                context.Estudiante.Remove(studentRetrieved);
                context.SaveChanges();
                var studentRemoved = context.Estudiante.Find(soloID);
                Assert.IsNull(studentRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var studentsList = context.Estudiante.Where(p => p.Correopersonal.Equals("test@gmail.com"));
                context.Estudiante.RemoveRange(studentsList);
                context.SaveChanges();

                var studentRemoved = context.Estudiante.Find(duoID[0]);
                Assert.IsNull(studentRemoved);
            }
        }
    }
}