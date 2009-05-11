using System;
using ProtoBuf.ProtoBcl;
namespace ProtoBuf.Decorators
{
    sealed class DateTimeSerializer : CompositeSerializer
    {
        public DateTimeSerializer(int tag, DataFormat format, DateTime? defaultValue)
            : base(tag, format)
        {
            this.defaultValue = defaultValue;
        }
        readonly DateTime? defaultValue;

        public override int Serialize(SerializationContext context, object value)
        {
            DateTime when = (DateTime)value;
            if (defaultValue != null && when == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoTimeSpan.SerializeDateTime(when, context, true);
                case DataFormat.Group:
                    return context.EncodeUInt32(FieldPrefix)
                        + ProtoTimeSpan.SerializeDateTime(when, context, false)
                        + context.EncodeUInt32(GroupSuffix);
                case DataFormat.FixedSize:
                    throw new NotImplementedException("todo");
            }
            return base.Serialize(context, value);
        }
    }
}
