#if !CF
//#define EMIT
#endif
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
#if EMIT
using System.Reflection.Emit;
#endif
using ProtoBuf.Property;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using ProtoBuf.ProtoBcl;


namespace ProtoBuf.Decorators
{
    internal interface ISerializerBuilder
    {
        ISerializer Create();
    }
    internal interface ISerializer
    {
        void Prepare();
        uint FieldPrefix { get; }
        int Serialize(SerializationContext context, object value);
        object Deserialize(SerializationContext context, object value);
#if EMIT
        void CompileSerialize(ILGenerator il);
        void CompileDeserialize(ILGenerator il);
        ISerializer Compile();
#endif
    }
 
    

    

    

    

    

    

    
    
    

    
    
    
    delegate int SerializerImpl(SerializationContext context, object value);
    delegate object DeserializerImpl(SerializationContext context, object value);
    
#if EMIT
    class CompiledSerializer : ISerializer
    {
        ISerializer ISerializer.Compile() { return this; }
        private readonly uint fieldPrefix;
        public uint FieldPrefix { get { return fieldPrefix; } }
        private readonly SerializerImpl serialize;
        private readonly DeserializerImpl deserialize;
        public CompiledSerializer(
            SerializerImpl serialize,
            DeserializerImpl deserialize,
            uint fieldPrefix)
        {
            this.serialize = serialize;
            this.deserialize = deserialize;
        }


        public int Serialize(SerializationContext context, object value)
        {
            return serialize(context, value);
        }

        public object Deserialize(SerializationContext context, object value)
        {
            return deserialize(context,value);
        }

        public void Compile(ILGenerator serialize, ILGenerator deserialize)
        {
            throw new NotImplementedException();
        }
    }
#endif
    

    
    

    

    

    
    
    

    

    

#if EMIT && !NET_2_0
    public static class PerfTest
    {
        [ProtoContract]
        class TestObject
        {
            [ProtoMember(1)]
            public int TestValue { get; set; }
        }
#if EMIT && !SILVERLIGHT
        public static void RunPerformance()
        {
            PropertyInfo prop = typeof(TestObject).GetProperty("TestValue");

            //Entity
            //ClassSerializer.Build(new Entity(
            IRootSerializer dec = SerializerFactory.Create(prop, 1, DataFormat.TwosComplement, null);
            ISerializer compiled = dec.Compile();   

            Property<TestObject> oldProp = PropertyFactory.Create<TestObject>(prop);

            SerializationContext ctx;
            MemoryStream ms;
            
            TestObject obj = new TestObject { TestValue = 123};

            ms = new MemoryStream();
            ctx = new SerializationContext(ms, null);
            Console.WriteLine(oldProp.Serialize(obj, ctx));
            ctx.Flush();
            string s1 = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

            ms = new MemoryStream();
            ctx = new SerializationContext(ms, null);
            Console.WriteLine(dec.Serialize(ctx, obj));
            ctx.Flush();
            string s2 = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

            ms = new MemoryStream();
            ctx = new SerializationContext(ms, null);
            Console.WriteLine(compiled.Serialize(ctx, obj));
            ctx.Flush();
            string s3 = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);

            ctx = new SerializationContext(Stream.Null, null);

            const int LOOP = 30000000;
            GC.Collect();
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < LOOP; i++)
            {
                oldProp.Serialize(obj, ctx);
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            ctx = new SerializationContext(Stream.Null, null);
            GC.Collect();
            watch = Stopwatch.StartNew();
            for (int i = 0; i < LOOP; i++)
            {
                dec.Serialize(ctx, obj);
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);

            ctx = new SerializationContext(Stream.Null, null);
            GC.Collect();
            watch = Stopwatch.StartNew();
            for (int i = 0; i < LOOP; i++)
            {
                compiled.Serialize(ctx, obj);
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
#endif
    }   
#endif
}
