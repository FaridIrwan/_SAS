using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace HTS.SAS.Entities
{
    [System.SerializableAttribute()]
    //[DataContract]
    public class SagaEn
    {
        //sas_saga - start
        private int csAuto_No;
        private string csAuto_Prefix;
        private int csPosting_Type;
        private int csAuto_Length;
        //sas_saga - end

        //sas_saga_posting - start
        private int csPosting_Id;
        private string csBatch_Code;
        private string csReference_No;
        //private int csPosting_Type;
        private DateTime csPosting_Date;
        //sas_saga_posting - end

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public int Auto_No
        {
            get { return csAuto_No; }
            set { csAuto_No = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public string Auto_Prefix
        {
            get { return csAuto_Prefix; }
            set { csAuto_Prefix = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public int Posting_Type
        {
            get { return csPosting_Type; }
            set { csPosting_Type = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public int Auto_Length
        {
            get { return csAuto_Length; }
            set { csAuto_Length = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public int Posting_Id
        {
            get { return csPosting_Id; }
            set { csPosting_Id = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public string Batch_Code
        {
            get { return csBatch_Code; }
            set { csBatch_Code = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public string Reference_No
        {
            get { return csReference_No; }
            set { csReference_No = value; }
        }

        [System.Xml.Serialization.XmlElement]
        //[DataMember]
        public DateTime Posting_Date
        {
            get { return csPosting_Date; }
            set { csPosting_Date = value; }
        }

    }
}
