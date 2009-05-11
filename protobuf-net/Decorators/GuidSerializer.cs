using System;
using ProtoBuf.ProtoBcl;
namespace ProtoBuf.Decorators
{
    sealed class GuidSerializer : CompositeSerializer
    {
        public GuidSerializer(int tag, DataFormat format, Guid? defaultValue)
            : base(tag, format)
        {
            this.defaultValue = defaultValue;
        }
        readonly Guid? defaultValue;

        public override int Serialize(SerializationContext context, object value)
        {
            Guid guid = (Guid)value;
            if (defaultValue != null && guid == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoGuid.Serialize(guid, context, true);
                case DataFormat.Group:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoGuid.Serialize(guid, context, false)
                        + context.EncodeUInt32(GroupSuffix);
                default:
                    return base.Serialize(context, value);
            }
        }
    }
}
