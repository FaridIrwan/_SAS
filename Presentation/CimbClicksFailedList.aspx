<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CimbClicksFailedList.aspx.vb" Inherits="CimbClicksFailedList" MaintainScrollPositionOnPostback="true"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body style="margin-top:0;margin-left:0;" alink="#00ff66">
    <form id="form1" runat="server">
        <script language="javascript" type="text/javascript">
            function RefreshParent() {
                window.opener.document.getElementById('ctl00_ContentPlaceHolder1_btnHiddenApp').click();
                window.close();
            }
        </script>
    <div style="text-align: left">
        <table>
            <tr>
                <td style="width: 70%;">
                     <asp:GridView ID="gvClicksFailed" runat="server" AutoGenerateColumns="false" Width="100%" Style="vertical-align: text-top">
                        <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                        <SelectedRowStyle CssClass="dgSelectedItemStyle" />
                        <AlternatingRowStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                            Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                        <RowStyle CssClass="dgItemStyle" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#00699b" CssClass="dgHeaderStyle" ForeColor="#ffffff" Font-Bold="False" Font-Italic="False"
                            Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" />
                         <Columns>
                            <asp:BoundField DataField="STUDENT_NAME" HeaderText="Student Name" ></asp:BoundField>
                            <asp:BoundField DataField="MATRIC_NO" HeaderText="Matric No" ></asp:BoundField>
                            <asp:BoundField DataField="IC_NO" HeaderText="Identity No" ></asp:BoundField>
                            <asp:BoundField DataField="RECEIPT_NO" HeaderText="Receipt No"></asp:BoundField>
                            <asp:BoundField DataField="RECEIPT_DATE" HeaderText="Receipt Date"></asp:BoundField>
                            <asp:BoundField DataField="PAID_AMOUNT" HeaderText="Paid Amount"></asp:BoundField>
                       </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 16px; text-align: center">
                   <table style="width: 100%; height: 100%">
                        <tr>
                            <td style="width: 34px; height: 30px; text-align: left">
                                <asp:ImageButton ID="ibtnClose" runat="server" Height="28px" ImageUrl="~/images/ok_cancel.jpg"
                                    Width="28px" /></td>
                            <td style="height: 30px; text-align: left">
                                <asp:Label ID="Label2" runat="server" Text="Close" Width="21px"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
       <br />
    </div>
   </form>
</body>
</html>
