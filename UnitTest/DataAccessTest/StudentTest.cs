using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using SCPP.DataAcces;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class StudentTest
    {
        Estudiante testStudent;
        List<Estudiante> testStudentsList;
        int expectedInDB;

        [TestInitialize]
        public void TestInitialize()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Estudiante.ToList().Count();
            }
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
                    Correopersonal = "omarini2k@gmail.com",
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
                    Correopersonal = "omarini2k@gmail.com",
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
                context.Estudiante.Add(testStudent);
                context.SaveChanges();
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
                var studentRetrieved = context.Estudiante.Find(testStudentsList[0].Matricula);
                Assert.IsNotNull(studentRetrieved);
                Assert.AreEqual(studentRetrieved.Apellidopaterno, testStudentsList[0].Apellidopaterno);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var studentRetrieved = context.Estudiante.Find(testStudentsList[1].Matricula + 1);
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
                var studentRetrieved = context.Estudiante.Find(testStudent.Matricula);
                Assert.IsNotNull(studentRetrieved);

                context.Estudiante.Remove(studentRetrieved);
                context.SaveChanges();
                var studentRemoved = context.Estudiante.Find(testStudent.Matricula);
                Assert.IsNull(studentRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var studentsList = context.Estudiante.Where(p => p.Promedio < 8);
                context.Estudiante.RemoveRange(studentsList);
                context.SaveChanges();

                var studentRemoved = context.Estudiante.Find("S18012186");
                Assert.IsNull(studentRemoved);
            }
        }


    }
}
