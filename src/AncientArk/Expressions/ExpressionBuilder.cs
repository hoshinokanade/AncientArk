/*
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AncientArk.Expressions
{
    /// <summary>
    /// Higher level utilities for building expression tree.
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Wrap a type cast before executing a lambda expression. The first parameter of the source lambda expression is the target of type cast.
        /// </summary>
        /// <param name="source">Lambda expression to be executed after the type cast.</param>
        /// <param name="type">Type to cast to before execution of source lambda expression.</param>
        /// <returns></returns>
        public static LambdaExpression WrapTypeCastBeforeLambda(LambdaExpression source, Type type)
        {
            var targetType = source.Parameters[0].Type;
            var instance = Expression.Parameter(type, $"{type.Name}Instance");
            var casted = Expression.Convert(instance, targetType);

            var parameters = new List<Expression> { casted };
            parameters.AddRange(source.Parameters.Skip(1));

            var invocation = Expression.Invoke(source, parameters);
            return Expression.Lambda(invocation, instance);
        }

        /// <summary>
        /// Wrap a type cast before executing a lambda expression
        /// </summary>
        /// <param name="source">Lambda expression to be executed after the type cast.</param>
        /// <param name="sourceType">Only the parameter that match with sourceType is the target of type cast.</param>
        /// <param name="type">Type to cast to before execution of source lambda expression.</param>
        /// <returns></returns>
        public static LambdaExpression WrapTypeCastBeforeLambda(LambdaExpression expression, Type sourceType, Type type)
        {
            var parameters = new Dictionary<ParameterExpression, Expression>();
            foreach (var param in expression.Parameters)
            {
                if (param.Type == sourceType)
                {
                    var targetType = param.Type;
                    var instance = Expression.Parameter(type, $"{type.Name}Instance");
                    var casted = Expression.Convert(instance, targetType);
                    parameters.Add(instance, casted);
                }
                else
                {
                    parameters.Add(param, param);
                }
            }
            var invocation = Expression.Invoke(expression, parameters.Values);
            return Expression.Lambda(invocation, parameters.Keys);
        }

        /// <summary>
        /// Wrap type casts before executing a lambda expression. All parameters  of the source lambda expression are the target of type cast.
        /// </summary>
        /// <param name="source">Lambda expression to be executed after the type cast.</param>
        /// <param name="type">Type to cast to before execution of source lambda expression.</param>
        /// <returns></returns>
        public static LambdaExpression WrapAllTypeCastsBeforeLambda(LambdaExpression source, Type type)
        {
            var parameters = new Dictionary<ParameterExpression, Expression>();
            foreach (var param in source.Parameters)
            {
                var targetType = param.Type;
                var instance = Expression.Parameter(type, $"{type.Name}Instance");
                var casted = Expression.Convert(instance, targetType);
                parameters.Add(instance, casted);
            }
            var invocation = Expression.Invoke(source, parameters.Values);
            return Expression.Lambda(invocation, parameters.Keys);
        }

        /// <summary>
        /// Wrap a type cast after executing a lambda expression. The first parameter of the source lambda expression is the target of type cast.
        /// </summary>
        /// <param name="source">Lambda expression to be executed after the type cast.</param>
        /// <param name="type">Type to cast to before execution of source lambda expression.</param>
        /// <returns></returns>
        public static LambdaExpression WrapTypeCastAfterLambda(LambdaExpression expression, Type type)
        {
            var invocation = Expression.Invoke(expression, expression.Parameters);
            var casted = Expression.Convert(invocation, type);
            return Expression.Lambda(casted, expression.Parameters);
        }

        /// <summary>
        /// Build a getter expression from a member
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="accessExpression"></param>
        /// <returns></returns>
        public static LambdaExpression BuildDefaultGetter(MemberExpression memberExpression, LambdaExpression accessExpression)
        {
            InvocationExpression invocation = Expression.Invoke(accessExpression, accessExpression.Parameters);
            BlockExpression block = Expression.Block(invocation, memberExpression);
            LambdaExpression getter = Expression.Lambda(block, accessExpression.Parameters);
            return getter;
        }

        /// <summary>
        /// Build the final getter expression by wrapping the TIn and TOut as objects
        /// </summary>
        /// <param name="getterExpression"></param>
        /// <returns></returns>
        public static LambdaExpression BuildFinalGetter(LambdaExpression getterExpression)
        {
            LambdaExpression fromObject = WrapTypeCastBeforeLambda(getterExpression, typeof(object));
            LambdaExpression toObject = WrapTypeCastAfterLambda(fromObject, typeof(object));
            return toObject;
        }

        /// <summary>
        /// Build a setter expression from a member
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="accessExpression"></param>
        /// <returns></returns>
        public static LambdaExpression BuildDefaultSetter(MemberExpression memberExpression, LambdaExpression accessExpression)
        {
            InvocationExpression invocation = Expression.Invoke(accessExpression, accessExpression.Parameters);

            ParameterExpression value = Expression.Parameter(memberExpression.Member.GetMemberType(), "value");
            MethodCallExpression setter = ExpressionHelper.GetSetterExpression(memberExpression, value);
            BlockExpression block = Expression.Block(invocation, setter);

            var parameters = new List<ParameterExpression>(accessExpression.Parameters);
            parameters.Add(value);
            var lambda = Expression.Lambda(block, parameters);
            return lambda;
        }

        /// <summary>
        /// Build the final getter expression by wrapping the TIn1 and TIn2 as objects
        /// </summary>
        /// <param name="setterExpression"></param>
        /// <returns></returns>
        public static LambdaExpression BuildFinalSetter(LambdaExpression setterExpression)
        {
            return WrapAllTypeCastsBeforeLambda(setterExpression, typeof(object));
        }
    }
}
