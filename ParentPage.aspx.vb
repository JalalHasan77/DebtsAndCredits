Partial Class ParentPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadOptions()
        End If
    End Sub

    Private Sub LoadOptions()
        Dim options As New List(Of OptionItem) From {
            New OptionItem With {.OptionName = "Product Alpha", .OptionValue = "ALPHA"},
            New OptionItem With {.OptionName = "Product Beta", .OptionValue = "BETA"},
            New OptionItem With {.OptionName = "Product Gamma", .OptionValue = "GAMMA"}
        }
        rptOptions.DataSource = options
        rptOptions.DataBind()
    End Sub

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnProcess.Click
        Dim returnedValue As String = hdnSelectedValue.Value
        lblResult.Text = If(String.IsNullOrEmpty(returnedValue),
                            "No value selected.",
                            "You selected: " & returnedValue)

        Dim a As String
        a = a.Replace("http", "https")
    End Sub

    Public Class OptionItem
        Public Property OptionName As String
        Public Property OptionValue As String
    End Class

End Class
