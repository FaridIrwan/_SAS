<%@ Page Language="VB" MasterPageFile="~/MasterPage3.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="DashboardHome.aspx.vb" Inherits="Default4" title="Welcome To SAS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script language="javascript" src="Scripts/popcalendar.js" type="text/javascript"></script>
    <script language="javascript" src="Scripts/functions.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function OpenWindow(URL) {
            var WindowName = "MyPopup";
            var Features = "location=no,toolbar=no,menubar=no,height =100,scrollbars=yes";
            window.open(URL, WindowName, Features);
        }
        </script>
    <%--<table border="0" cellpadding="0" cellspacing="0" width="85%">
        <tr>
            <td style="height: 43px">
                &nbsp;<asp:SiteMapPath ID="SiteMapPath1" runat="server">
                </asp:SiteMapPath>
                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
            </td>
        </tr>
        </table>
    <table class="mainbg" style="width: 100%;">
            <tr>
                <td style="width: 494px; height: 39px;">
                    <asp:SiteMapPath ID="SiteMapPath2" runat="server">
                    </asp:SiteMapPath>
                </td>
                <td class="pagetext" style="height: 39px" align="right">
                    <asp:Label ID="lblMenuName" runat="server" Text="Account WorkFlow Details" Width="" Style="margin-left: 0px"></asp:Label>
                </td>
            </tr>
        </table>--%>
    <asp:Panel ID="Panel1" runat="server" Width="100%">
        <table style="background-image: url(images/Sample.png);">
            <tr>
                <td style="width: 4px; height: 14px"></td>
                <td style="width: 14px; height: 14px"></td>                
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnView" runat="server" ImageUrl="~/images/find.png" ToolTip="View" Visible ="false"/>
                            </td>
                            <td>
                                <asp:Label ID="Label16" runat="server" Text="Search" Visible ="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnPrint" runat="server" ImageUrl="~/images/print.png" ToolTip="Print" Visible ="false"/>
                            </td>
                            <td>
                                <asp:Label ID="Label17" runat="server" Text="Print" Visible ="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnPosting" runat="server" ImageUrl="~/images/gposting.png"
                                    ToolTip="Cancel" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text="Posting" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnOthers" runat="server" ImageUrl="~/images/gothers.gif" ToolTip="Cancel" Visible="false"/>
                            </td>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="Others" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/images/cancel.png" ToolTip="Cancel" Visible="false"/>
                            </td>
                            <td>
                                <asp:Label ID="Label18" runat="server" Text="Cancel" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 3%; height: 14px"></td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnFirst" runat="server" ImageUrl="~/images/new_last.png" Visible="false"/>
                </td>
                <td style="height: 14px; width: 4px;"></td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnPrevs" runat="server" ImageUrl="~/images/new_prev.png" Visible="false"/>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:TextBox ID="txtRecNo" runat="server" Width="52px" AutoPostBack="True" Style="text-align: right"
                        MaxLength="7" ReadOnly="true" CssClass="text_box" disabled="disabled" TabIndex="1"
                        dir="ltr" Visible="false"></asp:TextBox>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="Label47" runat="server" Visible="false">Of</asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="lblCount" runat="server" Width="20px" Visible="false"></asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnNext" runat="server" ImageUrl="~/images/new_next.png" Visible="false"/>
                </td>
                <td style="height: 14px"></td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnLast" runat="server" ImageUrl="~/images/new_first.png" Visible="false"/>
                </td>
                <td style="width: 2%; height: 14px"></td>
                 <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnNew" runat="server" ImageUrl="images/add.png" ToolTip="New" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label11" runat="server" Text="New" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnSave" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label14" runat="server" Text="Save" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnDelete" runat="server" ImageUrl="~/images/delete.png" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label13" runat="server" Text="Delete" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 100%; height: 14px"></td>
                <td style="width: 100%; height: 14px"></td>
            </tr>
        </table>
        <table class="mainbg" style="width: 100%;">
            <tr>
                <td class="vline" style="width: 98%; height: 1px"></td>
            </tr>
        </table>
        <table class="mainbg" style="width: 100%;">
            <tr>
                <td style="width: 494px; height: 39px;">
                    <asp:SiteMapPath ID="SiteMapPath1" runat="server">
                    </asp:SiteMapPath>
                </td>
                <td class="pagetext" style="height: 39px" align="right">
                    <asp:Label ID="lblMenuName" runat="server" Text="Dashboard" Width="" Style="margin-left: 0px"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
        <%--<tr>

            <td>--%>
                <div align="center">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                                                    <td style="width: 100px" align="center">
                                                        <asp:Label ID="lblPending" runat="server" Text="Pending Approval" Width="150px" Font-Size="Medium"></asp:Label>
                                                        &nbsp;
                                                    </td>
                          <td style="height: 25px; width: 3px;">
                                </td>
                            <td style="width: 100px" align="center">
                                                        <asp:Label ID="lblDone" runat="server" Text="Approved" Width="150px" Font-Size="Medium"></asp:Label>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                        <tr>
                        
                                        <td style="width: 50%; vertical-align: top;">
                                            <asp:gridview ID="dgPending" runat="server" AutoGenerateColumns="False" Width="100%">
                                                <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                <SelectedRowStyle CssClass="dgSelectedItemStyle" />
                                <AlternatingRowStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                                    Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                <RowStyle CssClass="dgItemStyle" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#00699b" CssClass="dgHeaderStyle" ForeColor="#ffffff" Font-Bold="False" Font-Italic="False"
                                    Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                <Columns>
                                                    <asp:BoundField DataField="AccountsEn.BatchCode" HeaderText="Batch Code"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.BatchDate" HeaderText="Batch Date" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.Category" HeaderText="Trans Type"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.SubType" HeaderText="Subtype"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.TransactionAmount" HeaderText="Amount" DataFormatString="{0:F}"></asp:BoundField>
                                                </Columns>
                                            </asp:gridview>
                                        </td>
                          <td style="height: 25px; width: 3px;">
                                </td>
                                        <td style="width: 50%; vertical-align: top;">
                                            <asp:gridview ID="dgDone" runat="server" AutoGenerateColumns="False" Width="100%">
                                                <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                <SelectedRowStyle CssClass="dgSelectedItemStyle" />
                                <AlternatingRowStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                                    Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                <RowStyle CssClass="dgItemStyle" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#00699b" CssClass="dgHeaderStyle" ForeColor="#ffffff" Font-Bold="False" Font-Italic="False"
                                    Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                <Columns>
                                                    <asp:BoundField DataField="AccountsEn.BatchCode" HeaderText="Batch Code"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.BatchDate" HeaderText="Batch Date" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.Category" HeaderText="Trans Type"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.SubType" HeaderText="Subtype"></asp:BoundField>
                                                    <asp:BoundField DataField="AccountsEn.TransactionAmount" HeaderText="Amount" DataFormatString="{0:F}"></asp:BoundField>
                                                    <%--<asp:TemplateField HeaderText="Status" SortExpression="Status">
            <ItemTemplate>
                <asp:HyperLink ID="LbPath" runat="server" 
                    Text='<%# Eval("BatchCode")%>'
                    CommandName="PathUpdate" 
                    CommandArgument='<%#Bind("BatchCode")%>'>
                </asp:HyperLink>
            </ItemTemplate>--%>
        <%--</asp:TemplateField>--%>
                                                    <asp:TemplateField HeaderText="Status">
                                        <HeaderStyle HorizontalAlign="Center" Width="12%" />
                                        <ItemTemplate>
                                        <center>
                                            <asp:HyperLink ID="Status" runat="server" Text ='<%# Eval("BatchCode")%>'></asp:HyperLink>                                            
                                        </center>
                                            <%--<input type="hidden" id="Hidden_WorkFlowId" name="H_WId"  value='<%#Eval("WorkflowId")%>' runat="server" />
                                            <input type="hidden" id="Hidden_TransId" name="H_TId" value='<%#Eval("AccountsEn.BatchCode")%>' runat="server" />--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:gridview>
                                        </td>
                            </tr>
                       
                            
                    </table>

                </div>
          <%--  </td>
        </tr>--%>
        <tr>
            <td>
                &nbsp;</td>
             <td style="width: 178px; height: 26px">
                                    <asp:HiddenField ID="hidUserRole" runat="server" Visible="false" />
                                </td>
            <td style="width: 178px; height: 26px">
                                    <asp:HiddenField ID="hidUserId" runat="server" Visible="false" />
                                </td>
        </tr>
    <%--<asp:Button ID="btnHiddenApp" runat="Server" OnClick="btnHiddenApp_Click" Style="display: none" />--%>
</asp:Content>

