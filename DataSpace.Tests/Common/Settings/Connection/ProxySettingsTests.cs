using NUnit.Framework;
using DataSpace.Common.Settings.Connection.W32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSpace.Common.Settings.Connection;
using System.ComponentModel;
using DataSpace.Common.Crypto;
using DataSpace.Common.Utils;

namespace Tests.Common.Settings.Connection
{
    [TestFixture()]
    public class ProxySettingsTest
    {
        private string _Url = "test.url.com";
        private string _UserName = "TestName";
        private string _Password = "TestPassword";

        [OneTimeSetUp]
        public void Init()
        {
            // calling a logger function triggers reading attributed log4Net settings
            log4net.LogManager.GetLogger(typeof(ProxySettings));
        }
        [Test()]
        public void Constructor()
        {
            IProxySettings ProxSet = new ProxySettings();
            // check default values
            Assert.AreEqual(false, ProxSet.IsDirty);
            Assert.AreEqual(false, ProxSet.NeedLogin);
            Assert.AreEqual(ProxyType.None, ProxSet.ProxyType);
            Assert.AreEqual(string.Empty, ProxSet.Url);
            Assert.AreEqual(string.Empty, ProxSet.UserName);
            Assert.AreEqual(string.Empty, ProxSet.Password.ConvertToUnsecureString());
        }
        [Test]
        public void PropertyGetSet()
        {
            IProxySettings ProxSet = new ProxySettings();
            // act
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);
            ProxSet.NeedLogin = true;
            ProxSet.ProxyType = ProxyType.Custom;
            // assert
            Assert.AreEqual(_Url, ProxSet.Url);
            Assert.AreEqual(_UserName, ProxSet.UserName);
            Assert.AreEqual(_Password, ProxSet.Password.ConvertToUnsecureString());
            Assert.AreEqual(true, ProxSet.NeedLogin);
            Assert.AreEqual(ProxyType.Custom, ProxSet.ProxyType);
            Assert.AreEqual(true, ProxSet.IsDirty);
        }
        [Test]
        public void CreateNew()
        {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            ProxSet.ProxyType = ProxyType.Custom;
            ProxSet.NeedLogin = true;
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);
            ProxSet.Save();
        }

        [Test()]
        public void DeleteTest()
        {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;

            ProxSet.Delete();

            Assert.AreEqual(false, ProxSet.IsDirty);
            Assert.AreEqual(false, ProxSet.NeedLogin);
            Assert.AreEqual(ProxyType.None, ProxSet.ProxyType);
            Assert.AreEqual(string.Empty, ProxSet.Url);
            Assert.AreEqual(string.Empty, ProxSet.UserName);
            Assert.AreEqual(string.Empty, ProxSet.Password.ConvertToUnsecureString());
        }

        [Test()]
        public void Load_TriggersEvent()
        {
            string tstr = AppDomain.CurrentDomain.BaseDirectory;
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // Load Event Handler
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };

            // Load 
            ProxSet.Load();

            Assert.AreEqual(true, IsTriggered);
        }

        [Test()]
        public void Save_TriggersEvent()
        {
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // Save Event Handler
            bool IsTriggered = false;
            ProxSet.SettingsSaved += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // Save 
            ProxSet.Save();

            Assert.AreEqual(true, IsTriggered);
        }

       [Test]
        public void OnPropertyChanged_Success()
        {
            IProxySettings ProxSet = new ProxySettings();

            List<string> ReceivedEvents = new List<string>();

            ProxSet.PropertyChanged += delegate (object sender, PropertyChangedEventArgs args)
                {
                    ReceivedEvents.Add(args.PropertyName);
                };
            // Act
            ProxSet.ProxyType = ProxyType.Custom;
            ProxSet.NeedLogin = true;
            ProxSet.Url = _Url;
            ProxSet.UserName = _UserName;
            ProxSet.Password = new System.Security.SecureString().Init(_Password);

            // Assert
            Assert.AreEqual(true, ProxSet.IsDirty);
            Assert.AreEqual(ProxyType.Custom, ProxSet.ProxyType);
            Assert.AreEqual(true, ProxSet.NeedLogin);
            Assert.AreEqual(_Url, ProxSet.Url);
            Assert.AreEqual(_UserName, ProxSet.UserName);
            Assert.AreEqual(_Password, ProxSet.Password.ConvertToUnsecureString());

            Assert.AreEqual(6, ReceivedEvents.Count);               
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.ProxyType), ReceivedEvents[0]);        
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.IsDirty), ReceivedEvents[1]);          
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.NeedLogin), ReceivedEvents[2]);        
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Url), ReceivedEvents[3]);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.UserName), ReceivedEvents[4]);
            Assert.AreEqual(Property.NameOf((IProxySettings a) => a.Password), ReceivedEvents[5]);

        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_TriggersLoad()
        {
            //prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(6020);
            Url = ProxSet.Url;
            // assert
            Assert.AreEqual(true, IsTriggered);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);

        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadInRefreshSpan()
        {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(500);
            Url = ProxSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadDisabled()
        {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            // set 5 sec timer
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            // set disabled
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 0);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            System.Threading.Thread.Sleep(5020);
            Url = ProxSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
        [Test]
        [NUnit.Framework.Category("Slow")]
        public void PropGet_NoTriggersLoadEditMode()
        {
            // prep
            IProxySettings ProxSet = new ConnectionSettingsFactory().ProxySettings;
            // clear dirty flag
            ProxSet.Load();
            ProxySettings ProxSetObj = ProxSet as ProxySettings;
            // set 5 sec timer
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 0, 5);
            bool IsTriggered = false;
            ProxSet.SettingsLoaded += (sender, arg) =>
            {
                IsTriggered = true;
            };
            // act
            string Url = ProxSet.Url;
            // modify -> go in Edit mode
            ProxSet.Url = Url + "!";
            System.Threading.Thread.Sleep(5020);
            Url = ProxSet.Url;
            // assert
            Assert.AreEqual(false, IsTriggered);
            // house keeping
            ProxSetObj.PropsRefreshSpan = new TimeSpan(0, 2, 0);
        }
    }
}