<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GLFailedList.aspx.vb" MaintainScrollPositionOnPostback="true" Inherits="GLFailedList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title></title>
    <link href="style.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        //window.onbeforeunload = confirmExit;
        //function confirmExit() {
        //    return "You have attempted to leave this page.  If you have made any changes to the fields without clicking the Save button, your changes will be lost.  Are you sure you want to exit this page?";
        //}
    </script>
    <style type="text/css">
        .auto-style6 {
            height: 21px;
            width: 100%;
        }
        .auto-style7 {
            height: 20px;
            width: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <script language="javascript" type="text/javascript">
            function RefreshParent() {
                window.opener.document.getElementById('ctl00_ContentPlaceHolder1_btnHiddenApp').click();
                window.close();
            }
        </script>

    <h4 id="h4" runat="server">GL Failed List</h4>
    <asp:Panel ID="PnlApprover" runat="server" Width="800px">
        <table>
            <tr>
                <td style="width: 6px; height: 21px"></td>
                <td style="width: 3px; height: 21px">
                    <asp:Label ID="lblProcessName" runat="server" Font-Bold="False" Text="Process Name" Width="113px"></asp:Label>
                </td>
                <td style="width: 270px; height: 21px">
                    <asp:TextBox ID="txtProcessName" runat="server" Enabled="false"></asp:TextBox>
                </td>
                <td style="width: 100px; height: 21px"></td>
            </tr>

            <tr>
                <td style="width: 6px; height: 21px"></td>
                <td style="width: 3px; height: 21px">
                    <asp:Label ID="lblFailedTime" runat="server" Font-Bold="False" Text="Failed Time" Width="113px"></asp:Label>
                </td>
                <td style="width: 270px; height: 21px">
                    <asp:TextBox ID="txtTFailedTime" runat="server" Enabled="false"></asp:TextBox>
                </td>
                <td style="width: 100px; height: 21px"></td>
            </tr>
            <tr>
                <td colspan="4" style="height: 21px">
                    <asp:Label ID="lblMsg" runat="server" CssClass="lblError" Visible="false"></asp:Label>
                </td>
            </tr>
        </table>
                
        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        
        <table style="width: 100%; height: 100%; padding-top:20px">
            <tr>
                <td align="center" class="auto-style7">
                    <asp:ImageButton ID="ibtnClose" runat="server" Height="18px" ImageUrl="~/images/ok_cancel.jpg"
                        Width="18px" />
                </td>
                <td style="width: 158px; height: 30px; text-align: left">
                    <asp:Label ID="Label4" runat="server" Text="Close" Width="21px"></asp:Label>
                </td>
                <td style="width: 142px; height: 30px; text-align: left">
                </td>
                <td style="width: 100px; height: 30px; text-align: left">
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnPageName" runat="server" />
    </asp:Panel>
    </form>
</body>
</html>
