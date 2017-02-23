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
    /// <summary>
    /// Class to handle all the StudentSponsor Methods.
    /// </summary>
    public class StudentSponDAL
    {
        #region Global Declarations 

        private DbParameterCollection _DbParameterCollection = null;

        private MaxModule.DatabaseProvider _DatabaseFactory =
            new MaxModule.DatabaseProvider();

        private string DataBaseConnectionString = Helper.
           GetConnectionString();

        #endregion

        public StudentSponDAL()
        {
        }

        #region GetList 

        /// <summary>
        /// Method to Get List of StudentSponsor
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity as an Input.</param>
        /// <returns>Returns List of StudentSponsor</returns>
        public List<StudentSponEn> GetList(StudentSponEn argEn)
        {
            List<StudentSponEn> loEnList = new List<StudentSponEn>();
            string sqlCmd = "select * from SAS_StudentSpon";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            StudentSponEn loItem = LoadObject(loReader);
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

        #region GetStudentSpn 

        /// <summary>
        /// Method to Get List of StudentSponsor by MatricNo
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity as an Input.MatricNo as Input Property.</param>
        /// <returns>Returns List of StudentSponsor</returns>
        public List<StudentSponEn> GetStudentSpn(string argEn)
        {
            List<StudentSponEn> loEnList = new List<StudentSponEn>();
            string sqlCmd = "select * from SAS_StudentSpon where SASI_MatricNo='"+argEn+"'";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            StudentSponEn loItem = LoadObject(loReader);
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

        #region GetStuSponserList 

        /// <summary>
        /// Method to Get List of All StudentSponsors
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity as an Input.MatricNo as Input Property.</param>
        /// <returns>Returns List of StudentSponsor</returns>
        public List<StudentSponEn> GetStuSponserList(StudentSponEn argEn)
        {
            List<StudentSponEn> loEnList = new List<StudentSponEn>();
            //string sqlCmd = "select SS.*,SP.SASR_Name from sas_StudentSpon SS,sas_Sponsor SP where SS.SASS_Sponsor=SP.SASR_Code";
            string sqlCmd="SELECT SAS_StudentSpon.SASI_MatricNo, SAS_StudentSpon.SASS_Sponsor, SAS_Sponsor.SASR_Name, SAS_StudentSpon.SASS_SDate, " +
                      " SAS_StudentSpon.SASS_EDate, SAS_StudentSpon.SASS_Status, SAS_StudentSpon.SASS_Num, SAS_StudentSpon.SASS_Type, SAS_StudentSpon.SASS_Limit " +
                      " FROM  SAS_Student INNER JOIN SAS_StudentSpon ON SAS_Student.SASI_MatricNo = SAS_StudentSpon.SASI_MatricNo INNER JOIN " +
                      " SAS_Sponsor ON SAS_StudentSpon.SASS_Sponsor = SAS_Sponsor.SASR_Code WHERE SAS_StudentSpon.SASI_MatricNo ='" +argEn.MatricNo + "'";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            StudentSponEn loItem = LoadObject(loReader);
                            loItem.Name = GetValue<string>(loReader, "SASR_Name");
                            StuSponFeeTypesDAL loStuSpnFTDal = new StuSponFeeTypesDAL();
                            StuSponFeeTypesEn loStuSpnFTEn = new StuSponFeeTypesEn();
                            loStuSpnFTEn.MatricNo = loItem.MatricNo;
                            loStuSpnFTEn.SASR_Code = loItem.Sponsor;
                            loItem.ListStuSponFeeTypes = loStuSpnFTDal.GetStuSponFTList(loStuSpnFTEn);
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

        /// <summary>
        /// Method to Get StudentSponsor Entity
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity is an Input.MatricNo as Input Property.</param>
        /// <returns>Returns StudentSponsor Entity</returns>
        public StudentSponEn GetItem(StudentSponEn argEn)
        {
            StudentSponEn loItem = new StudentSponEn();
            string sqlCmd = "Select * FROM SAS_StudentSpon WHERE SASI_MatricNo = @SASI_MatricNo";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmd, "@SASI_MatricNo", DbType.String, argEn.MatricNo);
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

        #region Insert 

        /// <summary>
        /// Method to Insert StudentSponsor 
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity is an Input.</param>
        /// <returns>Returns Boolean</returns> 
        public bool Insert(StudentSponEn argEn)
        {
            bool lbRes = false;
            string sqlCmd;
            string d1, m1, y1, d2, m2, y2;
            string todate = ""; string fromdate = "";
            if (argEn.SDate != "")
            {
                d1 = argEn.SDate.Substring(0, 2);
                m1 = argEn.SDate.Substring(3, 2);
                y1 = argEn.SDate.Substring(6, 4);
                fromdate = y1 + "-" + m1 + "-" + d1;
            }
            else if (argEn.SDate == "")
            {
                //loItem.Intake = "-1";
            }

            if (argEn.EDate != "")
            {
                d2 = argEn.EDate.Substring(0, 2);
                m2 = argEn.EDate.Substring(3, 2);
                y2 = argEn.EDate.Substring(6, 4);
                todate = y2 + "-" + m2 + "-" + d2;
            }
            else if (argEn.EDate == "")
            {
                //loItem.CurretSemesterYear = "-1";
            }
                            
            try
            {
                sqlCmd = "INSERT INTO SAS_StudentSpon(SASI_MatricNo,SASS_Sponsor,SASS_SDate,SASS_EDate,SASS_Status,SASS_Num,SASS_Type,sass_limit) " +
                            "VALUES (@SASI_MatricNo,@SASS_Sponsor,@SASS_SDate,@SASS_EDate,@SASS_Status,@SASS_Num,@SASS_Type,@SASS_Limit) ";
                        if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASI_MatricNo", DbType.String, argEn.MatricNo);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Sponsor", DbType.String, argEn.Sponsor);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_SDate", DbType.String, fromdate);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_EDate", DbType.String, todate);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Status", DbType.Boolean, argEn.Status);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Num", DbType.Int32, argEn.Num);                            
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Type", DbType.Boolean, argEn.FullySponsered);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Limit", DbType.Decimal, argEn.SponsorLimit);

                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);
                            
                            if (liRowAffected > -1)
                                lbRes = true;
                            else
                                throw new Exception("Insertion Failed! No Row has been updated...");
                        }
             }
                
            
            catch (Exception ex)
            {
                throw ex;
            }
            return lbRes;
        }

        #endregion

        #region Update 

        /// <summary>
        /// Method to Update StudentSponsor 
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity is an Input.</param>
        /// <returns>Returns Boolean</returns> 
        public bool Update(StudentSponEn argEn)
        {
            bool lbRes = false;
            int iOut = 0;
            string sqlCmd = "Select count(*) as cnt From SAS_StudentSpon WHERE SASI_MatricNo = @SASI_MatricNo";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmdSel = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@SASI_MatricNo", DbType.String, argEn.MatricNo);
                    _DbParameterCollection = cmdSel.Parameters;

                    using (IDataReader dr = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmdSel,
                        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader()) 
                    {
                        if (dr.Read())
                            iOut = clsGeneric.NullToInteger(dr["cnt"]);
                        if (iOut > 0)
                            throw new Exception("Update Failed! No Record Exist!");
                    }
                    if (iOut == 0)
                    {
                        sqlCmd = "UPDATE SAS_StudentSpon SET SASI_MatricNo = @SASI_MatricNo, SASS_Sponsor = @SASS_Sponsor, SASS_SDate = @SASS_SDate, " +
                                    "SASS_EDate = @SASS_EDate, SASS_Status = @SASS_Status, SASS_Num = @SASS_Num, SASS_Type = @SASS_Type, SASS_Limit = @SASS_Limit " +
                                    "WHERE SASI_MatricNo = @SASI_MatricNo";
                        if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASI_MatricNo", DbType.String, argEn.MatricNo);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Sponsor", DbType.String, argEn.Sponsor);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_SDate", DbType.String, argEn.SDate);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_EDate", DbType.String, argEn.EDate);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Status", DbType.Boolean, argEn.Status);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Num", DbType.Int32, argEn.Num);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Type", DbType.Boolean, argEn.FullySponsered);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SASS_Limit", DbType.Decimal, argEn.SponsorLimit);

                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);
                            
                            if (liRowAffected > -1)
                                lbRes = true;
                            else
                                throw new Exception("Update Failed! No Row has been updated...");
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

        #region Delete 

        /// <summary>
        /// Method to Delete StudentSponsor 
        /// </summary>
        /// <param name="argEn">StudentSponsor Entity is an Input.MatricNo as Input Property.</param>
        /// <returns>Returns Boolean</returns> 
        public bool Delete(StudentSponEn argEn)
        {
            bool lbRes = false;
            string sqlCmd = "DELETE FROM SAS_StudentSpon WHERE SASI_MatricNo = @SASI_MatricNo";
           try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmd, "@SASI_MatricNo", DbType.String, argEn.MatricNo);
                    _DbParameterCollection = cmd.Parameters;

                    int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);
                    
                    if (liRowAffected > -1)
                        lbRes = true;
                    else
                        throw new Exception("Insertion Failed! No Row has been updated...");
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

        /// <summary>
        /// Method to Load StudentSponsor Entity 
        /// </summary>
        /// <param name="argReader">IDataReader Object is an Input.</param>
        /// <returns>Returns StudentSponsor Entity</returns>
        private StudentSponEn LoadObject(IDataReader argReader)
        {
            StudentSponEn loItem = new StudentSponEn();
            loItem.MatricNo = GetValue<string>(argReader, "SASI_MatricNo");
            loItem.Sponsor = GetValue<string>(argReader, "SASS_Sponsor");
            string code = GetValue<string>(argReader, "SASS_SDate");
            string fromdate = ""; string todate = "";
            string d1, m1, y1, d2, m2, y2;
            
            if (!string.IsNullOrWhiteSpace(code))
            {
                y1 = code.Substring(0, 4);
                m1 = code.Substring(5, 2);
                d1 = code.Substring(8, 2);
                fromdate = d1 + "/" + m1 + "/" + y1;
            }

            string code2 = GetValue<string>(argReader, "SASS_EDate");
            if (!string.IsNullOrWhiteSpace(code2))
            {
                y2 = code2.Substring(0, 4);
                m2 = code2.Substring(5, 2);
                d2 = code2.Substring(8, 2);
                todate = d2 + "/" + m2 + "/" + y2;
            }

            loItem.SDate = fromdate;
            loItem.EDate = todate;
            loItem.Status = GetValue<bool>(argReader, "SASS_Status");
            loItem.Num = GetValue<int>(argReader, "SASS_Num");
            loItem.FullySponsered = GetValue<bool>(argReader, "SASS_Type");
            loItem.SponsorLimit = GetValue<double>(argReader, "SASS_Limit");
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
