using System;
namespace ProtoBuf.Decorators
{
    abstract class DecoratorBase : ISerializer
    {

#if EMIT
        ISerializer ISerializer.Compile()
        {
            return Compile();
        }


        public ISerializer Compile()
        {
            DynamicMethod method = new DynamicMethod("ser_" + member.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(int), new Type[] { typeof(SerializationContext), typeof(object) }, ExpectedType, true);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, ExpectedType);
            CompileSerialize(il);
            il.Emit(OpCodes.Ret);
            SerializerImpl ser = (SerializerImpl)method.CreateDelegate(typeof(SerializerImpl), null);
/*
            DynamicMethod method = new DynamicMethod("deser_" + member.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(int), new Type[] { typeof(SerializationContext), typeof(object) }, ExpectedType, true);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, ExpectedType);
            CompileSerialize(il);
            il.Emit(OpCodes.Ret);*/
            DeserializerImpl deser = null; //(DeserializerImpl)method.CreateDelegate(typeof(DeserializerImpl), null);
            return new CompiledSerializer(ser, deser, Tail.FieldPrefix);
        }
#endif
        protected abstract Type ExpectedType { get; }
        private ISerializerBuilder builder;
        void ISerializer.Prepare()
        {
            Build();
        }
        ISerializer Build()
        {
            if (tail == null)
            {
                tail = builder.Create();
                tail.Prepare();
                builder = null;
            }
            return tail;
        }
        protected ISerializer Tail { get { return tail ?? Build(); } }

        private ISerializer tail;
        uint ISerializer.FieldPrefix { get { return Tail.FieldPrefix; } }
        protected DecoratorBase(ISerializerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            this.builder = builder;
        }
        private Exception NotSupported(string method)
        {
            return new NotSupportedException("No " + method + " implementation found for " + GetType().Name + " (field " + Serializer.ParseTag(Tail.FieldPrefix) + ")");
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
        public abstract void CompileSerialize(ILGenerator il);
        public abstract void CompileDeserialize(ILGenerator il);
#endif
    }
}
