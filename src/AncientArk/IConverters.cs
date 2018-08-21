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

namespace AncientArk
{
    /// <summary>
    /// Defines the converters when reading objects from binary stream
    /// </summary>
    public interface IReadingConverters
    {
        Dictionary<Type, Func<BinaryReader, object>> Converters { get; }
    }

    /// <summary>
    /// Defines the converters when writing objects to binary stream
    /// </summary>
    public interface IWritingConverters
    {
        Dictionary<Type, Action<BinaryWriter, object>> Converters { get; }
    }
}
