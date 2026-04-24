<%@ Page Language="VB" AutoEventWireup="true" CodeFile="VendorPopup.aspx.vb" Inherits="VendorPopup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select Vendor</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            margin: 15px;
            background-color: #ffffff;
        }

        h3 {
            margin-top: 0;
            margin-bottom: 15px;
        }

        .option-link {
            display: block;
            padding: 10px 14px;
            margin: 6px 0;
            background: #f0f4ff;
            border-radius: 5px;
            text-decoration: none;
            color: #333;
            font-size: 15px;
            cursor: pointer;
            border: 1px solid #d6defb;
        }

        .option-link:hover {
            background: #d0dcff;
        }
    </style>

    <script type="text/javascript">
        function selectVendor(value, text) {
            if (window.parent && window.parent.receiveVendorValue) {
                window.parent.receiveVendorValue(value, text);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <h3>&#10022; Select a Vendor</h3>

        <asp:Repeater ID="rptVendorOptions" runat="server">
            <ItemTemplate>
                <a class="option-link" href="javascript:void(0);"
                   onclick="selectVendor('<%# System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("OptionValue"))) %>', '<%# System.Web.HttpUtility.JavaScriptStringEncode(Convert.ToString(Eval("OptionName"))) %>');">
                    <%# Server.HtmlEncode(Convert.ToString(Eval("OptionName"))) %>
                </a>
            </ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
