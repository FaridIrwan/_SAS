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
    public class SagaDAL
    {
        #region Global Declarations

        private DbParameterCollection _DbParameterCollection = null;

        private MaxModule.DatabaseProvider _DatabaseFactory = new MaxModule.DatabaseProvider();

        private string DataBaseConnectionString = Helper.GetConnectionString();

        #endregion

        public SagaDAL()
        {
        }

        #region Insert

        public bool Insert(SagaEn argEn)
        {
            int cnt = 0;
            bool lbRes = false;
            string sqlCmd = "SELECT COUNT(*) AS cnt FROM SAS_Saga WHERE Posting_Type = @Posting_Type AND Auto_Prefix = @Auto_Prefix";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmdSel = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@Posting_Type", DbType.Int16, argEn.Posting_Type);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@Auto_Prefix", DbType.String, argEn.Auto_Prefix);
                    _DbParameterCollection = cmdSel.Parameters;

                    using (IDataReader dr = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmdSel,
                        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader())
                    {
                        if (dr.Read())
                            cnt = clsGeneric.NullToInteger(dr["cnt"]);
                        if (cnt > 0)
                            throw new Exception("Record Already Exist");
                    }
                    if (cnt == 0)
                    {
                        sqlCmd = "UPDATE SAS_Saga SET Auto_Prefix = @Auto_Prefix WHERE Posting_Type = @Posting_Type";

                        if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@Posting_Type", DbType.Int16, argEn.Posting_Type);
                            _DatabaseFactory.AddInParameter(ref cmd, "@Auto_Prefix", DbType.String, argEn.Auto_Prefix);
                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);

                            if (liRowAffected > -1)
                                lbRes = true;
                            else
                                throw new Exception("Insertion Failed! No Row has been updated...");
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

        #region GetSagaList

        public List<SagaEn> GetList(SagaEn argEn)
        {
            List<SagaEn> loEnList = new List<SagaEn>();
            string sqlCmd = "SELECT * FROM SAS_Saga ";

            if (argEn.Posting_Type != -1) sqlCmd = sqlCmd + " WHERE Posting_Type = " + argEn.Posting_Type + " ";

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            SagaEn loItem = LoadObjectSaga(loReader);
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

        #region GetSagaPostingList

        public List<SagaEn> GetSagaPostingList(SagaEn argEn)
        {
            List<SagaEn> loEnList = new List<SagaEn>();
            string sqlCmd = "SELECT * FROM SAS_Saga_Posting ";

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            SagaEn loItem = LoadObjectSagaPosting(loReader);
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

        #region GetSelectedSaga

        public SagaEn GetSelectedSaga(int posting_type)
        {
            SagaEn loItem = new SagaEn();
            string sqlCmd = "SELECT * FROM SAS_Saga WHERE Posting_Type = " + posting_type;

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            loItem = LoadObjectSaga(loReader);
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

        #region LoadObject
        
        private SagaEn LoadObjectSaga(IDataReader argReader)
        {
            SagaEn loItem = new SagaEn();
            loItem.Auto_No = GetValue<int>(argReader, "Auto_No");
            loItem.Auto_Prefix = GetValue<string>(argReader, "Auto_Prefix");
            loItem.Posting_Type = GetValue<int>(argReader, "Posting_Type");
            loItem.Auto_Length = GetValue<int>(argReader, "Auto_Length");

            return loItem;
        }

        private SagaEn LoadObjectSagaPosting(IDataReader argReader)
        {
            SagaEn loItem = new SagaEn();
            loItem.Posting_Id = GetValue<int>(argReader, "Posting_Id");
            loItem.Batch_Code = GetValue<string>(argReader, "Batch_Code");
            loItem.Reference_No = GetValue<string>(argReader, "Reference_No");
            loItem.Posting_Type = GetValue<int>(argReader, "Posting_Type");
            loItem.Posting_Date = GetValue<DateTime>(argReader, "Posting_Date");

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

    }
}
