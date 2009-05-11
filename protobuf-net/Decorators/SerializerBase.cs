using System;
namespace ProtoBuf.Decorators
{
    abstract class SerializerBase : ISerializer
    {
        protected virtual void Prepare() { }
        void ISerializer.Prepare() { Prepare(); }
        uint ISerializer.FieldPrefix { get { return this.FieldPrefix; } }
        protected readonly uint FieldPrefix;
        protected readonly DataFormat Format;
        protected SerializerBase(uint fieldPrefix, DataFormat format)
        {
            FieldPrefix = fieldPrefix;
            Format = format;
        }
        protected SerializerBase(uint fieldPrefix) : this(fieldPrefix, DataFormat.Default) { }

        private Exception NotSupported(string method)
        {
            return new NotSupportedException("No " + Format + " " + method + " implementation found for " + GetType().Name + " (field " + Serializer.ParseTag(FieldPrefix) + ")");
        }
        public virtual int Serialize(SerializationContext context, object value)
        {
            throw NotSupported("Serialize");
        }
        public virtual object Deserialize(SerializationContext context, object value)
        {
            throw NotSupported("Deserialize");
        }
#if EMIT
        public virtual void CompileSerialize(ILGenerator il)
        {
            throw NotSupported("CompileSerialize");
        }
        public virtual void CompileDeserialize(ILGenerator il)
        {
            throw NotSupported("CompileDeserialize");
        }

        protected static void EmitLoad(ILGenerator il, uint value)
        {
            switch (value)
            {
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default: il.Emit(OpCodes.Ldc_I4, value); break;
            }
        }
        protected static void EmitLoadContext(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
        }
        protected void EmitField(ILGenerator il)
        {
            EmitLoadContext(il);
            EmitLoad(il, FieldPrefix);
            il.Emit(OpCodes.Callvirt, encodeUInt32);
        }
        static readonly MethodInfo encodeUInt32 = typeof(SerializationContext)
            .GetMethod("EncodeUInt32", BindingFlags.Instance | BindingFlags.Public);
#endif
    }
}
