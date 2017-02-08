Imports System.Collections.Generic
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.Entities
Imports System.Data
Imports System.IO
Imports System.Web.Configuration
Imports System.Data.SqlClient

Partial Class Student
    Inherits System.Web.UI.Page
    Dim Dfflag As String
    Dim dflag As String
    Dim CFlag As String
    Private loList As New List(Of StudentSponEn)
    Dim ListObjects As List(Of StudentEn)
    Private ErrorDescription As String
    Private spncode As String
    Private cnt As Integer
    Private StuMatrixNo As String
    ''Private LogErrors As LogError

    Protected Sub IbtnView_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnView.Click
        'loadgrid()
        ' load  List of Objects
        If lblCount.Text <> "" Then
            If CInt(lblCount.Text) > 0 Then
                OnClearData()
                If ibtnNew.Enabled = False Then
                    ibtnSave.Enabled = False
                    ibtnSave.ImageUrl = "images/gsave.png"
                    ibtnSave.ToolTip = "Access Denied"
                End If
            Else
                Session("PageMode") = "Edit"
                'FillDropDownList()
                LoadListObjects()
            End If
        Else
            Session("PageMode") = "Edit"
            'FillDropDownList()
            LoadListObjects()
        End If
        If lblCount.Text.Length = 0 Then
            Session("PageMode") = "Add"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack() Then
            LoadViewOne()
            'Adding validation for save button
            ibtnSave.Attributes.Add("onclick", "return validate()")
            ibtnDelete.Attributes.Add("onclick", "return getconfirm()")
            'btnSelection.Attributes.Add("onclick", "return validate()")
            txtmoblieno.Attributes.Add("onKeypress", "checkValue()")
            txtPhone.Attributes.Add("onKeypress", "checkValue()")
            txtPostcode.Attributes.Add("onKeypress", "checkValue()")
            ibtnFDate1.Attributes.Add("onClick", "return getDate1from()")
            ibtnTDate1.Attributes.Add("onClick", "return getDate1to()")
            txtVFrom1.Attributes.Add("OnKeyup", "return CheckVFromDate()")
            txtspTo1.Attributes.Add("OnKeyup", "return CheckT0date()")
            ibtnPrint.Attributes.Add("onclick", "return getDate()")

            'Added Zoya 16/2/2016
            txtCreditHrs.Attributes.Add("onKeypress", "isNumberKey(event)")
            txtPostcode.Attributes.Add("onKeypress", "isNumberKey(event)")
            txtPhone.Attributes.Add("onKeypress", "isNumberKey(event)")
            txtmoblieno.Attributes.Add("onKeypress", "isNumberKey(event)")
            txtmPostcode.Attributes.Add("onKeypress", "isNumberKey(event)")

            'While loading the page make the CFlag as null
            Session("PageMode") = ""
            'Loading User Rights
            LoadUserRights()
            'loadgrid()
            'while loading list object make it nothing
            Session("ListObj") = Nothing
            DisableRecordNavigator()
            'load PageName
            FillDropDownList()
            Menuname(CInt(Request.QueryString("Menuid")))
            ibtn_spn1_feetype.Attributes.Add("onclick", "new_window=window.open('AddSpnFee.aspx?Spid=1','Hanodale','width=520,height=500,resizable=0');new_window.focus();")
            btnAddSponser.Attributes.Add("onclick", "return DateCopare()")
            dates()
            ibtnSpn1.Attributes.Add("onclick", "new_window=window.open('Addspn.aspx?Spid=1','Hanodale','width=420,height=200,resizable=0');new_window.focus();")
            btnStuNotes.Attributes.Add("onclick", "new_window=window.open('StudentNotes.aspx?MatrixNo=" & cnt & "','Hanodale','width=450,height=600,resizable=0');new_window.focus();")
            Session("student") = Nothing
        End If
        lblMsg.Visible = False
        If Not Session("Spneobj") Is Nothing Then
            addspn()
            lblMsg.Visible = True
        End If
        If Not Session("eobj") Is Nothing Then
            addFeeType()
            Session("eobj") = Nothing
        End If
    End Sub

    Private Sub trimTxt()
        txtMatricNo.Text = Trim(txtMatricNo.Text)
        txtName.Text = Trim(txtName.Text)
        txtIcNo.Text = Trim(txtIcNo.Text)
        txtAccNo.Text = Trim(txtAccNo.Text)
        txtFloorNo.Text = Trim(txtFloorNo.Text)
        txtkolejgit.Text = Trim(txtkolejgit.Text)
        txtGpa.Text = Trim(txtGpa.Text)
        'txtcgpa.Text = Trim(txtcgpa.Text)
        txtHostel.Text = Trim(txtHostel.Text)
        txtCreditHrs.Text = Trim(txtCreditHrs.Text)
        txtSpn1.Text = Trim(txtSpn1.Text)
        txtVFrom1.Text = Trim(txtVFrom1.Text)
        txtspTo1.Text = Trim(txtspTo1.Text)
        txtSpnLimit.Text = Trim(txtSpnLimit.Text)
        txtAdd1.Text = Trim(txtAdd1.Text)
        txtAdd2.Text = Trim(txtAdd2.Text)
        txtAdd3.Text = Trim(txtAdd3.Text)
        txtCity.Text = Trim(txtCity.Text)
        txtState.Text = Trim(txtState.Text)
        txtPostcode.Text = Trim(txtPostcode.Text)
        txtEmail.Text = Trim(txtEmail.Text)
        txtPhone.Text = Trim(txtPhone.Text)
        txtmoblieno.Text = Trim(txtmoblieno.Text)
    End Sub

    Protected Sub txtRecNo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRecNo.TextChanged
        If txtRecNo.Text = " " Then
            txtRecNo.Text = 0
            If lblCount.Text <> Nothing Then
                If CInt(txtRecNo.Text) > CInt(lblCount.Text) Then
                    txtRecNo.Text = lblCount.Text
                End If
                FillData(CInt(txtRecNo.Text) - 1)
            Else
                txtRecNo.Text = ""
            End If
        Else
            If lblCount.Text <> Nothing Then
                If CInt(txtRecNo.Text) > CInt(lblCount.Text) Then
                    txtRecNo.Text = lblCount.Text
                End If
                FillData(CInt(txtRecNo.Text) - 1)
            Else
                txtRecNo.Text = ""
            End If
        End If

    End Sub

    Protected Sub ibtnSave_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnSave.Click
        SpaceValidation()
    End Sub

    Protected Sub ibtnDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnDelete.Click
        OnDelete()
    End Sub

    Protected Sub ibtnNew_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnNew.Click
        OnAdd()
    End Sub

    Protected Sub ibtnNext_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnNext.Click
        OnMoveNext()
    End Sub

    Protected Sub ibtnLast_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnLast.Click
        OnMoveLast()
    End Sub

    Protected Sub ibtnPrevs_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnPrevs.Click
        OnMovePrevious()
    End Sub

    Protected Sub ibtnFirst_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnFirst.Click
        OnMoveFirst()
    End Sub

    Protected Sub ibtnCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnCancel.Click
        LoadUserRights()
        OnClearData()
    End Sub
