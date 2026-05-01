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

            .section-wrapper {
        width: 100%;
        margin-bottom: 16px;
    }

    .section-title {
        font-family: Arial;
        font-size: 16px;
        font-weight: bold;
        color: #2f4f6f;
        margin: 0 0 6px 8px;
    }

    .section-panel {
        border: 1px solid #cfd8e3;
        border-radius: 14px;
        padding: 14px;
        background-color: #ffffff;
        box-sizing: border-box;
    }

    .items-links-row {
        margin-bottom: 12px;
    }

    .items-grid-wrap {
        display: flex;
        flex-direction: row;
        justify-content: flex-start;
        overflow-x: auto;
    }

    .exp-rdc-row {
        width: 100%;
        margin-top: 10px;
        display: flex;
        gap: 16px;
        align-items: flex-start;
        text-align: left;
        box-sizing: border-box;
    }

    .exp-rdc-box {
        width: 66.6667%;
        text-align: left;
        box-sizing: border-box;
    }

    .calc-box {
        width: 33.3333%;
        text-align: left;
        box-sizing: border-box;
    }

    .exp-rdc-link {
        display: inline-block;
        margin-bottom: 10px;
    }

    .exp-rdc-grid,
    .calc-grid {
        border-collapse: collapse;
        table-layout: fixed;
        width: 100%;
    }

    .exp-rdc-grid .col-action {
        width: 36px;
        min-width: 36px;
        max-width: 36px;
        white-space: nowrap;
        text-align: center;
        padding-left: 6px;
        padding-right: 6px;
        box-sizing: border-box;
    }

    .exp-rdc-grid .col-action input,
    .exp-rdc-grid .col-action img {
        display: block;
        margin: 0 auto;
    }

    .exp-rdc-grid .col-80 {
        width: 80px;
        min-width: 80px;
        max-width: 80px;
        text-align: center;
    }

    .calc-grid .col-payment {
        width: 65%;
    }

    .calc-grid .col-amount {
        width: 35%;
        text-align: center;
    }
    .btn-action {
        background: transparent;
        color: #666;
        border: 1.5px solid #aaa;
        border-radius: 0;
        padding: 3px 18px;
        font-family: Arial;
        font-size: 13px;
        cursor: pointer;
        margin: 0;
        margin-right: -1px;
        position: relative;
        transition: background-color 0.15s, color 0.15s, border-color 0.15s;
    }

    .btn-action:hover {
        background-color: #e8e8e8;
        color: #333;
        border-color: #888;
        z-index: 1;
    }

    .btn-action:active {
        background-color: #d0d0d0;
    }
