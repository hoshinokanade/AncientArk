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
using System.IO;
using System.Reflection;
using System.Text;

namespace AncientArk
{
    /// <summary>
    /// Defines the behavior of a profile registry
    /// </summary>
    public interface IProfileMapper
    {
        /// <summary>
        /// Register a profile map
        /// </summary>
        /// <typeparam name="TProfileMap"></typeparam>
        void RegisterProfile<TProfileMap>() where TProfileMap : ProfileMap, new();

        /// <summary>
        /// Register a profile map
        /// </summary>
        /// <typeparam name="TProfileMap"></typeparam>
        void RegisterProfile(ProfileMap profile);

        /// <summary>
        /// Register all profile maps under the specified assembly
        /// </summary>
        /// <param name="assembly"></param>
        void RegisterProfiles(Assembly assembly);
    }

    /// <summary>
    /// Defines the behavior of a reading/writing type registry
    /// </summary>
    public interface ITypeMapper
    {
        /// <summary>
        /// Register a custom type serializer/deserialize function
        /// </summary>
        void RegisterType<T>(Action<BinaryWriter, object> serialize, Func<BinaryReader, object> deserialize);

        /// <summary>
        /// Register a custom type serializer/deserialize function
        /// </summary>
        void RegisterType(Type type, Action<BinaryWriter, object> serialize, Func<BinaryReader, object> deserialize);
    }
}
