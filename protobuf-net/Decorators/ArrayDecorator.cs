using System;
namespace ProtoBuf.Decorators
{
    class ArrayDecorator : DecoratorBase
    {
        private readonly Type arrayType;
        protected override Type ExpectedType
        {
            get { return arrayType; }
        }
        public ArrayDecorator(Type arrayType, ISerializerBuilder builder)
            : base(builder)
        {
            if (arrayType == null) throw new ArgumentNullException("arrayType");
            if (!arrayType.IsArray) throw new ArgumentException(arrayType.Name + " is not an array", "arrayType");
            if (arrayType.GetArrayRank() != 1) throw new ArgumentException(arrayType.Name + " is not an 1-dimension array", "arrayType");
            if (arrayType.GetElementType().IsArray) throw new ArgumentException(arrayType.Name + " is a jagged array", "arrayType");
            this.arrayType = arrayType;
        }
        public override int Serialize(SerializationContext context, object value)
        {
            Array arr = (Array)value;
            int arrLen = arr.Length;
            int len = 0;
            if (arrLen > 0)
            {
                ISerializer tail = Tail;
                for (int i = 0; i < arrLen; i++)
                {
                    object val = arr.GetValue(i);
                    if (val != null)
                    {
                        len += tail.Serialize(context, val);
                    }
                }
            }
            return len;

        }
    }
}
