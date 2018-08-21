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

namespace AncientArk
{
    public abstract class ProfileMap
    {
        /// <summary>
        /// List of all members
        /// </summary>
        public List<ProfileMember> ProfileMembers { get; }

        /// <summary>
        /// Indicate which Type this entity mapper is
        /// </summary>
        public Type ProfileType { get; set; }

        /// <summary>
        /// Instantiate a profile map that targets to a profile type
        /// </summary>
        /// <param name="type">The profile type.</param>
        public ProfileMap(Type type)
        {
            ProfileType = type;
            ProfileMembers = new List<ProfileMember>();
        }
        
        /// <summary>
        /// Finalize the mapping expressions of profile members
        /// </summary>
        public void Build()
        {
            foreach (ProfileMember member in ProfileMembers)
            {
                member.Build();
            }
        }
    }
}
