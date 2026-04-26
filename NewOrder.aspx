<%@ Page Language="VB" AutoEventWireup="true" Codefile="NewOrder.aspx.vb" Inherits="NewOrder"%>
<%@ Register assembly="ServerControl1" namespace="ServerControl1" tagprefix="cc1" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="assets/jquery-ui-1.13.2.min.css" />
    <link rel="stylesheet" href="assets/jquery.timepicker-1.3.5.min.css" />
    <style type="text/css">
        .auto-style2 {
            width: 100%;
            border-collapse: collapse;
        }

        .auto-style3 {
            width: 90%;
            border-collapse: collapse;
        }

        .cell-wrapper {
            width: 100%;
            height: 100%;
            cursor: pointer;
            padding: 4px;
        }

        .cell-wrapper:hover {
            background-color: #f5f5f5;
        }

        .txtbox {
            border-radius: 5px;
            height: 25px;
            background: #f2f2f2;
            border: 1px solid #676767;
            width: 100%;
        }

        .outerBlock {
            text-align: left;
            width: 100%;
        }

        .row {
            margin-bottom: 8px;
            text-align: left;
        }

        .labelCol {
            display: inline-block;
            width: 100px;
            vertical-align: middle;
        }

        .spacer {
            display: inline-block;
            width: 15px;
        }

        .auto-style4 {
            direction: ltr;
        }
