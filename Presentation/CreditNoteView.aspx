<%@ Page Language="VB" MasterPageFile="~/MasterPage3.master" AutoEventWireup="false"
    CodeFile="CreditNoteView.aspx.vb" Inherits="CreditNoteView" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" src="Scripts/popcalendar.js" type="text/javascript"></script>
    <script language="javascript" src="Scripts/functions.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function checkValue() {

            if (((event.keyCode < 48) || (event.keyCode > 57)) && (event.keyCode != 13) && (event.keyCode != 46)) {
                alert("Enter Valid Amount");
                event.preventDefault();
                return false;
            }
        }
    </script>
    <table>
        <tr>
            <td style="width: 98px; height: 1px">
                <table>
                    <tr>
                        <td></td>
                        <td colspan="3" style="height: 12px" align="left">
                            <asp:Label ID="lblMsg" runat="server" CssClass="lblError" Style="text-align: center"
                                Width="444px"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <ul class="ext" >
                                <li class="ext">
                                    <asp:Button ID="btnBatchInvoice" runat="server" Height="24px" Text="Invoice" Width="108px" CssClass="TabButton" />
                                </li>

                               
                            </ul>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:HiddenField ID="lblStatus" runat="server" />
    <asp:HiddenField ID="hfValidateAmt"  runat="server"/>
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%">
                        <div style="border: thin solid #A6D9F4; width: 100%">
                            <asp:Panel ID="pnlBatch" runat="server" Height="100%" Width="100%">
                                <table>
                                    <tr>
                                        <td style="width: 60%">
                                            <table style="width: 40%">
                                                <tr>
                                                    <td style="height: 23px">
                                                        <span style="color: #ff0000">*</span>
                                                    </td>
                                                    <td style="width: 225px; height: 23px">
                                                        <asp:Label ID="Label1" runat="server" Text="Batch No" Width="59px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 23px"></td>
                                                    <td colspan="3" style="height: 23px">
                                                        <asp:TextBox ID="txtBatchNo" runat="server" Width="142px"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 81px; height: 23px">
                                                        <span style="color: #ff0000"></span>
                                                    </td>
                                                    <td style="width: 133px; height: 23px"></td>
                                                    <td style="width: 100px;" rowspan="6">&nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px">
                                                        <%--<span style="color: #ff0000">*</span>--%>
                                                    </td>
                                                    <td style="width: 225px; height: 25px">
                                                        <asp:Label ID="lblBatchIntake" runat="server" Text="Batch Intake" Width="59px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 25px"></td>
                                                    <td style="height: 25px">
                                                        <asp:DropDownList ID="ddlIntake" runat="server" AppendDataBoundItems="true">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td></td>
                                                    <td rowspan="4" valign="top">
                                                        <asp:ImageButton ID="ibtnStatus" runat="server" CssClass="cursor" Enabled="False"
                                                            ImageUrl="~/images/NotReady.gif" />
                                                    </td>
                                                    <td>
                                                        <asp:HiddenField ID="lblModule" runat="server" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px">
                                                        <span style="color: #ff0000">*</span>
                                                    </td>
                                                    <td style="width: 225px; height: 25px">
                                                        <asp:Label ID="Label3" runat="server" Text="Batch Date" Width="61px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px">
                                                        <asp:TextBox ID="txtBatchDate" runat="server" MaxLength="10" Width="73px"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 88px; height: 25px">
                                                        <asp:Image ID="ibtnBDate" runat="server" ImageUrl="~/images/cal.gif" />
                                                    </td>
                                                    <td style="width: 81px; height: 25px">
                                                        <asp:HiddenField ID="today" runat="server" />
                                                    </td>
                                                    <td style="width: 133px; height: 25px"></td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px">
                                                        <span style="color: #ff0000">*</span>
                                                    </td>
                                                    <td style="width: 225px; height: 25px">
                                                        <asp:Label ID="Label7" runat="server" Text="Invoice Date" Width="64px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px">
                                                        <asp:TextBox ID="txtInvoiceDate" runat="server" Width="73px" MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td style="width: 88px; height: 25px">
                                                        <asp:Image ID="ibtnInDate" runat="server" ImageUrl="~/images/cal.gif" />
                                                    </td>
                                                    <td style="width: 81px; height: 25px"></td>
                                                    <td style="width: 133px; text-align: right;" rowspan="3">
                                                        <asp:Label ID="todate" runat="server" Text="                                         "
                                                            Width="64px" Visible="False"></asp:Label>&nbsp;&nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px" valign="top">
                                                        <span style="color: #ff0000">*</span>
                                                    </td>
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="Label9" runat="server" Text="Due Date" Width="64px"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: top; width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px; vertical-align: top;">
                                                        <asp:TextBox ID="txtDuedate" runat="server" onBlur="checkdate(this)" Width="73px"
                                                            MaxLength="10"></asp:TextBox>
                                                    </td>
                                                    <td style="vertical-align: top; width: 88px; height: 25px">
                                                        <asp:Image ID="ibtnDueDate" runat="server" ImageUrl="~/images/cal.gif" />
                                                    </td>
                                                    <td style="width: 81px; height: 25px">
                                                        <asp:Label ID="Label25" runat="server" Text="welcome to sas invoice screen" Visible="False"
                                                            Width="64px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px" valign="top">
                                                        <%-- <span style="color: #ff0000">*</span>--%>
                                                    </td>
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="Label23" runat="server" Text="Description " Width="69px"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: top; width: 91px; height: 25px">&nbsp;
                                                    </td>
                                                    <td style="height: 25px; vertical-align: top;" colspan="4">
                                                        <asp:TextBox ID="txtDesc" runat="server" Height="20px" MaxLength="50"
                                                            Width="300px"></asp:TextBox>
                                                    </td>
                                                     <td style="height: 25px; vertical-align: top;" colspan="4">
                                                        <asp:TextBox ID="txtMode" runat="server" Height="20px" MaxLength="50"
                                                            Width="300px" Visible ="false"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                 <tr>
                                                    <td style="height: 25px" valign="top">
                                                       
                                                    </td>
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="lblpaidamt" runat="server" Text="Total Paid" Width="64px"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: top; width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px; vertical-align: top;">
                                                        <asp:TextBox ID="txttotalpaid" runat="server" Width="73px" ReadOnly ="true"
                                                            ></asp:TextBox>
                                                    </td>
                                                    
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="lblstatuss" runat="server" Text="Status" Width="64px"></asp:Label>
                                                    </td>
                                                     <td style="width: 106px; height: 25px; vertical-align: top;">
                                                        <asp:TextBox ID="txtstatuss" runat="server" Width="73px" ReadOnly ="true"
                                                            ></asp:TextBox>
                                                    </td>
                                                    
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>&nbsp;
                                                    </td>
                                                    <td style="width: 81px">&nbsp;
                                                    </td>
                                                    <td style="width: 175px">&nbsp;</td>
                                                    <td style="width: 497px"></td>
                                                    <td style="width: 497px">&nbsp;
                                                    </td>
                                                    <td style="width: 100px"></td>
                                                    <td style="width: 100px">&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                           
                                        </td>
                                    </tr>
                                </table>                             
                                <asp:Panel ID="pnlDgView" Width="100%" runat="server" Visible="true">
                                    <table style="width: 100%">
                                        <tr>
                                            <td width="100%">
                                                <asp:CheckBox ID="chkSelectedView" runat="server" 
                                                    AutoPostBack="True" Text="Select All" Visible="false"/>
                                                <asp:DataGrid ID="dgView" runat="server" AutoGenerateColumns="False" Width="100%"
                                                    ShowFooter="True" DataKeyField="ReferenceCode"
                                                    Style="vertical-align: top" >
                                                    <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                                    <SelectedItemStyle CssClass="dgSelectedItemStyle" />
                                                    <AlternatingItemStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                                                        Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                                    <ItemStyle CssClass="dgItemStyle" />
                                                    <HeaderStyle BackColor="#CDD7EE" CssClass="dgHeaderStyle" Font-Bold="True" Font-Italic="False"
                                                        Font-Overline="False" Font-Size="Medium" Font-Strikeout="False" Font-Underline="False" />
                                                    <Columns>
                                                        <asp:TemplateColumn HeaderText="Select" Visible="false">
                                                            <ItemTemplate>
                                                                &nbsp;<asp:CheckBox ID="chkView" runat="server" AutoPostBack="true"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn HeaderText="Matric No" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Name" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="ReferenceCode" HeaderText="Fee Code"
                                                           ></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Description" HeaderText="Fee Desc"></asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Fee Amount">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtFeeAmt" runat="server" Width="98px" AutoPostBack="True" Text='<%# GSTFeeAmt(Eval("TransactionAmount"), Eval("GSTAmount"), Eval("TaxId"))%>' Style="text-align: right" Height="18px" MaxLength="10"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <HeaderStyle Width="15%" />
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn DataField="TransactionAmount" HeaderText="FeeAmount" Visible="False">
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Fee Code" Visible="False"></asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Actual Fee Amount" FooterText="Total">
                                                            <ItemTemplate>
                                                                <%# GSTActAmt(Eval("TransactionAmount"), Eval("GSTAmount"), Eval("TaxId"))%>
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn DataField="GSTAmount" HeaderText="GST Amount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:F}">
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Total Fee Amount">
                                                            <ItemTemplate>
                                                                <%# Eval("TransactionAmount", "{0:F}")%>
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn DataField="Priority" HeaderText="Priority" Visible="false">
                                                            <HeaderStyle Width="15%" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="TaxId" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="TransactionID" Visible="false"></asp:BoundColumn>
                                                    </Columns>
                                                </asp:DataGrid>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 100%; text-align: right;">
                                        <tr>
                                            <td style="width: 76%">
                                                <asp:Label ID="lblTotal" runat="server" Text="Total Amount" Width="65px" Visible ="false"></asp:Label>
                                            </td>
                                            <td style="text-align: left">
                                                <asp:TextBox ID="txtTotal" runat="server" Width="106px"  Style="text-align: right"  Enabled="false" Visible ="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                  

                            </asp:Panel>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:View>
        
    </asp:MultiView>
    <asp:Button ID="btnHidden" runat="Server" OnClick="btnHidden_Click" Style="display: none" />

    <asp:HiddenField ID="MenuId" runat="server" />
    <asp:Button ID="btnHiddenApp" runat="Server" OnClick="btnHiddenApp_Click" Style="display: none" />
</asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="head">
    <style type="text/css">
        ul.ext {
            list-style-type: none;
            margin: 0;
            padding: 0;
            overflow: hidden;
            background-color: transparent;
        }

        li.ext {
            float: left;
            padding-right:15px;
        }

        li.ext a, .dropbtn {
            display: inline-block;
            color: white;
            text-align: center;
            padding: 14px 16px;
            text-decoration: none;
        }

        li.ext a:hover, .dropdown:hover .dropbtn {
            background-color: red;
        }

        li.dropdown {
            display: inline-block;
        }

        .dropdown-content {
            display: none;
            position: absolute;
            background-color: #f9f9f9;
            min-width: 160px;
            box-shadow: 0px 8px 16px 0px rgba(0,0,0,0.2);
        }

        .dropdown-content a {
            color: black;
            padding: 12px 16px;
            text-decoration: none;
            display: block;
            text-align: left;
        }

        .dropdown-content a:hover {background-color: #f1f1f1}

        .dropdown:hover .dropdown-content {
            display: block;
        }
    </style>
</asp:Content>