/*
 * Copyright (C) 2014-2015 Mauricio David
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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AncientArk
{
    /// <summary>
    /// Converter of profiles into/from stream
    /// </summary>
    public class ProfileSerializer: IProfileSerializer
    {
        /// <summary>
        /// Mapping of custom types to serializer
        /// </summary>
        private readonly Dictionary<Type, Action<BinaryWriter, object>> _serializer = new Dictionary<Type, Action<BinaryWriter, object>>();

        /// <summary>
        /// Mapping of custom types to deserializer
        /// </summary>
        private readonly Dictionary<Type, Func<BinaryReader, object>> _deserializer = new Dictionary<Type, Func<BinaryReader, object>>();
        
        /// <summary>
        /// Mapping of custom types to deserializer
        /// </summary>
        private readonly Dictionary<Type, ProfileMap> _profiles = new Dictionary<Type, ProfileMap>();

        /// <summary>
        /// Global instance, although the usage of an instance API is preferred
        /// </summary>
        public static IProfileSerializer Default = new ProfileSerializer();
        
        /// <summary>
        /// Instantiate an instance with default writing and reading type converters
        /// </summary>
        public ProfileSerializer()
        {
            _serializer = new Dictionary<Type, Action<BinaryWriter, object>>(DefaultConverters._writeTypeConverters);
            _deserializer = new Dictionary<Type, Func<BinaryReader, object>>(DefaultConverters._readTypeConverters);
        }

        /// <summary>
        /// Instantiate an instance with custom writing and reading type converters
        /// </summary>
        /// <param name="writingConverters"></param>
        /// <param name="readingConverters"></param>
        public ProfileSerializer(IWritingConverters writingConverters, IReadingConverters readingConverters)
        {
            _serializer = new Dictionary<Type, Action<BinaryWriter, object>>(writingConverters.Converters);
            _deserializer = new Dictionary<Type, Func<BinaryReader, object>>(readingConverters.Converters);
        }

        /// <summary>
        /// Register a custom type serializer/deserialize function
        /// </summary>
        public void RegisterType<T>(Action<BinaryWriter, object> serialize, Func<BinaryReader, object> deserialize)
        {
            _serializer[typeof(T)] = (stream, obj) => serialize(stream, (T)obj);
            _deserializer[typeof(T)] = (stream) => (T)deserialize(stream);
        }

        /// <summary>
        /// Register a custom type serializer/deserialize function
        /// </summary>
        public void RegisterType(Type type, Action<BinaryWriter, object> serialize, Func<BinaryReader, object> deserialize)
        {
            _serializer[type] = (stream, obj) => serialize(stream, obj);
            _deserializer[type] = (stream) => deserialize(stream);
        }

        /// <summary>
        /// Register a profile
        /// </summary>
        public void RegisterProfile<TProfileMap>() where TProfileMap : ProfileMap, new()
        {
            RegisterProfile(Activator.CreateInstance<TProfileMap>());
        }

        /// <summary>
        /// Register all profile maps under the specified assembly
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterProfiles(Assembly assembly)
        {
            var maps = from type in assembly.GetTypes()
                       where typeof(ProfileMap).IsAssignableFrom(type)
                       select (Activator.CreateInstance(type));
            foreach (ProfileMap map in maps)
            {
                RegisterProfile(map);
            }
        }

        /// <summary>
        /// Register a profile
        /// </summary>
        public void RegisterProfile(ProfileMap profile)
        {
            profile.Build();
            _profiles[profile.ProfileType] = profile;
        }

        /// <summary>
        /// Serialize profile to binary stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        public void Serialize<T>(Stream stream, T profile)
        {
            Serialize(stream, typeof(T), profile);
        }

        /// <summary>
        /// Serialize profile to binary stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="profile"></param>
        public void Serialize(Stream stream, Type type, object profile)
        {
            ProfileMap profileMap = _profiles[type];
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (ProfileMember member in profileMap.ProfileMembers)
            {
                object value = member.Getter.Invoke(profile);
                _serializer[member.SerializedType].Invoke(writer, value);
            }
            writer.Flush();
        }

        /// <summary>
        /// Deserialize binary stream to profile
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        public object Deserialize(Stream stream, Type type)
        {
            return Deserialize(stream, type, ReflectionHelper.CreateInstance(type));
        }

        /// <summary>
        /// Deserialize binary stream to provided profile instance
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        public object Deserialize(Stream stream, Type type, object profile)
        {
            ProfileMap profileMap = _profiles[type];
            BinaryReader reader = new BinaryReader(stream);
            foreach (ProfileMember member in profileMap.ProfileMembers)
            {
                object value = _deserializer[member.SerializedType].Invoke(reader);
                member.Setter.Invoke(profile, value);
            }
            return profile;
        }

        /// <summary>
        /// Deserialize binary stream to profile
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        public T Deserialize<T>(Stream stream) where T : new()
        {
            return (T)Deserialize(stream, typeof(T), Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Deserialize binary stream to provided profile instance
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="profile"></param>
        public T Deserialize<T>(Stream stream, T profile)
        {
            return (T)Deserialize(stream, typeof(T), profile);
        }
    }
}