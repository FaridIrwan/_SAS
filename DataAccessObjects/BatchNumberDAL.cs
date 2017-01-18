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

//added by Hafiz @ 08/12/2016
//Batch Format By Category

namespace HTS.SAS.DataAccessObjects
{

    public class BatchNumberDAL
    {
        #region Global Declarations 

        private DbParameterCollection _DbParameterCollection = null;
        private MaxModule.DatabaseProvider _DatabaseFactory = new MaxModule.DatabaseProvider();
        private string DataBaseConnectionString = Helper.GetConnectionString();

        #endregion

        public BatchNumberDAL()
        {
        }

        #region GetList 

        //get list of BatchNumber
        public List<BatchNumberEn> GetList(BatchNumberEn argEn)
        {
            List<BatchNumberEn> loEnList = new List<BatchNumberEn>();
            string sqlCmd = "SELECT * FROM SAS_BatchNumber";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                       DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            BatchNumberEn loItem = LoadObject(loReader);
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

        #region GetItem 

        //get BatchNumber Entity
        public BatchNumberEn GetItem(BatchNumberEn argEn)
        {
            BatchNumberEn loItem = new BatchNumberEn();
            string sqlCmd = "SELECT * FROM SAS_BatchNumber WHERE BN_Id = @BN_Id AND BN_Process = @BN_Process";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmd, "@BN_Id", DbType.Int32, argEn.BN_Id);
                    _DatabaseFactory.AddInParameter(ref cmd, "@BN_Process", DbType.String, argEn.BN_Process);
                    _DbParameterCollection = cmd.Parameters;

                    using (IDataReader loReader = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmd,
                        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader()) 
                    {
                        if (loReader != null)
                        {
                            loReader.Read();
                            loItem = LoadObject(loReader);
                        }
                        loReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return loItem;
        }

        #endregion

        #region Update 

        //update BatchNumber
        public bool Update(BatchNumberEn argEn)
        {
            bool lbRes = false;
            int cnt = 0;

            string sqlCmd = "SELECT COUNT(*) AS cnt FROM SAS_BatchNumber WHERE BN_Id = " + argEn.BN_Id + " AND BN_AutoNo = '" + argEn.BN_AutoNo + "'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader dr = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        if (dr.Read())
                            cnt = clsGeneric.NullToInteger(dr["cnt"]);
                        if (cnt > 0)
                            throw new Exception("Record Already Exist");
                    }

                    if (cnt == 0)
                    {
                        sqlCmd = "UPDATE SAS_BatchNumber SET BN_CurNo = @BN_CurNo, BN_AutoNo = @BN_AutoNo WHERE BN_Id = @BN_Id AND BN_Process = @BN_Process";

                        if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@BN_Id", DbType.Int32, argEn.BN_Id);
                            _DatabaseFactory.AddInParameter(ref cmd, "@BN_Process", DbType.String, argEn.BN_Process);
                            _DatabaseFactory.AddInParameter(ref cmd, "@BN_CurNo", DbType.Int32, argEn.BN_CurNo);
                            _DatabaseFactory.AddInParameter(ref cmd, "@BN_AutoNo", DbType.String, argEn.BN_AutoNo);
                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                        DataBaseConnectionString, sqlCmd, _DbParameterCollection);

                            if (liRowAffected > -1)
                                lbRes = true;
                            else
                                throw new Exception("Update Failed! No Row has been updated.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lbRes;
        }

        #endregion

        #region LoadObject 

        //load BatchNumber Entity
        private BatchNumberEn LoadObject(IDataReader argReader)
        {
            BatchNumberEn loItem = new BatchNumberEn();
            loItem.BN_Id = GetValue<int>(argReader, "BN_Id");
            loItem.BN_Process = GetValue<string>(argReader, "BN_Process");
            loItem.BN_Prefix = GetValue<string>(argReader, "BN_Prefix");
            loItem.BN_NoDigit = GetValue<int>(argReader, "BN_NoDigit");
            loItem.BN_StartNo = GetValue<int>(argReader, "BN_StartNo");
            loItem.BN_CurNo = GetValue<int>(argReader, "BN_CurNo");
            loItem.BN_AutoNo = GetValue<string>(argReader, "BN_AutoNo");

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

        #region GenerateBatchNumber

        //generate BatcNumber By Category specified
        public string GenerateBatchNumber(string Category)
        {
            int id = 0, nodigit = 0, curno = 0, i = 0;
            string prefix = "";

            string SqlStr = "SELECT * FROM SAS_BatchNumber WHERE BN_Process = '" + Category + "'";

            try
            {
                IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType, DataBaseConnectionString,
                    SqlStr).CreateDataReader();

                if (loReader.Read())
                {
                    id = Convert.ToInt32(loReader["BN_Id"]);
                    prefix = Convert.ToString(loReader["BN_Prefix"]);
                    nodigit = Convert.ToInt32(loReader["BN_NoDigit"]);
                    curno = Convert.ToInt32(loReader["BN_CurNo"]) + 1;

                    if (curno.ToString().Length < nodigit)
                    {
                        while (i < nodigit - curno.ToString().Length)
                        {
                            prefix = prefix + "0";
                            i = i + 1;
                        }
                        prefix = prefix + curno;
                    }
                    loReader.Close();
                }

                BatchNumberEn loItem = new BatchNumberEn();

                loItem.BN_Id = Convert.ToInt32(id);
                loItem.BN_Process = Category;
                loItem.BN_CurNo = curno;
                loItem.BN_AutoNo = prefix;

                if (new BatchNumberDAL().Update(loItem) == false)
                {
                    return "";
                }

                return prefix;
            }

            catch (Exception ex)
            {
                Console.Write("GenerateBatchNumber failed with " + ex.Message);
                return ex.ToString();
            }

        }

        #endregion
    }
}


