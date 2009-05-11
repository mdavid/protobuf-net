using System;
namespace ProtoBuf.Decorators
{
    sealed class ArgsArrayDecorator : DecoratorBase
    {
        protected override Type ExpectedType
        {
            get { return typeof(object[]); }
        }
#if EMIT
        public override void CompileSerialize(ILGenerator il)
        {
            throw new NotImplementedException();
        }
        public override void CompileDeserialize(ILGenerator il)
        {
            throw new NotImplementedException();
        }
#endif
        private readonly int index;
        public ArgsArrayDecorator(int index, ISerializerBuilder builder)
            : base(builder)
        {
            this.index = index;
        }
        public override int Serialize(SerializationContext context, object value)
        {
            value = ((object[])value)[index];
            return value == null ? 0 : Tail.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object value)
        {
            ((object[])value)[index] = Tail.Deserialize(context, value);
            return value;
        }
    }
}
