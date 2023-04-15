using GNet.Packets;
using System;
using System.Reflection;

namespace GNet.Packets
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ReadPacketAttribute : Attribute
    {
        public Int32 Packet { get; set; }
    }

    public class PacketReaderInfo
    {
        public Type type { get; private set; }
        public MethodInfo info { get; private set; }

        public void Invoke(Packet _packet, out object[] _data)
        {
            object[] parameters = new object[] { _packet, null };
            info.Invoke(null, parameters);
            _data = (object[])parameters[1];
        }

        public PacketReaderInfo(Type _type, MethodInfo _info)
        {
            type = _type;
            info = _info;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BindPacketAttribute : Attribute
    {
        public int Packet { get; set; }
    }

    public class PacketBinder
    {
        public object instance { get; private set; }
        public MethodInfo info { get; private set; }

        public void Invoke(object[] parameters)
        {
            info.Invoke(instance, parameters);
        }

        public PacketBinder(object _instance, MethodInfo _info)
        {
            instance = _instance;
            info = _info;
        }
    }
}