</style>

    <script type="text/javascript">

        function editCell(wrapper) {
            var lbl = wrapper.querySelector("span");
            var txt = wrapper.querySelector("input");

            if (txt.style.display === "inline") return;

            lbl.style.display = "none";
            txt.style.display = "inline";
            txt.focus();
            txt.select();
        }

        function saveCell(textbox) {
            var wrapper = textbox.closest(".cell-wrapper");
            var colIndex = wrapper.getAttribute("data-columnindex");
            var lbl = textbox.previousElementSibling;
            lbl.innerText = textbox.value;
            textbox.style.display = "none";
            lbl.style.display = "inline";
            //alert('');
            //====================================================
            //====================================================
            // Get cell info
            var rowIndex = wrapper.getAttribute("data-rowindex");
            var columnName = wrapper.getAttribute("data-column");

            PageMethods.SaveCell(
                parseInt(rowIndex),
                columnName,
                textbox.value,
                function () {

                },
                function (error) {

                }
            );

            calculateColumn(colIndex);
            iterateRowCells(rowIndex);

        }

        function iterateRowCells(rowIndex) {
            rowIndex = rowIndex.toString();

            var rowCells = document.querySelectorAll(
                ".cell-wrapper[data-rowindex='" + rowIndex + "']"
            );
            var MasterTotalProfit;
            MasterTotalProfit = 0;

            rowCells.forEach(function (cellWrapper) {
                var TotalProfit;
                TotalProfit = 0;

                var colIndex = parseInt(cellWrapper.getAttribute("data-columnindex"));

                // ✅ Skip first 4 columns
                if (colIndex <= 3) return;

                var span = cellWrapper.querySelector("span");
                var input = cellWrapper.querySelector("input");

                var value = 0;

                if (span && span.innerText.trim() !== "") {
                    value = parseFloat(span.innerText) || 0;
                }
                else if (input && input.value.trim() !== "") {
                    value = parseFloat(input.value) || 0;

                }

                if (value != 0) {
                    // alert(value);
                    var cellWrapper2 = document.querySelector(
                        ".cell-wrapper[data-headercol='" + colIndex + "'][data-headerlevel='1']"
                    );
                    var value2;
                    var span2 = cellWrapper2.querySelector("span");
                    var input2 = cellWrapper2.querySelector("input");
                    if (span2 && span2.innerText.trim() !== "") {
                        value2 = parseFloat(span2.innerText) || 0;
                    }
                    else if (input2 && input2.value.trim() !== "") {
                        value2 = parseFloat(input2.value) || 0;
                    }

                    if (value2 != 0) {
                        //  alert('Net profit' + value * value2);
                        MasterTotalProfit = MasterTotalProfit + value * value2;
                    }
                }
                // 👉 Your calculation here
            });

            var profitCell = document.querySelector(
                ".cell-wrapper[data-rowindex='" + rowIndex + "'][data-columnindex='3'] span"
            );
            var columnName = "Profit";
            if (profitCell) {
                profitCell.innerText = MasterTotalProfit.toFixed(3);

                PageMethods.SaveCell(
                    parseInt(rowIndex),
                    columnName,
                    profitCell.innerText,
                    function () {
                    },
                    function (error) {
                    }
                );
            }
        }

        function iterateColumnCells(colIndex) {
            colIndex = colIndex.toString();

            var columnCells = document.querySelectorAll(
                ".cell-wrapper[data-columnindex='" + colIndex + "']"
            );

            columnCells.forEach(function (cellWrapper) {

                var rowIndex = cellWrapper.getAttribute("data-rowindex");

                var span = cellWrapper.querySelector("span");
                var input = cellWrapper.querySelector("input");

                var value = 0;

                if (span && span.innerText.trim() !== "")
                    value = parseFloat(span.innerText) || 0;
                else if (input && input.value.trim() !== "")
                    value = parseFloat(input.value) || 0;
                // 👉 Do your calculation here
            });
        }

        function handleEnter(e, textbox) {
            if (e.key === "Enter") {
                textbox.blur();
                return false;
            }
            return true;
        }

        function saveHeader(textbox) {

            var wrapper = textbox.closest(".cell-wrapper");
            var colIndex = wrapper.getAttribute("data-columnindex");
            var lbl = textbox.previousElementSibling;

            lbl.innerText = textbox.value;
            textbox.style.display = "none";
            lbl.style.display = "inline";

            var colIndex = wrapper.getAttribute("data-headercol");
            var level = wrapper.getAttribute("data-headerlevel");

            PageMethods.SaveHeader(
                parseInt(colIndex),
                parseInt(level),
                textbox.value,
                function () { },
                function (err) { console.log(err.get_message()); }
            );

            calculateColumn(colIndex);

            if (level == 1) {
                iterateThroughAllCells()
            }

        }

        function iterateThroughAllCells() {
            //====================
            var rowCells = document.querySelectorAll(
                ".cell-wrapper[data-columnindex='4']"
            );
            rowCells.forEach(function (cellWrapper) {
                var rowIndex = cellWrapper.getAttribute("data-rowindex");

                if (parseInt(rowIndex) > 0) {
                    // safe to execute
                    iterateRowCells(rowIndex);
                }
            })
        }

        function calculateColumn(colIndex) {

            var sum = 0;

            // Sum all data cells in this column
            var cells = document.querySelectorAll(
                ".data-cell[data-columnindex='" + colIndex + "'] input"
            );

            cells.forEach(function (txt) {
                var val = parseFloat(txt.value);
                if (!isNaN(val)) {
                    sum += val;
                }
            });

            // Get price header
            var priceInput = document.querySelector(
                ".cell-wrapper[data-columnindex='" + colIndex + "'][data-level='5'] input"
            );

            if (!priceInput) return;

            var price = parseFloat(priceInput.value);
            if (isNaN(price)) price = 0;

            var lcValue = sum * price;

            updateHeader(colIndex, '2', lcValue);
            updateHeader(colIndex, '3', sum);
        }

        function updateHeader(colIndex, level, lcValue) {

            var wrapper = document.querySelector(
                ".cell-wrapper[data-columnindex='" + colIndex + "'][data-level='" + level + "']"
            );

            var lcLabel = wrapper ? wrapper.querySelector("span") : null;

            if (lcLabel) {
                lcLabel.innerText = lcValue.toFixed(3);

                // Get Header info
                var headerColIndex = wrapper.getAttribute("data-headercol");
                var headerLevel = wrapper.getAttribute("data-headerlevel");

                PageMethods.SaveHeader(
                    parseInt(headerColIndex),
                    parseInt(headerLevel),
                    lcLabel.innerText,
                    function () { },
                    function (err) { console.log(err.get_message()); }
                );
            }
        }

        function initializeDateTimePickers() {
            var dateSelector = '#TextBox2';
            var timeSelector = '#TextBox3';

            if (window.jQuery) {
                $(dateSelector).datepicker({
                    dateFormat: 'yy-mm-dd',
                    changeMonth: true,
                    changeYear: true
                });

                $(timeSelector).timepicker({
                    timeFormat: 'H:i',
                    step: 1,
                    scrollDefault: 'now'
                });

                var now = new Date();
                var yyyy = now.getFullYear();
                var mm = String(now.getMonth() + 1).padStart(2, '0');
                var dd = String(now.getDate()).padStart(2, '0');
                var hh = String(now.getHours()).padStart(2, '0');
                var mi = String(now.getMinutes()).padStart(2, '0');

                if (!$(dateSelector).val()) {
                    $(dateSelector).val(yyyy + '-' + mm + '-' + dd);
                }

                if (!$(timeSelector).val()) {
                    $(timeSelector).val(hh + ':' + mi);
                }
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            initializeDateTimePickers();
        });
</script>
    <script src="assets/jquery-3.7.1.min.js"></script>
    <script src="assets/jquery-ui-1.13.2.min.js"></script>
    <script src="assets/jquery.timepicker-1.3.5.min.js"></script>

