<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SampleParentPage.aspx.vb" Inherits="SampleParentPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sample Parent Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:LinkButton ID="btnVendorSearch" runat="server" Text="Search Vendor" />
            <br /><br />
            <asp:HiddenField ID="hdnSelectedVendorValue" runat="server" />
            <asp:HiddenField ID="hdnSelectedVendorText" runat="server" />
            <asp:TextBox ID="TextBox1" runat="server" Width="300" />
        </div>
    </form>
</body>
</html>
