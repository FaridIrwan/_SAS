Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.Entities
Imports System.Data
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports HTS.SAS.DataAccessObjects
Imports SQLPowerQueryManager.PowerQueryManager
Imports System.Linq
Imports System.IO
Imports System.Net.Mail
Imports System.Net
Imports System.Configuration
Imports System.Net.Configuration

Partial Class RptIncomeReport
    Inherits System.Web.UI.Page
#Region "Global Declarations "
    'Global Declaration - Starting
    'Private _Helper As New Helper
    'Private ListTRD As New List(Of AccountsDetailsEn)
    'Private ListObjects As List(Of AccountsEn)
    'Private _GstSetupDal As New HTS.SAS.DataAccessObjects.GSTSetupDAL

    'Dim listStu As New List(Of StudentEn)
    'Private ErrorDescription As String
    'Private sumAmt As Double = 0
    'Dim sumGST As Double = 0
    'Dim DFlag As String
    'Dim AutoNo As Boolean
    'Dim ListObjectsStudent As List(Of StudentEn)
    'Dim MJ_JnlLineEn As New BusinessEntities.MJ_JnlLine
    'Dim MJ_JnlHdrEn As New BusinessEntities.MJ_JnlHdr
    'Dim AutoNumberEn As New BusinessEntities.AutoNumberEn
    'Dim AccountEn As New BusinessEntities.AccountsEn
    'Dim objIntegrationDL As New SQLPowerQueryManager.PowerQueryManager.IntegrationDL
    'Dim objIntegration As New IntegrationModule.IntegrationNameSpace.Integration

    'Dim dsReturn As New DataSet
    'Dim dsReturn_II As New DataSet

    ''Selection Criteria Declaration - start
    'Private scFaculty As String
    Private scfee As New List(Of String)
    'Private scSponsor As New List(Of String)
    'Private scHostel As New List(Of String)
    'Private scStudentCategory As String
    'Private scSemester As String
    'Private getStudentDetailsChange As New List(Of StudentEn)
    ''Selection Criteria Declaration - start
    'Private StuChgMatricNo As New List(Of StudentEn)
    'Private StuToSave As New List(Of StudentEn)
    'Global Declaration - Ended
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            Menuname(CInt(Request.QueryString("Menuid")))
            Session("Menuid") = Request.QueryString("Menuid")

            'ibtnTodate.Attributes.Add("onClick", "return getDateto()")

            
            ibtnPrint.Attributes.Add("onclick", "return getDate()")


            'txtTodate.Attributes.Add("OnKeyup", "return CheckToDate()")
            Image2.Attributes.Add("onClick", "return getimage2()")
            Image4.Attributes.Add("onClick", "return getimage4()")
            ibtnFDate.Attributes.Add("onClick", "return getibtnFDate()")
            ibtnTodate.Attributes.Add("onClick", "return getibtnTodate()")
            ibtnyearfrom.Attributes.Add("onClick", "return getibtnyearfrom()")
            ibtnyearto.Attributes.Add("onClick", "return getibtnyearto()")
            Faculty()
            fee()
            Program()
            semfrom()
            semto()
            dates()
            'schdept()
            stustatus()
            'ddlprogram.Items.Insert(0, New ListItem("--Please Select--", "-1"))

            Session("report") = Nothing
            Session("program") = Nothing
            Session("faculty") = Nothing
            Session("status") = Nothing
            Session("sortby") = Nothing
            Session("chkmatricno") = Nothing
            Session("chkinvoicenoo") = Nothing
            Session("typemyra1") = Nothing
            Session("typemyra2") = Nothing
            Session("typemyra3") = Nothing
            Session("radiodate") = Nothing
            Session("radioyear") = Nothing
            'Session("LstObjFeeTypes") = Nothing
            Session("scfee") = Nothing
            Session("LstStustatus") = Nothing
            Session("LstFeeObj") = Nothing
            Session("selectall") = Nothing
            Session("category") = Nothing
            Session("sponsor") = Nothing
            Session("faculty") = Nothing
            Session("program") = Nothing
            Session("student") = Nothing
            Session("stustatus") = Nothing
        End If
    End Sub
    Private Sub Menuname(ByVal MenuId As Integer)
        Dim eobj As New MenuEn
        Dim bobj As New MenuBAL
        eobj.MenuId = MenuId
        eobj = bobj.GetMenus(eobj)
        lblMenuName.Text = eobj.MenuName
    End Sub
    Private Sub dates()
        'txtFrom.Text = Format(CDate("01/01/2013"), "dd/MM/yyyy")
        'txtTodate.Text = Format(Date.Now, "dd/MM/yyyy")
    End Sub

    Private Sub Faculty()
        Dim ObjFacultyEn As New FacultyEn
        Dim ObjFacultyBAL As New FacultyBAL
        Dim LstObjFaculty As New List(Of FacultyEn)
        ObjFacultyEn.SAFC_Code = "%"
        LstObjFaculty = ObjFacultyBAL.GetList(ObjFacultyEn)
        ddlfaculty.Items.Clear()
        'ddlFaculty.Items.Add(New ListItem("-- Select --", "-1"))
        ddlfaculty.DataTextField = "SAFC_Desc"
        ddlfaculty.DataValueField = "SAFC_Code"
        ddlfaculty.DataSource = LstObjFaculty
        ddlfaculty.DataBind()
        ddlfaculty.Items.Insert(0, New ListItem("--Please Select--", "-1"))
        ddlfaculty.Items.Insert(1, New ListItem("Select All", "0"))
    End Sub

    Private Sub fee()
        Dim ObjFeeTypesEn As New FeeTypesEn
        Dim ObjFeeTypesBAL As New FeeTypesDAL
        Dim LstObjFeeTypes As New List(Of FeeTypesEn)
        ObjFeeTypesEn.FeeTypeCode = "%"
        ObjFeeTypesEn.FeeType = "%"
        ObjFeeTypesEn.Hostel = "%"
        ObjFeeTypesEn.Description = "%"
        ObjFeeTypesEn.GLCode = "%"
        ObjFeeTypesEn.Status = True
        LstObjFeeTypes = ObjFeeTypesBAL.GetFeeTypesList(ObjFeeTypesEn)
        Session("LstObjFeeTypes") = LstObjFeeTypes
        dgFee.DataSource = LstObjFeeTypes
        dgFee.DataBind()

    End Sub
    Protected Sub dgFee_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgFee.ItemDataBound
        If Not Session("scfee") Is Nothing Then
            scfee = Session("scfee")
        Else
            scfee = New List(Of String)
        End If
        Dim Chk As CheckBox

        If chkSelectAllFee.Checked = True Then
            Select Case e.Item.ItemType
                Case ListItemType.Item, ListItemType.AlternatingItem
                    Chk = CType(e.Item.FindControl("chkFee"), CheckBox)
                    Chk.Checked = True
                    If Not scfee.Contains(e.Item.Cells(1).Text) Then
                        scfee.Add(e.Item.Cells(1).Text)
                        Session("scfee") = scfee
                    End If
            End Select
        End If
    End Sub

    Private Sub Program()
        Dim ObjProgramEn As New ProgramInfoEn
        Dim ObjProgramBAL As New ProgramInfoBAL
        Dim LstProgram As New List(Of ProgramInfoEn)
        If ddlfaculty.SelectedValue = "-1" Then

            ObjProgramEn.SAFC_Code = ddlfaculty.SelectedValue

            LstProgram = ObjProgramBAL.GetAllProgramInfoList(ObjProgramEn.SAFC_Code)
            ddlprogram.Items.Clear()
            'ddlProgram.Items.Add(New ListItem("-- Select --", "-1"))
            ddlprogram.DataSource = LstProgram
            ddlprogram.DataTextField = "Program"
            ddlprogram.DataValueField = "ProgramCode"
            ddlprogram.DataBind()
            ddlprogram.Items.Insert(0, New ListItem("--Please Select--", "-1"))
            ddlprogram.Items.Insert(1, New ListItem("Select All", "0"))
        ElseIf ddlfaculty.SelectedValue = "0" Then
            ObjProgramEn.SAFC_Code = ddlfaculty.SelectedValue


            LstProgram = ObjProgramBAL.GetProgramInfoListAll(ObjProgramEn.SAFC_Code)
            ddlprogram.Items.Clear()
            'ddlProgram.Items.Add(New ListItem("-- Select --", "-1"))
            ddlprogram.DataSource = LstProgram
            ddlprogram.DataTextField = "Program"
            ddlprogram.DataValueField = "ProgramCode"
            ddlprogram.DataBind()
            ddlprogram.Items.Insert(0, New ListItem("--Please Select--", "-1"))
            ddlprogram.Items.Insert(1, New ListItem("Select All", "0"))
        Else
            ObjProgramEn.SAFC_Code = ddlfaculty.SelectedValue


            LstProgram = ObjProgramBAL.GetProgramInfoListAll(ObjProgramEn.SAFC_Code)
            ddlprogram.Items.Clear()
            'ddlProgram.Items.Add(New ListItem("-- Select --", "-1"))
            ddlprogram.DataSource = LstProgram
            ddlprogram.DataTextField = "Program"
            ddlprogram.DataValueField = "ProgramCode"
            ddlprogram.DataBind()
            ddlprogram.Items.Insert(0, New ListItem("--Please Select--", "-1"))
            'ddlprogram.Items.Insert(1, New ListItem("Select All", "0"))
        End If
    End Sub
    Private Sub semfrom()
        Dim eIntake As New SemesterSetupEn
        Dim bIntake As New SemesterSetupBAL
        Dim listIntake As New List(Of SemesterSetupEn)
        ddlsemfrom.Items.Clear()
        ddlsemfrom.Items.Add(New System.Web.UI.WebControls.ListItem("--Select--", "-1"))
        'ddlIntake.Items.Add(New ListItem("All", "1"))
        ddlsemfrom.DataTextField = "SemisterSetupCode"
        ddlsemfrom.DataValueField = "SemisterSetupCode"
        eIntake.SemisterSetupCode = "%"

        Try
            listIntake = bIntake.GetListSemesterCode(eIntake)
        Catch ex As Exception
            LogError.Log("RptIncomeReport", "semfrom", ex.Message)
        End Try
        ddlsemfrom.DataSource = listIntake
        ddlsemfrom.DataBind()
        'Session("faculty") = listfac
    End Sub
    Private Sub semto()
        Dim eIntake As New SemesterSetupEn
        Dim bIntake As New SemesterSetupBAL
        Dim listIntake As New List(Of SemesterSetupEn)
        ddlsemto.Items.Clear()
        ddlsemto.Items.Add(New System.Web.UI.WebControls.ListItem("--Select--", "-1"))
        'ddlIntake.Items.Add(New ListItem("All", "1"))
        ddlsemto.DataTextField = "SemisterSetupCode"
        ddlsemto.DataValueField = "SemisterSetupCode"
        eIntake.SemisterSetupCode = "%"

        Try
            listIntake = bIntake.GetListSemesterCode(eIntake)
        Catch ex As Exception
            LogError.Log("RptIncomeReport", "semto", ex.Message)
        End Try
        ddlsemto.DataSource = listIntake
        ddlsemto.DataBind()
        'Session("faculty") = listfac
    End Sub
    'Private Sub schdept()
    '    'Dim DSReturn As New List(Of DepartmentEn)
    '    'Dim objDepartmentEn As New DepartmentEn
    '    'Dim objDepartmentDL As New DepartmentDAL

    '    'Try
    '    '    DSReturn = objDepartmentDL.GetDepartmentList(objDepartmentEn)

    '    '    If DSReturn IsNot Nothing Then
    '    '        ddlschdept.DataSource = DSReturn
    '    '        ddlschdept.DataTextField = "Department"
    '    '        ddlschdept.DataValueField = "DepartmentID"
    '    '        ddlschdept.DataBind()
    '    '        ddlschdept.Items.Insert(0, New ListItem("--Please Select--", "-1"))
    '    '    Else
    '    '        LogError.Log("User", "FillDepartment", "No Record In Department")
    '    '        lblMsg.Text = "No Record In Department"
    '    '    End If
    '    'Catch ex As Exception
    '    '    LogError.Log("RptIncomeReport", "schdept", ex.Message)
    '    '    lblMsg.Text = ex.Message
    '    'End Try
    '    ''Session("faculty") = listfac
    '    Dim ObjFacultyEn As New FacultyEn
    '    Dim ObjFacultyBAL As New FacultyBAL
    '    Dim LstObjFaculty As New List(Of FacultyEn)
    '    ObjFacultyEn.SAFC_Code = "%"
    '    LstObjFaculty = ObjFacultyBAL.GetList(ObjFacultyEn)
    '    ddlschdept.Items.Clear()
    '    'ddlFaculty.Items.Add(New ListItem("-- Select --", "-1"))
    '    ddlschdept.DataTextField = "SAFC_Desc"
    '    ddlschdept.DataValueField = "SAFC_Code"
    '    ddlschdept.DataSource = LstObjFaculty
    '    ddlschdept.DataBind()
    '    ddlschdept.Items.Insert(0, New ListItem("--Please Select--", "-1"))
    'End Sub
    Private Sub stustatus()
        Dim bStuStatus As New StudentStatusBAL
        Dim eStuStatus As New StudentStatusEn
        Dim LstStustatus As New List(Of StudentStatusEn)
        eStuStatus.StudentStatusCode = ""
        eStuStatus.Description = ""
        eStuStatus.Status = True
        eStuStatus.BlStatus = True
        ddlstudentStatus.Items.Clear()
        ddlstudentStatus.Items.Add(New ListItem("---Select---", "-1"))
        ddlstudentStatus.DataTextField = "Description"
        ddlstudentStatus.DataValueField = "StudentStatusCode"
        'If Session("PageMode") = "Add" Then
        'Try
        '    ddlstudentStatus.DataSource = bStuStatus.GetStudentStatusList(eStuStatus)
        'Catch ex As Exception
        '    LogError.Log("Student", "FillDropDownList", ex.Message)
        'End Try

        'Else

        Try
            LstStustatus = bStuStatus.GetStudentStatusListAll(eStuStatus)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlstudentStatus.DataSource = LstStustatus
        ddlstudentStatus.DataBind()
        Session("LstStustatus") = LstStustatus
    End Sub

    Protected Sub ibtnCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnCancel.Click
        ddltypeofreport.SelectedValue = "-1"

        ddlfaculty.SelectedValue = "-1"
        ddlprogram.SelectedValue = "-1"
        ddlsemfrom.SelectedValue = "-1"
        ddlsemto.SelectedValue = "-1"
        'ddlschdept.SelectedValue = "-1"
        ddlmyra.SelectedValue = "-1"
        'ddllevel.SelectedValue = "-1"
        dates()
        Response.Redirect("/GroupReport/RptIncomeReport.aspx?Menuid=124")
        Session("report") = Nothing
        Session("program") = Nothing
        Session("faculty") = Nothing
        Session("status") = Nothing
        Session("sortby") = Nothing
        Session("chkmatricno") = Nothing
        Session("chkinvoicenoo") = Nothing
        Session("typemyra1") = Nothing
        Session("typemyra2") = Nothing
        Session("typemyra3") = Nothing
        Session("radiodate") = Nothing
        Session("radioyear") = Nothing
        Session("LstObjFeeTypes") = Nothing
        Session("scfee") = Nothing
        Session("LstStustatus") = Nothing
        Session("LstFeeObj") = Nothing
        Session("selectall") = Nothing
        Session("category") = Nothing
        Session("sponsor") = Nothing
        Session("faculty") = Nothing
        Session("program") = Nothing
        Session("student") = Nothing
        Session("stustatus") = Nothing
    End Sub

    Protected Sub ddlFaculty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlfaculty.SelectedIndexChanged

        Dim dropFaculty As String

        dropFaculty = ddlfaculty.SelectedValue

        If dropFaculty = "-1" Then
            Session("ddlFaculty") = Nothing
        Else
            Session("ddlFaculty") = dropFaculty
        End If

        Program()

    End Sub
    Protected Sub ddlProgram_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlprogram.SelectedIndexChanged

        Dim program As String

        program = ddlprogram.SelectedValue

        If program = "-1" Then
            Session("program") = Nothing
        Else
            Session("program") = program
        End If
    End Sub
    Protected Sub ddlsemfrom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsemfrom.SelectedIndexChanged

        Dim semfrom As String

        semfrom = ddlsemfrom.SelectedValue

        If semfrom = "-1" Then
            Session("ddlsemfrom") = Nothing
        Else
            Session("ddlsemfrom") = semfrom
        End If

        'Program()

    End Sub
    Protected Sub ddlsemto_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsemto.SelectedIndexChanged

        Dim semto As String

        semto = ddlsemto.SelectedValue

        If semto = "-1" Then
            Session("ddlsemto") = Nothing
        Else
            Session("ddlsemto") = semto
        End If

        'Program()

    End Sub

    Protected Sub ddltypeofreport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddltypeofreport.SelectedIndexChanged

        Dim report As String

        report = ddltypeofreport.SelectedValue

        If report = "-1" Then
            Response.Redirect("/GroupReport/RptIncomeReport.aspx?Menuid=124")
            Session("report") = Nothing
        ElseIf report = "1" Then
            Loadincomefee()
            Session("report") = report
        ElseIf report = "2" Then
            lblfaculty.Text = "Faculty"
            Loadincomefaculty()
            Session("report") = report
        ElseIf report = "3" Then
            Loadincomemymohe()
            Session("report") = report
        ElseIf report = "4" Then
            Loadincomemyra()
            Session("report") = report
        End If
    End Sub
    Protected Sub ddlstudentStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlstudentStatus.SelectedIndexChanged

        Dim studentStatus As String

        studentStatus = ddlstudentStatus.SelectedValue

        If studentStatus = "-1" Then
            Session("studStatus") = Nothing
        Else
            Session("studStatus") = studentStatus
        End If

        'Program()

    End Sub
    Private Sub Loadincomefee()
        lblfee.Visible = True
        chkSelectAllFee.Visible = True
        dgFee.Visible = True
        pnlIncomegroupfee.Visible = True
        lblfaculty.Visible = False
        ddlfaculty.Visible = False
        lblprogram.Visible = False
        ddlprogram.Visible = False
        lblsemfrom.Visible = False
        ddlsemfrom.Visible = False
        lblsemto.Visible = False
        ddlsemto.Visible = False
        rdrdaterange.Visible = False
        lblfrom.Visible = False
        txtFrom2.Visible = False
        Image2.Visible = False
        lblto.Visible = False
        txtTo.Visible = False
        Image4.Visible = False
        pnlincomemymohe.Visible = False
        lblreportMyRA.Visible = False
        ddlmyra.Visible = False
    End Sub
    Private Sub Loadincomefaculty()
        lblfee.Visible = False
        dgFee.Visible = False
        chkSelectAllFee.Visible = False
        pnlIncomegroupfee.Visible = False
        rdrdaterange.Visible = False
        lblfrom.Visible = False
        txtFrom2.Visible = False
        Image2.Visible = False
        lblto.Visible = False
        txtTo.Visible = False
        Image4.Visible = False
        pnlincomemymohe.Visible = False
        lblreportMyRA.Visible = False
        ddlmyra.Visible = False
        lblfaculty.Visible = True
        ddlfaculty.Visible = True
        lblprogram.Visible = True
        ddlprogram.Visible = True
        lblsemfrom.Visible = True
        ddlsemfrom.Visible = True
        lblsemto.Visible = True
        ddlsemto.Visible = True
    End Sub
    Private Sub Loadincomemymohe()
        lblfee.Visible = False
        dgFee.Visible = False
        chkSelectAllFee.Visible = False
        pnlIncomegroupfee.Visible = False
        lblfaculty.Visible = False
        ddlfaculty.Visible = False
        lblprogram.Visible = False
        ddlprogram.Visible = False
        lblsemfrom.Visible = False
        ddlsemfrom.Visible = False
        lblsemto.Visible = False
        ddlsemto.Visible = False
        rdrdaterange.Visible = True
        lblfrom.Visible = True
        txtFrom2.Visible = True
        Image2.Visible = True
        lblto.Visible = True
        txtTo.Visible = True
        Image4.Visible = True
        pnlincomemymohe.Visible = True
    End Sub
    Private Sub Loadincomemyra()
        lblfee.Visible = False
        dgFee.Visible = False
        chkSelectAllFee.Visible = False
        pnlIncomegroupfee.Visible = False
        lblfaculty.Visible = False
        ddlfaculty.Visible = False
        lblprogram.Visible = False
        ddlprogram.Visible = False
        lblsemfrom.Visible = False
        ddlsemfrom.Visible = False
        lblsemto.Visible = False
        ddlsemto.Visible = False
        pnlincomemymohe.Visible = False
        lblreportMyRA.Visible = True
        ddlmyra.Visible = True
        rdrdaterange.Visible = True
        lblfrom.Visible = True
        txtFrom2.Visible = True
        Image2.Visible = True
        lblto.Visible = True
        txtTo.Visible = True
        Image4.Visible = True
    End Sub


    Protected Sub ddlmyra_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlmyra.SelectedIndexChanged

        Dim typemyra As String

        typemyra = ddlmyra.SelectedValue

        If typemyra = "-1" Then
            Session("typemyra1") = Nothing
            Session("typemyra2") = Nothing
            Session("typemyra3") = Nothing
            Session("txtlevel") = Nothing
        ElseIf typemyra = "1" Then
            Loadsummarygross()
            Session("typemyra1") = typemyra
        ElseIf typemyra = "2" Then
            Dim level As String
            level = Trim(txtlevel.Text)
            Loaddetailsgross()
            lblfaculty.Text = "School/Department"
            Session("typemyra2") = typemyra
            Session("txtlevel") = level
        ElseIf typemyra = "3" Then
            Loadsummarygross()
            Session("typemyra3") = typemyra
        End If

    End Sub
    Private Sub Loadsummarygross()
        chkSelectAllFee.Visible = False
        lblreportMyRA.Visible = True
        lblfaculty.Visible = False
        ddlfaculty.Visible = False
        lblprogram.Visible = False
        ddlprogram.Visible = False
        lbllevel.Visible = False
        txtlevel.Visible = False
        ddlmyra.Visible = True
        rdrdaterange.Visible = True
        lblfrom.Visible = True
        txtFrom2.Visible = True
        Image2.Visible = True
        lblto.Visible = True
        txtTo.Visible = True
        Image4.Visible = True
    End Sub
    Private Sub Loaddetailsgross()
        chkSelectAllFee.Visible = False
        lblreportMyRA.Visible = True
        ddlmyra.Visible = True
        lblfaculty.Visible = True
        ddlfaculty.Visible = True
        lblprogram.Visible = True
        ddlprogram.Visible = True
        lbllevel.Visible = True
        txtlevel.Visible = True
        rdrdaterange.Visible = True
        lblfrom.Visible = True
        txtFrom2.Visible = True
        Image2.Visible = True
        lblto.Visible = True
        txtTo.Visible = True
        Image4.Visible = True
    End Sub
    Protected Sub txtlevel_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        'Dim txtlevell As TextBox
        Dim level As String
        level = Trim(txtlevel.Text)
        Session("txtlevel") = level
    End Sub
    Protected Sub chkSelectAllFee_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim LstObjFeeTypes As List(Of FeeTypesEn)
        Dim LstObjectFeeTypes As New List(Of FeeTypesEn)
        Dim selectall As String

        If Not Session("LstObjFeeTypes") Is Nothing Then
            LstObjFeeTypes = Session("LstObjFeeTypes")
        Else
            LstObjFeeTypes = New List(Of FeeTypesEn)
        End If

        Dim chk As CheckBox
        Dim dgitem As DataGridItem
        If chkSelectAllFee.Checked = True Then
            selectall = chkSelectAllFee.Checked = True
            Session("selectall") = selectall
            For Each dgitem In dgFee.Items
                chk = dgitem.Cells(0).Controls(1)
                chk.Checked = True
                LstObjectFeeTypes.AddRange(LstObjFeeTypes.Where(Function(x) x.FeeTypeCode = dgitem.Cells(1).Text).ToList())
            Next
            'hfProgramCount.Value = dgStudent.Items.Count
        Else
            Session("selectall") = Nothing
            For Each dgitem In dgFee.Items
                chk = dgitem.Cells(0).Controls(1)
                chk.Checked = False
            Next
            'LstStueObj = New List(Of StudentEn)
            'hfProgramCount.Value = 0
        End If
        Session("LstFeeObj") = LstObjectFeeTypes
    End Sub
    Protected Sub chkFee_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim LstObjFeeTypes As List(Of FeeTypesEn)
        Dim LstObjectFeeTypes As New List(Of FeeTypesEn)

        If Not Session("LstObjFeeTypes") Is Nothing Then
            LstObjFeeTypes = Session("LstObjFeeTypes")
        Else
            LstObjFeeTypes = New List(Of FeeTypesEn)
        End If

        Dim chk As CheckBox
        Dim dgitem As DataGridItem
        For Each dgitem In dgFee.Items
            chk = dgitem.Cells(0).Controls(1)
            If chk.Checked = True Then
                LstObjectFeeTypes.AddRange(LstObjFeeTypes.Where(Function(x) x.FeeTypeCode = dgitem.Cells(1).Text).ToList())
            Else
                If chkSelectAllFee.Checked = True Then
                    chkSelectAllFee.Checked = False
                End If
            End If
        Next

        Session("LstFeeObj") = LstObjectFeeTypes
       


    End Sub
    'Protected Sub chkallcategory_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim allcategory As String
    '    If chkcategory.Checked = False And chksponsor.Checked = False And chkfaculty.Checked = False And chkstudent.Checked = False And chkprogram.Checked = False And
    '    chkstatus.Checked = False Then
    '        chkallcategory.Checked = True
    '        allcategory = chkcategory.Checked
    '        Session("chkallcategory") = allcategory
    '    Else
    '        chkcategory.Checked = False
    '        Session("chkallcategory") = Nothing
    '    End If
    'End Sub
    Protected Sub chkcategory_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim category As String


        If chkcategory.Checked = True Then
            category = chkcategory.Checked
            Session("category") = category
        End If
    End Sub
    Protected Sub chksponsor_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sponsor As String


        If chksponsor.Checked = True Then
            sponsor = chksponsor.Checked
            Session("sponsor") = sponsor
        ElseIf chksponsor.Checked = False Then
            'student = chkstudent.Checked
            Session("sponsor") = Nothing
        End If
    End Sub
    Protected Sub chkfaculty_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim faculty As String
       
        If chkfaculty.Checked = True Then
            faculty = chkfaculty.Checked
            Session("faculty") = faculty
        ElseIf chkfaculty.Checked = False Then
            'student = chkstudent.Checked
            Session("faculty") = Nothing
        End If
    End Sub
    Protected Sub chkprogram_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim program As String
      
        If chkprogram.Checked = True Then
            program = chkprogram.Checked
            Session("program") = program
        ElseIf chkprogram.Checked = False Then
            'student = chkstudent.Checked
            Session("program") = Nothing
        End If
    End Sub
    Protected Sub chkstudent_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim student As String
     
        If chkstudent.Checked = True Then
            student = chkstudent.Checked
            Session("student") = student
        ElseIf chkstudent.Checked = False Then
            'student = chkstudent.Checked
            Session("student") = Nothing
        End If
    End Sub
    Protected Sub chkstatus_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim stustatus As String

        If chkstatus.Checked = True Then
            ddlstudentStatus.Visible = True
            stustatus = chkstatus.Checked
            Session("stustatus") = stustatus
        Else
            ddlstudentStatus.Visible = False
        End If
    End Sub
    Protected Sub chkinvoiceno_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim invoiceno As String

        If chkinvoiceno.Checked = True Then
            invoiceno = chkinvoiceno.Checked
            Session("invoiceno") = invoiceno
        End If
    End Sub
    Protected Sub chkinvoicedate_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim invoicedate As String

        If chkinvoicedate.Checked = True Then
            invoicedate = chkinvoicedate.Checked
            Session("invoicedate") = invoicedate
        End If
    End Sub
    Protected Sub chkinvoiceamt_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim invoiceamt As String

        If chkinvoiceamt.Checked = True Then
            invoiceamt = chkinvoiceamt.Checked
            Session("invoiceamt") = invoiceamt
        End If
    End Sub
    Protected Sub chkDetails_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Details As String

        If chkDetails.Checked = True Then
            Details = chkDetails.Checked
            Session("Details") = Details
        End If
    End Sub

    Protected Sub chkmatricno_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sortmatricno As String

        If chkmatricno.Checked = True Then
            sortmatricno = chkmatricno.Checked
            Session("chkmatricno") = sortmatricno
        ElseIf chkmatricno.Checked = False Then
            Session("chkmatricno") = Nothing
        End If
    End Sub

    Protected Sub chkinvoicenoo_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sortinvoicenoo As String

        If chkinvoicenoo.Checked = True Then
            sortinvoicenoo = chkinvoicenoo.Checked
            Session("chkinvoicenoo") = sortinvoicenoo
        ElseIf chkinvoicenoo.Checked = False Then
            Session("chkinvoicenoo") = Nothing
        End If
    End Sub
    Protected Sub rdrdate_checkedchanged(sender As Object, e As EventArgs)
        Dim radiodate As String
        If rdrdate.Checked = True Then
            RadioButton2.Checked = False
            RadioButton2.Enabled = False
            radiodate = rdrdate.Checked
            Session("radiodate") = radiodate
        ElseIf rdrdate.Checked = False Then
            'RadioButton2.Checked = False
            RadioButton2.Enabled = True
            Session("radiodate") = Nothing
        End If
    End Sub
    Protected Sub rdryear_checkedchanged(sender As Object, e As EventArgs)
        Dim radioyear As String
        If RadioButton2.Checked = True Then
            rdrdate.Checked = False
            rdrdate.Enabled = False
            radioyear = RadioButton2.Checked
            Session("radioyear") = radioyear
        ElseIf RadioButton2.Checked = False Then
            'RadioButton2.Checked = False
            rdrdate.Enabled = True
            Session("radioyear") = Nothing
        End If
    End Sub
    Protected Sub chkReset_checkedchanged(ByVal sender As Object, ByVal e As System.EventArgs)

        If chkReset.Checked = True Then
            rdrdate.Checked = False
            RadioButton2.Checked = False
            RadioButton2.Enabled = True
            rdrdate.Enabled = True
            Session("radiodate") = Nothing
            Session("radioyear") = Nothing
        End If

    End Sub
End Class
