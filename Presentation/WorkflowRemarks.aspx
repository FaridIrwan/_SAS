<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WorkflowRemarks.aspx.vb" Inherits="WorkflowRemarks" MaintainScrollPositionOnPostback="true"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Worflow Remarks</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <table align ="center">
            <tr>
                <td style="width: 100px">
                    <asp:Label ID="Label1" runat="server" Text="BatchCode" Width="99px"></asp:Label></td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtBatchCode" runat="server" Width="500px" Readonly="true"></asp:TextBox></td>
                
            </tr>
            <tr>
                <td style="width: 100px">
                    <asp:Label ID="Label2" runat="server" Text="Remarks" Width="100px"></asp:Label></td>
                <td style="width: 100px">
                    <asp:TextBox ID="txtRemarks" runat="server" Width="500px" Readonly="true"></asp:TextBox></td>
                <td style="width: 100px">
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                </td>
                <td style="width: 100px">
                </td>
            </tr>
        </table>

          
        
        <br />
    
    </div>
    </form>
</body>
</html>
