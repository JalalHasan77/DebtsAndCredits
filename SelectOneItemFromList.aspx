<%@ Page Language="VB" AutoEventWireup="true" CodeFile="SelectOneItemFromList.aspx.vb" Inherits="SelectOneItemFromList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select Vendor</title>
    <style type="text/css">
        html, body, form {
            width: 100%;
            height: 100%;
            margin: 0;
        }

        body {
            background: transparent;
            overflow: hidden;
            font-family: Arial, sans-serif;
        }

        .vendor-card {
            width: 100%;
            height: 100%;
            box-sizing: border-box;
            padding: 20px 22px;
            background: #ffffff;
            display: flex;
            flex-direction: column;
        }

        .vendor-title {
            font-size: 20px;
            font-weight: 700;
            color: #111827;
            margin-bottom: 14px;
            flex: 0 0 auto;
        }

        .vendor-search {
            flex: 0 0 auto;
            margin-bottom: 12px;
        }

        .vendor-search-box {
            width: 100%;
            height: 36px;
            padding: 6px 12px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            background: #f8fafc;
            font-size: 14px;
            color: #111827;
            box-sizing: border-box;
            outline: none;
        }

        .vendor-search-box:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
            background: #ffffff;
        }

        .vendor-list-wrap {
            flex: 1 1 auto;
            overflow-y: auto;
            overflow-x: hidden;
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            background: #f8fafc;
            padding: 10px;
            box-sizing: border-box;
        }

        .vendor-option {
            display: block;
            width: 100%;
            text-align: left;
            padding: 10px 12px;
            margin: 0 0 8px 0;
            background: #ffffff;
            border: 1px solid #dbe3f0;
            border-radius: 8px;
            color: #1f2937;
            font-size: 14px;
            cursor: pointer;
            box-sizing: border-box;
            transition: background-color 0.15s ease, border-color 0.15s ease, box-shadow 0.15s ease;
        }

        .vendor-option:last-child {
            margin-bottom: 0;
        }

        .vendor-option:hover {
            background: #eef4ff;
            border-color: #93c5fd;
        }

        .vendor-option.selected {
            background: #dbeafe;
            border-color: #2563eb;
            box-shadow: inset 0 0 0 1px #2563eb;
            color: #1d4ed8;
            font-weight: 700;
        }

        .vendor-empty {
            display: none;
            padding: 12px;
            text-align: center;
            color: #6b7280;
            font-size: 14px;
        }

        .vendor-highlight {
            background-color: #fef08a;
            color: #111827;
            font-weight: 700;
            padding: 0 1px;
            border-radius: 3px;
        }

        .vendor-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 18px;
            padding-top: 14px;
            border-top: 1px solid #e5e7eb;
            flex: 0 0 auto;
        }

        .btn-modern {
            min-width: 90px;
            height: 36px;
            padding: 0 16px;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
        }

        .btn-add {
            background: #2563eb;
            color: #ffffff;
        }

        .btn-add:hover {
            background: #1d4ed8;
        }

        .btn-add:disabled {
            background: #93c5fd;
            cursor: not-allowed;
        }

        .btn-cancel {
            background: #e5e7eb;
            color: #374151;
        }

        .btn-cancel:hover {
            background: #d1d5db;
        }
    </style>

    <script type="text/javascript">
        function htmlEncode(value) {
            var div = document.createElement('div');
            div.textContent = value;
            return div.innerHTML;
        }

        function escapeRegExp(text) {
            return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        }

        function buildHighlightedHtml(text, keyword) {
            if (!keyword) {
                return htmlEncode(text);
            }

            var regex = new RegExp('(' + escapeRegExp(keyword) + ')', 'ig');
            var parts = text.split(regex);
            var html = '';

            for (var i = 0; i < parts.length; i++) {
                var part = parts[i];
                if (part.toLowerCase() === keyword.toLowerCase()) {
                    html += '<span class="vendor-highlight">' + htmlEncode(part) + '</span>';
                } else {
                    html += htmlEncode(part);
                }
            }

            return html;
        }

        function selectVendorOption(element, value, text) {
            var options = document.querySelectorAll('.vendor-option');
            for (var i = 0; i < options.length; i++) {
                options[i].classList.remove('selected');
            }

            element.classList.add('selected');
            document.getElementById('<%= hdnSelectedVendorValue.ClientID %>').value = value || '';
            document.getElementById('<%= hdnSelectedVendorText.ClientID %>').value = text || '';

            var btnAdd = document.getElementById('btnAddVendor');
            if (btnAdd) {
                btnAdd.disabled = false;
            }
        }

        function confirmAddVendor() {
            var value = document.getElementById('<%= hdnSelectedVendorValue.ClientID %>').value || '';
            var text = document.getElementById('<%= hdnSelectedVendorText.ClientID %>').value || '';

            if (!value || !text) {
                alert('Please select a vendor first.');
                return false;
            }

            if (window.parent && typeof window.parent.receiveVendorValue === 'function') {
                window.parent.receiveVendorValue(value, text);
            }

            return false;
        }

        function cancelVendorSelection() {
            if (window.parent && typeof window.parent.closeVendorDialog === 'function') {
                window.parent.closeVendorDialog();
            }
            return false;
        }

        function filterVendors() {
            var txt = document.getElementById('<%= txtSearchVendor.ClientID %>');
            var keyword = txt ? txt.value.toLowerCase().trim() : '';
            var items = document.querySelectorAll('.vendor-option');
            var visibleCount = 0;

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var originalText = item.getAttribute('data-text') || '';
                var isMatch = (keyword === '' || originalText.toLowerCase().indexOf(keyword) > -1);

                item.style.display = isMatch ? '' : 'none';
                item.innerHTML = buildHighlightedHtml(originalText, isMatch ? keyword : '');

                if (isMatch) {
                    visibleCount++;
                }
            }

            var empty = document.getElementById('vendorEmptyState');
            if (empty) {
                empty.style.display = visibleCount === 0 ? 'block' : 'none';
            }
        }

        function initializeVendorPopup() {
            var btnAdd = document.getElementById('btnAddVendor');
            if (btnAdd) {
                btnAdd.disabled = true;
            }
            filterVendors();
        }
    </script>
