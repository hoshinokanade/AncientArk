# AncientArk

[![Build Status](https://travis-ci.com/hoshinokanade/AncientArk.svg?branch=master)](https://travis-ci.com/hoshinokanade/AncientArk)



AncientArk is a binary serializer to adapt to legacy data storage and data transfer protocols.
It allows expression mappings of custom types.

Supported platforms: .NET Standard 2.0

AncientArk is suitable for emulating any sequenced data transfer protocols
(which does not require jumping when reading/writing).

## Legacy data transfer problem
When newly developed applications must to be compatible with a legacy data protocol,
where the sequences and types of read/write are magically known in advance.
Most of these data protocols are not json, msgpack, not even xml, but some dirty hardcodes.
They could be file and packet protocols, as long as they end up in binary format.

AncientArk is then used to bridge the data transfer objects to fit into the protocol.
This keeps the compatibility while decouples the nice modern code from the dirty legacy data protocol.

Legacy data protocol are sometimes transferred in TCP/IP instead of mainstream higher-level protocols like HTTPS. Try also [Ether.Network](https://github.com/Eastrall/Ether.Network) to tackle this scenario.

## Usage
```
// Must have a public parameterless constructor
public class Ping
{
    // Now only for properties with public getter & setter
    public int AckNumber {get; set;}
    
    // Custom types could be used with prior registration
    // Custom types must have public parameterless constructor
    public OtherObj ClientInfo {get; set;}
    
    public sealed class PingMap: ProfileMap<Ping>
    {
        public PingMap()
        {
            Map(x => x.AckNumber);
            // ClientInfo is not mapped in, thereby ignored
        }
    }
}

ProfileSerializer.Default.RegisterProfile<Ping>();
Ping ping = new Ping();
ping.AckNumber = 2018;

Memorystream ms = new Memorystream();
ProfileSerializer.Serialize<Ping>(ms, ping);
// Output: E2 07 00 00
```
## Documentation

[Our Wiki](../../wiki)

## License
GNU General Public License v3

http://www.gnu.org/licenses