</style>

    <script type="text/javascript">

        function editCell(wrapper) {
            var lbl = wrapper.querySelector("span");
            var txt = wrapper.querySelector("input[type=text], input:not([type=hidden])");
            var hfv = wrapper.querySelector("input[type=hidden]");

            if (txt.style.display === "inline") return;

            lbl.style.display = "none";
            txt.style.display = "inline";
            if (hfv) hfv.value = "1";  // mark cell as open
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
            var hfv = wrapper.querySelector("input[type=hidden]");
            if (hfv) hfv.value = "0";  // mark cell as closed
            //====================================================
            // Get cell info
            var rowIndex = wrapper.getAttribute("data-rowindex");
            var columnName = wrapper.getAttribute("data-column");

            // calculateColumn and iterateRowCells are pure DOM operations — run immediately.
            calculateColumn(colIndex);
            iterateRowCells(rowIndex);
            refreshTotalIn();

            // SaveCell must complete first so the session has the new value
            // before RecalculateSubtotal reads it.
            PageMethods.SaveCell(
                parseInt(rowIndex),
                columnName,
                textbox.value,
                function () {
                    // Success: session is now updated — safe to recalculate.
                    recalculateSubtotal();
                },
                function (error) {
                    console.log('SaveCell error: ' + error.get_message());
                }
            );
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

                // Skip first 4 columns
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
                        MasterTotalProfit = MasterTotalProfit + value * value2;
                    }
                }
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

            // calculateColumn is a pure DOM operation — run immediately.
            calculateColumn(colIndex);

            if (level == 1) {
                iterateThroughAllCells();
            }

            // SaveHeader must complete first so the session has the new value
            // before RecalculateSubtotal reads it.
            PageMethods.SaveHeader(
                parseInt(colIndex),
                parseInt(level),
                textbox.value,
                function () {
                    // Success: session is now updated — safe to recalculate.
                    recalculateSubtotal();
                },
                function (err) { console.log(err.get_message()); }
            );

        }

        function iterateThroughAllCells() {
            var rowCells = document.querySelectorAll(
                ".cell-wrapper[data-columnindex='4']"
            );
            rowCells.forEach(function (cellWrapper) {
                var rowIndex = cellWrapper.getAttribute("data-rowindex");

                if (parseInt(rowIndex) > 0) {
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

        // =====================================================
        // Editable NetValue cell in GridView2
        // =====================================================
        function editNetCell(div) {
            var span = div.querySelector("span");
            var txt = div.querySelector("input");
            if (txt.style.display === "inline") return;
            txt.value = span.innerText.trim();
            span.style.display = "none";
            txt.style.display = "inline";
            txt.focus();
            txt.select();
        }

        function saveNetValue(input) {
            var div = input.parentElement;
            var span = div.querySelector("span");
            var guid = div.getAttribute("data-rowguid");
            var val = input.value.trim();

            // 1. Update the GridView2 cell display
            span.innerText = val;
            input.style.display = "none";
            span.style.display = "inline";

            // 2. Update the matching rptSummary value span by rowguid
            var summaryRow = document.querySelector(
                "[data-rowguid='" + guid + "'].rpt-summary-row"
            );
            if (summaryRow) {
                var summaryVal = summaryRow.querySelector(".rpt-summary-value");
                if (summaryVal) summaryVal.innerText = parseFloat(val).toFixed(3);
            }

            // 3. Persist to server, then refresh Grand Total
            PageMethods.SaveNetValue(guid, val,
                function () { refreshGrandTotal(); },
                function (e) { console.log("SaveNetValue error: " + e.get_message()); }
            );
        }

        // =====================================================
        // Sums all rptSummary values + Subtotal → Grand Total,
        // then mirrors it to Label8.
        // =====================================================
        function refreshGrandTotal() {
            var total = 0;

            document.querySelectorAll(".rpt-summary-value").forEach(function (el) {
                total += parseFloat(el.innerText) || 0;
            });

            var subEl = document.getElementById("litSubtotal");
            if (subEl) total += parseFloat(subEl.innerText) || 0;

            var fmt = total.toFixed(3);

            var gtEl = document.getElementById("litGrandTotal");
            if (gtEl) gtEl.innerText = fmt;

            var lb8 = document.getElementById("Label8");
            if (lb8) lb8.innerText = fmt;

            refreshTotalIn();
        }

        // =====================================================
        // Sums Deposit - Debt across all member rows → Total In.
        // Reads live DOM: instant on cell edit / row delete.
        // =====================================================
        function refreshTotalIn() {
            var deposit = 0;
            var debt = 0;

            document.querySelectorAll(".cell-wrapper[data-column='Deposit'] span").forEach(function (el) {
                deposit += parseFloat(el.innerText) || 0;
            });

            document.querySelectorAll(".cell-wrapper[data-column='Debt'] span").forEach(function (el) {
                debt += parseFloat(el.innerText) || 0;
            });

            var fmt = (deposit - debt).toFixed(3);

            var tiEl = document.getElementById("litTotalIn");
            if (tiEl) tiEl.innerText = fmt;

            var lb10 = document.getElementById("Label10");
            if (lb10) lb10.innerText = fmt;

            updateBalanceBadge();
        }

        // =====================================================
        // Compares Grand Total (Label8) vs Total In (Label10).
        // Updates badge text and background — works both from JS
        // (client-side changes) and is also set server-side on
        // every postback so it survives GridView2 row delete.
        // =====================================================
        function updateBalanceBadge() {
            var badge = document.getElementById("balanceBadge");
            if (!badge) return;

            var gt = parseFloat((document.getElementById("Label8") || {}).innerText) || 0;
            var ti = parseFloat((document.getElementById("Label10") || {}).innerText) || 0;

            if (Math.abs(gt - ti) < 0.0005) {
                badge.innerText = "Balanced";
                badge.style.backgroundColor = "#add8e6";
            } else {
                badge.innerText = "Unbalanced";
                badge.style.backgroundColor = "#ffb3b3";
            }
        }

        // =====================================================
        // Recalculates Subtotal live without a full postback.
        // Called from saveCell() and saveHeader().
        // =====================================================
        function recalculateSubtotal() {
            PageMethods.RecalculateSubtotal(
                function (result) {
                    var el = document.getElementById('litSubtotal');
                    if (el) el.innerText = result;
                    refreshGrandTotal();
                },
                function (err) {
                    console.log('Subtotal error: ' + err.get_message());
                }
            );
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
                                        <div class="outerBlock">

    <div class="row" style="text-align:left; margin-left:0; padding-left:0;">
        <asp:Button ID="btnLoad" runat="server" Text="Load" CssClass="btn-action" OnClick="btnLoad_Click" />
        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn-action" OnClick="btnSave_Click" />
    </div>

    <div class="row">
        <span class="labelCol">
            <asp:LinkButton ID="LinkButton3" runat="server" Font-Names="Arial" >
                Vender
            </asp:LinkButton>
        </span>
        <asp:TextBox ID="TextBox1" runat="server" CssClass="txtbox" Width="300px" ReadOnly="true"></asp:TextBox>
        <asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
        <asp:Label ID="lblVendorText" runat="server" Text="Label"></asp:Label>
        <asp:Label ID="lblVendorValue" runat="server" Text="Label"></asp:Label>
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
<div style="display:flex; align-items:stretch; gap:16px;">
    <div>
        <div class="row">
            <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="20px" Text="Grande Total:" style="display:inline-block; width:160px;"></asp:Label>
            <asp:Label ID="Label8" runat="server" ClientIDMode="Static" Font-Bold="True" Font-Names="Arial" Font-Size="20px" Text="0.000" style="margin-left:12px;"></asp:Label>
        </div>
        <div class="row">
            <asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="20px" Text="Total In:" style="display:inline-block; width:160px;"></asp:Label>
            <asp:Label ID="Label10" runat="server" ClientIDMode="Static" Font-Bold="True" Font-Names="Arial" Font-Size="20px" Text="0.000" style="margin-left:12px;"></asp:Label>
        </div>
    </div>
    <asp:Label ID="balanceBadge" runat="server" ClientIDMode="Static"
        Text="Balanced"
        style="display:flex; align-items:center; justify-content:center; padding:0 22px; border-radius:18px; background-color:#add8e6; font-family:Arial; font-size:18px; font-weight:bold; color:#1a1a1a; min-width:120px; text-align:center; transition:background-color 0.3s;">
    </asp:Label>
</div>
    <br />
<div class="section-wrapper">
    <div class="section-title">Items</div>
    <div class="section-panel">
        <div class="row items-links-row">
            <asp:LinkButton ID="lnkBtnAddMembers" runat="server" Font-Names="Arial" Font-Size="14px">Add Members
            </asp:LinkButton>

            <span style="display:inline-block; width:15px;"></span>

            <asp:LinkButton ID="lnkBttnAddItems" runat="server" Font-Names="Arial" Font-Size="14px">Add Items</asp:LinkButton>

            <span style="display:inline-block; width:15px;"></span>

            <%-- Button2 uses ClientIDMode="Static" so its ID is always "Button2" in the DOM --%>
            <asp:Button ID="Button2" runat="server" ClientIDMode="Static" Style="display:none;" />

            <asp:LinkButton ID="LinkButton4"
                            runat="server"
                            Font-Names="Arial"
                            Font-Size="14px"
                            OnClientClick="document.getElementById('Button2').click(); return false;">
                Add one Column
            </asp:LinkButton>
        </div>

        <div class="items-grid-wrap">
            <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" Font-Names="Arial" AutoGenerateColumns="False" OnRowCreated="GridView1_RowCreated" Font-Size="12px">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Trash16x16.png" OnClick="ImageButton2_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>
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
    </div>
</div>
                                       <div class="row exp-rdc-row">
    <div class="exp-rdc-box">
        <div class="section-title">Other Additions and Reductions</div>
        <div class="section-panel">
            <asp:LinkButton ID="btnAddExpRdc"
                            runat="server"
                            CssClass="exp-rdc-link"
                            Font-Names="Arial"
                            Font-Size="14px">
                Add Other Expenses/Reduction
            </asp:LinkButton>

            <asp:GridView ID="GridView2"
                  runat="server"
                  CssClass="exp-rdc-grid"
                  CellPadding="4"
                  ForeColor="#333333"
                  Font-Names="Arial"
                  Font-Size="12px"
                  AutoGenerateColumns="False"
                  OnRowCommand="GridView2_RowCommand" ShowHeaderWhenEmpty="True">
        <AlternatingRowStyle BackColor="White" />

        <Columns>
            <asp:TemplateField HeaderStyle-CssClass="col-action"
                               ItemStyle-CssClass="col-action"
                               HeaderStyle-Width="36px"
                               ItemStyle-Width="36px">
                <ItemTemplate>
                    <asp:ImageButton ID="ImageButton1"
                                     runat="server"
                                     ImageUrl="~/Images/Trash16x16.png"
                                     CommandName="DeleteRow"
                                     CommandArgument='<%# Eval("__RowGuid") %>'
                                     CausesValidation="False"
                                     ToolTip="Delete" />
                </ItemTemplate>

    <HeaderStyle CssClass="col-action"></HeaderStyle>

    <ItemStyle CssClass="col-action"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField DataField="adjusmentName"
                            HeaderText="Title"
                            HeaderStyle-CssClass="col-80"
                            ItemStyle-CssClass="col-80" >

    <HeaderStyle CssClass="col-80"></HeaderStyle>

    <ItemStyle CssClass="col-80"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="adjusmentType"
                            HeaderText="Inc or Dec"
                            HeaderStyle-CssClass="col-80"
                            ItemStyle-CssClass="col-80" >

    <HeaderStyle CssClass="col-80"></HeaderStyle>

    <ItemStyle CssClass="col-80"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="adjusmentCalculation"
                            HeaderText="Fixed or %"
                            HeaderStyle-CssClass="col-80"
                            ItemStyle-CssClass="col-80" >

    <HeaderStyle CssClass="col-80"></HeaderStyle>

    <ItemStyle CssClass="col-80"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="CalculationAmount"
                            HeaderText="Amount"
                            HeaderStyle-CssClass="col-80"
                            ItemStyle-CssClass="col-80" >
    <HeaderStyle CssClass="col-80"></HeaderStyle>

    <ItemStyle CssClass="col-80"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Distrbution" HeaderText="Distribution" />

            <asp:TemplateField HeaderText="Net Value"
                               HeaderStyle-CssClass="col-80"
                               ItemStyle-CssClass="col-80">
                <HeaderStyle CssClass="col-80"></HeaderStyle>
                <ItemStyle CssClass="col-80" HorizontalAlign="Center"></ItemStyle>
                <ItemTemplate>
                    <div class="gv2-net-cell"
                         data-rowguid='<%# Eval("__RowGuid") %>'
                         onclick="editNetCell(this)"
                         style="cursor:pointer; padding:2px; text-align:center;">
                        <span><%# Eval("NetValue") %></span>
                        <input type="text"
                               style="display:none; width:80px; text-align:right;"
                               onblur="saveNetValue(this)"
                               onkeydown="return handleEnter(event, this);" />
                    </div>
                </ItemTemplate>
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
    </div>
    <div class="calc-box">
        <div class="section-title">Calculations</div>
        <div class="section-panel">
            <%-- Calculations summary panel: div-based, Arial font, no <% %> code blocks --%>
            <div style="width:300px; font-family:Arial; font-size:16px;">
                <asp:Repeater ID="rptSummary" runat="server">
                    <ItemTemplate>
                        <div class="rpt-summary-row"
                             style="display:flex; justify-content:space-between; padding:2px 0; font-size:15px;"
                             data-rowguid='<%# Eval("RowGuid") %>'>
                            <span><%# Eval("Label") %></span>
                            <span class="rpt-summary-value" style="text-align:right;"><%# Eval("Value", "{0:0.000}") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div style="display:flex; justify-content:space-between; padding:2px 0; font-size:15px;">
                    <span>Subtotal</span>
                    <%-- asp:Label renders as <span id="litSubtotal"> so JS getElementById works --%>
                    <asp:Label ID="litSubtotal" runat="server" ClientIDMode="Static" Text="0.000" />
                </div>

                <hr />

                <div style="display:flex; justify-content:space-between; padding:2px 0; font-weight:bold; font-size:17px;">
                    <span>Grand Total</span>
                    <asp:Label ID="litGrandTotal" runat="server" ClientIDMode="Static" Text="0.000" />
                </div>

                <div style="display:flex; justify-content:space-between; padding:2px 0; font-size:15px;">
                    <span>Total In</span>
                    <asp:Label ID="litTotalIn" runat="server" ClientIDMode="Static" Text="0.000" />
                </div>
            </div>
        </div>
    </div>
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