</head>
<body onload="initializeVendorPopup();">
    <form id="form1" runat="server">
        <div class="vendor-card">
            <div class="vendor-title">Select Vendor</div>

            <div class="vendor-search">
                <asp:TextBox ID="txtSearchVendor" runat="server" CssClass="vendor-search-box" autocomplete="off"
                    placeholder="Search vendor..." onkeyup="filterVendors();"></asp:TextBox>
            </div>

            <div class="vendor-list-wrap" id="vendorListWrap">
                <asp:Repeater ID="rptVendorOptions" runat="server">
                    <ItemTemplate>
                        <div class="vendor-option"
                             data-value="<%# Server.HtmlEncode(Convert.ToString(Eval("OptionValue"))) %>"
                             data-text="<%# Server.HtmlEncode(Convert.ToString(Eval("OptionName"))) %>"
                             onclick="selectVendorOption(this, '<%# System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("OptionValue"))) %>', '<%# System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("OptionName"))) %>');">
                            <%# Server.HtmlEncode(Convert.ToString(Eval("OptionName"))) %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div id="vendorEmptyState" class="vendor-empty">No vendors found.</div>
            </div>

            <asp:HiddenField ID="hdnSelectedVendorValue" runat="server" />
            <asp:HiddenField ID="hdnSelectedVendorText" runat="server" />

            <div class="vendor-actions">
                <button id="btnAddVendor" type="button" class="btn-modern btn-add" onclick="return confirmAddVendor();">Add</button>
                <button id="btnCancelVendor" type="button" class="btn-modern btn-cancel" onclick="return cancelVendorSelection();">Cancel</button>
            </div>
        </div>
    </form>
</body>
</html>
