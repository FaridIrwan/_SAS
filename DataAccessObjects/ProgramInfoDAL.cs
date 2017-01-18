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
    /// Class to handle all the ProgramInfo Methods.
    /// </summary>
    public class ProgramInfoDAL
    {
        #region Global Declarations 

        private DbParameterCollection _DbParameterCollection = null;

        private MaxModule.DatabaseProvider _DatabaseFactory =
            new MaxModule.DatabaseProvider();

        private string DataBaseConnectionString = Helper.
           GetConnectionString();

        #endregion

        public ProgramInfoDAL()
        {
        }

        #region GetProgramListByDegreeType 

        /// <summary>
        /// Method to Get All Program
        /// </summary>
        /// <param name="argEn">SAPG_ProgramType as Input if not null.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetProgramListByDegreeType(DegreeTypeEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            argEn.DegreeTypeCode = argEn.DegreeTypeCode.Replace("*", "%");
            string sqlCmd = "select * from SAS_Program";
            if (argEn.DegreeTypeCode.Length != 0) sqlCmd += " where SAPG_ProgramType = '" + argEn.DegreeTypeCode + "'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramList 

        /// <summary>
        /// Method to Get List of Active ProgramInfo
        /// </summary>
        /// <param name="argEn">SAFC_Code as Input.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetProgramList(string argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            //string sqlCmd = "select * from SAS_Program where SAFC_Code='" + argEn + "'and SAPG_Status = '1'";
            string sqlCmd = "select * from SAS_Program where SAFC_Code='" + argEn + "'and SAPG_Status = 'true'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramList 

        public List<ProgramInfoEn> GetProgramList(string argEn,bool IsBM)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            //string sqlCmd = "select * from SAS_Program where SAFC_Code='" + argEn + "'and SAPG_Status = '1'";
            string sqlCmd = "select * from SAS_Program where SAFC_Code='" + argEn + "'and SAPG_Status = 'true'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = new ProgramInfoEn();
                            loItem.ProgramCode = GetValue<string>(loReader, "SAPG_Code");
                            loItem.Program = GetValue<string>(loReader, "SAPG_Code") + "-" + GetValue<string>(loReader, "SAPG_ProgramBM");
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

        #region GetList 

        /// <summary>
        /// Method to Get Programs
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as Input.SAFC_Code as Input Property.</param>
        /// <returns>Returns List Of Programs</returns>
        public List<ProgramInfoEn> GetList(ProgramInfoEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            string sqlCmd = "select * from SAS_Program where SAFC_Code ='" + argEn.SAFC_Code + "' Order by SAPG_ProgramType";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramInfoList 

        /// <summary>
        /// Method to Get List of Active or Inactive ProgramInfo
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as an Input.ProgramCode,Program,ProgramBM and Status as Input Properties.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetProgramInfoList(ProgramInfoEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            //argEn.ProgramType = argEn.ProgramType.Replace("*", "%");
            argEn.ProgramCode = argEn.ProgramCode.Replace("*", "%");
            argEn.Program = argEn.Program.Replace("*", "%");
            argEn.ProgramBM = argEn.ProgramBM.Replace("*", "%");
            string sqlCmd = "SELECT SAS_Faculty.SAFC_Code , SAS_Faculty.SAFC_Desc, SAS_Program.*, SAS_Program.SAFC_Code" +
                            " FROM  SAS_Faculty INNER JOIN SAS_Program ON SAS_Faculty.SAFC_Code = SAS_Program.SAFC_Code WHERE SAS_Program.SAPG_Code <> '0'";
            if (argEn.ProgramCode.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Code like '%" + argEn.ProgramCode + "%'";
            if (argEn.Program.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Program like '%" + argEn.Program + "%'";
            if (argEn.ProgramBM.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_ProgramBM like '%" + argEn.ProgramBM + "%'";
            if (argEn.ProgramType.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_ProgramType like '%" + argEn.ProgramType + "%'";
            if (argEn.Faculty.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAFC_Code = '" + argEn.Faculty + "'";
            //if (argEn.FieldStudy.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_FieldStudy like '" + argEn.FieldStudy + "'";
            //if (argEn.Status == true) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Status =1";
            if (argEn.Status == true) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Status = 'true'";
            //if (argEn.Status == false) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Status =0";
            if (argEn.Status == false) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Status ='false'";
            sqlCmd = sqlCmd + " order by SAS_Program.SAPG_Code";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramInfoAll 

        /// <summary>
        /// Method to Get List of All ProgramInfo
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as an Input.ProgramCode,Program and ProgramBM  as Input Properties.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetProgramInfoAll(ProgramInfoEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            argEn.ProgramCode = argEn.ProgramCode.Replace("*", "%");
            argEn.Program = argEn.Program.Replace("*", "%");
            argEn.ProgramBM = argEn.ProgramBM.Replace("*", "%");
            string sqlCmd = "SELECT SAS_Faculty.SAFC_Code , SAS_Faculty.SAFC_Desc, SAS_Program.*, SAS_Program.SAFC_Code" +
                            " FROM  SAS_Faculty INNER JOIN SAS_Program ON SAS_Faculty.SAFC_Code = SAS_Program.SAFC_Code WHERE SAS_Program.SAPG_Code <> '0'";
            if (argEn.ProgramCode.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Code like '" + argEn.ProgramCode + "'";
            if (argEn.Program.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_Program like '" + argEn.Program + "'";
            if (argEn.ProgramBM.Length != 0) sqlCmd = sqlCmd + " and SAS_Program.SAPG_ProgramBM like '" + argEn.ProgramBM + "'";
            sqlCmd = sqlCmd + " order by SAS_Program.SAPG_Code";

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramInfoListAll 

        /// <summary>
        /// Method to Get List of ProgramInfo by SAFC_Code
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as an Input.SAFC_Code  as Input Property.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetProgramInfoListAll(string argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            string sqlCmd = "select * from SAS_Program where SAFC_Code='" + argEn + "'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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

        #region GetProgramInfoListAllMatricNo

        public List<StudentEn> GetProgramInfoListAllMatricNo(AFCEn argEn)
        {
            List<StudentEn> loEnList = new List<StudentEn>();
            string sqlCmd = @"SELECT ST.SASI_MatricNo FROM SAS_Student ST " +
                //WHERE ST.SASI_CurSemYr='" + argEn.Semester + "' " +
                                "WHERE ST.SASI_Intake='" + argEn.Semester + "' " +
                                "AND ST.SASI_PgId='" + argEn.SASI_Name + "' " +
                                "AND ST.SASI_Faculty='" + argEn.SAFC_Code + "' " +
                                @"AND ST.SASI_PostStatus= '" + argEn.PostStatus + "' " +
                                @"AND ST.SASI_StatusRec = '1' AND ST.SASS_CODE = '1' ";
            //AND SASI_CurSemYr IN (select Sast_Code from sas_semestersetup where sast_iscurrentsem = true)"; commented by Hafiz @ 08/6/2016

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            StudentEn loItem = new StudentEn();
                            loItem.MatricNo = GetValue<string>(loReader, "SASI_MatricNo");
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

        #region GetAllMatricNoForPosting 

        public List<StudentEn> GetAllMatricNoForPosting(AFCEn argEn)
        {
            List<StudentEn> loEnList = new List<StudentEn>();
            string sqlCmd = @" SELECT ST.SASI_MatricNo
                    FROM SAS_AFC AF 
                    LEFT JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode 
                    LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode
                    LEFT JOIN SAS_AccountsDetails ACD ON AC.TransCode=ACD.TransCode
                    LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo
                    LEFT JOIN SAS_Program SP ON AD.ProgramCode=SP.SAPG_Code
                    WHERE AF.Reference='Ready'  AND ST.SASI_PostStatus=1  
					AND AF.Semester='" + argEn.Semester + "'" +
                    " AND AF.SAFC_Code='" + argEn.SAFC_Code + "'" +
                    " AND ST.SASI_PgId='" + argEn.SASI_Name + "'";
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            StudentEn loItem = new StudentEn();
                            loItem.MatricNo = GetValue<string>(loReader, "SASI_MatricNo");
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

        #region GetAfcPrograms

        /// <summary>
        /// Method to Get List of AFC Programs
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as an Input.SAFC_Code  as Input Property.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        /// Modified by Hafiz @ 07/6/2016 - include logic for ddlCurrSem.SelectedValue
        /// Modified by Hafiz @ 22/10/2016 - remove join table with SAS_AccountsDetails;no use&long query execution time
        /// Modified by Hafiz @ 12/11/2016 - not displaying correct result

        public List<ProgramInfoEn> GetAfcPrograms(ProgramInfoEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();

            argEn.BidangCode = argEn.BidangCode.Replace("*", "%");
            argEn.Faculty = argEn.Faculty.Replace("*", "%");
            argEn.Semester = argEn.Semester.Replace("*", "%");
            argEn.CurrentSemester = argEn.CurrentSemester.Replace("*", "%");
            string sqlCmd = string.Empty;

            if (argEn.TransStatus == "2")
            {
                //POSTED
                sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.Reference as PostStatus,ST.SASI_Intake as SASI_Intake, 
                            '1' as  StudentCount,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr
                            FROM SAS_AFC AF 
                            INNER JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode
                            INNER JOIN SAS_Program SP ON AF.SAFC_Code=SP.SAFC_Code AND AD.ProgramCode=SP.SAPG_Code
                            INNER JOIN SAS_Accounts SA ON AF.Batchcode=SA.Batchcode
                            INNER JOIN SAS_Student ST ON AF.Semester = ST.SASI_CurSemYr AND AD.ProgramCode=ST.SASI_PgId AND SA.Creditref=ST.SASI_MatricNo
                            WHERE AF.Reference='Posted' 
                            AND AF.SAFC_Code IS NOT NULL 
                            AND  ST.SASI_Reg_Status = 2                             
                            AND SASS_Code IN (SELECT SASS_Code FROM SAS_StudentStatus WHERE SASS_Blstatus  ='true') ";
              
                if (argEn.BidangCode.Length != 0) sqlCmd += " AND SP.SABP_Code = '" + argEn.BidangCode + "'";
                if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester = '" + argEn.Semester + "'";
                if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code = '" + argEn.Faculty + "'";
                if (argEn.CurrentSemester.Length != 0) sqlCmd += " AND ST.SASI_CurSemYr LIKE '" + argEn.CurrentSemester + "'";
                if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode = '" + argEn.BatchNo + "'";
                sqlCmd += " GROUP BY AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.Reference,ST.SASI_Intake,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr ORDER BY SASI_PgId";
            }
            if (argEn.TransStatus == "1")
            {
                //READY
                sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.Reference as PostStatus,ST.SASI_Intake as SASI_Intake, 
                            '1' as  StudentCount,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr
                            FROM SAS_AFC AF 
                            INNER JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode
                            INNER JOIN SAS_Program SP ON AF.SAFC_Code=SP.SAFC_Code AND AD.ProgramCode=SP.SAPG_Code
                            INNER JOIN SAS_Accounts SA ON AF.Batchcode=SA.Batchcode
                            INNER JOIN SAS_Student ST ON AF.Semester = ST.SASI_CurSemYr AND AD.ProgramCode=ST.SASI_PgId AND SA.Creditref=ST.SASI_MatricNo
                            WHERE AF.Reference='Ready' 
                            AND AF.SAFC_Code IS NOT NULL  
                            AND  ST.SASI_Reg_Status = 2                             
                            AND SASS_Code IN (SELECT SASS_Code FROM SAS_StudentStatus WHERE SASS_Blstatus  ='true') ";

                if (argEn.BidangCode.Length != 0) sqlCmd += " AND SP.SABP_Code = '" + argEn.BidangCode + "'";
                if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester = '" + argEn.Semester + "'";
                if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code = '" + argEn.Faculty + "'";
                if (argEn.CurrentSemester.Length != 0) sqlCmd += " AND ST.SASI_CurSemYr like '" + argEn.CurrentSemester + "' ";
                if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode='" + argEn.BatchNo + "'";
                sqlCmd += " GROUP BY AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.Reference,ST.SASI_Intake,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr ORDER BY SASI_PgId";
            }
            if (argEn.TransStatus == "-1")
            {
                //STATUS=-1
                if (!string.IsNullOrEmpty(argEn.BatchNo))
                {
                    sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.Reference as PostStatus,ST.SASI_Intake as SASI_Intake, 
                            '1' as  StudentCount,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr
                            FROM SAS_AFC AF 
                            INNER JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode
                            INNER JOIN SAS_Program SP ON AF.SAFC_Code=SP.SAFC_Code AND AD.ProgramCode=SP.SAPG_Code
                            INNER JOIN SAS_Accounts SA ON AF.Batchcode=SA.Batchcode
                            INNER JOIN SAS_Student ST ON AF.Semester = ST.SASI_CurSemYr AND AD.ProgramCode=ST.SASI_PgId AND SA.Creditref=ST.SASI_MatricNo
                            WHERE AF.SAFC_Code IS NOT NULL
                            AND  ST.SASI_Reg_Status = 2                             
                            AND SASS_Code IN (SELECT SASS_Code FROM SAS_StudentStatus WHERE SASS_Blstatus  ='true') ";

                    if (argEn.BidangCode.Length != 0) sqlCmd += " AND SP.SABP_Code = '" + argEn.BidangCode + "'";
                    if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester = '" + argEn.Semester + "'";
                    if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code = '" + argEn.Faculty + "'";
                    if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode = '" + argEn.BatchNo + "'";
                    if (argEn.CurrentSemester.Length != 0) sqlCmd += " AND ST.SASI_CurSemYr like '" + argEn.CurrentSemester + "'";
                    sqlCmd += " GROUP BY AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.Reference,ST.SASI_Intake,AF.BatchCode,SP.SABP_Code,ST.SASI_CurSemYr ORDER BY SASI_PgId";
                }
                else
                {
                    sqlCmd = " SELECT SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                    //if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                    //if (argEn.TransStatus == "1") sqlCmd += "SAS_AFC.Semester as SASI_CurSemYr,";

                    //Updated by Mona - change SASI_CurSemYr to SASI_Intake
                    if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_Intake, ";
                    if (argEn.TransStatus == "1") sqlCmd += "SAS_Student.SASI_Intake as SASI_Intake,";
                    sqlCmd += "count(SAS_Student.SASI_PgId) As StudentCount,case SAS_Student.SASI_PostStatus when '0' then 'Held' when '1' then 'Ready' else 'Posted' end as PostStatus,SAS_AFC.BatchCode,SAS_Program.sabp_code,SAS_Student.SASI_CurSemYr" +
                                   " FROM SAS_Student INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code " +
                                   "left join SAS_Accounts ac on SAS_Student.SASI_MatricNo=ac.CreditRef left outer join " +
                                   " SAS_AFCDetails on SAS_AFCDetails.StudentsNo = SAS_Student.SASI_MatricNo " +
                        //"left outer join SAS_AFC on ac.BatchCode=SAS_AFC.BatchCode and SAS_Student.SASI_CurSemYr=SAS_AFC.Semester " +
                        //Updated by Mona - change SASI_CurSemYr to SASI_Intake
                                   "left outer join SAS_AFC on ac.BatchCode=SAS_AFC.BatchCode and SAS_Student.SASI_Intake=SAS_AFC.Semester " +
                        //" left outer join SAS_FeeStruct on (SAS_FeeStruct.SAPG_Code = SAS_AFC.SAFC_Code and SAS_FeeStruct.SAST_Code = SAS_Student.SASI_CurSemYr) " +
                        //Updated by Mona - change SASI_CurSemYr to SASI_Intake
                                   " left outer join SAS_FeeStruct on (SAS_FeeStruct.SAPG_Code = SAS_AFC.SAFC_Code and SAS_FeeStruct.SAST_Code = SAS_Student.SASI_Intake) " +
                                   " WHERE SAS_Student.SASI_PostStatus =' " + argEn.TransStatus +
                        //"' AND  SAS_Student.SASI_reg_status = 2  AND SAS_Student.SASI_StatusRec = '1' and SAS_Student.SASS_CODE = '01' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true') ";
                                   "' AND SAS_Student.SASI_reg_status = 2 AND SAS_Student.SASI_StatusRec = 'true' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true') ";

                    if (argEn.BidangCode.Length != 0) sqlCmd += " AND SAS_Program.sabp_code like '" + argEn.BidangCode + "'";
                    if (argEn.Faculty.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_Faculty like '" + argEn.Faculty + "'";
                    //if (argEn.ProgramCode.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_PgId like  '" + argEn.ProgramCode + "'";
                    //if (argEn.Semester.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_CurSemYr like  '" + argEn.Semester + "'";
                    //Updated by Mona - change SASI_CurSemYr to SASI_Intake
                    if (argEn.Semester.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_Intake like  '" + argEn.Semester + "'";
                    if (argEn.CurrentSemester.Length != 0) sqlCmd += " AND SAS_Student.SASI_CurSemYr like '" + argEn.CurrentSemester + "'";
                    sqlCmd = sqlCmd + " GROUP BY SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                    //if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                    //Updated by Mona - change SASI_CurSemYr to SASI_Intake
                    if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_Intake, ";
                    if (argEn.TransStatus == "1") sqlCmd += "SAS_Student.SASI_Intake,";
                    sqlCmd += " SAS_AFC.Reference,SAS_Student.SASI_PostStatus,SAS_AFC.BatchCode,SAS_Program.sabp_code,SAS_Student.SASI_CurSemYr order by SAS_Student.SASI_PgId";
                }

            }
            else if (argEn.TransStatus == "0")
            {
                //Modified Mona 8/3/2016
                sqlCmd = "SELECT SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,SAS_Student.SASI_Intake, count(SAS_Student.SASI_PgId) As StudentCount" +
                            ",case SAS_Student.SASI_PostStatus when '0' then 'Held' when '1' then 'Ready' else 'Posted' end as PostStatus, '' as BatchCode,SAS_Program.sabp_code,SAS_Student.SASI_CurSemYr " +
                            "FROM SAS_Student INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code and SAS_Student.SASI_Faculty=SAS_Program.SAFC_Code " +
                            "left outer join SAS_FeeStruct on (SAS_FeeStruct.SAST_Code = SAS_Student.SASI_Intake) " +
                            "WHERE SAS_Student.SASI_PostStatus = '0' AND SAS_Student.SASI_reg_status = 2 AND SAS_Student.SASI_StatusRec = 'true' " +
                            "AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true') ";
                if (argEn.BidangCode.Length != 0) sqlCmd += " AND SAS_Program.sabp_code like '" + argEn.BidangCode + "'";
                if (argEn.Semester.Length != 0) sqlCmd = sqlCmd + " AND SAS_Student.SASI_Intake like  '" + argEn.Semester + "'";
                if (argEn.CurrentSemester.Length != 0) sqlCmd += " AND SAS_Student.SASI_CurSemYr like '" + argEn.CurrentSemester + "'";
                //sqlCmd = sqlCmd + " AND SAS_Student.SASI_CurSemYr IN (select Sast_Code from sas_semestersetup where sast_iscurrentsem = true) " +  commented by Hafiz @ 07/6/2016 
                sqlCmd = sqlCmd + "GROUP BY SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,SAS_Student.SASI_Intake,SAS_Student.SASI_PostStatus,SAS_Program.sabp_code,SAS_Student.SASI_CurSemYr " +
                                  "ORDER BY SAS_Student.SASI_PgId";
            }

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = new ProgramInfoEn();
                            loItem.Program = GetValue<string>(loReader, "SAPG_Program");
                            loItem.Faculty = GetValue<string>(loReader, "SASI_Faculty");
                            loItem.ProgramID = GetValue<string>(loReader, "SASI_PgId");
                            loItem.BatchNo = GetValue<string>(loReader, "BatchCode");
                            //loItem.Semester = GetValue<string>(loReader, "SASI_CurSemYr");
                            loItem.Semester = GetValue<string>(loReader, "SASI_Intake");
                            loItem.TransStatus = GetValue<string>(loReader, "PostStatus");
                            loItem.BidangCode = GetValue<string>(loReader, "sabp_code");
                            string d1, m1, y1;
                            string code = GetValue<string>(loReader, "SASI_Intake");
                            d1 = code.Substring(0, 4);
                            m1 = code.Substring(4, 4);
                            y1 = code.Substring(8, 1);
                            //string semestercode = d1 + "/" + m1 + "-" + y1;
                            loItem.Semester = d1 + "/" + m1 + "-" + y1;
                            loItem.CurrentSemester = GetValue<string>(loReader, "SASI_CurSemYr");
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
        /// Method to Get ProgramInfo Entity
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity is an Input.SAPG_SName as Input Property.</param>
        /// <returns>Returns ProgramInfo Entity</returns>
        public ProgramInfoEn GetItem(ProgramInfoEn argEn)
        {
            ProgramInfoEn loItem = new ProgramInfoEn();
            string sqlCmd = "Select SAPG_Code FROM SAS_Program WHERE  SAPG_SName = " + clsGeneric.AddQuotes(argEn.SAFC_SName);
            
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
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
        /// Method to Insert ProgramInfo 
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity is an Input.</param>
        /// <returns>Returns Boolean</returns>
        public bool Insert(ProgramInfoEn argEn)
        {
            bool lbRes = false;
            int iOut = 0;
            string sqlCmd = "Select count(*) as cnt From SAS_Program WHERE SAPG_Code = @SAPG_Code or SAPG_Program = @SAPG_Program";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmdSel = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@SAPG_Program", DbType.String, argEn.Program);
                    _DbParameterCollection = cmdSel.Parameters;

                    using (IDataReader dr = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmdSel,
                        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader()) 
                    {
                        if (dr.Read())
                            iOut = clsGeneric.NullToInteger(dr["cnt"]);
                        if (iOut > 0)       
                            throw new Exception("Record Already Exist");
                    }
                    if (iOut == 0)
                     {

                         sqlCmd = "INSERT INTO SAS_Program(SAPG_Code,SAPG_Program,SAPG_ProgramType,SAPG_ProgramBM,SAPG_SName,SAPG_SNameBM,SAPG_TotalSem,SAPG_SemByYear," +//SAPG_FieldStudy
                                 " SAPG_Desc,SAFC_Code,SAPG_TI,SAPG_TIDes,SAPG_AD,SAPG_ADDes,SAPG_Status,SABP_Code,SAPG_UpdatedBy,SAPG_UpdatedDtTm) VALUES" +
                                 " (@SAPG_Code,@SAPG_Program,@SAPG_ProgramType,@SAPG_ProgramBM,@SAPG_SName,@SAPG_SNameBM,@SAPG_TotalSem,@SAPG_SemByYear,@SAPG_Desc," +//@SAPG_FieldStudy
                                 " @SAFC_Code,@SAPG_TI,@SAPG_TIDes,@SAPG_AD,@SAPG_ADDes,@SAPG_Status,@SABP_Code,@SAPG_UpdatedBy,@SAPG_UpdatedDtTm) ";

                         if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Program", DbType.String, argEn.Program);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ProgramType", DbType.String, argEn.ProgramType);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ProgramBM", DbType.String, argEn.ProgramBM);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SName", DbType.String, argEn.SName);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SNameBM", DbType.String, string.Empty);                           
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TotalSem", DbType.Int32, argEn.TotalSem);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SemByYear", DbType.Int32, argEn.SemByYear);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Desc", DbType.String, argEn.Description);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAFC_Code", DbType.String, argEn.Code);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TI", DbType.String, argEn.Tutionacc);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TIDes", DbType.String, argEn.TutionDes);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_AD", DbType.String, argEn.Accountinfo);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ADDes", DbType.String, argEn.AccountDes);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Status", DbType.Boolean, argEn.Status);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SABP_Code", DbType.String, argEn.BidangCode);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_UpdatedBy", DbType.String, argEn.UpdatedBy);                          
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_UpdatedDtTm", DbType.String,DateTime.Now.ToString("dd/MM/yyyy"));
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

        #region UpdateStudentFaculty 

        /// <summary>
        /// Method to Update Student Faculty
/// </summary>
        /// <param name="argEn">Students Entity is the Input.MatricNo,Updateby and UpdatedTime are Input Properties</param>
/// <param name="argprogram">ProgramInfo Entity is the Input.Code is the Input Property.</param>
/// <returns></returns>
        public bool UpdateStudentFaculty(StudentEn argEn, ProgramInfoEn argprogram)
        {
            bool lbRes = false;

            string sqlCmd;

            try
            {
                sqlCmd = "UPDATE SAS_Student SET SASI_Faculty = '" + argprogram.Code + "', SASI_UpdatedBy = @UpdatedBy, SASI_UpdatedDtTm = @UpdatedTime WHERE SASI_MatricNo = '" + argEn.MatricNo + "'";

                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmd, "@UpdatedBy", DbType.String, argprogram.UpdatedBy);
                    _DatabaseFactory.AddInParameter(ref cmd, "@UpdatedTime", DbType.DateTime, argprogram.UpdatedDtTm);
                    _DbParameterCollection = cmd.Parameters;

                    int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection); 
                    
                    if (liRowAffected > -1)
                        lbRes = true;
                    else
                        throw new Exception("Update Failed! No Row has been updated...");
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
        /// Method to Update ProgramInfo 
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity is an Input.</param>
        /// <returns>Returns Boolean</returns>
        public bool Update(ProgramInfoEn argEn)
        {
            bool lbRes = false;
            int iOut = 0;
            //string sqlCmd = "Select count(*) as cnt From SAS_Program WHERE SAPG_Code != @SAPG_Code and SAPG_Program = @SAPG_Program";
            try
            {
                //if (!FormHelp.IsBlank(sqlCmd))
                //{
                //    DbCommand cmdSel = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                //    _DatabaseFactory.AddInParameter(ref cmdSel, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                //    _DatabaseFactory.AddInParameter(ref cmdSel, "@SAPG_Program", DbType.String, argEn.Program);
                //    _DbParameterCollection = cmdSel.Parameters;

                //    using (IDataReader dr = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmdSel,
                //        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader())
                //    {
                //        if (dr.Read())
                //            iOut = clsGeneric.NullToInteger(dr["cnt"]);
                //        if (iOut > 0)
                //            throw new Exception("Record Already Exist");
                //    }
                //    if (iOut == 0)
                //    {
                        StudentDAL lostuds = new StudentDAL();
                        List<StudentEn> loEnList = new List<StudentEn>();
                        string sqlCmdst = "select * from SAS_Student where SASI_PGID ='" + argEn.ProgramCode + "'";

                        if (!FormHelp.IsBlank(sqlCmdst))
                        {
                            using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                                DataBaseConnectionString, sqlCmdst).CreateDataReader())
                            {
                                while (loReader.Read())
                                {

                                    StudentEn loItem = lostuds.LoadObject(loReader);
                                    loEnList.Add(loItem);
                                }
                                loReader.Close();
                            }
                        }
                        int i = 0;
                        if (loEnList.Count > 0)
                        {
                            for (i = 0; i < loEnList.Count; i++)
                            {
                                UpdateStudentFaculty(loEnList[i], argEn);
                            }
                        }

                        string sqlCmd = "UPDATE SAS_Program SET SAPG_Code = @SAPG_Code, SAPG_Program = @SAPG_Program,SAPG_ProgramType = @SAPG_ProgramType,SAPG_ProgramBM = @SAPG_ProgramBM," +// SAPG_FieldStudy = @SAPG_FieldStudy,
                                 " SAPG_SName = @SAPG_SName, SAPG_SNameBM = @SAPG_SNameBM, SAPG_TotalSem = @SAPG_TotalSem, SAPG_SemByYear = @SAPG_SemByYear," +
                                  " SAPG_Desc = @SAPG_Desc, SAFC_Code = @SAFC_Code, SAPG_Status = @SAPG_Status, SABP_Code = @SABP_Code, SAPG_UpdatedBy = @SAPG_UpdatedBy," +
                                  " SAPG_TI = @SAPG_TI,SAPG_TIDes = @SAPG_TIDes,SAPG_AD = @SAPG_AD,SAPG_ADDes = @SAPG_ADDes," +
                                  " SAPG_UpdatedDtTm = @SAPG_UpdatedDtTm WHERE SAPG_Code = @SAPG_Code ";

                        if (!FormHelp.IsBlank(sqlCmd))
                        {
                            DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Program", DbType.String, argEn.Program);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ProgramType", DbType.String, argEn.ProgramType);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ProgramBM", DbType.String, argEn.ProgramBM);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SName", DbType.String, argEn.SName);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SNameBM", DbType.String, argEn.SNameBM);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TotalSem", DbType.Int32, argEn.TotalSem);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_SemByYear", DbType.Int32, argEn.SemByYear);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Desc", DbType.String, argEn.Description);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAFC_Code", DbType.String, argEn.Code);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TI", DbType.String, argEn.Tutionacc);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_TIDes", DbType.String, argEn.TutionDes);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_AD", DbType.String, argEn.Accountinfo);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_ADDes", DbType.String, argEn.AccountDes);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Status", DbType.Boolean, argEn.Status);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SABP_Code", DbType.String, argEn.BidangCode);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_UpdatedBy", DbType.String, argEn.UpdatedBy);
                            _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_UpdatedDtTm", DbType.String, argEn.UpdatedDtTm);
                          //  _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_UpdatedDtTm", DbType.String, DateTime.Now.ToString("dd/MM/yyyy"));
                            _DbParameterCollection = cmd.Parameters;

                            int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);
                            
                            if (liRowAffected > -1)
                                lbRes = true;
                            else
                                throw new Exception("Update Failed! No Row has been updated...");
                        }
                //    }
                //}
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
        /// Method to Delete ProgramInfo 
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity is an Input.</param>
        /// <returns>Returns Boolean</returns>
        public bool Delete(ProgramInfoEn argEn)
        {

            bool lbRes = false;

            int total = 0;
            string sqlCmd = "select sum(rows) as total from (select count(*) as rows  from SAS_FeeStruct WHERE SAPG_Code = @SAPG_Code  union all select count(*) as rows from SAS_Student WHERE SASI_PgId = @SAPG_Code)AS U";
            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    DbCommand cmdSel = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                    _DatabaseFactory.AddInParameter(ref cmdSel, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                    _DbParameterCollection = cmdSel.Parameters;

                    using (IDataReader dr = _DatabaseFactory.GetIDataReader(Helper.GetDataBaseType, cmdSel,
                        DataBaseConnectionString, sqlCmd, _DbParameterCollection).CreateDataReader()) 
                    {
                        if (dr.Read())
                            total = clsGeneric.NullToInteger(dr["total"]);
                        if (total > 0)
                            throw new Exception("Record Already in Use");
                    }
                        if (total == 0)
                        {
                            sqlCmd = "DELETE FROM SAS_Program WHERE SAPG_Code = @SAPG_Code ";

                            if (!FormHelp.IsBlank(sqlCmd))
                            {
                                DbCommand cmd = _DatabaseFactory.GetDbCommand(Helper.GetDataBaseType, sqlCmd, DataBaseConnectionString);
                                _DatabaseFactory.AddInParameter(ref cmd, "@SAPG_Code", DbType.String, argEn.ProgramCode);
                                _DbParameterCollection = cmd.Parameters;

                                int liRowAffected = _DatabaseFactory.ExecuteNonQuery(Helper.GetDataBaseType, cmd,
                                DataBaseConnectionString, sqlCmd, _DbParameterCollection);
                                
                                if (liRowAffected > -1)
                                    lbRes = true;
                                else
                                    throw new Exception("Delete Failed! No Row has been deleted...");
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

        /// <summary>
        /// Method to Load ProgramInfo Entity 
        /// </summary>
        /// <param name="argReader">IDataReader Object is an Input.</param>
        /// <returns>Returns ProgramInfo Entity</returns>
        private ProgramInfoEn LoadObject(IDataReader argReader)
        {
            ProgramInfoEn loItem = new ProgramInfoEn();
            loItem.ProgramCode = GetValue<string>(argReader, "SAPG_Code");
            loItem.Program = GetValue<string>(argReader, "SAPG_Program");
            loItem.ProgramType = GetValue<string>(argReader, "SAPG_ProgramType");
            loItem.ProgramBM = GetValue<string>(argReader, "SAPG_ProgramBM");
            loItem.SName = GetValue<string>(argReader, "SAPG_SName");
            loItem.TotalSem = GetValue<int>(argReader, "SAPG_TotalSem");
            loItem.SemByYear = GetValue<int>(argReader, "SAPG_SemByYear");
            loItem.Description = GetValue<string>(argReader, "SAPG_Desc");
            loItem.Code = GetValue<string>(argReader, "SAFC_Code");
            loItem.Tutionacc = GetValue<string>(argReader, "SAPG_TI");
            loItem.TutionDes = GetValue<string>(argReader, "SAPG_TIDes");
            loItem.Accountinfo = GetValue<string>(argReader, "SAPG_AD");
            loItem.AccountDes = GetValue<string>(argReader, "SAPG_ADDes");
            loItem.Status = GetValue<bool>(argReader, "SAPG_Status");
            loItem.UpdatedBy = GetValue<string>(argReader, "SAPG_UpdatedBy");
            loItem.UpdatedDtTm = GetValue<string>(argReader, "SAPG_UpdatedDtTm");
            loItem.CodeProgram = loItem.ProgramCode + "-" + loItem.ProgramBM;
            loItem.BidangCode = GetValue<string>(argReader, "SABP_Code");            

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

        #region GetAfcProgramsByBidang

        public List<ProgramInfoEn> GetAfcProgramsByBidang(ProgramInfoEn argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            argEn.Faculty = argEn.Faculty.Replace("*", "%");
            //argEn.ProgramCode = argEn.ProgramCode.Replace("*", "%");
            argEn.Semester = argEn.Semester.Replace("*", "%");
            string sqlCmd = string.Empty;
            if (argEn.TransStatus == "2")
            {
                sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.Reference as PostStatus,AF.Semester as SASI_CurSemYr, '1' as  StudentCount,AF.BatchCode
                            FROM SAS_AFC AF 
                            LEFT JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode 
                            LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode
                            LEFT JOIN SAS_AccountsDetails ACD ON AC.TransCode=ACD.TransCode
                            LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo
                            LEFT JOIN SAS_Program SP ON AD.ProgramCode=SP.SAPG_Code
                            WHERE AF.Reference='Posted' and AF.SAFC_Code IS NOT NULL 
                                AND  ST.SASI_reg_status = 2                             
                             AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus  ='true')
                           ";
                if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester='" + argEn.Semester + "'";
                if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code='" + argEn.Faculty + "'";
                if (argEn.ProgramCode.Length != 0) sqlCmd += " AND AD.ProgramCode='" + argEn.ProgramCode + "'";
                if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode='" + argEn.BatchNo + "'";
                sqlCmd += " Group by  AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.Reference,AF.Semester,AF.BatchCode order by AD.ProgramCode";
            }
            if (argEn.TransStatus == "1")
            {
                sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.BatchCode,AF.Reference as PostStatus,AF.Semester as SASI_CurSemYr, '1' as  StudentCount
                            FROM SAS_AFC AF 
                            LEFT JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode 
                            LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode
                            LEFT JOIN SAS_AccountsDetails ACD ON AC.TransCode=ACD.TransCode
                            LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo
                            LEFT JOIN SAS_Program SP ON AD.ProgramCode=SP.SAPG_Code
                            WHERE AF.Reference='Ready' and AF.SAFC_Code IS NOT NULL  
                            AND  ST.SASI_reg_status = 2
                           AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus  ='true')";

                if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester='" + argEn.Semester + "'";
                if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code='" + argEn.Faculty + "'";
                if (argEn.ProgramCode.Length != 0) sqlCmd += " AND AD.ProgramCode='" + argEn.ProgramCode + "'";
                if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode='" + argEn.BatchNo + "'";
                sqlCmd += " Group by  AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.BatchCode,AF.Reference,AF.Semester order by AD.ProgramCode";

            }
            if (argEn.TransStatus == "-1")
            {
                if (!string.IsNullOrEmpty(argEn.BatchNo))
                {
                    sqlCmd = @" SELECT AF.SAFC_Code as SASI_Faculty,SP.SAPG_Program,AD.ProgramCode as SASI_PgId,AF.BatchCode,AF.Reference as PostStatus,AF.Semester as SASI_CurSemYr, '1' as  StudentCount
                            FROM SAS_AFC AF 
                            LEFT JOIN SAS_AFCDetails AD ON AF.TransCode=AD.TransCode 
                            LEFT JOIN SAS_Accounts AC ON AF.BatchCode=AC.BatchCode
                            LEFT JOIN SAS_AccountsDetails ACD ON AC.TransCode=ACD.TransCode
                            LEFT JOIN SAS_Student ST ON AC.CreditRef=ST.SASI_MatricNo
                            LEFT JOIN SAS_Program SP ON AD.ProgramCode=SP.SAPG_Code
                            WHERE AF.SAFC_Code IS NOT NULL
                            AND  ST.SASI_reg_status = 1
                           AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus  ='true')";

                    if (argEn.Semester.Length != 0) sqlCmd += " AND AF.Semester='" + argEn.Semester + "'";
                    if (argEn.Faculty.Length != 0) sqlCmd += " AND AF.SAFC_Code='" + argEn.Faculty + "'";
                    if (argEn.ProgramCode.Length != 0) sqlCmd += " AND AD.ProgramCode='" + argEn.ProgramCode + "'";
                    if (argEn.BatchNo.Length != 0) sqlCmd += " AND AF.BatchCode='" + argEn.BatchNo + "'";
                    sqlCmd += " Group by  AF.SAFC_Code,SP.SAPG_Program,AD.ProgramCode,AF.BatchCode,AF.Reference,AF.Semester order by AD.ProgramCode";
                }
                else
                {
                    sqlCmd = " SELECT SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                    if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                    if (argEn.TransStatus == "1") sqlCmd += "SAS_AFC.Semester as SASI_CurSemYr,";
                    sqlCmd += "count(SAS_Student.SASI_PgId) As StudentCount,case SAS_Student.SASI_PostStatus when '0' then 'Held' when '1' then 'Ready' else 'Posted' end as PostStatus,SAS_AFC.BatchCode" +
                                   " FROM SAS_Student INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code " +
                                   "left join SAS_Accounts ac on SAS_Student.SASI_MatricNo=ac.CreditRef left outer join " +
                                   " SAS_AFCDetails on SAS_AFCDetails.StudentsNo = SAS_Student.SASI_MatricNo " +
                                   "left outer join SAS_AFC on ac.BatchCode=SAS_AFC.BatchCode and SAS_Student.SASI_CurSemYr=SAS_AFC.Semester " +
                                   " left outer join SAS_FeeStruct on (SAS_FeeStruct.SAPG_Code = SAS_AFC.SAFC_Code and SAS_FeeStruct.SAST_Code = SAS_Student.SASI_CurSemYr) " +
                                   " WHERE SAS_Student.SASI_PostStatus =' " + argEn.TransStatus +
                        //"' AND  SAS_Student.SASI_reg_status = 2  AND SAS_Student.SASI_StatusRec = '1' and SAS_Student.SASS_CODE = '01' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true') ";
                                   "' AND  SAS_Student.SASI_reg_status = 2  AND SAS_Student.SASI_StatusRec = 'true' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true') ";
                    if (argEn.Faculty.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_Faculty like '" + argEn.Faculty + "'";
                    if (argEn.ProgramCode.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_PgId like  '" + argEn.ProgramCode + "'";
                    if (argEn.Semester.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_CurSemYr like  '" + argEn.Semester + "'";
                    sqlCmd = sqlCmd + " GROUP BY SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                    if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                    if (argEn.TransStatus == "1") sqlCmd += "SAS_AFC.Semester,";
                    sqlCmd += " SAS_AFC.Reference,SAS_Student.SASI_PostStatus,SAS_AFC.BatchCode order by SAS_Student.SASI_PgId";
                }

            }
            else if (argEn.TransStatus == "0")
            { 
                //sqlCmd = "SELECT SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                sqlCmd = "SELECT distinct on (1,2,3,4) SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                if (argEn.TransStatus == "1") sqlCmd += "SAS_AFC.Semester as SASI_CurSemYr,";
                sqlCmd += "count(SAS_Student.SASI_PgId) As StudentCount,case SAS_Student.SASI_PostStatus when '0' then 'Held' when '1' then 'Ready' else 'Posted' end as PostStatus,SAS_AFC.BatchCode" +
                               " FROM SAS_Student INNER JOIN SAS_Program ON SAS_Student.SASI_PgId = SAS_Program.SAPG_Code and SAS_Student.SASI_Faculty=SAS_Program.SAFC_Code " +
                               "left join SAS_Accounts ac on SAS_Student.SASI_MatricNo=ac.CreditRef left outer join " +
                               " SAS_AFCDetails on SAS_AFCDetails.StudentsNo = SAS_Student.SASI_MatricNo " +
                               "left outer join SAS_AFC on ac.BatchCode=SAS_AFC.BatchCode and SAS_Student.SASI_CurSemYr=SAS_AFC.Semester " +
                               " left outer join SAS_FeeStruct on (SAS_FeeStruct.SAPG_Code = SAS_AFC.SAFC_Code and SAS_FeeStruct.SAST_Code = SAS_Student.SASI_CurSemYr) " +
                               " WHERE SAS_Student.SASI_PostStatus = '" + argEn.TransStatus +

                               //"'  AND  SAS_Student.SASI_reg_status = 2 AND SAS_Student.SASI_StatusRec = '1' and SAS_Student.SASS_CODE = '01' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true')";
                               "'  AND  SAS_Student.SASI_reg_status = 2 AND SAS_Student.SASI_StatusRec = 'true' AND sass_code IN (SELECT sass_code FROM sas_studentstatus where sass_blstatus ='true')";
                if (argEn.Faculty.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_Faculty like '" + argEn.Faculty + "'";
                if (argEn.ProgramCode.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_PgId like  '" + argEn.ProgramCode + "'";
                if (argEn.Semester.Length != 0) sqlCmd = sqlCmd + " and SAS_Student.SASI_CurSemYr like  '" + argEn.Semester + "'";
                sqlCmd = sqlCmd + " GROUP BY SAS_Student.SASI_Faculty, SAS_Program.SAPG_Program, SAS_Student.SASI_PgId,";
                if (argEn.TransStatus == "0") sqlCmd += "SAS_Student.SASI_CurSemYr, ";
                if (argEn.TransStatus == "1") sqlCmd += "SAS_AFC.Semester,";
                sqlCmd += " SAS_AFC.Reference,SAS_Student.SASI_PostStatus,SAS_AFC.BatchCode Order by SAS_Student.SASI_PgId";
            }

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = new ProgramInfoEn();
                            loItem.Program = GetValue<string>(loReader, "SAPG_Program");
                            loItem.Faculty = GetValue<string>(loReader, "SASI_Faculty");
                            loItem.ProgramID = GetValue<string>(loReader, "SASI_PgId");
                            loItem.BatchNo = GetValue<string>(loReader, "BatchCode");
                            loItem.Semester = GetValue<string>(loReader, "SASI_CurSemYr");
                            loItem.TransStatus = GetValue<string>(loReader, "PostStatus");
                            //if (loItem.TransStatus == "Ready" || loItem.TransStatus == "Posted")
                            //{
                            //    loItem.TransCode = GetValue<int>(loReader, "TransCode");
                            //}
                            //else
                            //{
                            //    loItem.TransCode = 0;
                            //}                            
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

        #region GetAllProgramInfoList

        /// <summary>
        /// Method to Get List of ProgramInfo by SAFC_Code
        /// </summary>
        /// <param name="argEn">ProgramInfo Entity as an Input.SAFC_Code  as Input Property.</param>
        /// <returns>Returns List of ProgramInfo</returns>
        public List<ProgramInfoEn> GetAllProgramInfoList(string argEn)
        {
            List<ProgramInfoEn> loEnList = new List<ProgramInfoEn>();
            string sqlCmd = "select * from SAS_Program";

            try
            {
                if (!FormHelp.IsBlank(sqlCmd))
                {
                    using (IDataReader loReader = _DatabaseFactory.ExecuteReader(Helper.GetDataBaseType,
                        DataBaseConnectionString, sqlCmd).CreateDataReader())
                    {
                        while (loReader.Read())
                        {
                            ProgramInfoEn loItem = LoadObject(loReader);
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
    }

}
