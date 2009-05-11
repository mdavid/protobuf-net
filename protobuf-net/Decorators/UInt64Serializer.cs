namespace ProtoBuf.Decorators
{
    class UInt64Serializer : SerializerBase
    {
        private readonly ulong? defaultValue;
        public UInt64Serializer(int tag, DataFormat format, ulong? defaultValue)
            : base(
                Serializer.GetFieldToken(tag, format == DataFormat.FixedSize ? WireType.Fixed64 : WireType.Variant), format)
        {
            this.defaultValue = defaultValue;
        }

        public override int Serialize(SerializationContext context, object value)
        {
            ulong actualValue = (ulong)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeUInt64(actualValue);
                case DataFormat.FixedSize:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt64Fixed((long)actualValue);
            }
            return base.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.DecodeUInt64();
                case DataFormat.FixedSize:
                    return (uint)context.DecodeInt64Fixed();
            }
            return base.Deserialize(context, value);
        }
    }

}
