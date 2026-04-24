<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DialogPage.aspx.vb" Inherits="DialogPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Dialog</title>
    <script type="text/javascript">
        // Sends selected value back to parent window and closes dialog
        function returnValue(value) {
            if (window.opener && !window.opener.closed) {
                // Call the parent's receiveValue() function
                window.opener.receiveValue(value);
            }
            window.close(); // Close the dialog
        }
    </script>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .option-link { display: block; margin: 10px 0; font-size: 16px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Select an Option</h3>

        <!-- Static links that return a hardcoded value -->
        <a class="option-link" href="javascript:void(0);" onclick="returnValue('Option A');">Option A</a>
        <a class="option-link" href="javascript:void(0);" onclick="returnValue('Option B');">Option B</a>
        <a class="option-link" href="javascript:void(0);" onclick="returnValue('Option C');">Option C</a>

        <hr />

        <!-- OR: Dynamic options loaded from server (GridView/Repeater) -->
        <asp:Repeater ID="rptOptions" runat="server">
            <ItemTemplate>
                <a class="option-link" href="javascript:void(0);"
                   onclick="returnValue('<%# Eval("OptionValue") %>');">
                    <%# Eval("OptionName") %>
                </a>
            </ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
