<%@ Page Language="VB" AutoEventWireup="false" CodeFile="GLFailedList.aspx.vb" MaintainScrollPositionOnPostback="true" Inherits="GLFailedList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title></title>
    <link href="style.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        //var textFieldInFocus;
        //function handleOnFocus(form_element) {
        //    textFieldInFocus = form_element;
        //}
        //function handleOnBlur() {
        //    textFieldInFocus = null;
        //}
    </script>
    <style type="text/css">
        .auto-style6 {
            height: 21px;
            width: 60%;
        }
        .auto-style7 {
            height: 30px;
            width: 20px;
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
    <asp:Panel ID="PnlApprover" runat="server" Height="50px" Width="400px">
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
        </table>
        <table style="width: 100%;">
             <tr>
                <td style="width: 6px; height: 21px">
                    <asp:Label ID="lblMsg" runat="server" CssClass="lblError" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style6">

                    <asp:GridView ID="gvUser" runat="server" AutoGenerateColumns="false" Width="80%" Style="vertical-align: text-top">
                        
                        <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                        <SelectedRowStyle CssClass="dgSelectedItemStyle" />
                        <AlternatingRowStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                            Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                        <RowStyle CssClass="dgItemStyle" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#00699b" CssClass="dgHeaderStyle" ForeColor="#ffffff" Font-Bold="False" Font-Italic="False"
                            Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" />
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="NAME" HeaderText="Name" />
                            <asp:BoundField DataField="SOURCE" HeaderText="Source" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>

        <table style="width: 100%; height: 100%">
            <tr>
                <td style="text-align: left" class="auto-style7">
                    <asp:ImageButton ID="ibtnClose" runat="server" Height="24px" ImageUrl="~/images/ok_cancel.jpg"
                        Width="24px" />
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
    </asp:Panel>
    </form>
</body>
</html>
