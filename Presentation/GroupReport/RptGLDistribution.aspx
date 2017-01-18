<%@ Page Language="C#" MasterPageFile="~/MasterPage3.master" AutoEventWireup="true" CodeFile="RptGLDistribution.aspx.cs" Inherits="RptGLDistribution" Title="Welcome To SAS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../Scripts/functions.js" type="text/javascript"></script>
    <script src="../Scripts/popcalendar.js" type="text/javascript"></script>
    <link href="../style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script language="javascript" type="text/javascript">
        function dateString() {
            var date, text = " "
            date = new Date()

            text += (date.getMonth() + 1) + "/"
            text += date.getDate() + "/"
            if (navigator.appName == "Netscape") {
                text += date.getYear() + 1900
            }
            if (navigator.appName == "Microsoft Internet Explorer") {
                text += date.getYear()
            }
            return (text)
        }

        function CheckFromDate() {
            var digits = "0123456789/";
             var temp;
            for (var i = 0; i < document.getElementById("<%=txtDteFrom.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtDteFrom.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtDteFrom.ClientID%>").value = "";
                    document.getElementById("<%=txtDteFrom.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }

        function CheckToDate() {
            var digits = "0123456789/";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtDteTo%>").value.length; i++) {
                temp = document.getElementById("<%=txtDteTo.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtDteTo.ClientID%>").value = "";
                    document.getElementById("<%=txtDteTo.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }

        function getDteFrom() {
            popUpCalendar(document.getElementById("<%=imgDteFrom.ClientID%>"), document.getElementById("<%=txtDteFrom.ClientID%>"), 'dd/mm/yyyy')

        }

        function getDteTo() {
            popUpCalendar(document.getElementById("<%=imgDteTo.ClientID%>"), document.getElementById("<%=txtDteTo.ClientID%>"), 'dd/mm/yyyy')

        }

        function getDate() {
            var curenttempdate = dateString()
            var fromdate = new Date(document.getElementById("<%=txtDteFrom.ClientID%>").value)
            var todate = new Date(document.getElementById("<%=txtDteTo.ClientID%>").value)
            var currentdate = new Date(curenttempdate)

            if (document.getElementById("<%=txtDteFrom.ClientID%>").value.length == 0 || document.getElementById("<%=txtDteTo.ClientID%>").value.length == 0) {
                alert("Date Field Cannot Be Blank")
                return false;
            }


            //Checking correct date format
            var len = document.getElementById("<%=txtDteFrom.ClientID%>").value
            var RegExPattern = /^((((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))|(((0[1-9]|[12]\d|3[01])(0[13578]|1[02])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|[12]\d|30)(0[13456789]|1[012])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|1\d|2[0-8])02((1[6-9]|[2-9]\d)?\d{2}))|(2902((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00))))$/;
            var errorMessage = 'Enter Valid Date (dd/mm/yyyy)';


            if (document.getElementById("<%=txtDteFrom.ClientID%>").value.match(RegExPattern)) {
                if (len.length == 8) {
                    alert(errorMessage);
                    document.getElementById("<%=txtDteFrom.ClientID%>").value = "";
                    document.getElementById("<%=txtDteFrom.ClientID%>").focus();
                    return false;
                }
            }
            else {
                alert(errorMessage);
                document.getElementById("<%=txtDteFrom.ClientID%>").value = "";
                document.getElementById("<%=txtDteFrom.ClientID%>").focus();
                return false;
            }

            if (document.getElementById("<%=txtDteTo.ClientID%>").value.match(RegExPattern)) {

                if (len.length == 8) {
                    alert(errorMessage);
                    document.getElementById("<%=txtDteTo.ClientID%>").value = "";
                    document.getElementById("<%=txtDteTo.ClientID%>").focus();
                    return false;
                }
            }
            else {
                alert(errorMessage);
                document.getElementById("<%=txtDteTo.ClientID%>").value = "";
                document.getElementById("<%=txtDteTo.ClientID%>").focus();
                return false;
            }

            //Checking date between from and to
            var str1 = document.getElementById("<%=txtDteFrom.ClientID %>").value;
            var str2 = document.getElementById("<%=txtDteTo.ClientID %>").value;
            var dt1 = parseInt(str1.substring(0, 2), 10);
            var mon1 = parseInt(str1.substring(3, 5), 10);
            var yr1 = parseInt(str1.substring(6, 10), 10);
            var dt2 = parseInt(str2.substring(0, 2), 10);
            var mon2 = parseInt(str2.substring(3, 5), 10);
            var yr2 = parseInt(str2.substring(6, 10), 10);
            var date1 = new Date(yr1, mon1, dt1);
            var date2 = new Date(yr2, mon2, dt2);

            if (document.getElementById("<%=txtDteFrom.ClientID%>").value.length != 10) {
                alert("Enter Valid Date (dd/mm/yyyy)")
                document.getElementById("<%=txtDteFrom.ClientID%>").focus();
                return false
            }

            if (document.getElementById("<%=txtDteTo.ClientID%>").value.length != 10) {
                alert("Enter Valid Date (dd/mm/yyyy)")
                document.getElementById("<%=txtDteTo.ClientID%>").focus()
                return false
            }

            if (date2 < date1) {
                alert("From Date Cannot Be Greater Than To Date");
                return false;
            }

            else {
                dllValues()
            }
        }

        function dllValues() {
            var fromdate1 = "0"
            var todate1 = "0"

            fromdate1 = document.getElementById("<%=txtDteFrom.ClientID%>").value
            todate1 = document.getElementById("<%=txtDteTo.ClientID%>").value

            new_window = window.open('../GroupReport/RptGLDistributionViewer.aspx?fdate=' + fromdate1 + '&tdate=' + todate1, 'SAS', 'width=700,height=500,resizable=1'); new_window.focus();

            return true;
        }
    </script>

    <asp:Panel ID="Panel1" runat="server" Width="100%">
        <table style="background-image: url(../images/Sample.png);">
            <tr>
                <td style="width: 4px; height: 14px"></td>
                <td style="width: 14px; height: 14px"></td>

                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="height: 14px">
                                <asp:ImageButton ID="ibtnNew" runat="server" ImageUrl="~/images/add.png" ToolTip="New" /></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label11" runat="server" Text="New"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <asp:ImageButton ID="ibtnSave" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" Visible="False"/></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label14" runat="server" Text="Save" Visible="False"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="height: 14px; width: 3%;">
                                <asp:ImageButton ID="ibtnDelete" runat="server" ImageUrl="~/images/delete.png" Visible="False"/></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label13" runat="server" Text="Delete" Visible="False"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <asp:ImageButton ID="ibtnView" runat="server" ImageUrl="~/images/find.png" ToolTip="View" Visible="False"/></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label16" runat="server" Text="Search" Visible="False"></asp:Label></td>
                        </tr>
                    </table>
                </td>
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
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 2%; height: 14px">
                                <asp:ImageButton ID="ibtnPosting" runat="server" ImageUrl="~/images/gposting.png"
                                    ToolTip="Cancel" Visible="false"/></td>
                            <td style="width: 2%; height: 14px">&nbsp;<asp:Label ID="Label6" runat="server" Text="Posting" Visible="false"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                 
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <asp:ImageButton ID="ibtnOthers" runat="server" ImageUrl="~/images/gothers.png"
                                    ToolTip="Cancel" Visible="false"/></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label5" runat="server" Text="Others" Visible="false"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                <td style="float: left">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';" onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/images/cancel.png"
                                    ToolTip="Cancel" OnClick="ibtnCancel_Click" /></td>
                            <td style="width: 3%; height: 14px">
                                <asp:Label ID="Label18" runat="server" Text="Cancel"></asp:Label></td>
                        </tr>
                    </table>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnFirst" runat="server" ImageUrl="~/images/new_last.png" Visible="false"/>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnPrevs" runat="server" ImageUrl="~/images/new_prev.png" Visible="false"/>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:TextBox ID="txtRecNo" runat="server" ReadOnly="true" CssClass="text_box" Width="52px"
                        AutoPostBack="True" Style="text-align: right" disabled="disabled" TabIndex="1"
                        dir="ltr" Visible="false"></asp:TextBox>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:Label ID="Label47" runat="server" Visible="false">Of</asp:Label>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:Label ID="lblCount" runat="server" Width="20px" Visible="false"></asp:Label>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnNext" runat="server" ImageUrl="~/images/new_next.png" Visible="false"/>
                </td>
                <td style="float: left;display:None; visibility:collapse;width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnLast" runat="server" ImageUrl="~/images/new_first.png" Visible="false"/>
                </td>
                <td style="width: 2%; height: 14px"></td>
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
                    <asp:SiteMapPath ID="SiteMapPath1" runat="server" Width="270px">
                    </asp:SiteMapPath>
                </td>
                <td class="pagetext" style="height: 39px;text-align: right">
                    <asp:Label ID="lblMenuName" runat="server" Text="University Fund" Width="445px"></asp:Label></td>
            </tr>
        </table>
    </asp:Panel>
    <table style="width: 100%">
        <tr>
            <td style="width: 1px; height: 16px"></td>
            <td style="width: 98%; margin-left: auto; margin-right: auto; height: 16px">
                &nbsp;<fieldset style="width: 98%; border: thin solid #A6D9F4;">
                <legend><strong><span style="color: #000000">Selection Criteria</span></strong></legend>
                <table style="width: 100%">
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkALL" Text="Select All" runat="server" OnCheckedChanged="chkALL_CheckedChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                        <asp:CheckBoxList ID="chkTransType" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" AutoPostBack="true" OnSelectedIndexChanged="chkTransType_SelectedIndexChanged">
                            <asp:ListItem Text="AFC"></asp:ListItem>
                            <asp:ListItem Text="Student Credit Note"></asp:ListItem>
                            <asp:ListItem Text="Sponsor Allocation"></asp:ListItem>
                            <asp:ListItem Text="Student Invoice"></asp:ListItem>
                            <asp:ListItem Text="Receipt (Student)"></asp:ListItem>
                            <asp:ListItem Text="Sponsor Debit Note"></asp:ListItem>
                            <asp:ListItem Text="Student Debit Note"></asp:ListItem>
                            <asp:ListItem Text="Receipt (Sponsor)"></asp:ListItem>
                            <asp:ListItem Text="Sponsor Credit Note"></asp:ListItem>
                            <asp:ListItem Text="Student Payment (Refund/Pocket Expense)"></asp:ListItem>
                        </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td style="width: 15px; height: 16px">
                                        <asp:Label ID="Label1" runat="server" Text="From"></asp:Label></td>
                                    <td style="height: 16px">
                                        <asp:TextBox ID="txtDteFrom" runat="server"></asp:TextBox></td>
                                    <td style="width: 10px; height: 16px">
                                        <asp:Image ID="imgDteFrom" runat="server" ImageUrl="~/images/cal.gif" /></td>
                                    <td style="width: 9px; height: 16px">
                                        <asp:Label ID="Label2" runat="server" Text="To"></asp:Label></td>
                                    <td style="width: 63px; height: 16px">
                                        <asp:TextBox ID="txtDteTo" runat="server"></asp:TextBox></td>
                                    <td style="width: 648px; height: 16px">
                                        <asp:Image ID="imgDteTo" runat="server" ImageUrl="~/images/cal.gif" /></td>
                                    <td style="width: 1px; height: 16px"></td>
                                    <td style="width: 1px; height: 16px"></td>
                                    <td style="width: 1px; height: 16px"></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>

