// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    using Fakes;

    public class ServiceManagerTests
    {
        private ServiceManager _serviceManager;

        private IService _service1;
        private IService _service2;

        [SetUp]
        public void SetUp()
        {
            _serviceManager = new ServiceManager();

            _service1 = new Service1();
            _service2 = new Service2();

            _serviceManager.AddService(_service1);
            _serviceManager.AddService(_service2);
        }

        [Test]
        public void InitializeAllServices()
        {
            _serviceManager.StartServices();

            Assert.That(_service1.Status, Is.EqualTo(ServiceStatus.Started));
            Assert.That(_service2.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [Test]
        public void InitializationFailure1()
        {
            ((FakeProjectService)_service1).FailToStart = true;
            Assert.That(() => _serviceManager.StartServices(),
                Throws.InstanceOf<InvalidOperationException>().And.Message.Contains("Service1"));
        }

        [Test]
        public void InitializationFailure2()
        {
            ((FakeProjectService)_service2).FailToStart = true;
            Assert.That(() => _serviceManager.StartServices(),
                Throws.InstanceOf<InvalidOperationException>().And.Message.Contains("Service2"));
        }

        [Test]
        public void TerminationFailure1()
        {
            ((FakeProjectService)_service1).FailToStop = true;
            _serviceManager.StartServices();

            Assert.DoesNotThrow(() => _serviceManager.StopServices());
            Assert.That(_service1.Status, Is.EqualTo(ServiceStatus.Error));
        }

        [Test]
        public void TerminationFailure2()
        {
            ((FakeProjectService)_service2).FailToStop = true;
            _serviceManager.StartServices();

            Assert.DoesNotThrow(() => _serviceManager.StopServices());
            Assert.That(_service2.Status, Is.EqualTo(ServiceStatus.Error));
        }

        [TestCase(typeof(Service1))]
        [TestCase(typeof(Service2))]
        public void AccessServicesByClass(Type type)
        {
            IService service = _serviceManager.GetService(type);
            Assert.That(service, Is.Not.Null);
            Assert.That(service, Is.TypeOf(type));
        }

        [TestCase(typeof(IService1))]
        [TestCase(typeof(IService2))]
        public void AccessServicesByInterface(Type type)
        {
            IService service = _serviceManager.GetService(type);
            Assert.That(service, Is.Not.Null);
            Assert.That(service, Is.AssignableTo(type));
        }

        private interface IService1 : IService { }
        private class Service1 : FakeProjectService, IService1 { }

        private interface IService2 : IService { }
        private class Service2 : FakeProjectService, IService2 { }
    }
}
