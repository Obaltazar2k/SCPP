using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCPP.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.DataAccessTest
{
    [TestClass]
    public class OrganizationTest
    {
        Organización testOrganization;
        List<Organización> testOrganizationList;
        int expectedInDB;
        static int soloID;
        static int[] duoID = new int[2];

        [TestInitialize]
        public void TestInitialize()
        {
            testOrganization = new Organización()
            {
                Telefono = "2731125239",
                Numext = 1300,
                Nombre = "Coca Cola",
                Correo = "cocacola@Coke.com",
                Colonia = "Centro",
                Codigopostal = "94100",
                Calle = "2"
            };

            testOrganizationList = new List<Organización>()
            {
                new Organización
                {
                    Telefono = "2731125211",
                    Numext = 1000,
                    Nombre = "Amazon",
                    Correo = "Amazon@Amazon.com",
                    Colonia = "Centro",
                    Codigopostal = "94100",
                    Calle = "3"

                },

                new Organización
                {
                    Telefono = "2731120000",
                    Numext = 1100,
                    Nombre = "Mercado Libre",
                    Correo = "MercadoLibre@MercadoLibre.com",
                    Colonia = "Centro",
                    Codigopostal = "94100",
                    Calle = "2"
                }
            };

        }

        [TestMethod]
        public void AddNewOrganization_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                expectedInDB = context.Organización.ToList().Count();
                context.Organización.Add(testOrganization);
                context.SaveChanges();
                soloID = testOrganization.OrganizaciónID;
                var expected = context.Organización.Find(testOrganization.OrganizaciónID);
                Assert.AreEqual(expected.OrganizaciónID, testOrganization.OrganizaciónID);
            }
        }

        [TestMethod]
        public void AddRangeOrganizations_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Organización.AddRange(testOrganizationList);
                context.SaveChanges();
                duoID[0] = testOrganizationList[0].OrganizaciónID;
                duoID[1] = testOrganizationList[1].OrganizaciónID;
                var expected = context.Organización.Find(testOrganizationList[1].OrganizaciónID);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, testOrganizationList[1].Nombre);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(soloID);
                Assert.IsNotNull(organizationRetrieved);
                Assert.AreEqual(organizationRetrieved.Nombre, testOrganization.Nombre);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(0);
                Assert.IsNull(organizationRetrieved);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullStudent_DoesNotAffectDatabase()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Organización.Add(null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Null object")]
        public void AddNullRangeStudents_DoesNotAffectDatabase()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Organización.AddRange(null);
                context.SaveChanges();
            }
        }


        [TestMethod]
        public void GetAllStudents_Succes()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var actualInDB = context.Organización.ToList().Count();
                Assert.AreEqual(expectedInDB + 3, actualInDB);
            }
        }



        [TestMethod]
        public void Remove_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(soloID);
                Assert.IsNotNull(organizationRetrieved);

                context.Organización.Remove(organizationRetrieved);
                context.SaveChanges();
                var organizationRemoved = context.Organización.Find(soloID);
                Assert.IsNull(organizationRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using (SCPPContext context = new SCPPContext())
            {
                var tmp = duoID[0];
                var organizationList = context.Organización.Where(p => p.OrganizaciónID >= (tmp));
                context.Organización.RemoveRange(organizationList);
                context.SaveChanges();

                var organizationRemoved = context.Organización.Find(duoID[0]);
                Assert.IsNull(organizationRemoved);
            }
        }

    }
}
