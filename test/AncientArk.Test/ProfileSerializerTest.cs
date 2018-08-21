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
using System.Text;
using Xunit;

namespace AncientArk.Test
{
    public class ProfileSerializerTest
    {
        [Fact]
        public void TestSerialize()
        {
            TestObject testObject = TestObject.GetTestObject();
            TestObject.Map map = new TestObject.Map();
            ProfileSerializer.Default.RegisterProfile(map);

            MemoryStream stream = new MemoryStream();
            ProfileSerializer.Default.Serialize(stream, testObject);
            byte[] debug = stream.ToArray();
        }

        [Fact]
        public void TestDeserialize()
        {
            TestObject testObject = TestObject.GetTestObject();
            TestObject.Map map = new TestObject.Map();
            ProfileSerializer.Default.RegisterProfile(map);

            MemoryStream stream = new MemoryStream();
            ProfileSerializer.Default.Serialize(stream, testObject);
            stream.Seek(0, SeekOrigin.Begin);

            TestObject deserialized = new TestObject();
            ProfileSerializer.Default.Deserialize(stream, deserialized);
            Assert.Equal(200, deserialized.EmbededObjectA.EmbededObjectB.EmbededPropertyB);
        }
    }
}
