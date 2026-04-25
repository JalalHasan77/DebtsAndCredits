<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddAdditionReduction.aspx.vb" Inherits="AddAdditionReduction" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

<style type="text/css">
    .adj-card {
        width: 460px;
        padding: 20px 22px;
        border: 1px solid #d9e2ec;
        border-radius: 12px;
        background: #ffffff;
        box-shadow: 0 4px 14px rgba(0, 0, 0, 0.08);
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

    .adj-field {
    display: flex;
    align-items: center;
    gap: 12px;
}

.adj-options-vertical {
    display: flex;
    flex-direction: column;
    gap: 10px;
}
</style>







</head>
<body>
    <form id="form1" runat="server">
<div class="adj-card">
    <div class="adj-title">Adjustment Details</div>

    <!-- Row 1 -->
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

    <!-- Row 2 -->
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

    <!-- Row 3 -->
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

    <!-- Buttons -->
    <div class="adj-buttons">
        <asp:Button ID="Button1" runat="server" Text="Add" CssClass="btn-modern btn-add" />
        <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="btn-modern btn-cancel" />
    </div>
</div>


    </form>
</body>
</html>
