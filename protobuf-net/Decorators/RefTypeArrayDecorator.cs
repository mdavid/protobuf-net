using System;
namespace ProtoBuf.Decorators
{
    class RefTypeArrayDecorator : ArrayDecorator
    {
        public RefTypeArrayDecorator(Type arrayType, ISerializerBuilder builder)
            : base(arrayType, builder) { }
        public override int Serialize(SerializationContext context, object value)
        { // use array-type covariance
            object[] arr = (object[])value;
            int len = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                object val = arr[i];
                if (val != null)
                {
                    len += Tail.Serialize(context, val);
                }
            }
            return len;
        }
    }
}
