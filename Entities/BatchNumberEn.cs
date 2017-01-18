using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace HTS.SAS.Entities
{
    [System.SerializableAttribute()]
    //[DataContract]
    public class BatchNumberEn
    {
        private int enBN_Id;
        private string enBN_Process;
        private string enBN_Prefix;
        private int enBN_NoDigit;
        private int enBN_StartNo;
        private int enBN_CurNo;
        private string enBN_AutoNo;

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public int BN_Id
        {
            get { return enBN_Id; }
            set { enBN_Id = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public string BN_Process
        {
            get { return enBN_Process; }
            set { enBN_Process = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public string BN_Prefix
        {
            get { return enBN_Prefix; }
            set { enBN_Prefix = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public int BN_NoDigit
        {
            get { return enBN_NoDigit; }
            set { enBN_NoDigit = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public int BN_StartNo
        {
            get { return enBN_StartNo; }
            set { enBN_StartNo = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public int BN_CurNo
        {
            get { return enBN_CurNo; }
            set { enBN_CurNo = value; }
        }

        [System.Xml.Serialization.XmlElement]
        ////[DataMember]
        public string BN_AutoNo
        {
            get { return enBN_AutoNo; }
            set { enBN_AutoNo = value; }
        }
    }
}


