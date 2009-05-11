using System;
namespace ProtoBuf.Decorators
{
    sealed class ClassDecorator : DecoratorBase
    {
        private readonly uint prefix, suffix;
        private readonly int tag;
        private readonly DataFormat format;
        protected override Type ExpectedType
        {
            get { return ((ClassSerializer)Tail).ExpectedType; }
        }
        public ClassDecorator(int tag, DataFormat format, ISerializerBuilder builder)
            : base(builder)
        {
            this.tag = tag;
            this.format = format;
            if (format == DataFormat.Group)
            {
                prefix = Serializer.GetFieldToken(tag, WireType.StartGroup);
                suffix = Serializer.GetFieldToken(tag, WireType.EndGroup);
            }
            else
            {
                prefix = Serializer.GetFieldToken(tag, WireType.String);
            }
        }

        public override int Serialize(SerializationContext context, object value)
        {
            switch (format)
            {
                case DataFormat.Group:
                    return context.EncodeUInt32(prefix) + Tail.Serialize(context, value)
                        + context.EncodeUInt32(suffix);
                case DataFormat.Default:
                    return context.EncodeUInt32(prefix) + context.WriteLengthPrefixed(value, 0, Tail.Serialize);
                default:
                    return base.Serialize(context, value);
            }
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            switch (format)
            {
                case DataFormat.Group:
                    context.StartGroup(tag); // group will be ended automatically
                    return Tail.Deserialize(context, value);
                default:
                    long restore = context.LimitByLengthPrefix();
                    value = Tail.Deserialize(context, value);
                    context.MaxReadPosition = restore; // restore the max-pos
                    return value;
            }
        }
    }
}
