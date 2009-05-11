
using System;
using System.Collections;
using System.Reflection;
namespace ProtoBuf.Decorators
{
    class RepeatedDecorator : DecoratorBase
    {
        private readonly Type elementType;
        private readonly Type listType;
        protected override Type ExpectedType
        {
            get { return elementType; }
        }
        public static Type GetElementType(Type listType)
        {
            MethodInfo addMethod;
            Type elementType;
            ResolveAdd(listType, out elementType, out addMethod);
            return elementType;
        }
        private static bool ResolveAdd(Type listType, out Type elementType, out MethodInfo addMethod) {
            addMethod = null;
            elementType = null;
            if (listType == null) throw new ArgumentNullException("listType");
            if (listType.IsArray || !typeof(IEnumerable).IsAssignableFrom(listType)) return false;

            MethodInfo candidate = null;
            ParameterInfo[] args = null;
            int addCount = 0;
            foreach (MethodInfo method in listType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                
                if (method.Name == "Add" && (args = method.GetParameters()).Length == 1)
                {
                    candidate = method;
                    addCount++;
                }
            }
            if(addCount == 1) {
                addMethod = candidate;
                elementType = args[0].ParameterType;
                return true;
            }
            return false;
        }
        public RepeatedDecorator(Type listType, Type elementType, ISerializerBuilder builder) : base(builder)
        {
            if (listType == null) throw new ArgumentNullException("listType");
            if (elementType == null) throw new ArgumentNullException("elementType");
            this.listType = listType;
            this.elementType = elementType;            
        }
        public override int Serialize(SerializationContext context, object value)
        {
            int len = 0;
            IEnumerator iter = ((IEnumerable)value).GetEnumerator();
            using (iter as IDisposable)
            {
                if (iter.MoveNext())
                {
                    ISerializer tail = Tail;
                    do
                    {
                        value = iter.Current;
                        if (value == null) throw new ProtoException("Cannot serialize null in a collection");
                        len += tail.Serialize(context, value);
                    } while (iter.MoveNext());
                }
            }
            return len;
        }
    }
}
