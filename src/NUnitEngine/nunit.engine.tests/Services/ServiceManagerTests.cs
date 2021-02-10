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

        private IService _projectService;

        [SetUp]
        public void SetUp()
        {
            _serviceManager = new ServiceManager();
            _projectService = new FakeProjectService();
            _serviceManager.AddService(new ExtensionService());
            _serviceManager.AddService(_projectService);
        }

        [Test]
        public void InitializeServices()
        {
            _serviceManager.StartServices();

            IService service = _serviceManager.GetService(typeof(IExtensionService));
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Started));
            service = _serviceManager.GetService(typeof(IProjectService));
            Assert.That(service.Status, Is.EqualTo(ServiceStatus.Started));
        }

        [Test]
        public void InitializationFailure()
        {
            ((FakeProjectService)_projectService).FailToStart = true;
            Assert.That(() => _serviceManager.StartServices(), 
                Throws.InstanceOf<InvalidOperationException>().And.Message.Contains("FakeProjectService"));
        }

        [Test]
        public void TerminationFailure()
        {
            ((FakeProjectService)_projectService).FailToStop = true;
            _projectService.StartService();

            Assert.DoesNotThrow(() => _serviceManager.StopServices());
        }

        [Test]
        public void AccessServiceByClass()
        {
            IService service = _serviceManager.GetService(typeof(FakeProjectService));
            Assert.That(service, Is.SameAs(_projectService));
        }

        [Test]
        public void AccessServiceByInterface()
        {
            IService service = _serviceManager.GetService(typeof(IProjectService));
            Assert.That(service, Is.SameAs(_projectService));
        }
    }
}
