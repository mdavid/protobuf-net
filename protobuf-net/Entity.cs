using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
#if NET_3_0
using System.Runtime.Serialization;
#endif
using KeyedEntity = System.Collections.Generic.KeyValuePair<System.Type, ProtoBuf.Entity>;
using EntityCache = ProtoBuf.Node<System.Collections.Generic.KeyValuePair<System.Type, ProtoBuf.Entity>>;
using System.Reflection;
using ProtoBuf.Decorators;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;

namespace ProtoBuf
{
    
    class EntityBuilder
    {
        List<EntityMember> members = new List<EntityMember>();
        List<EntitySubclass> knownTypes = new List<EntitySubclass>();

        readonly Type type;
        readonly string name;
        readonly bool isWrapper;
        private readonly EntityModel model;
        private Entity entity;
        internal Entity Entity { get { return entity; } }
        internal static EntityBuilder Create(EntityModel model, Type type, Attribute[] attributes)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (model == null) throw new ArgumentNullException("model");
            if (attributes == null) throw new ArgumentNullException("attributes");

            bool isEntity = false;
            string name = null;

            if (!isEntity)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is ProtoContractAttribute)
                    {
                        ProtoContractAttribute pca = (ProtoContractAttribute)attributes[i];
                        name = pca.Name;
                        isEntity = true;
                        break;
                    }
                }
            }
#if NET_3_0
            if (!isEntity)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is DataContractAttribute)
                    {
                        DataContractAttribute dca = (DataContractAttribute)attributes[i];
                        name = dca.Name;
                        isEntity = true;
                        break;
                    }
                }
            }
