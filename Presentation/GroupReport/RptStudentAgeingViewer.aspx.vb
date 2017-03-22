Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports HTS.SAS.Entities
Imports HTS.SAS.DataAccessObjects
Imports System.Linq
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Collections.Generic
Imports CrystalDecisions.Shared

Partial Class RptStudentAgeingViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument

#Region "Page Load Starting  "

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'Modified by Hafiz @ 08/11/2016 for NEW RA

        Try
            If Not IsPostBack Then

                Dim str As String = Nothing
                Dim ReportPath As String = Nothing, _ReportHelper As New ReportHelper

                If Request.QueryString("Report") = "1" Then

                    'Ageing Report Based On Student Matric ID - START
                    Dim FeeType As String = Request.QueryString("FeeType")
                    Dim Status As String = Request.QueryString("Status")
                    Dim Info As String = Request.QueryString("Info")
                    Dim AgeingBy As String = Request.QueryString("AgeingBy")
                    Dim DateTo As String = Nothing
                    Dim DateFrom As String = Nothing
                    Dim ByDate As String = Request.QueryString("ByDate")
                    Dim d2, m2, y2 As String

                    Dim constr As String() = Info.Split(";")

                    d2 = Mid(ByDate, 1, 2)
                    m2 = Mid(ByDate, 4, 2)
                    y2 = Mid(ByDate, 7, 4)
                    DateTo = y2 + "/" + m2 + "/" + d2
                    DateFrom = "2013/01/01"

                    Dim StudentStatus As String = Nothing, FilterStatus As String = Nothing, SumQuery As String = Nothing
                    If Status <> "-1" Then
                        StudentStatus = "AND EXISTS(SELECT 1 FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef AND SASS_Code = '" + Status + "') "
                        'FilterStatus = "AND SS.SASS_Code = '" + Status + "' "
                        'StatusQuery = "LEFT JOIN SAS_Studentstatus SS ON SS.SASS_Code IN (SELECT SASS_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) "
                    End If

                    Dim Header_FT As String = Nothing, FT_join As String = Nothing, FT_filter As String = Nothing

                    Dim paramField As ParameterField
                    Dim paramFields As ParameterFields
                    Dim paramDiscreteValue As ParameterDiscreteValue
                    Dim columnNo As Integer = 0

                    paramFields = New ParameterFields()

                    Dim Header As String = "SELECT ageing.CreditRef as MatricNo,"

                    For i As Integer = 0 To constr.Length - 1
                        If constr(i) <> "" Then

                            If constr(i).Equals("program") Then
                                columnNo = columnNo + 1

                                Header += "(SELECT SAPG_Code || ' - ' || SAPG_ProgramBM FROM SAS_Program WHERE SAPG_Code IN "
                                Header += "(SELECT SASI_PgId FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef)) AS col" + columnNo.ToString() + ","

                                paramField = New ParameterField()
                                paramField.Name = "col_" + columnNo.ToString()
                                paramDiscreteValue = New ParameterDiscreteValue()
                                paramDiscreteValue.Value = "PROGRAM"
                                paramField.CurrentValues.Add(paramDiscreteValue)

                                paramFields.Add(paramField)
                            End If

                        End If
                    Next

                    If Not String.IsNullOrEmpty(FeeType) Then
                        columnNo = columnNo + 1

                        Header_FT = "COALESCE(SFGL.GL_account,SKGL.GL_account) AS col" + columnNo.ToString() + ","

                        FT_join = "LEFT JOIN SAS_Faculty_GLaccount SFGL ON SFGL.SAFT_Code = ageing.RefCode AND SFGL.SAFC_Code = "
                        FT_join += "(SELECT SASI_Faculty FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) "
                        FT_join += "LEFT JOIN SAS_Kolej_glaccount SKGL ON SKGL.SAFT_Code = ageing.RefCode AND SKGL.Sako_code = "
                        FT_join += "(SELECT SAKO_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) "

                        FT_filter = "AND ageing.RefCode = '" & FeeType & "' "

                        paramField = New ParameterField()
                        paramField.Name = "col_" + columnNo.ToString()
                        paramDiscreteValue = New ParameterDiscreteValue()
                        paramDiscreteValue.Value = "TABUNG"
                        paramField.CurrentValues.Add(paramDiscreteValue)

                        paramFields.Add(paramField)
                    End If

                    For i As Integer = columnNo To 2 - 1
                        columnNo = columnNo + 1
                        paramField = New ParameterField()
                        paramField.Name = "col_" + columnNo.ToString()
                        paramDiscreteValue = New ParameterDiscreteValue()
                        paramDiscreteValue.Value = ""
                        paramField.CurrentValues.Add(paramDiscreteValue)

                        paramFields.Add(paramField)
                    Next i

                    CrystalReportViewer1.ParameterFieldInfo = paramFields

                    'QUERY BUILDER - START
                    If Not String.IsNullOrEmpty(FeeType) Then
                        If AgeingBy = "rbMonthly" Then
                            ReportPath = "~/GroupReport/RptStudentAgeingType1a_Month.rpt"
                        Else
                            ReportPath = "~/GroupReport/RptStudentAgeingType1a.rpt"
                        End If

                        str = Header
                        str += Header_FT
                        str += QueryBuilder(AgeingBy, "ageing", "HEAD")
                        str += "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo "
                        str += "FROM ( "
                        str += "SELECT a.CreditRef,SAD.RefCode,"
                        str += LoadAgeingByQuery("FT", AgeingBy, DateTo)
                        str += "FROM ( "
                        str += "WITH REC AS (SELECT a.INV_NO,a.TransCode,a.RefCode, " +
                                "CASE WHEN SAS_Accounts.TransDate > '" + DateTo + "'::date THEN " +
                                "SUM(a.PaidAmount) " +
                                "ELSE 0 END AS PAID_AMT " +
                                "FROM ( " +
                                "SELECT SA.CreditRef,SAD.RefCode,SAD.INV_NO,SAD.TransCode,SAD.PaidAmount " +
                                "FROM SAS_Accounts SA " +
                                "INNER JOIN SAS_AccountsDetails SAD ON SAD.INV_NO=SA.TransCode " +
                                "WHERE SA.TransType='Debit' " +
                                "AND SA.PostStatus='Posted' ) a " +
                                "INNER JOIN SAS_Accounts ON SAS_Accounts.TransCode=a.TransCode AND SAS_Accounts.Creditref=a.CreditRef " +
                                "GROUP BY a.INV_NO,a.TransCode,a.RefCode,SAS_Accounts.TransDate " +
                                "ORDER BY a.INV_NO ) "
                        str += "SELECT DISTINCT SA.CreditRef,SA.TransCode,SA.TransDate,REC.RefCode,"
                        str += "COALESCE(SUM(REC.PAID_AMT),0) AS PAID_AMT "
                        str += "FROM SAS_Accounts SA "
                        str += "LEFT JOIN REC ON REC.INV_NO=SA.TransCode "
                        str += "WHERE SA.TransAmount > 0"
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.TransStatus <> 'Closed' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SA.TransId,REC.RefCode ) a "
                        str += "INNER JOIN SAS_Accounts SA ON SA.TransCode=a.TransCode AND SA.CreditRef=a.CreditRef "
                        str += "INNER JOIN SAS_AccountsDetails SAD ON SAD.TransCode=a.TransCode "
                        str += " WHERE SA.TransDate <= '" + DateTo + "'"
                        str += "GROUP BY a.CreditRef,SA.TransDate,SAD.RefCode,SAD.TransAmount "
                        str += ") ageing "
                        str += FT_join
                        str += "WHERE ageing.CreditRef <> '' "
                        str += StudentStatus
                        str += FT_filter
                        str += "GROUP BY ageing.CreditRef,SFGL.GL_account,SKGL.GL_account "
                        str += "ORDER BY ageing.CreditRef"
                    Else
                        If AgeingBy = "rbMonthly" Then
                            ReportPath = "~/GroupReport/RptStudentAgeingType1b_Month.rpt"
                        Else
                            ReportPath = "~/GroupReport/RptStudentAgeingType1b.rpt"
                        End If

                        str = Header
                        str += QueryBuilder(AgeingBy, "ageing", "HEAD")
                        str += "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo "
                        str += "FROM ( "
                        str += "SELECT a.CreditRef,"
                        str += LoadAgeingByQuery("nonFT", AgeingBy, DateTo)
                        str += "FROM ( "
                        str += "WITH REC AS (SELECT a.INV_NO,a.TransCode, "
                        str += "CASE WHEN SAS_Accounts.TransDate > '" + DateTo + "'::date THEN "
                        str += "CASE WHEN SUM(a.PaidAmount) < SAS_Accounts.TransAmount THEN SUM(a.PaidAmount) "
                        str += "ELSE SAS_Accounts.TransAmount END "
                        str += "ELSE 0 END AS PAID_AMT "
                        str += "FROM ("
                        str += "SELECT SA.CreditRef,SAD.INV_NO,SAD.TransCode,SAD.PaidAmount "
                        str += "FROM SAS_Accounts SA "
                        str += "INNER JOIN SAS_AccountsDetails SAD ON SAD.INV_NO=SA.TransCode "
                        str += "WHERE SA.TransType='Debit' "
                        str += "AND SA.PostStatus='Posted') a "
                        str += "INNER JOIN SAS_Accounts ON SAS_Accounts.TransCode=a.TransCode AND SAS_Accounts.Creditref=a.CreditRef "
                        str += "GROUP BY a.INV_NO,a.TransCode,a.PaidAmount,SAS_Accounts.TransAmount,SAS_Accounts.TransDate"
                        str += ")"
                        str += "SELECT DISTINCT SA.CreditRef,SA.TransCode,COALESCE(SUM(REC.PAID_AMT),0) AS PAID_AMT "
                        str += "FROM SAS_Accounts SA "
                        str += "LEFT JOIN REC ON REC.INV_NO=SA.TransCode "
                        str += "WHERE SA.TransAmount > 0 "
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.TransStatus <> 'Closed' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SA.TransId) a "
                        str += "INNER JOIN SAS_Accounts SA ON SA.TransCode=a.TransCode AND SA.CreditRef=a.CreditRef "
                        str += " WHERE SA.TransDate <= '" + DateTo + "'"
                        str += "GROUP BY a.CreditRef,a.TransCode,SA.TransDate,SA.TransAmount,SA.PaidAmount,a.PAID_AMT"
                        str += ") ageing "
                        str += "WHERE ageing.CreditRef <> '' "
                        str += StudentStatus
                        str += "GROUP BY ageing.CreditRef "
                        str += "ORDER BY ageing.CreditRef "
                    End If
                    'QUERY BUILDER - END

                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    _DataSet.Tables(0).TableName = "Table"

                    Dim s As String = Nothing
                    If AgeingBy = "rbMonthly" Then
                        s = Server.MapPath("~/xml/StudentAgeingType1_Month.xml")
                    Else
                        s = Server.MapPath("~/xml/StudentAgeingType1.xml")
                    End If
                    _DataSet.WriteXml(s)

                    'custom header - start
                    Dim CheckboxType As String = AgeingBy
                    Dim HeaderField As String = Nothing
                    Dim Dt As Date = CDate(CStr(DateTo))
                    Dim FormattedDate As String = Format(Dt, "dd/MM/yyyy")

                    If Not String.IsNullOrEmpty(FeeType) Then
                        Dim argEn As FeeTypesEn = New FeeTypesDAL().GetItem(New FeeTypesEn With {.FeeTypeCode = Trim(FeeType)})
                        If argEn.FeeType = "01" Then
                            HeaderField = "Penyata Hutang Yuran Kredit Pelajar Setakat " + FormattedDate + " Universiti Putra Malaysia"
                        Else
                            HeaderField = "Penyata Hutang Pelajar Setakat " + FormattedDate + " Universiti Putra Malaysia"
                        End If
                    Else
                        HeaderField = "Penyata Hutang Pelajar Setakat " + FormattedDate + " Universiti Putra Malaysia"
                    End If
                    'custom header - end

                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")
                    Else
                        MyReportDocument.Load(Server.MapPath(ReportPath))
                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        'pass params to formula fields - start
                        MyReportDocument.DataDefinition.FormulaFields("HeaderField").Text = "'" & HeaderField & "'"
                        MyReportDocument.DataDefinition.FormulaFields("CBType").Text = "'" & CheckboxType & "'"
                        'pass params to formula fields - end
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()
                    End If
                    'Ageing Report Based On Student Matric ID - END

                ElseIf Request.QueryString("Report") = "2" Then

                    'Details Ageing Report - START 
                    Dim Status As String = Request.QueryString("Status")
                    Dim Nationality As String = Request.QueryString("Nationality")
                    Dim Faculty As String = Request.QueryString("Faculty")
                    Dim Sponsor As String = Request.QueryString("Sponsor")
                    Dim ByDate As String = Request.QueryString("ByDate")
                    Dim AgeingBy As String = Request.QueryString("AgeingBy")

                    Dim DateTo As String = Nothing
                    Dim d2, m2, y2 As String

                    d2 = Mid(ByDate, 1, 2)
                    m2 = Mid(ByDate, 4, 2)
                    y2 = Mid(ByDate, 7, 4)
                    DateTo = y2 + "/" + m2 + "/" + d2

                    Dim StatusStr As String = Nothing, NationalityStr As String = Nothing, FacultyStr As String = Nothing, SponsorStr As String = Nothing

                    'Status
                    If Status <> "-1" Then
                        StatusStr = "AND EXISTS(SELECT 1 FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef AND SASS_Code = '" + Status + "') "
                    End If

                    'Nationality
                    If Nationality <> "-1" Then
                        NationalityStr = "AND EXISTS(SELECT 1 FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef AND SASC_Code ='" + IIf(Nationality = "1", "W", "BW") + "') "
                    End If

                    'Faculty
                    If Faculty <> "-1" Then
                        FacultyStr = "AND EXISTS(SELECT 1 FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef AND SASI_Faculty='" + Faculty + "') "
                    End If

                    'Sponsor
                    If Sponsor <> "-1" Then
                        SponsorStr = "AND STS.SASS_Sponsor = '" + Sponsor + "' "
                    End If

                    'QUERY BUILDER - START
                    If AgeingBy = "rbMonthly" Then
                        ReportPath = "~/GroupReport/RptStudentAgeingType2_Month.rpt"
                    Else
                        ReportPath = "~/GroupReport/RptStudentAgeingType2.rpt"
                    End If

                    str = "SELECT ageing.CreditRef as MatricNo,"
                    str += "(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code IN (SELECT SASI_Faculty FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef)) AS FACULTY,"
                    str += "(SELECT SASR_Name FROM SAS_Sponsor WHERE SASR_Code=ageing.SASS_Sponsor) AS SPONSOR,"
                    str += "(SELECT SASS_Description FROM SAS_StudentStatus WHERE SASS_Code=(SELECT SASS_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef)) AS STATUS,"
                    str += "CASE WHEN (SELECT SASC_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) = 'W' THEN 'LOCAL' "
                    str += "WHEN (SELECT SASC_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) = 'BW' THEN 'FOREIGNER' "
                    str += "END AS CATEGORY,"
                    str += QueryBuilder(AgeingBy, "ageing", "HEAD")
                    str += "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo "
                    str += "FROM ( "
                    str += "SELECT a.CreditRef,STS.SASS_Sponsor,"
                    str += LoadAgeingByQuery("nonFT", AgeingBy, DateTo)
                    str += "FROM ( "
                    str += "WITH REC AS (SELECT a.INV_NO,a.TransCode, "
                    str += "CASE WHEN SAS_Accounts.TransDate > '" + DateTo + "'::date THEN "
                    str += "CASE WHEN SUM(a.PaidAmount) < SAS_Accounts.TransAmount THEN SUM(a.PaidAmount) "
                    str += "ELSE SAS_Accounts.TransAmount END "
                    str += "ELSE 0 END AS PAID_AMT "
                    str += "FROM ("
                    str += "SELECT SA.CreditRef,SAD.INV_NO,SAD.TransCode,SAD.PaidAmount "
                    str += "FROM SAS_Accounts SA "
                    str += "INNER JOIN SAS_AccountsDetails SAD ON SAD.INV_NO=SA.TransCode "
                    str += "WHERE SA.TransType='Debit' "
                    str += "AND SA.PostStatus='Posted') a "
                    str += "INNER JOIN SAS_Accounts ON SAS_Accounts.TransCode=a.TransCode AND SAS_Accounts.Creditref=a.CreditRef "
                    str += "GROUP BY a.INV_NO,a.TransCode,a.PaidAmount,SAS_Accounts.TransAmount,SAS_Accounts.TransDate"
                    str += ")"
                    str += "SELECT DISTINCT SA.CreditRef,SA.TransCode,COALESCE(SUM(REC.PAID_AMT),0) AS PAID_AMT "
                    str += "FROM SAS_Accounts SA "
                    str += "LEFT JOIN REC ON REC.INV_NO=SA.TransCode "
                    str += "WHERE SA.TransAmount > 0 "
                    str += "AND SA.SubType <> 'Sponsor' "
                    str += "AND SA.Category <> 'Payment' "
                    str += "AND SA.TransType <> 'Credit' "
                    str += "AND SA.TransStatus <> 'Closed' "
                    str += "AND SA.PostStatus = 'Posted' "
                    str += "GROUP BY SA.TransId) a "
                    str += "INNER JOIN SAS_Accounts SA ON SA.TransCode=a.TransCode AND SA.CreditRef=a.CreditRef "
                    str += "INNER JOIN SAS_StudentSpon STS ON STS.SASI_MatricNo=a.CreditRef "
                    str += "WHERE SA.TransDate <= '" + DateTo + "' "
                    str += SponsorStr
                    str += "GROUP BY a.CreditRef,a.TransCode,SA.TransDate,SA.TransAmount,SA.PaidAmount,STS.SASS_Sponsor,a.PAID_AMT"
                    str += ") ageing "
                    str += "WHERE ageing.CreditRef <> '' "
                    str += StatusStr
                    str += NationalityStr
                    str += FacultyStr
                    str += "GROUP BY ageing.CreditRef,ageing.SASS_Sponsor "
                    str += "ORDER BY ageing.CreditRef "
                    'QUERY BUILDER - END

                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    _DataSet.Tables(0).TableName = "Table"

                    Dim s As String = Nothing
                    If AgeingBy = "rbMonthly" Then
                        s = Server.MapPath("~/xml/StudentAgeingType2_Month.xml")
                    Else
                        s = Server.MapPath("~/xml/StudentAgeingType2.xml")
                    End If
                    _DataSet.WriteXml(s)

                    'custom header - start
                    Dim HeaderField As String = Nothing
                    Dim Dt As Date = CDate(CStr(DateTo))
                    Dim FormattedDate As String = Format(Dt, "dd/MM/yyyy")

                    If Not Faculty = "-1" Then
                        Dim argEn As ProgramInfoEn = New ProgramInfoDAL().GetList(New ProgramInfoEn With {.SAFC_Code = Trim(Faculty)}).FirstOrDefault()

                        If argEn.ProgramType = "UG" Then
                            HeaderField = "Penyata Hutang Pelajar Prasiswazah Setakat " + FormattedDate + " Universiti Putra Malaysia"
                        Else
                            HeaderField = "Penyata Hutang Pelajar Siswazah Setakat " + FormattedDate + " Universiti Putra Malaysia"
                        End If
                    Else
                        HeaderField = "Penyata Hutang Pelajar Siswazah Setakat " + FormattedDate + " Universiti Putra Malaysia"
                    End If
                    'custom header - end

                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")
                    Else
                        MyReportDocument.Load(Server.MapPath(ReportPath))
                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        'pass custom params - START
                        MyReportDocument.DataDefinition.FormulaFields("HeaderField").Text = "'" & HeaderField & "'"
                        MyReportDocument.DataDefinition.FormulaFields("CBType").Text = "'" & AgeingBy & "'"
                        'pass custom params - END
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()
                    End If
                    'Details Ageing Report - END 

                ElseIf Request.QueryString("Report") = "3" Then

                    'Report For KPT - START
                    Dim CurrAgeingDt As String = Request.QueryString("CurrAgeingDt")
                    Dim LastAgeingDt As String = Request.QueryString("LastAgeingDt")
                    Dim Date_CurrAgeing As String = Nothing, Date_LastAgeing As String = Nothing
                    Dim d1, m1, y1 As String, d2, m2, y2 As String

                    d1 = Mid(CurrAgeingDt, 1, 2)
                    m1 = Mid(CurrAgeingDt, 4, 2)
                    y1 = Mid(CurrAgeingDt, 7, 4)
                    Date_CurrAgeing = y1 + "/" + m1 + "/" + d1

                    d2 = Mid(LastAgeingDt, 1, 2)
                    m2 = Mid(LastAgeingDt, 4, 2)
                    y2 = Mid(LastAgeingDt, 7, 4)
                    Date_LastAgeing = y2 + "/" + m2 + "/" + d2

                    str = "SELECT " +
                            "b.refcode," +
                            "b.""BIL_1""," +
                            "b.""<3 months""," +
                            "b.collection1," +
                            "b.adjustment1," +
                            "CASE WHEN b.final_total1 <= 0 THEN 0 " +
                            "ELSE b.final_total1 END AS final_total1," +
                            "b.""BIL_2""," +
                            "b.""4-12 months""," +
                            "b.collection2," +
                            "b.adjustment2," +
                            "CASE WHEN b.final_total2 <= 0 THEN 0 " +
                            "ELSE b.final_total2 END AS final_total2," +
                            "b.""BIL_3""," +
                            "b.""1-3 years""," +
                            "b.collection3," +
                            "b.adjustment3," +
                            "CASE WHEN b.final_total3 <= 0 THEN 0 " +
                            "ELSE b.final_total3 END AS final_total3," +
                            "b.""BIL_4""," +
                            "b."">3 years""," +
                            "b.collection4," +
                            "b.adjustment4," +
                            "CASE WHEN b.final_total4 <= 0 THEN 0 " +
                            "ELSE b.final_total4 END AS final_total4 " +
                            "FROM ("
                    str += "SELECT " +
                            "RefCode," +
                            "SUM(a.bil1) AS ""BIL_1""," +
                            "SUM(a.bil2) AS ""BIL_2""," +
                            "SUM(a.bil3) AS ""BIL_3""," +
                            "SUM(a.bil4) AS ""BIL_4""," +
                            "SUM(a.data1) AS ""<3 months""," +
                            "SUM(a.data2) AS ""4-12 months""," +
                            "SUM(a.data3) AS ""1-3 years""," +
                            "SUM(a.data4) AS "">3 years""," +
                            "SUM(cols1) AS collection1," +
                            "SUM(cols2) AS collection2," +
                            "SUM(cols3) AS collection3," +
                            "SUM(cols4) AS collection4," +
                            "(SUM(tot_dbt1) - SUM(tot_crdt1)) AS adjustment1," +
                            "(SUM(tot_dbt2) - SUM(tot_crdt2)) AS adjustment2," +
                            "(SUM(tot_dbt3) - SUM(tot_crdt3)) AS adjustment3," +
                            "(SUM(tot_dbt4) - SUM(tot_crdt4)) AS adjustment4," +
                            "CASE WHEN (SUM(tot_dbt1) - SUM(tot_crdt1)) < 0 THEN SUM(a.data1) - (SUM(cols1) - (SUM(tot_dbt1) - SUM(tot_crdt1))) " +
                            "WHEN (SUM(tot_dbt1) - SUM(tot_crdt1)) > 0 THEN SUM(a.data1) - (SUM(cols1) + (SUM(tot_dbt1) - SUM(tot_crdt1))) " +
                            "ELSE SUM(a.data1) - SUM(cols1) " +
                            "END AS final_total1," +
                            "CASE WHEN (SUM(tot_dbt2) - SUM(tot_crdt2)) < 0 THEN SUM(a.data2) - (SUM(cols2) - (SUM(tot_dbt2) - SUM(tot_crdt2))) " +
                            "WHEN (SUM(tot_dbt2) - SUM(tot_crdt2)) > 0 THEN SUM(a.data2) - (SUM(cols2) + (SUM(tot_dbt2) - SUM(tot_crdt2))) " +
                            "ELSE SUM(a.data2) - SUM(cols2) " +
                            "END AS final_total2," +
                            "CASE WHEN (SUM(tot_dbt3) - SUM(tot_crdt3)) < 0 THEN SUM(a.data3) - (SUM(cols3) - (SUM(tot_dbt3) - SUM(tot_crdt3))) " +
                            "WHEN (SUM(tot_dbt3) - SUM(tot_crdt3)) > 0 THEN SUM(a.data3) - (SUM(cols3) + (SUM(tot_dbt3) - SUM(tot_crdt3))) " +
                            "ELSE SUM(a.data3) - SUM(cols3) " +
                            "END AS final_total3," +
                            "CASE WHEN (SUM(tot_dbt4) - SUM(tot_crdt4)) < 0 THEN SUM(a.data4) - (SUM(cols4) - (SUM(tot_dbt4) - SUM(tot_crdt4))) " +
                            "WHEN (SUM(tot_dbt4) - SUM(tot_crdt4)) > 0 THEN SUM(a.data4) - (SUM(cols4) + (SUM(tot_dbt4) - SUM(tot_crdt4))) " +
                            "ELSE SUM(a.data4) - SUM(cols4) " +
                            "END AS final_total4 "
                    str += "FROM (" +
                            "SELECT " +
                            "SAD.RefCode," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN " +
                            "COUNT(SA.CreditRef) " +
                            "ELSE 0 END AS bil1," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN " +
                            "COUNT(SA.CreditRef) " +
                            "ELSE 0 END AS bil2," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN " +
                            "COUNT(SA.CreditRef) " +
                            "ELSE 0 END AS bil3," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN " +
                            "COUNT(SA.CreditRef) " +
                            "ELSE 0 END AS bil4," +
                            "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN " +
                            "COALESCE(SAD.TransAmount, 0) " +
                            "ELSE '0.00' END AS data1," +
                            "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN " +
                            "COALESCE(SAD.TransAmount, 0) " +
                            "ELSE '0.00' END AS data2," +
                            "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN " +
                            "COALESCE(SAD.TransAmount, 0) " +
                            "ELSE '0.00' END AS data3," +
                            "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN " +
                            "COALESCE(SAD.TransAmount, 0) " +
                            "ELSE '0.00' END AS data4," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN " +
                            "CASE WHEN SA.Category = 'Receipt' THEN SA.TransAmount " +
                            "WHEN SA.Category = 'SPA' THEN " +
                            "CASE WHEN SA.Description LIKE '%Allocation%' THEN SA.TransAmount END " +
                            "ELSE 0.0 END " +
                            "ELSE 0.0 END AS cols1," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN " +
                            "CASE WHEN SA.Category = 'Receipt' THEN SA.TransAmount " +
                            "WHEN SA.Category = 'SPA' THEN " +
                            "CASE WHEN SA.Description LIKE '%Allocation%' THEN SA.TransAmount END " +
                            "ELSE 0.0 END " +
                            "ELSE 0.0 END AS cols2," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN " +
                            "CASE WHEN SA.Category = 'Receipt' THEN SA.TransAmount " +
                            "WHEN SA.Category = 'SPA' THEN " +
                            "CASE WHEN SA.Description LIKE '%Allocation%' THEN SA.TransAmount END " +
                            "ELSE 0.0 END " +
                            "ELSE 0.0 END AS cols3," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN " +
                            "CASE WHEN SA.Category = 'Receipt' THEN SA.TransAmount " +
                            "WHEN SA.Category = 'SPA' THEN " +
                            "CASE WHEN SA.Description LIKE '%Allocation%' THEN SA.TransAmount END " +
                            "ELSE 0.0 END " +
                            "ELSE 0.0 END AS cols4," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN " +
                            "CASE WHEN SA.Category = 'Debit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_dbt1," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN " +
                            "CASE WHEN SA.Category = 'Debit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_dbt2," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN " +
                            "CASE WHEN SA.Category = 'Debit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_dbt3," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN " +
                            "CASE WHEN SA.Category = 'Debit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_dbt4," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN " +
                            "CASE WHEN SA.Category = 'Credit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_crdt1," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN " +
                            "CASE WHEN SA.Category = 'Credit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_crdt2," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN " +
                            "CASE WHEN SA.Category = 'Credit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_crdt3," +
                            "CASE WHEN '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN " +
                            "CASE WHEN SA.Category = 'Credit Note' THEN SAD.TransAmount ELSE 0.0 END " +
                            "ELSE 0.0 END AS tot_crdt4 "
                    str += "FROM SAS_AccountsDetails SAD "
                    str += "INNER JOIN SAS_Accounts SA ON SA.TransId=SAD.TransId "
                    str += "INNER JOIN SAS_FeeTypes SF ON SF.SAFT_Code=SAD.RefCode "
                    str += "WHERE SA.PostStatus='Posted' "
                    str += "AND SA.TransDate <= '" + Date_LastAgeing + "' "
                    str += "GROUP BY SAD.RefCode,SA.CreditRef,SA.TransDate,SA.Category,SAD.TransAmount,SA.Description,SA.TransAmount "
                    str += "UNION "
                    str += "SELECT SPID.RefCode,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 91 THEN "
                    str += "COUNT(SPI.CreditRef)"
                    str += "ELSE 0 END AS bil1,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 365 THEN "
                    str += "COUNT(SPI.CreditRef)"
                    str += "ELSE 0 END AS bil2,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 1095 THEN "
                    str += "COUNT(SPI.CreditRef)"
                    str += "ELSE 0 END AS bil3,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 1095 THEN "
                    str += "COUNT(SPI.CreditRef)"
                    str += "ELSE 0 END AS bil4,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 91 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS data1,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS data2,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 1095 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS data3,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 1095 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS data4,"
                    str += "0.0 AS cols1,0.0 AS cols2,0.0 AS cols3,0.0 AS cols4,"
                    str += "0.0 AS tot_dbt1,0.0 AS tot_dbt2,0.0 AS tot_dbt3,0.0 AS tot_dbt4,"
                    str += "0.0 AS tot_crdt1,0.0 AS tot_crdt2,0.0 AS tot_crdt3,0.0 AS tot_crdt4 "
                    str += "FROM SAS_SponsorInvoiceDetails SPID "
                    str += "INNER JOIN SAS_SponsorInvoice SPI ON SPI.TransId=SPID.TransId "
                    str += "INNER JOIN SAS_FeeTypes SF ON SF.SAFT_Code=SPID.RefCode     "
                    str += "WHERE SPI.PostStatus='Posted' "
                    str += "AND SPI.TransDate <= '" + Date_LastAgeing + "' "
                    str += "GROUP BY SPID.RefCode,SPI.CreditRef,SPI.TransDate,SPID.TransAmount ) a "
                    str += "GROUP BY a.RefCode "
                    str += "ORDER BY a.RefCode "
                    str += ") b"
                    'Report For KPT - END

                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    _DataSet.Tables(0).TableName = "Table"

                    Dim s As String = Server.MapPath("~/xml/StudentAgeingType3.xml")
                    _DataSet.WriteXml(s)

                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")
                    Else
                        MyReportDocument.Load(Server.MapPath("~/GroupReport/RptStudentAgeingType3.rpt"))
                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()
                    End If
                End If

            Else
                MyReportDocument = Session("reportobject")
                CrystalReportViewer1.ReportSource = MyReportDocument
                CrystalReportViewer1.DataBind()
                MyReportDocument.Refresh()
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

