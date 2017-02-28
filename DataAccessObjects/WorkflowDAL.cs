#region NameSpaces

using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Common;
using HTS.SAS.Entities;
using MaxGeneric;
using System.Linq;

#endregion

namespace HTS.SAS.DataAccessObjects
{
    public class WorkflowDAL
    {
        #region Global Declarations

        private DbParameterCollection _DbParameterCollection = null;

        private MaxModule.DatabaseProvider _DatabaseFactory =
            new MaxModule.DatabaseProvider();

        private string DataBaseConnectionString = Helper.
           GetConnectionString();

        public MaxModule.CfCommon _CfCommon = new MaxModule.CfCommon();

        #endregion

        #region Insert / Update Workflow

        /// <summary>
        /// Method to Insert / Update
        /// </summary>
        /// <param name="argEn">GST SetUp is an Input.   </param>
        /// <returns>Returns Boolean</returns>
        public bool Workflow(string BatchCode, string UserId, string PageName)
        {
            //variable declarations
            string SqlStatement = null; int WorkflowId = 0; int Result = 0;
            string SqlCount = null; int HasRows = 0; bool AFCResult = false;

            //Validate if same batchcode already posted to workflow -Start
            try
            {
                //Build Sql Statement - Start
                SqlCount = "SELECT count(*) as rows FROM sas_workflow WHERE workflow_status <> 3 AND batch_code = ";
                SqlCount += clsGeneric.AddQuotes(BatchCode);
                //Build Sql Statement - Stop

                //Get Program having BidangCode selected - Start
                IDataReader _IDataReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlCount).CreateDataReader();
                //Get Program having BidangCode selected - Stop

                //if details available - Start
                if (_IDataReader != null)
                {
                    if (_IDataReader.Read())
                    {
                        HasRows = clsGeneric.NullToInteger(_IDataReader["rows"]);
                        if (HasRows > 0)
                            throw new Exception("Record Already Posted");
                    }

                    _IDataReader.Close();
                    _IDataReader.Dispose();

                    //if record not used - Start
                    if (HasRows == 0)
                    { 
                        //Build sql statment - Start
                        SqlStatement = "INSERT INTO sas_workflow(batch_code, date_time, workflow_status, user_id,page_name)";
                        SqlStatement += " VALUES (" + clsGeneric.AddQuotes(BatchCode);
                        SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
                        SqlStatement += clsGeneric.AddComma() + (short)(Helper.WorkflowStatus.Received);
                        SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(UserId);                
                        SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(PageName);
                        SqlStatement += "); SELECT MAX(workflow_id) from sas_workflow;";
                        //Build sql statment - Stop

                        if (!FormHelp.IsBlank(SqlStatement))
                        {
                            //track workflow details - Start
                            WorkflowId = clsGeneric.NullToInteger(_DatabaseFactory.ExecuteScalar(
                                Helper.GetDataBaseType, DataBaseConnectionString, SqlStatement));
                            //track workflow details - Stop

                            //Build Sql Statement - Start
                            SqlStatement = "INSERT INTO sas_workflow_status(workflow_id, workflow_status, date_time,workflow_role, user_name) ";
                            SqlStatement += "VALUES ("+ WorkflowId;
                            SqlStatement += clsGeneric.AddComma() + (short)(Helper.WorkflowStatus.Received);
                            SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));                    
                            SqlStatement += ", 'P', " + clsGeneric.AddQuotes(UserId) + " )";
                            //Build Sql Statement - Stop

                            //track workdlow status - start
                            Result = _DatabaseFactory.ExecuteSqlStatement(Helper.GetDataBaseType,
                                DataBaseConnectionString, SqlStatement);
                            //track workdlow status - stop
                        }

                        if (Result > -1)
                        {   
                            //return true;
                            AFCResult = true;
                        }
                    }
                    //AFCResult = false;
                }
                return AFCResult;
            }
           
            //Validate if same batchcode already posted to workflow -End
            
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);
                return false;
            }
        }

        #endregion

        #region Update WorkFlow for Approve

        public int UpdateWorkflow(int WorkflowId, short WorkflowStatus, string Remark, string WorkFlowRole, string UserId)
        {
            //variable declarations
            string SqlStatement = null; 

            try
            {

                //Build sql statment - Start
                SqlStatement = "Update sas_workflow Set workflow_status=";
                SqlStatement += WorkflowStatus + " " + clsGeneric.AddComma();
                SqlStatement += " workflow_remarks= ";
                SqlStatement += clsGeneric.AddQuotes(Remark);
                SqlStatement += " " + clsGeneric.AddComma();
                SqlStatement += " date_time=";
                SqlStatement += clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
                SqlStatement += " where workflow_id=" + WorkflowId;
                //Build sql statment - Stop

                if (WorkflowStatus == 2)
                {
                    //Build Sql Statement - Start
                    SqlStatement += "; UPDATE sas_workflow_status SET workflow_status = " + WorkflowStatus;
                    SqlStatement += " WHERE workflow_role = " + clsGeneric.AddQuotes(WorkFlowRole);
                    SqlStatement += " AND user_name = " + clsGeneric.AddQuotes(UserId);                     
                    SqlStatement += " AND workflow_id = " + WorkflowId;  
                    //Build Sql Statement - Stop
                }

                if (WorkflowStatus == 3)
                {
                    //Build Sql Statement - Start
                    SqlStatement += "; INSERT INTO sas_workflow_status(workflow_id, workflow_status, date_time, workflow_role, user_name) VALUES (";                
                    SqlStatement += WorkflowId;
                    SqlStatement += clsGeneric.AddComma() + WorkflowStatus;
                    SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
                    SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(WorkFlowRole);
                    SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(UserId);
                    SqlStatement += ")";
                    //Build Sql Statement - Stop
                }

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //track workdflow status - start
                    return _DatabaseFactory.ExecuteSqlStatement(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //track workdflow status - stop
                }

                return 1;
            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);
                return -1;
            }
        }
        #endregion

        #region Update WorkFlow for Reject

        //public int UpdateWorkflowReject(int WorkflowId, short WorkflowStatus, string Remark)
        //{
        //    //variable declarations
        //    string SqlStatement = null;

        //    try
        //    {

        //        //Build sql statment - Start
        //        SqlStatement = "Update sas_workflow Set workflow_status=";
        //        SqlStatement += WorkflowStatus + " " + clsGeneric.AddComma();
        //        SqlStatement += " workflow_remarks= ";
        //        SqlStatement += clsGeneric.AddQuotes(Remark);
        //        SqlStatement +=  " " + clsGeneric.AddComma();
        //        SqlStatement += " date_time=";
        //        SqlStatement += clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
        //        SqlStatement += " where workflow_id=" + WorkflowId;
        //        //Build sql statment - Stop

        //        //Build Sql Statement - Start
        //        SqlStatement += "; INSERT INTO sas_workflow_status(workflow_id, workflow_status, date_time) VALUES (";
        //        //SqlStatement += clsGeneric.AddComma() + WorkflowId;
        //        SqlStatement += WorkflowId;
        //        SqlStatement += clsGeneric.AddComma() + WorkflowStatus;
        //        SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
        //        SqlStatement += ")";
        //        //Build Sql Statement - Stop

        //        if (!FormHelp.IsBlank(SqlStatement))
        //        {
        //            //track workdflow status - start
        //            return _DatabaseFactory.ExecuteSqlStatement(Helper.GetDataBaseType,
        //                DataBaseConnectionString, SqlStatement);
        //            //track workdflow status - stop
        //        }

        //        return -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        MaxModule.Helper.LogError(ex.Message);
        //        return -1;
        //    }
        //}
        #endregion             

        #region Get WorkFlow SASAcount Details

        public DataSet WorkFlowSASAcountDetails()
        {
            //variable declarations
            string SqlStatement = null;

            try
            {
                //Build sql statment - Start  
                //SqlStatement = "select distinct wf.workflow_id,a.category,a.batchcode";
                //SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') ";
                //SqlStatement += "THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode)) ";
                //SqlStatement += "WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) ";
                //SqlStatement += "ELSE SUM(a.transamount) END as transamount";
                //SqlStatement += ", a.postedby as createdby, a.batchdate, CASE WHEN a.description = '' THEN 'Description Details' ELSE a.description END as description, wf.page_name, a.subtype";
                //SqlStatement += " from sas_accounts a,sas_workflow wf where a.batchcode=wf.batch_code and wf.workflow_status = 1 and a.category NOT IN ('SPA','STA')";
                //SqlStatement += " group by wf.workflow_id, a.batchcode, a.category,a.postedby,a.batchdate,a.description,wf.page_name,a.subtype";
                //SqlStatement += " order by wf.workflow_id desc";
                SqlStatement = "select distinct wf.workflow_id,a.category,a.batchcode";
                SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') AND subtype = 'Student' ";
                SqlStatement += "THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode)) ";
                SqlStatement += "WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) ";
                SqlStatement += "WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN ";
                SqlStatement += "(select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "WHEN Category IN ('AFC') THEN ";
                SqlStatement += "(select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "ELSE SUM(a.transamount) END as transamount";
                SqlStatement += ", wf.user_id as createdby, ";
                SqlStatement += "CASE WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN ";
                SqlStatement += "date_trunc('day',a.batchdate) ELSE a.batchdate	END as batchdate, ";
                SqlStatement += "CASE WHEN Category IN ('AFC') THEN (select batchcode from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "ELSE a.description END as description, ";
                SqlStatement += "wf.page_name, a.subtype ";
                SqlStatement += " from sas_accounts a,sas_workflow wf where a.batchcode=wf.batch_code and wf.workflow_status = 1 and a.category NOT IN ('SPA','STA')";
                SqlStatement += " group by wf.workflow_id, a.batchcode, a.category,wf.user_id,a.batchdate,a.description,wf.page_name,a.subtype";
                SqlStatement += " UNION select distinct wf.workflow_id,b.category,b.batchcode";
                SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note')";
                SqlStatement += " THEN (select sum(transamount) from sas_sponsorinvoicedetails where transid in (select transid from sas_sponsorinvoice where batchcode = b.batchcode))";
                SqlStatement += " ELSE SUM(b.transamount) END as transamount, wf.user_id as createdby, b.batchdate, b.description, wf.page_name, b.subtype";
                SqlStatement += " FROM sas_sponsorinvoice b,sas_workflow wf where b.batchcode=wf.batch_code and wf.workflow_status = 1 and b.category NOT IN ('SPA','STA')";
                SqlStatement += " group by wf.workflow_id, b.batchcode, b.category,wf.user_id,b.batchdate,b.description,wf.page_name,b.subtype";
                SqlStatement += " order by workflow_id desc";
                //Build sql statment - Stop

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //Binding Sas Account Details - start
                    DataSet _DataSet = _DatabaseFactory.ExecuteDataSet(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //Sas Account status - Ended
                    return _DataSet;
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }
            return null;
        }

        #endregion

        #region Get WorkFlow SASAcount Student Details 

        public DataSet WorkFlowSASAcountStudentDetails(string TransId, string Type, string SubType, string MatricNo)
        {
            //variable declarations
            string SqlStatement = null;

            try
            {
                if (SubType == "Student")
                {
                    if (Type == "Invoice" || Type == "Credit Note" || Type == "Debit Note")
                    {
                        SqlStatement = "SELECT ST.SASI_MatricNo,ST.SASI_Name,sum(ACD.transamount) as transamount,'' as SASR_Code,'' as SASR_Name,'' as sasi_cursem, ";
                        SqlStatement += "'' as FeeCode,'' as FeeDesc FROM SAS_Accounts AC LEFT JOIN SAS_AccountsDetails ACD ON AC.TransID=ACD.TransID ";
                        SqlStatement += "LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo where AC.batchcode = " + clsGeneric.AddQuotes(TransId);
                        SqlStatement += " GROUP BY ST.SASI_MatricNo,ST.SASI_Name";
                    }
                    if (Type == "AFC")
                    {
                        //Build sql statment - Start
                        SqlStatement = "SELECT ST.SASI_MatricNo,ST.SASI_Name,AC.transamount,'' as SASR_Code,'' as SASR_Name,ST.sasi_cursem,'' as FeeCode,'' as FeeDesc FROM SAS_AFC AF";
                        SqlStatement += " LEFT JOIN SAS_AFCDetails AFD ON AF.TransCode=AFD.TransCode";
                        SqlStatement += " LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode";
                        SqlStatement += " LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo where AC.batchcode = " + clsGeneric.AddQuotes(TransId);
                        SqlStatement += " ORDER BY SASI_MatricNo ";
                        //Build sql statment - Stop
                    }
                    else
                    {
                        SqlStatement = "SELECT ST.SASI_MatricNo,ST.SASI_Name,sum(AC.transamount) as transamount,'' as SASR_Code,'' as SASR_Name,'' as sasi_cursem, ";
                        SqlStatement += "'' as FeeCode,'' as FeeDesc FROM SAS_Accounts AC LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo where AC.batchcode = " + clsGeneric.AddQuotes(TransId);
                        
                        //added by Hafiz @ 31/3/2016
                        if (MatricNo.Length != 0) SqlStatement += " AND ST.SASI_MatricNo ='" + MatricNo + "'";
                                                
                        SqlStatement += " GROUP BY ST.SASI_MatricNo,ST.SASI_Name";
                    }
                }                

                else if (SubType == "Sponsor")
                {
                    if (Type == "Credit Note" || Type == "Debit Note")
                    {
                        SqlStatement = "SELECT SP.SASR_Code,SP.SASR_Name,sum(AC.tempamount) as transamount,'' as SASI_MatricNo,'' as SASI_Name,'' as sasi_cursem, ";
                        SqlStatement += "'' as FeeCode,'' as FeeDesc FROM SAS_Accounts AC LEFT JOIN SAS_Sponsor SP ON AC.CreditRef=SP.SASR_Code where AC.batchcode = " + clsGeneric.AddQuotes(TransId);
                        SqlStatement += "GROUP BY SP.SASR_Code,SP.SASR_Name";
                    }
                    if (Type == "Allocation" || Type == "Payment" || Type == "Receipt")
                    {
                        SqlStatement = "SELECT SP.SASR_Code,SP.SASR_Name,sum(AC.transamount) as transamount,'' as SASI_MatricNo,'' as SASI_Name,'' as sasi_cursem, ";
                        SqlStatement += "'' as FeeCode,'' as FeeDesc FROM SAS_Accounts AC INNER JOIN SAS_Sponsor SP ON AC.CreditRef=SP.SASR_Code where AC.batchcode = " + clsGeneric.AddQuotes(TransId);
                        SqlStatement += "GROUP BY SP.SASR_Code,SP.SASR_Name";
                    }
                    if (Type == "Invoice")
                    {
                        SqlStatement = "SELECT SAS_SponsorInvoiceDetails.refcode as FeeCode,sas_feetypes.saft_desc as FeeDesc, sum(SAS_SponsorInvoiceDetails.transamount) as transamount,";
                        SqlStatement += " '' as SASI_MatricNo,'' as SASI_Name,'' as SASR_Code,'' as SASR_Name,'' as sasi_cursem ";
                        SqlStatement += "FROM SAS_SponsorInvoiceDetails inner join sas_feetypes on SAS_SponsorInvoiceDetails.refcode = sas_feetypes.saft_code ";                        
                        SqlStatement += "where SAS_SponsorInvoiceDetails.transid in (select transid from SAS_SponsorInvoice where batchcode = " + clsGeneric.AddQuotes(TransId);
                        SqlStatement += ") GROUP BY SAS_SponsorInvoiceDetails.refcode,sas_feetypes.saft_desc,SAS_SponsorInvoiceDetails.transamount ";
                        SqlStatement += "ORDER BY SAS_SponsorInvoiceDetails.refcode";

                    }
                    
                }
                
                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //Binding Sas Account Details - start
                    DataSet _DataSet = _DatabaseFactory.ExecuteDataSet(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //Sas Account status - Ended
                    return _DataSet;
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }
            return null;
        }
        #endregion

        #region Get WorkFlow Approve Details

        public DataSet WorkFlowSASApproveDetails()
        {
            //variable declarations
            string SqlStatement = null;

            try
            {
                //Build sql statment - Start   
                //SqlStatement = "select distinct wf.workflow_id,a.category,a.batchcode";
                //SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') ";
                //SqlStatement += "THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode)) ";
                //SqlStatement += "WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) ";
                //SqlStatement += "ELSE SUM(a.transamount) END as transamount";
                //SqlStatement += ", a.postedby as createdby, a.batchdate, CASE WHEN a.description = '' THEN 'Description Details' ELSE a.description END as description, wf.page_name, a.subtype";
                //SqlStatement += " from sas_accounts a,sas_workflow wf where a.batchcode=wf.batch_code and wf.workflow_status = 5 and a.category NOT IN ('SPA','STA')";
                //SqlStatement += " group by wf.workflow_id, a.batchcode, a.category,a.postedby,a.batchdate,a.description,wf.page_name,a.subtype";
                //SqlStatement += " order by wf.workflow_id desc";
                SqlStatement = "select distinct wf.workflow_id,a.category,a.batchcode";
                SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') AND subtype = 'Student' ";
                SqlStatement += "THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode)) ";
                SqlStatement += "WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) ";
                SqlStatement += "WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN ";
                SqlStatement += "(select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "WHEN Category IN ('AFC') THEN ";
                SqlStatement += "(select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "ELSE SUM(a.transamount) END as transamount";
                SqlStatement += ", wf.user_id as createdby, ";
                SqlStatement += "CASE WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN ";
                SqlStatement += "date_trunc('day',a.batchdate) ELSE a.batchdate	END as batchdate, ";
                SqlStatement += "CASE WHEN Category IN ('AFC') THEN (select batchcode from sas_accounts where batchcode = a.batchcode group by batchcode) ";
                SqlStatement += "ELSE a.description END as description, ";
                SqlStatement += "wf.page_name, a.subtype ";
                SqlStatement += " from sas_accounts a,sas_workflow wf where a.batchcode=wf.batch_code and wf.workflow_status = 5 and a.category NOT IN ('SPA','STA')";
                SqlStatement += " group by wf.workflow_id, a.batchcode, a.category,wf.user_id,a.batchdate,a.description,wf.page_name,a.subtype";
                SqlStatement += " UNION select distinct wf.workflow_id,b.category,b.batchcode";
                SqlStatement += ", CASE WHEN Category IN ('Invoice','Credit Note','Debit Note')";
                SqlStatement += " THEN (select sum(transamount) from sas_sponsorinvoicedetails where transid in (select transid from sas_sponsorinvoice where batchcode = b.batchcode))";
                SqlStatement += " ELSE SUM(b.transamount) END as transamount, wf.user_id as createdby, b.batchdate, b.description, wf.page_name, b.subtype";
                SqlStatement += " FROM sas_sponsorinvoice b,sas_workflow wf where b.batchcode=wf.batch_code and wf.workflow_status = 5 and b.category NOT IN ('SPA','STA')";
                SqlStatement += " group by wf.workflow_id, b.batchcode, b.category,wf.user_id,b.batchdate,b.description,wf.page_name,b.subtype";
                SqlStatement += " order by workflow_id desc";
                //Build sql statment - Stop

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //Binding Sas Account Details - start
                    DataSet _DataSet = _DatabaseFactory.ExecuteDataSet(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //Sas Account status - Ended
                    return _DataSet;
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }
            return null;
        }

        #endregion

        #region GetWorkflowProcess

        public int GetWorkflowProcess(int WorkflowId, string WorkFlowRole)
        {
            int IntProcess = 0;
            string sqlGetReviewer = "select count(*) as GroupUser from sas_workflow_status where workflow_id = " + WorkflowId + " and workflow_role = " + clsGeneric.AddQuotes(WorkFlowRole);

            try
            {
                if (!FormHelp.IsBlank(sqlGetReviewer))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlGetReviewer).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            IntProcess = clsGeneric.NullToInteger(loReader["GroupUser"]);                            
                        }
                        loReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IntProcess;
        }

        #endregion

        #region Update WorkFlow for Reviewer/Approver

        public int UpdateReviewApprove(int WorkflowId, short WorkflowStatus, string WorkFlowRole, string UserId)
        {
            //variable declarations
            string SqlStatement = null;

            try
            {             
                //Build Sql Statement - Start
                SqlStatement += "INSERT INTO sas_workflow_status(workflow_id, workflow_status, date_time, workflow_role,user_name) VALUES (";
                //SqlStatement += clsGeneric.AddComma() + WorkflowId;
                SqlStatement += WorkflowId;
                SqlStatement += clsGeneric.AddComma() + WorkflowStatus;
                SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(Helper.DateConversion(DateTime.Now));
                SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(WorkFlowRole);
                SqlStatement += clsGeneric.AddComma() + clsGeneric.AddQuotes(UserId);
                SqlStatement += ")";
                //Build Sql Statement - Stop

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //track workdflow status - start
                    return _DatabaseFactory.ExecuteSqlStatement(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //track workdflow status - stop
                }

                return 1;
            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);
                return -1;
            }
        }
        #endregion

        #region Delete WorkFlow for Reviewer/Approver

        public int DeleteReviewApprove(int WorkflowId, short WorkflowStatus, string WorkFlowRole, string UserId)
        {
            //variable declarations
            string SqlStatement = null;

            try
            {
                //Build Sql Statement - Start
                SqlStatement += "DELETE FROM sas_workflow_status WHERE workflow_id = " + WorkflowId;                
                SqlStatement += " AND workflow_status = " + WorkflowStatus;
                SqlStatement += " AND workflow_role = " + clsGeneric.AddQuotes(WorkFlowRole);
                SqlStatement += " AND user_name = " + clsGeneric.AddQuotes(UserId);                     

                //Build Sql Statement - Stop

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //track workdflow status - start
                    return _DatabaseFactory.ExecuteSqlStatement(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //track workdflow status - stop
                }

                return 1;
            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);
                return -1;
            }
        }
        #endregion

        #region Get Program/Bidang-Student Intake

        public DataSet GetProgrambySemIntake(string Batchcode)
        {
            //variable declarations
            string SqlStatement = null;

            try
            {
                //modified by Hafiz @ 27/7/2016
                //SqlStatement = "SELECT (select sabp_desc from sas_bidang where sabp_code in (select sabp_code from sas_program where sapg_code = ST.sasi_pgid)) as Bidang, ";
                //SqlStatement += "(select sapg_program from sas_program where sapg_code = ST.sasi_pgid) as Program, ST.sasi_intake as SemIntake ";
                //SqlStatement += "FROM SAS_AFC AF LEFT JOIN SAS_AFCDetails AFD ON AF.TransCode=AFD.TransCode LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode ";
                //SqlStatement += "LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo WHERE AC.batchcode = " + clsGeneric.AddQuotes(Batchcode);
                //SqlStatement += " limit 1"; 

                //SqlStatement = "SELECT distinct (select sabp_desc from sas_bidang where sabp_code in (select sabp_code from sas_program where sapg_code = ST.sasi_pgid)) as Bidang, ";
                //SqlStatement += "(select sapg_program from sas_program where sapg_code = ST.sasi_pgid) as Program, ";
                //SqlStatement += "ST.sasi_intake as SemIntake, ";
                //SqlStatement += "(select SUM(transamount) from sas_accounts where creditref=ST.SASI_MatricNo) as Amount ";
                //SqlStatement += "from SAS_Accounts AC ";
                //SqlStatement += "inner join SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo ";
                //SqlStatement += "where AC.batchcode = " + clsGeneric.AddQuotes(Batchcode) + " and AC.category='AFC' ";
                
                //modified by Mona @ 3/8/2016
                SqlStatement = "select distinct on (1) AFD.programcode, SP.sapg_program as Program, SB.sabp_desc As Bidang, AFD.transamount as Amount, AF.semester as SemIntake ";
                SqlStatement += "from sas_afcdetails AFD, sas_program SP,sas_accounts AC,sas_student ST, sas_afc AF, sas_bidang SB ";
                SqlStatement += "where AFD.transcode in (select transcode from sas_afc where batchcode = " + clsGeneric.AddQuotes(Batchcode) + ") ";
                SqlStatement += "and AFD.programcode = SP.sapg_code and SP.sabp_code = SB.sabp_code and AC.batchcode = AF.batchcode ";
                SqlStatement += "group by AFD.programcode, SP.sapg_program, SB.sabp_desc,AFD.transamount,AF.semester ";
                SqlStatement += "order by AFD.programcode,AF.semester";

                if (!FormHelp.IsBlank(SqlStatement))
                {
                    //Binding Sas Account Details - start
                    DataSet _DataSet = _DatabaseFactory.ExecuteDataSet(Helper.GetDataBaseType,
                        DataBaseConnectionString, SqlStatement);
                    //Sas Account status - Ended
                    return _DataSet;
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }
            return null;
        }

        #endregion

        #region GetWorkflowDetails

        public List<WorkflowEn> GetWorkflowDetails(int userid)
        {
            List<WorkflowEn> loEnList = new List<WorkflowEn>();

            try
            {
                string sqlCmd = @"select distinct wf.workflow_id,mm.menuid,a.category,a.batchcode, 
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') AND subtype = 'Student' THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode))
                             WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) 
                             WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode)
                             WHEN Category IN ('AFC') THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE SUM(a.transamount) END as transamount, 
                        wf.user_id as createdby, 
                        CASE WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN date_trunc('day',a.batchdate) 
                          ELSE a.batchdate END as batchdate, 
                        CASE WHEN Category IN ('AFC') THEN (select batchcode from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE a.description END as description, 
                        wf.page_name, a.subtype 
                        from sas_accounts a
                        inner join sas_workflow wf on a.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on a.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status = 1 
                        and a.category NOT IN ('SPA','STA')
                        and wfa.username = (select username from ur_users where userid =" + userid + @")
                        group by wf.workflow_id, mm.menuid, a.batchcode, a.category,wf.user_id,a.batchdate,a.description,wf.page_name,a.subtype

                        UNION 

                        select distinct wf.workflow_id,mm.menuid,b.category,b.batchcode, 
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') THEN (select sum(transamount) from sas_sponsorinvoicedetails where transid in (select transid from sas_sponsorinvoice where batchcode = b.batchcode))
                          ELSE SUM(b.transamount) END as transamount, wf.user_id as createdby, b.batchdate, b.description, wf.page_name, b.subtype
                        FROM sas_sponsorinvoice b
                        inner join sas_workflow wf ON b.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on b.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status = 1 
                        and b.category NOT IN ('SPA','STA')
                        and wfa.username = (select username from ur_users where userid =" + userid + @")
                        group by wf.workflow_id, mm.menuid, b.batchcode, b.category,wf.user_id,b.batchdate,b.description,wf.page_name,b.subtype
                        order by workflow_id desc";

                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            WorkflowEn obj = new WorkflowEn();
                            obj.WorkflowId = GetValue<int>(loReader, "workflow_id");
                            obj.MenuMasterEn = new MenuMasterEn();
                            obj.MenuMasterEn.MenuID = GetValue<int>(loReader, "menuid");
                            obj.AccountsEn = new AccountsEn();
                            obj.AccountsEn.Category = GetValue<string>(loReader, "category");
                            obj.AccountsEn.BatchCode = GetValue<string>(loReader, "batchcode");
                            obj.AccountsEn.TransactionAmount = GetValue<double>(loReader, "transamount");
                            obj.UserId = GetValue<string>(loReader, "createdby");
                            obj.AccountsEn.BatchDate = GetValue<DateTime>(loReader, "batchdate");
                            obj.AccountsEn.Description = GetValue<string>(loReader, "description");
                            obj.PageName = GetValue<string>(loReader, "page_name");
                            obj.AccountsEn.SubType = GetValue<string>(loReader, "subtype");

                            loEnList.Add(obj);
                        }
                        loReader.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }

            return loEnList;
        }

        #endregion

        #region InsertWFApprovalList

        public bool InsertWFApprovalList(string batchcode, string pagename, string username, string usergroupname)
        {
            int RowsCount = 0;
            bool result = false;
            string SqlStatement = null;

            try
            {
                string SqlCount = "SELECT count(*) AS rows FROM sas_wf_approverlist WHERE batchcode = " + clsGeneric.AddQuotes(batchcode);
                SqlCount += "AND pagename = " + clsGeneric.AddQuotes(pagename) + " AND username = " + clsGeneric.AddQuotes(username);

                IDataReader _IDataReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType, DataBaseConnectionString, SqlCount).CreateDataReader();

                if (_IDataReader != null)
                {
                    if (_IDataReader.Read())
                    {
                        RowsCount = clsGeneric.NullToInteger(_IDataReader["rows"]);
                        
                        //if (RowsCount > 0)
                        //    throw new Exception("Record Already Posted");
                    }

                    _IDataReader.Close();
                    _IDataReader.Dispose();

                    if (RowsCount == 0)
                    {
                        SqlStatement = "INSERT INTO sas_wf_approverlist(batchcode, pagename, username, usergroupname)";
                        SqlStatement += " VALUES (@batchcode, @pagename, @username, @usergroupname) ";

                        if (!FormHelp.IsBlank(SqlStatement))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, SqlStatement, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@batchcode", DbType.String, batchcode);
                            _DatabaseFactory.AddInParameter(ref cmd, "@pagename", DbType.String, pagename);
                            _DatabaseFactory.AddInParameter(ref cmd, "@username", DbType.String, username);
                            _DatabaseFactory.AddInParameter(ref cmd, "@usergroupname", DbType.String, usergroupname);
                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = clsGeneric.NullToInteger(_DatabaseFactory.ExecuteScalarCommand(Helper.GetDataBaseType, cmd,
                            DataBaseConnectionString, SqlStatement, _DbParameterCollection));

                            if (liRowAffected > -1)
                            {
                                result = true;
                            }
                            else
                            {
                                throw new Exception("Insert Failed! No Row has been inserted.");
                            }
                        }
                    }
                }

                return result;
            }
           
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);
                return false;
            }
        }

        #endregion

        #region GetList

        public List<WorkflowEn> GetList()
        {
            List<WorkflowEn> loEnList = new List<WorkflowEn>();
            string sqlCmd = "SELECT * FROM sas_workflow";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            WorkflowEn loItem = LoadObject(loReader);
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

        #region LoadObject

        private WorkflowEn LoadObject(IDataReader argReader)
        {
            WorkflowEn loItem = new WorkflowEn();

            loItem.WorkflowId = GetValue<int>(argReader, "workflow_id");
            loItem.BatchCode = GetValue<string>(argReader, "batch_code");
            loItem.DateTime = GetValue<DateTime>(argReader, "date_time");
            loItem.WorkflowStatus = GetValue<Int16>(argReader, "workflow_status");
            loItem.UserId = GetValue<string>(argReader, "user_id");
            loItem.PageName = GetValue<string>(argReader, "page_name");
            loItem.WorkflowRemarks = GetValue<string>(argReader, "workflow_remarks");

            return loItem;
        }

        private static T GetValue<T>(IDataReader argReader, string argColNm)
        {
            if (!argReader.IsDBNull(argReader.GetOrdinal(argColNm)))
                return (T)argReader.GetValue(argReader.GetOrdinal(argColNm));
            else
                return default(T);
        }

        #endregion

        #region GetPendingWorkflowDetailsPreparer

        public List<WorkflowEn> GetPendingWorkflowDetailsPreparer(int userid)
        {
            List<WorkflowEn> loEnList = new List<WorkflowEn>();

            try
            {
                string sqlCmd = @"select distinct wf.workflow_id,mm.menuid,a.category,a.batchcode, 
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') AND subtype = 'Student' THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode))
                             WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) 
                             WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode)
                             WHEN Category IN ('AFC') THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE SUM(a.transamount) END as transamount, 
                        wf.user_id as createdby, 
                        CASE WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN date_trunc('day',a.batchdate) 
                          ELSE a.batchdate END as batchdate, 
                        CASE WHEN Category IN ('AFC') THEN (select batchcode from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE a.description END as description, 
                        wf.page_name, a.subtype 
                        from sas_accounts a
                        inner join sas_workflow wf on a.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on a.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status = 1 
                        and a.category NOT IN ('SPA','STA')
                        and wf.workflow_id in (select workflow_id from sas_workflow_status where user_name in (select username from ur_users where userid =" + userid + @"))
                        group by wf.workflow_id, mm.menuid, a.batchcode, a.category,wf.user_id,a.batchdate,a.description,wf.page_name,a.subtype

                        UNION 

                        select distinct wf.workflow_id,mm.menuid,b.category,b.batchcode, 
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') THEN (select sum(transamount) from sas_sponsorinvoicedetails where transid in (select transid from sas_sponsorinvoice where batchcode = b.batchcode))
                          ELSE SUM(b.transamount) END as transamount, wf.user_id as createdby, b.batchdate, b.description, wf.page_name, b.subtype
                        FROM sas_sponsorinvoice b
                        inner join sas_workflow wf ON b.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on b.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status = 1 
                        and b.category NOT IN ('SPA','STA')
                        and wf.workflow_id in (select workflow_id from sas_workflow_status where user_name in (select username from ur_users where userid =" + userid + @"))
                        group by wf.workflow_id, mm.menuid, b.batchcode, b.category,wf.user_id,b.batchdate,b.description,wf.page_name,b.subtype
                        order by workflow_id desc";

                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            WorkflowEn obj = new WorkflowEn();
                            obj.WorkflowId = GetValue<int>(loReader, "workflow_id");
                            obj.MenuMasterEn = new MenuMasterEn();
                            obj.MenuMasterEn.MenuID = GetValue<int>(loReader, "menuid");
                            obj.AccountsEn = new AccountsEn();
                            obj.AccountsEn.Category = GetValue<string>(loReader, "category");
                            obj.AccountsEn.BatchCode = GetValue<string>(loReader, "batchcode");
                            obj.AccountsEn.TransactionAmount = GetValue<double>(loReader, "transamount");
                            obj.UserId = GetValue<string>(loReader, "createdby");
                            obj.AccountsEn.BatchDate = GetValue<DateTime>(loReader, "batchdate");
                            obj.AccountsEn.Description = GetValue<string>(loReader, "description");
                            obj.PageName = GetValue<string>(loReader, "page_name");
                            obj.AccountsEn.SubType = GetValue<string>(loReader, "subtype");

                            loEnList.Add(obj);
                        }
                        loReader.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }

            return loEnList;
        }

        #endregion

        #region GetPostedWorkflowDetails

        public List<WorkflowEn> GetPostedWorkflowDetails(int userid)
        {
            List<WorkflowEn> loEnList = new List<WorkflowEn>();

            try
            {
                string sqlCmd = @"select distinct wf.workflow_id,wf.workflow_remarks,mm.menuid,a.category,a.batchcode, a.postedtimestamp,
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') AND subtype = 'Student' THEN (select sum(transamount) from sas_accountsdetails where transid in (select transid from sas_accounts where batchcode = a.batchcode))
                             WHEN Category IN ('Credit Note','Debit Note') AND subtype = 'Sponsor' THEN SUM(a.tempamount) 
                             WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode)
                             WHEN Category IN ('AFC') THEN (select SUM(transamount) as transamount from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE SUM(a.transamount) END as transamount, 
                        wf.user_id as createdby, 
                        CASE WHEN Category IN ('Receipt') AND Description like '%CIMB CLICKS%' THEN date_trunc('day',a.batchdate) 
                          ELSE a.batchdate END as batchdate, 
                        CASE WHEN Category IN ('AFC') THEN (select batchcode from sas_accounts where batchcode = a.batchcode group by batchcode) 
                          ELSE a.description END as description, 
                        CASE WHEN workflow_status = 3 THEN 'Rejected'
                          ELSE 'Approved' END as Status,
                        wf.page_name, a.subtype 
                        from sas_accounts a
                        inner join sas_workflow wf on a.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on a.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status in (2,3)  
                        and a.category NOT IN ('SPA','STA')
                        and wf.workflow_id in (select workflow_id from sas_workflow_status where user_name in (select username from ur_users where userid =" + userid + @"))
                        group by wf.workflow_id,wf.workflow_remarks, mm.menuid, a.batchcode, a.postedtimestamp, a.category,wf.user_id,a.batchdate,a.description,wf.page_name,a.subtype

                        UNION 

                        select distinct wf.workflow_id,wf.workflow_remarks,mm.menuid,b.category,b.batchcode, b.postedtimestamp,
                        CASE WHEN Category IN ('Invoice','Credit Note','Debit Note') THEN (select sum(transamount) from sas_sponsorinvoicedetails where transid in (select transid from sas_sponsorinvoice where batchcode = b.batchcode))
                          ELSE SUM(b.transamount) END as transamount, wf.user_id as createdby, b.batchdate, b.description, 
                        CASE WHEN workflow_status = 3 THEN 'Rejected'
                          ELSE 'Approved' END as Status,
                        wf.page_name, b.subtype
                        FROM sas_sponsorinvoice b
                        inner join sas_workflow wf ON b.batchcode=wf.batch_code 
                        inner join sas_wf_approverlist wfa on b.batchcode=wfa.batchcode
                        inner join ur_menumaster mm on wfa.pagename = mm.pagename
                        where wf.workflow_status in (2,3) 
                        and b.category NOT IN ('SPA','STA')                        
                        and wf.workflow_id in (select workflow_id from sas_workflow_status where user_name in (select username from ur_users where userid =" + userid + @"))
                        group by wf.workflow_id,wf.workflow_remarks, mm.menuid, b.batchcode, b.postedtimestamp, b.category,wf.user_id,b.batchdate,b.description,wf.page_name,b.subtype
                        order by workflow_id desc";

                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            WorkflowEn obj = new WorkflowEn();
                            obj.WorkflowId = GetValue<int>(loReader, "workflow_id");
                            obj.WorkflowRemarks = GetValue<string>(loReader, "workflow_remarks");
                            obj.BatchCode = GetValue<string>(loReader, "Status");
                            obj.MenuMasterEn = new MenuMasterEn();
                            obj.MenuMasterEn.MenuID = GetValue<int>(loReader, "menuid");
                            obj.AccountsEn = new AccountsEn();
                            obj.AccountsEn.Category = GetValue<string>(loReader, "category");
                            obj.AccountsEn.BatchCode = GetValue<string>(loReader, "batchcode");
                            obj.AccountsEn.TransactionAmount = GetValue<double>(loReader, "transamount");
                            obj.UserId = GetValue<string>(loReader, "createdby");
                            obj.AccountsEn.BatchDate = GetValue<DateTime>(loReader, "batchdate");
                            obj.AccountsEn.PostedDateTime = GetValue<DateTime>(loReader, "postedtimestamp");
                            obj.AccountsEn.Description = GetValue<string>(loReader, "description");
                            obj.PageName = GetValue<string>(loReader, "page_name");
                            obj.AccountsEn.SubType = GetValue<string>(loReader, "subtype");

                            loEnList.Add(obj);
                        }
                        loReader.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MaxModule.Helper.LogError(ex.Message);

            }

            return loEnList;
        }

        #endregion

        #region GetWorkflowRemarks

        public String GetWorkflowRemarks(String Batchcode)
        {
            string Remarks = "";
            string sql = "select workflow_remarks from sas_workflow where batch_code = " + clsGeneric.AddQuotes(Batchcode);

            try
            {
                if (!FormHelp.IsBlank(sql))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sql).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            Remarks = GetValue<string>(loReader, "workflow_remarks");
                        }
                        loReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Remarks;
        }

        #endregion

        #region CheckGL
        //added by Hafiz @ 17/02/2017
        //legend -> type=MJ/CB, subtype=Student/Sponsor, category=AFC/Invoice/Receipts/Payment

        public bool CheckGL(string Type, string BatchCode, string SubType, ref List<WorkflowEn> List_Failed, string Category = "")
        {
            bool result = false;
            List_Failed = new List<WorkflowEn>();

            if (Type == "MJ")
            {
                bool debit_flag = false, credit_flag = false;
                bool dbt_skip = false, crdt_skip = false;

                //DEBIT LINE - START
                string SqlStatement = null, SOURCE = null;

                try
                {
                    if (SubType == "Sponsor")
                    {
                        if (Category == MaxModule.CfGeneric.CategoryTypeCreditNote)
                        {
                            SOURCE = "Accounts";

                            SqlStatement = "SELECT SPN.SASR_Code AS id,SPN.SASR_Name AS name,SA.GLcode AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                        else
                        {
                            SOURCE = "Sponsor";

                            SqlStatement = "SELECT SPN.SASR_Code AS id,SPN.SASR_Name AS name,SPN.SASR_Glaccount AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                    }
                    else if (SubType == "Student")
                    {
                        if (Category == MaxModule.CfGeneric.CategoryTypeLoan)
                        {
                            SOURCE = "University Fund";

                            SqlStatement ="SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                                "SS.SASI_PGid||' - '||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                                "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                                "SA.CreditRef AS id,SS.SASI_Name AS name,SU.SAUF_Glcode AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_StudentLoan SL ON SA.Batchcode=SL.Batchcode " +
                                "INNER JOIN SAS_Universityfund SU ON SA.SubRef1=SU.SAUF_Code " +
                                "INNER JOIN SAS_Student SS ON SA.CreditRef=SS.SASI_MatricNo " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                        else if (Category == MaxModule.CfGeneric.CategoryTypeCreditNote)
                        {
                            dbt_skip = true;

                            List<WorkflowEn> FacultyGL = new List<WorkflowEn>();
                            FacultyGL = ConstructQuery(BatchCode, "FacultyGL");

                            if (FacultyGL.Count() > 0)
                            {
                                foreach (WorkflowEn enFGL in FacultyGL)
                                {
                                    if (string.IsNullOrEmpty(enFGL.GLCODE))
                                    {
                                        List_Failed.Add(enFGL);
                                    }
                                    else
                                    {
                                        debit_flag = true;
                                    }
                                }
                            }
                            else
                            {
                                debit_flag = true;
                            }

                            List<WorkflowEn> KolejGL = new List<WorkflowEn>();
                            KolejGL = ConstructQuery(BatchCode, "KolejGL");

                            if (KolejGL.Count() > 0)
                            {
                                foreach (WorkflowEn enKGL in KolejGL)
                                {
                                    if (string.IsNullOrEmpty(enKGL.GLCODE))
                                    {
                                        List_Failed.Add(enKGL);
                                    }
                                    else
                                    {
                                        debit_flag = true;
                                    }
                                }
                            }
                            else
                            {
                                debit_flag = true;
                            }
                        }
                        else
                        {
                            SOURCE = "Program";

                             SqlStatement = "SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                                "SP.SAPG_Code||' - '||SP.SAPG_Program AS prog," +
                                "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                                "SS.SASI_MatricNo AS id,SS.SASI_Name AS name,SP.SAPG_TI AS glcode " +
                                "FROM SAS_Student SS " +
                                "INNER JOIN SAS_Accounts SA ON SA.CreditRef = SS.SASI_MatricNo " +
                                "INNER JOIN SAS_Program SP ON SP.SAPG_Code = SS.SASI_PgId " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode) + " ";
                        }
                    }

                    if (dbt_skip == false)
                    {
                        if (!FormHelp.IsBlank(SqlStatement))
                        {
                            using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                                DataBaseConnectionString, SqlStatement).CreateDataReader())
                            {
                                while (loReader.Read())
                                {
                                    WorkflowEn _WorkflowEn = new WorkflowEn();
                                    _WorkflowEn.ID = GetValue<string>(loReader, "id");
                                    _WorkflowEn.NAME = GetValue<string>(loReader, "name");
                                    _WorkflowEn.GLCODE = clsGeneric.NullToString(GetValue<string>(loReader, "glcode"));
                                    _WorkflowEn.SOURCE = SOURCE;
                                    _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                    if (SubType == "Student")
                                    { 
                                        _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                        _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                        _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                    }

                                    if (!string.IsNullOrEmpty(_WorkflowEn.GLCODE))
                                    {
                                        debit_flag = true;
                                    }
                                    else
                                    {
                                        List_Failed.Add(_WorkflowEn);
                                    }
                                }

                                if (((System.Data.DataTableReader)(loReader)).HasRows == false)
                                {
                                    debit_flag = true;
                                }

                                loReader.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //DEBIT LINE - END

                //CREDIT LINE - START
                String SqlStatement1 = null, SOURCE1 = null;

                try
                {
                    if (SubType == "Sponsor")
                    {
                        if (Category == MaxModule.CfGeneric.CategoryTypeDebitNote)
                        {
                            SOURCE1 = "Accounts";

                            SqlStatement1 = "SELECT SPN.SASR_Code AS id,SPN.SASR_Name AS name,SA.GLcode AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                        else if (Category == MaxModule.CfGeneric.CategoryTypeAllocation)
                        {
                            SOURCE1 = "Program";

                            SqlStatement1 = "SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                                "SP.SAPG_Code||' - '||SP.SAPG_Program AS prog," +
                                "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                                "SS.SASI_MatricNo AS id,SS.SASI_Name AS name,SP.SAPG_TI AS glcode " +
                                "FROM SAS_Student SS " +
                                "INNER JOIN SAS_Accounts SA ON SA.CreditRef = SS.SASI_MatricNo " +
                                "INNER JOIN SAS_Program SP ON SP.SAPG_Code = SS.SASI_PgId " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode) + " " +
                                "AND SA.Category='SPA' " +
                                "GROUP BY fac,prog,kol,id,name,SP.SAPG_TI";
                        }
                        else
                        {
                            SOURCE1 = "Sponsor";

                            SqlStatement1 = "SELECT SPN.SASR_Code AS id,SPN.SASR_Name AS name,SPN.SASR_Glaccount AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_Sponsor SPN ON SPN.SASR_Code= SA.CreditRef " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                    }
                    else if (SubType == "Student")
                    {
                        if (Category == MaxModule.CfGeneric.CategoryTypeLoan)
                        {
                            SOURCE1 = "University Fund";

                            SqlStatement1 = "SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                                "SS.SASI_PGid||' - '||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                                "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                                "SA.CreditRef AS id,SS.SASI_Name AS name,SU.SAUF_Glcode AS glcode " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_StudentLoan SL ON SA.Batchcode=SL.Batchcode " +
                                "INNER JOIN SAS_Universityfund SU ON SA.SubRef1=SU.SAUF_Code " +
                                "INNER JOIN SAS_Student SS ON SA.CreditRef=SS.SASI_MatricNo " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode);
                        }
                        else if (Category == MaxModule.CfGeneric.CategoryTypeCreditNote)
                        {
                            SOURCE1 = "Program";

                            SqlStatement1 = "SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                                "SP.SAPG_Code||' - '||SP.SAPG_Program AS prog," +
                                "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                                "SS.SASI_MatricNo AS id,SS.SASI_Name AS name,SP.SAPG_TI AS glcode " +
                                "FROM SAS_Student SS " +
                                "INNER JOIN SAS_Accounts SA ON SA.CreditRef = SS.SASI_MatricNo " +
                                "INNER JOIN SAS_Program SP ON SP.SAPG_Code = SS.SASI_PgId " +
                                "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode) + " ";
                        }
                        else
                        {
                            crdt_skip = true;

                            List<WorkflowEn> FacultyGL = new List<WorkflowEn>();
                            FacultyGL = ConstructQuery(BatchCode, "FacultyGL");

                            if (FacultyGL.Count() > 0)
                            {
                                foreach (WorkflowEn enFGL in FacultyGL)
                                {
                                    if (string.IsNullOrEmpty(enFGL.GLCODE))
                                    {
                                        List_Failed.Add(enFGL);
                                    }
                                    else
                                    {
                                        credit_flag = true;
                                    }
                                }
                            }
                            else
                            {
                                credit_flag = true;
                            }

                            List<WorkflowEn> KolejGL = new List<WorkflowEn>();
                            KolejGL = ConstructQuery(BatchCode, "KolejGL");

                            if (KolejGL.Count() > 0)
                            {
                                foreach (WorkflowEn enKGL in KolejGL)
                                {
                                    if (string.IsNullOrEmpty(enKGL.GLCODE))
                                    {
                                        List_Failed.Add(enKGL);
                                    }
                                    else
                                    {
                                        credit_flag = true;
                                    }
                                }
                            }
                            else
                            {
                                credit_flag = true;
                            }
                        }
                    }

                    if (crdt_skip == false)
                    {
                        if (!FormHelp.IsBlank(SqlStatement1))
                        {
                            using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                                DataBaseConnectionString, SqlStatement1).CreateDataReader())
                            {
                                while (loReader.Read())
                                {
                                    WorkflowEn _WorkflowEn = new WorkflowEn();
                                    _WorkflowEn.ID = GetValue<string>(loReader, "id");
                                    _WorkflowEn.NAME = GetValue<string>(loReader, "name");
                                    _WorkflowEn.GLCODE = clsGeneric.NullToString(GetValue<string>(loReader, "glcode"));
                                    _WorkflowEn.SOURCE = SOURCE1;
                                    _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                    if (SubType == "Student")
                                    {
                                        _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                        _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                        _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                    }
                                    else
                                    {
                                        if (Category == MaxModule.CfGeneric.CategoryTypeAllocation)
                                        {
                                            _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                            _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                            _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(_WorkflowEn.GLCODE))
                                    {
                                        credit_flag = true;
                                    }
                                    else
                                    {
                                        List_Failed.Add(_WorkflowEn);
                                    }
                                }

                                if (((System.Data.DataTableReader)(loReader)).HasRows == false)
                                {
                                    credit_flag = true;
                                }

                                loReader.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (debit_flag == true && credit_flag == true)
                {
                    if (List_Failed.Count == 0)
                    {
                        result = true;
                    }
                }

            }
            else if (Type == "CBR")
            {
                bool debit_flag = false, credit_flag = false;
                String str = "", InnerJoin = "", GL_CODE = "";

                String SqlStatement = null;

                try
                {
                    if (SubType == "Sponsor")
                    {
                        str = "SELECT SAS_Sponsor.SASR_Name AS Payee_Name,";

                        InnerJoin = "INNER JOIN SAS_Sponsor ON SA.CreditRef = SAS_Sponsor.SASR_Code ";
                    }
                    else
                    {
                        str = "SELECT SS.SASI_Faculty||'-'||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," + 
                        "SS.SASI_PGid||'-'||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                        "SS.SAKO_Code||'-'||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol,"+
                        "SS.SASI_Name AS Payee_Name,";

                        InnerJoin = "INNER JOIN SAS_Student SS ON SA.CreditRef = SS.SASI_MatricNo ";
                    }

                    SqlStatement = str;
                    SqlStatement += "SA.CreditRef AS Matric_No,";
                    SqlStatement += "SA.BankCode AS Bank_Code ";
                    SqlStatement += "FROM SAS_Accounts SA ";
                    SqlStatement += InnerJoin;
                    SqlStatement += "WHERE SA.BatchCode = " + clsGeneric.AddQuotes(BatchCode);

                    if (!FormHelp.IsBlank(SqlStatement))
                    {
                        using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                            DataBaseConnectionString, SqlStatement).CreateDataReader())
                        {
                            while (loReader.Read())
                            {
                                string Bank_Code = GetValue<string>(loReader, "Bank_Code");
                                string Matric_No = GetValue<string>(loReader, "Matric_No");

                                WorkflowEn _WorkflowEn = new WorkflowEn();
                                _WorkflowEn.ID = Matric_No;
                                _WorkflowEn.NAME = GetValue<string>(loReader, "Payee_Name");
                                _WorkflowEn.SOURCE = "Bank Details";
                                _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                if (SubType == "Student")
                                {
                                    _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                    _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                    _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                }

                                GL_CODE = _CfCommon.GetGlCode(Bank_Code, Matric_No, MaxModule.CfGeneric.GlType.BankCode, Type, SubType);
                                if (!string.IsNullOrEmpty(GL_CODE))
                                {
                                    debit_flag = true;
                                }
                                else
                                {
                                    List_Failed.Add(_WorkflowEn);
                                }

                                _WorkflowEn = new WorkflowEn();
                                _WorkflowEn.ID = Matric_No;
                                _WorkflowEn.NAME = GetValue<string>(loReader, "Payee_Name");
                                _WorkflowEn.SOURCE = (SubType == "Sponsor") ? "Sponsor" : "Program";
                                _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                if (SubType == "Student")
                                {
                                    _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                    _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                    _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                }

                                GL_CODE = _CfCommon.GetGlCode(Bank_Code, Matric_No, MaxModule.CfGeneric.GlType.StudentProgram, Type, SubType);
                                if (!string.IsNullOrEmpty(GL_CODE))
                                {
                                    credit_flag = true;
                                }
                                else
                                {
                                    List_Failed.Add(_WorkflowEn);
                                }

                            }

                            if (((System.Data.DataTableReader)(loReader)).HasRows == false)
                            {
                                debit_flag = true;
                                credit_flag = true;
                            }

                            loReader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (debit_flag == true && credit_flag == true)
                {
                    if (List_Failed.Count == 0)
                    {
                        result = true;
                    }
                }

            }
            else if (Type == "CBP")
            {
                bool debit_flag = false, credit_flag = false;
                String str = "", InnerJoin = "", GL_CODE = "";

                String SqlStatement = null;

                try
                {
                    if (SubType == "Sponsor")
                    {
                        str = "SELECT SAS_Sponsor.SASR_Name AS Payee_Name,";
                        
                        InnerJoin = "INNER JOIN SAS_Sponsor ON SAS_Accounts.CreditRef = SAS_Sponsor.SASR_Code ";
                    }
                    else
                    {
                        str = "SELECT SS.SASI_Faculty||'-'||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                        "SS.SASI_PGid||'-'||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                        "SS.SAKO_Code||'-'||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                        "SS.SASI_Name AS Payee_Name,";
                        
                        InnerJoin = "INNER JOIN SAS_Student SS ON SAS_Accounts.CreditRef = SS.SASI_MatricNo ";
                    }

                    SqlStatement = str;
                    SqlStatement += "SAS_Accounts.CreditRef AS Matric_No,";
                    SqlStatement += "CASE WHEN SAS_Accounts.Paymentmode = 'EFT' THEN ";
                    SqlStatement += "CASE WHEN SAS_Accounts.Subtype = 'Student' THEN (SELECT SASI_Bank FROM SAS_Student WHERE SASI_MatricNo = SAS_Accounts.CreditRef) ";
                    SqlStatement += "ELSE SAS_Accounts.Bankcode END ";
                    SqlStatement += "ELSE SAS_Accounts.Bankcode END AS Bank_Code ";
                    SqlStatement += "FROM  SAS_Accounts ";
                    SqlStatement += InnerJoin;
                    SqlStatement += "WHERE SAS_Accounts.Batchcode = " + clsGeneric.AddQuotes(BatchCode);

                    if (!FormHelp.IsBlank(SqlStatement))
                    {
                        using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                            DataBaseConnectionString, SqlStatement).CreateDataReader())
                        {
                            while (loReader.Read())
                            {
                                string Bank_Code = GetValue<string>(loReader, "Bank_Code");
                                string Matric_No = GetValue<string>(loReader, "Matric_No");

                                WorkflowEn _WorkflowEn = new WorkflowEn();
                                _WorkflowEn.ID = Matric_No;
                                _WorkflowEn.NAME = GetValue<string>(loReader, "Payee_Name");
                                _WorkflowEn.SOURCE = (SubType == "Sponsor") ? "Sponsor" : "Bank Details";
                                _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                if (SubType == "Student")
                                {
                                    _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                    _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                    _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                }

                                GL_CODE = _CfCommon.GetGlCode(Bank_Code, Matric_No, MaxModule.CfGeneric.GlType.BankCode, Type, SubType);
                                if (!string.IsNullOrEmpty(GL_CODE))
                                {
                                    debit_flag = true;
                                }
                                else
                                {
                                    List_Failed.Add(_WorkflowEn);
                                }

                                _WorkflowEn = new WorkflowEn();
                                _WorkflowEn.ID = Matric_No;
                                _WorkflowEn.NAME = GetValue<string>(loReader, "Payee_Name");
                                _WorkflowEn.SOURCE = (SubType == "Sponsor") ? "Bank Details" : "Program";
                                _WorkflowEn.SUBTYPE = (SubType == "Student") ? "Student" : "Sponsor";

                                if (SubType == "Student")
                                {
                                    _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                                    _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                                    _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));
                                }

                                GL_CODE = _CfCommon.GetGlCode(Bank_Code, Matric_No, MaxModule.CfGeneric.GlType.StudentProgram, Type, SubType);
                                if (!string.IsNullOrEmpty(GL_CODE))
                                {
                                    credit_flag = true;
                                }
                                else
                                {
                                    List_Failed.Add(_WorkflowEn);
                                }

                            }

                            if (((System.Data.DataTableReader)(loReader)).HasRows == false)
                            {
                                debit_flag = true;
                                credit_flag = true;
                            }

                            loReader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (debit_flag == true && credit_flag == true)
                {
                    if (List_Failed.Count == 0)
                    {
                        result = true;
                    }
                }
            }
            
            return result;
        }

        public List<WorkflowEn> ConstructQuery(string BatchCode,string stats)
        {
            String SqlStatement = "", SOURCE = null;
            List<WorkflowEn> LIST = new List<WorkflowEn>();

            if (stats=="FacultyGL")
            {
                SOURCE = "Faculty";

                SqlStatement = "SELECT FGL.SAFC_Code||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=FGL.SAFC_Code) AS fac," +
                    "SS.SASI_PGid||' - '||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                    "SS.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=SS.SAKO_Code) AS kol," +
                    "SA.CreditRef AS id,SS.SASI_Name AS name,FGL.GL_Account AS glcode " +
                    "FROM SAS_Accounts SA " +
                    "INNER JOIN SAS_AccountsDetails SAD ON SA.TransId=SAD.TransID " +
                    "INNER JOIN SAS_Student SS ON SA.CreditRef = SS.SASI_MatricNo " +
                    "INNER JOIN SAS_Faculty_GLaccount FGL ON FGL.SAFC_Code = SS.SASI_Faculty AND FGL.SAFT_Code  = SAD.RefCode " +
                    "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode) + " " +
                    "GROUP BY id,name,FGL.GL_Account,fac,prog,kol ";
            }

            if (stats=="KolejGL")
            {
                SOURCE = "College";

                SqlStatement = "SELECT SS.SASI_Faculty||' - '||(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code=SS.SASI_Faculty) AS fac," +
                    "SS.SASI_PGid||' - '||(SELECT SAPG_Program FROM SAS_Program WHERE SAPG_Code=SS.SASI_PGid) AS prog," +
                    "KGL.SAKO_Code||' - '||(SELECT SAKO_Description FROM SAS_Kolej WHERE SAKO_Code=KGL.SAKO_Code) AS kol," +
                    "SA.CreditRef AS id,SS.SASI_Name AS name,KGL.GL_Account AS glcode " +
                    "FROM SAS_Accounts SA " +
                    "INNER JOIN SAS_AccountsDetails SAD ON SA.TransId=SAD.TransID " +
                    "INNER JOIN SAS_Student SS ON SA.CreditRef = SS.SASI_MatricNo " +
                    "INNER JOIN SAS_Kolej_GLaccount KGL ON KGL.SAKO_Code = SS.SAKO_Code AND KGL.SAFT_Code = SAD.RefCode " +
                    "WHERE SA.Batchcode = " + clsGeneric.AddQuotes(BatchCode) + " " +
                    "GROUP BY id,name,KGL.GL_Account,fac,prog,kol ";
            }
           
            if (!FormHelp.IsBlank(SqlStatement))
            {
                using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                    DataBaseConnectionString, SqlStatement).CreateDataReader())
                {
                    while (loReader.Read())
                    {
                        WorkflowEn _WorkflowEn = new WorkflowEn();
                        _WorkflowEn.ID = GetValue<string>(loReader, "id");
                        _WorkflowEn.NAME = GetValue<string>(loReader, "name");
                        _WorkflowEn.GLCODE = clsGeneric.NullToString(GetValue<string>(loReader, "glcode"));
                        _WorkflowEn.SOURCE = SOURCE;
                        _WorkflowEn.SUBTYPE = "Student";
                        _WorkflowEn.FAC = clsGeneric.NullToString(GetValue<string>(loReader, "fac"));
                        _WorkflowEn.PROG = clsGeneric.NullToString(GetValue<string>(loReader, "prog"));
                        _WorkflowEn.KOL = clsGeneric.NullToString(GetValue<string>(loReader, "kol"));

                        LIST.Add(_WorkflowEn);
                    }
                    loReader.Close();
                }
            }

            return LIST;
        }

        #endregion
    }
}
