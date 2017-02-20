<%@ Page Language="VB" Title="" MasterPageFile="~/MasterPage3.master" AutoEventWireup="false" CodeFile="AFCDetails.aspx.vb" Inherits="AFCDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:Label ID="lblMsg" runat="server" CssClass="lblError" Style="text-align: center"
                    Width="348px"></asp:Label>
            </td>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td class="pagetext" style="text-align: left">
                <asp:Label ID="lbl1" runat="server" Text="AFC Fee Details"></asp:Label>
                </td>
            <td>
                <br />
            </td>
        </tr> 
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
                                                        
                                                    </td>
                                                    <td style="width: 225px; height: 25px">
                                                        <asp:Label ID="Label3" runat="server" Text="Batch Date" Width="61px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px">
                                                        <asp:TextBox ID="txtBatchDate" runat="server" MaxLength="10" Width="142px"></asp:TextBox>
                                                    </td>
                                                    
                                                    <td style="width: 133px; height: 25px"></td>
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
                                                     
                                                </tr>
                                                 <tr>
                                                    <td style="height: 25px" valign="top">
                                                       
                                                    </td>
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="lblpaidamt" runat="server" Text="Total Paid" Width="64px"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: top; width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px; vertical-align: top;">
                                                        <asp:TextBox ID="txttotalpaid" runat="server" Width="142px" ReadOnly ="true"
                                                            ></asp:TextBox>
                                                    </td>
                                                    
                                                    <td style="width: 225px; height: 25px; vertical-align: top;">
                                                        <asp:Label ID="lblstatuss" runat="server" Text="Status" Width="64px"></asp:Label>
                                                    </td>
                                                     <td style="width: 106px; height: 25px; vertical-align: top;">
                                                        <asp:TextBox ID="txtstatuss" runat="server" Width="142px" ReadOnly ="true"
                                                            ></asp:TextBox>
                                                    </td>
                                                    
                                                </tr>
                                                <tr>
                                                    <td style="height: 25px">
                                                        
                                                    </td>
                                                    <td style="width: 225px; height: 25px">
                                                        <asp:Label ID="lbltotalamt" runat="server" Text="Total Amount" Width="61px"></asp:Label>
                                                    </td>
                                                    <td style="width: 91px; height: 25px"></td>
                                                    <td style="width: 106px; height: 25px">
                                                        <asp:TextBox ID="txtAmount" runat="server" MaxLength="10" Width="142px"></asp:TextBox>
                                                    </td>
                                                    
                                                    <td style="width: 133px; height: 25px"></td>
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
                </asp:Panel>
                            </div>
                            </td>
        </tr>       
        <tr>
            <td style="width: 71%; height: 149px; vertical-align: top;">
                <fieldset>
                    <asp:DataGrid ID="dgFeeType" runat="server" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="25"
                        Height="123px" Style="vertical-align: text-top">
                        <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                        <PagerStyle Mode="NumericPages" HorizontalAlign="Center" />
                        <SelectedItemStyle CssClass="dgSelectedItemStyle" />
                        <AlternatingItemStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                            Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                        <ItemStyle CssClass="dgItemStyle" />
                        <HeaderStyle BackColor="#CDD7EE" CssClass="dgHeaderStyle" Font-Bold="True" Font-Italic="False"
                            Font-Overline="False" Font-Size="Medium" Font-Strikeout="False" Font-Underline="False" />
                        <Columns>
                            <asp:BoundColumn DataField="SAFT_Desc" HeaderText="Fee Name"></asp:BoundColumn>
                            <asp:BoundColumn DataField="SAFT_Code" HeaderText="Fee Code"></asp:BoundColumn>
                            <asp:BoundColumn DataField="SAFD_Type" HeaderText="Fee Type"></asp:BoundColumn>   
                            <asp:BoundColumn DataField="SAFA_Amount" HeaderText="Fee Amount" DataFormatString="{0:F}"></asp:BoundColumn>
                            <%--<asp:BoundColumn DataField="SAST_Code" HeaderText="Semester Code" Visible="false"></asp:BoundColumn>--%>
                            <%--<asp:BoundColumn DataField="SAFD_Sem" HeaderText="Semester" Visible="false"></asp:BoundColumn>--%>                            
                        </Columns>
                    </asp:DataGrid>
                </fieldset>
            </td>
            <td style="width: 104%; height: 149px; vertical-align: top;">&nbsp;</td>
        </tr>
    </table>
</asp:Content>