#End Region

#Region "LoadAgeingByQuery"
    'created by Hafiz @ 8/11/2016 for NEW RA

    Protected Function LoadAgeingByQuery(ByVal stats As String, ByVal AgeingBy As String, ByVal DateTo As String) As String

        Dim AgeingByQuery As String = Nothing

        If AgeingBy = "rbYearly" Then
            'yearly - START
            For yr As Integer = 0 To -4 Step -1

                'Dim res As Date = dt.AddYears(yr)
                'Dim YearColumn As String = "<" & CStr(res.Year)

                Select Case yr
                    Case 0
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FirstX"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FirstX"","
                        End If
                    Case -1
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SecondX"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SecondX"","
                        End If
                    Case -2
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 AND '" + DateTo + "'::date - SA.TransDate::date <= 1095 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""ThirdX"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 AND '" + DateTo + "'::date - SA.TransDate::date <= 1095 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""ThirdX"","
                        End If
                    Case -3
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1095 AND '" + DateTo + "'::date - SA.TransDate::date <= 1460 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FourthX"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1095 AND '" + DateTo + "'::date - SA.TransDate::date <= 1460 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FourthX"","
                        End If
                    Case -4
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1460 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FifthX"" "
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1460 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FifthX"" "
                        End If
                End Select

            Next
            'yearly - END

        ElseIf AgeingBy = "rbVariousMonths" Then
            '6/12/36 months - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 182.5 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 547.5 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 547.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FifthX"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 182.5 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 547.5 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 547.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FifthX"" "
            End If
            '6/12/36 months - END

        ElseIf AgeingBy = "rbQuaterly" Then
            '30/12/2016 - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 90 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 90 AND '" + DateTo + "'::date - SA.TransDate::date <= 181 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 181 AND '" + DateTo + "'::date - SA.TransDate::date <= 273 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 273 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FifthX"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 90 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 90 AND '" + DateTo + "'::date - SA.TransDate::date <= 181 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 181 AND '" + DateTo + "'::date - SA.TransDate::date <= 273 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 273 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FifthX"" "
            End If
            '30/12/2016 - END

        ElseIf AgeingBy = "rbMonthly" Then
            '30/12/2016 - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 31 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 31 AND '" + DateTo + "'::date - SA.TransDate::date <= 60 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 60 AND '" + DateTo + "'::date - SA.TransDate::date <= 91 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 91 AND '" + DateTo + "'::date - SA.TransDate::date <= 121 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 121 AND '" + DateTo + "'::date - SA.TransDate::date <= 152 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""FifthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 152 AND '" + DateTo + "'::date - SA.TransDate::date <= 182 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SixthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182 AND '" + DateTo + "'::date - SA.TransDate::date <= 213 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""SeventhX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 213 AND '" + DateTo + "'::date - SA.TransDate::date <= 244 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""EighthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 244 AND '" + DateTo + "'::date - SA.TransDate::date <= 274 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""NinthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 274 AND '" + DateTo + "'::date - SA.TransDate::date <= 305 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""TenthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 305 AND '" + DateTo + "'::date - SA.TransDate::date <= 335 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""EleventhX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 335 AND '" + DateTo + "'::date - SA.TransDate::date <= 366 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""TwelfthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 366 THEN " & GetPaidAmount("FT") & " ELSE '0.00' END AS ""ThirteenthX"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 31 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FirstX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 31 AND '" + DateTo + "'::date - SA.TransDate::date <= 60 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SecondX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 60 AND '" + DateTo + "'::date - SA.TransDate::date <= 91 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""ThirdX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 91 AND '" + DateTo + "'::date - SA.TransDate::date <= 121 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FourthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 121 AND '" + DateTo + "'::date - SA.TransDate::date <= 152 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""FifthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 152 AND '" + DateTo + "'::date - SA.TransDate::date <= 182 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SixthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182 AND '" + DateTo + "'::date - SA.TransDate::date <= 213 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""SeventhX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 213 AND '" + DateTo + "'::date - SA.TransDate::date <= 244 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""EighthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 244 AND '" + DateTo + "'::date - SA.TransDate::date <= 274 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""NinthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 274 AND '" + DateTo + "'::date - SA.TransDate::date <= 305 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""TenthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 305 AND '" + DateTo + "'::date - SA.TransDate::date <= 335 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""EleventhX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 335 AND '" + DateTo + "'::date - SA.TransDate::date <= 366 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""TwelfthX"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 366 THEN " & GetPaidAmount("nonFT") & " ELSE '0.00' END AS ""ThirteenthX"" "
            End If
            '30/12/2016 - END
        End If

        Return AgeingByQuery

    End Function

