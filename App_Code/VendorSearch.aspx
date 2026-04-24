<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VendorSearch.aspx.vb" Inherits="VendorSearch" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vendor Search</title>
    <script type="text/javascript">
        function selectVendor(value, text) {
            if (window.parent && typeof window.parent.receiveVendorValue === 'function') {
                window.parent.receiveVendorValue(value, text);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Select a Vendor</h3>
            <asp:Button ID="btnVendor1" runat="server" Text="Choose Vendor ABC" OnClientClick="selectVendor('ABC', 'Vendor ABC'); return false;" />
            <br /><br />
            <asp:Button ID="btnVendor2" runat="server" Text="Choose Vendor XYZ" OnClientClick="selectVendor('XYZ', 'Vendor XYZ'); return false;" />
        </div>
    </form>
</body>
</html>
