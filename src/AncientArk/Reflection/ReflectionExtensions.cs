/*
 * Copyright (C) 2009-2017 Josh Close and Contributors
 * Copyright (C) 2018 Hoshinoakanade
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or any later version.
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Linq;

namespace AncientArk
{
    /// <summary>
    /// Extensions to help with reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the type from the member.
        /// </summary>
        /// <param name="member">The member to get the type from.</param>
        /// <returns>The type.</returns>
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }
            if (member is FieldInfo field)
            {
                return field.FieldType;
            }
            throw new InvalidOperationException($"Member {member.Name} is neither a property nor a field.");
        }

        /// <summary>
        /// Gets a member expression for the member.
        /// </summary>
        /// <param name="member">The member to get the expression for.</param>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member expression.</returns>
        public static MemberExpression GetMemberExpression(this MemberInfo member, Expression expression)
        {
            if (member is PropertyInfo property)
            {
                return Expression.Property(expression, property);
            }
            if (member is FieldInfo field)
            {
                return Expression.Field(expression, field);
            }
            throw new InvalidOperationException($"Member {member.Name} is neither a property nor a field.");
        }

        /// <summary>
        /// Gets a value indicating if the given type is anonymous.
        /// True for anonymous, otherwise false.
        /// </summary>
        /// <param name="type">The type.</param>
        public static bool IsAnonymous(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            // Check of anonymous type, see https://stackoverflow.com/a/2483054/68499
            bool isAnonymous = Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.Public) != TypeAttributes.Public;

            return isAnonymous;
        }
        
        /// <summary>
        /// Gets the constructor that contains the most parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        public static ConstructorInfo GetConstructorWithParameters(this Type type, params object[] args)
        {
            var argumentTypes = args.Select(a => a.GetType()).ToArray();
            var constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
            return constructorInfo;
        }

        /// <summary>
        /// Gets the constructor that contains the most parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        public static ConstructorInfo GetParameterlessConstructor(this Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic).Where(c => c.GetParameters().Length == 0).FirstOrDefault();
        }

        /// <summary>
        /// Gets a value indicating if the type is a user defined struct.
        /// True if it is a primitive struct, otherwise false.
        /// </summary>
        /// <param name="type">The type.</param>
        public static bool IsPrimitiveStruct(this Type type)
        {
            return type.IsValueType && type.IsPrimitive && !type.IsEnum;
        }
    }
}