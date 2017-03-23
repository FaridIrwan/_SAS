Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.Entities
Imports System.Data
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Configuration
Imports CrystalDecisions.CrystalReports.Engine

Partial Class RptCollectionAnalysis
    Inherits System.Web.UI.Page

    Private Sub Menuname(ByVal MenuId As Integer)
        Dim eobj As New MenuEn
        Dim bobj As New MenuBAL
        eobj.MenuId = MenuId
        eobj = bobj.GetMenus(eobj)
        lblMenuName.Text = eobj.MenuName
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            Menuname(CInt(Request.QueryString("Menuid")))
            ibtnFDate.Attributes.Add("onClick", "return getibtnFDate()")
            ibtnTodate.Attributes.Add("onClick", "return getDateto()")
            ibtnPrint.Attributes.Add("onclick", "return getDate()")
            ibtnView.Attributes.Add("onclick", "return getDate()")
            ibtnView.Attributes.Add("onclick", "new_window=window.open('../GroupReport/RptCollectionAnalysisViewer.aspx','Hanodale','width=520,height=400,resize=1,scrollbars=1');new_window.focus();")
            txtFrom.Attributes.Add("OnKeyup", "return CheckFromDate()")
            txtTodate.Attributes.Add("OnKeyup", "return CheckToDate()")
            LoadUserRights()
            txtFrom.ReadOnly = True
            txtTodate.ReadOnly = True
            ibtnFDate.Visible = False
            ibtnTodate.Visible = False
            Sponsor()
            dates()
            Session("program") = Nothing
            Session("faculty") = Nothing
            Session("sponsor") = Nothing
            Session("status") = Nothing
            Session("sortby") = Nothing
        End If
    End Sub
    Private Sub dates()
        txtFrom.Text = Format(Date.Now, "dd/MM/yyyy")
        txtTodate.Text = Format(Date.Now, "dd/MM/yyyy")
    End Sub   
    Private Sub Sponsor()
        Dim eobjSponsorEn As New SponsorEn
        Dim objSponsorBs As New SponsorBAL
        Dim listSponsor As New List(Of SponsorEn)
        listSponsor = objSponsorBs.GetList(eobjSponsorEn)
        ddlSponsor.Items.Clear()
        ddlSponsor.DataSource = listSponsor
        ddlSponsor.DataTextField = "Name"
        ddlSponsor.DataValueField = "SponserCode"
        ddlSponsor.DataBind()
        ddlSponsor.Items.Insert(0, New ListItem("-- Select --", "-1"))
        ddlSponsor.Items.Insert(1, New ListItem("Select All", "0"))
    End Sub
    Private Sub LoadUserRights()

        Dim obj As New UsersBAL
        Dim eobj As UserRightsEn

        'eobj = obj.GetUserRights(5, 1)
        eobj = obj.GetUserRights(CInt(Request.QueryString("Menuid")), CInt(Session("UserGroup")))
        'Rights for Add
        If eobj.IsAdd = True Then
            ibtnSave.Enabled = True
            'OnAdd()
        Else
            ibtnNew.Enabled = False
            ibtnNew.ImageUrl = "../images/gAdd.png"
            ibtnNew.ToolTip = "Access Denied"
            ibtnDelete.Enabled = False
            ibtnDelete.ImageUrl = "../images/gdelete.png"
            ibtnDelete.ToolTip = "Access Denied"
            '-----------------------------------------------
            ibtnFirst.Enabled = False
            ibtnLast.Enabled = False
            ibtnPrevs.Enabled = False
            ibtnNext.Enabled = False
            ibtnFirst.ToolTip = "Access Denied"
            ibtnLast.ToolTip = "Access Denied"
            ibtnPrevs.ToolTip = "Access Denied"
            ibtnNext.ToolTip = "Access Denied"
            ibtnFirst.ImageUrl = "../images/gnew_first.png"
            ibtnLast.ImageUrl = "../images/gnew_last.png"
            ibtnPrevs.ImageUrl = "../images/gnew_Prev.png"
            ibtnNext.ImageUrl = "../images/gnew_next.png"
            '------------------------------------------------
            ibtnSave.Enabled = False
            ibtnSave.ImageUrl = "../images/gsave.png"
            ibtnSave.ToolTip = "Access Denied"
        End If
        'Rights for Edit
        If eobj.IsEdit = True Then
            'ibtnSave.ToolTip = "Access Denied"
            Session("EditFlag") = True
        Else
            Session("EditFlag") = False
        End If
        'Rights for View
        ibtnView.Enabled = eobj.IsView
        If eobj.IsView = True Then
            ibtnView.ImageUrl = "../images/find.png"
            ibtnView.Enabled = True
        Else
            ibtnView.ImageUrl = "../images/gfind.png"
            ibtnView.ToolTip = "Access Denied"
        End If

        'Commenetd and uncommenetd by Zoya @ 26/02/2016
        'Rights for Print
        ibtnPrint.Enabled = eobj.IsPrint
        If eobj.IsPrint = True Then
            'EnablePrintUserRights()
            ibtnPrint.Enabled = True
            ibtnPrint.ImageUrl = "../images/print.png"
            ibtnPrint.ToolTip = "Print"
        Else
            'DisablePrintUserRights()
            ibtnPrint.Enabled = False
            ibtnPrint.ImageUrl = "../images/gprint.png"
            ibtnPrint.ToolTip = "Access Denied"
        End If
        'End Commenetd and uncommenetd by Zoya @ 26/02/2016

        'Checking Default mode
        If eobj.IsAddModeDefault = True Then
            ' pnlView.Visible = False
            'pnlAdd.Visible = True
        Else
            'pnlAdd.Visible = False
            ' pnlView.Visible = True
            'LoadGrid()
        End If
        If eobj.IsOthers = True Then
            ibtnOthers.Enabled = True
            ibtnOthers.ImageUrl = "../images/others.png"
            ibtnOthers.ToolTip = "Others"
        Else
            ibtnOthers.Enabled = False
            ibtnOthers.ImageUrl = "../images/gothers.png"
            ibtnOthers.ToolTip = "Access Denied"
        End If
        If eobj.IsPost = True Then
            ibtnPosting.Enabled = True
            ibtnPosting.ImageUrl = "../images/posting.png"
            ibtnPosting.ToolTip = "Posting"
        Else
            ibtnPosting.Enabled = False
            ibtnPosting.ImageUrl = "../images/gposting.png"
            ibtnPosting.ToolTip = "Access Denied"
        End If
    End Sub
    Protected Sub ChkDateRange_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChkDateRange.CheckedChanged
        If ChkDateRange.Checked = True Then
            txtFrom.ReadOnly = False
            txtTodate.ReadOnly = False
            ibtnFDate.Visible = True
            ibtnTodate.Visible = True
        Else
            txtFrom.ReadOnly = True
            txtTodate.ReadOnly = True
            ibtnFDate.Visible = False
            ibtnTodate.Visible = False
            dates()
        End If
    End Sub

    Protected Sub ibtnPrint_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        'Dim cryRpt As ReportDocument = New ReportDocument
        'cryRpt.Load(~/GroupReport/RptCollectionAnalysis.rpt")",crysta
    End Sub

    Protected Sub ibtnView_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        
    End Sub

    Protected Sub ibtnNew_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        'Modified by Hafiz @ 10/3/2016
        Call ibtnCancel_Click(sender, e)
    End Sub

    Protected Sub ibtnCancel_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Session("program") = Nothing
        Session("faculty") = Nothing
        Session("sponsor") = Nothing
        Session("status") = Nothing
        Panel3.Visible = False
        ddlType.SelectedIndex = -1
        lblSponsor.Visible = False
        ddlSponsor.Visible = False
        ddlSponsor.SelectedIndex = -1
        ChkDateRange.Checked = False
        txtFrom.ReadOnly = True
        txtTodate.ReadOnly = True
        ibtnFDate.Visible = False
        ibtnTodate.Visible = False
        dates()
    End Sub
    Protected Sub ibtnDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

    End Sub

    Protected Sub ddltype_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim type As String
        type = ddlType.SelectedValue
        If type = "-1" Then
            lblSponsor.Visible = False
            ddlSponsor.Visible = False
            Panel3.Visible = False
            Session("type") = Nothing
        ElseIf type = "1" Then
            lblSponsor.Visible = True
            ddlSponsor.Visible = True
            Panel3.Visible = False
            Session("type") = type
        ElseIf type = "2" Then
            lblSponsor.Visible = False
            ddlSponsor.Visible = False
            Panel3.Visible = True
            Session("type") = type
        End If
    End Sub

    Protected Sub ddlSponsor_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim sponsor As String
        sponsor = ddlSponsor.SelectedValue
        If sponsor = "0" Or sponsor = "-1" Then
            'sponsor = "%"
            Session("sponsor") = Nothing
        Else
            Session("sponsor") = sponsor
        End If
    End Sub
    Protected Sub txtFrom_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub
    Protected Sub chkinvoice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Invoice As String


        If chkinvoice.Checked = True Then
            Invoice = chkinvoice.Checked
            Session("invoice") = Invoice
        ElseIf chkinvoice.Checked = False Then
            'student = chkstudent.Checked
            Session("invoice") = Nothing
        End If
    End Sub
    Protected Sub chkdateinvoice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim dateinvoice As String


        If chkdateinvoice.Checked = True Then
            dateinvoice = chkdateinvoice.Checked
            Session("dateinvoice") = dateinvoice
        ElseIf chkdateinvoice.Checked = False Then
            'student = chkstudent.Checked
            Session("dateinvoice") = Nothing
        End If
    End Sub
    Protected Sub chkamtinvoice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim amtinvoice As String


        If chkamtinvoice.Checked = True Then
            amtinvoice = chkamtinvoice.Checked
            Session("amtinvoice") = amtinvoice
        ElseIf chkamtinvoice.Checked = False Then
            'student = chkstudent.Checked
            Session("amtinvoice") = Nothing
        End If
    End Sub
    Protected Sub chkreceipt_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim receipt As String


        If chkreceipt.Checked = True Then
            receipt = chkreceipt.Checked
            Session("receipt") = receipt
        ElseIf chkreceipt.Checked = False Then
            'student = chkstudent.Checked
            Session("receipt") = Nothing
        End If
    End Sub
    Protected Sub chkdatereceipt_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim datereceipt As String


        If chkdatereceipt.Checked = True Then
            datereceipt = chkdatereceipt.Checked
            Session("datereceipt") = datereceipt
        ElseIf chkdatereceipt.Checked = False Then
            'student = chkstudent.Checked
            Session("datereceipt") = Nothing
        End If
    End Sub
    Protected Sub chkamtreceipt_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim amtreceipt As String


        If chkamtreceipt.Checked = True Then
            amtreceipt = chkamtreceipt.Checked
            Session("amtreceipt") = amtreceipt
        ElseIf chkamtreceipt.Checked = False Then
            'student = chkstudent.Checked
            Session("amtreceipt") = Nothing
        End If
    End Sub
End Class
