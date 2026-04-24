Imports System.Data
Partial Class MainPage
    Inherits System.Web.UI.Page

    Dim DT As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)


    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Label2.Text = getServerName()
    End Sub
End Class