#End Region

#Region "Monthly CB Query Builder"

    Public Function QueryBuilder(ByVal AgeingBy As String, ByVal Header As String, Optional ByVal str As String = Nothing) As String

        Dim SumQuery As String = Nothing

        If AgeingBy = "rbMonthly" Then
            If str = "HEAD" Then
                SumQuery = "SUM(" & Header & ".""FirstX"") AS ""First"","
                SumQuery += "SUM(" & Header & ".""SecondX"") AS ""Second"","
                SumQuery += "SUM(" & Header & ".""ThirdX"") AS ""Third"","
                SumQuery += "SUM(" & Header & ".""FourthX"") AS ""Fourth"","
                SumQuery += "SUM(" & Header & ".""FifthX"") AS ""Fifth"","
                SumQuery += "SUM(" & Header & ".""SixthX"") AS ""Sixth"","
                SumQuery += "SUM(" & Header & ".""SeventhX"") AS ""Seventh"","
                SumQuery += "SUM(" & Header & ".""EighthX"") AS ""Eighth"","
                SumQuery += "SUM(" & Header & ".""NinthX"") AS ""Ninth"","
                SumQuery += "SUM(" & Header & ".""TenthX"") AS ""Tenth"","
                SumQuery += "SUM(" & Header & ".""EleventhX"") AS ""Eleventh"","
                SumQuery += "SUM(" & Header & ".""TwelfthX"") AS ""Twelfth"","
                SumQuery += "SUM(" & Header & ".""FirstX"")+SUM(" & Header & ".""SecondX"")+SUM(" & Header & ".""ThirdX"")+SUM(" & Header & ".""FourthX"")+SUM(" & Header & ".""FifthX"")+" +
                    "SUM(" & Header & ".""SixthX"")+SUM(" & Header & ".""SeventhX"")+SUM(" & Header & ".""EighthX"")+SUM(" & Header & ".""NinthX"")+SUM(" & Header & ".""TenthX"")+" +
                    "SUM(" & Header & ".""EleventhX"")+SUM(" & Header & ".""TwelfthX"") AS ""TotalAmount"","
            End If
        Else
            If str = "HEAD" Then
                SumQuery = "SUM(" & Header & ".""FirstX"") AS ""First"","
                SumQuery += "SUM(" & Header & ".""SecondX"") AS ""Second"","
                SumQuery += "SUM(" & Header & ".""ThirdX"") AS ""Third"","
                SumQuery += "SUM(" & Header & ".""FourthX"") AS ""Fourth"","
                SumQuery += "SUM(" & Header & ".""FifthX"") AS ""Fifth"","
                SumQuery += "SUM(" & Header & ".""FirstX"")+SUM(" & Header & ".""SecondX"")+SUM(" & Header & ".""ThirdX"")+SUM(" & Header & ".""FourthX"")+SUM(" & Header & ".""FifthX"") AS ""TotalAmount"","
            End If
        End If

        Return SumQuery

    End Function

#End Region

#Region "GetPaidAmount"
    'added by Hafiz @ 06/02/2017

    Protected Function GetPaidAmount(ByVal stats As String) As String

        Dim Query As String = String.Empty

        If stats = "FT" Then
            Query = " CASE WHEN COALESCE(SAD.TransAmount,0) - SUM(a.PAID_AMT) < 0 THEN 0 " +
                "ELSE COALESCE(SAD.TransAmount,0) - SUM(a.PAID_AMT) END"
        ElseIf stats = "nonFT" Then
            Query = " CASE WHEN SA.PaidAmount=SUM(a.PAID_AMT) THEN COALESCE(SA.TransAmount,0)-0 " +
                "ELSE COALESCE(SA.TransAmount,0)-(COALESCE(SA.PaidAmount,0)-SUM(a.PAID_AMT)) " +
                "END"
        End If

        Return Query

    End Function

#End Region

#Region "Page_Unload"

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        'MyReportDocument.Close()
        'MyReportDocument.Dispose()
    End Sub

#End Region

End Class
