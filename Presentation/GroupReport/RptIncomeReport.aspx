<%@ Page Language="VB" MasterPageFile="~/MasterPage3.master" AutoEventWireup="false" MaintainScrollPositionOnPostback="true"
    CodeFile="RptIncomeReport.aspx.vb" Inherits="RptIncomeReport" Title="Welcome To SAS" %>

<%@ Register Assembly="AtlasControlToolkit" Namespace="AtlasControlToolkit" TagPrefix="atlasToolkit" %>
<asp:content ID="Content2" contentplaceholderid="head" runat="server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>
    <script src="../Scripts/popcalendar.js" type="text/javascript"></script>
    <link href="../style.css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1
        {
            height: 16px;
        }
    </style>
</asp:content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        function OpenWindow(URL) {
            var WindowName = "MyPopup";
            var Features = "location=no,toolbar=no,menubar=no,height =600,scrollbars=yes";
            window.open(URL, WindowName, Features);
        }

        function refreshParentPage() {
            window.document.forms(0).submit();
        }
        function CheckToDate() {
            var digits = "0123456789/";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtToDate.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtToDate.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtToDate.ClientID%>").value = "";
                    document.getElementById("<%=txtToDate.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function getDateto() {
            popUpCalendar(document.getElementById("<%=ibtnTodate.ClientID%>"), document.getElementById("<%=txtToDate.ClientID%>"), 'dd/mm/yyyy')

        }
        function dateString() {
            var date, text = " "
            date = new Date()

            text += date.getDate() + "/"
            text += (date.getMonth() + 1) + "/"
            if (navigator.appName == "Netscape") {
                text += date.getYear() + 1900
            }
            if (navigator.appName == "Microsoft Internet Explorer") {
                text += date.getYear()
            }
            return (text)
        }

      
        function getDate() {
           

            
           

            dllValues1()

        }

        function dllValues1() {
            var fromdate1 = "0"
            var report = "0"
            var todate1 = "0"
            var lbl = false
            if ((document.getElementById("<%=ddltypeofreport.ClientID%>").value == "-1") ) {
                alert("Please select type of report")
                return false;
            }
            else if ((document.getElementById("<%=ddltypeofreport.ClientID%>").value == "1")) {
                if (document.getElementById("<%=rdrdate.ClientID%>").checked) {

                    if (document.getElementById("<%=txtFrom.ClientID%>").value.length == 0) {
                        alert("From Date Field Cannot Be Blank")
                        return false;
                    }
                    else if 
                        (document.getElementById("<%=txtTodate.ClientID%>").value.length == 0) {
                        alert("To Date Field Cannot Be Blank")
                        return false;
                    }
                    else {
                        
                        fromdate1 = document.getElementById("<%=txtFrom.ClientID%>").value
                        todate1 = document.getElementById("<%=txtTodate.ClientID%>").value
                    }

                    OpenPopup('../GroupReport/RptIncomeReportViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', '700', '650')
                    //new_window = window.open('../GroupReport/RptCollectionAnalysisViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();
                    return true;
                }
                else if (document.getElementById("<%=RadioButton2.ClientID%>").checked) {
                    if (document.getElementById("<%=txtyearfrom.ClientID%>").value.length == 0) {
                        alert("From Date Field Cannot Be Blank")
                        return false;
                    }
                    else if
                        (document.getElementById("<%=txtyearto.ClientID%>").value.length == 0) {
                        alert("To Date Field Cannot Be Blank")
                        return false;
                    }
                    else {
                        
                        fromdate1 = document.getElementById("<%=txtyearfrom.ClientID%>").value
                        todate1 = document.getElementById("<%=txtyearto.ClientID%>").value
                    }

                OpenPopup('../GroupReport/RptIncomeReportViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', '700', '650')
                    //new_window = window.open('../GroupReport/RptCollectionAnalysisViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();
                return true;
                }
                else {
                    alert("Please select date range or Year")

                    return false;
                }
            }
            else if ((document.getElementById("<%=ddltypeofreport.ClientID%>").value == "2")) {
                OpenPopup('../GroupReport/RptIncomeReportViewer.aspx', 'SAS', '700', '650')
                //new_window = window.open('../GroupReport/RptCollectionAnalysisViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();
                return true;
            }
            else if ((document.getElementById("<%=ddltypeofreport.ClientID%>").value == "3")) {
                if (document.getElementById("<%=txtFrom2.ClientID%>").value.length == 0) {
                    alert("From Date Field Cannot Be Blank")
                    return false;
                }
                else if
                        (document.getElementById("<%=txtTo.ClientID%>").value.length == 0) {
                        alert("To Date Field Cannot Be Blank")
                        return false;
                }
                else {
                    
                    fromdate1 = document.getElementById("<%=txtFrom2.ClientID%>").value
                    todate1 = document.getElementById("<%=txtTo.ClientID%>").value
                }
                OpenPopup('../GroupReport/RptIncomeReportViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', '700', '650')
                //new_window = window.open('../GroupReport/RptCollectionAnalysisViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();
                return true;
            }
            else if ((document.getElementById("<%=ddltypeofreport.ClientID%>").value == "4")) {
                if (document.getElementById("<%=txtFrom2.ClientID%>").value.length == 0) {
                    alert("From Date Field Cannot Be Blank")
                    return false;
                }
                else if 
                        (document.getElementById("<%=txtTo.ClientID%>").value.length == 0) {
                    alert("To Date Field Cannot Be Blank")
                    return false;
                }
                else if
                        (document.getElementById("<%=ddlmyra.ClientID%>").value == "-1") {
                    alert("Please select Type of Report MyRA")
                        return false;
                    }
                else {

                    fromdate1 = document.getElementById("<%=txtFrom2.ClientID%>").value
                    todate1 = document.getElementById("<%=txtTo.ClientID%>").value
                }
            OpenPopup('../GroupReport/RptIncomeReportViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', '700', '650')
                //new_window = window.open('../GroupReport/RptCollectionAnalysisViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();
            return true;
        }
        }

        function OpenPopup(pageURL, title, w, h) {
            var left = (screen.width - w) / 2;
            var top = (screen.height - h) / 4;  // for 25% - devide by 4  |  for 33% - devide by 3
            var targetWin = window.open(pageURL, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            targetWin.focus();
            return true;
        }

        function getimage2() {
            popUpCalendar(document.getElementById("<%=Image2.ClientID%>"), document.getElementById("<%=txtFrom2.ClientID%>"), 'dd/mm/yyyy')

        }

        function getimage4() {
            popUpCalendar(document.getElementById("<%=Image4.ClientID%>"), document.getElementById("<%=txtTo.ClientID%>"), 'dd/mm/yyyy')

        }

        function getibtnFDate() {
            popUpCalendar(document.getElementById("<%=ibtnFDate.ClientID%>"), document.getElementById("<%=txtFrom.ClientID%>"), 'dd/mm/yyyy').left

        }

        function getibtnTodate() {
            popUpCalendar(document.getElementById("<%=ibtnTodate.ClientID%>"), document.getElementById("<%=txtTodate.ClientID%>"), 'dd/mm/yyyy')

        }

        function getibtnyearfrom() {
            popUpCalendar(document.getElementById("<%=ibtnyearfrom.ClientID%>"), document.getElementById("<%=txtyearfrom.ClientID%>"), 'yyyy')

        }

        function getibtnyearto() {
            popUpCalendar(document.getElementById("<%=ibtnyearto.ClientID%>"), document.getElementById("<%=txtyearto.ClientID%>"), 'yyyy')

        }

    </script>

   
    <asp:Panel ID="Panel1" runat="server" Width="100%">
        <table style="background-image: url(../images/Sample.png);">
            <tr>
                <td style="width: 4px; height: 14px"></td>
                <td style="width: 14px; height: 14px"></td>

              
                 <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnPrint" runat="server" ImageUrl="~/images/print.png" ToolTip="Print"/>
                            </td>
                            <td>
                                <asp:Label ID="Label17" runat="server" Text="Print"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                 

                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <asp:ImageButton ID="ibtnCancel" runat="server" OnClick="ibtnCancel_Click" ImageUrl="~/images/cancel.png" ToolTip="Cancel" />
                            </td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label18" runat="server" Text="Cancel"></asp:Label>
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
                    <asp:SiteMapPath ID="SiteMapPath1" runat="server" Width="346px">
                    </asp:SiteMapPath>
                </td>
                <td class="pagetext" style="text-align: right">
                    <asp:Label ID="lblMenuName" runat="server" Width="273px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                    <asp:Label ID="lblMsg" runat="server" CssClass="lblError" Style="text-align: left" Width="301px"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <table width="100%">
        <tr>
            <td width="1%"></td>
            <td width="98%">
           <fieldset style="width: 100%; border: thin solid #A6D9F4;">
                    <legend><strong><span style="color: #000000;">Selection Criteria</span></strong></legend>
                    <table style="width: 100%" id ="Table1" visible="true">
                        
                        <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblreport" runat="server" Text="Type of Report"></asp:Label>
                            </td>
                             
                            <td class="auto-style1">
                                <asp:DropDownList ID="ddltypeofreport" runat="server" Width="400px" AutoPostBack="true" OnSelectedIndexChanged="ddltypeofreport_SelectedIndexChanged">
                                    <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                    <asp:ListItem Value="1">Income by Group FEE</asp:ListItem>
                                    <asp:ListItem Value="2">Income by Faculty</asp:ListItem>
                                    <asp:ListItem Value="3">Income MyMohe</asp:ListItem>
                                    <asp:ListItem Value="4">Income MyRA(post graduate)</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblfee" runat="server" Text="Type of Fee" Visible ="false"></asp:Label>
                            </td>
                            
                                                        
                                                   
                           <td style="width: 30%; vertical-align: top;">
                               <asp:CheckBox ID="chkSelectAllFee" runat="server" Text="Select All" AutoPostBack="True" Visible ="false" OnCheckedChanged="chkSelectAllFee_CheckedChanged" Checked="false"/>
                                            <asp:DataGrid ID="dgFee" runat="server" AutoGenerateColumns="False" Width="100%" Visible ="false">
                                                <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                                <SelectedItemStyle CssClass="dgSelectedItemStyle" />
                                                <AlternatingItemStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                                                    Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                                <ItemStyle CssClass="dgItemStyle" />
                                                <HeaderStyle BackColor="#CDD7EE" CssClass="dgHeaderStyle" Font-Bold="True" Font-Italic="False"
                                                    Font-Overline="False" Font-Size="Medium" Font-Strikeout="False" Font-Underline="False" />
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="Select">
                                                        <ItemTemplate>
                                                            &nbsp;<asp:CheckBox ID="chkFee" runat="server" OnCheckedChanged="chkFee_checkedchanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="FeeTypeCode" HeaderText="Fee Code"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Description" HeaderText="Fee Desc"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                        </tr>
                        <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblreportMyRA" runat="server" Text="Type of Report MyRA" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlmyra" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                OnSelectedIndexChanged="ddlmyra_SelectedIndexChanged" TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                    <asp:ListItem Value="1">Summary of Section F1 Gross Income From Post Graduate Fee</asp:ListItem>
                                    <asp:ListItem Value="2">Details of Section F1 Gross Income From Post Graduate Fee</asp:ListItem>
                                    <asp:ListItem Value="3">Summary Income Based on Program for Post Graduate Fee</asp:ListItem>
                                  
                                            </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblfaculty" runat="server" Text="Faculty" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlfaculty" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                OnSelectedIndexChanged="ddlfaculty_SelectedIndexChanged" TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                   <asp:ListItem Value="0">Select All</asp:ListItem>
                                            </asp:DropDownList>
                            </td>
                        </tr>
            <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblprogram" runat="server" Text="Program" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlprogram" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                OnSelectedIndexChanged="ddlprogram_SelectedIndexChanged" TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                   <asp:ListItem Value="0">Select All</asp:ListItem>
                                            </asp:DropDownList>
                            </td>
                        </tr>
            <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblsemfrom" runat="server" Text="Semester From" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlsemfrom" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                            </asp:DropDownList>
                            </td>
                        </tr>
            <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblsemto" runat="server" Text="Semester To" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlsemto" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                            </asp:DropDownList>
                            </td>
                        </tr>
                       
                       <%-- <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lblschdept" runat="server" Text="School/Department" Visible ="false"></asp:Label>
                            </td>

                           <%-- <td class="auto-style1">
                               <asp:DropDownList ID="ddlschdept" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                TabIndex="12" 
                                                Width="400px" Visible ="false">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                   
                                  
                                            </asp:DropDownList>
                            </td>
                        </tr>--%>
                      
                        <tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="lbllevel" runat="server" Text="Level" Visible ="false"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:TextBox ID="txtlevel" runat="server" AutoPostBack="True" Visible ="False" Width ="400px" OnTextChanged="txtlevel_TextChanged"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="height: 16px">
                                <asp:Label ID="rdrdaterange" runat="server" Text ="Date Range" Visible ="False" />
                            </td>
                             
                                <td style="width: 1px; height: 16px">
                                    <asp:Label ID="lblfrom" runat="server" Text="From" Visible ="False"></asp:Label>
                                    
                                    <asp:TextBox ID="txtFrom2" runat="server" AutoPostBack="True" Visible ="False" Width ="100px"></asp:TextBox>
                                    <asp:Image ID="Image2" runat="server" ImageUrl="../images/cal.gif" Visible ="False" />
                                    <asp:Label ID="lblto" runat="server" Text="To" Visible ="False"></asp:Label>
                                     <asp:TextBox ID="txtTo" runat="server" AutoPostBack="True" Visible ="False"></asp:TextBox>
                                    <asp:Image ID="Image4" runat="server" ImageUrl="../images/cal.gif" Visible ="False"/>
                                    
                                </td>
                                <td style="height: 16px">
                                    
                                </td>
                                <td style="width: 10px; height: 16px">
                                    
                                </td>
                                <td style="width: 9px; height: 16px">
                                    
                                </td>
                                <td style="width: 63px; height: 16px">
                                   
                                </td>
                                <td style="width: 1px; height: 16px">
                                    
                                </td>

                        </tr>
                        </table>
             </fieldset>
                </td>
            </tr>
        </table>
    <asp:Panel ID="pnlIncomegroupfee" runat="server" Width="100%" Visible="false">
    <table width="100%">
        <tr>
            <td width="1%"></td>
            <td width="98%">
                <fieldset style="width: 100%; border: thin solid #A6D9F4;">
                   
                 
          <table style="width: 100%" id ="dateddl1" visible="false">
               <%--<tr>
                            <td style="width: 10%; height: 16px">
                                <asp:Label ID="Label9" runat="server" Text="Type of Fee"></asp:Label>
                            </td>

                            <td class="auto-style1">
                               <asp:DropDownList ID="ddlFeeList" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" 
                                                OnSelectedIndexChanged="ddlFeeList_SelectedIndexChanged" TabIndex="12" 
                                                Width="400px">
                                                <asp:ListItem Value="-1">-- Select --</asp:ListItem>
                                            </asp:DropDownList>
                            </td>
                        </tr>--%>
                        <tr>
                           
                                <td style="height: 16px">
                                <asp:RadioButton ID="rdrdate" runat="server" Text ="Date Range" OnCheckedChanged="rdrdate_checkedchanged" checked ="false" AutoPostBack="True" />
                            </td>
                             
                                <td style="width: 1px; height: 16px">
                                    <asp:Label ID="lblfrom2" runat="server" Text="From"></asp:Label>
                                </td>
                                <td style="height: 16px">
                                    <asp:TextBox ID="txtFrom" runat="server" AutoPostBack="True" ></asp:TextBox>
                                </td>
                                <td style="width: 10px; height: 16px">
                                    <asp:Image ID="ibtnFDate" runat="server" ImageUrl="../images/cal.gif" />
                                </td>
                                <td style="width: 9px; height: 16px">
                                    <asp:Label ID="Label2" runat="server" Text="To"></asp:Label>
                                </td>
                                <td style="width: 63px; height: 16px">
                                    <asp:TextBox ID="txtTodate" runat="server" AutoPostBack="True"></asp:TextBox>
                                </td>
                                <td style="width: 1px; height: 16px">
                                    <asp:Image ID="ibtnTodate" runat="server" ImageUrl="../images/cal.gif" />
                                </td>
                                <td style="width: 1px; height: 16px">
                                    <asp:RadioButton ID="RadioButton2" runat="server" Text ="Year" Width="116px" OnCheckedChanged="rdryear_checkedchanged" checked ="false" AutoPostBack="True"/>
                                </td>
                                <td style="width: 1px; height: 16px">
                                    <asp:Label ID="Label8" runat="server" Text="From" ></asp:Label>
                                </td>
                                
                            <td style="height: 16px">
                                    <asp:TextBox ID="txtyearfrom" runat="server" AutoPostBack="True"></asp:TextBox>
                                </td>
                                <td style="width: 10px; height: 16px">
                                    <asp:Image ID="ibtnyearfrom" runat="server" ImageUrl="../images/cal.gif" />
                                </td>
                                <td style="width: 9px; height: 16px">
                                    <asp:Label ID="Label11" runat="server" Text="To"></asp:Label>
                                </td>
                                <td style="width: 63px; height: 16px">
                                    <asp:TextBox ID="txtyearto" runat="server" AutoPostBack="True"></asp:TextBox>
                                </td>
                            <td style="width: 10px; height: 16px">
                                    <asp:Image ID="ibtnyearto" runat="server" ImageUrl="../images/cal.gif" />
                                </td>
                           
                            
                          
                    </tr>
              <tr>
                   <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkReset" runat="server" Text ="Reset" Width="116px" OnCheckedChanged="chkReset_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
              </tr>
                    </table>
                </fieldset>
                 <fieldset style="width: 100%; border: thin solid #A6D9F4;">
                    <legend><strong><span style="color: #000000;">Sort By</span></strong></legend>
                     
                    <table style="width: 100%" id ="sortddl1" visible="false">
                       <%-- <tr>
                     <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkallcategory" runat="server" Text ="chkallcategory" Width="116px" checked ="false" AutoPostBack="True" Visible ="false"/>
                           </td>
                     </tr>--%>
                        <tr>
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkcategory" runat="server" Text ="Category" Width="116px" OnCheckedChanged="chkcategory_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chksponsor" runat="server" Text ="Sponsor" Width="116px" OnCheckedChanged="chksponsor_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                        </tr>
                        <tr>
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkfaculty" runat="server" Text ="Faculty" Width="116px" OnCheckedChanged="chkfaculty_checkedchanged" checked ="false" AutoPostBack="True" />
                           </td>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkstudent" runat="server" Text ="Student" Width="116px" OnCheckedChanged="chkstudent_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                        </tr>
          <tr>
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkprogram" runat="server" Text ="Program" Width="116px" OnCheckedChanged="chkprogram_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkstatus" runat="server" Text ="Student Status" Width="116px" OnCheckedChanged="chkstatus_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                           <td style="width: 56px; height: 14px">
                                            <asp:DropDownList ID="ddlstudentStatus" runat="server" Width="153px" AppendDataBoundItems="True"
                                                TabIndex="2" Visible ="false">
                                                <asp:ListItem Value="-1">--Select--</asp:ListItem>
                                            </asp:DropDownList>
                                            </td>
                        </tr>
                        </table>
                </fieldset>
            </td>
            <td style="width: 113px; height: 16px"></td>
        </tr>
    </table>
</asp:Panel>
    <asp:Panel ID="pnlincomemymohe" runat="server" Width="100%" Visible="false">
        <table width="100%">
        <tr>
            <td width="1%"></td>
            <td width="98%">
                <fieldset style="width: 100%; border: thin solid #A6D9F4;">
        <table style="width: 100%" id ="Table2">
           
            <tr>
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkinvoiceno" runat="server" Text ="Invoice No" Width="116px" OnCheckedChanged="chkinvoiceno_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkinvoicedate" runat="server" Text ="Invoice Date" Width="116px" OnCheckedChanged="chkinvoicedate_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                      
                       
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkinvoiceamt" runat="server" Text ="Invoice Amount" Width="116px" OnCheckedChanged="chkinvoiceamt_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkDetails" runat="server" Text ="Details" Width="116px" OnCheckedChanged="chkDetails_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                        </tr>
            </table>
                    </fieldset>
                 <fieldset style="width: 100%; border: thin solid #A6D9F4;">
                    <legend><strong><span style="color: #000000;">Sort By</span></strong></legend>
                    <table style="width: 100%" id ="Table3">
                        <tr>
                           <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkmatricno" runat="server" Text ="Student Matric ID" Width="116px" OnCheckedChanged="chkmatricno_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                            </tr>
                        <tr>
                            <td style="width: 10px; height: 16px">
                               <asp:CheckBox ID="chkinvoicenoo" runat="server" Text ="Invoice No" Width="116px" OnCheckedChanged="chkinvoicenoo_checkedchanged" checked ="false" AutoPostBack="True"/>
                           </td>
                        </tr>
                       
                        </table>
                </fieldset>
            </td>
            <td style="width: 113px; height: 16px"></td>
        </tr>
                </table>

        </asp:Panel>
     <%--<asp:Panel ID="pnlIncomeMyMohe" runat="server" Width="100%" Visible="false">
        <table width="100%">
        <tr>
            <td width="1%"></td>
            <td width="98%">
                <fieldset style="width: 100%; border: thin solid #A6D9F4;">
        <table style="width: 100%" id ="Table3" visible="false">

            </table>
                    </fieldset>
            </td>
            <td style="width: 113px; height: 16px"></td>
        </tr>
                </table>

        </asp:Panel>--%>
</asp:Content>
