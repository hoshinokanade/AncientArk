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

using AncientArk.Expressions;

namespace AncientArk
{
    public abstract class ProfileMap<TProfile> : ProfileMap
    {
        /// <summary>
        /// Instantiate a profile map that targets to a profile type
        /// </summary>
        /// <param name="memberExpression"></param>
        public ProfileMap(): base(typeof(TProfile))
        {

        }

        /// <summary>
        /// Map a member to relate its serialization/deserialization behaviors.
        /// </summary>
        /// <param name="memberExpression"></param>
        public ProfileMember<TMember> Map<TMember>(Expression<Func<TProfile, TMember>> expression)
        {
            MemberExpression memberExpression = ExpressionHelper.GetMemberExpression(expression.Body);
            Type genericType = typeof(ProfileMember<>).MakeGenericType(typeof(TMember));
            ProfileMember<TMember> member = Activator.CreateInstance(genericType, memberExpression) as ProfileMember<TMember>;
            
            ProfileMembers.Add(member);
            return member;
        }
    }
}
