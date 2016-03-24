
namespace Tests {
    using System;

    using DataSpace.Common.Settings.Connection;

    using NUnit.Framework;

    [TestFixture]
    public class AccountSettingsFactoryTest {
        [Test]
        public void CreateInstance() {
            IAccountSettingsFactory underTest = new AccountSettingsFactory();
        }
    }
}