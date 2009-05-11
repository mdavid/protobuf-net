namespace ProtoBuf.Decorators
{
    class Int32Serializer : SerializerBase
    {
        private readonly int? defaultValue;
        public Int32Serializer(int tag, DataFormat format, int? defaultValue)
            : base(
                Serializer.GetFieldToken(tag, format == DataFormat.FixedSize ? WireType.Fixed32 : WireType.Variant), format)
        {
            this.defaultValue = defaultValue;
        }

        public override int Serialize(SerializationContext context, object value)
        {
            int actualValue = (int)value;
            if (defaultValue.HasValue && actualValue == defaultValue.GetValueOrDefault()) return 0;
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt32(actualValue);
                case DataFormat.ZigZag:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeUInt32(SerializationContext.ZigInt32(actualValue));
                case DataFormat.FixedSize:
                    return context.EncodeUInt32(FieldPrefix) + context.EncodeInt32Fixed(actualValue);
            }
            return base.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            switch (Format)
            {
                case DataFormat.Default:
                case DataFormat.TwosComplement:
                    return context.DecodeInt32();
                case DataFormat.ZigZag:
                    return SerializationContext.ZagInt32(context.DecodeUInt32());
                case DataFormat.FixedSize:
                    return context.DecodeInt32Fixed();
            }
            return base.Deserialize(context, value);
        }
#if EMIT
        public override void Compile(ILGenerator serialize, ILGenerator deserialize)
        {
            if (defaultValue.HasValue) throw new NotImplementedException("default values");

            LocalBuilder loc = serialize.DeclareLocal(typeof(int));
            serialize.Emit(OpCodes.Stloc, loc);
            EmitField(serialize);

            EmitLoadContext(serialize);
            serialize.Emit(OpCodes.Ldloc, loc);
            switch(Format) {
                case DataFormat.TwosComplement:
                    serialize.Emit(OpCodes.Callvirt, typeof(SerializationContext).GetMethod("EncodeInt32",
                        BindingFlags.Instance | BindingFlags.Public));
                    break;
                case DataFormat.ZigZag:
                    serialize.Emit(OpCodes.Call, typeof(SerializationContext).GetMethod("ZigInt32",
                        BindingFlags.Static | BindingFlags.Public));
                    serialize.Emit(OpCodes.Callvirt, typeof(SerializationContext).GetMethod("EncodeUInt32",
                        BindingFlags.Instance | BindingFlags.Public));
                    break;
                case DataFormat.FixedSize:
                    serialize.Emit(OpCodes.Callvirt, typeof(SerializationContext).GetMethod("EncodeInt32Fixed",
                        BindingFlags.Instance | BindingFlags.Public));
                    break;
                default:
                    base.Compile(serialize, deserialize);
                    break;
            }
            serialize.Emit(OpCodes.Add);    
        }
#endif
    }
}
