# nanoFramework.MessagePack

nanoFramework.MessagePack is a simple, lightweight [MsgPack.Light](https://github.com/progaudi/MsgPack.Light) serialization library that can be used in [nanoFramework](https://github.com/nanoframework) solutions.

[MessagePack](https://github.com/msgpack/msgpack) is an object serialization specification like JSON.

## Usage
### Serialization to bytes array:
```C#
var value = new TestClass();
var bytes = MessagePackSerializer.Serialize(value);
```
### Deserialization:
```C#
var result = (TestClass)MessagePackSerializer.Deserialize(typeof(TestClass), bytes);
```
### Your type serialization/deserialization:
If you want to work with your own types, first thing you need - type converter.
<<TODO>>
