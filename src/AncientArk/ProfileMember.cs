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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AncientArk.Expressions;

namespace AncientArk
{
    public abstract class ProfileMember
    {
        internal Func<object, object> Getter { get; private set; }

        internal Action<object, object> Setter { get; private set; }

        protected internal Type SerializedType { get; protected set; }

        protected MemberExpression MemberExpression { get; set; }

        protected LambdaExpression AccessExpression { get; set; }

        protected LambdaExpression GetterExpression { get; set; }

        protected LambdaExpression SetterExpression { get; set; }

        /// <summary>
        /// Instantiate a profile member that targets to a member
        /// </summary>
        /// <param name="memberExpression"></param>
        public ProfileMember(MemberExpression memberExpression)
        {
            (ParameterExpression parameter, Stack<MemberExpression> members) = ExpressionHelper.GetMemberHierarchy(memberExpression);
            Expression accessor = ExpressionHelper.VisitMembers(members);
            LambdaExpression accessExpression = Expression.Lambda(accessor, parameter);

            AccessExpression = accessExpression;
            MemberExpression = memberExpression;
            GetterExpression = ExpressionBuilder.BuildDefaultGetter(memberExpression, AccessExpression);
            SetterExpression = ExpressionBuilder.BuildDefaultSetter(memberExpression, AccessExpression);
            SerializedType = memberExpression.Type;
        }

        /// <summary>
        /// Build the delegates
        /// </summary>
        internal void Build()
        {
            // Build the final getter and setter
            Getter = ExpressionBuilder.BuildFinalGetter(GetterExpression).Compile() as Func<object, object>;
            Setter = ExpressionBuilder.BuildFinalSetter(SetterExpression).Compile() as Action<object, object>;
            // Release memory
            MemberExpression = null;
            AccessExpression = null;
            GetterExpression = null;
            SetterExpression = null;
        }
    }
}
