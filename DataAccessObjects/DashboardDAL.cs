#region NameSpaces 

using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Common;
using HTS.SAS.Entities;
using MaxGeneric;

#endregion

namespace HTS.SAS.DataAccessObjects
{
    public class DashboardDAL
    {
        #region Global Declarations 

        private DbParameterCollection _DbParameterCollection = null;

        private MaxModule.DatabaseProvider _DatabaseFactory =
            new MaxModule.DatabaseProvider();

        private string DataBaseConnectionString = Helper.
           GetConnectionString();

        #endregion

        #region Get Dashboard 

        public enum DashboardType
        {
            Student_Outstanding_Invoice = 1,
            Collection_By_Semester = 2,
            Net_Invoices_Against_Payment = 3,
            Sponsor_Ageing_Monthly = 4,
            Student_Ageing_By_Faculty = 5
        }

        public DataSet GetData(DashboardType dashboardType)
        {
            DataSet dt = new DataSet();

            string strStoredProc = GetStoredProcedureName(dashboardType);

            if (!string.IsNullOrEmpty(strStoredProc))
            {
                dt = _DatabaseFactory.ExecuteDataSet(Helper.GetDataBaseType,
                    DataBaseConnectionString, strStoredProc, true);
            }
            return dt;
        }

        public string GetStoredProcedureName(DashboardType dashboardType)
        {
            switch (dashboardType)
            {
                case DashboardType.Collection_By_Semester:
                    return "DB_GetCollectionBySemester";
                
                case DashboardType.Net_Invoices_Against_Payment:
                    return "DB_GetNetInvoicesAgainstPayment";

                case DashboardType.Sponsor_Ageing_Monthly:
                    return "DB_GetSponsorAgeingMonthly";

                case DashboardType.Student_Ageing_By_Faculty:
                    return "DB_GetStudentAgeingByFaculty";

                case DashboardType.Student_Outstanding_Invoice:
                    return "DB_GetStudentOutstandingInvoice";

                default:
                    return string.Empty;
            }
        }

        #endregion

        #region Dashboard V2

