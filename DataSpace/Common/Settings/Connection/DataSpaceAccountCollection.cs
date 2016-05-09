//-----------------------------------------------------------------------
// <copyright file="DataSpaceAccountCollection.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Settings.Connection {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration;

    using DataSpace.Common.Utils;

    public class DataSpaceAccountCollection : ObservableCollection<IDataSpaceAccount>, ISettingsPersist, INotifySettingsChanged {
        private bool isDirty = false;
        private Configuration config;
        private IDataSpaceAccountFactory accountFactory;
        private DataSpaceAccountSectionGroup group;
        public DataSpaceAccountCollection(Configuration config, IDataSpaceAccountFactory factory = null) {
            if (config == null) {
                throw new ArgumentNullException("config");
            }

            this.config = config;
            this.accountFactory = factory ?? new DataSpaceAccountFactory();
            this.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                IsDirty = true;
            };
            this.Load();
        }

        public void Load() {
            this.group = this.config.GetOrCreateSectionGroup<DataSpaceAccountSectionGroup>(DataSpaceAccountSectionGroup.DefaultSectionGroupName);
            this.Clear();
            foreach (var entry in this.group.Sections) {
                base.Add(this.accountFactory.CreateInstance(config, entry as ConfigurationSection));
            }

            SettingsLoaded.Invoke(this, new EventArgs());
        }

        public void Save() {
            foreach (var entry in this) {
                entry.Save();
            }

            this.config.Save();
            SettingsSaved.Invoke(this, new EventArgs());
            this.isDirty = false;
        }

        public void Delete() {
            foreach (var entry in this) {
                entry.Delete();
            }

            this.config.SectionGroups.Remove(DataSpaceAccountSectionGroup.DefaultSectionGroupName);
            this.config.Save();
            SettingsSaved.Invoke(this, new EventArgs());
            this.isDirty = false;
        }

        public bool IsDirty {
            get {
                if (isDirty) {
                    return true;
                }

                foreach (var entry in this) {
                    if (entry.IsDirty) {
                        return true;
                    }
                }

                return false;
            }

            private set {
                if (value != isDirty) {
                    isDirty = value;
                }
            }
        }

        public event EventHandler SettingsLoaded = delegate { };
        public event EventHandler SettingsSaved = delegate { };
    }
}