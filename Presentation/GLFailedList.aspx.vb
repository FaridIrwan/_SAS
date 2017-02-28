Imports HTS.SAS.Entities
Imports HTS.SAS.BusinessObjects
Imports HTS.SAS.DataAccessObjects
Imports MaxGeneric
Imports System.Data
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Linq

'added by Hafiz @ 21/02/2017

Partial Class GLFailedList
    Inherits System.Web.UI.Page

    Dim gv As New GridView()
    Dim MenuId As Integer, Batchcode As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        MenuId = Request.QueryString("MenuId")

        If Not IsPostBack() Then
            Session("LoadGrid") = Nothing

            LoadTextBox()
            LoadGLFailedList()
        End If

    End Sub

    'Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
    'End Sub

    Protected Sub LoadTextBox()
        Dim _MenuMasterEn As MenuMasterEn = New MenuDAL().GetMenuMasterList().FindAll(Function(x) x.MenuID = MenuId).FirstOrDefault()

        txtProcessName.Text = _MenuMasterEn.PageName
        txtTFailedTime.Text = DateTime.Now
        hdnPageName.Value = _MenuMasterEn.PageName
    End Sub

    Protected Sub LoadGLFailedList()

        Try
            Batchcode = Request.QueryString("Batchcode")
            If String.IsNullOrEmpty(Batchcode) Then
                Throw New Exception("Batch number not found or empty.")
            End If

            If Not Session("List_Failed") Is Nothing Then

                Dim table1 As New HtmlTable
                Dim FailedList As List(Of WorkflowEn) = Session("List_Failed")
                Session("SubType") = FailedList.Select(Function(x) x.SUBTYPE).Distinct.FirstOrDefault()

                If FailedList.Where(Function(x) x.SUBTYPE = "Sponsor").Count > 0 Then
                    'SPONSOR
                    If hdnPageName.Value.Contains("Sponsor Allocation") Then

                        If FailedList.Select(Function(x) x.FAC).Distinct().Count() > 0 Then
                            Dim AllocationList As List(Of WorkflowEn) = FailedList.FindAll(Function(x) Not String.IsNullOrEmpty(x.FAC)).ToList()

                            Session("SubType") = "Student"
                            GridViewStudent(AllocationList, table1)
                        End If

                        If FailedList.Select(Function(x) String.IsNullOrEmpty(x.FAC)).Distinct().Count() > 0 Then
                            Dim AllocationList As List(Of WorkflowEn) = FailedList.FindAll(Function(x) String.IsNullOrEmpty(x.FAC)).ToList()

                            Session("SubType") = "Sponsor"
                            GridViewSponsor(AllocationList, table1)
                        End If

                    Else
                        '!= Sponsor Allocation
                        GridViewSponsor(FailedList, table1)

                    End If

                Else
                    'STUDENT
                    GridViewStudent(FailedList, table1)

                End If

                PlaceHolder1.Controls.Add(table1)

            Else
                UpdateGrid()
            End If

        Catch ex As Exception
            lblMsg.Visible = True
            lblMsg.Text = ex.Message.ToString()
        End Try

    End Sub

    'CONTRUCT SPONSOR GRIDVIEW - start
    Protected Sub GridViewSponsor(ByVal FailedList As List(Of WorkflowEn), ByRef table1 As HtmlTable)

        Dim rowSpn As New HtmlTableRow()
        Dim cellSpn As New HtmlTableCell()
        gv = New GridView()

        Session("LoadGrid") = FailedList.GroupBy(Function(x) New With {Key x.ID, x.NAME, x.SOURCE, x.SUBTYPE}) _
                            .Select(Function(y) New With
                                                {.ID = y.Key.ID,
                                                 .NAME = y.Key.NAME,
                                                 .SOURCE = y.Key.SOURCE,
                                                 .SUBTYPE = y.Key.SUBTYPE}).OrderBy(Function(z) z.ID).ToList()
        LoadGrid(1, gv)

        cellSpn.Controls.Add(gv)
        rowSpn.Cells.Add(cellSpn)
        table1.Rows.Add(rowSpn)

    End Sub
    'CONTRUCT SPONSOR GRIDVIEW - end

    'CONTRUCT STUDENT GRIDVIEW - start
    Protected Sub GridViewStudent(ByVal FailedList As List(Of WorkflowEn), ByRef table1 As HtmlTable)

        Dim Faculty As List(Of String) = FailedList.Select(Function(x) x.FAC).Distinct().ToList()
        For cntFac As Integer = 0 To Faculty.Count() - 1

            Dim fac As String = Faculty(cntFac)
            Dim rowFac As New HtmlTableRow()
            Dim cellFac As New HtmlTableCell()
            Dim lblFac As New Label()

            lblFac.Font.Size = 12
            lblFac.Font.Bold = True
            lblFac.Text = "Faculty: " & fac
            cellFac.Controls.Add(lblFac)

            rowFac.Cells.Add(cellFac)
            table1.Rows.Add(rowFac)

            Dim Program As List(Of String) = FailedList.Where(Function(x) x.FAC = fac).Select(Function(y) y.PROG).Distinct().ToList()
            For cntProg As Integer = 0 To Program.Count - 1

                Dim prog As String = Program(cntProg)
                Dim rowProg As New HtmlTableRow()
                Dim cellProg As New HtmlTableCell()
                Dim lblProg As New Label()
                gv = New GridView()

                lblProg.Font.Size = 10
                lblProg.Font.Bold = True
                lblProg.ForeColor = Drawing.Color.DarkBlue
                lblProg.Text = "Program: " & prog
                cellProg.Controls.Add(lblProg)
                cellProg.Controls.Add(New LiteralControl("<br />"))
                cellProg.Controls.Add(New LiteralControl("<br />"))

                rowProg.Cells.Add(cellProg)
                table1.Rows.Add(rowProg)

                'construct gv - start
                Session("LoadGrid") = FailedList.Where(Function(i) i.FAC = fac And i.PROG = prog).GroupBy(Function(x) New With {Key x.ID, x.NAME, x.KOL, x.SOURCE, x.SUBTYPE}) _
                                    .Select(Function(y) New With
                                                        {.ID = y.Key.ID,
                                                         .NAME = y.Key.NAME,
                                                         .KOL = y.Key.KOL,
                                                         .SOURCE = y.Key.SOURCE,
                                                         .SUBTYPE = y.Key.SUBTYPE}).OrderBy(Function(z) z.ID).ToList()
                LoadGrid(cntProg, gv)

                cellProg.Controls.Add(gv)
                cellProg.Controls.Add(New LiteralControl("<br />"))
                cellProg.Controls.Add(New LiteralControl("<br />"))
                'construct gv - end

                rowProg.Cells.Add(cellProg)
                table1.Rows.Add(rowProg)

            Next cntProg

        Next cntFac

    End Sub
    'CONTRUCT STUDENT GRIDVIEW - end

    Protected Sub LoadGrid(ByVal cntProg As Integer, ByRef gv As GridView)

        LoadGridViewSetting(gv)

        gv.ID = "GridView" & cntProg

        If Session("SubType") = "Student" Then
            gv.DataSource = Session("LoadGrid")
            gv.DataBind()

            If gv.Columns.Count > 0 Then
                gv.Columns(4).Visible = False
            Else
                gv.HeaderRow.Cells(4).Visible = False
                For Each gvr As GridViewRow In gv.Rows
                    gvr.Cells(4).Visible = False
                Next
            End If

        ElseIf Session("SubType") = "Sponsor" Then
            gv.DataSource = Session("LoadGrid")
            gv.DataBind()

            If gv.Columns.Count > 0 Then
                gv.Columns(3).Visible = False
            Else
                gv.HeaderRow.Cells(3).Visible = False
                For Each gvr As GridViewRow In gv.Rows
                    gvr.Cells(3).Visible = False
                Next
            End If

        End If

    End Sub

    Protected Sub LoadGridViewSetting(ByRef gv As GridView)

        With gv
            '.AutoGenerateColumns = False
            '.AllowPaging = True
            '.PageSize = 15
            .Width = Unit.Percentage(120%)
            .FooterStyle.CssClass = "dgFooterStyle"
            .SelectedRowStyle.CssClass = "dgSelectedItemStyle"
            With .AlternatingRowStyle
                .BackColor = Drawing.Color.Beige
                .CssClass = "dgAlternatingItemStyle"
            End With
            With .RowStyle
                .CssClass = "dgItemStyle"
                .HorizontalAlign = HorizontalAlign.Center
            End With
            With .HeaderStyle
                .BackColor = Drawing.ColorTranslator.FromHtml("#00699b")
                .CssClass = "dgHeaderStyle"
                .ForeColor = Drawing.ColorTranslator.FromHtml("#ffffff")
                .Font.Size = 8
            End With
        End With

        AddHandler gv.RowDataBound, AddressOf gv_RowDataBound

    End Sub

    Protected Sub gv_RowDataBound(source As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.Header Then

            If Session("SubType") = "Student" Then
                e.Row.Cells(0).Text = "Student ID"
                e.Row.Cells(1).Text = "Student Name"
                e.Row.Cells(2).Text = "College"
                e.Row.Cells(3).Text = "GL Source"
            ElseIf Session("SubType") = "Sponsor" Then
                e.Row.Cells(0).Text = "Sponsor ID"
                e.Row.Cells(1).Text = "Sponsor Name"
                e.Row.Cells(2).Text = "GL Source"
            End If

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then

            If Session("SubType") = "Student" Then
                If e.Row.Cells(2).Text = "&nbsp;" Then
                    e.Row.Cells(2).Text = "-1 - TIADA DATA"
                End If
            End If

        End If

    End Sub

    Private Sub UpdateGrid()
        Dim cScript As String = ""

        Session("List_Failed") = Nothing

        cScript = "<SCRIPT LANGUAGE=""JavaScript"">window.opener.document.getElementById('ctl00_ContentPlaceHolder1_btnHiddenApp').click();self.close();</SCRIPT>"
        ClientScript.RegisterStartupScript(Me.GetType(), "ClosePopUp", cScript)
    End Sub

    Protected Sub ibtnClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnClose.Click
        UpdateGrid()
    End Sub

End Class
