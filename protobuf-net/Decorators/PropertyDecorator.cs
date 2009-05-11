using System.Reflection;
using System;
using ProtoBuf.Property;
namespace ProtoBuf.Decorators
{
    sealed class PropertyDecorator : DecoratorBase
    {
        protected override Type ExpectedType
        {
            get { return property.DeclaringType; }
        }
        private readonly PropertyInfo property;
        private readonly PropertyInfo isSpecified;
        public PropertyDecorator(PropertyInfo property, ISerializerBuilder builder)
            : base(builder)
        {
            this.property = property;
            isSpecified = PropertySpecified.GetSpecified(property.DeclaringType, property.Name);
        }
        public override int Serialize(SerializationContext context, object entity)
        {
            if (isSpecified != null && !(bool)isSpecified.GetValue(entity, null)) return 0;
            object value = property.GetValue(entity, null);
            return value == null ? 0 : Tail.Serialize(context, value);
        }
        public override object Deserialize(SerializationContext context, object entity)
        {
            object value = property.GetValue(entity, null);
            value = Tail.Deserialize(context, value);
            property.SetValue(entity, value, null);
            if (isSpecified != null && isSpecified.CanWrite) isSpecified.SetValue(entity, true, null);
            return entity;
        }
#if EMIT
        private bool IsNullable
        {
            get
            {
                if (property.PropertyType.IsValueType &&
                    Nullable.GetUnderlyingType(property.PropertyType) != null) return true;
                return false;
            }
        }
        private void EmitTailConsideringNotNull(Type type, ILGenerator serialize, ILGenerator deserialize)
        {
            if (type.IsValueType)
            {
                if(Nullable.GetUnderlyingType(type) == null)
                {
                    // cannot ever be null
                    Tail.Compile(serialize, deserialize);
                }
                else
                {

                }
            } else
            {

            }
        }
        public override void Compile(ILGenerator serialize, ILGenerator deserialize)
        {
            serialize.EmitCall(OpCodes.Callvirt, property.GetGetMethod());
            deserialize.Emit(OpCodes.Dup);
            deserialize.EmitCall(OpCodes.Callvirt, property.GetGetMethod());
            EmitConsideringNotNull(property.PropertyType, serialize, deserialize);
        }
#endif
    }
        
    /*
#if EMIT
        public override void Compile(ILGenerator serialize, ILGenerator deserialize)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo prop = (PropertyInfo)member;
                    if (prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    {
                        throw new NotImplementedException("Nullable<T>");
                    }
                    serialize.Emit(OpCodes.Callvirt, prop.GetGetMethod());
                    Tail.Compile(serialize, deserialize);
                    break;
                default:
                    throw new NotImplementedException(member.MemberType.ToString());
            }

        }
#endif
    }
     */ 
}


