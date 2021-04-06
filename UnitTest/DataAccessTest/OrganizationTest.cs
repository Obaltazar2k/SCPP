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

        [TestInitialize]
        public void TestInitialize()
        {
            testOrganization = new Organización()
            {
                OrganizaciónID = 1,
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
                    OrganizaciónID = 0,
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
                    OrganizaciónID = 0,
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
        public void AddNewProyect_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Organización.Add(testOrganization);
                context.SaveChanges();
                var expected = context.Organización.Find(testOrganization.OrganizaciónID);
                Assert.AreEqual(expected.OrganizaciónID, testOrganization.OrganizaciónID);
            }
        }

        [TestMethod]
        public void AddRangeStudents_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                context.Organización.AddRange(testOrganizationList);
                context.SaveChanges();
                var expected = context.Organización.Find(2);
                Assert.IsNotNull(expected);
                Assert.AreEqual(expected.Nombre, "Amazon");
            }
        }

        [TestMethod]
        public void Remove_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(testOrganization.OrganizaciónID);
                Assert.IsNotNull(organizationRetrieved);

                context.Organización.Remove(organizationRetrieved);
                context.SaveChanges();
                var proyectRemoved = context.Organización.Find(testOrganization.OrganizaciónID);
                Assert.IsNull(proyectRemoved);
            }
        }

        [TestMethod]
        public void RemoveRange_Success()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationList = context.Organización.Where(p => p.Nombre.Equals("Amazon"));
                context.Organización.RemoveRange(organizationList);
                context.SaveChanges();

                var organizationRemoved = context.Organización.Find(2);
                Assert.IsNull(organizationRemoved);
            }
        }

        [TestMethod]
        public void Find_Succes()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(3);
                Assert.IsNotNull(organizationRetrieved);
                Assert.AreEqual("Mercado Libre", organizationRetrieved.Nombre);
            }
        }

        [TestMethod]
        public void Find_ReturnsEmptyObject()
        {
            using(SCPPContext context = new SCPPContext())
            {
                var organizationRetrieved = context.Organización.Find(1);
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
                var organizationRetrieved = context.Organización.ToList();
                int expected = 1;
                int actual = organizationRetrieved.Count();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
