<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddAdditionReduction.aspx.vb" Inherits="AddAdditionReduction" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

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
    }

    .adj-title {
        font-size: 18px;
        font-weight: 700;
        color: #1f2937;
        margin-bottom: 18px;
    }

    .adj-row {
        display: grid;
        grid-template-columns: 100px 1fr;
        align-items: start;
        column-gap: 14px;
        row-gap: 8px;
        margin-bottom: 14px;
    }

    .adj-label {
        font-weight: 700;
        color: #374151;
        padding-top: 6px;
        white-space: nowrap;
    }

    .adj-field {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        gap: 12px;
    }

    .adj-dropdown,
    .adj-textbox {
        height: 34px;
        padding: 6px 10px;
        border: 1px solid #cbd5e1;
        border-radius: 8px;
        background: #f8fafc;
        font-size: 14px;
        color: #111827;
        box-sizing: border-box;
    }

    .adj-dropdown {
        min-width: 180px;
    }

    .adj-textbox {
        width: 180px;
    }

    .adj-options-horizontal {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        gap: 14px;
    }

    .adj-radio {
        margin-right: 2px;
    }

    .adj-buttons {
        display: flex;
        justify-content: flex-end;
        gap: 10px;
        margin-top: 18px;
        padding-top: 14px;
        border-top: 1px solid #e5e7eb;
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

    .adj-options-vertical {
        display: flex;
        flex-direction: column;
        gap: 10px;
    }
</style>

<script type="text/javascript">
    function closeParentVendorPopup() {
        if (window.parent && typeof window.parent.closeVendorDialog === 'function') {
            window.parent.closeVendorDialog();
        }
        return false;
    }

    function getSelectedTypeValue() {
        var reduction = document.getElementById('<%= RadioButton6.ClientID %>');
        var addition = document.getElementById('<%= RadioButton7.ClientID %>');
        var neutral = document.getElementById('<%= RadioButton8.ClientID %>');

        if (reduction && reduction.checked) return 'Reduction';
        if (addition && addition.checked) return 'Addition';
        if (neutral && neutral.checked) return 'Neutral';

        return '';
    }

    function setParentNewORder(value) {
        if (!window.parent || !window.parent.document) return;

        var parentDoc = window.parent.document;
        var target = parentDoc.getElementById('NewORder');
        var i;

        if (!target) {
            var inputs = parentDoc.getElementsByTagName('input');
            for (i = 0; i < inputs.length; i++) {
                if ((inputs[i].id && /NewORder$/i.test(inputs[i].id)) ||
                    (inputs[i].name && /NewORder$/i.test(inputs[i].name))) {
                    target = inputs[i];
                    break;
                }
            }
        }

        if (!target) {
            var selects = parentDoc.getElementsByTagName('select');
            for (i = 0; i < selects.length; i++) {
                if ((selects[i].id && /NewORder$/i.test(selects[i].id)) ||
                    (selects[i].name && /NewORder$/i.test(selects[i].name))) {
                    target = selects[i];
                    break;
                }
            }
        }

        if (target) {
            if (typeof target.value !== 'undefined') {
                target.value = value;
            } else {
                target.innerHTML = value;
            }
        }
    }

    function addAdjustmentAndClose() {
        var ddl = document.getElementById('<%= DropDownList2.ClientID %>');
        var selectedValue = '';
        var selectedText = '';
        var selectedTypeValue = getSelectedTypeValue();

        if (ddl) {
            selectedValue = ddl.value || '';
            if (ddl.selectedIndex >= 0) {
                selectedText = ddl.options[ddl.selectedIndex].text || selectedValue;
            }
        }

        if (selectedTypeValue !== '') {
            setParentNewORder(selectedTypeValue);
        }

        if (window.parent) {
            if (typeof window.parent.receiveVendorValue === 'function') {
                window.parent.receiveVendorValue(selectedValue, selectedText);
                return false;
            }

            if (typeof window.parent.closeVendorDialog === 'function') {
                window.parent.closeVendorDialog();
            }
        }

        return false;
    }
</script>

</head>
<body>
    <form id="form1" runat="server">
<div class="adj-card">
    <div class="adj-title">Adjustment Details</div>

    <div class="adj-row">
        <div class="adj-label">
            <asp:Label ID="Label4" runat="server" Text="Adj. Name"></asp:Label>
        </div>
        <div class="adj-field">
            <asp:DropDownList ID="DropDownList2" runat="server" CssClass="adj-dropdown">
                <asp:ListItem Selected="True">VAT</asp:ListItem>
                <asp:ListItem>Discount</asp:ListItem>
                <asp:ListItem>Fees</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div class="adj-row">
        <div class="adj-label">
            <asp:Label ID="Label5" runat="server" Text="Type"></asp:Label>
        </div>
        <div class="adj-field adj-options-horizontal">
            <asp:RadioButton ID="RadioButton6" runat="server" Text="Reduction" GroupName="TypeGroup" CssClass="adj-radio" />
            <asp:RadioButton ID="RadioButton7" runat="server" Text="Addition" GroupName="TypeGroup" CssClass="adj-radio" />
            <asp:RadioButton ID="RadioButton8" runat="server" Text="Neutral" GroupName="TypeGroup" CssClass="adj-radio" />
        </div>
    </div>

    <div class="adj-row">
        <div class="adj-label">
            <asp:Label ID="Label6" runat="server" Text="Amount"></asp:Label>
        </div>

        <div class="adj-field">
            <div class="adj-options-vertical">
                <asp:RadioButton ID="RadioButton9" runat="server" Text="Percentage" GroupName="AmountGroup" CssClass="adj-radio" />
                <asp:RadioButton ID="RadioButton10" runat="server" Text="Fixed Amount" GroupName="AmountGroup" CssClass="adj-radio" />
            </div>

            <asp:TextBox ID="TextBox2" runat="server" CssClass="adj-textbox" Width="100px"></asp:TextBox>
        </div>
    </div>

    <div class="adj-buttons">
        <asp:Button ID="Button1" runat="server" Text="Add" CssClass="btn-modern btn-add" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="btn-modern btn-cancel" OnClientClick="return closeParentVendorPopup();" UseSubmitBehavior="false" />
    </div>
</div>

    </form>
</body>
</html>
