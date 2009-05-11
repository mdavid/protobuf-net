using System;
namespace ProtoBuf.Decorators
{
    abstract class CompositeSerializer : SerializerBase
    {
        protected static uint GetField(int tag, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Default:
                    return Serializer.GetFieldToken(tag, WireType.String);
                case DataFormat.Group:
                    return Serializer.GetFieldToken(tag, WireType.StartGroup);
                default:
                    throw new NotSupportedException("Composite data format: " + format);
            }
        }
        protected readonly uint GroupSuffix;
        protected CompositeSerializer(uint prefix, DataFormat format)
            : base(prefix, format)
        {
            GroupSuffix = (prefix & ~(uint)7) | (int)WireType.EndGroup;
        }
        protected CompositeSerializer(int tag, DataFormat format)
            : base(GetField(tag, format), format)
        {
            GroupSuffix = Serializer.GetFieldToken(tag, WireType.EndGroup);
        }
    }
}
