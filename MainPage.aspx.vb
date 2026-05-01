Imports System.Data
Partial Class MainPage
    Inherits System.Web.UI.Page

    Dim DT As DataTable
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddJQueryLinks(Page, True)


    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim allSQL As String = TextBox1.Text
        Dim L As New List(Of String)
        L.AddRange(Split(allSQL, ";"))
        L = L.Where(Function(s) Not String.IsNullOrWhiteSpace(s)).ToList()

        For Each SQL As String In L
            Try
                DB.ExecuteNonQuery(InfoDB, SQL)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        Next
        TextBox1.Text = ""
    End Sub
End Class




