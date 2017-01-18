using System;
using System.Collections.Generic;
using System.Text;

namespace HTS.SAS.Entities
{
   public class CIMBclicksEn
    {
        private Int32 enFile_Id;
        private String enFile_Name;
        private Double  enTotal_Amount;
        private String enTotal_Trans;
        private DateTime enUpload_Date;
        private String enBank_Code;
        private String enHeader_No;
        private String enBatchCode;
        private String enPost_Status;

        public Int32 File_Id
        {
            get { return enFile_Id; }
            set { enFile_Id = value; }
        }

        public String File_Name
        {
            get { return enFile_Name; }
            set { enFile_Name = value; }
        }

        public Double Total_Amount
        {
            get { return enTotal_Amount; }
            set { enTotal_Amount = value; }
        }

        public String Total_Trans
        {
            get { return enTotal_Trans; }
            set { enTotal_Trans = value; }
        }

        public DateTime Upload_Date
        {
            get { return enUpload_Date; }
            set { enUpload_Date = value; }
        }

        public String Bank_Code
        {
            get { return enBank_Code; }
            set { enBank_Code = value; }
        }

        public String Header_No
        {
            get { return enHeader_No; }
            set { enHeader_No = value; }
        }

        public String BatchCode
        {
            get { return enBatchCode; }
            set { enBatchCode = value; }
        }

        public String Post_Status
        {
            get { return enPost_Status; }
            set { enPost_Status = value; }
        }
    }
}
