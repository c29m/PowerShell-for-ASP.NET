using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Web.PowerShell.Tests
{
    [TestClass]
    public class HttpPowerShellTests
    {
        [TestMethod]
        public void compare_getdate_to_datetime_now()
        {
            var date = HttpPowerShellCommand.FromCommand("Get-Date").Invoke<DateTime>().First();
            var now  = DateTime.Now;

            if (now.Subtract(date).TotalSeconds > 5)
            {
                Assert.Fail("The date returned from the PowerShell command 'Get-Date' and the date retrieved from 'DateTime.Now' is different by more than five seconds.");
            }
        }

        [TestMethod]
        public void get_wmi_win32_computersystem_object_and_compare_environment_machinename()
        {
            using (var Win32_ComputerSystem = HttpPowerShell.Invoke("Get-WmiObject Win32_ComputerSystem").AsDisposable())
            {
                var wmi = Win32_ComputerSystem.First();

                if (!string.Equals((string)wmi.ToDynamic().Name, Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Assert.Fail("The WMI::Win32_ComputerSystem.Name value is not equal to Environment.MachineName.");
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CmdletInvocationException))]
        public void should_throw_CmdletInvocationException_when_getcredential()
        {
            HttpPowerShell.Invoke("Get-Credential");
        }

        [TestMethod]
        public void parameter_value_passed_to_script_via_parameters_is_returned()
        {
            var guid = Guid.NewGuid();
            Assert.AreEqual<Guid>(guid, HttpPowerShell.Invoke<Guid>("param($guid) $guid", parameters: new { guid = guid }).First());
        }

        [TestMethod]
        public void parameter_value_passed_to_script_via_input_is_returned_from_process_block()
        {
            var guid = Guid.NewGuid();
            Assert.AreEqual<Guid>(guid, HttpPowerShell.Invoke<Guid>("process { $_ }", input: new[] { guid }).First());
        }

        [TestMethod]
        public void input_values_sorted_by_sortobject_command()
        {
            var values = new[] { 2, 1 };
            var sorted = HttpPowerShellCommand.FromCommand("Sort-Object").Invoke<int>(values).ToArray();

            Assert.AreEqual<int>(values[0], sorted[1]);
            Assert.AreEqual<int>(values[1], sorted[0]);
        }

        [TestMethod]
        public void can_invoke_script_from_resource_file()
        {
            Assert.AreEqual(DateTime.Now.Year, HttpPowerShellCommand.FromResource<PowerShellScripts>("GetYearFromDate").Invoke<int>().First());
        }

        [TestMethod]
        public void can_invoke_script_from_resource_string()
        {
            Assert.AreEqual(true, HttpPowerShellCommand.FromResource(PowerShellScripts.ResourceManager, "EchoInputs").Invoke<bool>(new bool[] { true }).First());
        }

        [TestMethod]
        public void can_invoke_script_from_resource_by_basename()
        {
            Assert.AreNotEqual(false, HttpPowerShellCommand.FromResource<HttpPowerShellTests>("System.Web.PowerShell.Tests.PowerShellScripts", "EchoInputs").Invoke<bool>(new bool[] { true }).First());
        }
    }
}
