using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace PBTestClasses
{
    [ProtoContract]
    public class Message12 : Message
    {
        public Message12()
        {
        }

        private Guid m_OrderId;

        [ProtoMember(1001)]
        public Guid OrderId
        {
            get { return m_OrderId; }
            set { m_OrderId = value; }
        }

        private Guid m_ClientId;

        [ProtoMember(1002)]
        public Guid ClientId
        {
            get { return m_ClientId; }
            set { m_ClientId = value; }
        }

        private int m_Station;

        [ProtoMember(1003)]
        public int Station
        {
            get { return m_Station; }
            set { m_Station = value; }
        }

        private string m_Name;

        [ProtoMember(1004)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
    }
}
