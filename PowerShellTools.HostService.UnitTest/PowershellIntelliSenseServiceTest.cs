﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerShellTools.HostService.ServiceManagement;
using PowerShellTools.Intellisense;
using PowerShellTools.Common.ServiceManagement.IntelliSenseContract;
using System.Threading;

namespace PowerShellTools.HostService.UnitTest
{
    [TestClass]
    public class PowershellIntelliSenseServiceTest
    {
        private PowershellIntelliSenseService _service;
        private IIntelliSenseServiceCallback _context;

        [TestInitialize]
        public void Init()
        {
            _context = new IntelliSenseEventsHandlerProxy();
            _service = new PowershellIntelliSenseService(_context);

        }

        [TestCleanup]
        public void Clean()
        {
        }

        [TestMethod]
        public void GetCompletionResultsDashTriggerTest()
        {
            var mre = new ManualResetEvent(false);

            CompletionResultList result = null;
            ((IntelliSenseEventsHandlerProxy)_context).CompletionListUpdated += (sender, args) => { result = args.Value; mre.Set(); };
            
            _service.RequestCompletionResults("Write-", 6, DateTime.UtcNow.ToString());

            mre.WaitOne();

            Assert.AreEqual<int>(0, result.ReplacementIndex);
            Assert.AreEqual<int>(6, result.ReplacementLength);
        }

        [TestMethod]
        public void GetCompletionResultsDollarTriggerTest()
        {
            var mre = new ManualResetEvent(false);

            CompletionResultList result = null;
            ((IntelliSenseEventsHandlerProxy)_context).CompletionListUpdated += (sender, args) => { result = args.Value; mre.Set(); };

            string script = @"$myVar = 2; $myStrVar = 'String variable'; Write-Host $";
            _service.RequestCompletionResults(script, 55, DateTime.UtcNow.ToString());

            mre.WaitOne();

            Assert.AreEqual<string>("$myVar", result.CompletionMatches[0].CompletionText);
            Assert.AreEqual<string>("$myStrVar", result.CompletionMatches[1].CompletionText);
            Assert.AreEqual<int>(54, result.ReplacementIndex);
            Assert.AreEqual<int>(1, result.ReplacementLength);
        }
    }
}