#Region "Methods"
    ''' <summary>
    ''' Method to Validate
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SpaceValidation()
        Dim GBFormat As System.Globalization.CultureInfo
        GBFormat = New System.Globalization.CultureInfo("en-GB")
        If txtMatricNo.Text.Length = 0 Or txtName.Text.Length = 0 Or txtIcNo.Text.Length = 0 And txtIcNo.Text.Length = 0 And ddlKolej.SelectedValue = "-1" And ddlFaculty.SelectedValue = "-1" Then
            lblMsg.Text = "Enter All Required Fields "
            lblMsg.Visible = True
            Exit Sub
        End If

        If Trim(txtName.Text).Length = 0 Then

            txtName.Text = Trim(txtName.Text)
            lblMsg.Text = "Enter valid Student Name "
            lblMsg.Visible = True
            txtName.Focus()
            Exit Sub
        End If
        If Trim(txtIcNo.Text).Length = 0 Then

            txtIcNo.Text = Trim(txtIcNo.Text)
            lblMsg.Text = "Enter valid ICNo "
            lblMsg.Visible = True
            txtIcNo.Focus()
            Exit Sub
        End If
        'If Trim(txtVFrom1.Text).Length < 10 Then
        '    lblMsg.Text = "Enter Valid txtVFrom1 Date"
        '    lblMsg.Visible = True
        '    txtVFrom1.Focus()
        '    Exit Sub
        'Else
        '    Try
        '        txtVFrom1.Text = DateTime.Parse(txtVFrom1.Text, GBFormat)
        '    Catch ex As Exception
        '        lblMsg.Text = "Enter Valid txtVFrom1 Date"
        '        lblMsg.Visible = True
        '        txtVFrom1.Focus()
        '        Exit Sub
        '    End Try
        'End If

        'Due date
        'If Trim(txtspTo1.Text).Length < 10 Then
        '    lblMsg.Text = "Enter Valid spTo Date"
        '    lblMsg.Visible = True
        '    txtspTo1.Focus()
        '    Exit Sub
        'Else
        '    Try
        '        txtspTo1.Text = DateTime.Parse(txtspTo1.Text, GBFormat)
        '    Catch ex As Exception
        '        lblMsg.Text = "Enter Valid spTo Date"
        '        lblMsg.Visible = True
        '        txtspTo1.Focus()
        '        Exit Sub
        '    End Try
        'End If

        'If Trim(txtAccNo.Text).Length = 0 Then

        '    txtAccNo.Text = Trim(txtAccNo.Text)
        '    lblMsg.Text = "Enter valid Correct Account No "
        '    lblMsg.Visible = True
        '    txtAccNo.Focus()
        '    Exit Sub
        'End If

        OnSave()
        If (Trim(txtVFrom1.Text).Length > 0 Or Trim(txtspTo1.Text).Length > 0) Then
            SetDateFormat()
        End If
    End Sub
    ''' <summary>
    ''' Method to Change the Date Format
    ''' </summary>
    ''' <remarks>Date in ddd/mm/yyyy Format</remarks>
    Private Sub dates()
        Dim GBFormat As System.Globalization.CultureInfo
        GBFormat = New System.Globalization.CultureInfo("en-GB")

        txtVFrom1.Text = Format(Date.Now, "dd/MM/yyyy")
        txtspTo1.Text = Format(Date.Now, "dd/MM/yyyy")
    End Sub
    ''' <summary>
    ''' Method to Add FeeTypes to the ListBox
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub addFeeType()
        Dim listfee As New List(Of FeeTypesEn)
        Dim eobjFt As New FeeTypesEn
        listfee = Session("eobj")
        Dim i As Integer = 0
        If listfee.Count <> 0 Then
            While i < listfee.Count
                eobjFt = listfee(i)
                If eobjFt.Priority = 1 Then
                    If chkSpnFeetypes1.Checked = False Then
                        lstStudentFeetype1.Items.Add(New ListItem(eobjFt.Description, eobjFt.FeeTypeCode))
                    End If
                End If
                i = i + 1
            End While
        End If
    End Sub
    ''' <summary>
    ''' Method to Load Programs Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadProgram()
        Dim objBAL As New ProgramInfoBAL
        Dim listObj As New List(Of ProgramInfoEn)
        If Session("PageMode") = "Add" Then

            Try
                listObj = objBAL.GetProgramList(ddlFaculty.SelectedValue)
            Catch ex As Exception
                LogError.Log("Student", "LoadProgram", ex.Message)
            End Try
        Else
            Try
                listObj = objBAL.GetProgramInfoListAll(ddlFaculty.SelectedValue)
            Catch ex As Exception
                LogError.Log("Student", "LoadProgram", ex.Message)
            End Try

        End If
        ddlProgram.Items.Clear()
        ddlProgram.Items.Add(New ListItem("--Select--", "-1"))
        ddlProgram.DataTextField = "Program"
        ddlProgram.DataValueField = "ProgramCode"
        ddlProgram.DataSource = listObj
        ddlProgram.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Load Programs Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadFaculty()
        Dim eFaculty As New FacultyEn
        Dim bFaculty As New FacultyBAL
        ddlFaculty.Items.Clear()
        ddlFaculty.Items.Add(New ListItem("---Select---", "-1"))
        ddlFaculty.DataTextField = "SAFC_Desc"
        ddlFaculty.DataValueField = "SAFC_Code"
        eFaculty.SAFC_Code = "%"
        Try
            ddlFaculty.DataSource = bFaculty.GetList(eFaculty)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlFaculty.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Load All the Dropdowns
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillDropDownList()
        Dim bStuStatus As New StudentStatusBAL
        Dim eStuStatus As New StudentStatusEn
        Dim bStuCategory As New StudentCategoryBAL
        Dim eStuCategory As New StudentCategoryEn
        Dim bSemister As New SemesterSetupBAL
        'Dim bBank As New BankProfileBAL
        'Dim eBank As New BankProfileEn
        Dim bSemSetup As New SemesterSetupBAL
        Dim eSemSetup As New SemesterSetupEn
        Dim eFaculty As New FacultyEn
        Dim bFaculty As New FacultyBAL
        Dim eKolej As New KolejEn
        Dim bKolej As New KolejBAL
        Dim bKoko As New FeeTypesBAL
        Dim eKoko As New FeeTypesEn
        Dim bBank As New StudentBankBAL
        Dim eBank As New StudentBankEn

        ddlKolej.Items.Clear()
        ddlKolej.Items.Add(New ListItem("---Select---", "-1"))
        ddlKolej.DataTextField = "SAKO_Description"
        ddlKolej.DataValueField = "SAKO_Code"
        Try
            ddlKolej.DataSource = bKolej.GetList(eKolej)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlKolej.DataBind()

        ddlFaculty.Items.Clear()
        ddlFaculty.Items.Add(New ListItem("---Select---", "-1"))
        ddlFaculty.DataTextField = "SAFC_Desc"
        ddlFaculty.DataValueField = "SAFC_Code"
        eFaculty.SAFC_Code = "%"
        Try
            ddlFaculty.DataSource = bFaculty.GetList(eFaculty)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlFaculty.DataBind()
        'Session("ddlfaculty") = ddlFaculty.DataSource

        eStuStatus.StudentStatusCode = ""
        eStuStatus.Description = ""
        eStuStatus.Status = True
        eStuStatus.BlStatus = True
        ddlstudentStatus.Items.Clear()
        ddlstudentStatus.Items.Add(New ListItem("---Select---", "-1"))
        ddlstudentStatus.DataTextField = "Description"
        ddlstudentStatus.DataValueField = "StudentStatusCode"
        'If Session("PageMode") = "Add" Then
        Try
            ddlstudentStatus.DataSource = bStuStatus.GetStudentStatusListAll(eStuStatus)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try

        'Else

        'Try
        '    ddlstudentStatus.DataSource = bStuStatus.GetStudentStatusListAll(eStuStatus)
        'Catch ex As Exception
        '    LogError.Log("Student", "FillDropDownList", ex.Message)
        'End Try
        'End If
        ddlstudentStatus.DataBind()

        'eBank.BankDetailsCode = ""
        eBank.StudentBankCode = ""
        eBank.Description = ""
        'eBank.ACCode = ""
        'eBank.GLCode = ""
        eBank.Status = True
        ddlBank.Items.Clear()
        ddlBank.Items.Add(New ListItem("---Select---", "-1"))
        ddlBank.DataTextField = "Description"
        'ddlBank.DataValueField = "BankDetailsCode"
        ddlBank.DataValueField = "StudentBankCode"
        If Session("PageMode") = "Add" Then

            Try
                'ddlBank.DataSource = bBank.GetBankProfileList(eBank)
                ddlBank.DataSource = bBank.GetStudentBankTypelist(eBank)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        Else

            Try
                'ddlBank.DataSource = bBank.GetBankProfileListAll(eBank)
                ddlBank.DataSource = bBank.GetStudentBankTypeListAll(eBank)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        End If
        ddlBank.DataBind()

        eSemSetup.SemisterSetupCode = ""
        eSemSetup.Description = ""
        eSemSetup.Status = True
        'ddlSem.Items.Clear()
        'ddlSem.Items.Add(New ListItem("---Select---", "-1"))
        'ddlSem.DataTextField = "Semester"
        'ddlSem.DataValueField = "Semester"
        'Try
        '    ddlSem.DataSource = bSemSetup.GetList(eSemSetup)
        'Catch ex As Exception
        '    LogError.Log("Student", "FillDropDownList", ex.Message)
        'End Try
        ddlSem.DataBind()

        'ddlIntkSemester.Items.Clear()
        'ddlIntkSemester.Items.Add(New ListItem("---Select---", "-1"))
        'ddlIntkSemester.DataTextField = "Semester"
        'ddlIntkSemester.DataValueField = "Semester"

        ddlIntakeSession.Items.Clear()
        ddlIntakeSession.Items.Add(New ListItem("---Select---", "-1"))
        ddlIntakeSession.DataTextField = "SemisterCode2"
        ddlIntakeSession.DataValueField = "SemisterSetupCode"
        Try
            ddlIntakeSession.DataSource = bSemSetup.GetListSemesterCode(eSemSetup)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlIntakeSession.DataBind()

        eSemSetup.SemisterSetupCode = ""
        eSemSetup.Description = ""
        eSemSetup.Status = True
        ddlCurSession.Items.Clear()
        ddlCurSession.Items.Add(New ListItem("---Select---", "-1"))
        ddlCurSession.DataTextField = "SemisterCode2"
        ddlCurSession.DataValueField = "SemisterSetupCode"

        Try
            ddlCurSession.DataSource = bSemSetup.GetListSemesterCode(eSemSetup)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try
        ddlCurSession.DataBind()

        eStuCategory.StudentCategoryCode = ""
        eStuCategory.Description = ""
        eStuCategory.Status = True
        ddlStudentCategory.Items.Clear()

        Dim ListStuCat As New List(Of StudentCategoryEn)
        ddlStudentCategory.Items.Add(New ListItem("---Select---", "-1"))
        ddlStudentCategory.DataTextField = "Description"
        ddlStudentCategory.DataValueField = "StudentCategoryCode"
        If Session("PageMode") = "Add" Then

            Try
                ddlStudentCategory.DataSource = bStuCategory.GetStudentCategoryList(eStuCategory)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        Else
            Try
                ddlStudentCategory.DataSource = bStuCategory.GetStudentCategoryListAll(eStuCategory)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        End If

        ddlStudentCategory.DataBind()

        ddlFeeCat.Items.Clear()
        ddlFeeCat.Items.Add(New ListItem("---Select---", "-1"))
        ddlFeeCat.DataTextField = "Description"
        ddlFeeCat.DataValueField = "StudentCategoryCode"
        If Session("PageMode") = "Add" Then
            Try
                ddlFeeCat.DataSource = bStuCategory.GetStudentCategoryList(eStuCategory)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        Else
            Try
                ddlFeeCat.DataSource = bStuCategory.GetStudentCategoryListAll(eStuCategory)
            Catch ex As Exception
                LogError.Log("Student", "FillDropDownList", ex.Message)
            End Try
        End If
        ddlFeeCat.DataBind()

        ddlKokoList.Items.Clear()
        ddlKokoList.Items.Add(New ListItem("---Select---", "-1"))
        ddlKokoList.DataTextField = "SAKO_Code"
        ddlKokoList.DataValueField = "SAKO_Code"
        Try
            'ddlKokoList.DataSource = bKoko.GetKokoListddl(eKoko)
            ddlKokoList.DataSource = bKolej.GetList(eKolej)
        Catch ex As Exception
            LogError.Log("Student", "FillDropDownList", ex.Message)
        End Try

        ddlKokoList.DataBind()

    End Sub
    ''' <summary>
    ''' Method to Load Sponsors
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub addspn()
        Dim eobjF As SponsorEn
        eobjF = Session("Spneobj")

        txtSpn1.Text = eobjF.Name
        txtSpCode.Text = eobjF.SponserCode
        lblMsg.Text = ""

        Dim bsobjSpoFeetype As New SponsorFeeTypesBAL
        Dim eobjSponfee As New SponsorFeeTypesEn
        Dim listSponFeeTypes As New List(Of SponsorFeeTypesEn)
        eobjSponfee.SponserCode = eobjF.SponserCode
        Try
            listSponFeeTypes = bsobjSpoFeetype.GetSPFeeTypeList(eobjSponfee)
        Catch ex As Exception
            LogError.Log("Student", "addspn", ex.Message)
        End Try

        lstStudentFeetype1.Items.Clear()
        If Not listSponFeeTypes Is Nothing Then
            Dim j As Integer = 0
            While j < listSponFeeTypes.Count
                lstStudentFeetype1.Items.Add(New ListItem(listSponFeeTypes(j).FeeTypeDesc, listSponFeeTypes(j).FeeTypeCode))
                j = j + 1
            End While
            'If listSponFeeTypes.Count = 0 Then
            '    chkSpnFeetypes1.Checked = True
            'Else
            '    chkSpnFeetypes1.Checked = False
            'End If
        Else
            chkSpnFeetypes1.Checked = True
        End If
        listSponFeeTypes = Nothing
        Session("Spneobj") = Nothing
        dates()
    End Sub
    ''' <summary>
    ''' Method to Load UserRights
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadUserRights()
        Dim obj As New UsersBAL
        Dim eobj As New UserRightsEn

        Try
            eobj = obj.GetUserRights(CInt(Request.QueryString("Menuid")), CInt(Session("UserGroup")))

        Catch ex As Exception
            LogError.Log("Student", "LoadUserRights", ex.Message)
        End Try
        'Rights for Add

        If eobj.IsAdd = True Then

            OnAdd()
            ibtnNew.ImageUrl = "images/add.png"
            ibtnNew.Enabled = True
        Else
            ibtnNew.ImageUrl = "images/gadd.png"
            ibtnNew.Enabled = False
            ibtnNew.ToolTip = "Access Denied"

        End If
        'Rights for Edit
        If eobj.IsEdit = True Then
            ibtnSave.Enabled = True
            ibtnSave.ImageUrl = "images/save.png"
            ibtnSave.ToolTip = "Edit"
            If eobj.IsAdd = False Then
                ibtnSave.Enabled = False
                ibtnSave.ImageUrl = "images/gsave.png"
                ibtnSave.ToolTip = "Access Denied"
            End If

            Session("EditFlag") = True

        Else
            Session("EditFlag") = False
            ibtnSave.Enabled = False
            ibtnSave.ImageUrl = "images/gsave.png"
        End If
        'Rights for View
        ibtnView.Enabled = eobj.IsView
        If eobj.IsView = True Then
            ibtnView.ImageUrl = "images/find.png"
            ibtnView.Enabled = True
        Else
            ibtnView.ImageUrl = "images/gfind.png"
            ibtnView.ToolTip = "Access Denied"
        End If
        'Rights for Delete
        If eobj.IsDelete = True Then
            ibtnDelete.ImageUrl = "images/delete.png"
            ibtnDelete.Enabled = True
        Else
            ibtnDelete.ImageUrl = "images/gdelete.png"
            ibtnDelete.ToolTip = "Access Denied"
            ibtnDelete.Enabled = False
        End If
        'Rights for Print
        ibtnPrint.Enabled = eobj.IsPrint
        If eobj.IsPrint = True Then
            ibtnPrint.Enabled = True
            ibtnPrint.ImageUrl = "images/print.png"
            ibtnPrint.ToolTip = "Print"
        Else
            ibtnPrint.Enabled = False
            ibtnPrint.ImageUrl = "images/gprint.png"
            ibtnPrint.ToolTip = "Access Denied"
        End If
        If eobj.IsOthers = True Then
            ibtnOthers.Enabled = True
            ibtnOthers.ImageUrl = "images/others.png"
            ibtnOthers.ToolTip = "Others"
        Else
            ibtnOthers.Enabled = False
            ibtnOthers.ImageUrl = "images/gothers.png"
            ibtnOthers.ToolTip = "Access Denied"
        End If
        If eobj.IsPost = True Then
            ibtnPosting.Enabled = True
            ibtnPosting.ImageUrl = "images/posting.png"
            ibtnPosting.ToolTip = "Posting"
        Else
            ibtnPosting.Enabled = False
            ibtnPosting.ImageUrl = "images/gposting.png"
            ibtnPosting.ToolTip = "Access Denied"
        End If
    End Sub
    ''' <summary>
    ''' Method to Load Fields in New Mode
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnAdd()
        Session("PageMode") = "Add"
        FillDropDownList()
        PnlAdd.Visible = True
        OnClearData()

    End Sub
    ''' <summary>
    ''' Method to Clear the Field Values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnClearData()
        txtMatricNo.Enabled = True
        Session("ListObj") = Nothing
        DisableRecordNavigator()
        txtMatricNo.Text = ""
        txtName.Text = ""
        ddlstudentStatus.SelectedValue = "-1"
        ddlIcNo.SelectedValue = "-1"
        txtIcNo.Text = ""
        ddlFaculty.SelectedValue = "-1"
        ddlProgram.Items.Clear()
        ddlProgram.Items.Add(New ListItem("--Select--", "-1"))
        ddlSem.SelectedValue = "-1"
        ddlIntkSemester.SelectedValue = "-1"
        ddlFeeCat.SelectedValue = "-1"
        ddlRoomType.SelectedValue = "-1"
        ddlCurSession.SelectedValue = "-1"
        ddlStudyType.SelectedValue = "-1"
        ddlStudentCategory.SelectedValue = "-1"
        ddlBank.SelectedValue = "-1"
        txtAccNo.Text = ""
        dates()
        ddlHostel.SelectedValue = "-1"
        ddlKolej.SelectedValue = "-1"
        ddlblock.Items.Clear()
        ddlblock.Items.Add(New ListItem("--Select--", "-1"))
        txtFloorNo.Text = ""
        txtkolejgit.Text = ""
        txtGpa.Text = ""
        'txtcgpa.Text = ""
        txtHostel.Text = ""
        txtCreditHrs.Text = ""
        ddlStatus.SelectedValue = "1"
        ddlIntakeSession.SelectedValue = "-1"
        txtSpn1.Text = ""
        txtAdd1.Text = ""
        txtAdd2.Text = ""
        txtAdd3.Text = ""
        txtCity.Text = ""
        txtState.Text = ""
        ddlCountry.SelectedValue = -1
        txtPostcode.Text = ""
        txtmAddress1.Text = ""
        txtmAddress2.Text = ""
        txtmAddress3.Text = ""
        txtmCity.Text = ""
        txtmState.Text = ""
        ddlmCountry.SelectedValue = -1
        txtmPostcode.Text = ""
        txtEmail.Text = ""
        txtPhone.Text = ""
        txtmoblieno.Text = ""
        btnStuNotes.Enabled = False
        Session("PageMode") = "Add"
        'chkSpnFeetypes1.Checked = False
        Session("StuSpon") = Nothing
        lstStudentFeetype1.Items.Clear()
        dgStuSponser.DataSource = Nothing
        dgStuSponser.DataBind()
        LoadViewOne()
        Session("Spneobj") = Nothing
    End Sub
    ''' <summary>
    ''' Method to Change to Edit Mode
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnEdit()
        Session("PageMode") = "Edit"
        txtMatricNo.Enabled = False
    End Sub
    ''' <summary>
    ''' Method to Validate Sponsor Dates
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub spndate()
        Dim spn1 As Boolean
        Dim spn2 As Boolean
        Dim spn3 As Boolean
        Dim d As Long
        If txtSpn1.Text = "" Then
            spn1 = True
        Else
            If txtVFrom1.Text = "" Then
                spn1 = True
            Else
                If txtspTo1.Text = "" Then
                    spn1 = False
                Else

                    If Trim(txtVFrom1.Text).Length <> 0 And Trim(txtspTo1.Text).Length <> 0 Then
                        d = DateDiff(DateInterval.Day, CDate(txtVFrom1.Text), CDate(txtspTo1.Text))
                        If d < 0 Then
                            txtspTo1.Text = ""
                            spn1 = False
                        Else
                            spn1 = True
                        End If
                    End If

                End If

            End If
        End If


        If spn1 = False Or spn2 = False Or spn3 = False Then
            Session("False") = "False"
        End If
    End Sub
    ''' <summary>
    ''' Method To Change the Date Format(dd/MM/yyyy)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetDateFormat()
        Dim myReceiptDate As Date = CDate(CStr(txtVFrom1.Text))
        Dim myFormat As String = "dd/MM/yyyy"
        Dim myFormattedDate As String = Format(myReceiptDate, myFormat)
        txtVFrom1.Text = myFormattedDate
        Dim myBatchDate As Date = CDate(CStr(txtspTo1.Text))
        Dim myFormattedDate1 As String = Format(myBatchDate, myFormat)
        txtspTo1.Text = myFormattedDate1
    End Sub
    ''' <summary>
    ''' Method to Save and Update Students
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnSave()
        'SetDateFormat()
        Dim bsobg As New StudentStatusBAL
        Dim bsobj As New StudentBAL
        Dim eobj As New StudentEn
        Dim eobjstatus As New StudentStatusEn
        Dim RecAff As Integer
        Dim LstStuSpn As New List(Of StudentSponEn)
        Dim objStuSpn As New StudentSponEn
        Dim objstufeetype As New StuSponFeeTypesEn
        Dim LstStuSpnFeetype As New List(Of StuSponFeeTypesEn)
        Dim oldsemyear As String
        Dim newsemyear As String
        Dim oldintake As String
        Dim newintake As String
        lblMsg.Visible = True
        eobj.MatricNo = Trim(txtMatricNo.Text)
        eobj.StudentName = Trim(txtName.Text)

        eobj.StudentCode = ddlstudentStatus.SelectedValue

        eobj.RegistrationStatus = Convert.ToInt32(ddlRegistrationStatus.SelectedValue)

        eobj.ID = ddlIcNo.SelectedValue

        'If ddlIcNo.SelectedValue = 2 Then
        '    eobj.ICNo = ""
        '    eobj.Passport = Trim(txtIcNo.Text)
        'Else
        '    eobj.ICNo = Trim(txtIcNo.Text)
        '    eobj.Passport = ""
        'End If

        eobj.ICNo = Trim(txtIcNo.Text)
        eobj.Passport = ""
        eobj.Faculty = ddlFaculty.SelectedValue
        eobj.ProgramID = ddlProgram.SelectedValue
        eobj.CurrentSemester = ddlSem.SelectedValue
        oldsemyear = ddlCurSession.SelectedValue
        eobj.CurretSemesterYear = oldsemyear.Replace("/", "").Replace("-", "")
        oldintake = ddlIntakeSession.SelectedValue
        eobj.Intake = oldintake.Replace("/", "").Replace("-", "")
        'eobj.CurretSemesterYear = ddlCurSession.SelectedValue
        eobj.Studytype = ddlStudyType.SelectedValue
        eobj.CategoryCode = ddlStudentCategory.SelectedValue
        eobj.SASI_Bank = ddlBank.SelectedValue
        eobj.SASI_AccNo = Trim(txtAccNo.Text)
        'eobj.Intake = ddlIntakeSession.SelectedValue
        eobj.Hostel = ddlHostel.SelectedValue
        eobj.SAKO_Code = ddlKolej.SelectedValue

        If ddlKokoList.SelectedValue = "--Select--" Then
            eobj.KokoCode = -1
        Else
            eobj.KokoCode = ddlKokoList.SelectedValue
        End If

        eobj.UpdatedBy = Session("User")

        If ddlblock.SelectedValue = "--Select--" Then
            eobj.SABK_Code = -1
        Else
            eobj.SABK_Code = ddlblock.SelectedValue
        End If

        eobj.SART_Code = ddlRoomType.SelectedValue
        eobj.SASI_FloorNo = Trim(txtFloorNo.Text)
        eobj.SASI_OtherID = Trim(txtkolejgit.Text)
        If Trim(txtGpa.Text).Length <> 0 Then
            eobj.SASI_GPA = Trim(txtGpa.Text)
        Else
            eobj.SASI_GPA = 0
        End If
        'If Trim(txtcgpa.Text).Length <> 0 Then
        '    eobj.SASI_CGPA = Trim(txtcgpa.Text)
        'Else
        '    eobj.SASI_CGPA = 0
        'End If
        eobj.HostelIntake = Trim(txtHostel.Text)

        If Trim(txtCreditHrs.Text).Length <> 0 Then
            eobj.SASI_CrditHrs = Trim(txtCreditHrs.Text)
        Else
            eobj.SASI_CrditHrs = 0
        End If

        eobj.SASI_StatusRec = True
        eobj.SASI_AFCStatus = False
        eobj.SASI_MAdd1 = Trim(txtmAddress1.Text)
        eobj.SASI_MAdd2 = Trim(txtmAddress2.Text)
        eobj.SASI_MAdd3 = Trim(txtmAddress3.Text)
        eobj.SASI_MCity = Trim(txtmCity.Text)
        eobj.SASI_MState = Trim(txtmState.Text)
        eobj.SASI_MCountry = ddlmCountry.SelectedValue
        eobj.SASI_MPostcode = Trim(txtPostcode.Text)
        eobj.SASI_Add1 = Trim(txtAdd1.Text)
        eobj.SASI_Add2 = Trim(txtAdd2.Text)
        eobj.SASI_Add3 = Trim(txtAdd3.Text)
        eobj.SASI_City = Trim(txtCity.Text)
        eobj.SASI_State = Trim(txtState.Text)
        eobj.SASI_Country = ddlCountry.SelectedValue
        eobj.SASI_Email = Trim(txtEmail.Text)
        eobj.SASI_Tel = Trim(txtPhone.Text)
        eobj.SASI_Postcode = Trim(txtPostcode.Text)
        eobj.SASI_HP = Trim(txtmoblieno.Text)
        eobj.SASI_GLCode = ""
        eobj.SASI_AccNo = Trim(txtAccNo.Text)
        'eobj.UpdatedBy = Session("User")
        eobj.SASI_UpdatedBy = Session("User")
        eobj.SASI_UpdatedDtTm = Date.Now.ToString()
        'eobj.UpdatedTime = Date.Now.ToString()
        eobj.FeeCat = ddlFeeCat.SelectedValue

        Dim loList As New List(Of StudentSponEn)
        Dim objFeeTypes As New List(Of StuSponFeeTypesEn)
        Dim i As Integer = 0
        Dim j As Integer = 0
        If Not Session("StuSpon") Is Nothing Then
            loList = Session("StuSpon")
            While i < loList.Count
                loList(i).MatricNo = txtMatricNo.Text
                objFeeTypes = loList(i).ListStuSponFeeTypes
                j = 0
                While j < objFeeTypes.Count
                    objFeeTypes(j).MatricNo = txtMatricNo.Text
                    j = j + 1
                End While
                i = i + 1
            End While
            eobj.ListStuSponser = loList
        End If

        If Session("PageMode") = "Add" Then
            Try
                RecAff = bsobj.Insert(eobj)
                ErrorDescription = "Record Saved Successfully "
                lblMsg.Text = ErrorDescription
                Session("spnCode1") = ""
            Catch ex As Exception
                'lblMsg.Text = ex.Message.ToString()
                lblMsg.Text = "Unable to add manually as all the students information derived from SMP."
                LogError.Log("Student", "OnSave", ex.Message)
            End Try
        ElseIf Session("PageMode") = "Edit" Then
            Try
                RecAff = bsobj.Update(eobj)
                ListObjects = Session("ListObj")
                ListObjects(CInt(txtRecNo.Text) - 1) = eobj
                Session("ListObj") = ListObjects
                Session("spnCode1") = ""
                'Session("StuSpon") = ""
                ErrorDescription = "Record Updated Successfully "
                lblMsg.Text = ErrorDescription
            Catch ex As Exception
                lblMsg.Text = ex.Message.ToString()
                LogError.Log("Student", "OnSave", ex.Message)
            End Try
        End If
        lblMsg.Visible = True
        txtMatricNo.Enabled = True
    End Sub

    ''' <summary>
    ''' Method to Delete Students
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private Sub OnDelete()
        If txtMatricNo.Text <> "" Then
            If lblCount.Text = "" Then lblCount.Text = 0
            If lblCount.Text > 0 Then
                Dim bsobj As New StudentBAL
                Dim eobj As New StudentEn
                Dim RecAff As Integer
                eobj.MatricNo = txtMatricNo.Text
                Try
                    RecAff = bsobj.Delete(eobj)
                    ListObjects = Session("ListObj")
                    ListObjects.RemoveAt(CInt(txtRecNo.Text) - 1)
                    lblCount.Text = lblCount.Text - 1
                    Session("ListObj") = ListObjects
                    ErrorDescription = "Record Deleted Successfully "
                    lblMsg.Visible = True
                    lblMsg.Text = ErrorDescription
                    Dfflag = "Delete"

                Catch ex As Exception
                    lblMsg.Visible = True
                    lblMsg.Text = ex.Message.ToString()
                    LogError.Log("Student", "OnDelete", ex.Message)
                    dflag = "in use"
                End Try
                txtMatricNo.Text = ""
                txtName.Text = ""
                ddlstudentStatus.SelectedValue = -1
                ddlIcNo.SelectedValue = -1
                txtIcNo.Text = ""
                ddlFaculty.SelectedValue = -1
                ddlProgram.Items.Clear()
                ddlProgram.Items.Add(New ListItem("---Select---", "-1"))
                ddlSem.SelectedValue = -1
                ddlStudyType.SelectedValue = -1
                ddlStudentCategory.SelectedValue = -1
                ddlBank.SelectedValue = -1
                txtAccNo.Text = ""
                ddlHostel.SelectedValue = -1
                ddlKolej.SelectedValue = -1
                ddlblock.Items.Clear()
                ddlblock.Items.Add(New ListItem("---Select---", "-1"))
                txtFloorNo.Text = ""
                txtkolejgit.Text = ""
                txtGpa.Text = ""
                'txtcgpa.Text = ""
                txtHostel.Text = ""
                txtCreditHrs.Text = ""
                ddlStatus.SelectedValue = "1"
                ddlIntakeSession.SelectedValue = "-1"
                txtSpn1.Text = ""
                txtSpnLimit.Text = ""
                txtVFrom1.Text = ""
                txtspTo1.Text = ""
                txtAdd1.Text = ""
                txtAdd2.Text = ""
                txtAdd3.Text = ""
                txtCity.Text = ""
                txtState.Text = ""
                ddlCountry.SelectedValue = -1
                txtPostcode.Text = ""
                txtEmail.Text = ""
                txtPhone.Text = ""
                txtmoblieno.Text = ""
                txtmAddress1.Text = ""
                txtmAddress2.Text = ""
                txtmAddress3.Text = ""
                txtmCity.Text = ""
                txtmState.Text = ""

                ddlmCountry.SelectedValue = -1
                txtmPostcode.Text = ""
                LoadListObjects()
            Else
                ErrorDescription = "Select a Record to Delete"
                lblMsg.Text = ErrorDescription
            End If
        Else
            ErrorDescription = "Select a Record to Delete"
            lblMsg.Text = ErrorDescription
        End If

    End Sub
    ''' <summary>
    ''' Method to get the List Of Students
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadListObjects()
        Dim ds As New DataSet
        Dim bobj As New StudentBAL
        Dim recStu As Integer
        Dim pgid As String
        Dim studentStatus As String
        Dim eobj As New StudentEn
        Dim FeeCategory As String
        Dim StudentCategory As String
        Dim koko As String
        Dim fac As String
        Dim intakeSem As String
        Dim intakeSession As String
        'modified by farid on 06122016
        Dim currSem As Integer
        Dim currSession As String
        Dim studytype As String
        Dim bank As String
        Dim hostel As Integer
        'Dim RegStatus As String

        If ddlFaculty.SelectedValue = "-1" Then
            fac = "-1"
        Else
            fac = ddlFaculty.SelectedValue
        End If
        If ddlIntakeSession.SelectedValue = "-1" Then
            intakeSession = "-1"
        Else
            intakeSession = ddlIntakeSession.SelectedValue
            intakeSession = intakeSession.Replace("/", "").Replace("-", "")
        End If
        If ddlIntkSemester.SelectedValue = "-1" Then
            intakeSem = "-1"
        Else
            intakeSem = ddlIntkSemester.SelectedValue
        End If
        If ddlSem.SelectedValue = "-1" Then
            currSem = -1
        Else
            currSem = ddlSem.SelectedValue
        End If
        If ddlCurSession.SelectedValue = "-1" Then
            currSession = "-1"
        Else
            currSession = ddlCurSession.SelectedValue
            currSession = currSession.Replace("/", "").Replace("-", "")
        End If
        If ddlStatus.SelectedValue = "-1" Then
            eobj.SASI_StatusRec = True
            'eobj.SASI_StatusRec = recStu
        ElseIf ddlStatus.SelectedValue = "0" Then
            eobj.SASI_StatusRec = False
        ElseIf ddlStatus.SelectedValue = "1" Then
            eobj.SASI_StatusRec = True
        End If
        If ddlProgram.SelectedValue = "-1" Then
            pgid = "-1"
        Else
            pgid = ddlProgram.SelectedValue
        End If
        If ddlstudentStatus.SelectedValue = "-1" Then
            studentStatus = "-1"
        Else
            studentStatus = ddlstudentStatus.SelectedValue
        End If

        If ddlFeeCat.SelectedValue = "-1" Then
            FeeCategory = "-1"
        Else
            FeeCategory = ddlFeeCat.SelectedValue
        End If
        If ddlStudentCategory.SelectedValue = "-1" Then
            StudentCategory = "-1"
        Else
            StudentCategory = ddlStudentCategory.SelectedValue
        End If

        If ddlKokoList.SelectedValue = "-1" Then
            koko = -1
        Else
            koko = ddlKokoList.SelectedValue
        End If

        'added by farid 06122016
        If ddlStudyType.SelectedValue = "-1" Then
            studytype = "-1"
        Else
            studytype = ddlStudyType.SelectedValue
        End If

        If ddlBank.SelectedValue = "-1" Then
            bank = "-1"
        Else
            bank = ddlBank.SelectedValue
        End If

        'If ddlHostel.SelectedValue = "-1" Then
        '    'hostel = -1
        '    eobj.Hostel = True And False
        'ElseIf ddlHostel.SelectedValue = "1" Then
        '    'hostel = ddlHostel.SelectedValue
        '    eobj.Hostel = True
        'ElseIf ddlHostel.SelectedValue = "0" Then
        '    eobj.Hostel = False
        'End If

        eobj.MatricNo = Trim(txtMatricNo.Text)
        eobj.StudentName = Trim(txtName.Text)
        eobj.ProgramID = pgid

        eobj.ICNo = Trim(txtIcNo.Text)
        eobj.StudentCode = studentStatus
        eobj.FeeCat = FeeCategory
        eobj.KokoCode = koko
        'commented by farid 06122016
        'eobj.CurretSemesterYear = currSem & currSession
        eobj.CurrentSemester = currSem
        eobj.CurretSemesterYear = currSession
        eobj.Faculty = fac
        eobj.Studytype = studytype
        eobj.BankCode = bank
        eobj.CategoryCode = StudentCategory


        'commented by farid 06122016
        'eobj.Intake = intakeSem & intakeSession
        eobj.Intake = intakeSession
        'eobj.RegistrationStatus = Convert.ToInt32(ddlRegistrationStatus.SelectedValue)

        Try
            ListObjects = bobj.GetStudentList(eobj)
        Catch ex As Exception
            LogError.Log("Student", "LoadListObjects", ex.Message)
        End Try
        Session("ListObj") = ListObjects
        lblCount.Text = ListObjects.Count.ToString()

        If ListObjects.Count <> 0 Then
            DisableRecordNavigator()
            txtRecNo.Text = "1"
            PnlAdd.Visible = True
            OnMoveFirst()
            If Session("EditFlag") = True Then
                Session("PageMode") = "Edit"
                txtMatricNo.Enabled = False
                ibtnSave.Enabled = True
                ibtnSave.ImageUrl = "images/save.png"
            Else
                Session("PageMode") = ""
                ibtnSave.Enabled = False
                ibtnSave.ImageUrl = "images/gsave.png"
            End If
        Else
            txtRecNo.Text = ""
            lblCount.Text = ""
            OnClearData()
            If Dfflag = "Delete" Then
            ElseIf dflag = "in use" Then
                lblMsg.Visible = True
                lblMsg.Text = "Record Already in Use"

            Else

                lblMsg.Visible = True
                ErrorDescription = "Record did not Exist"
                lblMsg.Text = ErrorDescription

            End If
        End If
    End Sub

    ''' <summary>
    ''' Method to Enable or Disable Navigation Buttons
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisableRecordNavigator()
        Dim flag As Boolean
        If Session("ListObj") Is Nothing Then
            flag = False
            txtRecNo.Text = ""
            lblCount.Text = ""
        Else
            flag = True
        End If
        ibtnFirst.Enabled = flag
        ibtnLast.Enabled = flag
        ibtnPrevs.Enabled = flag
        ibtnNext.Enabled = flag
        If flag = False Then
            ibtnFirst.ImageUrl = "images/gnew_first.png"
            ibtnLast.ImageUrl = "images/gnew_last.png"
            ibtnPrevs.ImageUrl = "images/gnew_Prev.png"
            ibtnNext.ImageUrl = "images/gnew_next.png"
        Else
            ibtnFirst.ImageUrl = "images/new_last.png"
            ibtnLast.ImageUrl = "images/new_first.png"
            ibtnPrevs.ImageUrl = "images/new_Prev.png"
            ibtnNext.ImageUrl = "images/new_next.png"

        End If
    End Sub
    ''' <summary>
    ''' Method to Move to First Record
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnMoveFirst()
        txtRecNo.Text = "1"
        FillData(0)
    End Sub
    ''' <summary>
    ''' Method to Move to Next Record
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnMoveNext()
        txtRecNo.Text = CInt(txtRecNo.Text) + 1
        FillData(CInt(txtRecNo.Text) - 1)
    End Sub
    ''' <summary>
    ''' Method to Move to Previous Record
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnMovePrevious()
        txtRecNo.Text = CInt(txtRecNo.Text) - 1
        FillData(CInt(txtRecNo.Text) - 1)
    End Sub
    ''' <summary>
    ''' Method to Move to Last Record
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnMoveLast()
        txtRecNo.Text = lblCount.Text
        FillData(CInt(lblCount.Text) - 1)
    End Sub

    ''' <summary>
    ''' Method to Fill the Field Values
    ''' </summary>
    ''' <param name="RecNo"></param>
    ''' <remarks></remarks>
    Private Sub FillData(ByVal RecNo As Integer)
        'Conditions for Button Enable & Disable
        If txtRecNo.Text = lblCount.Text Then
            ibtnNext.Enabled = False
            ibtnNext.ImageUrl = "images/gnew_next.png"
            ibtnLast.Enabled = False
            ibtnLast.ImageUrl = "images/gnew_last.png"
        Else
            ibtnNext.Enabled = True
            ibtnNext.ImageUrl = "images/new_next.png"
            ibtnLast.Enabled = True
            ibtnLast.ImageUrl = "images/new_last.png"
        End If
        If txtRecNo.Text = "1" Then
            ibtnPrevs.Enabled = False
            ibtnPrevs.ImageUrl = "images/gnew_Prev.png"
            ibtnFirst.Enabled = False
            ibtnFirst.ImageUrl = "images/gnew_first.png"
        Else
            ibtnPrevs.Enabled = True
            ibtnPrevs.ImageUrl = "images/new_prev.png"
            ibtnFirst.Enabled = True
            ibtnFirst.ImageUrl = "images/new_first.png"
        End If
        If txtRecNo.Text = 0 Then
            txtRecNo.Text = 1
        Else

            If lblCount.Text = 0 Then
                txtRecNo.Text = 0
            Else
                'PnlView.Visible = False
                PnlAdd.Visible = True
                Dim obj As StudentEn
                Dim loList As New List(Of StudentSponEn)
                Dim eobjStu As New StudentSponEn
                Dim objFeeTypes As New List(Of StuSponFeeTypesEn)
                Dim ListStu As New List(Of Student)
                Dim i As Integer = 0
                Dim j As Integer = 0
                Dim k As Integer = 0
                Dim a As Integer = 0
                Dim kk As Integer = 0
                Dim eKolej As New KolejEn
                Dim bKolej As New KolejBAL
                ListObjects = Session("ListObj")
                obj = ListObjects(RecNo)
                txtMatricNo.Text = obj.MatricNo
                Session("MatricNo") = txtMatricNo.Text
                txtName.Text = obj.StudentName
                ddlstudentStatus.SelectedValue = obj.StudentCode
                ddlRegistrationStatus.SelectedValue = obj.RegistrationStatus
                ddlIcNo.SelectedValue = obj.ID
                txtIcNo.Text = obj.ICNo
                Dim kokolist As New List(Of KolejEn)


                If obj.KokoCode = "" Or obj.KokoCode Is Nothing Then
                    ddlKokoList.SelectedValue = "-1"
                Else
                    eKolej.SAKO_Code = obj.KokoCode
                    Try
                        kokolist = bKolej.GetList(eKolej)
                    Catch ex As Exception
                        LogError.Log("Student", "FillDropDownList", ex.Message)
                    End Try

                    If kokolist.Count > 0 Then
                        Dim ko As Integer = 0
                        While ko < kokolist.Count
                            For Each ListItem In kokolist
                                If obj.KokoCode = kokolist(ko).SAKO_Code Then
                                    ddlKokoList.SelectedValue = obj.KokoCode
                                Else
                                    'lblMsg.Text = "Program Id " + obj.ProgramID + " does not exist"
                                    'lblMsg.Visible = True
                                    'ddlProgram.SelectedValue = "-1"
                                    Exit For
                                End If
                            Next
                            ko = ko + 1
                        End While
                        'ddlKokoList.SelectedValue = obj.KokoCode
                    Else
                        ddlKokoList.SelectedValue = "-1"
                    End If

                End If


                Dim listObj As New List(Of ProgramInfoEn)
                Dim listfaculty As New List(Of FacultyEn)
                Dim fac As Integer = 0
                'Student faculty and program
                LoadFaculty()
                listfaculty = ddlFaculty.DataSource
                While fac < ddlFaculty.DataSource.Count
                    For Each ListItem In ddlFaculty.Items
                        If obj.Faculty = listfaculty(fac).SAFC_Code Then
                            ddlFaculty.SelectedValue = obj.Faculty
                        Else
                            'lblMsg.Text = "Faculty " + obj.Faculty + " does not exist"
                            'lblMsg.Visible = True
                            Exit For
                        End If
                    Next
                    fac = fac + 1
                End While
                'If Not obj.Faculty = Session("ddlfaculty") Then
                '    ddlFaculty.SelectedValue = "-1"
                'Else
                '    ddlFaculty.SelectedValue = obj.Faculty
                'End If

                LoadProgram()
                listObj = ddlProgram.DataSource
                'listObj = Session("listObj")
                While a < ddlProgram.DataSource.Count
                    For Each ListItem In ddlProgram.Items
                        If obj.ProgramID = listObj(a).ProgramCode Then
                            ddlProgram.SelectedValue = obj.ProgramID
                        Else
                            'lblMsg.Text = "Program Id " + obj.ProgramID + " does not exist"
                            'lblMsg.Visible = True
                            'ddlProgram.SelectedValue = "-1"
                            Exit For
                        End If
                    Next
                    a = a + 1
                End While

                'If ddlProgram.Items.FindByText <> obj.ProgramID Then
                '    ddlProgram.SelectedValue = "-1"
                'Else

                'End If


                'study type and student category
                ddlStudyType.SelectedValue = obj.Studytype
                ddlStudentCategory.SelectedValue = obj.CategoryCode

                'bank
                With obj
                    If .SASI_Bank = "" Or .SASI_Bank Is Nothing Then
                        ddlBank.SelectedIndex = 0
                    Else
                        ddlBank.SelectedValue = obj.SASI_Bank
                    End If

                    If .SASI_AccNo = "" Or .SASI_AccNo Is Nothing Then
                        txtAccNo.Text = ""
                    Else
                        txtAccNo.Text = obj.SASI_AccNo
                    End If
                End With

                'current semester and session
                Dim bob As New SemesterSetupBAL
                Dim eob As New SemesterSetupEn
                'Dim Newsemyear As String
                Dim Oldsemyear As String
                Oldsemyear = obj.CurretSemesterYear
                'Newsemyear = Oldsemyear.Replace("/", "").Replace("-", "")
                obj.CurretSemesterYear = Oldsemyear.Replace("/", "").Replace("-", "")
                Try
                    eob = bob.GetSessionItem(obj.CurretSemesterYear)
                Catch ex As Exception
                    LogError.Log("Student", "FillData", ex.Message)
                End Try
                'Shah
                'LoadcurrentSemYer(eob.Semester)
                ddlSem.SelectedValue = obj.CurrentSemester
                'LoadcurrentSession()
                'ddlCurSession.SelectedValue = obj.CurretSemesterYear
                Dim d1, m1, y1, d2, m2, y2 As String
                If obj.CurrentSemester <> Nothing Then
                    LoadcurrentSession()
                    If Oldsemyear.Length = 9 Then
                        d1 = Mid(Oldsemyear, 1, 4)
                        m1 = Mid(Oldsemyear, 5, 4)
                        y1 = Mid(Oldsemyear, 9, 2)
                        Dim semestercode As String = d1 + "/" + m1 + "-" + y1
                        ddlCurSession.SelectedValue = semestercode
                    Else
                        ddlCurSession.SelectedValue = Oldsemyear
                    End If

                End If

                'intake semester and session
                Try
                    eob = bob.GetSessionItem(obj.Intake)
                Catch ex As Exception
                    LogError.Log("Student", "FillData", ex.Message)
                End Try
                'ddlIntkSemester.SelectedValue = eob.Semester
                'LoadIntakeSession()
                ddlIntakeSession.SelectedValue = obj.Intake

                If obj.Hostel = True Then
                    Dim eKolej1 As New KolejEn
                    Dim bKolej1 As New KolejBAL
                    Dim kolejlist As New List(Of KolejEn)
                    ddlHostel.SelectedValue = 1
                    If obj.SAKO_Code = "" Or obj.SAKO_Code Is Nothing Then
                        ddlKolej.SelectedValue = "-1"
                    Else
                        eKolej1.SAKO_Code = obj.SAKO_Code
                        Try
                            kolejlist = bKolej1.GetListKolej(eKolej1)
                        Catch ex As Exception
                            LogError.Log("Student", "FillDropDownList", ex.Message)
                        End Try

                        If kolejlist.Count > 0 Then
                            ddlKolej.SelectedValue = obj.SAKO_Code
                        Else
                            ddlKolej.SelectedValue = "-1"
                            obj.SABK_Code = "-1"
                        End If

                    End If
                    ddlRoomType.Enabled = True
                    ddlKolej.Enabled = True
                    ddlblock.Enabled = True
                    txtFloorNo.Enabled = True
                    LoadBlock()
                    If (obj.SABK_Code = "TIADA MAKLUMAT") Then
                        ddlblock.SelectedValue = "-1"
                    Else
                        ddlblock.SelectedValue = obj.SABK_Code
                    End If

                    LoadRoom()
                    ddlRoomType.SelectedValue = obj.SART_Code
                    txtFloorNo.Text = obj.SASI_FloorNo
                Else
                    ddlHostel.SelectedValue = 0
                    ddlRoomType.Enabled = False
                    ddlKolej.Enabled = False
                    ddlblock.Enabled = False
                    txtFloorNo.Enabled = False
                    ddlKolej.SelectedValue = "-1"
                    ddlblock.SelectedValue = "-1"
                    ddlRoomType.SelectedValue = "-1"
                    txtFloorNo.Text = ""
                End If
                txtkolejgit.Text = obj.SASI_OtherID
                If obj.SASI_GPA = 0 Then
                    txtGpa.Text = ""
                Else
                    txtGpa.Text = obj.SASI_GPA
                End If
                'If obj.SASI_CGPA = 0 Then
                '    txtcgpa.Text = ""
                'Else
                '    txtcgpa.Text = obj.SASI_CGPA
                'End If
                txtHostel.Text = ""

                If obj.SASI_CrditHrs = 0 Then
                    txtCreditHrs.Text = ""
                Else
                    txtCreditHrs.Text = obj.SASI_CrditHrs
                End If

                txtAdd1.Text = obj.SASI_Add1
                txtAdd2.Text = obj.SASI_Add2
                txtAdd3.Text = obj.SASI_Add3
                txtCity.Text = obj.SASI_City
                txtState.Text = obj.SASI_State

                ddlCountry.SelectedValue = obj.SASI_Country
                txtEmail.Text = obj.SASI_Email
                txtPhone.Text = obj.SASI_Tel
                txtmoblieno.Text = obj.SASI_HP
                txtPostcode.Text = obj.SASI_Postcode

                txtmAddress1.Text = obj.SASI_MAdd1
                txtmAddress2.Text = obj.SASI_MAdd2
                txtmAddress3.Text = obj.SASI_MAdd3
                txtmCity.Text = obj.SASI_MCity
                txtmState.Text = obj.SASI_MState
                ddlmCountry.SelectedValue = obj.SASI_MCountry

                With obj
                    If .FeeCat = "" Or .FeeCat Is Nothing Then
                        ddlFeeCat.SelectedIndex = 0
                    Else
                        ddlFeeCat.SelectedValue = obj.FeeCat
                    End If
                End With

                'ddlFeeCat.SelectedValue = obj.FeeCat
                txtmPostcode.Text = obj.SASI_MPostcode
                If txtMatricNo.Text Is Nothing Then
                    btnStuNotes.Enabled = False
                Else
                    StuMatrixNo = txtMatricNo.Text
                    Session("mno") = txtMatricNo.Text
                    btnStuNotes.Enabled = True
                End If

                If obj.SASI_StatusRec = True Then
                    ddlStatus.SelectedValue = 1
                Else
                    ddlStatus.SelectedValue = 0
                End If

                lstStudentFeetype1.Items.Clear()
                txtSpn1.Text = ""
                txtVFrom1.Text = ""
                txtspTo1.Text = ""
                txtSpCode.Text = ""
                txtSpnLimit.Text = ""
                chkSpnFeetypes1.Checked = True

                loList = obj.ListStuSponser
                If Not loList.Count = Nothing Then
                    Session("StuSpon") = loList
                    'End While
                    dgStuSponser.DataSource = loList
                    dgStuSponser.DataBind()
                Else
                    lstStudentFeetype1.Items.Clear()
                    txtSpn1.Text = ""
                    Session("StuSpon") = Nothing
                    dgStuSponser.DataSource = Nothing
                    dgStuSponser.DataBind()
                End If

            End If
        End If
        'SetDateFormat()
    End Sub

    ''' <summary>
    ''' Method to Load StudentFeeTypes
    ''' </summary>
    ''' <param name="Mcode"></param>
    ''' <remarks></remarks>
    Private Sub loadStufeeType(ByVal Mcode As String)
        Dim bobj As New StudentBAL
        Dim espn As New SponsorEn
        Dim eobj As New StuSponFeeTypesEn
        Dim i As Integer
        Dim objspn As New SponsorBAL
        Dim bobjstuSpn As New StuSponFeeTypesBAL
        Dim listStuFeetypes As New List(Of StuSponFeeTypesEn)
        eobj.MatricNo = Mcode

        Try
            listStuFeetypes = bobjstuSpn.GetStuSpnFTList(eobj)
        Catch ex As Exception
            LogError.Log("Student", "loadStufeeType", ex.Message)
        End Try
        If listStuFeetypes.Count = 0 Then
            lstStudentFeetype1.Items.Clear()
        Else
            While i < listStuFeetypes.Count
                eobj = listStuFeetypes(i)
                lstStudentFeetype1.Items.Add(New ListItem(eobj.FeeDesc, eobj.SAFT_Code))
                i = i + 1
            End While

        End If

    End Sub
    ''' <summary>
    ''' Method to get the MenuName
    ''' </summary>
    ''' <param name="MenuId">Parameter is MenuId</param>
    ''' <remarks></remarks>
    Private Sub Menuname(ByVal MenuId As Integer)
        Dim eobj As New MenuEn
        Dim bobj As New MenuBAL
        eobj.MenuId = MenuId
        Try
            eobj = bobj.GetMenus(eobj)
        Catch ex As Exception
            LogError.Log("Student", "Menuname", ex.Message)
        End Try
        lblMenuName.Text = eobj.MenuName
    End Sub
    ''' <summary>
    ''' Method to Load Block Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadBlock()
        Dim eBlock As New BlockEn
        Dim bBlock As New BlockBAL
        ddlblock.Items.Clear()
        ddlblock.Items.Add(New ListItem("---Select---", "-1"))
        ddlblock.DataTextField = "SABK_Description"
        ddlblock.DataValueField = "SABK_Code"
        Try
            ddlblock.DataSource = bBlock.GetBlockList(ddlKolej.SelectedValue)
        Catch ex As Exception
            LogError.Log("Student", "LoadBlock", ex.Message)
        End Try
        ddlblock.DataBind()
        ddlRoomType.Items.Clear()
        ddlRoomType.Items.Add(New ListItem("---Select---", "-1"))
    End Sub
    ''' <summary>
    ''' Method to Load Room Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadRoom()
        Dim bRoomType As New RoomTypeBAL
        ddlRoomType.Items.Clear()
        ddlRoomType.Items.Add(New ListItem("---Select---", "-1"))
        ddlRoomType.DataTextField = "SART_Description"
        ddlRoomType.DataValueField = "SART_Code"

        Try
            ddlRoomType.DataSource = bRoomType.GetRoomTypeList(ddlblock.SelectedValue)
        Catch ex As Exception
            LogError.Log("Student", "LoadRoom", ex.Message)
        End Try
        ddlRoomType.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Load Student Tab
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadViewOne()
        'imgLeft1.ImageUrl = "images/b_white_left.gif"
        'imgRight1.ImageUrl = "images/b_white_right.gif"
        btnBatchInvoice.CssClass = "TabButtonClick"
        imgLeft2.ImageUrl = "images/b_orange_left.gif"
        imgRight2.ImageUrl = "images/b_orange_right.gif"
        btnSelection.CssClass = "TabButton"
        MultiView1.SetActiveView(View1)
    End Sub
    ''' <summary>
    ''' Method to Load Current Session Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadcurrentSemYer(ByVal enSem As String)
        Dim objBAL As New SemesterSetupBAL
        Dim listObj As New SemesterSetupEn
        Try
            listObj = objBAL.GetSessionItem(enSem)
            ddlSem.SelectedValue = listObj.SemisterSetupCode
        Catch ex As Exception
            LogError.Log("Student", "LoadcurrentSemYear", ex.Message)
        End Try

    End Sub
    ''' <summary>
    ''' Method to Load Current Session Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadcurrentSession()
        Dim objEn As New SemesterSetupEn
        Dim objBAL As New SemesterSetupBAL
        Dim listObj As New List(Of SemesterSetupEn)

        Try
            'listObj = objBAL.GetSessionList(ddlSem.SelectedItem.ToString())
            'listObj = objBAL.GetCurrentSessionList(objEn)
            listObj = objBAL.GetListSemesterCode(objEn)
        Catch ex As Exception
            LogError.Log("Student", "LoadcurrentSession", ex.Message)
        End Try
        ddlCurSession.Items.Clear()
        ddlCurSession.Items.Add(New ListItem("--Select--", "-1"))
        ddlCurSession.DataTextField = "SemisterCode2"
        ddlCurSession.DataValueField = "SemisterSetupCode"
        ddlCurSession.DataSource = listObj
        ddlCurSession.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Load Intake Session Dropdown
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadIntakeSession()
        Dim objEn As New SemesterSetupEn
        Dim objBAL As New SemesterSetupBAL
        Dim listObj As New List(Of SemesterSetupEn)

        Try
            'listObj = objBAL.GetSessionList(ddlIntkSemester.SelectedItem.ToString())
            listObj = objBAL.GetCurrentSessionList(objEn)
        Catch ex As Exception
            LogError.Log("Student", "LoadIntakeSession", ex.Message)
        End Try
        ddlIntakeSession.Items.Clear()
        ddlIntakeSession.Items.Add(New ListItem("--Select--", "-1"))
        ddlIntakeSession.DataTextField = "SemisterCode2"
        ddlIntakeSession.DataValueField = "SemisterSetupCode"
        ddlIntakeSession.DataSource = listObj
        ddlIntakeSession.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Load Sponsor Grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadgrid()
        Dim bsponser As New StudentSponBAL
        Dim esponser As New StudentSponEn
        Dim listobj As New List(Of StudentSponEn)
        listobj = Session("StuSpon") 'bsponser.GetStuSponsorList(esponser)
        dgStuSponser.DataSource = listobj
        dgStuSponser.DataBind()
    End Sub
    ''' <summary>
    ''' Method to Add StudentSponsors
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddSposor()
        If lstStudentFeetype1.Items.Count <> 0 Or chkSpnFeetypes1.Checked = True Then
            Dim i As Integer = 0
            Dim eobj As New StudentSponEn
            Dim loList As New List(Of StudentSponEn)
            Dim objFeeTypes As New List(Of StuSponFeeTypesEn)
            eobj.Name = txtSpn1.Text
            eobj.Sponsor = txtSpCode.Text
            eobj.SponserCode = txtSpCode.Text
            eobj.SDate = txtVFrom1.Text
            eobj.EDate = txtspTo1.Text
            'eobj.SponsorLimit = txtSpnLimit.Text
            If Trim(txtSpnLimit.Text).Length <> 0 Then
                eobj.SponsorLimit = Trim(txtSpnLimit.Text)
            Else
                eobj.SponsorLimit = 0
            End If
            If chkSpnFeetypes1.Checked = True Then
                eobj.FullySponsered = True
            Else
                eobj.FullySponsered = False
                While i < lstStudentFeetype1.Items.Count
                    Dim objFee As New StuSponFeeTypesEn
                    objFee.MatricNo = txtMatricNo.Text
                    objFee.FeeDesc = lstStudentFeetype1.Items(i).Text
                    objFee.SAFT_Code = lstStudentFeetype1.Items(i).Value
                    objFee.SASR_Code = eobj.SponserCode
                    objFeeTypes.Add(objFee)
                    objFee = Nothing
                    i = i + 1
                End While
            End If
            eobj.ListStuSponFeeTypes = objFeeTypes

            If Not Session("StuSpon") Is Nothing Then
                loList = Session("StuSpon")
            End If
            i = 0
            Dim SFlag As Boolean = False
            While i < loList.Count
                If loList(i).Sponsor = eobj.SponserCode Then
                    SFlag = True
                    loList(i) = eobj
                    Exit While
                End If
                i = i + 1
            End While

            If SFlag = False Then
                loList.Add(eobj)
            End If
            If loList.Count > 0 Then
                Session("StuSpon") = loList
                loadgrid()
            Else
                lblMsg.Text = "Add a Sponsor"
                Exit Sub
            End If
        Else
            lblMsg.Text = "Add a FeeType for Sponsor"

        End If
        'Session("StuSpon") = Nothing
    End Sub
#End Region

    Protected Sub txtspTo1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtspTo1.TextChanged, txtspTo1.Load

    End Sub

    Protected Sub ddlFaculty_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlFaculty.SelectedIndexChanged
        LoadProgram()
    End Sub


    Protected Sub ddlKolej_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlKolej.SelectedIndexChanged
        LoadBlock()
    End Sub


    Protected Sub chkmadd_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkmadd.CheckedChanged
        If chkmadd.Checked = True Then
            txtmAddress1.Text = txtAdd1.Text
            txtmAddress2.Text = txtAdd2.Text
            txtmAddress3.Text = txtAdd3.Text
            txtmCity.Text = txtCity.Text
            txtmState.Text = txtState.Text
            txtmPostcode.Text = txtPostcode.Text
            ddlmCountry.SelectedValue = ddlCountry.SelectedValue
        Else
            txtmAddress1.Text = ""
            txtmAddress2.Text = ""
            txtmAddress3.Text = ""
            txtmCity.Text = ""
            txtmState.Text = ""
            ddlmCountry.SelectedValue = -1
        End If
    End Sub

    Protected Sub ddlblock_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlblock.SelectedIndexChanged
        LoadRoom()
    End Sub


    Protected Sub ddlProgram_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlblock.SelectedIndexChanged

    End Sub
    Protected Sub btnBatchInvoice_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBatchInvoice.Click
        LoadViewOne()
    End Sub


    Protected Sub btnSelection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelection.Click
        'imgLeft2.ImageUrl = "images/b_white_left.gif"
        'imgRight2.ImageUrl = "images/b_white_right.gif"
        btnSelection.CssClass = "TabButtonClick"
        'imgLeft1.ImageUrl = "images/b_orange_left.gif"
        'imgRight1.ImageUrl = "images/b_orange_right.gif"
        btnBatchInvoice.CssClass = "TabButton"
        MultiView1.SetActiveView(View2)
    End Sub

    Protected Sub chkSpnFeetypes1_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        If chkSpnFeetypes1.Checked = True Then
            'lstStudentFeetype1.Items.Clear()
            lstStudentFeetype1.Enabled = False
            ibtn_spn1_feetype.Enabled = False
            ibtn_spn1_feetype.ImageUrl = "images/find_dull.gif"
        Else
            lstStudentFeetype1.Enabled = True
            ibtn_spn1_feetype.Enabled = True

            Dim bsobjSpoFeetype As New SponsorFeeTypesBAL
            Dim eobjSponfee As New SponsorFeeTypesEn
            Dim objFeeTypes As New List(Of SponsorFeeTypesEn)
            Dim i As Integer = 0

            eobjSponfee.SponserCode = txtSpCode.Text
            'lstStudentFeetype1.Items.Clear()

            Try
                objFeeTypes = bsobjSpoFeetype.GetSPFeeTypeList(eobjSponfee)
            Catch ex As Exception
                LogError.Log("Student", "chkSpnFeetypes1_CheckedChanged", ex.Message)
            End Try
            While i < objFeeTypes.Count
                lstStudentFeetype1.Items.Add(New ListItem(objFeeTypes(i).FeeTypeDesc, objFeeTypes(i).FeeTypeCode))
                i = i + 1
            End While
            ibtn_spn1_feetype.ImageUrl = "images/find_img.png"
        End If
    End Sub

    Protected Sub lstStudentFeetype2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
    Protected Sub ddlSem_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSem.SelectedIndexChanged
        'LoadcurrentSession()
    End Sub


    Protected Sub ddlHostel_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlHostel.SelectedIndexChanged
        If ddlHostel.SelectedValue = "1" Then
            ddlRoomType.Enabled = True
            ddlKolej.Enabled = True
            ddlblock.Enabled = True
            txtFloorNo.Enabled = True
        Else
            ddlRoomType.Enabled = False
            ddlKolej.Enabled = False
            ddlblock.Enabled = False
            txtFloorNo.Enabled = False
            ddlKolej.SelectedValue = "-1"
            ddlblock.SelectedValue = "-1"
            ddlRoomType.SelectedValue = "-1"
            txtFloorNo.Text = ""
        End If

    End Sub

    Protected Sub ddlIntkSemester_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlIntkSemester.SelectedIndexChanged
        LoadIntakeSession()
    End Sub

    Protected Sub dgStuSponser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        If dgStuSponser.SelectedIndex <> -1 Then
            Dim eobj As New List(Of StudentSponEn)
            Dim eobj1 As New StudentEn
            Dim dgitem As DataGridItem
            Dim link As LinkButton
            dgitem = dgStuSponser.SelectedItem
            link = dgitem.Cells(0).Controls(0)
            Dim i As Integer = 0
            txtSpn1.Text = dgitem.Cells(1).Text
            txtVFrom1.Text = dgitem.Cells(2).Text
            txtspTo1.Text = dgitem.Cells(3).Text
            txtSpnLimit.Text = dgitem.Cells(5).Text

            txtSpCode.Text = link.Text
            If dgitem.Cells(4).Text = True Then
                chkSpnFeetypes1.Checked = True
                lstStudentFeetype1.Enabled = False
                ibtn_spn1_feetype.Enabled = False
                ibtn_spn1_feetype.ImageUrl = "images/find_dull.gif"

            Else
                chkSpnFeetypes1.Checked = False
                lstStudentFeetype1.Enabled = True
                ibtn_spn1_feetype.Enabled = True
                ibtn_spn1_feetype.ImageUrl = "images/find_img.png"
            End If
            Dim listSponFeeTypes As New List(Of StuSponFeeTypesEn)
            eobj = Session("StuSpon")

            Dim bsobjSpoFeetype As New SponsorFeeTypesBAL
            Dim eobjSponfee As New SponsorFeeTypesEn
            Dim objFeeTypes As New List(Of SponsorFeeTypesEn)

            eobjSponfee.SponserCode = txtSpCode.Text

            Try
                objFeeTypes = bsobjSpoFeetype.GetSPFeeTypeList(eobjSponfee)
            Catch ex As Exception
                LogError.Log("Student", "dgStuSponser_SelectedIndexChanged", ex.Message)
            End Try
            lstStudentFeetype1.Items.Clear()
            While i < objFeeTypes.Count
                lstStudentFeetype1.Items.Add(New ListItem(objFeeTypes(i).FeeTypeDesc, objFeeTypes(i).FeeTypeCode))
                i = i + 1
            End While

        End If
    End Sub


    Protected Sub ddlStudentCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        ddlFeeCat.SelectedValue = ddlStudentCategory.SelectedValue
    End Sub

    Protected Sub btnAddSponser_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        AddSposor()
        dates()

    End Sub

    Protected Sub btnDelSponser_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If dgStuSponser.SelectedIndex <> -1 Then
            Dim dgitem As DataGridItem
            Dim i As Integer = 0
            Dim k As Integer = 0
            For Each dgitem In dgStuSponser.Items
                dgitem.Cells(0).Text = i
                i = i + 1
            Next
            loList = Session("StuSpon")
            If Not loList Is Nothing Then
                If dgStuSponser.SelectedIndex <> -1 Then
                    loList.RemoveAt(CInt(dgStuSponser.SelectedItem.Cells(0).Text))
                    dgStuSponser.DataSource = loList
                    dgStuSponser.DataBind()
                    If loList.Count <> 0 Then
                        Session("StuSpon") = loList
                        'Dim j As Integer = 0
                        'While j < loList.Count
                        '    txtSpn1.Text = loList(j).Name
                        '    txtVFrom1.Text = loList(j).SDate
                        '    txtspTo1.Text = loList(j).EDate
                        '    txtSpCode
                        '    Dim objFeeTypes As New List(Of StuSponFeeTypesEn)
                        '    lstStudentFeetype1.Items.Clear()
                        '    objFeeTypes = loList(j).ListStuSponFeeTypes
                        '    While k < loList(j).ListStuSponFeeTypes.Count
                        '        lstStudentFeetype1.Items.Add(New ListItem(objFeeTypes(k).FeeDesc, objFeeTypes(k).SAFT_Code))
                        '        k = k + 1
                        '    End While
                        '    j = j + 1
                        'End While
                        txtSpn1.Text = ""
                        txtVFrom1.Text = ""
                        txtspTo1.Text = ""
                        txtSpCode.Text = ""
                        lstStudentFeetype1.Items.Clear()
                        dates()
                    Else
                        Session("StuSpon") = Nothing
                        Session("spnCode1") = Nothing
                        txtSpn1.Text = ""
                        txtVFrom1.Text = ""
                        txtspTo1.Text = ""
                        txtSpCode.Text = ""
                        lstStudentFeetype1.Items.Clear()
                        dates()
                    End If
                    dgStuSponser.SelectedIndex = -1
                End If
            End If
        Else
            lblMsg.Text = "Select A Sponsor to Remove"
            lblMsg.Visible = True
        End If
    End Sub



    Protected Sub btnStuNotes_Click(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub

    Protected Sub btnStuNotes_Click1(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub

    Protected Sub ibtnPrint_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnPrint.Click
        'Response.Redirect("ProcessReport/RptStudentInfo.aspx
        Response.Redirect("ProcessReport/StudentInfo.rpt")
    End Sub

    Protected Sub btnHidden_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHidden.Click

    End Sub

    Protected Sub ibtn_spn1_feetype_Click(sender As Object, e As ImageClickEventArgs) Handles ibtn_spn1_feetype.Click

    End Sub

End Class
