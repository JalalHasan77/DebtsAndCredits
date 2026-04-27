<%@ Page Language="VB" AutoEventWireup="true" CodeFile="SelectOneItemFromListMultiColumns.aspx.vb" Inherits="SelectOneItemFromListMultiColumns" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select One Item From List</title>
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

        .select-card {
            width: 100%;
            height: 100%;
            box-sizing: border-box;
            padding: 20px 22px;
            background: #ffffff;
            display: flex;
            flex-direction: column;
        }

        .select-title {
            font-size: 20px;
            font-weight: 700;
            color: #111827;
            margin-bottom: 14px;
            flex: 0 0 auto;
        }

        .select-search {
            flex: 0 0 auto;
            margin-bottom: 12px;
        }

        .select-search-box {
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

        .select-search-box:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
            background: #ffffff;
        }

        .select-list-wrap {
            flex: 1 1 auto;
            overflow-y: auto;
            overflow-x: auto;
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            background: #f8fafc;
            box-sizing: border-box;
        }

        .select-table {
            width: 100%;
            min-width: 100%;
            border-collapse: collapse;
            table-layout: auto;
            font-size: 14px;
            color: #111827;
        }

        .select-table th {
            position: sticky;
            top: 0;
            z-index: 1;
            background: #e5eefc;
            color: #1f2937;
            text-align: left;
            font-weight: 700;
            padding: 10px 12px;
            border-bottom: 1px solid #cbd5e1;
            white-space: nowrap;
        }

        .select-table td {
            padding: 10px 12px;
            border-bottom: 1px solid #e5e7eb;
            vertical-align: middle;
            word-break: break-word;
        }

        .select-row {
            cursor: pointer;
            background: #ffffff;
        }

        .select-row:hover {
            background: #eef4ff;
        }

        .select-row.selected {
            background: #dbeafe;
        }

        .select-row.selected td {
            color: #1d4ed8;
            font-weight: 700;
        }

        .select-highlight {
            background-color: #fef08a;
            color: #111827;
            font-weight: 700;
            padding: 0 1px;
            border-radius: 3px;
        }

        .select-empty {
            display: none;
            padding: 18px;
            text-align: center;
            color: #6b7280;
            font-size: 14px;
        }

        .select-actions {
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

        .btn-cancel {
            background: #e5e7eb;
            color: #374151;
        }

        .btn-cancel:hover {
            background: #d1d5db;
        }
    </style>

    <script type="text/javascript">
        function closeParentPopup() {
            if (window.parent && typeof window.parent.closeVendorDialog === 'function') {
                window.parent.closeVendorDialog();
            }
            return false;
        }

        function htmlEncode(value) {
            var div = document.createElement('div');
            div.textContent = value || '';
            return div.innerHTML;
        }

        function escapeRegExp(text) {
            return (text || '').replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        }

        function buildHighlightedHtml(text, keyword) {
            text = text || '';
            if (!keyword) {
                return htmlEncode(text);
            }

            var regex = new RegExp('(' + escapeRegExp(keyword) + ')', 'ig');
            var parts = text.split(regex);
            var html = '';

            for (var i = 0; i < parts.length; i++) {
                var part = parts[i];
                if (part.toLowerCase() === keyword.toLowerCase()) {
                    html += '<span class="select-highlight">' + htmlEncode(part) + '</span>';
                } else {
                    html += htmlEncode(part);
                }
            }

            return html;
        }

        function selectItemRow(row, value, text) {
            var rows = document.querySelectorAll('.select-row');
            for (var i = 0; i < rows.length; i++) {
                rows[i].classList.remove('selected');
            }

            row.classList.add('selected');
            document.getElementById('<%= hdnSelectedValue.ClientID %>').value = value || '';
            document.getElementById('<%= hdnSelectedText.ClientID %>').value = text || '';
        }

        function filterItems() {
            var txt = document.getElementById('<%= TextBoxSearch.ClientID %>');
            var keyword = txt ? txt.value.toLowerCase().trim() : '';
            var rows = document.querySelectorAll('.select-row');
            var visibleCount = 0;

            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var searchText = row.getAttribute('data-searchtext') || '';
                var isMatch = keyword === '' || searchText.toLowerCase().indexOf(keyword) > -1;
                row.style.display = isMatch ? '' : 'none';

                var cells = row.querySelectorAll('td');
                for (var c = 0; c < cells.length; c++) {
                    var original = cells[c].getAttribute('data-original-text') || '';
                    cells[c].innerHTML = buildHighlightedHtml(original, isMatch ? keyword : '');
                }

                if (isMatch) {
                    visibleCount++;
                }
            }

            var empty = document.getElementById('emptyState');
            if (empty) {
                empty.style.display = visibleCount === 0 ? 'block' : 'none';
            }
        }

        function addSelectedItem() {
            var value = document.getElementById('<%= hdnSelectedValue.ClientID %>').value || '';
            var text = document.getElementById('<%= hdnSelectedText.ClientID %>').value || '';

            if (!value) {
                alert('Please select one item first.');
                return false;
            }

            if (window.parent && typeof window.parent.receiveVendorValue === 'function') {
                window.parent.receiveVendorValue(value, text);
            }

            return false;
        }

        function initializeSelectOneItemFromList() {
            filterItems();
        }
    </script>
</head>
<body onload="initializeSelectOneItemFromList();">
    <form id="form1" runat="server">
        <div class="select-card">
            <div class="select-title">
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></div>

            <div class="select-search">
                <asp:TextBox ID="TextBoxSearch" runat="server" CssClass="select-search-box" autocomplete="off" placeholder="Search in list..." onkeyup="filterItems();"></asp:TextBox>
            </div>

            <div class="select-list-wrap">
                <asp:GridView ID="gvOptions" runat="server" AutoGenerateColumns="False" CssClass="select-table" GridLines="None" ShowHeader="True"></asp:GridView>
                <div id="emptyState" class="select-empty">No items found.</div>
            </div>

            <asp:HiddenField ID="hdnSelectedValue" runat="server" />
            <asp:HiddenField ID="hdnSelectedText" runat="server" />

            <div class="select-actions">
                <button type="button" class="btn-modern btn-add" onclick="return addSelectedItem();">Add</button>
                <button type="button" class="btn-modern btn-cancel" onclick="return closeParentPopup();">Cancel</button>
            </div>
        </div>
    </form>
</body>
</html>