        public List<AccountsEn> GetDashboardV2Data()
        {
            string sqlCmd = String.Empty;
            List<AccountsEn> loEnList = new List<AccountsEn>();

            sqlCmd = "SELECT ledger.Category,SUM(ledger.Debit) AS Outstanding_Amt FROM ( ";

            sqlCmd += @"(SELECT acc.PostedTimestamp, 
                            CASE WHEN acc.Category = 'Receipt' THEN
                                 CASE WHEN acc.SourceType = 'FER' THEN acc.batchdate ELSE
	                                ( SELECT date_time FROM SAS_workflow_status WHERE workflow_id = 
		                              (SELECT workflow_id FROM SAS_workflow
		                              WHERE batch_code=acc.BatchCode)
	                                  AND workflow_status=2
	                                  ORDER by date_time DESC
	                                  LIMIT 1 )
                                 END
	                        ELSE acc.TransDate
	                        END TransDate,
                            CASE WHEN acc.Category = 'Receipt' THEN 
                                      CASE WHEN acc.SourceType = 'FER' THEN acc.TransCode 
                                      ELSE 
                                           CASE WHEN acc.Description LIKE 'CIMB CLICKS%' THEN acc.TransCode 
	                                       ELSE 
                                                CASE WHEN acc.Subcategory='Loan' THEN acc.TransCode 
                                                ELSE acc.BankRecNo END
                                           END
                                      END 
			                     ELSE
				                    CASE WHEN Category = 'Loan' THEN BatchCode
				                    ELSE acc.TransCode
				                    END
			                END TransCode,                            
                        CASE WHEN acc.Category = 'SPA' THEN
                                
	                                ( SELECT Description FROM sas_accounts WHERE batchcode = acc.BatchCode and category = 'Allocation' and subtype = 'Sponsor'
		                              )
	                        ELSE acc.Description
	                        END Description, acc.Category ,
                            CASE WHEN acc.category = 'Credit Note' or acc.category = 'Debit Note' or acc.category = 'Invoice' THEN
                                CASE
                                    WHEN acc.transtype = 'Debit' then SUM(de.TransAmount)
                                    ELSE 0
                                END
                            ELSE 
                                CASE 
                                    WHEN acc.TransType = 'Debit' THEN acc.TransAmount
                                    ELSE 0
                                END 
                            END Debit,
                            CASE WHEN acc.category = 'Credit Note' or acc.category = 'Debit Note' or acc.category = 'Invoice' THEN 
                                CASE 
                                    WHEN acc.transtype = 'Credit' Then SUM(de.transamount)
                                    ELSE 0
                                END
                            ELSE     
                                CASE 
                                    WHEN acc.TransType = 'Credit' THEN acc.TransAmount
                                    ELSE 0
                                END
                            END Credit,
                            CASE WHEN acc.category = 'Credit Note' or acc.category = 'Debit Note' or acc.category = 'Invoice' THEN 
                                SUM(de.TransAmount)
                            ELSE
                                acc.TransAmount
                            END TransAmount,    
                            CASE WHEN TransType = 'Credit' THEN 
			                     CASE WHEN Category = 'Loan' THEN 'Debit'
			                     ELSE 'Credit'
			                     END
			                ELSE 'Debit'	
			                END TransType,
                        acc.BatchCode
                    FROM SAS_Accounts acc
                    left join sas_accountsdetails de on acc.transid = de.transid
                    WHERE acc.SubType = 'Student' AND acc.PostStatus ='Posted' ";

            sqlCmd += @"AND acc.Category NOT IN ('Loan') AND acc.TransAmount > 0 
                        GROUP BY acc.TransDate,  acc.ReceiptDate, acc.BankRecNo, acc.Description, acc.Category, acc.Subcategory, acc.TransCode, acc.TransType, acc.BatchCode, 
                        acc.TransAmount, acc.postedtimestamp,acc.SourceType,acc.BatchDate) ";

            sqlCmd += "UNION ";

            sqlCmd += @"(SELECT SAS_StudentLoan.PostedTimestamp, TransDate, TransCode, Description, Category,
                            --debit
			                CASE WHEN Category = 'Receipt' THEN 
			                     CASE WHEN TransType = 'Debit' THEN TransAmount
			                     ELSE 0
			                     END
			                ELSE
			                     CASE WHEN TransType = 'Credit' THEN TransAmount
			                     ELSE 0
			                     END
			                END Debit,
			                --credit
			                CASE WHEN Category = 'Receipt' THEN 
			                     CASE WHEN TransType = 'Credit' THEN TransAmount
			                     ELSE 0
			                     END
			                ELSE
			                     CASE WHEN TransType = 'Debit' THEN TransAmount
			                     ELSE 0
			                     END
			                END Credit,
                        TransAmount, 
                            CASE WHEN TransType = 'Credit' THEN 
			                        CASE WHEN Category = 'Loan' THEN 'Debit'
			                        ELSE 'Credit'	
			                        END
			                ELSE 'Debit'
			                END TransType,
                        BatchCode
                    FROM SAS_StudentLoan
                    WHERE SubType = 'Student' AND PostStatus ='Posted' AND Category NOT IN ('Receipt') ) ";

            sqlCmd += ") ledger WHERE ledger.TransType <> 'Credit' GROUP BY ledger.Category";

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                                   DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            AccountsEn loItem = new AccountsEn();
                            loItem.Category = GetValue<string>(loReader, "Category");
                            loItem.TransactionAmount = GetValue<double>(loReader, "Outstanding_Amt");
                            loEnList.Add(loItem);
                        }
                        loReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return loEnList;
        }

        #endregion

        private static T GetValue<T>(IDataReader argReader, string argColNm)
        {
            if (!argReader.IsDBNull(argReader.GetOrdinal(argColNm)))
                return (T)argReader.GetValue(argReader.GetOrdinal(argColNm));
            else
                return default(T);
        }

    }
}
