namespace ProtoBuf.Decorators
{
    class SingleSerializer : SerializerBase
    {
        private readonly float? defaultValue;
        public SingleSerializer(int tag, float? defaultValue)
            : base(Serializer.GetFieldToken(tag, WireType.Fixed32), DataFormat.FixedSize)
        {
            this.defaultValue = defaultValue;
        }
        public override int Serialize(SerializationContext context, object value)
        {
            float actualValue = (float)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            return context.EncodeUInt32(FieldPrefix) + context.EncodeSingle(actualValue);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            return context.DecodeSingle();
        }
    }
    
}