</head>
<body style="margin: 0px;">
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <table cellpadding="0" class="auto-style2">
            <tr>
                <td style="background-color: #3366FF">
                    <table cellpadding="15" cellspacing="15" class="auto-style2">
                        <tr>
                            <td style="width: 50%">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="24pt" Text="Debit/Credit Transaction" ForeColor="White"></asp:Label>
                            </td>
                            <td style="vertical-align: top; width: 50%;" align="right">
                            <cc1:anyObject ID="clTemp" runat="server" Visible="False" />
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <table cellpadding="10" cellspacing="10" class="auto-style3">
                                <tr>
                                    <td style="width: 20%; vertical-align: top; text-align: center;" class="auto-style4">
                                        <asp:Button ID="Button1" runat="server" Text="Button" />
                                        <asp:HiddenField ID="hfRowIndex" runat="server" />
                                        <asp:HiddenField ID="hfColumnName" runat="server" />
                                        <asp:HiddenField ID="hfNewValue" runat="server" />
                                        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                                        <asp:Button ID="btnTest" runat="server" Text="Test" OnClientClick="testCall(); return false;" />

                                        <br />
                                        <br />
        <span class="labelCol">
            <asp:LinkButton ID="LinkButton6" runat="server" Font-Names="Arial" >
                Vender
            </asp:LinkButton>
        </span>

                                    </td>
                                    <td style="width: 60%;align-items:flex-start" >
                                    <asp:Button ID="Button2" runat="server" Text="Button" />
                                     <br />                                        <div class="outerBlock">

    <div class="row">
        <span class="labelCol">
            <asp:LinkButton ID="LinkButton3" runat="server" Font-Names="Arial" >
                Vender
            </asp:LinkButton>
        </span>
        <asp:TextBox ID="TextBox1" runat="server" CssClass="txtbox" Width="300px" ReadOnly="true"></asp:TextBox>
        <asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
        <asp:HiddenField ID="hdnSelectedVendorValue" runat="server" />
        <asp:HiddenField ID="hdnSelectedVendorText" runat="server" />
    </div>

    <div class="row">
        <span class="labelCol">
            <asp:Label ID="Label3" runat="server" Text="Date" Font-Names="Arial"></asp:Label>
        </span>
        <asp:TextBox ID="TextBox2" runat="server" ClientIDMode="Static" CssClass="txtbox" Width="150px" placeholder="yyyy-MM-dd"></asp:TextBox>

        <span class="spacer"></span>

        <asp:Label ID="Label4" runat="server" Text="Time" Font-Names="Arial"></asp:Label>
        <asp:TextBox ID="TextBox3" runat="server" ClientIDMode="Static" CssClass="txtbox" Width="90px" placeholder="HH:mm"></asp:TextBox>
    </div>

    <div class="row">
        <span class="labelCol">
                <asp:Label ID="Label5" runat="server" Text="Number" Font-Names="Arial"></asp:Label>
        </span>
        <asp:TextBox ID="TextBox4" runat="server" CssClass="txtbox" Width="50px"></asp:TextBox>

    </div>

</div>
                                        <br />
<div class="row">
    <asp:LinkButton ID="btnAddExpRdc" runat="server" Font-Names="Arial" Font-Size="14px">
        Add Other Expenses/Reduction</asp:LinkButton>
    <br />

                                        <asp:GridView ID="GridView2" runat="server" CellPadding="4" ForeColor="#333333" Font-Names="Arial" Font-Size="12px">
                                            <AlternatingRowStyle BackColor="White" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="#EFF3FB" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                        </asp:GridView>
    <br />
        <asp:TextBox ID="TextBox5" runat="server" CssClass="txtbox" Width="300px" ReadOnly="true"></asp:TextBox>
        <asp:HiddenField ID="hdnSelectedVendorValue0" runat="server" />
        <asp:HiddenField ID="hdnSelectedVendorText0" runat="server" />
</div>
<br />
<div class="row">
    <asp:LinkButton ID="LinkButton2" runat="server" Font-Names="Arial" Font-Size="14px">
        Add Members
    </asp:LinkButton>

    <span style="display:inline-block; width:15px;"></span>

    <asp:LinkButton ID="LinkButton1" runat="server" Font-Names="Arial" Font-Size="14px">
        Add one Item
    </asp:LinkButton>

    <span style="display:inline-block; width:15px;"></span>

    <asp:LinkButton ID="LinkButton4" runat="server" Font-Names="Arial" Font-Size="14px">
        Add one Column
    </asp:LinkButton>
</div>

                                        <div style="display: flex;flex-direction: row; justify-content:flex-start">

                                        <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" Font-Names="Arial" AutoGenerateColumns="False" OnRowCreated="GridView1_RowCreated" Font-Size="12px">
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="MemberName" />
                                                <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <div class="cell-wrapper" onclick="editCell(this)">
                                                                <asp:Label ID="lblValue" runat="server"
                                                                    Text='' />

                                                                <asp:TextBox ID="txtValue" runat="server"
                                                                    Text=''
                                                                    Style="display:none; width:85%;"
                                                                    onblur="saveCell(this)"
                                                                    onkeydown="return handleEnter(event, this);" />
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="150px" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EditRowStyle BackColor="#2461BF" />
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="#EFF3FB" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                        </asp:GridView>
                                       </div>
                                    </td>
                                    <td style="width: 20%; vertical-align: top;">
                                        &nbsp;</td>
                        </tr>
                                </table>
                        </td>
                     </tr>
                        </table>
</form>
</body>
</html>

