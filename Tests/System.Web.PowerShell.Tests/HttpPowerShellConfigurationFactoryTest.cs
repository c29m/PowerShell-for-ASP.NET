﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Web.PowerShell.Tests
{
    [TestClass]
    public class HttpPowerShellConfigurationFactoryTest
    {
        [TestMethod]
        public void configuration_factory_for_datetime_returns_from_getdate_command()
        {
            var date = HttpPowerShellConfigurationFactory<DateTime>.FromCommand("Get-Date").CreateInstance();
            var now = DateTime.Now;

            if (now.Subtract(date).TotalSeconds > 5)
            {
                Assert.Fail("The date returned from the PowerShell command 'Get-Date' and the date retrieved from 'DateTime.Now' is different by more than five seconds.");
            }
        }
    }
}
