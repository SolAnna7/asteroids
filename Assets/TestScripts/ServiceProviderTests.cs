using System;
using System.Collections.Generic;
using Asteroid.Services;
using NUnit.Framework;

namespace Asteroid.Tests
{
    public class ServiceProviderTests
    {
        [Test]
        public void TestSimpleSuccess()
        {
            //Given
            var sp = ServiceProvider.Build()
            .RegisterService<TestInterfaceA>(new TestClassA1())
            .RegisterService<TestInterfaceB>(new TestClassB1())
            .Initialise();

            //When
            TestInterfaceA tca = sp.GetService<TestInterfaceA>();
            TestInterfaceB tcb = sp.GetService<TestInterfaceB>();

            //Then
            Assert.AreEqual("A1", tca.Foo());
            Assert.AreEqual("B1", tcb.Bar());
        }

        [Test]
        public void TestRebuild()
        {
            //Given
            var sp = ServiceProvider.Build()
            .RegisterService<TestInterfaceA>(new TestClassA1())
            .Initialise();

            //When
            TestInterfaceA tca = sp.GetService<TestInterfaceA>();
            sp = ServiceProvider.Build()
            .RegisterService<TestInterfaceA>(new TestClassA2())
            .Initialise();
            TestInterfaceA tca2 = sp.GetService<TestInterfaceA>();

            //Then
            Assert.AreEqual("A1", tca.Foo());
            Assert.AreEqual("A2", tca2.Foo());
        }

        [Test]
        public void TestInitFunctions()
        {
            //Given
            var sp = ServiceProvider.Build()
            .RegisterService<TestInterfaceA>(new TestClassA3())
            .RegisterService<TestInterfaceB>(new TestClassB3())
            .Initialise();

            //When
            TestInterfaceA tca = sp.GetService<TestInterfaceA>();
            TestInterfaceB tcb = sp.GetService<TestInterfaceB>();

            //Then
            Assert.AreEqual("A3y", tca.Foo());
            Assert.AreEqual("B3y", tcb.Bar());
        }

        [Test]
        public void TestNullRegistrationError()
        {
            Assert.That(() =>
            {
                ServiceProvider.Build()
                .RegisterService<TestInterfaceA>(null)
                .Initialise();
            },
            Throws.TypeOf(typeof(ArgumentNullException)));
        }
        [Test]
        public void TestMultipleRegistrationError()
        {
            Assert.That(() =>
            {
                ServiceProvider.Build()
                .RegisterService<TestInterfaceA>(new TestClassA1())
                .RegisterService<TestInterfaceA>(new TestClassA2())
                .Initialise();
            },
            Throws.TypeOf(typeof(ArgumentException)));
        }
        public void TestNoInstanceError()
        {
            Assert.That(() =>
            {
                var sp = ServiceProvider.Build()
                .RegisterService<TestInterfaceA>(null)
                .Initialise();

                TestInterfaceB tcb = sp.GetService<TestInterfaceB>();
            },
            Throws.TypeOf(typeof(KeyNotFoundException)));
        }

        //Test classes
        private interface TestInterfaceA
        {
            string Foo();
        }
        private interface TestInterfaceB
        {
            string Bar();
        }
        private class TestClassA1 : TestInterfaceA
        {
            public string Foo() => "A1";
        }
        private class TestClassA2 : TestInterfaceA
        {
            public string Foo() => "A2";
        }
        private class TestClassA3 : TestInterfaceA, ServiceProvider.IInitialisableService
        {
            public string Foo() => "A3" + (_res ? "y" : "n");

            private bool _res = false;

            public void Initialise(ServiceProvider sp)
            {
                var a = sp.GetService<TestInterfaceB>();
                _res = !string.IsNullOrEmpty(a.Bar());
            }
        }


        private class TestClassB1 : TestInterfaceB
        {
            public string Bar() => "B1";
        }

        private class TestClassB3 : TestInterfaceB, ServiceProvider.IInitialisableService
        {
            public string Bar() => "B3" + (_res ? "y" : "n");

            private bool _res = false;

            public void Initialise(ServiceProvider sp)
            {
                var a = sp.GetService<TestInterfaceA>();
                _res = !string.IsNullOrEmpty(a.Foo());
            }
        }
    }
}