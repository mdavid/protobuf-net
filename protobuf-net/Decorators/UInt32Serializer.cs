namespace ProtoBuf.Decorators
{
    class UInt32Serializer : SerializerBase
    {
        private readonly uint? defaultValue;
        public UInt32Serializer(int tag, DataFormat format, uint? defaultValue)
            : base(
                Serializer.GetFieldToken(tag, format == DataFormat.FixedSize ? WireType.Fixed32 : WireType.Variant), format)
        {
            this.defaultValue = defaultValue;
        }

        public override int Serialize(SerializationContext context, object value)
        {
            uint actualValue = (uint)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeUInt32(actualValue);
                case DataFormat.FixedSize:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt32Fixed((int)actualValue);
            }
            return base.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.DecodeUInt32();
                case DataFormat.FixedSize:
                    return (uint)context.DecodeInt32Fixed();
            }
            return base.Deserialize(context, value);
        }
    }

    
}
