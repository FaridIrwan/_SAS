Imports System.Collections.Generic
Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
'Imports CrystalDecisions.Shared.ParameterField
Imports System.Linq
Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection
Imports System.Reflection.Emit

Partial Class GroupReport_RptIncomeReportViewer
    Inherits System.Web.UI.Page
    Private MyReportDocument As New ReportDocument
    Private _ReportHelper As New ReportHelper

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try

            If Not IsPostBack Then
                Dim str As String = Nothing
                Dim str2 As String = Nothing
                Dim bobj1 As New FeeTypesDAL
                Dim eobj1 As New List(Of FeeTypesEn)
                Dim lstFeeType As New List(Of String)
                Dim objSpon As String = " "
                Dim ListObjects As New List(Of AccountsEn)
                Dim ListObjects2 As New List(Of AccountsEn)
                Dim bobje As New AccountsDAL
                'Dim stustatus As String
                If Session("report") = "1" Then
                    Dim listobjspon As New List(Of StudentEn)
                    Dim obje As String = Nothing
                    Dim obje1 As String = Nothing
                    Dim obje2 As String = Nothing
                    Dim obje3 As String = Nothing
                    Dim objeall As String = Nothing
                    Dim datefromm As String = Nothing
                    Dim datetoo As String = Nothing
                    'listobjspon.Add(objSpon)
                    If Not Session("radiodate") Is Nothing Then
                        Dim datef As String = Request.QueryString("fdate")
                        Dim datet As String = Request.QueryString("tdate")
                        Dim d1, m1, y1, d2, m2, y2 As String
                        d1 = Mid(datef, 1, 2)
                        m1 = Mid(datef, 4, 2)
                        y1 = Mid(datef, 7, 4)
                        Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                        d2 = Mid(datet, 1, 2)
                        m2 = Mid(datet, 4, 2)
                        y2 = Mid(datet, 7, 4)
                        Dim dateto As String = y2 + "/" + m2 + "/" + d2

                        ListObjects = bobje.IncomeGroupFee(datef, datet, datefrom, dateto)

                        'If Not Session("selectall") Is Nothing Then

                        'Else

                        If Not Session("LstFeeObj").ToString = "0" Then
                            eobj1 = Session("LstFeeObj")
                            ListObjects2 = ListObjects.Where(Function(x) eobj1.Any(Function(y) y.FeeTypeCode = x.ReferenceCode)).ToList()
                            lstFeeType = eobj1.Select(Function(x) x.FeeTypeCode).Distinct().ToList()
                        Else

                            'str += ""
                        End If


                        'End If
                        datefromm = datef
                        datetoo = datet
                    End If
                    If Not Session("radioyear") Is Nothing Then
                        Dim datef As String = Request.QueryString("fdate")
                        Dim datet As String = Request.QueryString("tdate")
                        Dim d1, m1, d2, m2 As String
                        d1 = "01"
                        m1 = "01"
                        Dim datefrom As String = datef + "/" + m1 + "/" + d1
                        d2 = "01"
                        m2 = "01"
                        Dim dateto As String = datet + "/" + m2 + "/" + d2

                        ListObjects = bobje.IncomeGroupFee(datef, datet, datefrom, dateto)

                        'If Not Session("selectall") Is Nothing Then

                        'Else

                        If Not Session("LstFeeObj").ToString = "0" Then
                            eobj1 = Session("LstFeeObj")
                            ListObjects2 = ListObjects.Where(Function(x) eobj1.Any(Function(y) y.FeeTypeCode = x.ReferenceCode)).ToList()
                            lstFeeType = eobj1.Select(Function(x) x.FeeTypeCode).Distinct().ToList()
                        Else

                            'str += ""
                        End If


                        'End If
                        datefromm = datef
                        datetoo = datet
                    End If
                    If Not Session("category") Is Nothing Then

                    End If
                    If Not Session("student") Is Nothing Then
                        obje = "student"

                    ElseIf Session("student") Is Nothing Then
                        obje = ""

                    End If
                    If Not Session("sponsor") Is Nothing Then
                        obje1 = "sponsor"

                    ElseIf Session("sponsor") Is Nothing Then
                        obje1 = ""

                    End If
                    If Not Session("faculty") Is Nothing Then
                        obje2 = "faculty"

                    ElseIf Session("faculty") Is Nothing Then
                        obje2 = ""

                    End If
                    If Not Session("program") Is Nothing Then
                        obje3 = "program"

                    ElseIf Session("program") Is Nothing Then
                        obje3 = ""

                    End If

                    'Report XML Ended
                    If Session("category") Is Nothing And Session("sponsor") Is Nothing And Session("faculty") Is Nothing And Session("program") Is Nothing And Session("student") Is Nothing And Session("stustatus") Is Nothing Then

                        objeall = "selectall"

                    Else
                        objeall = ""

                    End If
                    'Records Checking
                    'str2 = "select 'selectall' as E"
                    'Dim category As String

                    'Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    'Dim _Sponsor As DataSet = GetDataSetGroupFee(objSpon)
                    'Dim hide As Boolean
                    Dim _result As New DataSet()
                    _result.Tables.Add("IncomeGroupFeeDataSet")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("Option")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("student")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("spon")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("prog")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("facul")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("Sponsor")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("faculty")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("Program")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("amount")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("RefCode")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("Desc")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("MatricNo")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("datefrom")
                    _result.Tables("IncomeGroupFeeDataSet").Columns.Add("dateto")
                    For Each item In ListObjects2
                        Dim newRow As DataRow = _
                            _result.Tables("IncomeGroupFeeDataSet").NewRow()
                        newRow("Option") = objeall
                        newRow("student") = obje
                        newRow("spon") = obje1
                        newRow("prog") = obje3
                        newRow("facul") = obje2
                        If item.PayeeName = Nothing Then
                            newRow("Sponsor") = ""
                        Else
                            newRow("Sponsor") = item.PayeeName
                        End If

                        newRow("faculty") = item.SubReferenceOne
                        newRow("Program") = item.SubReferenceTwo
                        newRow("amount") = item.TransactionAmount
                        newRow("RefCode") = item.ReferenceCode
                        newRow("Desc") = item.Description
                        newRow("MatricNo") = item.CreditRef
                        newRow("datefrom") = datefromm
                        newRow("dateto") = datetoo
                        _result.Tables("IncomeGroupFeeDataSet").Rows.Add(newRow)
                    Next
                    Dim _IncomeGroupFeeDataSet As DataSet = _result

                    _IncomeGroupFeeDataSet.Tables(0).TableName = "IncomeGroupFeeDataSet"
                    '_DataSet.Tables.Add(_Sponsor.Tables(0).Copy())
                    '_DataSet.Tables( = "IncomeGroupfee1"
                    Dim s As String = Server.MapPath("~/xml/IncomeGroupfee.xml")


                    _IncomeGroupFeeDataSet.WriteXml(s)
                    If _IncomeGroupFeeDataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")
                    Else

                        'Report Loading 

                        'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                        MyReportDocument.Load(Server.MapPath("~/GroupReport/RptIncomeGroupFee.rpt"))
                        MyReportDocument.SetDataSource(_IncomeGroupFeeDataSet)
                        'MyReportDocument.OpenSubreport("RptGroupFee1").SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()

                        'Report Ended

                    End If


                ElseIf Session("report") = "2" Then
                    If Not Session("ddlFaculty") Is Nothing Then
                        Dim ddl As String = Session("ddlFaculty")
                        Dim semfrom As String = Session("ddlsemfrom")
                        Dim semto As String = Session("ddlsemto")
                        Dim bobj As New StudentBAL
                        Dim eobj As New StudentEn
                        Dim lstfacul As New List(Of FacultyEn)
                        Dim facul As New FacultyDAL

                        str = "select distinct sf.safc_code,sf.safc_desc,count(distinct sa.creditref) as bilangan,0 as bil, sum(sad.transamount) as transamount ,0 as amount," +
                            " substring(sc.semester from 9 for Length(sc.semester)) as sem,'" & semfrom.Replace("/", "").Replace("-", "") & "'as semfrom,'" & semto.Replace("/", "").Replace("-", "") & "' as semto " +
                            " from sas_accounts sa left join sas_accountsdetails sad on " +
                            " sad.transid = sa.transid left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_faculty sf on sf.safc_code " +
                            " = ss.sasi_faculty left join sas_afc sc on sc.batchcode = sa.batchcode where sa.poststatus = 'Posted' and sa.category = 'AFC' " +
                            " and sc.semester = '" & semfrom.Replace("/", "").Replace("-", "") & "'" +
                            " group by sf.safc_code,sf.safc_desc,sc.semester" +
                            " union " +
                            "select distinct sf.safc_code,sf.safc_desc,0 as bil,count(distinct sa.creditref) as bilangan,0 as amount, sum(sad.transamount) as transamount ," +
                            " substring(sc.semester from 9 for Length(sc.semester)) as sem,'" & semfrom.Replace("/", "").Replace("-", "") & "' as semfrom,'" & semto.Replace("/", "").Replace("-", "") & "'as semto " +
                            " from sas_accounts sa left join sas_accountsdetails sad on " +
                            " sad.transid = sa.transid left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_faculty sf on sf.safc_code " +
                            " = ss.sasi_faculty left join sas_afc sc on sc.batchcode = sa.batchcode where sa.poststatus = 'Posted' and sa.category = 'AFC' " +
                            " and sc.semester = '" & semto.Replace("/", "").Replace("-", "") & "'" +
                            " group by sf.safc_code,sf.safc_desc,sc.semester"
                        'str2 = "select distinct sf.safc_code,sf.safc_desc,count(distinct sa.creditref) as bilangan, sum(sad.transamount) as transamount ," +
                        '    " substring(sc.semester from 9 for Length(sc.semester)) as sem,'" & semto.Replace("/", "").Replace("-", "") & "'as semto from sas_accounts sa left join sas_accountsdetails sad on " +
                        '    " sad.transid = sa.transid left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_faculty sf on sf.safc_code " +
                        '    " = ss.sasi_faculty left join sas_afc sc on sc.batchcode = sa.batchcode where sa.poststatus = 'Posted' and sa.category = 'AFC' " +
                        '    " and sc.semester = '" & semto.Replace("/", "").Replace("-", "") & "'" +
                        '    " group by sf.safc_code,sf.safc_desc,sc.semester"
                        'str as Incomebyfaculty
                        'str2 as 1
                        Dim ObjFacultyEn As New FacultyEn
                        Dim ObjFacultyBAL As New FacultyBAL
                        Dim LstObjFaculty As New List(Of FacultyEn)
                        If ddl.ToString = "0" Then
                            ObjFacultyEn.SAFC_Code = "%"
                        Else
                            ObjFacultyEn.SAFC_Code = Session("ddlFaculty")
                        End If
                        LstObjFaculty = ObjFacultyBAL.GetList(ObjFacultyEn)


                        Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)


                        'Dim _Sponsor As DataSet = GetDataSetSponsor(objSpon)
                        Dim _result As New DataSet()
                        _result.Tables.Add("IncomefacultyDataSet")
                        _result.Tables("IncomefacultyDataSet").Columns.Add("Code")
                        _result.Tables("IncomefacultyDataSet").Columns.Add("FAKULTI")

                        For Each item As FacultyEn In LstObjFaculty
                            Dim newRow As DataRow = _
                                _result.Tables("IncomefacultyDataSet").NewRow()
                            newRow("Code") = item.SAFC_Code
                            newRow("FAKULTI") = item.SAFC_Desc
                            _result.Tables("IncomefacultyDataSet").Rows.Add(newRow)
                        Next
                        Dim _IncomefacultyDataSet As DataSet = _result
                        _DataSet.Tables.Add(_IncomefacultyDataSet.Tables(0).Copy())
                        '_DataSet.Tables.Add(_Sponsor.Tables(0).Copy())
                        _DataSet.Tables(0).TableName = "IncomebyFaculty"
                        Dim s As String = Server.MapPath("~/xml/IncomebyFaculty.xml")


                        _DataSet.WriteXml(s)

                        'Report XML Ended

                        'Records Checking

                        If _DataSet.Tables(0).Rows.Count = 0 Then
                            Response.Write("No Record Found")
                        Else

                            'Report Loading 

                            'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                            MyReportDocument.Load(Server.MapPath("~/GroupReport/RptIncomebyFaculty.rpt"))
                            MyReportDocument.SetDataSource(_DataSet)
                            Session("reportobject") = MyReportDocument
                            CrystalReportViewer1.ReportSource = MyReportDocument
                            CrystalReportViewer1.DataBind()
                            MyReportDocument.Refresh()

                            'Report Ended

                        End If
                    End If
                ElseIf Session("report") = "3" Then
                    'If Not Session("invoiceno") Is Nothing And Not Session("invoicedate") Is Nothing Then

                    'End If

                    Dim datef As String = Request.QueryString("fdate")
                    Dim datet As String = Request.QueryString("tdate")
                    Dim d1, m1, y1, d2, m2, y2 As String
                    d1 = Mid(datef, 1, 2)
                    m1 = Mid(datef, 4, 2)
                    y1 = Mid(datef, 7, 4)
                    Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                    d2 = Mid(datet, 1, 2)
                    m2 = Mid(datet, 4, 2)
                    y2 = Mid(datet, 7, 4)
                    Dim dateto As String = y2 + "/" + m2 + "/" + d2


                    str = "select distinct sa.transcode,sa.creditref,sa.transdate,sa.transamount,sa.description from sas_accounts sa " +
                          "where sa.Poststatus = 'Posted' and sa.transdate between'" + datef + "' and '" + datet + "'" +
                          " group by sa.transcode,sa.creditref,sa.transdate,sa.transamount,sa.description " + ""


                    If Not Session("chkinvoicenoo") Is Nothing Then
                        str += " order by sa.transcode asc"
                    End If
                    If Not Session("chkmatricno") Is Nothing Then
                        str += " order by sa.creditref asc"
                    End If

                    Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                    _DataSet.Tables(0).TableName = "IncomebyMyMohe"
                    Dim s As String = Server.MapPath("~/xml/IncomebyMyMohe.xml")


                    _DataSet.WriteXml(s)

                    'Report XML Ended

                    'Records Checking

                    If _DataSet.Tables(0).Rows.Count = 0 Then
                        Response.Write("No Record Found")
                    Else

                        'Report Loading 

                        'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                        MyReportDocument.Load(Server.MapPath("~/GroupReport/RptIncomeMyMohe.rpt"))
                        MyReportDocument.SetDataSource(_DataSet)
                        Session("reportobject") = MyReportDocument
                        CrystalReportViewer1.ReportSource = MyReportDocument
                        CrystalReportViewer1.DataBind()
                        MyReportDocument.Refresh()

                        'Report Ended

                    End If
                    'End If
                ElseIf Session("report") = "4" Then
                    If Not Session("typemyra1") Is Nothing Then
                        Dim datef As String = Request.QueryString("fdate")
                        Dim datet As String = Request.QueryString("tdate")
                        Dim d1, m1, y1, d2, m2, y2 As String
                        d1 = Mid(datef, 1, 2)
                        m1 = Mid(datef, 4, 2)
                        y1 = Mid(datef, 7, 4)
                        Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                        d2 = Mid(datet, 1, 2)
                        m2 = Mid(datet, 4, 2)
                        y2 = Mid(datet, 7, 4)
                        Dim dateto As String = y2 + "/" + m2 + "/" + d2


                        str = "select sdt.SADT_Desc,sum(sad.transamount) as transamount from sas_accounts sa inner join sas_accountsdetails sad " +
                              "on sa.transid = sad.transid left join sas_student ss on ss.sasi_matricno = sa.creditref" +
                              " left join sas_program sp on sp.sapg_code = ss.sasi_pgid left join SAS_DegreeType sdt on sp.sapg_programtype = sdt.sadt_code " +
                              "where sa.poststatus = 'Posted' and sdt.sadt_code = 'PG' " +
                              " and sa.subtype = 'Student' and sa.category NOT IN('Payment','SPA','STA','Refund','Loan','Receipt') " +
                              "and sa.transdate between '" + datefrom + "' and '" + dateto + "'" + " group by sdt.SADT_Desc" + ""

                        Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                        _DataSet.Tables(0).TableName = "IncomebyMyraSummaryGross"
                        Dim s As String = Server.MapPath("~/xml/IncomebyMyraSummaryGross.xml")


                        _DataSet.WriteXml(s)

                        'Report XML Ended

                        'Records Checking

                        If _DataSet.Tables(0).Rows.Count = 0 Then
                            Response.Write("No Record Found")
                        Else

                            'Report Loading 

                            'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                            MyReportDocument.Load(Server.MapPath("~/GroupReport/RptIncomeMyrasummarygross.rpt"))
                            MyReportDocument.SetDataSource(_DataSet)
                            Session("reportobject") = MyReportDocument
                            CrystalReportViewer1.ReportSource = MyReportDocument
                            CrystalReportViewer1.DataBind()
                            MyReportDocument.Refresh()

                            'Report Ended

                        End If
                    End If
                    If Not Session("typemyra2") Is Nothing Then
                        If Not Session("ddlFaculty") Is Nothing Then
                            Dim faculty As String = Session("ddlFaculty")
                            Dim datef As String = Request.QueryString("fdate")
                            Dim datet As String = Request.QueryString("tdate")
                            Dim d1, m1, y1, d2, m2, y2 As String
                            d1 = Mid(datef, 1, 2)
                            m1 = Mid(datef, 4, 2)
                            y1 = Mid(datef, 7, 4)
                            Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                            d2 = Mid(datet, 1, 2)
                            m2 = Mid(datet, 4, 2)
                            y2 = Mid(datet, 7, 4)
                            Dim dateto As String = y2 + "/" + m2 + "/" + d2

                            If faculty.ToString = "0" Then
                                str = "select sa.transcode,ss.sasi_matricno,ss.sasi_name,sa.transdate,ss.sasi_pgid,sf.safc_desc,sp.sapg_program,sum(sad.transamount) as transamount," +
                                      "Count(*)As NoStudent,'" & Session("txtlevel") & "' as level,sa.description " +
                                      "from sas_accounts sa inner join sas_accountsdetails sad on sa.transid = sad.transid" +
                                      " left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_program sp on sp.sapg_code = ss.sasi_pgid " +
                                      "left join SAS_DegreeType sdt on sp.sapg_programtype = sdt.sadt_code" +
                                      " left join sas_faculty sf on sf.safc_code = ss.sasi_faculty " +
                                      "where sa.poststatus = 'Posted'" +
                                      " and sdt.sadt_code = 'PG'  and sa.subtype = 'Student' and sa.category " +
                                      " NOT IN('Payment','SPA','STA','Refund','Loan','Receipt') and sa.transdate between '" + datefrom + "' and '" + dateto + "'" +
                                      " group by sa.transcode,ss.sasi_matricno,ss.sasi_name,sa.transdate,ss.sasi_pgid,sf.safc_desc,sp.sapg_program,sa.description" + ""

                            Else
                                str = "select sa.transcode,ss.sasi_matricno,ss.sasi_name,sa.transdate,ss.sasi_pgid,sf.safc_desc,sp.sapg_program,sum(sad.transamount) as transamount," +
                                      "Count(*)As NoStudent,'" & Session("txtlevel") & "' as level,sa.description " +
                                      "from sas_accounts sa inner join sas_accountsdetails sad on sa.transid = sad.transid" +
                                      " left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_program sp on sp.sapg_code = ss.sasi_pgid " +
                                      "left join SAS_DegreeType sdt on sp.sapg_programtype = sdt.sadt_code" +
                                      " left join sas_faculty sf on sf.safc_code = ss.sasi_faculty " +
                                      "where sf.safc_code = '" & Session("ddlFaculty") & "' and sp.sapg_code = '" & Session("program") & "' and sa.poststatus = 'Posted'" +
                                      " and sdt.sadt_code = 'PG'  and sa.subtype = 'Student' and sa.category " +
                                      " NOT IN('Payment','SPA','STA','Refund','Loan','Receipt') and sa.transdate between '" + datefrom + "' and '" + dateto + "'" +
                                      " group by sa.transcode,ss.sasi_matricno,ss.sasi_name,sa.transdate,ss.sasi_pgid,sf.safc_desc,sp.sapg_program,sa.description" + ""
                            End If

                            Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                            _DataSet.Tables(0).TableName = "IncomebyMyraGrossIncome"
                            Dim s As String = Server.MapPath("~/xml/IncomebyMyraGrossIncome.xml")


                            _DataSet.WriteXml(s)

                            'Report XML Ended

                            'Records Checking

                            If _DataSet.Tables(0).Rows.Count = 0 Then
                                Response.Write("No Record Found")
                            Else

                                'Report Loading 

                                'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                                MyReportDocument.Load(Server.MapPath("~/GroupReport/RptIncomeMyraGrossIncome.rpt"))
                                MyReportDocument.SetDataSource(_DataSet)
                                Session("reportobject") = MyReportDocument
                                CrystalReportViewer1.ReportSource = MyReportDocument
                                CrystalReportViewer1.DataBind()
                                MyReportDocument.Refresh()

                                'Report Ended

                            End If
                        End If
                    End If
                    If Not Session("typemyra3") Is Nothing Then
                        Dim datef As String = Request.QueryString("fdate")
                        Dim datet As String = Request.QueryString("tdate")
                        Dim d1, m1, y1, d2, m2, y2 As String
                        d1 = Mid(datef, 1, 2)
                        m1 = Mid(datef, 4, 2)
                        y1 = Mid(datef, 7, 4)
                        Dim datefrom As String = y1 + "/" + m1 + "/" + d1
                        d2 = Mid(datet, 1, 2)
                        m2 = Mid(datet, 4, 2)
                        y2 = Mid(datet, 7, 4)
                        Dim dateto As String = y2 + "/" + m2 + "/" + d2


                        str = "select sp.sapg_program,Count(sa.creditref)As NoStudent,sum(sad.transamount) as transamount,sa.transdate from sas_accounts sa inner join sas_accountsdetails sad on " +
                              "sa.transid = sad.transid left join sas_student ss on ss.sasi_matricno = sa.creditref left join sas_program sp on sp.sapg_code = ss.sasi_pgid " +
                              "left join SAS_DegreeType sdt on sp.sapg_programtype = sdt.sadt_code left join sas_faculty sf on sf.safc_code = ss.sasi_faculty " +
                              "where sa.poststatus = 'Posted' and sdt.sadt_code = 'PG' and sa.subtype = 'Student' and sa.category  NOT IN('Payment','SPA','STA','Refund','Loan','Receipt')" +
                              " and sa.transdate between '" + datefrom + "' and '" + dateto + "'" +
                              " group by sp.sapg_program,sa.transdate" + ""

                        Dim _DataSet As DataSet = _ReportHelper.GetDataSet(str)
                        _DataSet.Tables(0).TableName = "Incomebasedonprogram"
                        Dim s As String = Server.MapPath("~/xml/Incomebasedonprogram.xml")


                        _DataSet.WriteXml(s)

                        'Report XML Ended

                        'Records Checking

                        If _DataSet.Tables(0).Rows.Count = 0 Then
                            Response.Write("No Record Found")
                        Else

                            'Report Loading 

                            'MyReportDocument.Load(Server.MapPath("RptStuAgeingCRFinal.rpt"))
                            MyReportDocument.Load(Server.MapPath("~/GroupReport/RptSummaryIncomebasedonprogram.rpt"))
                            MyReportDocument.SetDataSource(_DataSet)
                            Session("reportobject") = MyReportDocument
                            CrystalReportViewer1.ReportSource = MyReportDocument
                            CrystalReportViewer1.DataBind()
                            MyReportDocument.Refresh()

                            'Report Ended

                        End If
                    End If






                End If

                'End If


            Else

                'Report Loading

                MyReportDocument = Session("reportobject")
                CrystalReportViewer1.ReportSource = MyReportDocument
                CrystalReportViewer1.DataBind()
                MyReportDocument.Refresh()

                'Report Ended

            End If
            'End If

        Catch ex As Exception

            Response.Write(ex.Message)

        End Try
    End Sub

    '' ''http://www.codeproject.com/Articles/16252/Generic-List-Of-to-DataSet-Three-Approaches
    Private Function GetIncomefacultyDataSet(ByVal list As  _
          List(Of StudentEn)) As DataSet
        Dim _result As New DataSet()
        _result.Tables.Add("IncomefacultyDataSet")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_code")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_title")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_ourref")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_yourref")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_address")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_message")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_signby")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_name")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_frdate")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_todate")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_updatedby")
        _result.Tables("IncomefacultyDataSet").Columns.Add("sascl_updatedtime")

        For Each item As StudentEn In list
            Dim newRow As DataRow = _
                _result.Tables("IncomefacultyDataSet").NewRow()
            'newRow("sascl_code") = item.
            'newRow("sascl_address") = item.Address
            'newRow("sascl_frdate") = item.FromDate
            'newRow("sascl_todate") = item.ToDate
            'newRow("sascl_title") = item.Title
            'newRow("sascl_ourref") = item.OurRef
            'newRow("sascl_yourref") = item.YourRef
            'newRow("sascl_message") = item.Message
            'newRow("sascl_signby") = item.SignBy
            'newRow("sascl_name") = item.Name
            'newRow("sascl_updatedby") = item.UpdatedBy
            'newRow("sascl_updatedtime") = item.UpdatedTime
            _result.Tables("IncomefacultyDataSet").Rows.Add(newRow)
        Next
        Return _result
    End Function

    'Private Function GetDataSetSponsor(ByVal obj As SponsorEn) As DataSet
    '    Dim _result As New DataSet()
    '    _result.Tables.Add("Sponsor")
    '    _result.Tables("Sponsor").Columns.Add("sasr_code")
    '    _result.Tables("Sponsor").Columns.Add("sasr_name")
    '    _result.Tables("Sponsor").Columns.Add("sassr_sname")
    '    _result.Tables("Sponsor").Columns.Add("sasr_address")
    '    _result.Tables("Sponsor").Columns.Add("sasr_address1")
    '    _result.Tables("Sponsor").Columns.Add("sasr_address2")
    '    _result.Tables("Sponsor").Columns.Add("sasr_contact")
    '    _result.Tables("Sponsor").Columns.Add("sasr_phone")

    '    Dim newRow As DataRow = _result.Tables("Sponsor").NewRow()
    '    newRow("sasr_code") = obj.SponserCode
    '    newRow("sasr_name") = obj.SponsorName
    '    newRow("sassr_sname") = obj.SName
    '    newRow("sasr_address") = obj.Address
    '    newRow("sasr_address1") = obj.Address1
    '    newRow("sasr_address2") = obj.Address2
    '    newRow("sasr_contact") = obj.Contact
    '    newRow("sasr_phone") = obj.Phone
    '    _result.Tables("Sponsor").Rows.Add(newRow)
    '    Return _result
    'End Function
    Private Function GetDataSetGroupFee(ByVal objspon As String) As DataSet
        Dim _result As New DataSet()
        _result.Tables.Add("GetDataSetGroupFee")
        _result.Tables("GetDataSetGroupFee").Columns.Add("student")

        For Each item In objspon
            Dim newRow As DataRow = _result.Tables("GetDataSetGroupFee").NewRow()
            newRow("student") = objspon
            _result.Tables("GetDataSetGroupFee").Rows.Add(newRow)
        Next


        Return _result
    End Function
End Class
