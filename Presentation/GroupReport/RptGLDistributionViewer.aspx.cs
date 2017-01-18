using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;

public partial class RptGLDistributionViewer : MenuAccess
{
    private ReportHelper reportHelper = new ReportHelper();
    private ReportDocument reportDocument = new ReportDocument();

    protected void Page_Init(object sender, EventArgs e)
    {
        IEnumerable<string> allChecked;
        StringBuilder sbFilter = new StringBuilder();

        if (!Page.IsPostBack)
        {
            sbFilter.AppendFormat("SELECT '{0} - {1}' dtFilter, SAS_Accounts.*, SAS_CF_DoubleEntry.* FROM SAS_CF_DoubleEntry INNER JOIN ", this.DateFrom, this.DateTo);
            sbFilter.Append("(SELECT DISTINCT 0 as transid, '' as transtempcode,'' as transcode, batchcode, description, transdate, category, subtype  FROM SAS_Accounts) SAS_Accounts ");
            sbFilter.Append("ON SAS_Accounts.batchcode = SAS_CF_DoubleEntry.reference_no WHERE 1=1");

            if (!string.IsNullOrWhiteSpace(this.DateFrom) && this.DateFrom != "0" &&
                !string.IsNullOrWhiteSpace(this.DateTo) && this.DateTo != "0")
            {
                sbFilter.AppendFormat(" AND SAS_Accounts.transdate BETWEEN '{0}' AND '{1}'", this.DateFromCondition, this.DateToCondition);
            }

            if (Session["transtype"] != null)
            {
                allChecked = Session["transtype"] as IEnumerable<string>;

                using (var transTypeEnum = allChecked.GetEnumerator())
                {
                    sbFilter.Append(" AND (");

                    while (transTypeEnum.MoveNext())
                    {
                        Enumerations.TransactionType transType = (Enumerations.TransactionType)Enum.Parse(typeof(Enumerations.TransactionType), transTypeEnum.Current);

                        switch (transType)
                        {
                            case Enumerations.TransactionType.AFC:
                            case Enumerations.TransactionType.CreditNoteStu:
                            case Enumerations.TransactionType.AllocationSpo:
                            case Enumerations.TransactionType.InvoiceStu:
                            case Enumerations.TransactionType.ReceiptStu:
                            case Enumerations.TransactionType.DebitNoteSpo:
                            case Enumerations.TransactionType.DebitNoteStu:
                            case Enumerations.TransactionType.ReceiptSpo:
                            case Enumerations.TransactionType.CreditNoteSpo:
                            case Enumerations.TransactionType.PaymentStu:
                                sbFilter.Append(this.ConstructFilters(transType));
                                break;
                            default:
                                break;
                        }
                    }

                    sbFilter.Remove(sbFilter.Length - 2, 2);
                    sbFilter.Append(")");
                }
            }

            sbFilter.AppendFormat(" ORDER BY SAS_Accounts.transdate");

            DataSet dataSet = reportHelper.GetDataSet(sbFilter.ToString());
            dataSet.Tables[0].TableName = "SAS_Accounts";

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                Response.Write("No Record Found");
            }
            else
            {
                //Report Loading
                reportDocument.Load(Server.MapPath("~/GroupReport/RptGLDistribution.rpt"));
                reportDocument.SetDataSource(dataSet);
                this.ReportDocument = reportDocument;
                CrystalReportViewer1.ReportSource = reportDocument;
                CrystalReportViewer1.DataBind();
                reportDocument.Refresh();

                //Report Ended
            }
        }
        else
        {
            reportDocument = this.ReportDocument;
            CrystalReportViewer1.ReportSource = reportDocument;
            CrystalReportViewer1.DataBind();
            reportDocument.Refresh();
        }
    }

    protected string ConstructFilters(Enumerations.TransactionType transType)
    {
        StringBuilder sbFilter = new StringBuilder();

        var transMultiTypeArr = Enumerations.GetEnumDescription(transType).Split(new Char[] { '|', '\n' },
         StringSplitOptions.RemoveEmptyEntries);


        foreach (var multiTransType in transMultiTypeArr)
        {
            var transTypeArr = multiTransType.Split(new Char[] { ':', '\n' },
             StringSplitOptions.RemoveEmptyEntries);

            sbFilter.Append(this.BuildCategoryNSubTypeQuery(transTypeArr));
        }

        return sbFilter.ToString();
    }

    protected string BuildCategoryNSubTypeQuery(string[] transTypeArr)
    {
        StringBuilder sbFilter = new StringBuilder();

        if (transTypeArr.Count() == 1)
        {
            sbFilter.AppendFormat(" SAS_Accounts.category = '{0}' OR", transTypeArr[0]);
        }

        if (transTypeArr.Count() == 2)
        {
            sbFilter.AppendFormat(" (SAS_Accounts.category = '{0}'", transTypeArr[0]);
            sbFilter.AppendFormat(" AND SAS_Accounts.subtype = '{0}') OR", transTypeArr[1]);
        }

        return sbFilter.ToString();
    }
}