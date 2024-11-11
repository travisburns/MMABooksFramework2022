using NUnit.Framework;
using MMABooksBusiness;
using MMABooksProps;
using MMABooksDB;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;
using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductDBTest
    {
        ProductDB db;

        [SetUp]
        public void ResetData()
        {
            db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetProductData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestRetrieve()
        {
            ProductProps p = (ProductProps)db.Retrieve("A4CS");
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.AreEqual("Murach's ASP.NET 4 Web Programming with C# 2010", p.Description);
            Assert.AreEqual(56.50m, p.UnitPrice);
            Assert.AreEqual(4637, p.OnHandQuantity);
        }

        [Test]
        public void TestRetrieveAll()
        {
            List<ProductProps> list = (List<ProductProps>)db.RetrieveAll();
            Assert.AreEqual(16, list.Count);  
            Assert.IsTrue(list[0].ProductCode.Length > 0);
            Assert.IsTrue(list[0].Description.Length > 0);
        }

        [Test]
        public void TestCreate()
        {
            ProductProps p = new ProductProps();
            p.ProductCode = "TEST";
            p.Description = "Test Product";
            p.UnitPrice = Convert.ToDecimal("29.9900");  
            p.OnHandQuantity = 100;

            p = (ProductProps)db.Create(p);
            ProductProps p2 = (ProductProps)db.Retrieve(p.ProductCode);
            Assert.AreEqual(p.GetState(), p2.GetState());
        }


        [Test]
        public void TestDelete()
        {
            ProductProps p = (ProductProps)db.Retrieve("A4CS");
            Console.WriteLine($"Product ConcurrencyID: {p.ConcurrencyID}");

           
            p = (ProductProps)db.Retrieve("A4CS");

            Assert.True(db.Delete(p));
            Assert.Throws<Exception>(() => db.Retrieve("A4CS"));
        }

        [Test]
        public void TestUpdate()
        {
            ProductProps p = (ProductProps)db.Retrieve("A4CS");
            int originalConcurrencyID = p.ConcurrencyID;

            p.Description = "Updated Description";
            p.UnitPrice = 59.99m;

            try
            {
                Assert.True(db.Update(p));
                ProductProps p2 = (ProductProps)db.Retrieve("A4CS");
                Assert.AreEqual(p.Description, p2.Description);
                Assert.AreEqual(p.UnitPrice, p2.UnitPrice);
                Assert.AreEqual(originalConcurrencyID + 1, p2.ConcurrencyID);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Record cannot be updated. It has been edited by another user.")
                {
                
                    p = (ProductProps)db.Retrieve("A4CS");
                    p.Description = "Updated Description";
                    p.UnitPrice = 59.99m;
                    Assert.True(db.Update(p));

                    ProductProps p2 = (ProductProps)db.Retrieve("A4CS");
                    Assert.AreEqual(p.Description, p2.Description);
                    Assert.AreEqual(p.UnitPrice, p2.UnitPrice);
                    Assert.AreEqual(originalConcurrencyID + 1, p2.ConcurrencyID);
                }
                else
                {
                    throw;
                }
            }
        }
    }

        [TestFixture]
    public class ProductPropsTest
    {
        ProductProps? props;

        [SetUp]
        public void Setup()
        {
            props = new ProductProps();
            props.ProductCode = "TEST";
            props.Description = "Test Product";
            props.UnitPrice = 29.99m;
            props.OnHandQuantity = 100;
        }

        [Test]
        public void TestGetState()
        {
            string jsonString = props.GetState();
            Assert.IsTrue(jsonString.Contains(props.ProductCode));
            Assert.IsTrue(jsonString.Contains(props.Description));
            Assert.IsTrue(jsonString.Contains(props.UnitPrice.ToString()));
        }

        [Test]
        public void TestSetState()
        {
            string jsonString = props.GetState();
            ProductProps newProps = new ProductProps();
            newProps.SetState(jsonString);
            Assert.AreEqual(props.ProductCode, newProps.ProductCode);
            Assert.AreEqual(props.Description, newProps.Description);
            Assert.AreEqual(props.UnitPrice, newProps.UnitPrice);
            Assert.AreEqual(props.OnHandQuantity, newProps.OnHandQuantity);
            Assert.AreEqual(props.ConcurrencyID, newProps.ConcurrencyID);
        }

        [Test]
        public void TestClone()
        {
            ProductProps newProps = (ProductProps)props.Clone();
            Assert.AreEqual(props.ProductCode, newProps.ProductCode);
            Assert.AreEqual(props.Description, newProps.Description);
            Assert.AreEqual(props.UnitPrice, newProps.UnitPrice);
            Assert.AreEqual(props.OnHandQuantity, newProps.OnHandQuantity);
            Assert.AreEqual(props.ConcurrencyID, newProps.ConcurrencyID);
        }
    }

    [TestFixture]
    public class ProductTest
    {
        [SetUp]
        public void TestResetDatabase()
        {
            ProductDB db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetProductData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewProductConstructor()
        {
            Product p = new Product();
            Assert.AreEqual(string.Empty, p.ProductCode);
            Assert.AreEqual(string.Empty, p.Description);
            Assert.AreEqual(0m, p.UnitPrice);
            Assert.AreEqual(0, p.OnHandQuantity);
            Assert.IsTrue(p.IsNew);
            Assert.IsFalse(p.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreConstructor()
        {
            Product p = new Product("A4CS");
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.IsTrue(p.Description.Length > 0);
            Assert.IsTrue(p.UnitPrice > 0);
            Assert.IsFalse(p.IsNew);
            Assert.IsTrue(p.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Product p = new Product();
            p.ProductCode = "TEST";
            p.Description = "Test Product";
            p.UnitPrice = 29.99m;
            p.OnHandQuantity = 100;
            p.Save();

            Product p2 = new Product("TEST");
            Assert.AreEqual(p2.ProductCode, p.ProductCode);
            Assert.AreEqual(p2.Description, p.Description);
            Assert.AreEqual(p2.UnitPrice, p.UnitPrice);
            Assert.AreEqual(p2.OnHandQuantity, p.OnHandQuantity);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            Product p = new Product();
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            Product p = new Product();
            p.ProductCode = "TEST";
            Assert.Throws<Exception>(() => p.Save());
            p.Description = "Test Product";
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Product p = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => p.UnitPrice = -10.00m);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.OnHandQuantity = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.ProductCode = "ThisCodeIsTooLong");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Product p1 = new Product("A4CS");
            Product p2 = new Product("A4CS");

            p1.Description = "Updated first";
            p1.Save();

            p2.Description = "Updated second";
            Assert.Throws<Exception>(() => p2.Save());
        }
    }
}