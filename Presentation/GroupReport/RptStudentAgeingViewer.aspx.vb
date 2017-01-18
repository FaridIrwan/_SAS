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

                    Dim StatusQuery As String = Nothing, FilterStatus As String = Nothing, SumQuery As String = Nothing
                    If Status <> "-1" Then
                        FilterStatus = "AND SS.SASS_Code = '" + Status + "' "
                        StatusQuery = "LEFT JOIN SAS_Studentstatus SS ON SS.SASS_Code IN (SELECT SASS_Code FROM SAS_Student WHERE SASI_MatricNo=ageing.CreditRef) "
                    End If

                    Dim Header_FT_str As String = Nothing, FT_str As String = Nothing, FT_join As String = Nothing, FT_filter As String = Nothing

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

                        Header_FT_str = "ageing.GLaccount AS col" + columnNo.ToString() + ","
                        FT_str = "COALESCE(SFGL.GL_account,SKGL.GL_account) AS GLaccount,"

                        FT_join = "LEFT JOIN SAS_Faculty_GLaccount SFGL ON SFGL.SAFT_Code = a.RefCode AND SFGL.SAFC_Code = "
                        FT_join += "(SELECT SASI_Faculty FROM SAS_Student WHERE SASI_MatricNo=a.CreditRef) "
                        FT_join += "LEFT JOIN SAS_Kolej_glaccount SKGL ON SKGL.SAFT_Code = a.RefCode AND SKGL.Sako_code = "
                        FT_join += "(SELECT SAKO_Code FROM SAS_Student WHERE SASI_MatricNo=a.CreditRef) "

                        FT_filter = "WHERE a.RefCode = '" & FeeType & "' "

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
                        str += Header_FT_str
                        str += QueryBuilder(AgeingBy, "ageing", "SuperHeader")
                        str += "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo "
                        str += "FROM ( "
                        str += "SELECT b.CreditRef,b.GLAccount,"
                        str += QueryBuilder(AgeingBy, "b", "Header1")
                        str += "FROM ( "
                        str += "WITH PAID_AMT AS (SELECT CreditRef,SUM(TransAmount) AS PaidAmount " +
                                "FROM SAS_Accounts " +
                                "WHERE PostStatus='Posted' " +
                                "AND TransType='Credit' " +
                                "GROUP BY CreditRef) "
                        str += "SELECT a.CreditRef,"
                        str += FT_str
                        str += QueryBuilder(AgeingBy, "a", "Tail")
                        str += "FROM "
                        str += "(SELECT SAD.RefCode,SA.CreditRef,"
                        str += LoadAgeingByQuery("FT", AgeingBy, DateTo)
                        str += "FROM SAS_Accounts SA "
                        str += "INNER JOIN SAS_AccountsDetails SAD ON SAD.TransId=SA.TransId "
                        str += "WHERE SA.TransAmount > 0 "
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SAD.RefCode,SA.CreditRef,SA.TransDate,SAD.TransAmount ) a "
                        str += "INNER JOIN PAID_AMT ON PAID_AMT.CreditRef=a.CreditRef "
                        str += FT_join
                        str += FT_filter
                        str += "GROUP BY a.CreditRef,SFGL.GL_account,SKGL.GL_account,PAID_AMT.PaidAmount ) b "

                        str += "UNION "

                        str += "SELECT * FROM "
                        str += "(SELECT a.CreditRef,"
                        str += FT_str
                        str += QueryBuilder(AgeingBy, "a", "Header2")
                        str += "FROM "
                        str += "(SELECT SAD.RefCode,SA.CreditRef,"
                        str += LoadAgeingByQuery("FT", AgeingBy, DateTo)
                        str += "FROM SAS_Accounts SA "
                        str += "INNER JOIN SAS_AccountsDetails SAD ON SAD.TransId=SA.TransId "
                        str += "WHERE SA.TransAmount > 0 "
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SAD.RefCode,SA.CreditRef,SA.TransDate,SAD.TransAmount ) a "
                        str += FT_join
                        str += FT_filter
                        str += "GROUP BY a.CreditRef,SFGL.GL_account,SKGL.GL_account ) b "
                        str += "WHERE b.CreditRef NOT IN (SELECT CreditRef FROM SAS_Accounts WHERE TransType='Credit') "
                        str += ") ageing "
                        str += StatusQuery
                        str += "WHERE ageing.CreditRef IN (SELECT SASI_MatricNo FROM SAS_Student) "
                        str += FilterStatus
                        str += "ORDER BY ageing.CreditRef"

                    Else
                        If AgeingBy = "rbMonthly" Then
                            ReportPath = "~/GroupReport/RptStudentAgeingType1b_Month.rpt"
                        Else
                            ReportPath = "~/GroupReport/RptStudentAgeingType1b.rpt"
                        End If

                        str = Header
                        str += QueryBuilder(AgeingBy, "ageing", "SuperHeader")
                        str += "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo "
                        str += "FROM ( "
                        str += "SELECT b.CreditRef,"
                        str += QueryBuilder(AgeingBy, "b", "Header1")
                        str += "FROM ( "
                        str += "WITH PAID_AMT AS (SELECT CreditRef,SUM(TransAmount) AS PaidAmount " +
                                "FROM SAS_Accounts " +
                                "WHERE PostStatus='Posted' " +
                                "AND TransType='Credit' " +
                                "GROUP BY CreditRef) "
                        str += "SELECT a.CreditRef,"
                        str += QueryBuilder(AgeingBy, "a", "Tail")
                        str += "FROM "
                        str += "(SELECT SA.TransId,SA.CreditRef,"
                        str += LoadAgeingByQuery("nonFT", AgeingBy, DateTo)
                        str += "FROM SAS_Accounts SA "
                        str += "WHERE SA.TransAmount > 0 "
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SA.TransId,SA.CreditRef,SA.TransDate,SA.TransAmount ) a "
                        str += "INNER JOIN PAID_AMT ON PAID_AMT.CreditRef=a.CreditRef "
                        str += "GROUP BY a.CreditRef,PAID_AMT.PaidAmount ) b "

                        str += "UNION "

                        str += "SELECT * FROM "
                        str += "(SELECT a.CreditRef,"
                        str += QueryBuilder(AgeingBy, "a", "Header2")
                        str += "FROM "
                        str += "(SELECT SA.TransId,SA.CreditRef,"
                        str += LoadAgeingByQuery("nonFT", AgeingBy, DateTo)
                        str += "FROM SAS_Accounts SA "
                        str += "WHERE SA.TransAmount > 0 "
                        str += "AND SA.SubType <> 'Sponsor' "
                        str += "AND SA.Category <> 'Payment' "
                        str += "AND SA.TransType <> 'Credit' "
                        str += "AND SA.PostStatus = 'Posted' "
                        str += "GROUP BY SA.TransId,SA.CreditRef,SA.TransDate,SA.TransAmount ) a "
                        str += "GROUP BY a.CreditRef ) b "
                        str += "WHERE b.CreditRef NOT IN (SELECT CreditRef FROM SAS_Accounts WHERE TransType='Credit') "
                        str += ") ageing "
                        str += StatusQuery
                        str += "WHERE ageing.CreditRef IN (SELECT SASI_MatricNo FROM SAS_Student) "
                        str += FilterStatus
                        str += "ORDER BY ageing.CreditRef"

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

                    Dim StatusStr As String = Nothing, NationalityStr As String = Nothing
                    Dim FacultyStr As String = Nothing, SponsorStr As String = Nothing
                    Dim SumQuery As String = Nothing

                    'Status
                    If Status <> "-1" Then
                        StatusStr = "AND SS.SASS_Code = '" + Status + "' "
                    End If

                    'Nationality
                    If Nationality <> "-1" Then
                        NationalityStr = "AND (SELECT SASC_Code FROM SAS_Student WHERE SASI_MatricNo=SAR.MatricNo) = '" + IIf(Nationality = "1", "W", "BW") + "' "
                    End If

                    'Faculty
                    If Faculty <> "-1" Then
                        FacultyStr = "AND SAR.Faculty = (SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code = '" + Faculty + "') "
                    End If

                    'Sponsor
                    If Sponsor <> "-1" Then
                        SponsorStr = "AND SAR.Sponsor = (SELECT SASR_Name FROM SAS_Sponsor WHERE SASR_Code = '" + Sponsor + "') "
                    End If

                    'QUERY BUILDER - START
                    If AgeingBy = "rbMonthly" Then
                        ReportPath = "~/GroupReport/RptStudentAgeingType2_Month.rpt"
                    Else
                        ReportPath = "~/GroupReport/RptStudentAgeingType2.rpt"
                    End If

                    str = "SELECT SAR.MatricNo,SAR.Faculty,SAR.Sponsor," +
                            "SS.SASS_Description AS Status," +
                            "CASE WHEN (SELECT SASC_Code FROM SAS_Student WHERE SASI_MatricNo=SAR.MatricNo) = 'W' THEN 'LOCAL' " +
                            "ELSE 'FOREIGNER' END AS Category,"
                    str += QueryBuilder(AgeingBy, "SAR") +
                            "SUM(SAR.TotalAmount) AS TotalAmount," +
                            "TO_CHAR(DATE '" + DateTo + "', 'DD/MM/YYYY') AS DateTo " +
                            "FROM ( "
                    str += "(SELECT a.CreditRef AS MatricNo,(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code IN (SELECT SASI_Faculty FROM SAS_Student WHERE SASI_MatricNo=a.CreditRef)) AS Faculty," +
                            "(SELECT SASR_Name FROM SAS_Sponsor WHERE SASR_Code IN (SELECT CreditRef1 FROM SAS_Accounts WHERE CreditRef=a.CreditRef)) AS Sponsor,"
                    str += QueryBuilder(AgeingBy, "a") +
                            "SUM(a.TotalAmount) AS TotalAmount " +
                            "FROM ( " +
                            "SELECT TransId,CreditRef," + LoadAgeingByQuery("nonFT", AgeingBy, DateTo) +
                            "FROM SAS_Accounts SA " +
                            "WHERE TransAmount > 0 " +
                            "AND SubType <> 'Sponsor' AND Category <> 'Payment' AND TransType <> 'Credit' AND PostStatus = 'Posted' " +
                            "GROUP BY TransId,CreditRef,TransDate,TransAmount ) a " +
                            "WHERE a.CreditRef IN (SELECT sa.CreditRef FROM SAS_Accounts sa INNER JOIN SAS_Student ss ON ss.SASI_MatricNo=sa.CreditRef) " +
                            "GROUP BY a.CreditRef)"

                    str += " UNION "

                    str += "(SELECT b.CreditRef,(SELECT SAFC_Desc FROM SAS_Faculty WHERE SAFC_Code IN (SELECT SASI_Faculty FROM SAS_Student WHERE SASI_MatricNo=b.CreditRef)) AS Faculty," +
                            "SSP.SASR_Name AS Sponsor,"
                    str += QueryBuilder(AgeingBy, "b") +
                            "SUM(b.TotalAmount) AS TotalAmount " +
                            "FROM ( " +
                            "SELECT SPI.TransId, SPI.CreditRef, SPI.CreditRef1," + LoadAgeingByQuery("SPNSR", AgeingBy, DateTo) +
                            "FROM SAS_SponsorInvoice SPI " +
                            "INNER JOIN SAS_SponsorInvoiceDetails SPID ON SPI.TransId=SPID.TransId " +
                            "WHERE SPI.PostStatus = 'Posted' " +
                            "GROUP BY SPI.TransId,SPI.CreditRef,SPI.CreditRef1,SPI.TransDate,SPID.TransAmount ) b " +
                            "INNER JOIN SAS_Sponsor SSP ON SSP.SASR_Code=b.CreditRef1 " +
                            "GROUP BY b.CreditRef,SSP.SASR_Name)"
                    str += ") SAR "
                    str += "INNER JOIN SAS_Studentstatus SS ON SS.SASS_Code IN (SELECT SASS_Code FROM SAS_Student WHERE SASI_MatricNo=SAR.MatricNo) "
                    str += "WHERE SAR.MatricNo <> '' "
                    str += StatusStr
                    str += NationalityStr
                    str += FacultyStr
                    str += SponsorStr
                    str += "GROUP BY SAR.MatricNo,SAR.Faculty,SAR.Sponsor,SS.SASS_Description " +
                            "ORDER BY SAR.Sponsor"
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

                    str = "SELECT RefCode,COUNT(a.CreditRef) AS bil,"
                    str += "SUM(a.""<3 months"") AS ""<3 months"","
                    str += "SUM(a.""4-12 months"") AS ""4-12 months"","
                    str += "SUM(a.""1-3 years"") AS ""1-3 years"","
                    str += "SUM(a."">3 years"") AS "">3 years"","
                    str += "SUM(cols) AS collection,(SUM(tot_dbt) - SUM(tot_crdt)) AS adjustment,"
                    str += "CASE WHEN (SUM(tot_dbt) - SUM(tot_crdt)) < 0 THEN (SUM(a.""<3 months"")+SUM(a.""4-12 months"")+SUM(a.""1-3 years"")+SUM(a."">3 years"")) - " +
                        "SUM(cols) - SUM(tot_dbt) - SUM(tot_crdt) "
                    str += "WHEN (SUM(tot_dbt) - SUM(tot_crdt)) > 0 THEN (SUM(a.""<3 months"")+SUM(a.""4-12 months"")+SUM(a.""1-3 years"")+SUM(a."">3 years"")) - " +
                        "SUM(cols) + SUM(tot_dbt) - SUM(tot_crdt) "
                    str += "ELSE (SUM(a.""<3 months"")+SUM(a.""4-12 months"")+SUM(a.""1-3 years"")+SUM(a."">3 years"")) - SUM(cols) END AS final_total "
                    str += "FROM ("
                    str += "SELECT SAD.RefCode,SA.CreditRef,"
                    str += "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 91 THEN "
                    str += "COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""<3 months"","
                    str += "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 365 THEN "
                    str += "COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""4-12 months"","
                    str += "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date <= 1095 THEN "
                    str += "COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""1-3 years"","
                    str += "CASE WHEN SA.Category IN ('AFC','Invoice') AND '" & Date_CurrAgeing & "'::date - SA.TransDate::date > 1095 THEN "
                    str += "COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS "">3 years"","
                    str += "CASE WHEN SA.Category = 'Receipt' THEN SA.TransAmount "
                    str += "WHEN SA.Category = 'SPA' THEN CASE WHEN SA.Description LIKE '%Allocation%' THEN SA.TransAmount END "
                    str += "ELSE 0.0 END AS cols,"
                    str += "CASE WHEN SA.Category = 'Debit Note' THEN SAD.TransAmount ELSE 0.0 END AS tot_dbt,CASE WHEN SA.Category = 'Credit Note' THEN SAD.TransAmount ELSE 0.0 END AS tot_crdt "
                    str += "FROM SAS_AccountsDetails SAD "
                    str += "INNER JOIN SAS_Accounts SA ON SA.TransId=SAD.TransId "
                    str += "INNER JOIN SAS_FeeTypes SF ON SF.SAFT_Code=SAD.RefCode "
                    str += "WHERE SA.PostStatus='Posted' "
                    str += "GROUP BY SAD.RefCode,SA.CreditRef,SA.TransDate,SA.Category,SAD.TransAmount,SA.Description,SA.TransAmount "
                    str += "UNION "
                    str += "SELECT SPID.RefCode,SPI.CreditRef,"
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 91 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""<3 months"","
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 91 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""4-12 months"","
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 365 AND '" & Date_CurrAgeing & "'::date - SPI.TransDate::date <= 1095 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""1-3 years"","
                    str += "CASE WHEN '" & Date_CurrAgeing & "'::date - SPI.TransDate::date > 1095 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS "">3 years"","
                    str += "0 AS cols,0 AS tot_dbt,0 AS tot_crdt "
                    str += "FROM SAS_SponsorInvoiceDetails SPID "
                    str += "INNER JOIN SAS_SponsorInvoice SPI ON SPI.TransId=SPID.TransId "
                    str += "INNER JOIN SAS_FeeTypes SF ON SF.SAFT_Code=SPID.RefCode     "
                    str += "WHERE SPI.PostStatus='Posted' "
                    str += "GROUP BY SPID.RefCode,SPI.CreditRef,SPI.TransDate,SPID.TransAmount ) a "
                    str += "GROUP BY a.RefCode "
                    str += "ORDER BY a.RefCode "
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
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 365 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""First"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 365 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""First"","
                        ElseIf stats = "SPNSR" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date <= 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""First"","
                        End If
                    Case -1
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                        ElseIf stats = "SPNSR" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 365 AND '" + DateTo + "'::date - SPI.TransDate::date <= 730 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                        End If
                    Case -2
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 AND '" + DateTo + "'::date - SA.TransDate::date <= 1095 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 AND '" + DateTo + "'::date - SA.TransDate::date <= 1095 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                        ElseIf stats = "SPNSR" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 730 AND '" + DateTo + "'::date - SPI.TransDate::date <= 1095 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                        End If
                    Case -3
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1095 AND '" + DateTo + "'::date - SA.TransDate::date <= 1460 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1095 AND '" + DateTo + "'::date - SA.TransDate::date <= 1460 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                        ElseIf stats = "SPNSR" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 1095 AND '" + DateTo + "'::date - SPI.TransDate::date <= 1460 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                        End If
                    Case -4
                        If stats = "FT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1460 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
                        ElseIf stats = "nonFT" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 1460 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
                        ElseIf stats = "SPNSR" Then
                            AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 1460 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
                        End If
                End Select

            Next
            'yearly - END

        ElseIf AgeingBy = "rbVariousMonths" Then
            '6/12/36 months - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 182.5 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 547.5 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 547.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 182.5 THEN COALESCE(SA.TransAmount, 0)  ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN COALESCE(SA.TransAmount, 0)  ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 AND '" + DateTo + "'::date - SA.TransDate::date <= 547.5 THEN COALESCE(SA.TransAmount, 0)  ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 547.5 AND '" + DateTo + "'::date - SA.TransDate::date <= 730 THEN COALESCE(SA.TransAmount, 0)  ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 730 THEN COALESCE(SA.TransAmount, 0)  ELSE '0.00' END AS ""Fifth"" "
            ElseIf stats = "SPNSR" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date <= 182.5 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 182.5 AND '" + DateTo + "'::date - SPI.TransDate::date <= 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 365 AND '" + DateTo + "'::date - SPI.TransDate::date <= 547.5 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 547.5 AND '" + DateTo + "'::date - SPI.TransDate::date <= 730 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 730 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
            End If
            '6/12/36 months - END

        ElseIf AgeingBy = "rbQuaterly" Then
            '30/12/2016 - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 90 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 90 AND '" + DateTo + "'::date - SA.TransDate::date <= 181 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 181 AND '" + DateTo + "'::date - SA.TransDate::date <= 273 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 273 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 90 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 90 AND '" + DateTo + "'::date - SA.TransDate::date <= 181 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 181 AND '" + DateTo + "'::date - SA.TransDate::date <= 273 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 273 AND '" + DateTo + "'::date - SA.TransDate::date <= 365 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 365 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
            ElseIf stats = "SPNSR" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date <= 90 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 90 AND '" + DateTo + "'::date - SPI.TransDate::date <= 181 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 181 AND '" + DateTo + "'::date - SPI.TransDate::date <= 273 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 273 AND '" + DateTo + "'::date - SPI.TransDate::date <= 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 365 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fifth"" "
            End If
            '30/12/2016 - END

        ElseIf AgeingBy = "rbMonthly" Then
            '30/12/2016 - START
            If stats = "FT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 31 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 31 AND '" + DateTo + "'::date - SA.TransDate::date <= 60 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 60 AND '" + DateTo + "'::date - SA.TransDate::date <= 91 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 91 AND '" + DateTo + "'::date - SA.TransDate::date <= 121 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 121 AND '" + DateTo + "'::date - SA.TransDate::date <= 152 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Fifth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 152 AND '" + DateTo + "'::date - SA.TransDate::date <= 182 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Sixth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182 AND '" + DateTo + "'::date - SA.TransDate::date <= 213 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Seventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 213 AND '" + DateTo + "'::date - SA.TransDate::date <= 244 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Eighth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 244 AND '" + DateTo + "'::date - SA.TransDate::date <= 274 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Ninth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 274 AND '" + DateTo + "'::date - SA.TransDate::date <= 305 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Tenth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 305 AND '" + DateTo + "'::date - SA.TransDate::date <= 335 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Eleventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 335 AND '" + DateTo + "'::date - SA.TransDate::date <= 366 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Twelfth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 366 THEN COALESCE(SAD.TransAmount, 0) ELSE '0.00' END AS ""Thirteenth"" "
            ElseIf stats = "nonFT" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date <= 31 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 31 AND '" + DateTo + "'::date - SA.TransDate::date <= 60 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 60 AND '" + DateTo + "'::date - SA.TransDate::date <= 91 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 91 AND '" + DateTo + "'::date - SA.TransDate::date <= 121 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 121 AND '" + DateTo + "'::date - SA.TransDate::date <= 152 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Fifth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 152 AND '" + DateTo + "'::date - SA.TransDate::date <= 182 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Sixth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 182 AND '" + DateTo + "'::date - SA.TransDate::date <= 213 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Seventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 213 AND '" + DateTo + "'::date - SA.TransDate::date <= 244 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Eighth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 244 AND '" + DateTo + "'::date - SA.TransDate::date <= 274 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Ninth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 274 AND '" + DateTo + "'::date - SA.TransDate::date <= 305 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Tenth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 305 AND '" + DateTo + "'::date - SA.TransDate::date <= 335 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Eleventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 335 AND '" + DateTo + "'::date - SA.TransDate::date <= 366 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Twelfth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SA.TransDate::date > 366 THEN COALESCE(SA.TransAmount, 0) ELSE '0.00' END AS ""Thirteenth"" "
            ElseIf stats = "SPNSR" Then
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date <= 31 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""First"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 31 AND '" + DateTo + "'::date - SPI.TransDate::date <= 60 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Second"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 60 AND '" + DateTo + "'::date - SPI.TransDate::date <= 91 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Third"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 91 AND '" + DateTo + "'::date - SPI.TransDate::date <= 121 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fourth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 121 AND '" + DateTo + "'::date - SPI.TransDate::date <= 152 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Fifth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 152 AND '" + DateTo + "'::date - SPI.TransDate::date <= 182 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Sixth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 182 AND '" + DateTo + "'::date - SPI.TransDate::date <= 213 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Seventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 213 AND '" + DateTo + "'::date - SPI.TransDate::date <= 244 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Eighth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 244 AND '" + DateTo + "'::date - SPI.TransDate::date <= 274 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Ninth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 274 AND '" + DateTo + "'::date - SPI.TransDate::date <= 305 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Tenth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 305 AND '" + DateTo + "'::date - SPI.TransDate::date <= 335 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Eleventh"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 335 AND '" + DateTo + "'::date - SPI.TransDate::date <= 366 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Twelfth"","
                AgeingByQuery += "CASE WHEN '" & DateTo & "'::date - SPI.TransDate::date > 366 THEN COALESCE(SPID.TransAmount, 0) ELSE '0.00' END AS ""Thirteenth"" "
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

            If str = "SuperHeader" Then
                SumQuery = "COALESCE(" & Header & ".""First"",0) AS ""First"","
                SumQuery += "COALESCE(" & Header & ".""Second"",0) AS ""Second"","
                SumQuery += "COALESCE(" & Header & ".""Third"",0) AS ""Third"","
                SumQuery += "COALESCE(" & Header & ".""Fourth"",0) AS ""Fourth"","
                SumQuery += "COALESCE(" & Header & ".""Fifth"",0) AS ""Fifth"","
                SumQuery += "COALESCE(" & Header & ".""Sixth"",0) AS ""Sixth"","
                SumQuery += "COALESCE(" & Header & ".""Seventh"",0) AS ""Seventh"","
                SumQuery += "COALESCE(" & Header & ".""Eighth"",0) AS ""Eighth"","
                SumQuery += "COALESCE(" & Header & ".""Ninth"",0) AS ""Ninth"","
                SumQuery += "COALESCE(" & Header & ".""Tenth"",0) AS ""Tenth"","
                SumQuery += "COALESCE(" & Header & ".""Eleventh"",0) AS ""Eleventh"","
                SumQuery += "COALESCE(" & Header & ".""Twelfth"",0) AS ""Twelfth"","
                SumQuery += "COALESCE(" & Header & ".""TotalAmount"",0) AS ""TotalAmount"","
            ElseIf str = "Header1" Then
                SumQuery = "COALESCE(" & Header & ".""FirstX"",0) AS ""First"","
                SumQuery += "COALESCE(" & Header & ".""SecondX"",0) AS ""Second"","
                SumQuery += "COALESCE(" & Header & ".""ThirdX"",0) AS ""Third"","
                SumQuery += "COALESCE(" & Header & ".""FourthX"",0) AS ""Fourth"","
                SumQuery += "COALESCE(" & Header & ".""FifthX"",0) AS ""Fifth"","
                SumQuery += "COALESCE(" & Header & ".""SixthX"",0) AS ""Sixth"","
                SumQuery += "COALESCE(" & Header & ".""SeventhX"",0) AS ""Seventh"","
                SumQuery += "COALESCE(" & Header & ".""EighthX"",0) AS ""Eighth"","
                SumQuery += "COALESCE(" & Header & ".""NinthX"",0) AS ""Ninth"","
                SumQuery += "COALESCE(" & Header & ".""TenthX"",0) AS ""Tenth"","
                SumQuery += "COALESCE(" & Header & ".""EleventhX"",0) AS ""Eleventh"","
                SumQuery += "COALESCE(" & Header & ".""TwelfthX"",0) AS ""Twelfth"","
                SumQuery += "COALESCE(" & Header & ".""FirstX""+" & Header & ".""SecondX""+" & Header & ".""ThirdX""+" & Header & ".""FourthX""+" & Header & ".""FifthX""+" +
                    "" & Header & ".""SixthX""+" & Header & ".""SeventhX""+" & Header & ".""EighthX""+" & Header & ".""NinthX""+" & Header & ".""TenthX""+" & Header & ".""EleventhX""+" +
                    "" & Header & ".""TwelfthX"",0) AS ""TotalAmount"" "
            ElseIf str = "Header2" Then
                SumQuery = "SUM(" & Header & ".""First"") AS ""First"","
                SumQuery += "SUM(" & Header & ".""Second"") AS ""Second"","
                SumQuery += "SUM(" & Header & ".""Third"") AS ""Third"","
                SumQuery += "SUM(" & Header & ".""Fourth"") AS ""Fourth"","
                SumQuery += "SUM(" & Header & ".""Fifth"") AS ""Fifth"","
                SumQuery += "SUM(" & Header & ".""Sixth"") AS ""Sixth"","
                SumQuery += "SUM(" & Header & ".""Seventh"") AS ""Seventh"","
                SumQuery += "SUM(" & Header & ".""Eighth"") AS ""Eighth"","
                SumQuery += "SUM(" & Header & ".""Ninth"") AS ""Ninth"","
                SumQuery += "SUM(" & Header & ".""Tenth"") AS ""Tenth"","
                SumQuery += "SUM(" & Header & ".""Eleventh"") AS ""Eleventh"","
                SumQuery += "SUM(" & Header & ".""Twelfth"") AS ""Twelfth"","
                SumQuery += "SUM(" & Header & ".""First"")+SUM(" & Header & ".""Second"")+SUM(" & Header & ".""Third"")+SUM(" & Header & ".""Fourth"")+SUM(" & Header & ".""Fifth"")+" +
                    "SUM(" & Header & ".""Sixth"")+SUM(" & Header & ".""Seventh"")+SUM(" & Header & ".""Eighth"")+SUM(" & Header & ".""Ninth"")+SUM(" & Header & ".""Tenth"")+" +
                    "SUM(" & Header & ".""Eleventh"")+SUM(" & Header & ".""Twelfth"") AS ""TotalAmount"" "
            ElseIf str = "Tail" Then
                SumQuery = "CASE WHEN SUM(" & Header & ".""First"") != 0 THEN " +
                    "CASE WHEN SUM(" & Header & ".""First"") > PAID_AMT.PaidAmount THEN SUM(" & Header & ".""First"") - PAID_AMT.PaidAmount " +
                    "WHEN SUM(" & Header & ".""First"") < PAID_AMT.PaidAmount THEN 0.0 END " +
                    "ELSE 0.0 END AS ""FirstX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Second"") != 0 THEN " +
                    "CASE WHEN PAID_AMT.PaidAmount - SUM(" & Header & ".""First"") < 0 THEN SUM(" & Header & ".""Second"") ELSE " +
                    "CASE WHEN SUM(" & Header & ".""Second"") > (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) THEN SUM(" & Header & ".""Second"") - (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) " +
                    "WHEN SUM(" & Header & ".""Second"") < (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""SecondX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Third"") != 0 THEN " +
                    "CASE WHEN (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"") < 0 THEN SUM(" & Header & ".""Third"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Third"") > ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) " +
                    "THEN SUM(" & Header & ".""Third"") - (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"") " +
                    "WHEN SUM(" & Header & ".""Third"") < ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""ThirdX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Fourth"") != 0 THEN " +
                    "CASE WHEN ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") < 0 THEN SUM(" & Header & ".""Fourth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Fourth"") > ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") " +
                    "THEN SUM(" & Header & ".""Fourth"") - ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") " +
                    "WHEN SUM(" & Header & ".""Fourth"") < ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""FourthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Fifth"") != 0 THEN " +
                    "CASE WHEN (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") < 0 THEN SUM(" & Header & ".""Fifth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Fifth"") > (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") " +
                    "THEN SUM(" & Header & ".""Fifth"") - (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") " +
                    "WHEN SUM(" & Header & ".""Fifth"") < (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""FifthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Sixth"") != 0 THEN " +
                    "CASE WHEN ((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"") < 0 THEN SUM(" & Header & ".""Sixth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Sixth"") > ((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"") " +
                    "THEN SUM(" & Header & ".""Sixth"") - ((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"") " +
                    "WHEN SUM(" & Header & ".""Sixth"") < ((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""SixthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Seventh"") != 0 THEN " +
                    "CASE WHEN (((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"") < 0 THEN SUM(" & Header & ".""Seventh"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Seventh"") > (((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"") " +
                    "THEN SUM(" & Header & ".""Seventh"") - (((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"") " +
                    "WHEN SUM(" & Header & ".""Seventh"") < (((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""SeventhX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Eighth"") != 0 THEN " +
                    "CASE WHEN ((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"") < 0 THEN SUM(" & Header & ".""Eighth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Eighth"") > ((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"") " +
                    "THEN SUM(" & Header & ".""Eighth"") - ((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"") " +
                    "WHEN SUM(" & Header & ".""Eighth"") < ((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""EighthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Ninth"") != 0 THEN " +
                    "CASE WHEN (((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"") < 0 THEN SUM(" & Header & ".""Ninth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Ninth"") > (((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"") " +
                    "THEN SUM(" & Header & ".""Ninth"") - (((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"") " +
                    "WHEN SUM(" & Header & ".""Ninth"") < (((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""NinthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Tenth"") != 0 THEN " +
                    "CASE WHEN ((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"") < 0 THEN SUM(" & Header & ".""Tenth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Tenth"") > ((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"") " +
                    "THEN SUM(" & Header & ".""Tenth"") - ((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"") " +
                    "WHEN SUM(" & Header & ".""Tenth"") < ((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""TenthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Eleventh"") != 0 THEN " +
                    "CASE WHEN (((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"") < 0 THEN SUM(" & Header & ".""Eleventh"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Eleventh"") > (((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"") " +
                    "THEN SUM(" & Header & ".""Eleventh"") - (((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"") " +
                    "WHEN SUM(" & Header & ".""Eleventh"") < (((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""EleventhX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Twelfth"") != 0 THEN " +
                   "CASE WHEN ((((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"")) - SUM(" & Header & ".""Eleventh"") < 0 THEN SUM(" & Header & ".""Twelfth"") " +
                   "ELSE CASE WHEN SUM(" & Header & ".""Twelfth"") > ((((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"")) - SUM(" & Header & ".""Eleventh"") " +
                   "THEN SUM(" & Header & ".""Twelfth"") - ((((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"")) - SUM(" & Header & ".""Eleventh"") " +
                   "WHEN SUM(" & Header & ".""Twelfth"") < ((((((((((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"")) - SUM(" & Header & ".""Fifth"")) - SUM(" & Header & ".""Sixth"")) - SUM(" & Header & ".""Seventh"")) - SUM(" & Header & ".""Eighth"")) - SUM(" & Header & ".""Ninth"")) - SUM(" & Header & ".""Tenth"")) - SUM(" & Header & ".""Eleventh"") THEN 0.0 END " +
                   "END ELSE 0.0 END AS ""TwelfthX"" "
            End If
        Else
            If str = "SuperHeader" Then
                SumQuery = "COALESCE(" & Header & ".""First"",0) AS ""First"","
                SumQuery += "COALESCE(" & Header & ".""Second"",0) AS ""Second"","
                SumQuery += "COALESCE(" & Header & ".""Third"",0) AS ""Third"","
                SumQuery += "COALESCE(" & Header & ".""Fourth"",0) AS ""Fourth"","
                SumQuery += "COALESCE(" & Header & ".""Fifth"",0) AS ""Fifth"","
                SumQuery += "COALESCE(" & Header & ".""TotalAmount"",0) AS ""TotalAmount"","
            ElseIf str = "Header1" Then
                SumQuery = "COALESCE(" & Header & ".""FirstX"",0) AS ""First"","
                SumQuery += "COALESCE(" & Header & ".""SecondX"",0) AS ""Second"","
                SumQuery += "COALESCE(" & Header & ".""ThirdX"",0) AS ""Third"","
                SumQuery += "COALESCE(" & Header & ".""FourthX"",0) AS ""Fourth"","
                SumQuery += "COALESCE(" & Header & ".""FifthX"",0) AS ""Fifth"","
                SumQuery += "COALESCE(" & Header & ".""FirstX""+" & Header & ".""SecondX""+" & Header & ".""ThirdX""+" & Header & ".""FourthX""+" & Header & ".""FifthX"",0) AS ""TotalAmount"" "
            ElseIf str = "Header2" Then
                SumQuery = "SUM(" & Header & ".""First"") AS ""First"","
                SumQuery += "SUM(" & Header & ".""Second"") AS ""Second"","
                SumQuery += "SUM(" & Header & ".""Third"") AS ""Third"","
                SumQuery += "SUM(" & Header & ".""Fourth"") AS ""Fourth"","
                SumQuery += "SUM(" & Header & ".""Fifth"") AS ""Fifth"","
                SumQuery += "SUM(" & Header & ".""First"")+SUM(" & Header & ".""Second"")+SUM(" & Header & ".""Third"")+SUM(" & Header & ".""Fourth"")+SUM(" & Header & ".""Fifth"") AS ""TotalAmount"" "
            ElseIf str = "Tail" Then
                SumQuery = "CASE WHEN SUM(" & Header & ".""First"") != 0 THEN " +
                    "CASE WHEN SUM(" & Header & ".""First"") > PAID_AMT.PaidAmount THEN SUM(" & Header & ".""First"") - PAID_AMT.PaidAmount " +
                    "WHEN SUM(" & Header & ".""First"") < PAID_AMT.PaidAmount THEN 0.0 END " +
                    "ELSE 0.0 END AS ""FirstX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Second"") != 0 THEN " +
                    "CASE WHEN PAID_AMT.PaidAmount - SUM(" & Header & ".""First"") < 0 THEN SUM(" & Header & ".""Second"") ELSE " +
                    "CASE WHEN SUM(" & Header & ".""Second"") > (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) THEN SUM(" & Header & ".""Second"") - (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) " +
                    "WHEN SUM(" & Header & ".""Second"") < (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""SecondX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Third"") != 0 THEN " +
                    "CASE WHEN (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"") < 0 THEN SUM(" & Header & ".""Third"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Third"") > ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) " +
                    "THEN SUM(" & Header & ".""Third"") - (PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"") " +
                    "WHEN SUM(" & Header & ".""Third"") < ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""ThirdX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Fourth"") != 0 THEN " +
                    "CASE WHEN ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") < 0 THEN SUM(" & Header & ".""Fourth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Fourth"") > ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") " +
                    "THEN SUM(" & Header & ".""Fourth"") - ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") " +
                    "WHEN SUM(" & Header & ".""Fourth"") < ((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""FourthX"","
                SumQuery += "CASE WHEN SUM(" & Header & ".""Fifth"") != 0 THEN " +
                    "CASE WHEN (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") < 0 THEN SUM(" & Header & ".""Fifth"") " +
                    "ELSE CASE WHEN SUM(" & Header & ".""Fifth"") > (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") " +
                    "THEN SUM(" & Header & ".""Fifth"") - (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") " +
                    "WHEN SUM(" & Header & ".""Fifth"") < (((PAID_AMT.PaidAmount - SUM(" & Header & ".""First"")) - SUM(" & Header & ".""Second"")) - SUM(" & Header & ".""Third"")) - SUM(" & Header & ".""Fourth"") THEN 0.0 END " +
                    "END ELSE 0.0 END AS ""FifthX"" "
            End If

        End If

        Return SumQuery

    End Function

#End Region

#Region "Page_Unload"

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        'MyReportDocument.Close()
        'MyReportDocument.Dispose()
    End Sub

#End Region

End Class
