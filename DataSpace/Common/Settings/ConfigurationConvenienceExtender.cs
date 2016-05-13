//-----------------------------------------------------------------------
// <copyright file="ConfigurationConvenienceExtender.cs" company="GRAU DATA AG">
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General private License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General private License for more details.
//
//   You should have received a copy of the GNU General private License
//   along with this program. If not, see http://www.gnu.org/licenses/.
//
// </copyright>
//-----------------------------------------------------------------------
﻿
namespace DataSpace.Common.Settings {
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security;

    using DataSpace.Common.Settings.Connection;
    using DataSpace.Common.Settings.Connection.Native;

    public static class ConfigurationConvenienceExtender {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static INativeAccountStore nativeAccountStore = null;
        static ConfigurationConvenienceExtender() {
            switch (Environment.OSVersion.Platform) {
                case PlatformID.Win32NT:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32S:
                    goto case PlatformID.Win32Windows;
                case PlatformID.WinCE:
                    goto case PlatformID.Win32Windows;
                case PlatformID.Win32Windows:
                    nativeAccountStore = new DataSpace.Common.Settings.Connection.W32.NativeAccountStore();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reads or Creates ConfigurationSection derived objects
        /// </summary>
        /// <param name="SectionName">Name Tag in <code>Configuration.Sections</code></param>
        /// <param name="config">Configuration for load/save operations</param>
        /// <returns>Configuration section</returns>
        public static T GetOrCreateSection<T>(this Configuration config, string SectionName) where T : ConfigurationSection {
            T section = null;
            try {
                // try to get our section -- can except in case of version diff or something else
                // if section does not exist no exception is thrown an returns zero
                section = config.GetSection(SectionName) as T;
            } catch (ConfigurationErrorsException eConf) {
                logger.ErrorFormat("{0} -- Failed to load Section '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName, eConf.Message);
                // assume it already exists  -> delete it
                config.Sections.Remove(SectionName);
            } catch (Exception e) {
                // something else .. log it and eat it
                logger.ErrorFormat("{0} -- Failed to load Section '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName, e.Message);
            }

            if (section == null) {
                // Config without our section -> create and add it
                try {
                    section = Activator.CreateInstance<T>();
                    config.Sections.Add(SectionName, section);
                } catch (Exception e) {
                    logger.ErrorFormat("{0} -- Failed to create Section '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, SectionName, e.Message);
                    throw;
                }
            }

            return section;
        }

        /// <summary>
        /// Reads or Creates ConfigurationSectionGroup derived objects
        /// </summary>
        /// <param name="sectionGroupName">Name of Group in <code>Configuration.SectionGroups</code></param>
        /// <param name="config">Configuration for load/save operations</param>
        /// <returns>ConfigurationSectionGroup</returns>
        public static T GetOrCreateSectionGroup<T>(this Configuration config, string sectionGroupName) where T : ConfigurationSectionGroup {
            T sectionGroup = null;
            try {
                // try to get our section -- can except in case of version diff or something else
                // if section group does not exist no exception is thrown an returns zero
                sectionGroup = config.GetSectionGroup(sectionGroupName) as T;
            } catch (ConfigurationErrorsException eConf) {
                logger.ErrorFormat("{0} -- Failed to load Section Group '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, sectionGroupName, eConf.Message);
                // assume it already exists  -> delete it
                config.SectionGroups.Remove(sectionGroupName);
            } catch (Exception e) {
                // something else .. log it and eat it
                logger.ErrorFormat("{0} -- Failed to load Section Group '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, sectionGroupName, e.Message);
            }

            if (sectionGroup == null) {
                // Config without our section -> create and add it
                try {
                    sectionGroup = Activator.CreateInstance<T>();
                    config.SectionGroups.Add(sectionGroupName, sectionGroup);
                } catch (Exception e) {
                    logger.ErrorFormat("{0} -- Failed to create Section Group '{1}' - Exception: {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, sectionGroupName, e.Message);
                    throw;
                }
            }

            return sectionGroup;
        }

        public static void RemoveSection(this Configuration config, string sectionName) {
            var segments = sectionName.Split('/');

            int i = 0;
            var groups = config.SectionGroups;
            ConfigurationSectionGroup group = null;
            while (i < segments.Length) {
                group = groups.Get(segments[i]);
                groups = group.SectionGroups;
                i++;
            }

            if (segments.Length > 1) {
                groups.Remove(segments[i]);
            } else {
                config.Sections.Remove(sectionName);
            }
        }

        public static IDictionary<string, IAccount> GetDataSpaceAccounts(this Configuration config, IAccountFactory accountFactory = null) {
            accountFactory = accountFactory ?? new AccountFactory();
            var group = config.SectionGroups.Get(DataSpaceAccountSectionGroup.DefaultSectionGroupName) as DataSpaceAccountSectionGroup;
            var accounts = new Dictionary<string, IAccount>();
            if (group != null) {
                foreach (AbstractAccount account in group.Sections) {
                    accounts[account.Id] = account;
                }
            }

            return accounts;
        }

        public static IProxySettings GetProxySettings(this Configuration config, IProxyConfigFactory factory = null) {
            factory = factory ?? new ProxyConfigFactory();
            AbstractProxyConfig section = config.GetSection(AbstractProxyConfig.SectionName) as AbstractProxyConfig;
            if (section == null) {
                section = factory.CreateInstance();
                config.Sections.Add(AbstractProxyConfig.SectionName, section);
            }

            return section;
        }

        public static IAccount AddDataSpaceAccount(
            this Configuration config,
            string url,
            string userName,
            SecureString password,
            IAccountFactory accountFactory = null)
        {
            accountFactory = accountFactory ?? new AccountFactory();
            var accounts = config.GetOrCreateSectionGroup<DataSpaceAccountSectionGroup>(DataSpaceAccountSectionGroup.DefaultSectionGroupName);
            var account = accountFactory.CreateInstance(url, userName, password);
            accounts.Sections.Add(Guid.NewGuid().ToString(), account);
            return account;
        }

        internal static INativeAccountStore GetRegisteredStore() {
            return nativeAccountStore;
        }
    }
}