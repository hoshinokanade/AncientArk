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

using AncientArk.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AncientArk
{
    public class ProfileMember<TMember>: ProfileMember
    {
        /// <summary>
        /// Instantiate a profile member that targets to a member
        /// </summary>
        /// <param name="memberExpression"></param>
        public ProfileMember(MemberExpression memberExpression) : base(memberExpression)
        {

        }

        /// <summary>
        /// Short-cut for get/set mapping that only applicable for TMember which has an indexer.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public ProfileMember<TMember> OfKey<TKey>(TKey key)
        {
            PropertyInfo indexer = MemberExpression.Type.GetProperty("Item");
            if (indexer is null)
            {
                throw new InvalidOperationException($"{typeof(TMember)} does not contain indexer");
            }
            InvocationExpression preInvocation = Expression.Invoke(AccessExpression, AccessExpression.Parameters);
            
            ConstantExpression keyExpression = Expression.Constant(key);
            MethodCallExpression getStep = Expression.Call(MemberExpression, indexer.GetMethod, keyExpression);

            BlockExpression getterBlock = Expression.Block(preInvocation, getStep);
            GetterExpression = Expression.Lambda(getterBlock, AccessExpression.Parameters);

            ParameterExpression value = Expression.Parameter(indexer.PropertyType, "value");
            MethodCallExpression setStep = Expression.Call(MemberExpression, indexer.SetMethod, keyExpression, value);
            BlockExpression setterBlock = Expression.Block(preInvocation, setStep);
            SetterExpression = Expression.Lambda(setterBlock, AccessExpression.Parameters[0], value);

            SerializedType = indexer.PropertyType;
            return this;
        }

        /// <summary>
        /// Set the getter and setter expressions to be executed. If only either serialize/deserialize function is needed, it is advised to throw an exception in the expression.
        /// </summary>
        /// <typeparam name="TSerialized"></typeparam>
        /// <param name="retrieve"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public ProfileMember<TMember> MapBy<TSerialized>(Expression<Func<TMember, TSerialized>> retrieve, Expression<Action<TMember, TSerialized>> set)
        {
            InvocationExpression preInvocation = Expression.Invoke(AccessExpression, AccessExpression.Parameters);
            InvocationExpression retrieveExpression = Expression.Invoke(retrieve, MemberExpression);
            BlockExpression getterBlock = Expression.Block(preInvocation, retrieveExpression);
            GetterExpression = Expression.Lambda(getterBlock, AccessExpression.Parameters);

            InvocationExpression setExpression = Expression.Invoke(set, MemberExpression, set.Parameters[1]);
            BlockExpression setterBlock = Expression.Block(preInvocation, setExpression);
            SetterExpression = Expression.Lambda(setterBlock, AccessExpression.Parameters[0], set.Parameters[1]);

            SerializedType = typeof(TSerialized);
            return this;
        }

        /// <summary>
        /// Force this member to be serialized in the specified type using a cast.
        /// </summary>
        /// <typeparam name="TStruct">Serialized type</typeparam>
        public void As<TStruct>()
        {
            As(typeof(TStruct));
        }

        /// <summary>
        /// Force this member to be serialized in the specified type using a cast.
        /// </summary>
        /// <param name="type">Serialized type</param>
        public void As(Type type)
        {
            GetterExpression = ExpressionBuilder.WrapTypeCastAfterLambda(GetterExpression, type);
            SetterExpression = ExpressionBuilder.WrapTypeCastBeforeLambda(SetterExpression, SerializedType, type);
            SerializedType = type;
        }
    }
}
