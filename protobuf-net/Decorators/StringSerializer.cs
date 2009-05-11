using System.Text;
namespace ProtoBuf.Decorators
{
    sealed class StringSerializer : SerializerBase
    {
        private readonly string defaultValue;
        private static readonly UTF8Encoding utf8 = new UTF8Encoding(false, false);
        public StringSerializer(int tag, string defaultValue)
            : base(Serializer.GetFieldToken(tag, WireType.String), DataFormat.Default)
        {
            this.defaultValue = defaultValue;
        }
        private static int SerializeLongString(SerializationContext context, object obj)
        {
            string value = (string)obj;
            context.CheckSpace(Encoding.UTF8.GetMaxByteCount(value.Length));
            int len = utf8.GetBytes(value, 0, value.Length, context.Workspace, 0);
            context.WriteBlock(context.Workspace, 0, len);
            return len;
        }
        public override int Serialize(SerializationContext context, object untypedValue)
        {
            string value = (string)untypedValue;
            if (defaultValue != null && defaultValue == value) return 0;
            return context.EncodeUInt32(FieldPrefix) + WriteString(value, context);
        }
#if EMIT
        public override void Compile(ILGenerator serialize, ILGenerator deserialize)
        {
            Label end = null;
            if (defaultValue != null)
            {
                serialize.Emit(OpCodes.Dup);
                serialize.Emit(OpCodes.Ldstr, defaultValue);
                serialize.Emit(OpCodes.Call, typeof(string).GetMethod(
                    "Equals", BindingFlags.Public | BindingFlags.Static, null,
                    new Type[] { typeof(string), typeof(string) }, null));
                Label @else = serialize.DefineLabel();
                end = serialize.DefineLabel();
                serialize.Emit(OpCodes.Brtrue_S, lbl);
                serialize.Emit(OpCodes.Pop);
                serialize.Emit(OpCodes.Ldc_I4_0);
                serialize.Emit(OpCodes.Br, end);
                serialize.MarkLabel(@else);
            }
            EmitLoadContext(serialize);
            serialize.EmitCall(OpCodes.Call,
                typeof(StringSerializer).GetMethod(
                "WriteString", BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(SerializationContext), typeof(string) }, null));
            if (end != null) serialize.MarkLabel(end);

            deserialize.Emit(OpCodes.Pop);
            EmitLoadContext(deserialize);
            deserialize.EmitCall(OpCodes.Call, typeof(StringSerializer).GetMethod(
                "ReadString", BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(SerializationContext)}, null));
        }
#endif
        public static int WriteString(string value, SerializationContext context)
        {
            int charCount = value.Length;
            uint underEstimate = (uint)charCount;
            if (charCount == 0)
            {
                context.WriteByte(0);
                return 1;
            }
            else if (charCount <= 42)
            {
                // guaranteed to have a byte length at most 127, so single byte;
                // any text up to 42 chars will take at most 126 bytes
                context.CheckSpace((3 * charCount) + 1);
                int byteCount = utf8.GetBytes(value, 0, charCount, context.Workspace, 1);
                context.Workspace[0] = (byte)byteCount;
                context.WriteBlock(context.Workspace, 0, ++byteCount);
                return byteCount;

            }
            else if (charCount <= 127)
            {
                // common text in many locales will /tend/ to be single-byte. We'll
                // absorb the cost of checking the actual length, since we know it
                // is only a short string.
                underEstimate = (uint)utf8.GetByteCount(value);
                if (underEstimate <= 127)
                {
                    context.CheckSpace((int)(underEstimate + 1));
                    int byteCount = utf8.GetBytes(value, 0, charCount, context.Workspace, 1);
                    context.Workspace[0] = (byte)byteCount;
                    context.WriteBlock(context.Workspace, 0, ++byteCount);
                    return byteCount;
                }
                // note also that we update "underEstimate"; this means that even
                // if we find a 100-char string actually needs multiple bytes
                // (and so we'll use the callback below), we at least start the
                // callback with the correct length, avoiding the need to
                // encode it twice.
            }

            // when all else fails (longer strings), use a callback
            // to to a length prefix using our estimated length...
            return context.WriteLengthPrefixed(value, underEstimate, SerializeLongString);
        }
        public static string ReadString(SerializationContext context)
        {
            int len = (int)context.DecodeUInt32();
            if (len == 0)
            {
                return "";
            }
            else
            {
                if (len > SerializationContext.InitialBufferLength) context.CheckSpace(len);
                context.ReadBlock(len);
                return utf8.GetString(context.Workspace, 0, len);
            }
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            return ReadString(context);
        }
    }
}
