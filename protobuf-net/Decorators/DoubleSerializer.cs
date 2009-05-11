namespace ProtoBuf.Decorators
{
    class DoubleSerializer : SerializerBase
    {
        private readonly double? defaultValue;
        public DoubleSerializer(int tag, double? defaultValue)
            : base(Serializer.GetFieldToken(tag, WireType.Fixed64), DataFormat.FixedSize)
        {
            this.defaultValue = defaultValue;
        }
        public override int Serialize(SerializationContext context, object value)
        {
            double actualValue = (double)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            return context.EncodeUInt32(FieldPrefix) + context.EncodeDouble(actualValue);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            return context.DecodeDouble();
        }
    }
}
