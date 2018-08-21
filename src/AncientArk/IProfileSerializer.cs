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
    /// Defines the behavior of a serializer.
    /// </summary>
    public interface IProfileSerializer: IProfileMapper, ITypeMapper
    {
        /// <summary>
        /// Serialize profile to binary stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="profile"></param>
        void Serialize(Stream stream, Type type, object profile);

        /// <summary>
        /// Serialize profile to binary stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        void Serialize<T>(Stream stream, T profile);

        /// <summary>
        /// Deserialize binary stream to profile
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        object Deserialize(Stream stream, Type type);

        /// <summary>
        /// Deserialize binary stream to provided profile instance
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        object Deserialize(Stream stream, Type type, object profile);

        /// <summary>
        /// Deserialize binary stream to profile
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        T Deserialize<T>(Stream stream) where T: new();

        /// <summary>
        /// Deserialize binary stream to provided profile instance
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        T Deserialize<T>(Stream stream, T profile);
    }
}
