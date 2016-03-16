// teach log4 net to use this settings (other ways like app.config are difficult because of using integrated test host)
[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"Log4Net.config", Watch = true)]

namespace Tests.Common.Settings.Connection
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.W32;
    using DataSpace.Common.Crypto;
    using System.ComponentModel;

    [TestFixture]
    public class AccountSettingsTest
    {
        private string _Url = "test.url.com";
        private string _UserName = "TestName";
        private string _Password = "TestPassword";

        [OneTimeSetUp]
        public void Init()
        {
            // calling a logger function triggers reading attributed log4Net settings (see comment above)
            log4net.LogManager.GetLogger(typeof(AccountSettings));
        }
        [Test]
        public void Constructor()
        {
            IAccountSettings underTest = new AccountSettings();
            // Assert
            Assert.That(underTest.IsDirty, Is.False);
            Assert.That(underTest.Url, Is.EqualTo(string.Empty));
            Assert.That(underTest.UserName, Is.EqualTo(string.Empty));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(string.Empty));
        }
        [Test]
        public void PropertyGetSet()
        {
            IAccountSettings underTest = new AccountSettings();
            // act
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            // assert
            Assert.AreEqual(_Url, underTest.Url);
            Assert.AreEqual(_UserName, underTest.UserName);
            Assert.AreEqual(_Password, underTest.Password.ConvertToUnsecureString());
            Assert.AreEqual(true, underTest.IsDirty);

        }
        [Test]
        public void Read_Write()
        {
            IAccountSettings underTest = new ConnectionSettingsFactory().AccountSettings;
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();
        }

        [Test]
        public void CreateNew()
        {
            IAccountSettings underTest = new ConnectionSettingsFactory().AccountSettings;
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);
            underTest.Save();
        }

        [Test]
        public void Check_OnPropertyChanged_Success()
        {
            IAccountSettings underTest = new AccountSettings();

            List<string> ReceivedEvents = new List<string>();

            underTest.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args)
            {
                ReceivedEvents.Add(args.PropertyName);
            };
            // Act
            underTest.Url = _Url;
            underTest.UserName = _UserName;
            underTest.Password = new System.Security.SecureString().Init(_Password);

            // Assert
            Assert.That(underTest.IsDirty, Is.True);
            Assert.That(underTest.Url, Is.EqualTo(_Url));
            Assert.That(underTest.UserName, Is.EqualTo(_UserName));
            Assert.That(underTest.Password.ConvertToUnsecureString(), Is.EqualTo(_Password));

            Assert.That(ReceivedEvents.Count, Is.EqualTo(4));
            Assert.That(ReceivedEvents[0], Is.EqualTo("Url"));
            Assert.That(ReceivedEvents[1], Is.EqualTo("IsDirty"));
            Assert.That(ReceivedEvents[2], Is.EqualTo("UserName"));
            Assert.That(ReceivedEvents[3], Is.EqualTo("Password"));
        }
        [Test]
        public void Load_TriggersEvent()
        {
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // Load Event Handler
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };

            // Load 
            AccSet.Load();

            Assert.AreEqual(true, IsTriggered);
        }
        [Test]
        public void Save_TriggersEvent()
        {
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // Save Event Handler
            bool IsTriggered = false;
            AccSet.SettingsSaved += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // Save 
            AccSet.Save();

            Assert.AreEqual(true, IsTriggered);
        }

        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad()
        {
            //prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            System.Threading.Thread.Sleep(5020);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(true, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);

        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadInRefreshSpan()
        {
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            System.Threading.Thread.Sleep(1000);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadDisabled()
        {
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            // set disabled
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            System.Threading.Thread.Sleep(5020);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadEditMode()
        {
            // prep
            IAccountSettings AccSet = new ConnectionSettingsFactory().AccountSettings;
            // clear dirty flag
            AccSet.Load();
            AccountSettings AccSetObj = AccSet as AccountSettings;
            // set 5 sec timer
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 5, 0);
            bool IsTriggered = false;
            AccSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = AccSet.Url;
            // modify -> go in Edit mode
            AccSet.Url = Url + "!";
            System.Threading.Thread.Sleep(5020);
            Url = AccSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            AccSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
    }
}
