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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AncientArk.Expressions
{
    /// <summary>
    /// Lower level utilities for expression.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Visit along the member inheritance chain and return their member expressions as a stack.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The inheritance chain for the given member expression as a stack.</returns>
        public static (ParameterExpression, Stack<MemberExpression>) GetMemberHierarchy(Expression expression)
        {
            var currentExpression = expression;
            Stack<MemberExpression> stack = new Stack<MemberExpression>();
            var memberExpression = GetMemberExpression(currentExpression);
            while (memberExpression != null)
            {
                stack.Push(memberExpression);
                currentExpression = memberExpression.Expression;
                memberExpression = GetMemberExpression(currentExpression);
            }
            return (currentExpression as ParameterExpression, stack);
        }

        /// <summary>
        /// Step through the inheritance chain to perform step-wise checks.
        /// </summary>
        /// <param name="hierarchy">The inheritance chain for the given member expression as a stack.</param>
        /// <returns>An expression with step-wise null checks and auto instantiation.</returns>
        public static Expression VisitMembers(Stack<MemberExpression> hierarchy)
        {
            Expression currentExpression = Expression.Empty();
            var memberExpression = hierarchy.Pop();
            while (hierarchy.Count > 0)
            {
                currentExpression = AccessMember(memberExpression, currentExpression);
                memberExpression = hierarchy.Pop();
            }
            currentExpression = AccessMember(memberExpression, currentExpression);
            return currentExpression;
        }

        /// <summary>
        /// Build a null-check onto a member expression
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="currentExpression"></param>
        /// <returns></returns>
        public static Expression AccessMember(MemberExpression memberExpression, Expression currentExpression)
        {
            var memberType = memberExpression.Member.GetMemberType();
            if (!memberType.GetTypeInfo().IsValueType && memberType != typeof(string))
            {
                var defaultValue = Expression.Default(memberType);
                var nullCheck = Expression.Equal(memberExpression, defaultValue);
                var createMemberCondition = Expression.IfThen(nullCheck, CreateMember(memberExpression));
                currentExpression = Expression.Block(currentExpression, createMemberCondition);
            }
            return currentExpression;
        }

        /// <summary>
        /// Create a method call expression to new that member with public parameterless constructor
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="currentExpression"></param>
        /// <returns></returns>
        public static MethodCallExpression CreateMember(MemberExpression memberExpression)
        {
            var memberType = memberExpression.Member.GetMemberType();
            var constructExpression = Expression.New(memberType);
            var setInstance = GetSetterExpression(memberExpression, constructExpression);
            return setInstance;
        }

        /// <summary>
        /// Get the expression for the get method of property
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetGetterExpression(MemberExpression memberExpression)
        {
            if (memberExpression.Member is PropertyInfo property)
            {
                var getMethod = property.GetMethod;
                var objectExpression = memberExpression.Expression;
                var getExpression = Expression.Call(objectExpression, getMethod);
                return getExpression;
            }
            return null;
        }

        /// <summary>
        /// Get the expression for the set method of property
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetSetterExpression(MemberExpression memberExpression, Expression valueExpression)
        {
            if (memberExpression.Member is PropertyInfo property)
            {
                var setMethod = property.SetMethod;
                var objectExpression = memberExpression.Expression;
                var setExpression = Expression.Call(objectExpression, setMethod, valueExpression);
                return setExpression;
            }
            return null;
        }

        /// <summary>
        /// Get the member expression.
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression.NodeType is ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression;
                return body.Operand as MemberExpression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return expression as MemberExpression;
            }
            return null;
        }
    }
}
