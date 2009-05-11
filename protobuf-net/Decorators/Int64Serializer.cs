namespace ProtoBuf.Decorators
{
    class Int64Serializer : SerializerBase
    {
        private readonly long? defaultValue;
        public Int64Serializer(int tag, DataFormat format, long? defaultValue)
            : base(
                Serializer.GetFieldToken(tag, format == DataFormat.FixedSize ? WireType.Fixed64 : WireType.Variant), format)
        {
            this.defaultValue = defaultValue;
        }

        public override int Serialize(SerializationContext context, object value)
        {
            long actualValue = (long)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt64(actualValue);
                case DataFormat.ZigZag:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeUInt64(SerializationContext.ZigInt64(actualValue));
                case DataFormat.FixedSize:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt64Fixed(actualValue);
            }
            return base.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.DecodeInt64();
                case DataFormat.ZigZag:
                    return SerializationContext.ZagInt64(context.DecodeUInt64());
                case DataFormat.FixedSize:
                    return context.DecodeInt64Fixed();
            }
            return base.Deserialize(context, value);
        }
    }
}
