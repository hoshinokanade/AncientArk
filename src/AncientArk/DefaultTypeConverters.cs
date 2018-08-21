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
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AncientArk
{
    /// <summary>
    /// Defines the default reading and writing converters to convert object to/ from binary stream
    /// </summary>
    internal class DefaultConverters
    {
        /// <summary>
        /// Read methods dictionary.
        /// </summary>
        internal static readonly Dictionary<Type, Func<BinaryReader, object>> _readTypeConverters = new Dictionary<Type, Func<BinaryReader, object>>
        {
            { typeof(byte), reader => reader.ReadByte()},
            { typeof(sbyte), reader => reader.ReadSByte()},
            { typeof(short), reader => reader.ReadInt16()},
            { typeof(ushort), reader => reader.ReadUInt16()},
            { typeof(int), reader => reader.ReadInt32()},
            { typeof(uint), reader => reader.ReadUInt32()},
            { typeof(long), reader => reader.ReadInt64()},
            { typeof(ulong), reader => reader.ReadUInt64()},
        };

        /// <summary>
        /// Write methods dictionary.
        /// </summary>
        internal static readonly Dictionary<Type, Action<BinaryWriter, object>> _writeTypeConverters = new Dictionary<Type, Action<BinaryWriter, object>>
        {
            { typeof(byte), (writer, value) => writer.Write((byte)value) },
            { typeof(sbyte), (writer, value) => writer.Write((sbyte)value) },
            { typeof(short), (writer, value) => writer.Write((short)value) },
            { typeof(ushort), (writer, value) => writer.Write((ushort)value) },
            { typeof(int), (writer, value) => writer.Write((int)value) },
            { typeof(uint), (writer, value) => writer.Write((uint)value) },
            { typeof(long), (writer, value) => writer.Write((long)value) },
            { typeof(ulong), (writer, value) => writer.Write((ulong)value) },
        };
    }
}