#endif
            if (!isEntity)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] is XmlTypeAttribute)
                    {
                        XmlTypeAttribute xta = (XmlTypeAttribute)attributes[i];
                        name = xta.TypeName;
                        isEntity = true;
                        break;
                    }
                }
            }
            if (type.IsEnum) isEntity = true;
            if (string.IsNullOrEmpty(name)) name = type.Name;
            return isEntity ? new EntityBuilder(model, type, false, name) : null;
        }
        private EntityBuilder(EntityModel model, Type type, bool isWrapper, string name)
        {
            this.model = model;
            this.type = type;
            this.name = name;
            this.isWrapper = isWrapper;
        }

        public EntityBuilder AddKnownType(ProtoIncludeAttribute attribute)
        {            
            CheckCommitted();
            if (type.IsEnum)
            {
                throw new InvalidOperationException("Known types are not supported for enums");
            }
            knownTypes.Add(EntitySubclass.Create(attribute));
            return this;
        }

        public EntityBuilder AddKnownTypes()
        {
            CheckCommitted();
            foreach (ProtoIncludeAttribute attribute in
                type.GetCustomAttributes(typeof(ProtoIncludeAttribute), false))
            {
                AddKnownType(attribute);
            }
            return this;
        }

        BindingFlags MemberFlags
        {
            get {
                return type.IsEnum
                    ? BindingFlags.Public | BindingFlags.Static
                    : BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            }
        }
        public EntityBuilder AddMember(string name, params Attribute[] attributes)
        {
            CheckCommitted();
            MemberInfo[] tmp = type.GetMember(name, MemberFlags);
            if (tmp.Length != 1) throw new ArgumentException("Member not found: " + name, "name");
            EntityMember member = EntityMember.Create(tmp[0], attributes);
            if (member == null) throw new InvalidOperationException("Incomplete metadata for member " + name);
            members.Add(member);
            return this;
        }
        internal static Attribute[] GetAttributes(MemberInfo member)
        {
            object[] attribs = member.GetCustomAttributes(false);
            Attribute[] typedArr = new Attribute[attribs.Length];
            for (int i = 0; i < typedArr.Length; i++)
            {
                typedArr[i] = (Attribute)attribs[i];
            }
            return typedArr;
        }
        public EntityBuilder AddMembers()
        {
            CheckCommitted();
            if (type.IsEnum) {
                foreach (MemberInfo member in type.GetMembers(MemberFlags)) {
                    EntityMember em = EntityMember.Create(member, GetAttributes(member));
                    if (em != null) {
                        members.Add(em);
                    }
                }
            } else {
                foreach (MemberInfo member in Serializer.GetProtoMembers(type)) {
                    EntityMember em = EntityMember.Create(member, GetAttributes(member));
                    if (em != null) {
                        members.Add(em);
                    }
                }
            }
            return this;
        }

        void CheckCommitted()
        {
            if (members == null)
            {
                throw new InvalidOperationException("The entity has already be committed.");
            }
        }
        public EntityModel Commit()
        {
            CheckCommitted();
            entity = new Entity(model, type, isWrapper, name, null, members.ToArray(), knownTypes.ToArray());
            model.Add(new KeyedEntity(type, entity));
            members = null;
            return model;
        }
    }

    /// <summary>
    /// Immutable linked list with null-safe operation
    /// </summary>
    internal sealed class Node<T>
    {
        private readonly Node<T> Tail;
        private readonly T Head;
        private Node(Node<T> tail, T head)
        {
            Tail = tail;
            Head = head;
        }
        internal static Node<T> Add(Node<T> tail, T head)
        {
            return new Node<T>(tail, head);
        }
        internal static IEnumerable<T> AsEnumerable(Node<T> tail)
        {
            while (tail != null)
            {
                yield return tail.Head;
                tail = tail.Tail;
            }
        }
    }

    class EntityModel
    {
        internal ISerializer Create(MemberInfo member, int tag, DataFormat format, object defaultValue, bool isValueRequired)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo prop = (PropertyInfo)member;
                    return new PropertyDecorator(prop, CreateBuilder(prop.PropertyType, tag, format, defaultValue, isValueRequired));
                case MemberTypes.Field:
                    FieldInfo field = (FieldInfo)member;
                    return new FieldDecorator(field, CreateBuilder(field.FieldType, tag, format, defaultValue, isValueRequired));
                default:
                    throw new NotSupportedException(member.MemberType.ToString());
            }
        }
        internal ISerializerBuilder CreateBuilder(Type type, int tag, DataFormat format, object defaultValue, bool isValueRequired)
        {
            return new SerializerBuilder(this, type, tag, format, defaultValue, isValueRequired);
        }
        class SerializerBuilder : ISerializerBuilder
        {
            EntityModel model;
            Type type;
            int tag;
            DataFormat format;
            object defaultValue;
            ISerializer serializer;
            bool isValueRequired;
            public SerializerBuilder(EntityModel model, Type type, int tag, DataFormat format, object defaultValue, bool isValueRequired)
            {
                this.model = model;
                this.type = type;
                this.tag = tag;
                this.format = format;
                this.isValueRequired = isValueRequired;
                this.defaultValue = defaultValue;
            }
            public ISerializer Create() {
                if(serializer != null) return serializer;
                serializer = model.Create(type, tag, format, defaultValue, true, isValueRequired);
                model = null;
                type = null;
                defaultValue = null;
                return serializer;
            }
        }

        private static object ChangeType(object value, Type toType)
        {
            Type fromType;
            if (value == null || (fromType = value.GetType()) == toType) return value;

#if !SILVERLIGHT && !CF
            try
            {
#endif
                return Convert.ChangeType(value, toType, CultureInfo.InvariantCulture);
#if !SILVERLIGHT && !CF
            }
            catch
            {
                TypeConverter tc = TypeDescriptor.GetConverter(toType);
                if (tc.CanConvertFrom(fromType))
                {
                    return tc.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                }
#endif
                throw;
            }
        }

        internal ISerializer Create(Type type, int tag, DataFormat format, object defaultValue, bool demand, bool isValueRequired)
        {
            if (type == null) throw new ArgumentNullException("type");
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (defaultValue == null)
            {
                if (!isValueRequired && type.IsValueType)
                {
                    defaultValue = Activator.CreateInstance(type);
                }
            }
            else
            {
                defaultValue = ChangeType(defaultValue, type);
            }
            

            if (type == typeof(int)) return new Int32Serializer(tag, format, (int?)defaultValue);
            if (type == typeof(uint)) return new UInt32Serializer(tag, format, (uint?)defaultValue);
            if (type == typeof(long)) return new Int64Serializer(tag, format, (long?)defaultValue);
            if (type == typeof(ulong)) return new UInt64Serializer(tag, format, (ulong?)defaultValue);
            if (type == typeof(float)) return new SingleSerializer(tag, (float?)defaultValue);
            if (type == typeof(double)) return new DoubleSerializer(tag, (double?)defaultValue);


            if (type == typeof(string)) return new StringSerializer(tag, (string)defaultValue);
            if (type == typeof(Guid)) return new GuidSerializer(tag, format, (Guid?)defaultValue);
            if (type == typeof(DateTime)) return new DateTimeSerializer(tag, format, (DateTime?)defaultValue);
            if (type == typeof(TimeSpan)) return new TimeSpanSerializer(tag, format, (TimeSpan?)defaultValue);
            if (type == typeof(byte[])) return new BlobSerializer(tag);
            Type elementType;


            if ((elementType = RepeatedDecorator.GetElementType(type)) != null)
            {
                return new RepeatedDecorator(type, elementType, CreateBuilder(elementType, tag, format, null, true));
            }
            if (type.IsArray && type.GetArrayRank() == 1)
            {
                return type.GetElementType().IsValueType
                    ? new ArrayDecorator(type, CreateBuilder(type.GetElementType(), tag, format, null, true))
                    : new RefTypeArrayDecorator(type, CreateBuilder(type.GetElementType(), tag, format, null, true));
            }

            Entity e = Resolve(type, true);
            if (e != null && !e.IsWrapper)
            {
                if (type.IsEnum)
                {
                    return EnumSerializer.Build(tag, defaultValue, e);
                }
                return new ClassDecorator(tag, format, e);
            }
            if (demand)
            {
                throw new NotSupportedException("No " + format + " serializer identified for type " + type.FullName);
            }
            return null;
        }
        internal static readonly EntityModel Default = new EntityModel(true);

        private Node<KeyedEntity> items;

        private readonly bool autoAdd;

        public EntityModel(bool autoAdd)
        {
            this.autoAdd = autoAdd;
        }

        public bool IsEntity(Type type)
        {
            if (!type.IsClass || type.IsArray || type == typeof(object)
                || type == typeof(void) || type == typeof(string)) return false;

            Entity e = Resolve(type, false);
            return e != null && !e.IsWrapper;
        }

        public EntityBuilder Add(Type type, params Attribute[] attributes)
        {
            EntityBuilder builder = EntityBuilder.Create(this, type, attributes);
            if (builder == null) throw new InvalidOperationException("Incomplete metadata for type " + type.FullName);
            return builder;
        }

        public ISerializer GetSerializer(Type type, bool demand)
        {
            if(type == null) throw new ArgumentNullException("type");
            Entity entity = Resolve(type, false);
            if (entity == null) throw new InvalidOperationException("Unable to serialize: " + type.FullName);
            return entity.GetSerializer();
        }

        internal Entity Resolve(Type type, bool isCreatingSerializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            foreach (KeyedEntity ke in EntityCache.AsEnumerable(items))
            {
                if (ke.Key == type) return ke.Value;
            }
            Entity e = AutoCreate(type, isCreatingSerializer);
            if (e == null)
            {
                Add(new KeyedEntity(type, null));
            }
            return e;
        }
        
        public Entity this[Type type]
        {
            get {
                return Resolve(type, false);
            }
        }

        private Entity AutoCreate(Type type, bool isCreatingSerializer)
        {
            if (autoAdd)
            {
                Attribute[] attribs = Attribute.GetCustomAttributes(type);
                EntityBuilder eb = EntityBuilder.Create(this, type, attribs);
                if (eb != null)
                {
                    eb.AddMembers().AddKnownTypes().Commit();
                    return eb.Entity;
                }
            }
            ISerializer ser;
            if (!isCreatingSerializer && (ser = Create(type, 1, DataFormat.Default, null, false, false)) != null)
            {
                Entity ent = new Entity(this, type, true, type.FullName, ser, EmptyMembers, EmptySubclasses);
                Add(new KeyedEntity(type, ent));
                return ent;
            }
            return null;            
        }
        static readonly Attribute[] EmptyAttributes = new Attribute[0];
        static readonly EntityMember[] EmptyMembers = new EntityMember[0];
        static readonly EntitySubclass[] EmptySubclasses = new EntitySubclass[0];
        internal void Add(KeyedEntity entity)
        {
            items = EntityCache.Add(items, entity);
        }
    }


    class Entity : ISerializerBuilder
    {
        readonly EntityModel model;
        readonly bool isWrapper;
        public bool IsWrapper { get { return isWrapper; } }
        ISerializer serializer;
        ISerializer ISerializerBuilder.Create() { return GetSerializer(); }
        internal ISerializer GetSerializer()
        {
            if (serializer != null) return serializer;
            serializer = ClassSerializer.Build(model, this.Type, true);
            serializer.Prepare();
            return serializer;
        }

        private readonly Type type;
        private readonly string name;
        private readonly IList<EntityMember> members;
        private readonly IList<EntitySubclass> knownTypes;
        
        public string Name { get { return name; } }
        public Type Type { get { return type; } }
        public IList<EntityMember> Members { get { return members; } }
        internal IList<EntitySubclass> KnownTypes { get { return knownTypes; } }
        
        internal Entity(EntityModel model, Type type, bool isWrapper, string name, ISerializer serializerIfKnown, EntityMember[] members, EntitySubclass[] knownTypes)
        {
            if (model == null) throw new ArgumentNullException("model");
            this.model = model;
            if (type == null) throw new ArgumentNullException("type");
            this.type = type;
            if (string.IsNullOrEmpty(name)) name = type.Name;
            this.name = name;
            if (members == null) throw new ArgumentNullException("members");
            this.members = new ReadOnlyCollection<EntityMember>(members);
            if (knownTypes == null) throw new ArgumentNullException("knownTypes");
            this.knownTypes = new ReadOnlyCollection<EntitySubclass>(knownTypes);
            this.isWrapper = isWrapper;
            this.serializer = serializerIfKnown;
        }
    }

    sealed class EntitySubclass
    {
        private readonly int tag;
        public int Tag { get { return tag; } }

        private readonly DataFormat format;
        public DataFormat Format { get { return format; } }

        private readonly Type knownType;
        public Type KnownType { get { return knownType; } }

        private EntitySubclass(int tag, DataFormat format, Type knownType)
        {
            this.tag = tag;
            this.format = format;
            this.knownType = knownType;
        }

        internal static EntitySubclass Create(ProtoIncludeAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");
            if (attribute.KnownType == null) throw new InvalidOperationException("Incomplete metadata for known type " + attribute.KnownTypeName);
            if (attribute.Tag < 1) throw new InvalidOperationException("The tag must be a positive integer");
            EntitySubclass subclass = new EntitySubclass(
                attribute.Tag, attribute.DataFormat, attribute.KnownType);
            return subclass;            
        }
    }
    sealed class EntityMember
    {
        private readonly string name;
        public string Name { get { return name; } }
        private readonly int tag;
        public int Tag {get {return tag;}}

        private readonly bool isRequired;
        public bool IsRequired { get { return isRequired; } }

        private readonly DataFormat format;
        public DataFormat Format { get { return format; } }

        private readonly object defaultValue;
        public object DefaultValue { get { return defaultValue; } }

        private readonly MemberInfo member;
        public MemberInfo Member { get { return member; } }

        public EntityMember(string name, int tag, DataFormat format, object defaultValue, bool isRequired, MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException("member");
            this.name = name;
            this.tag = tag;
            this.format = format;
            this.defaultValue = defaultValue;
            this.isRequired = isRequired;
            this.member = member;
        }

        internal static EntityMember Create(MemberInfo member, Attribute[] attributes)
        {
            int tag;
            string name;
            DataFormat format;
            bool isRequired;
            Debug.WriteLine("Create member is incomplete...");
            object defaultValue = null;
            if (member.DeclaringType.IsEnum)
            {
                FieldInfo field = member as FieldInfo;
                if(field == null || !field.IsStatic || !field.IsPublic) {
                    return null;
                }
                name = null;
                bool skip = false;
                foreach (Attribute attrib in attributes)
                {
                    if (attrib is ProtoEnumAttribute)
                    {
                        ProtoEnumAttribute pea = (ProtoEnumAttribute)attrib;
                        if (!string.IsNullOrEmpty(pea.Name))
                        {
                            name = pea.Name;
                        }
                        if (pea.HasValue())
                        {
                            defaultValue = pea.Value;
                        }
                        break;
                    }
                    if (attrib is ProtoIgnoreAttribute || attrib is NonSerializedAttribute)
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    if (string.IsNullOrEmpty(name)) name = member.Name;
                    if (defaultValue == null) defaultValue = field.GetValue(null);
                    return new EntityMember(name, 0, DataFormat.Default, defaultValue, false, member);
                }
            }
            else
            {
                foreach (Attribute attrib in attributes)
                {
                    if (attrib is DefaultValueAttribute)
                    {
                        defaultValue = ((DefaultValueAttribute)attrib).Value;
                    }
                }
                if (Serializer.TryGetTag(member, out tag, out name, false, out format, out isRequired))
                {
                    return new EntityMember(name, tag, format, defaultValue, isRequired, member);
                }
            }
            return null;            
        }
    }
}
