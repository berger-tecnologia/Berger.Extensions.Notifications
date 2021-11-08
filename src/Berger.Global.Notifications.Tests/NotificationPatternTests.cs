using System;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Berger.Global.Notifications.Patterns;
using Berger.Global.Notifications.Tests.Models;
using Berger.Global.Notifications.Tests.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berger.Global.Notifications.Tests
{
    [TestClass]
    public class NotificationPatternTests : BaseConfiguration
    {
        private readonly Customer _customer = new Customer();

        public override void Dispose()
        {
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void GetNotificationFromContext()
        {
            new AddNotifications<Customer>(_customer).IfNullOrEmpty(x => x.Name);

            Assert.IsTrue(_customer.IsInvalid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNullOrEmpty()
        {
            string vazio = string.Empty;

            new AddNotifications<Customer>(_customer).IfNullOrEmpty(x => x.Name).IfNullOrEmpty(vazio, "Vazio");

            Assert.AreEqual(false, _customer.IsValid());
            Assert.IsTrue(_customer.Notifications.Count() == 2);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNullOrWhiteSpace()
        {
            new AddNotifications<Customer>(_customer)
                .IfNullOrWhiteSpace(x => x.Name)
                .IfNullOrWhiteSpace("", "Empty");

            Assert.AreEqual(false, _customer.IsValid());
            Assert.IsTrue(_customer.Notifications.Count() == 2);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotNullOrEmpty()
        {
            _customer.Name = "Robert";

            new AddNotifications<Customer>(_customer)
                .IfNotNullOrEmpty(x => x.Name)
                .IfNotNullOrEmpty("NotEmpty", "Any");

            Assert.AreEqual(false, _customer.IsValid());
            Assert.IsTrue(_customer.Notifications.Count() == 2);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLowerThan()
        {
            _customer.Age = 10;

            new AddNotifications<Customer>(_customer)
                .IfLowerThan(x => x.Age, 25);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfGreaterThan()
        {
            _customer.Age = 37;

            new AddNotifications<Customer>(_customer).IfGreaterThan(x => x.Age, 25);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLengthGreaterThan()
        {
            _customer.Name = "Robert";
            new AddNotifications<Customer>(_customer).IfLengthGreaterThan(x => x.Name, 1);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLengthLowerThan()
        {
            _customer.Name = "Robert";
            new AddNotifications<Customer>(_customer).IfLengthLowerThan(x => x.Name, 200);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLengthNoEqual()
        {
            new AddNotifications<Customer>(_customer).IfLengthNoEqual(x => x.Name, 3);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNullOrEmptyOrInvalidLength()
        {
            Customer customerNameEmpty = new Customer();
            Customer customerNameMinInvalid = new Customer();
            customerNameMinInvalid.Name = "a";
            Customer customerNameMaxInvalid = new Customer();
            customerNameMaxInvalid.Name = "Name with more than 10 characters";

            new AddNotifications<Customer>(customerNameEmpty).IfNullOrEmptyOrInvalidLength(x => x.Name, 3, 10);
            new AddNotifications<Customer>(customerNameMinInvalid).IfNullOrEmptyOrInvalidLength(x => x.Name, 3, 10);
            new AddNotifications<Customer>(customerNameMaxInvalid).IfNullOrEmptyOrInvalidLength(x => x.Name, 3, 10);

            Assert.AreEqual(1, customerNameEmpty.Notifications.Count);
            Assert.AreEqual(1, customerNameMinInvalid.Notifications.Count);
            Assert.AreEqual(1, customerNameMaxInvalid.Notifications.Count);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNullOrOrInvalidLength()
        {
            Customer customerNameEmpty = new Customer();
            Customer customerNameMinInvalid = new Customer();
            customerNameMinInvalid.Name = "a";
            Customer customerNameMaxInvalid = new Customer();
            customerNameMaxInvalid.Name = "Name with more than 10 characters";

            new AddNotifications<Customer>(customerNameEmpty).IfNullOrInvalidLength(x => x.Name, 3, 10);
            new AddNotifications<Customer>(customerNameMinInvalid).IfNullOrInvalidLength(x => x.Name, 3, 10);
            new AddNotifications<Customer>(customerNameMaxInvalid).IfNullOrInvalidLength(x => x.Name, 3, 10);

            Assert.AreEqual(1, customerNameEmpty.Notifications.Count);
            Assert.AreEqual(1, customerNameMinInvalid.Notifications.Count);
            Assert.AreEqual(1, customerNameMaxInvalid.Notifications.Count);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotEmail()
        {
            _customer.Name = "This is not an e-mail";
            new AddNotifications<Customer>(_customer).IfNotEmail(x => x.Name);
            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotUrl()
        {
            _customer.Name = "This is not an URL";
            new AddNotifications<Customer>(_customer).IfNotUrl(x => x.Name);
            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfGreaterOrEqualsThan()
        {
            Customer customer10 = new Customer();
            Customer customer11 = new Customer();
            customer10.Age = 10;
            customer11.Age = 11;

            new AddNotifications<Customer>(customer10).IfGreaterOrEqualsThan(x => x.Age, 10);
            new AddNotifications<Customer>(customer11).IfGreaterOrEqualsThan(x => x.Age, 10);
            Assert.AreEqual(false, customer10.IsValid());
            Assert.AreEqual(false, customer11.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfGreaterOrEqualsThan_DateTime()
        {
            Customer customer10 = new Customer();
            Customer customer11 = new Customer();

            DateTime now = DateTime.Now;

            customer10.CreationDate = now;
            customer11.CreationDate = now.AddDays(1);

            new AddNotifications<Customer>(customer10).IfGreaterOrEqualsThan(x => x.CreationDate, now);
            new AddNotifications<Customer>(customer11).IfGreaterOrEqualsThan(x => x.CreationDate, now);
            Assert.AreEqual(false, customer10.IsValid());
            Assert.AreEqual(false, customer11.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLowerOrEqualsThan()
        {
            Customer customer10 = new Customer();
            Customer customer09 = new Customer();

            customer10.Age = 10;
            customer09.Age = 09;

            new AddNotifications<Customer>(customer10).IfLowerOrEqualsThan(x => x.Age, 10);
            new AddNotifications<Customer>(customer09).IfLowerOrEqualsThan(x => x.Age, 10);

            Assert.AreEqual(false, customer10.IsValid());
            Assert.AreEqual(false, customer09.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfLowerOrEqualsThan_DateTime()
        {
            Customer customer10 = new Customer();
            Customer customer11 = new Customer();

            DateTime now = DateTime.Now;

            customer10.CreationDate = now;
            customer11.CreationDate = now.AddDays(-1);


            new AddNotifications<Customer>(customer10).IfLowerOrEqualsThan(x => x.CreationDate, now);
            new AddNotifications<Customer>(customer11).IfLowerOrEqualsThan(x => x.CreationDate, now);

            Assert.AreEqual(false, customer10.IsValid());
            Assert.AreEqual(false, customer11.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotRange()
        {
            _customer.Age = 10;

            new AddNotifications<Customer>(_customer).IfNotRange(x => x.Age, 11, 21);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotRange_Date()
        {
            DateTime now = DateTime.Now;
            _customer.CreationDate = now;

            new AddNotifications<Customer>(_customer).IfNotRange(x => x.CreationDate, now.AddMinutes(1), now.AddDays(1));

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfRange()
        {
            _customer.Age = 10;

            new AddNotifications<Customer>(_customer).IfRange(x => x.Age, 05, 21);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfRange_Date()
        {
            DateTime now = DateTime.Now;
            _customer.CreationDate = now;

            new AddNotifications<Customer>(_customer).IfNotRange(x => x.CreationDate, DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotContains()
        {
            _customer.Name = "Rafael";
            new AddNotifications<Customer>(_customer).IfNotContains(x => x.Name, "Robert");

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfContains()
        {
            _customer.Name = "Rafael";
            new AddNotifications<Customer>(_customer).IfContains(x => x.Name, "Rafael");

            Assert.AreEqual(false, _customer.IsValid());
        }
        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotAreEquals()
        {
            _customer.Name = "Rafael";
            new AddNotifications<Customer>(_customer).IfNotAreEquals(x => x.Name, "Robert");

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfAreEquals()
        {
            _customer.Name = "Rafael";
            new AddNotifications<Customer>(_customer).IfAreEquals(x => x.Name, "Rafael");

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfTrue()
        {
            _customer.Active = true;
            new AddNotifications<Customer>(_customer).IfTrue(x => x.Active);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfFalse()
        {
            _customer.Active = false;
            new AddNotifications<Customer>(_customer).IfFalse(x => x.Active);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotCpf()
        {
            _customer.Cpf = "0000000000";
            new AddNotifications<Customer>(_customer).IfNotCpf(x => x.Cpf);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotCnpj()
        {
            _customer.Cnpj = "00000000000";
            new AddNotifications<Customer>(_customer).IfNotCnpj(x => x.Cnpj);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotGuid()
        {
            _customer.Name = "Invalid Guid";
            new AddNotifications<Customer>(_customer).IfNotGuid(x => x.Name);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfCollectionIsNullOrEmpty()
        {
            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersIEnumerable);
            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersIList);
            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersICollection);

            _customer.CustomersIEnumerable = new List<Customer>().AsEnumerable();
            _customer.CustomersIList = new List<Customer>();
            _customer.CustomersICollection = new List<Customer>();

            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersIEnumerable);
            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersIList);
            new AddNotifications<Customer>(_customer).IfCollectionIsNullOrEmpty(x => x.CustomersICollection);

            Assert.AreEqual(false, _customer.IsValid());
            Assert.AreEqual(true, _customer.Notifications.Count() == 6);
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfEqualsZero()
        {
            _customer.Age = 0;

            new AddNotifications<Customer>(_customer).IfEqualsZero(x => x.Age);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNull()
        {
            new AddNotifications<Customer>(_customer).IfNull(x => x.Dependents);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNull_Object()
        {
            new AddNotifications<Customer>(_customer).IfNull(x => x.PropriedadeObject);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotNull()
        {
            _customer.Dependents = 2;
            new AddNotifications<Customer>(_customer).IfNotNull(x => x.Dependents);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void ShouldNotificateInEnglish()
        {
            _customer.Dependents = 2;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            new AddNotifications<Customer>(_customer).IfNotNull(x => x.Dependents);

            Assert.AreEqual(false, _customer.IsValid());

            Assert.IsTrue(_customer.Notifications.Count == 1);
            Assert.IsTrue(_customer.Notifications.Any(x => x.Message.Equals("Field Dependents should be equals to null.")), "� esperado uma mensagem no idioma ingles");
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfNotDate()
        {
            _customer.Name = "Invalid Date";
            new AddNotifications<Customer>(_customer).IfNotDate(x => x.Name);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void IfEnumInvalid()
        {
            Gender gender = (Gender)0;
            _customer.Gender = gender;

            new AddNotifications<Customer>(_customer).IfEnumInvalid(x => x.Gender);

            Assert.AreEqual(false, _customer.IsValid());
        }

        [TestMethod]
        [TestCategory("NotificationPattern")]
        public void AddNotificationIfNull()
        {
            Customer customer = new Customer();

            InsertCustomerRequest request = new InsertCustomerRequest()
            {
                Name = null,
                Age = 36
            };

            new AddNotifications<Customer>(customer).IfNull(request.Name, "Name");

            Assert.IsTrue(customer.IsInvalid());
        }
    }
}