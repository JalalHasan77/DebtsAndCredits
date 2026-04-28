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

    .adj-shell {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
        padding: 18px;
        background: linear-gradient(180deg, #f8fbff 0%, #eef4ff 100%);
    }

    .adj-card {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
        padding: 22px 24px;
        border: 1px solid #dbe5f2;
        border-radius: 18px;
        background: #ffffff;
        box-shadow: 0 14px 32px rgba(15, 23, 42, 0.10);
        font-family: Arial, sans-serif;
        overflow: auto;
    }

    .adj-title {
        font-size: 19px;
        font-weight: 700;
        color: #1f2937;
        margin-bottom: 14px;
    }

    .adj-form-body {
        border: 1px solid #e5e7eb;
        border-radius: 16px;
        overflow: hidden;
        background: #ffffff;
    }

    .adj-row {
        display: grid;
        grid-template-columns: 115px 1fr;
        align-items: start;
        column-gap: 14px;
        row-gap: 8px;
        margin-bottom: 0;
        padding: 14px 16px;
        border-bottom: 1px solid #e5e7eb;
    }

    .adj-form-body .adj-row:nth-child(odd) {
        background: #f8fbff;
    }

    .adj-form-body .adj-row:nth-child(even) {
        background: #ffffff;
    }

    .adj-form-body .adj-row:last-child {
        border-bottom: none;
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
        height: 36px;
        padding: 6px 10px;
        border: 1px solid #cbd5e1;
        border-radius: 10px;
        background: #ffffff;
        font-size: 14px;
        color: #111827;
        box-sizing: border-box;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
    }

    .adj-dropdown:focus,
    .adj-textbox:focus {
        outline: none;
        border-color: #60a5fa;
        box-shadow: 0 0 0 3px rgba(96, 165, 250, 0.18);
    }

    .adj-dropdown {
        min-width: 190px;
    }

    .adj-textbox {
        width: 120px;
    }

    .adj-options-horizontal {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        gap: 16px;
    }

    .adj-options-vertical {
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .adj-radio {
        margin-right: 2px;
    }

    .adj-buttons {
        display: flex;
        justify-content: flex-end;
        gap: 10px;
        margin-top: 18px;
        padding-top: 16px;
        border-top: 1px solid #e5e7eb;
    }

    .btn-modern {
        min-width: 90px;
        height: 38px;
        padding: 0 16px;
        border: none;
        border-radius: 10px;
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
</script>

</head>
<body>
    <form id="form1" runat="server">
<div class="adj-shell">
<div class="adj-card">
    <div class="adj-title">Adjustment Details</div>

    <div class="adj-form-body">
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

        <div class="adj-row">
            <div class="adj-label">
                <asp:Label ID="Label1" runat="server" Text="Distributed"></asp:Label>
            </div>
            <div class="adj-field adj-options-horizontal">
                <asp:RadioButton ID="RadioButton1" runat="server" Text="Equally" GroupName="distributedGroup" CssClass="adj-radio" />
                <asp:RadioButton ID="RadioButton2" runat="server" Text="By %" GroupName="distributedGroup" CssClass="adj-radio" />
            </div>
        </div>
    </div>

    <div class="adj-buttons">
        <asp:Button ID="Button1" runat="server" Text="Add" CssClass="btn-modern btn-add" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="btn-modern btn-cancel" OnClientClick="return closeParentVendorPopup();" UseSubmitBehavior="false" />
    </div>
</div>
</div>

    </form>
</body>
</html>
