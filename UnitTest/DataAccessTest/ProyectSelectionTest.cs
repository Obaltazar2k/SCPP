using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class ProyectSelectionTest
    {
        private Estudiante testStudent;
        private Proyecto testProyect;
        private Selecciónproyecto testSelection;
        private List<Selecciónproyecto> testSelectionList;
        private readonly DateTime thisDay = DateTime.Today;
        private static int expectedInDB;
        private static int soloID;
        private static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testSelection = new Selecciónproyecto()
            {
                Claveproyecto = 0,
                Fecha = thisDay,
                Matriculaestudiante = "STest",
                PeriodoID = "AGO2021-ENE2022",
                Estudiante = testStudent,
                Proyecto = testProyect
            };

            testSelectionList = new List<Selecciónproyecto>()
            {
                new Selecciónproyecto
                {
                    Claveproyecto = 0,
                    Fecha = thisDay,
                    Matriculaestudiante = "STest",
                    PeriodoID = "AGO2021-ENE2022",
                    Estudiante = testStudent,
                    Proyecto = testProyect
                },

                new Selecciónproyecto
                {
                    Claveproyecto = 0,
                    Fecha = thisDay,
                    Matriculaestudiante = "STest",
                    PeriodoID = "AGO2021-ENE2022",
                    Estudiante = testStudent,
                    Proyecto = testProyect
                }
            };
        }

        [TestMethod]
        public void AddNewSelection_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Selecciónproyecto.ToList().Count();
                context.Selecciónproyecto.Add(testSelection);
                context.SaveChanges();
                soloID = testSelection.SelecciónproyectoID;
                var expected = context.Selecciónproyecto.Find(testSelection.SelecciónproyectoID);
                Assert.AreEqual(expected.SelecciónproyectoID, testSelection.SelecciónproyectoID);
            }
        }

        [TestMethod]
        public void AddRangeSelection_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Selecciónproyecto.AddRange(testSelectionList);
                context.SaveChanges();
                duoID[0] = testSelectionList[0].SelecciónproyectoID;
                duoID[1] = testSelectionList[1].SelecciónproyectoID;
                var expected = context.Selecciónproyecto.Find(testSelectionList[1].SelecciónproyectoID);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.PeriodoID, testSelectionList[1].PeriodoID);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var selectionRetrieved = context.Selecciónproyecto.Find(soloID);
                Assert.IsNotNull(selectionRetrieved);
                Assert.AreEqual(selectionRetrieved.PeriodoID, testSelection.PeriodoID);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var selectionRetrieved = context.Grupo.Find(0);
                Assert.IsNull(selectionRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullGroup_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Selecciónproyecto.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeGroups_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Selecciónproyecto.AddRange(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetAllGroups_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Selecciónproyecto.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var selectionRetrieved = context.Selecciónproyecto.Find(soloID);
                Assert.IsNotNull(selectionRetrieved);

                context.Selecciónproyecto.Remove(selectionRetrieved);
                context.SaveChanges();
                var selectionRemoved = context.Selecciónproyecto.Find(soloID);
                Assert.IsNull(selectionRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var selectionList = context.Selecciónproyecto.Where(p => p.SelecciónproyectoID >= (tmp));
                context.Selecciónproyecto.RemoveRange(selectionList);
                context.SaveChanges();

                var selectionRemoved = context.Selecciónproyecto.Find(duoID[0]);
                Assert.IsNull(selectionRemoved);
            }
        }
    }
}