<%@ Page Language="VB" MasterPageFile="~/MasterPage3.master" AutoEventWireup="false"
    CodeFile="BatchInvoice.aspx.vb" Inherits="BatchInvoice" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" src="Scripts/popcalendar.js" type="text/javascript"></script>
    <script language="javascript" src="Scripts/functions.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function chkdate(objName) {
            alert('lllll');
            var strDatestyle = "EU"; //United States date style
            //var strDatestyle = "EU";  //European date style
            var strDate;
            var strDateArray;
            var strDay;
            var strMonth;
            var strYear;
            var intday;
            var intMonth;
            var intYear;
            var booFound = false;
            var datefield = objName;
            var strSeparatorArray = new Array("-", " ", "/", ".");
            var intElementNr;
            var err = 0;
            var strMonthArray = new Array(12);
            strMonthArray[0] = "01";
            strMonthArray[1] = "02";
            strMonthArray[2] = "03";
            strMonthArray[3] = "04";
            strMonthArray[4] = "05";
            strMonthArray[5] = "06";
            strMonthArray[6] = "07";
            strMonthArray[7] = "08";
            strMonthArray[8] = "09";
            strMonthArray[9] = "10";
            strMonthArray[10] = "11";
            strMonthArray[11] = "12";
            strDate = datefield.value;
            if (strDate.length < 1) {
                return true;
            }
            for (intElementNr = 0; intElementNr < strSeparatorArray.length; intElementNr++) {
                if (strDate.indexOf(strSeparatorArray[intElementNr]) != -1) {
                    strDateArray = strDate.split(strSeparatorArray[intElementNr]);
                    if (strDateArray.length != 3) {
                        err = 1;
                        return false;
                    }
                    else {
                        strDay = strDateArray[0];
                        strMonth = strDateArray[1];
                        strYear = strDateArray[2];
                    }
                    booFound = true;
                }
            }
            if (booFound == false) {
                if (strDate.length > 5) {
                    strDay = strDate.substr(0, 2);
                    strMonth = strDate.substr(2, 2);
                    strYear = strDate.substr(4);
                }
            }
            if (strYear.length == 2) {
                strYear = '20' + strYear;
            }
            // US style
            if (strDatestyle == "US") {
                strTemp = strDay;
                strDay = strMonth;
                strMonth = strTemp;
            }
            intday = parseInt(strDay, 10);
            if (isNaN(intday)) {
                err = 2;
                return false;
            }
            intMonth = parseInt(strMonth, 10);
            if (isNaN(intMonth)) {
                for (i = 0; i < 12; i++) {
                    if (strMonth.toUpperCase() == strMonthArray[i].toUpperCase()) {
                        intMonth = i + 1;
                        strMonth = strMonthArray[i];
                        i = 12;
                    }
                }
                if (isNaN(intMonth)) {
                    err = 3;
                    return false;
                }
            }
            intYear = parseInt(strYear, 10);
            if (isNaN(intYear)) {
                err = 4;
                return false;
            }
            if (intMonth > 12 || intMonth < 1) {
                err = 5;
                return false;
            }
            if ((intMonth == 1 || intMonth == 3 || intMonth == 5 || intMonth == 7 || intMonth == 8 || intMonth == 10 || intMonth == 12) && (intday > 31 || intday < 1)) {
                err = 6;
                return false;
            }
            if ((intMonth == 4 || intMonth == 6 || intMonth == 9 || intMonth == 11) && (intday > 30 || intday < 1)) {
                err = 7;
                return false;
            }
            if (intMonth == 2) {
                if (intday < 1) {
                    err = 8;
                    return false;
                }
                if (LeapYear(intYear) == true) {
                    if (intday > 29) {
                        err = 9;
                        return false;
                    }
                }
                else {
                    if (intday > 28) {
                        err = 10;
                        return false;
                    }
                }
            }
            if (intday < 10) {
                intday = "0" + intday
            }
            if (strDatestyle == "US") {
                datefield.value = strMonthArray[intMonth - 1] + "-" + intday + "-" + strYear;
            }
            else {
                datefield.value = intday + "-" + strMonthArray[intMonth - 1] + "-" + strYear;
            }
            return true;
        }
        function LeapYear(intYear) {
            if (intYear % 100 == 0) {
                if (intYear % 400 == 0) { return true; }
            }
            else {
                if ((intYear % 4) == 0) { return true; }
            }
            return false;
        }

        function checkValue() {

            if (((event.keyCode < 48) || (event.keyCode > 57)) && (event.keyCode != 13) && (event.keyCode != 46)) {
                alert("Enter Valid Amount");
                event.preventDefault();
                return false;
            }
        }
        function formatDate(date, format) {
            format = format + "";
            var result = "";
            var i_format = 0;
            var c = "";
            var token = "";
            var y = date.getYear() + "";
            var M = date.getMonth() + 1;
            var d = date.getDate();
            var E = date.getDay();
            var H = date.getHours();
            var m = date.getMinutes();
            var s = date.getSeconds();
            var yyyy, yy, MMM, MM, dd, hh, h, mm, ss, ampm, HH, H, KK, K, kk, k;
            // Convert real date parts into formatted versions
            var value = new Object();
            if (y.length < 4) { y = "" + (y - 0 + 1900); }
            value["y"] = "" + y;
            value["yyyy"] = y;
            value["yy"] = y.substring(2, 4);
            value["M"] = M;
            value["MM"] = LZ(M);
            value["MMM"] = MONTH_NAMES[M - 1];
            value["NNN"] = MONTH_NAMES[M + 11];
            value["d"] = d;
            value["dd"] = LZ(d);
            value["E"] = DAY_NAMES[E + 7];
            value["EE"] = DAY_NAMES[E];
            value["H"] = H;
            value["HH"] = LZ(H);
            if (H == 0) { value["h"] = 12; }
            else if (H > 12) { value["h"] = H - 12; }
            else { value["h"] = H; }
            value["hh"] = LZ(value["h"]);
            if (H > 11) { value["K"] = H - 12; } else { value["K"] = H; }
            value["k"] = H + 1;
            value["KK"] = LZ(value["K"]);
            value["kk"] = LZ(value["k"]);
            if (H > 11) { value["a"] = "PM"; }
            else { value["a"] = "AM"; }
            value["m"] = m;
            value["mm"] = LZ(m);
            value["s"] = s;
            value["ss"] = LZ(s);
            while (i_format < format.length) {
                c = format.charAt(i_format);
                token = "";
                while ((format.charAt(i_format) == c) && (i_format < format.length)) {
                    token += format.charAt(i_format++);
                }
                if (value[token] != null) { result = result + value[token]; }
                else { result = result + token; }
            }
            return result;
        }
        function compareDates(date1, dateformat1, date2, dateformat2) {
            var d1 = getDateFromFormat(date1, dateformat1);
            var d2 = getDateFromFormat(date2, dateformat2);
            if (d1 == 0 || d2 == 0) {
                return -1;
            }
            else if (d1 > d2) {
                return 1;
            }
            return 0;
        }

        function CheckInvDate() {
            var digits = "0123456789/";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtInvoiceDate.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtInvoiceDate.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").value = "";
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function CheckBatchDate() {
            var digits = "0123456789/";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtBatchDate.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtBatchDate.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtBatchDate.ClientID%>").value = "";
                    document.getElementById("<%=txtBatchDate.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function CheckDueDate() {
            var digits = "0123456789/";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtDuedate.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtDuedate.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Valid Date (dd/mm/yyyy)");
                    document.getElementById("<%=txtDuedate.ClientID%>").value = "";
                    document.getElementById("<%=txtDuedate.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function geterr() {
            var digits = "0123456789";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtRecNo.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtRecNo.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Please Enter Correct Record No");
                    document.getElementById("<%=txtRecNo.ClientID%>").value = 1;
                    document.getElementById("<%=txtRecNo.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function CheckSem() {
            var digits = "0123456789";
            var temp;
            for (var i = 0; i < document.getElementById("<%=txtSemster.ClientID %>").value.length; i++) {
                temp = document.getElementById("<%=txtSemster.ClientID%>").value.substring(i, i + 1);
                if (digits.indexOf(temp) == -1) {
                    alert("Enter Correct Semester No");
                    document.getElementById("<%=txtSemster.ClientID%>").value = 1;
                    document.getElementById("<%=txtSemster.ClientID%>").focus();
                    return false;
                }
            }
            return true;
        }
        function getconfirm() {
            if (document.getElementById("<%=lblStatus.ClientID%>").value == "Posted") {
                alert("Posted Record Cannot be Deleted");
                return false;
            }
            if (document.getElementById("<%=lblStatus.ClientID%>").value == "New") {
                alert("Select a Record to Delete");
                return false;
            }

            if (document.getElementById("<%=txtBatchNo.ClientID%>").value == "") {
                alert("Select a Record");
                return false;
            }
            else {
                if (confirm("Do You Want to Delete Record?")) {
                    return true;
                }
                else {
                    return false;
                }
            }
            return true;
        }
        function getcheck() {
            var digits = "0123456789.";
            var temp;
            return true;
        }
        function getpostconfirm() {
            if (document.getElementById("<%=lblStatus.ClientID%>").value == "New") {
                alert("Select a Record to Post");
                return false;
            }
            if (document.getElementById("<%=lblStatus.ClientID%>").value == "Posted") {
                alert("Record already Posted");
                return false;
            }

            //modified by Hafiz @ 19/02/2017
            document.getElementById("<%=GLflagTrigger.ClientID%>").value = "ON";

            PageMethods.CheckGL(document.getElementById("<%=txtBatchNo.ClientID%>").value, document.getElementById("<%= btnBatchInvoice.ClientID%>").value,
                function (response) {
                    onSuccess(response);

                    if (response == true) {
                            if (confirm("Posted Record Cannot Be Altered, Do You Want To Proceed?")) {
                                if (document.getElementById("<%=txtBatchNo.ClientID%>").value == "") {
                                                        alert("Error - Batch number not found or empty.");
                                                        return false;
                                                    }
                                                    else {
                                                        new_window = window.open('AddApprover.aspx?MenuId=' + document.getElementById("<%=MenuId.ClientID%>").value + '&Batchcode=' + document.getElementById("<%=txtBatchNo.ClientID%>").value + '',
                                   'Hanodale', 'width=500,height=400,resizable=0'); new_window.focus();
                                return true;
                            }
                        }
                    }
                    else {
                        alert("Posting Failed. NO GL FOUND.")

                        new_window = window.open('GLFailedList.aspx?MenuId=' + document.getElementById("<%=MenuId.ClientID%>").value + '&Batchcode=' + document.getElementById("<%=txtBatchNo.ClientID%>").value,'Hanodale', 'width=500,height=400,resizable=0'); new_window.focus();
                        return false;
                    }
                }, onFailure);
        }

        function onSuccess(response) {
            return response;
        }

        function onFailure(response) {
            alert("Posted Record Fail.");
        }

        function Validate() {
            var re = /\s*((\S+\s*)*)/;
            <%--if (document.getElementById("<%=txtBatchNo.ClientID%>") != null) {
                if (document.getElementById("<%=txtBatchNo.ClientID%>").value.replace(re, "$1").length == 0) {
                    alert("Batch No Field Cannot Be Blank");
                    document.getElementById("<%=txtBatchNo.ClientID%>").focus();
                    return false;
                }
            }--%>
            var digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz0123456789";
            var temp;
            if (document.getElementById("<%=txtBatchNo.ClientID %>") != null) {
                for (var i = 0; i < document.getElementById("<%=txtBatchNo.ClientID %>").value.length; i++) {
                    temp = document.getElementById("<%=txtBatchNo.ClientID%>").value.substring(i, i + 1);
                    if (digits.indexOf(temp) == -1) {
                        alert("Please Enter Correct Code");
                        document.getElementById("<%=txtBatchNo.ClientID%>").focus();
                        return false;
                    }
                }
            }
            if (document.getElementById("<%=txtBatchDate.ClientID%>") != null) {

                if (document.getElementById("<%=txtBatchDate.ClientID%>").value.replace(re, "$1").length == 0) {
                    alert("Batch Date Field Cannot Be Blank");
                    document.getElementById("<%=txtBatchDate.ClientID%>").focus();
                    return false;
                }
            }
            if (document.getElementById("<%=txtInvoiceDate.ClientID%>") != null) {
                if (document.getElementById("<%=txtInvoiceDate.ClientID%>").value == "") {
                    alert("Invoice Date Field Cannot Be Blank");
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                    return false;
                }
            }
            //if (document.getElementById("<%=txtDesc.ClientID%>") != null) {
            //if (document.getElementById("<%=txtDesc.ClientID%>").value.replace(re, "$1").length == 0) {
            //alert("Description Field Cannot Be Blank");
            //document.getElementById("<%=txtDesc.ClientID%>").focus();
            //return false;
            //}
            //}
            //txtBatchDate---------------------------------------------------------------------------
            if (document.getElementById("<%=txtBatchDate.ClientID%>") != null) {
                var len = document.getElementById("<%=txtBatchDate.ClientID%>").value
                var RegExPattern = /^((((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))|(((0[1-9]|[12]\d|3[01])(0[13578]|1[02])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|[12]\d|30)(0[13456789]|1[012])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|1\d|2[0-8])02((1[6-9]|[2-9]\d)?\d{2}))|(2902((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00))))$/;
                var errorMessage = 'Enter Valid Date (dd/mm/yyyy)';

                if (document.getElementById("<%=txtBatchDate.ClientID%>").value.match(RegExPattern)) {
                    if (len.length == 8) {
                        alert("ddd");
                        alert(errorMessage);
                        document.getElementById("<%=txtBatchDate.ClientID%>").value = "";
                        document.getElementById("<%=txtBatchDate.ClientID%>").focus();
                        return false;
                    }
                }
                else {

                    alert(errorMessage);
                    document.getElementById("<%=txtBatchDate.ClientID%>").value = "";
                    document.getElementById("<%=txtBatchDate.ClientID%>").focus();
                    return false;
                }
                var str1 = document.getElementById("<%=txtBatchDate.ClientID %>").value;
                var str2 = document.getElementById("<%=today.ClientID %>").value;
                var dt1 = parseInt(str1.substring(0, 2), 10);
                var mon1 = parseInt(str1.substring(3, 5), 10);
                var yr1 = parseInt(str1.substring(6, 10), 10);
                var dt2 = parseInt(str2.substring(0, 2), 10);
                var mon2 = parseInt(str2.substring(3, 5), 10);
                var yr2 = parseInt(str2.substring(6, 10), 10);
                var date1 = new Date(yr1, mon1, dt1);
                var date2 = new Date(yr2, mon2, dt2);




                if (date2 < date1) {
                    alert("Batch Date Cannot be Greater than Current Date");
                    document.getElementById("<%=txtBatchDate.ClientID%>").value = "";
                    document.getElementById("<%=txtBatchDate.ClientID%>").focus();
                    //"");
                    return false;
                }
            }
            //txtInvoiceDate---------------------------------------------------------------------------
            if (document.getElementById("<%=txtInvoiceDate.ClientID%>") != null) {
                var len = document.getElementById("<%=txtInvoiceDate.ClientID%>").value
                var RegExPattern = /^((((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))|(((0[1-9]|[12]\d|3[01])(0[13578]|1[02])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|[12]\d|30)(0[13456789]|1[012])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|1\d|2[0-8])02((1[6-9]|[2-9]\d)?\d{2}))|(2902((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00))))$/;
                var errorMessage = 'Enter Valid Date (dd/mm/yyyy)';

                if (document.getElementById("<%=txtInvoiceDate.ClientID%>").value.match(RegExPattern)) {
                    if (len.length == 8) {
                        alert(errorMessage);
                        document.getElementById("<%=txtInvoiceDate.ClientID%>").value = "";
                        document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                        return false;
                    }
                }
                else {
                    alert(errorMessage);
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").value = "";
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                    return false;
                }


                var str1 = document.getElementById("<%=txtInvoiceDate.ClientID %>").value;
                var str2 = document.getElementById("<%=today.ClientID %>").value;
                var dt1 = parseInt(str1.substring(0, 2), 10);
                var mon1 = parseInt(str1.substring(3, 5), 10);
                var yr1 = parseInt(str1.substring(6, 10), 10);
                var dt2 = parseInt(str2.substring(0, 2), 10);
                var mon2 = parseInt(str2.substring(3, 5), 10);
                var yr2 = parseInt(str2.substring(6, 10), 10);
                var date1 = new Date(yr1, mon1, dt1);
                var date2 = new Date(yr2, mon2, dt2);




                if (date2 < date1) {
                    alert("Invoice Date Cannot be Greater than Current Date");
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").value = "";
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                    //"");
                    return false;
                }
            }

            if (document.getElementById("<%=txtDuedate.ClientID%>") != null) {
                if (document.getElementById("<%=txtDuedate.ClientID%>").value.replace(re, "$1").length == 0) {
                    alert("Due Date Field Cannot Be Blank");
                    document.getElementById("<%=txtDuedate.ClientID%>").focus();
                    return false;
                }

                //txtDueDate---------------------------------------------------------------------------
                var len = document.getElementById("<%=txtDuedate.ClientID%>").value
                var RegExPattern = /^((((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))|(((0[1-9]|[12]\d|3[01])(0[13578]|1[02])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|[12]\d|30)(0[13456789]|1[012])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|1\d|2[0-8])02((1[6-9]|[2-9]\d)?\d{2}))|(2902((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00))))$/;
                var errorMessage = 'Enter Valid Date (dd/mm/yyyy)';

                if (document.getElementById("<%=txtDuedate.ClientID%>").value.match(RegExPattern)) {
                    if (len.length == 8) {
                        alert(errorMessage);
                        document.getElementById("<%=txtDuedate.ClientID%>").value = "";
                        document.getElementById("<%=txtDuedate.ClientID%>").focus();
                        return false;
                    }
                }
                else {
                    alert(errorMessage);
                    document.getElementById("<%=txtDuedate.ClientID%>").value = "";
                    document.getElementById("<%=txtDuedate.ClientID%>").focus();
                    return false;
                }

                //txtDesc----------------------------------------------------
                //if (document.getElementById("<%=txtDesc.ClientID%>").value == "") {
                //alert("Description Field Cannot Be Blank");
                //document.getElementById("<%=txtDesc.ClientID%>").focus();
                //return false;
                //}
                //Compare Dates----------------------------------------------
                var str1 = document.getElementById("<%=txtInvoiceDate.ClientID %>").value;
                var str2 = document.getElementById("<%=txtDuedate.ClientID %>").value;
                var dt1 = parseInt(str1.substring(0, 2), 10);
                var mon1 = parseInt(str1.substring(3, 5), 10);
                var yr1 = parseInt(str1.substring(6, 10), 10);
                var dt2 = parseInt(str2.substring(0, 2), 10);
                var mon2 = parseInt(str2.substring(3, 5), 10);
                var yr2 = parseInt(str2.substring(6, 10), 10);
                var date1 = new Date(yr1, mon1, dt1);
                var date2 = new Date(yr2, mon2, dt2);




                if (date2 < date1) {
                    alert("Due Date Should be Greater than Invoice Date");
                    document.getElementById("<%=txtDuedate.ClientID%>").value = "";
                    document.getElementById("<%=txtDuedate.ClientID%>").focus();
                    return false;
                }
            }
            if (document.getElementById("<%=txtInvoiceDate.ClientID %>") != null & document.getElementById("<%=txtBatchDate.ClientID %>")) {
                //Compare Dates----------------------------------------------
                var str1 = document.getElementById("<%=txtInvoiceDate.ClientID %>").value;
                var str2 = document.getElementById("<%=txtBatchDate.ClientID %>").value;
                var dt1 = parseInt(str1.substring(0, 2), 10);
                var mon1 = parseInt(str1.substring(3, 5), 10);
                var yr1 = parseInt(str1.substring(6, 10), 10);
                var dt2 = parseInt(str2.substring(0, 2), 10);
                var mon2 = parseInt(str2.substring(3, 5), 10);
                var yr2 = parseInt(str2.substring(6, 10), 10);
                var date1 = new Date(yr1, mon1, dt1);
                var date2 = new Date(yr2, mon2, dt2);




                if (date2 > date1) {
                    alert("Invoice Date Cannot be Lesser than Batch Date");
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").value = "";
                    document.getElementById("<%=txtInvoiceDate.ClientID%>").focus();
                    return false;
                }
            }

            var hfValidateAmt = document.getElementById("<%=hfValidateAmt.ClientID%>").value
            if (hfValidateAmt == "True") {
                alert("Invalid Fee Amount not Accepted");
                return false;
            }

            return true;
        }

        function getibtnBDate() {
            popUpCalendar(document.getElementById("<%=ibtnBDate.ClientID%>"), document.getElementById("<%=txtBatchDate.ClientID%>"), 'dd/mm/yyyy')

        }
        function getDate1from() {
            popUpCalendar(document.getElementById("<%=ibtnInDate.ClientID%>"), document.getElementById("<%=txtInvoiceDate.ClientID%>"), 'dd/mm/yyyy')

        }
        function getDate2from() {
            popUpCalendar(document.getElementById("<%=ibtnDueDate.ClientID%>"), document.getElementById("<%=txtDueDate.ClientID%>"), 'dd/mm/yyyy')

        }
        function ValidateObjects() {
            var jsFromDate = document.getElementById("<%=txtInvoiceDate.ClientID %>").value;
            var jsToDate = document.getElementById("<%=txtDuedate.ClientID %>").value;
            var TempFromDate = new Date(jsFromDate);
            var TempToDate = new Date(jsToDate);
            if (jsFromDate == '' && jsToDate != '') {
                alert("Please Enter Correct Record No");
                return false;
            }
            if (jsToDate == '' && jsFromDate != '') {
                alert("Please Enter Correct Record No");
                return false;
            }
            if (jsToDate == '' && jsFromDate == '') {
                alert("Please Enter Correct Record No");
                return false;
            }
            if (TempFromDate > TempToDate) {
                alert("Please Enter Correct Record No");

                return false;
            }
            else if (TempFromDate == TempToDate) {
                return true;
            }
            else if (TempFromDate <= TempToDate) {
                return true;
            }
        }
        //Print Record
        function getPrint() {

            var str1 = document.getElementById("<%=txtBatchNo.ClientID %>").value;
            var Formid = document.getElementById("<%= btnBatchInvoice.ClientID%>").value
            window.open('GroupReport/RptBatchInvoiceViewer.aspx?batchNo=' + str1 + '&Formid=' + Formid, 'SAS', 'width=700,height=500,resizable=1,scrollbars=1');
        }

        function CheckStudent() {
            var hfStudentCount = document.getElementById("<%=hfStudentCount.ClientID%>").value
            var strCat = document.getElementById("<%=hfStdCategory.ClientID%>").value;

            if (hfStudentCount > 0) {
                var new_window = window.open('StudentFeetype.aspx?Student=' + strCat, 'Hanodale', 'width=520,height=500,resizable=0');
                new_window.focus();
            }
            else {
                document.getElementById("<%=lblMsg.ClientID%>").innerHTML = "Please select at least one student"
                return false;
            }
        }

        function OpenWindow(URL) {
            var WindowName = "MyPopup";
            var Features = "location=no,toolbar=no,menubar=no,height =600,scrollbars=yes";
            window.open(URL, WindowName, Features);
        }

    </script>

    <asp:Panel ID="pnlToolbar" runat="server" Width="100%">
        <table style="background-image: url(images/Sample.png);">
            <tr>
                <td style="width: 4px; height: 14px"></td>
                <td style="width: 14px; height: 14px"></td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnNew" runat="server" ImageUrl="~/images/add.png" ToolTip="New" />
                            </td>
                            <td>
                                <asp:Label ID="Label11" runat="server" Text="New"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnSave" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" />
                            </td>
                            <td>
                                <asp:ImageButton ID="btnSaveChange" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label14" runat="server" Text="Save"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnDelete" runat="server" ImageUrl="~/images/delete.png" ToolTip="Delete" />
                            </td>
                            <td>
                                <asp:Label ID="Label13" runat="server" Text="Delete"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <div id="wrap">
                                    <ul id="navbar">
                                        <li><a href="#">
                                            <img src="images/find.png" width="24" height="24" border="0" align="middle" />&nbsp;Search
                                            <img src="images/down.png" width="16" height="16" border="0" align="middle" />
                                        </a>
                                            <ul>
                                                <li><a href="#">
                                                    <asp:ImageButton ID="ibtnView" runat="server" ImageUrl="~/images/ready.png" /></a></li>
                                                <li><a href="#">
                                                    <asp:ImageButton ID="ibtnOthers" runat="server" ImageUrl="~/images/post.png" ToolTip="Cancel"
                                                        OnClick="ibtnOthers_Click" /></li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr id="trPrint" runat="server">
                            <td>
                                <asp:ImageButton ID="ibtnPrint" runat="server" ImageUrl="~/images/gprint.png" ToolTip="Print" Enabled="false" />
                                <asp:HiddenField ID="hfIsPrint" runat="server" />
                            </td>
                            <td>
                                <asp:Label ID="lblPrint" runat="server" Text="Print"></asp:Label>
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
                                    ToolTip="Cancel" />
                            </td>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text="Posting"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                 <%-- Editted By Zoya @25/02/2016--%>
                <td style="float: left;display:None; visibility:collapse;">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/gothers.png"
                                    ToolTip="Cancel" OnClick="ibtnOthers_Click" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="Others" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="float: left">
                    <table style="border-collapse: collapse;" class="menuoff" onmouseover="className='menuon';"
                        onmouseout="className='menuoff';">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnCancel" runat="server" ImageUrl="~/images/cancel.png" ToolTip="Cancel" />
                            </td>
                            <td>
                                <asp:Label ID="Label18" runat="server" Text="Cancel"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                 <%-- Done Editted By Zoya @25/02/2016--%>

                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnFirst" runat="server" ImageUrl="~/images/new_last.png" ToolTip="First" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnPrevs" runat="server" ImageUrl="~/images/new_prev.png" ToolTip="Previous" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:TextBox ID="txtRecNo" runat="server" AutoPostBack="True" Width="50px" OnTextChanged="txtRecNo_TextChanged"
                        MaxLength="7" ReadOnly="true" CssClass="text_box" disabled="disabled" TabIndex="1"
                        dir="ltr"></asp:TextBox>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="Label47" runat="server">Of</asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="lblCount" runat="server" Width="20px"></asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnNext" runat="server" ImageUrl="~/images/new_next.png" ToolTip="Next" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnLast" runat="server" ImageUrl="~/images/new_first.png" ToolTip="Last" />
                </td>
                <td style="width: 2%; height: 14px"></td>
                <td style="width: 100%; height: 14px"></td>
                <td style="width: 100%; height: 14px"></td>
            </tr>
        </table>
        <table style="background-image: url(images/Sample.png);">
            <tr style="display: none;">
                <td style="width: 4px; height: 14px"></td>
                <td style="width: 14px; height: 14px"></td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnNew0" runat="server" ImageUrl="~/images/add.png" ToolTip="New" />
                            </td>
                            <td>
                                <asp:Label ID="Label48" runat="server" Text="New"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnSave0" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" />
                            </td>
                            <td>
                                <asp:ImageButton ID="btnSaveChange0" runat="server" ImageUrl="~/images/save.png" ToolTip="Save" Visible="false" />
                            </td>
                            <td>
                                <asp:Label ID="Label49" runat="server" Text="Save"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnDelete0" runat="server" ImageUrl="~/images/delete.png" ToolTip="Delete" />
                            </td>
                            <td>
                                <asp:Label ID="Label50" runat="server" Text="Delete"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td style="width: 3%; height: 14px">
                                <div id="Div1">
                                    <ul id="Ul1">
                                        <li><a href="#">
                                            <img src="images/find.png" width="24" height="24" border="0" align="middle" />
                                            &nbsp;Search
                                            <img src="images/down.png" width="16" height="16" border="0" align="middle" />
                                        </a>
                                            <ul>
                                                <li><a href="#">
                                                    <asp:ImageButton ID="ibtnView0" runat="server" ImageUrl="~/images/ready.png" />
                                                </a></li>
                                                <li><a href="#">
                                                    <asp:ImageButton ID="ibtnOthers0" runat="server" ImageUrl="~/images/post.png" OnClick="ibtnOthers_Click" ToolTip="Cancel" />
                                                </a></li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr id="trPrint0" runat="server">
                            <td>
                                <asp:ImageButton ID="ibtnPrint0" runat="server" ImageUrl="~/images/print.png" ToolTip="Print" />
                            </td>
                            <td>
                                <asp:Label ID="lblPrint0" runat="server" Text="Print"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/gothers.png" OnClick="ibtnOthers_Click" ToolTip="Cancel" />
                            </td>
                            <td>
                                <asp:Label ID="Label52" runat="server" Text="Others"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table class="menuoff" onmouseout="className='menuoff';" onmouseover="className='menuon';" style="border-collapse: collapse;">
                        <tr>
                            <td>
                                <asp:ImageButton ID="ibtnCancel0" runat="server" ImageUrl="~/images/cancel.png" ToolTip="Cancel" />
                            </td>
                            <td>
                                <asp:Label ID="Label53" runat="server" Text="Cancel"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnFirst0" runat="server" ImageUrl="~/images/new_last.png" ToolTip="First" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnPrevs0" runat="server" ImageUrl="~/images/new_prev.png" ToolTip="Previous" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:TextBox ID="txtRecNo0" runat="server" AutoPostBack="True" CssClass="text_box" dir="ltr" disabled="disabled" MaxLength="7" OnTextChanged="txtRecNo_TextChanged" ReadOnly="true" TabIndex="1" Width="50px"></asp:TextBox>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="Label54" runat="server">Of</asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:Label ID="lblCount0" runat="server" Width="20px"></asp:Label>
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnNext0" runat="server" ImageUrl="~/images/new_next.png" ToolTip="Next" />
                </td>
                <td style="width: 2%; height: 14px">
                    <asp:ImageButton ID="ibtnLast0" runat="server" ImageUrl="~/images/new_first.png" ToolTip="Last" />
                </td>
                <td style="width: 2%; height: 14px"></td>
                <td style="width: 100%; height: 14px"></td>
                <td style="width: 100%; height: 14px"></td>
            </tr>
        </table>
        <table style="width: 100%">
            <tr>
                <td class="vline" style="width: 746px; height: 1px"></td>
            </tr>
        </table>
        <table style="width: 100%">
            <tr>
                <td style="width: 400px">
                    <asp:SiteMapPath ID="SiteMapPath1" runat="server">
                    </asp:SiteMapPath>
                </td>
                <td align="right" class="pagetext">
                    <asp:Label ID="lblMenuName" runat="server" Width="350px"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>

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
                                <li class="ext">
                                    <asp:Button ID="btnSelection" runat="server" Height="25px" Text="Selection Criteria" Width="108px" CssClass="TabButton" />
                                </li>
                                <li class="dropdown">
                                    <asp:Button ID="btnViewStu" runat="server" Height="25px" Text="View Students" Width="108px" CssClass="TabButton" />
                                    <div class="dropdown-content">
                                        <asp:LinkButton id="link1" Text="Add Students" OnClick="btnViewStu_Click" runat="server"/>
                                        <asp:LinkButton id="link2" Text="Upload File" OnClick="btnViewUploadFile_Click" runat="server"/>
	                                </div>
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
                                            <table id="TableAddFeeManual" runat="server"  style="width: 283px">
                                                <tr>
                                                    <td style="height: 23px"></td>
                                                    <td style="width: 52px; height: 23px;">
                                                        <asp:Label ID="Label26" runat="server" Text="Add Fee Item" Width="64px" Visible ="true"></asp:Label>
                                                    </td>
                                                    <td style="width: 4px; height: 23px;">
                                                        <asp:ImageButton ID="ibtnAddFeeType" runat="server" Height="21px" ImageUrl="~/images/addrec.gif"
                                                            ToolTip="Add" OnClick="ibtnAddFeeType_Click" Visible ="true"/>
                                                        <asp:HiddenField ID="hfStudentCount" runat="server" Value="0" />
                                                    </td>
                                                    <td style="width: 135px; text-align: right; height: 23px;">
                                                        <asp:Label ID="Label27" runat="server" Text="Remove Fee Item" Width="90px" Visible ="true"></asp:Label>
                                                    </td>
                                                    <td style="width: 26px; height: 23px;">
                                                        <asp:ImageButton ID="ibtnRemoveFee" runat="server" Height="21px" ImageUrl="~/images/removey.gif"
                                                            ToolTip="Remove" Width="20px" OnClick="ibtnRemoveFee_Click" Visible ="true"/>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDgFeeType" runat="server" Width="100%" Visible="false">
                                    <table style="width: 100%">
                                        <tr>
                                            <td width="100%" style="height: 261px">

                                                <asp:CheckBox ID="chkFeeType" runat="server" AutoPostBack="True" OnCheckedChanged="chkFeeType_CheckedChanged" Text="Select All" />
                                                <asp:DataGrid ID="dgFeeType" runat="server" AutoGenerateColumns="False" Width="100%"
                                                    ShowFooter="True" DataKeyField="ReferenceCode" Style="vertical-align: top">
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
                                                                <asp:CheckBox ID="chkView" runat="server" AutoPostBack="true" />
                                                            </ItemTemplate>
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn HeaderText="Matric No" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Name" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Program ID" Visible="false"></asp:BoundColumn>
                                                        <asp:ButtonColumn CommandName="Select" DataTextField="ReferenceCode" HeaderText="Fee Code"
                                                            Text="ReferenceCode"></asp:ButtonColumn>
                                                        <asp:BoundColumn DataField="Description" HeaderText="Fee Desc"></asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Unit Amount">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtFeeAmt_dgFeeType" runat="server" Width="98px" AutoPostBack="True" OnTextChanged="txtFeeAmt_dgFeeType_TextChanged"
                                                                    Text='<%# GSTFeeAmtdgFeeType(Eval("TempPaidAmount"), Eval("TaxAmount"), Eval("TaxId"), Eval("ReferenceCode"))%>' Style="text-align: right" Height="18px" MaxLength="10"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="false" Font-Italic="false" Font-Overline="false" Font-Strikeout="false"
                                                                Font-Underline="false" HorizontalAlign="right" />
                                                            <FooterStyle Font-Bold="false" Font-Italic="false" Font-Overline="false" Font-Strikeout="false"
                                                                Font-Underline="false" HorizontalAlign="right" />
                                                            <HeaderStyle Width="15%" />
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn HeaderText="Student Qty" Visible="true" DataField="StudentQty"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="TransactionAmount" HeaderText="FeeAmount" Visible="False">
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                            <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Fee Code" Visible="False"></asp:BoundColumn>
                                                        <%--   <asp:TemplateColumn HeaderText="Actual Fee Amount" FooterText="Total">
                                                        <ItemTemplate>
                                                            <%# Eval("TempAmount", "{0:F}")%>
                                                        </ItemTemplate>
                                                        <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" HorizontalAlign="Right" />
                                                        <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                            Font-Underline="False" HorizontalAlign="Right" />
                                                    </asp:TemplateColumn>--%>
                                                        <asp:TemplateColumn HeaderText="Actual Fee Amount" FooterText="Total">
                                                            <ItemTemplate>
                                                                <%# Eval("TempAmount", "{0:F}")%>
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
                                                        <asp:BoundColumn DataField="TempPaidAmount" HeaderText="FeeAmount" Visible="false"></asp:BoundColumn>
                                                    </Columns>
                                                </asp:DataGrid>

                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 100%; text-align: right;">
                                        <tr>
                                            <td style="width: 76%">
                                                <asp:Label ID="lblTotalFeeAmt" runat="server" Text="Total Amount" Width="65px" ></asp:Label>
                                            </td>
                                            <td style="text-align: left">
                                                <asp:TextBox ID="txtTotalFeeAmt" runat="server" Width="106px" Style="text-align: right" Enabled="false"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="pnlDgView" Width="100%" runat="server" Visible="false">
                                    <table style="width: 100%">
                                        <tr>
                                            <td width="100%">
                                                <asp:CheckBox ID="chkSelectedView" runat="server" OnCheckedChanged="chkSelectedView_CheckedChanged" 
                                                    AutoPostBack="True" Text="Select All" />
                                                <asp:DataGrid ID="dgView" runat="server" AutoGenerateColumns="False" Width="100%"
                                                    ShowFooter="True" DataKeyField="ReferenceCode" OnSelectedIndexChanged="dgView_SelectedIndexChanged"
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
                                                                &nbsp;<asp:CheckBox ID="chkView" runat="server" AutoPostBack="true" OnCheckedChanged ="chk_SelectCheckedChanged"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn HeaderText="Matric No" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn HeaderText="Name" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="ReferenceCode" HeaderText="Fee Code"
                                                           ></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Description" HeaderText="Fee Desc"></asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Fee Amount">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtFeeAmt" runat="server" Width="98px" AutoPostBack="True" OnTextChanged="txtFeeAmt_TextChanged"
                                                                    Text='<%# GSTFeeAmt(Eval("TransactionAmount"), Eval("GSTAmount"), Eval("TaxId"))%>' Style="text-align: right" Height="18px" MaxLength="10"></asp:TextBox>
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
                                                        
                                                        <asp:BoundColumn DataField="TempPaidAmount" HeaderText="Original Amount" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:F}"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Transstatus" HeaderText="Status" Visible ="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Internal_Use" HeaderText="OpenTransID" Visible="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="batchno" HeaderText="Inv_no" Visible="false"></asp:BoundColumn>
                                                       
                                                        <asp:BoundColumn DataField="cat" HeaderText="Category" Visible ="false"></asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Inv_no" HeaderText="docno" Visible ="false"></asp:BoundColumn>
                                                         <asp:TemplateColumn HeaderText="View" Visible ="false">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <center>
                                        <asp:HyperLink ID="View" runat="server">View</asp:HyperLink></center>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                                                        <asp:BoundColumn DataField="PaidAmount" HeaderText="PaidAmt" Visible ="false"></asp:BoundColumn>
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

                                <asp:Panel ID="PnlFeeTypeFileUpload" runat="server" Width="100%" Visible="false">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td width="100%">
                                                <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" Text="Select All" Visible="false"  />
                                                <asp:GridView ID="gvFileUploadGrid" runat="server" AutoGenerateColumns="false"
	                                                Width="100%" ShowFooter="True" Style="vertical-align: text-top" OnRowDataBound="gvFileUploadGrid_RowDataBound">
	                                                <FooterStyle CssClass="dgFooterStyle" Height="20px" />
	                                                <SelectedRowStyle CssClass="dgSelectedItemStyle" />
	                                                <AlternatingRowStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
		                                                Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
	                                                <RowStyle CssClass="dgItemStyle" HorizontalAlign="Center" />
	                                                <HeaderStyle BackColor="#00699b" CssClass="dgHeaderStyle" ForeColor="#ffffff" Font-Bold="False" Font-Italic="False"
		                                                Font-Overline="False" Font-Size="8pt" Font-Strikeout="False" Font-Underline="False" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Matric No">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel0" runat="server" Text='<%# Eval("MatricNo")%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Student Name">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel1" runat="server" Text='<%# Eval("StudentName")%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Code">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel2" runat="server" Text='<%# Eval("ReferenceCode")%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Desc">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel3" runat="server" Text='<%# Eval("Description")%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel4" runat="server" Text='<%# GSTFeeAmt(Eval("TransactionAmount"), Eval("GSTAmount"), Eval("TaxId"))%>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                         <asp:TemplateField HeaderText="Actual Fee Amount" FooterText="Total">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel5" runat="server" Text='<%# GSTActAmt(Eval("TransactionAmount"), Eval("GSTAmount"), Eval("TaxId"))%>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
				                                                Font-Underline="False" HorizontalAlign="Right" />
			                                                <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
				                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                         <asp:TemplateField HeaderText="GST Amount" ItemStyle-HorizontalAlign="Right" >
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel6" runat="server" Text='<%# Eval("GSTAmount", "{0:F}")%>' />
                                                            </ItemTemplate>
                                                             <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
				                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                         <asp:TemplateField HeaderText="Total Fee Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Llabel7" runat="server" Text='<%# Eval("TransactionAmount", "{0:F}")%>' />
                                                            </ItemTemplate>
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
				                                                Font-Underline="False" HorizontalAlign="Right" />
			                                                <FooterStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
				                                                Font-Underline="False" HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 100%; text-align: right; visibility:hidden ">
                                        <tr>
                                            <td style="width: 76%">
                                                <asp:Label ID="Label16" runat="server" Text="Total Amount" Width="65px" ></asp:Label>
                                            </td>
                                            <td style="text-align: left">
                                                <asp:TextBox ID="txtUFtotamt" runat="server" Width="106px"  Style="text-align: right"  Enabled="false"></asp:TextBox>
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
        <asp:View ID="View2" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%">
                        <div style="border: thin solid #A6D9F4; width: 100%">
                            <asp:Panel ID="pnlSelection" runat="server" Height="100%" Width="100%" Visible="False">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 100px; text-align: left; height: 23px;">
                                            <asp:Label ID="Label24" runat="server" Text="Faculty " Width="45px">
                                            </asp:Label>
                                        </td>
                                        <td style="height: 23px">
                                            <asp:DropDownList ID="ddlFaculty" runat="server" AutoPostBack="True" Width="149px"
                                                AppendDataBoundItems="True">
                                                <asp:ListItem Value="-1">--Select--</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnChangeProg" runat="server" Height="27px"
                                                Text="Change of Program" Width="177px" Visible="false" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnChangeCdtHr" runat="server" Height="27px"
                                                Text="Change of Credit Hour" Width="177px" Visible="false" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnChangeHostel" runat="server" Height="27px"
                                                Text="Change of Hostel" Width="177px" Visible="false" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnChangeHostel2" runat="server" Height="27px"
                                                Text="Change Hostel" Width="177px" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                                <table style="width: 100%; height: 100%">
                                    <tr>
                                        <td style="width: 30%; height: 16px">
                                            <table style="width: 100%; height: 100%">
                                                <tr>
                                                    <td colspan="1" style="text-align: left">
                                                        <asp:Label ID="Label8" runat="server" Text="Program" Width="45px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                        <asp:Label ID="Label15" runat="server" Text="Include Program" Width="150px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                        <asp:Panel ID="Panel4" runat="server" Height="50px" Width="100%">
                                                            <asp:RadioButton ID="rbProYes" runat="server" Text="Yes" GroupName="GpProgram" AutoPostBack="true" OnCheckedChanged="rbSelectProgram_CheckedChanged" /><br />
                                                            <asp:RadioButton ID="rbProNo" runat="server" Text="No" GroupName="GpProgram" AutoPostBack="true" OnCheckedChanged="rbSelectProgram_CheckedChanged" />
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px; height: 12px">
                                                        <asp:CheckBox ID="chkSelectProgram" runat="server" Text="Select All" AutoPostBack="True" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 35%; height: 16px">&nbsp;<table style="width: 100%; height: 100%">
                                            <tr>
                                                <td colspan="1" style="text-align: left"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="1" style="text-align: left">
                                                    <asp:Label ID="Label19" runat="server" Text="Sponsor" Width="45px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100px">
                                                    <asp:Label ID="Label20" runat="server" Text="Include Sponsor" Width="150px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100px; height: 61px;">
                                                    <asp:Panel ID="Panel5" runat="server" Height="50px" Width="100%">
                                                        <asp:RadioButton ID="rbSemYes" runat="server" Text="Yes" GroupName="GpSponsor" AutoPostBack="true" OnCheckedChanged="rbSelectSponsor_CheckedChanged" /><br />
                                                        <asp:RadioButton ID="rbSemNo" runat="server" Text="No" GroupName="GpSponsor" AutoPostBack="true" OnCheckedChanged="rbSelectSponsor_CheckedChanged" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100px; height: 12px">
                                                    <asp:CheckBox ID="chkSelectSponsor" runat="server" Text="Select All" AutoPostBack="True" />
                                                </td>
                                            </tr>
                                        </table>
                                        </td>
                                        <td style="width: 35%; height: 16px">
                                            <table style="width: 100%; height: 100%">
                                                <tr>
                                                    <td style="width: 100px; text-align: left">
                                                        <asp:Label ID="Label21" runat="server" Text="Hostel" Width="45px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                        <asp:Label ID="Label22" runat="server" Text="Include Hostel" Width="150px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                        <asp:Panel ID="Panel6" runat="server" Height="50px" Width="100%">
                                                            <asp:RadioButton ID="rbHosYes" runat="server" Text="Yes" GroupName="GpHostel" AutoPostBack="true" OnCheckedChanged="rbSelectHostel_CheckedChanged" /><br />
                                                            <asp:RadioButton ID="rbHosNo" runat="server" Text="No" GroupName="GpHostel" AutoPostBack="true" OnCheckedChanged="rbSelectHostel_CheckedChanged" />
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px; height: 12px">
                                                        <asp:CheckBox ID="chkSelectHostel" runat="server" Text="Select All" AutoPostBack="True" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%; vertical-align: top;">
                                            <asp:DataGrid ID="dgProgram" runat="server" AutoGenerateColumns="False" Width="100%">
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
                                                            &nbsp;<asp:CheckBox ID="chkProgram" runat="server" OnCheckedChanged="chkProgram_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="ProgramCode" HeaderText="Code"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Program" HeaderText="Program Desc"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                                        <td style="width: 35%; vertical-align: top;">
                                            <asp:DataGrid ID="DgSponsor" runat="server" AutoGenerateColumns="False" Width="100%">
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
                                                            &nbsp;<asp:CheckBox ID="chkSponsor" runat="server" OnCheckedChanged="chkSponsor_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="SponserCode" HeaderText="Code"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Name" HeaderText="Sponsor "></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                                        <td style="width: 35%; vertical-align: top;">
                                            <asp:DataGrid ID="dgHostel" runat="server" AutoGenerateColumns="False" Width="100%">
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
                                                            &nbsp;<asp:CheckBox ID="chkHostel" runat="server" OnCheckedChanged="chkHostel_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="SAKO_Code" HeaderText="Code"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="SAKO_Description" HeaderText="Hostel "></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%;">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="height: 2px" colspan="3">
                                                        <asp:Label ID="Label10" runat="server" Text="Semester" Width="73px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 59px; text-align: right">
                                                        <asp:RadioButton ID="rbSemAll" runat="server" Text="All" GroupName="gpSemester" AutoPostBack="true"/>
                                                    </td>
                                                    <td style="width: 85px; text-align: right">
                                                        <asp:RadioButton ID="rbSemIndividual" runat="server" GroupName="gpSemester" Text="Individual" AutoPostBack="true" />
                                                    </td>
                                                    <td style="width: 100px">&nbsp;
                                                        <asp:TextBox ID="txtSemster" runat="server" Width="18px" Visible="false"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 59px; text-align: right"></td>
                                                    <td style="width: 85px; text-align: right"></td>
                                                    <td style="width: 100px"></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 35%;">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="width: 5px; text-align: right; height: 14px;"></td>
                                                    <td style="width: 100%; height: 14px;">&nbsp;<asp:Label ID="Label12" runat="server" Text="Student Category" Width="100px"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 5px; text-align: right; height: 32px;"></td>
                                                    <td style="width: 100%; height: 32px;">
                                                        <asp:DropDownList ID="ddlStudentType" runat="server" AppendDataBoundItems="True"
                                                            Width="144px">
                                                            <asp:ListItem Value="-1">--Select--</asp:ListItem>
                                                        </asp:DropDownList>&nbsp;                                                        
                                                        <asp:HiddenField ID="hfStdCategory" runat="server" Value="" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 35%;">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="width: 3px; text-align: right"></td>
                                                    <td style="width: 100px"></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 3px; text-align: right; height: 29px;"></td>
                                                    <td style="width: 100px; text-align: center; height: 29px;">
                                                        <asp:Button ID="btnUpdateCri" runat="server" Height="27px" Text="Update Criteria"
                                                            Width="177px" />
                                                        <br />
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
            </table>
        </asp:View>
        <asp:View ID="View3" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%">&nbsp;
                        <asp:Panel ID="pnlView" runat="server" Height="100%" Visible="False" Width="100%">
                            <table style="width: 100%;">
                                 <tr>
                                <td>
                                    <asp:Button ID="btnLoadFeeView3" runat="server" Text="Load Fee Type" Visible="false" />
                                     &nbsp;
                                     <asp:Label ID="lblSemyear" runat="server" Text="   Semester Year" Width="82px"></asp:Label>
                                     <asp:DropDownList ID="ddlsemyear" runat="server" AppendDataBoundItems="true" Width="150px">
                                                        
                                                        </asp:DropDownList>
                                     &nbsp;
                                     <asp:Label ID="lblTransstatus" runat="server" Text="   Status" Width="82px"></asp:Label>
                                     <asp:DropDownList ID="ddltransstatus" runat="server" AppendDataBoundItems="true" Width="150px">
                                                        <asp:ListItem Value="-1">All</asp:ListItem>
                                             <asp:ListItem Value="Open">Open</asp:ListItem>
                                                <asp:ListItem Value="Closed">Closed</asp:ListItem>
                                                <%--<asp:ListItem Value="3">3</asp:ListItem>--%>
                                                        </asp:DropDownList>
                                </td>
                            </tr>
                                <tr>
                                    <td style="width: 100%; vertical-align: top;">
                                        <table>
                                            <tr>
		                                        <td>
			                                         <asp:Label ID="Label2" runat="server" Text="Add Student" Width="68px"></asp:Label>
			                                         <asp:ImageButton ID="ibtnStudent" runat="server" Height="16px" ImageUrl="~/images/find_img.png" ToolTip="Select Student" Width="16px" />
		                                        </td>
		                                        <td style="width:50px">
			                                        <asp:Button ID="btnupload" runat="server" Text="Load template" Visible="false" />
		                                        </td>
	                                        </tr>
                                        </table>
                                        <br />
                                        <asp:CheckBox ID="chkStudent" runat="server" AutoPostBack="True" OnCheckedChanged="chkStudent_CheckedChanged" Visible="true" Text="Select All" />
                                        <asp:DataGrid ID="dgStudent" runat="server" AutoGenerateColumns="False" Width="100%" DataKeyField="MatricNo">
                                            <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                            <SelectedItemStyle CssClass="dgSelectedItemStyle" />
                                            <AlternatingItemStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                            <ItemStyle CssClass="dgItemStyle" />
                                            <HeaderStyle BackColor="#CDD7EE" CssClass="dgHeaderStyle" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Size="Medium" Font-Strikeout="False" Font-Underline="False" />
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Select">
                                                    <ItemTemplate>
                                                        &nbsp;<asp:CheckBox ID="chk" runat="server" AutoPostBack="True" Checked="true" OnCheckedChanged="chk_CheckedChanged" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="MatricNo" HeaderText="Matric No"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="StudentName" HeaderText="Student Name"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="ProgramID" HeaderText="Program Id"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="CurrentSemester" HeaderText="Semester"></asp:BoundColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View4" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%">&nbsp;<asp:Panel ID="pnlViewChange" runat="server" Height="100%" Width="100%" Visible="False">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <asp:Button ID="btnLoadFeeType" runat="server" Text="Load Fee Type" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%; vertical-align: top;">
                                    <asp:CheckBox ID="chkStudentProg" runat="server" OnCheckedChanged="chkStudentProg_CheckedChanged"
                                        AutoPostBack="True" Text="Select All" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:DataGrid ID="dgStudentProg" runat="server" AutoGenerateColumns="False" Width="100%">
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
                                                            &nbsp;<asp:CheckBox ID="chkStuProg" runat="server" AutoPostBack="True" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:BoundColumn DataField="MatricNo" HeaderText="Matric No"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="StudentName" HeaderText="Student Name"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="OldProgramID" HeaderText="Previous Program" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CurProgramID" HeaderText="Current Program" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="OldCrditHrs" HeaderText="Previous Credit Hr" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="SASI_CrditHrs" HeaderText="Current Credit Hr" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CrditHrDiff" HeaderText="Difference" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="ProgramID" HeaderText="Program" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="CurrentSemester" HeaderText="Semester" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Sako_Code" HeaderText="Sako Code" Visible="false"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Sabk_Code" HeaderText="Sabk Code" Visible="false"></asp:BoundColumn>
                                                </Columns>
                                            </asp:DataGrid><br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View5" runat="server">
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%">
                        <asp:Panel ID="pnlUploadFile" runat="server" Height="100%" Width="100%" Visible="False">
                            <table style="width: 100%;">
                                <tr>
		                            <td>
			                            <table id="tblUploadFile" runat="server" title="Upload File">
				                            <tr>
                                                <td></td>
					                            <td>
						                             <asp:FileUpload ID="UploadFile" ToolTip="Upload File" runat="server"/>
					                            </td>
				                            </tr>
				                            <tr>
                                                <td></td>
					                            <td>
						                            <asp:Button ID="UploadBTN" runat="server" Text="Upload File"></asp:Button>
					                            </td>
				                            </tr>
			                            </table>
		                            </td>
                                </tr>
                                <tr>
                                    <td style="width: 100%; vertical-align: top;">
                                        <asp:DataGrid ID="dgUploadFile" runat="server" AllowSorting="True" OnSortCommand="Sort_Grid"  
                                            AutoGenerateColumns="False" Width="100%">
                                            <FooterStyle CssClass="dgFooterStyle" Height="20px" />
                                            <SelectedItemStyle CssClass="dgSelectedItemStyle" />
                                            <AlternatingItemStyle BackColor="Beige" CssClass="dgAlternatingItemStyle" Font-Bold="False"
                                                Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                            <ItemStyle CssClass="dgItemStyle" />
                                            <HeaderStyle BackColor="#CDD7EE" ForeColor="White" CssClass="dgHeaderStyle" Font-Bold="True" Font-Italic="False"
                                                Font-Overline="False" Font-Size="Medium" Font-Strikeout="False" Font-Underline="False" />
                                            <Columns>
                                                <asp:BoundColumn DataField="MatricNo" HeaderText="Matric No" SortExpression="MatricNo"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="StudentName" HeaderText="Student Name" SortExpression="StudentName"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="FeeCode" HeaderText="Fee Code" SortExpression="FeeCode"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="FeeDesc" HeaderText="Fee Description" SortExpression="FeeDesc"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Amount" HeaderText="Amount" SortExpression="Amount"></asp:BoundColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                        <br />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
    <asp:Button ID="btnHidden" runat="Server" OnClick="btnHidden_Click" Style="display: none" />

    <asp:HiddenField ID="MenuId" runat="server" />
    <asp:HiddenField ID="GLflagTrigger" runat="server" />
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