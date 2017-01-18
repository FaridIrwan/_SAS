using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Enumerations
    {
        public enum TransactionType
        {
            [Description("AFC")]
            AFC,
            [Description("Invoice:Student")]
            InvoiceStu,
            [Description("Debit Note:Student")]
            DebitNoteStu,
            [Description("Debit Note:Sponsor")]
            DebitNoteSpo,
            [Description("Credit Note:Student")]
            CreditNoteStu,
            [Description("Credit Note:Sponsor")]
            CreditNoteSpo,
            [Description("Receipt:Student")]
            ReceiptStu,
            [Description("Receipt:Sponsor")]
            ReceiptSpo,
            [Description("Student Payment")]
            AllocationStu,
            [Description("Allocation:Sponsor")]
            AllocationSpo,
            [Description("Refund:Student|Payment:Student")]
            PaymentStu,
            [Description("Loan:Student")]
            LoanStu
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
