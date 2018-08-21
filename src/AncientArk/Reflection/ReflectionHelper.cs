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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AncientArk
{
    /// <summary>
    /// Common reflection tasks.
    /// </summary>
    internal static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<ConstructorInfo, Delegate> funcCache = new ConcurrentDictionary<ConstructorInfo, Delegate>(new ConstructorInfoEqualityComparer());

        /// <summary>
        /// Creates an instance of type T
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>A new instance of type T.</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// Creates an instance of the specified type
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>A new instance of the specified type.</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            var constructor = args.Length == 0 ? type.GetParameterlessConstructor() : type.GetConstructorWithParameters(args);
            if (constructor is null)
            {
                throw new InvalidOperationException($"No suitable constructors found for {type.Name}.");
            }
            if (!funcCache.TryGetValue(constructor, out Delegate func))
            {
                funcCache[constructor] = func = CreateInstanceDelegate(type, args);
            }
            try
            {
                return func.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
        
        public static Delegate CreateInstanceDelegate(Type type, params object[] args)
        {
            var constructor = type.GetConstructorWithParameters(args);
            if (constructor is null)
            {
                throw new InvalidOperationException($"No suitable constructors found for {type.Name}.");
            }
            if (type.GetTypeInfo().IsValueType)
            {
                Expression defaultValue = Expression.Default(type);
                return Expression.Lambda(defaultValue).Compile();
            }
            else
            {
                var argumentTypes = args.Select(a => a.GetType()).ToArray();
                var argumentExpressions = argumentTypes.Select((t, i) => Expression.Parameter(t, "var" + i)).ToArray();

                var constructorExpression = Expression.New(constructor, argumentExpressions);
                return Expression.Lambda(constructorExpression, argumentExpressions).Compile();
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> from the type where the property was declared.
        /// </summary>
        /// <param name="type">The type the property belongs to.</param>
        /// <param name="property">The property to search.</param>
        /// <param name="flags">Flags for how the property is retrieved.</param>
        public static PropertyInfo GetDeclaringProperty(Type type, PropertyInfo property, BindingFlags flags)
        {
            if (property.DeclaringType != type)
            {
                var declaringProperty = property.DeclaringType.GetProperty(property.Name, flags);
                return GetDeclaringProperty(property.DeclaringType, declaringProperty, flags);
            }

            return property;
        }

        /// <summary>
        /// Gets the property from the expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="PropertyInfo"/> for the expression.</returns>
        public static MemberInfo GetMember<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var member = GetMemberExpression(expression.Body).Member;
            if (member is PropertyInfo property)
            {
                return property;
            }
            if (member is FieldInfo field)
            {
                return field;
            }

            throw new InvalidOperationException($"'{member.Name}' is not a member.");
        }

        /// <summary>
        /// Gets the member inheritance chain as a stack.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The member expression.</param>
        /// <returns>The inheritance chain for the given member expression as a stack.</returns>
        public static Stack<MemberInfo> GetMembers<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var stack = new Stack<MemberInfo>();

            var currentExpression = expression.Body;
            while (true)
            {
                var memberExpression = GetMemberExpression(currentExpression);
                if (memberExpression == null)
                {
                    break;
                }

                stack.Push(memberExpression.Member);
                currentExpression = memberExpression.Expression;
            }

            return stack;
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            MemberExpression memberExpression = null;
            if (expression.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression as MemberExpression;
            }

            return memberExpression;
        }
    }
}