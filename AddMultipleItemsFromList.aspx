<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddMultipleItemsFromList.aspx.vb" Inherits="AddMultipleItemsFromList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Members</title>

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

        .adj-card {
            width: 100%;
            height: 100%;
            box-sizing: border-box;
            padding: 20px 22px;
            border: none;
            border-radius: 0;
            background: #ffffff;
            box-shadow: none;
            font-family: Arial, sans-serif;
            display: flex;
            flex-direction: column;
        }

        .adj-title {
            font-size: 18px;
            font-weight: 700;
            color: #1f2937;
            margin-bottom: 14px;
            flex: 0 0 auto;
        }

        .members-search {
            flex: 0 0 auto;
            margin-bottom: 12px;
        }

        .adj-textbox {
            width: 100%;
            height: 34px;
            padding: 6px 10px;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            background: #f8fafc;
            font-size: 14px;
            color: #111827;
            box-sizing: border-box;
        }

        .members-scroll {
            flex: 1 1 auto;
            overflow-y: auto;
            overflow-x: hidden;
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            background: #f8fafc;
            padding: 12px 14px;
            box-sizing: border-box;
        }

        .members-checklist {
            width: 100%;
            font-size: 14px;
            color: #111827;
        }

        .members-checklist td {
            padding: 4px 0;
        }

        .members-checklist input[type="checkbox"] {
            margin-right: 8px;
        }

        .members-checklist label {
            color: #374151;
        }

        .member-highlight {
            background-color: #fef08a;
            color: #111827;
            font-weight: 700;
            padding: 0 1px;
            border-radius: 3px;
        }

        .adj-buttons {
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
            color: white;
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
        function closeParentVendorPopup() {
            if (window.parent && typeof window.parent.closeVendorDialog === 'function') {
                window.parent.closeVendorDialog();
            }
            return false;
        }

        function htmlEncode(value) {
            var div = document.createElement('div');
            div.textContent = value;
            return div.innerHTML;
        }

        function escapeRegExp(text) {
            return text.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        }

        function findParentRow(element) {
            var node = element;
            while (node && node.tagName !== 'TR') {
                node = node.parentNode;
            }
            return node;
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
                    html += '<span class="member-highlight">' + htmlEncode(part) + '</span>';
                } else {
                    html += htmlEncode(part);
                }
            }

            return html;
        }

        function filterMembers() {
            var txt = document.getElementById('<%= TextBoxSearch.ClientID %>');
            var list = document.getElementById('<%= CheckBoxList1.ClientID %>');

            if (!txt || !list) return;

            var keyword = txt.value.toLowerCase().trim();
            var labels = list.getElementsByTagName('label');

            for (var i = 0; i < labels.length; i++) {
                var label = labels[i];
                var originalText = label.getAttribute('data-original-text');

                if (!originalText) {
                    originalText = label.textContent || label.innerText || '';
                    label.setAttribute('data-original-text', originalText);
                }

                var row = findParentRow(label);
                var textLower = originalText.toLowerCase();
                var isMatch = (keyword === '' || textLower.indexOf(keyword) > -1);

                if (row) {
                    row.style.display = isMatch ? '' : 'none';
                }

                label.innerHTML = buildHighlightedHtml(originalText, isMatch ? keyword : '');
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="adj-card">
            <div class="adj-title">Members</div>

            <div class="members-search">
                <asp:TextBox ID="TextBoxSearch"
                             runat="server"
                             CssClass="adj-textbox"
                             onkeyup="filterMembers()"
                             placeholder="Search members..."></asp:TextBox>
            </div>

            <div class="members-scroll">
                <asp:CheckBoxList ID="CheckBoxList1"
                                  runat="server"
                                  Font-Names="Arial"
                                  CssClass="members-checklist">
                </asp:CheckBoxList>
            </div>

            <div class="adj-buttons">
                <asp:Button ID="Button1"
                            runat="server"
                            Text="Add"
                            CssClass="btn-modern btn-add"
                            OnClick="Button1_Click" />

                <asp:Button ID="Button2"
                            runat="server"
                            Text="Cancel"
                            CssClass="btn-modern btn-cancel"
                            OnClientClick="return closeParentVendorPopup();"
                            UseSubmitBehavior="false" />
            </div>
        </div>
    </form>
</body>
</html>
