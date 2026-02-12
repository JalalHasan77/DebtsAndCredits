Imports System.Data
Partial Class MainPage
    Inherits System.Web.UI.Page

    Dim DT As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)
        'SetOnClientClick(btnAddNew, GetAppPath() + "/AddNew.aspx?MODE=NEW", 700, 1000, , False)
        'SetOnClientClick(btnAddNewCategory, GetAppPath() + "/AddNewCategory.aspx?MODE=NEW", 700, 1000, , False)
    End Sub


End Class




