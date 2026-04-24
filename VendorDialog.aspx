<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VendorDialog.aspx.vb" Inherits="VendorDialog" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Select Vendor</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f7f9fc;
            padding: 20px;
        }

        .dialog-card {
            max-width: 620px;
            margin: 0 auto;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 8px 24px rgba(0,0,0,0.12);
            padding: 24px;
        }

        .dialog-title {
            margin-top: 0;
            margin-bottom: 16px;
            color: #1f3c88;
        }

        .option-link {
            display: block;
            padding: 12px 14px;
            margin: 8px 0;
            background: #eef4ff;
            border-radius: 6px;
            text-decoration: none;
            color: #1d1d1d;
            font-size: 15px;
        }

        .option-link:hover {
            background: #d9e7ff;
        }

        .btn-close {
            margin-top: 18px;
            padding: 10px 18px;
            background: #cc0000;
            color: #fff;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .empty-state {
            color: #666;
            font-style: italic;
            margin-top: 12px;
        }
    </style>

    <script type="text/javascript">
        function returnVendorValue(value, text) {
            if (window.opener && !window.opener.closed && typeof window.opener.receiveVendorValue === 'function') {
                window.opener.receiveVendorValue(value, text);
                window.close();
            } else {
                alert('Parent page is not available.');
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="dialog-card">
            <h3 class="dialog-title">Select a Vendor</h3>

            <asp:Repeater ID="rptVendors" runat="server">
                <ItemTemplate>
                    <a class="option-link" href="javascript:void(0);"
                       onclick="returnVendorValue('<%# Eval("OptionValue") %>', '<%# Eval("OptionName") %>');">
                        <%# Eval("OptionName") %>
                    </a>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">No vendors were found.</div>
            </asp:Panel>

            <button type="button" class="btn-close" onclick="window.close();">Close</button>
        </div>
    </form>
</body>
</html>
