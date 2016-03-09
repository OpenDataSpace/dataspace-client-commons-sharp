//-----------------------------------------------------------------------
// <copyright file="PropertyUtils.cs" company="GRAU DATA AG">
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
namespace DataSpace.Common.Utils {
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// Enum utils.
    /// </summary>
    public static class Property {
        /// <summary>
        /// Helper method to return the property name of a given instance property.
        /// https://stackoverflow.com/questions/4266426/c-sharp-how-to-get-the-name-in-string-of-a-class-property
        /// Credits to: https://stackoverflow.com/users/115413/christian-hayter
        /// </summary>
        /// <returns>The property name as string.</returns>
        /// <param name="expr">Expression which points to a property.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string NameOf<T>(Expression<Func<T>> expr) {
            return ((MemberExpression)expr.Body).Member.Name;
        }

        /// <summary>
        /// Helper method to return the property name of a given class property.
        /// http://stackoverflow.com/questions/8136480/is-it-possible-to-get-an-object-property-name-string-without-creating-the-object
        /// Credits to: http://stackoverflow.com/users/295635/peter
        /// </summary>
        /// <returns>The property name as string.</returns>
        /// <param name="property">Property function.</param>
        /// <typeparam name="TModel">The 1st type parameter.</typeparam>
        /// <typeparam name="TProperty">The 2nd type parameter.</typeparam>
        public static string NameOf<TModel, TProperty>(Expression<Func<TModel, TProperty>> property) {
            MemberExpression memberExpression = (MemberExpression)property.Body;
            return memberExpression.Member.Name;
        }
    }
}