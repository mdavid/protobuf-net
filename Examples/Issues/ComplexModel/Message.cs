using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
using NUnit.Framework;

namespace PBTestClasses
{
    [TestFixture]
    public class TestMessageIssue
    {
        [Test]
        public void TestRoundtrip()
        {

            Message msg = new Message60 { Id = Guid.NewGuid() };
            Message clone = Serializer.DeepClone(msg);
            Assert.IsNotNull(clone);
            Assert.AreNotEqual(msg, clone);
            Assert.IsInstanceOfType(typeof(Message60), clone);
            Assert.AreEqual(msg.Id, clone.Id);
        }
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(Message1))]
    [ProtoInclude(2, typeof(Message2))]
    [ProtoInclude(3, typeof(Message3))]
    [ProtoInclude(4, typeof(Message4))]
    [ProtoInclude(5, typeof(Message5))]
    [ProtoInclude(6, typeof(Message6))]
    [ProtoInclude(7, typeof(Message7))]
    [ProtoInclude(8, typeof(Message8))]
    [ProtoInclude(9, typeof(Message9))]
    [ProtoInclude(10, typeof(Message10))]
    [ProtoInclude(11, typeof(Message11))]
    [ProtoInclude(12, typeof(Message12))]
    [ProtoInclude(13, typeof(Message13))]
    [ProtoInclude(14, typeof(Message14))]
    [ProtoInclude(15, typeof(Message15))]
    [ProtoInclude(16, typeof(Message16))]
    [ProtoInclude(17, typeof(Message17))]
    [ProtoInclude(18, typeof(Message18))]
    [ProtoInclude(19, typeof(Message19))]
    [ProtoInclude(20, typeof(Message20))]
    [ProtoInclude(21, typeof(Message21))]
    [ProtoInclude(22, typeof(Message22))]
    [ProtoInclude(23, typeof(Message23))]
    [ProtoInclude(24, typeof(Message24))]
    [ProtoInclude(25, typeof(Message25))]
    [ProtoInclude(26, typeof(Message26))]
    [ProtoInclude(27, typeof(Message27))]
    [ProtoInclude(28, typeof(Message28))]
    [ProtoInclude(29, typeof(Message29))]
    [ProtoInclude(30, typeof(Message30))]
    [ProtoInclude(31, typeof(Message31))]
    [ProtoInclude(32, typeof(Message32))]
    [ProtoInclude(33, typeof(Message33))]
    [ProtoInclude(34, typeof(Message34))]
    [ProtoInclude(35, typeof(Message35))]
    [ProtoInclude(36, typeof(Message36))]
    [ProtoInclude(37, typeof(Message37))]
    [ProtoInclude(38, typeof(Message38))]
    [ProtoInclude(39, typeof(Message39))]
    [ProtoInclude(40, typeof(Message40))]
    [ProtoInclude(41, typeof(Message41))]
    [ProtoInclude(42, typeof(Message42))]
    [ProtoInclude(43, typeof(Message43))]
    [ProtoInclude(44, typeof(Message44))]
    [ProtoInclude(45, typeof(Message45))]
    [ProtoInclude(46, typeof(Message46))]
    [ProtoInclude(47, typeof(Message47))]
    [ProtoInclude(48, typeof(Message48))]
    [ProtoInclude(49, typeof(Message49))]
    [ProtoInclude(50, typeof(Message50))]
    [ProtoInclude(51, typeof(Message51))]
    [ProtoInclude(52, typeof(Message52))]
    [ProtoInclude(53, typeof(Message53))]
    [ProtoInclude(54, typeof(Message54))]
    [ProtoInclude(55, typeof(Message55))]
    [ProtoInclude(56, typeof(Message56))]
    [ProtoInclude(57, typeof(Message57))]
    [ProtoInclude(58, typeof(Message58))]
    [ProtoInclude(59, typeof(Message59))]
    [ProtoInclude(60, typeof(Message60))]
    /*
    */ 
    public abstract class Message
    {
        public Message()
        {
        }

        private Guid m_Id;
        [ProtoMember(1001)]
        public Guid Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private DateTime m_Time;
        [ProtoMember(1002)]
        public DateTime Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        private string m_Source;
        [ProtoMember(1003)]
        public string Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }
    }
}
