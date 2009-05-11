using System;
using ProtoBuf.ProtoBcl;
namespace ProtoBuf.Decorators
{
    sealed class TimeSpanSerializer : CompositeSerializer
    {
        public TimeSpanSerializer(int tag, DataFormat format, TimeSpan? defaultValue)
            : base(tag, format)
        {
            this.defaultValue = defaultValue;
        }
        readonly TimeSpan? defaultValue;

        public override int Serialize(SerializationContext context, object value)
        {
            TimeSpan span = (TimeSpan)value;
            if (defaultValue != null && span == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoTimeSpan.SerializeTimeSpan(span, context, true);
                case DataFormat.Group:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoTimeSpan.SerializeTimeSpan(span, context, false)
                        + context.EncodeUInt32(GroupSuffix);
                case DataFormat.FixedSize:
                    throw new NotImplementedException("todo");
            }
            return base.Serialize(context, value);
        }
    }
}
