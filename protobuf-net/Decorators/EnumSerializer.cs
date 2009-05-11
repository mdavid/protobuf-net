using System.Collections;
using System;
using System.Reflection;
namespace ProtoBuf.Decorators
{
    sealed class EnumSerializer : Int32Serializer
    {
        private readonly Hashtable enumToWire;
        private EnumSerializer(int tag, int? defaultWireValue, Hashtable enumToWire)
            : base(tag, DataFormat.Default, defaultWireValue)
        {
            this.enumToWire = enumToWire;
        }

        internal static EnumSerializer Build(int tag, object defaultEnumValue, Entity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Hashtable enumToWire = new Hashtable(entity.Members.Count);
            foreach (EntityMember member in entity.Members)
            {
                object enumValue = ((FieldInfo)member.Member).GetValue(null);
                int wireValue = (int)(member.DefaultValue ?? enumValue);
                foreach (DictionaryEntry pair in enumToWire)
                {
                    if (pair.Key == enumValue || (int)pair.Value == wireValue)
                    {
                        throw new ProtoException(string.Format("The enum {0} has conflicting values {1} and {2}",
                         entity.Type, pair.Key, member.Name));
                    }
                }
                
                enumToWire.Add(enumValue, wireValue);
            }
            int? defaultWireValue = null;
            if (defaultEnumValue != null)
            {
                object tmp = enumToWire[defaultEnumValue];
                if (tmp == null)
                {
                    throw new ProtoException(string.Format(
                        "The default enum value ({0}.{1}) has no wire-representation",
                        defaultEnumValue.GetType().Name, defaultEnumValue));
                }
                defaultWireValue = (int)tmp;

            }

            EnumSerializer ser = new EnumSerializer(tag, defaultWireValue, enumToWire);
            return ser;
        }

        public override int Serialize(SerializationContext context, object value)
        {
            object wireValue = enumToWire[value];
            if (wireValue == null)
            {
                throw new ProtoException(string.Format(
                    "The value ({0}.{1}) has no wire-representation",
                    value.GetType().Name, value));
            }
            return base.Serialize(context, wireValue);
        }
    }
}
