namespace ProtoBuf.Decorators
{
    using KeyedSerializer = System.Collections.Generic.KeyValuePair<uint, ProtoBuf.Decorators.ISerializer>;
    using TypedSerializer = System.Collections.Generic.KeyValuePair<System.Type, ProtoBuf.Decorators.ISerializer>;
    using System;
    using System.Collections.Generic;

    sealed class ClassSerializer : SerializerBase
    {

        internal static ClassSerializer Build(EntityModel model, Type type, bool startFromRoot)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (type == null) throw new ArgumentNullException("type");
            Entity entity = model.Resolve(type, false);
            if (entity == null || entity.IsWrapper) throw new ArgumentException("Type is not in the model: " + type.FullName, "type");
            Type rootType = type;
            if (startFromRoot)
            {
                Entity parent;
                while (rootType.BaseType != typeof(object)
                    && (parent = model.Resolve(rootType.BaseType, false)) != null
                    && !parent.IsWrapper)
                {
                    rootType = rootType.BaseType;
                    entity = parent;
                }
            }

            ClassSerializer ser = new ClassSerializer(type, rootType);
            List<ISerializer> writers = new List<ISerializer>(entity.Members.Count);
            foreach (EntityMember member in entity.Members)
            {
                ISerializer writer = model.Create(member.Member, member.Tag, member.Format, member.DefaultValue, member.IsRequired);
                writers.Add(writer
#if EMIT
        .Compile()              
#endif
);
            }

            // use KeyedSerializer for readers

            List<TypedSerializer> subclasses = new List<TypedSerializer>();
            foreach (EntitySubclass knownType in entity.KnownTypes)
            {
                subclasses.Add(new TypedSerializer(knownType.KnownType,
                    new ClassDecorator(knownType.Tag, knownType.Format,
                        new KnownTypeBuilder(model, knownType.KnownType))));
            }

            ser.writeProps = writers.ToArray();
            ser.subclasses = subclasses.ToArray();
            return ser;
        }
        class KnownTypeBuilder : ISerializerBuilder
        {
            public KnownTypeBuilder(EntityModel model, Type type)
            {
                this.model = model;
                this.type = type;
            }
            ISerializer serializer;
            Type type;
            EntityModel model;
            ISerializer ISerializerBuilder.Create()
            {
                if (serializer != null) return serializer;
                serializer = ClassSerializer.Build(model, type, false);
                type = null;
                model = null;
                return serializer;
            }
        }
        KeyedSerializer[] readProps;
        ISerializer[] writeProps;
        TypedSerializer[] subclasses;

        private readonly Type concreteType, consideredType;
        internal Type ExpectedType { get { return consideredType; } }

        private ClassSerializer(Type concreteType, Type consideredType)
            : base(0, DataFormat.Default)
        {
            if (consideredType == null) throw new ArgumentNullException("consideredType");
            if (concreteType == null) throw new ArgumentNullException("concreteType");
            this.concreteType = concreteType;
            this.consideredType = consideredType;
        }


        public override int Serialize(SerializationContext context, object instance)
        {
            int total = 0;
            Type actualType = instance.GetType();
            if (actualType != consideredType)
            {
                bool subclassFound = false;
                foreach (TypedSerializer subclass in subclasses)
                {
                    if (subclass.Key.IsAssignableFrom(actualType))
                    {
                        total += subclass.Value.Serialize(context, instance);
                        subclassFound = true;
                        break;
                    }
                }
                if (!subclassFound)
                {
                    throw new ProtoException("Unexpected type found during serialization; types must be included with ProtoIncludeAttribute; "
                        + "found " + actualType.Name + " passed as " + consideredType.Name);
                }
            }
            context.Push(instance);
            for (int i = 0; i < writeProps.Length; i++)
            {
                total += writeProps[i].Serialize(context, instance);
            }
            context.Pop(instance);
            return total;
        }

        public override object Deserialize(SerializationContext context, object instance)
        {
            if (instance == null) instance = Activator.CreateInstance(concreteType);
            uint prefix;
            while (context.TryReadFieldPrefix(out prefix))
            {
                bool found = false;
                for (int i = 0; i < readProps.Length; i++)
                {
                    if (readProps[i].Key == prefix)
                    {
                        readProps[i].Value.Deserialize(context, instance);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    int tag;
                    WireType wireType;
                    Serializer.ParseFieldToken(prefix, out wireType, out tag);
                    Serializer.SkipData(context, tag, wireType);
                }
            }
            return instance;
        }
    }
}
