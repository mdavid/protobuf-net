using System.Reflection;
using System;
namespace ProtoBuf.Decorators
{
    sealed class FieldDecorator : DecoratorBase
    {
        protected override Type ExpectedType
        {
            get { return field.DeclaringType; }
        }
        private readonly FieldInfo field;
        public FieldDecorator(FieldInfo field, ISerializerBuilder builder)
            : base(builder)
        {
            this.field = field;
        }
        public override int Serialize(SerializationContext context, object entity)
        {
            object value = field.GetValue(entity);
            return value == null ? 0 : Tail.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object entity)
        {
            object value = field.GetValue(entity);
            value = Tail.Deserialize(context, value);
            field.SetValue(entity, value);
            return entity;
        }
    }
}
