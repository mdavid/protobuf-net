namespace ProtoBuf.Decorators
{
    sealed class BlobSerializer : SerializerBase
    {
        public BlobSerializer(int tag)
            : base(Serializer.GetFieldToken(tag, WireType.String))
        { }
        public override int Serialize(SerializationContext context, object obj)
        {
            byte[] value = (byte[])obj;
            int arrayLen = value.Length;
            int len = context.EncodeUInt32(FieldPrefix)
                + context.EncodeUInt32((uint)arrayLen);
            if (arrayLen > 0)
            {
                context.WriteBlock(value, 0, arrayLen);
            }
            return len + arrayLen;
        }
    }
}
