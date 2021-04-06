using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class GroupTest
    {
        Grupo testGroup;
        List<Grupo> testGroupList;
        static int expectedInDB;
        static int soloID;
        static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testGroup = new Grupo()
            {
                Bloque = "1",
                Cupo = 20,
                Nrc = 17817,
                Periodo = "AGO2021-ENE2022",
                Seccion = "Mañana",
                Inscripción = null,
                Profesor = null,
                Rfcprofesor = null
            };

            testGroupList = new List<Grupo>()
            {
                new Grupo
                {
                    Bloque = "2",
                    Cupo = 20,
                    Nrc = 17818,
                    Periodo = "AGO2021-ENE2022",
                    Seccion = "Tarde"
                },

                new Grupo
                {
                    Bloque = "1",
                    Cupo = 20,
                    Nrc = 17817,
                    Periodo = "FEB2022-JUL2022",
                    Seccion = "Mañana"
                }
            };

        }

        [TestMethod]
        public void AddNewGroup_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Grupo.ToList().Count();
                context.Grupo.Add(testGroup);
                context.SaveChanges();
                soloID = testGroup.GrupoID;
                var expected = context.Grupo.Find(testGroup.GrupoID);
                Assert.AreEqual(expected.GrupoID, testGroup.GrupoID);
            }
        }

        [TestMethod]
        public void AddRangeGroup_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Grupo.AddRange(testGroupList);
                context.SaveChanges();
                duoID[0] = testGroupList[0].GrupoID;
                duoID[1] = testGroupList[1].GrupoID;
                var expected = context.Grupo.Find(testGroupList[1].GrupoID);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nrc, testGroupList[1].Nrc);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var groupRetrieved = context.Grupo.Find(soloID);
                Assert.IsNotNull(groupRetrieved);
                Assert.AreEqual(groupRetrieved.Nrc, testGroup.Nrc);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Grupo.Find(0);
                Assert.IsNull(organizationRetrieved);
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullGroup_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Grupo.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeGroups_DoesNotAffectDatabase()
        {
            using (SCPPContext context = new SCPPContext())
            {
                context.Grupo.AddRange(null);
                context.SaveChanges();
            }
        }
        
        [TestMethod]
        public void GetAllGroups_Succes()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Grupo.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }
        
        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var groupRetrieved = context.Grupo.Find(soloID);
                Assert.IsNotNull(groupRetrieved);

                context.Grupo.Remove(groupRetrieved);
                context.SaveChanges();
                var groupRemoved = context.Grupo.Find(soloID);
                Assert.IsNull(groupRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var groupList = context.Grupo.Where(p => p.GrupoID >= (tmp));
                context.Grupo.RemoveRange(groupList);
                context.SaveChanges();

                var organizationRemoved = context.Grupo.Find(duoID[0]);
                Assert.IsNull(organizationRemoved);
            }
        }
    }
}
