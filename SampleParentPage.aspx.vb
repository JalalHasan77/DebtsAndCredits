Partial Class SampleParentPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RegisterVendorPopup(Me,
                            btnVendorSearch,
                            "~/VendorSearch.aspx",
                            900,
                            600,
                            PopupPlacement.Center,
                            hdnSelectedVendorValue,
                            hdnSelectedVendorText,
                            TextBox1,
                            "Select Vendor")
    End Sub
End Class
