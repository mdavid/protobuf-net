using System;
using System.IO;
using NUnit.Framework;
using ProtoBuf;
namespace Forum1
{
    // see: http://groups.google.com/group/protobuf/msg/bf71a8c50fc5cb32?
    [TestFixture]
    public class Forum1Tests
    {
        [Test]
        public void RoundTrip()
        {
            OrderProto order = new OrderProto();
            order.parentorderid = "abc";
            order.symbol = 12345;

            string s = Serialize(order);

            OrderProto clone = Deserialize(s);

            string s2 = Serialize(clone);

            Assert.AreEqual(s, s2);
        }

        private string Serialize(OrderProto proto) 
        { 
            using (MemoryStream stream = new MemoryStream()) 
            { 
                Serializer.Serialize<OrderProto>(stream, proto); 
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
            } 
        } 
        private OrderProto Deserialize(string str)
        { 
            byte[] data = Convert.FromBase64String(str);
            using (MemoryStream stream = new MemoryStream(data)) {
                return Serializer.Deserialize<OrderProto>(stream);
            }
        } 
    }

    [System.Serializable, ProtoBuf.ProtoContract(Name = @"Order")]
    public partial class OrderProto : ProtoBuf.IExtensible
    {
        public OrderProto() { }
        private string _ID0EW = "";
        [ProtoBuf.ProtoMember(1, IsRequired = false, Name =
    @"clearingid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string clearingid
        {
            get { return _ID0EW; }
            set { _ID0EW = value; }
        }
        private string _ID0E6 = "";
        [ProtoBuf.ProtoMember(2, IsRequired = false, Name =
    @"clientid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string clientid
        {
            get { return _ID0E6; }
            set { _ID0E6 = value; }
        }
        private string _ID0EIB = "";
        [ProtoBuf.ProtoMember(3, IsRequired = false, Name =
    @"ordersource", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string ordersource
        {
            get { return _ID0EIB; }
            set { _ID0EIB = value; }
        }
        private string _ID0ERB = "";
        [ProtoBuf.ProtoMember(4, IsRequired = false, Name =
    @"orderid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string orderid
        {
            get { return _ID0ERB; }
            set { _ID0ERB = value; }
        }
        private ulong _ID0E1B = default(ulong);
        [ProtoBuf.ProtoMember(5, IsRequired = false, Name =
    @"ordercreationtime", DataFormat =
    ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(ulong))]
        public ulong ordercreationtime
        {
            get { return _ID0E1B; }
            set { _ID0E1B = value; }
        }
        private string _ID0EDC = "";
        [ProtoBuf.ProtoMember(6, IsRequired = false, Name =
    @"parentorderid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string parentorderid
        {
            get { return _ID0EDC; }
            set { _ID0EDC = value; }
        }
        private string _ID0EMC = "";
        [ProtoBuf.ProtoMember(7, IsRequired = false, Name =
    @"rootorderid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string rootorderid
        {
            get { return _ID0EMC; }
            set { _ID0EMC = value; }
        }
        private string _ID0EVC = "";
        [ProtoBuf.ProtoMember(8, IsRequired = false, Name =
    @"subsystemid", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string subsystemid
        {
            get { return _ID0EVC; }
            set { _ID0EVC = value; }
        }
        private ulong _ID0E5C = default(ulong);
        [ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"symbol",
    DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(ulong))]
        public ulong symbol
        {
            get { return _ID0E5C; }
            set { _ID0E5C = value; }
        }
        private readonly
    System.Collections.Generic.List<OrderProto.OrderStateProto> _ID0EHD =
    new System.Collections.Generic.List<OrderProto.OrderStateProto>();
        [ProtoBuf.ProtoMember(1776, Name = @"orderstate", DataFormat =
    ProtoBuf.DataFormat.Default)]
        public
    System.Collections.Generic.List<OrderProto.OrderStateProto> orderstate
        {
            get { return _ID0EHD; }
        }
        [System.Serializable, ProtoBuf.ProtoContract(Name =
    @"OrderState")]
        public partial class OrderStateProto : ProtoBuf.IExtensible
        {
            public OrderStateProto() { }
            private int _ID0E6D = default(int);
            [ProtoBuf.ProtoMember(1, IsRequired = false, Name =
    @"orderversion", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int orderversion
            {
                get { return _ID0E6D; }
                set { _ID0E6D = value; }
            }
            private int _ID0EIE = default(int);
            [ProtoBuf.ProtoMember(2, IsRequired = false, Name =
    @"state", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int state
            {
                get { return _ID0EIE; }
                set { _ID0EIE = value; }
            }
            private int _ID0ERE = default(int);
            [ProtoBuf.ProtoMember(3, IsRequired = false, Name =
    @"ordertype", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int ordertype
            {
                get { return _ID0ERE; }
                set { _ID0ERE = value; }
            }
            private int _ID0E1E = default(int);
            [ProtoBuf.ProtoMember(4, IsRequired = false, Name =
    @"execat", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int execat
            {
                get { return _ID0E1E; }
                set { _ID0E1E = value; }
            }
            private uint _ID0EDF = default(uint);
            [ProtoBuf.ProtoMember(5, IsRequired = false, Name =
    @"execflags", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(uint))]
            public uint execflags
            {
                get { return _ID0EDF; }
                set { _ID0EDF = value; }
            }
            private int _ID0EMF = default(int);
            [ProtoBuf.ProtoMember(6, IsRequired = false, Name =
    @"side", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int side
            {
                get { return _ID0EMF; }
                set { _ID0EMF = value; }
            }
            private int _ID0EVF = default(int);
            [ProtoBuf.ProtoMember(7, IsRequired = false, Name =
    @"orderqty", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int orderqty
            {
                get { return _ID0EVF; }
                set { _ID0EVF = value; }
            }
            private int _ID0E5F = default(int);
            [ProtoBuf.ProtoMember(8, IsRequired = false, Name =
    @"cumqty", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int cumqty
            {
                get { return _ID0E5F; }
                set { _ID0E5F = value; }
            }
            private int _ID0EHG = default(int);
            [ProtoBuf.ProtoMember(9, IsRequired = false, Name =
    @"leavesqty", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int leavesqty
            {
                get { return _ID0EHG; }
                set { _ID0EHG = value; }
            }
            private long _ID0EQG = default(long);
            [ProtoBuf.ProtoMember(10, IsRequired = false, Name =
    @"avgpx", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long avgpx
            {
                get { return _ID0EQG; }
                set { _ID0EQG = value; }
            }
            private long _ID0EZG = default(long);
            [ProtoBuf.ProtoMember(11, IsRequired = false, Name =
    @"price", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long price
            {
                get { return _ID0EZG; }
                set { _ID0EZG = value; }
            }
            private long _ID0ECH = default(long);
            [ProtoBuf.ProtoMember(12, IsRequired = false, Name =
    @"stoppx", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long stoppx
            {
                get { return _ID0ECH; }
                set { _ID0ECH = value; }
            }
            private string _ID0ELH = "";
            [ProtoBuf.ProtoMember(13, IsRequired = false, Name =
    @"clordid", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string clordid
            {
                get { return _ID0ELH; }
                set { _ID0ELH = value; }
            }
            private uint _ID0EUH = default(uint);
            [ProtoBuf.ProtoMember(14, IsRequired = false, Name =
    @"ordstatus", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(uint))]
            public uint ordstatus
            {
                get { return _ID0EUH; }
                set { _ID0EUH = value; }
            }
            private string _ID0E4H = "";
            [ProtoBuf.ProtoMember(15, IsRequired = false, Name =
    @"exdestination", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string exdestination
            {
                get { return _ID0E4H; }
                set { _ID0E4H = value; }
            }
            private ulong _ID0EHAAC = default(ulong);
            [ProtoBuf.ProtoMember(16, IsRequired = false, Name =
    @"expirationtime", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(ulong))]
            public ulong expirationtime
            {
                get { return _ID0EHAAC; }
                set { _ID0EHAAC = value; }
            }
            private ulong _ID0EQAAC = default(ulong);
            [ProtoBuf.ProtoMember(17, IsRequired = false, Name =
    @"transactiontime", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(ulong))]
            public ulong transactiontime
            {
                get { return _ID0EQAAC; }
                set { _ID0EQAAC = value; }
            }
            private int _ID0EZAAC = default(int);
            [ProtoBuf.ProtoMember(18, IsRequired = false, Name =
    @"reasontype", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int reasontype
            {
                get { return _ID0EZAAC; }
                set { _ID0EZAAC = value; }
            }
            private string _ID0ECBAC = "";
            [ProtoBuf.ProtoMember(19, IsRequired = false, Name =
    @"reasontext", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string reasontext
            {
                get { return _ID0ECBAC; }
                set { _ID0ECBAC = value; }
            }
            private long _ID0ELBAC = default(long);
            [ProtoBuf.ProtoMember(20, IsRequired = false, Name =
    @"marketopen", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long marketopen
            {
                get { return _ID0ELBAC; }
                set { _ID0ELBAC = value; }
            }
            private long _ID0EUBAC = default(long);
            [ProtoBuf.ProtoMember(21, IsRequired = false, Name =
    @"bid", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long bid
            {
                get { return _ID0EUBAC; }
                set { _ID0EUBAC = value; }
            }
            private int _ID0E4BAC = default(int);
            [ProtoBuf.ProtoMember(22, IsRequired = false, Name =
    @"bidsize", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int bidsize
            {
                get { return _ID0E4BAC; }
                set { _ID0E4BAC = value; }
            }
            private string _ID0EGCAC = "";
            [ProtoBuf.ProtoMember(23, IsRequired = false, Name =
    @"bidmarket", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string bidmarket
            {
                get { return _ID0EGCAC; }
                set { _ID0EGCAC = value; }
            }
            private long _ID0EPCAC = default(long);
            [ProtoBuf.ProtoMember(24, IsRequired = false, Name =
    @"ask", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long ask
            {
                get { return _ID0EPCAC; }
                set { _ID0EPCAC = value; }
            }
            private int _ID0EYCAC = default(int);
            [ProtoBuf.ProtoMember(25, IsRequired = false, Name =
    @"asksize", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int asksize
            {
                get { return _ID0EYCAC; }
                set { _ID0EYCAC = value; }
            }
            private string _ID0EBDAC = "";
            [ProtoBuf.ProtoMember(26, IsRequired = false, Name =
    @"askmarket", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string askmarket
            {
                get { return _ID0EBDAC; }
                set { _ID0EBDAC = value; }
            }
            private ulong _ID0EKDAC = default(ulong);
            [ProtoBuf.ProtoMember(27, IsRequired = false, Name =
    @"nbbotime", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(ulong))]
            public ulong nbbotime
            {
                get { return _ID0EKDAC; }
                set { _ID0EKDAC = value; }
            }
            private int _ID0ETDAC = default(int);
            [ProtoBuf.ProtoMember(28, IsRequired = false, Name =
    @"exectype", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int exectype
            {
                get { return _ID0ETDAC; }
                set { _ID0ETDAC = value; }
            }
            private int _ID0E3DAC = default(int);
            [ProtoBuf.ProtoMember(29, IsRequired = false, Name =
    @"execsource", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int execsource
            {
                get { return _ID0E3DAC; }
                set { _ID0E3DAC = value; }
            }
            private string _ID0EFEAC = "";
            [ProtoBuf.ProtoMember(30, IsRequired = false, Name =
    @"execid", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string execid
            {
                get { return _ID0EFEAC; }
                set { _ID0EFEAC = value; }
            }
            private long _ID0EOEAC = default(long);
            [ProtoBuf.ProtoMember(31, IsRequired = false, Name =
    @"lastpx", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(long))]
            public long lastpx
            {
                get { return _ID0EOEAC; }
                set { _ID0EOEAC = value; }
            }
            private int _ID0EXEAC = default(int);
            [ProtoBuf.ProtoMember(32, IsRequired = false, Name =
    @"lastqty", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(int))]
            public int lastqty
            {
                get { return _ID0EXEAC; }
                set { _ID0EXEAC = value; }
            }
            private string _ID0EAFAC = "";
            [ProtoBuf.ProtoMember(33, IsRequired = false, Name =
    @"execrefid", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string execrefid
            {
                get { return _ID0EAFAC; }
                set { _ID0EAFAC = value; }
            }
            private string _ID0EJFAC = "";
            [ProtoBuf.ProtoMember(34, IsRequired = false, Name =
    @"liquidityflag", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string liquidityflag
            {
                get { return _ID0EJFAC; }
                set { _ID0EJFAC = value; }
            }
            private string _ID0ESFAC = "";
            [ProtoBuf.ProtoMember(35, IsRequired = false, Name =
    @"fixpropbag", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string fixpropbag
            {
                get { return _ID0ESFAC; }
                set { _ID0ESFAC = value; }
            }
            private uint _ID0E2FAC = default(uint);
            [ProtoBuf.ProtoMember(36, IsRequired = false, Name =
    @"ordercapacity", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(uint))]
            public uint ordercapacity
            {
                get { return _ID0E2FAC; }
                set { _ID0E2FAC = value; }
            }
            private uint _ID0EEGAC = default(uint);
            [ProtoBuf.ProtoMember(37, IsRequired = false, Name =
    @"lastcapacity", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
            [System.ComponentModel.DefaultValue(default(uint))]
            public uint lastcapacity
            {
                get { return _ID0EEGAC; }
                set { _ID0EEGAC = value; }
            }
            private string _ID0ENGAC = "";
            [ProtoBuf.ProtoMember(38, IsRequired = false, Name =
    @"traderid", DataFormat = ProtoBuf.DataFormat.Default)]
            [System.ComponentModel.DefaultValue("")]
            public string traderid
            {
                get { return _ID0ENGAC; }
                set { _ID0ENGAC = value; }
            }
            private ProtoBuf.IExtension extensionObject;
            ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject
    (bool createIfMissing)
            {
                return ProtoBuf.Extensible.GetExtensionObject(ref 
extensionObject, createIfMissing);
            }
        }
        private ProtoBuf.IExtension extensionObject;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject
    (bool createIfMissing)
        {
            return ProtoBuf.Extensible.GetExtensionObject(ref 
extensionObject, createIfMissing);
        }
    }
}
