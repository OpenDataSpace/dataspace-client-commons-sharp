//-----------------------------------------------------------------------
// <copyright file="DataSpaceBindingType.cs" company="GRAU DATA AG">
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

    using DotCMIS;
    using DotCMIS.Enums;

    public enum DataSpaceBindingType {
        [CmisValue(BindingType.Browser)]
        [DataSpaceUrlSuffix("/cmis/browser")]
        Browser,

        [CmisValue(BindingType.AtomPub)]
        [DataSpaceUrlSuffix("/cmis/atom11")]
        AtomPub1_1,

        [CmisValue(BindingType.AtomPub)]
        [DataSpaceUrlSuffix("/cmis/atom")]
        AtomPub1_0
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DataSpaceUrlSuffixAttribute : System.Attribute {
        public DataSpaceUrlSuffixAttribute(string suffix) {
            Suffix = suffix;
        }

        public string Suffix {
            get;
            private set;
        }
    }
}