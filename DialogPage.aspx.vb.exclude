Partial Class DialogPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadOptions()
        End If
    End Sub

    Private Sub LoadOptions()
        ' Example: bind dynamic options to the Repeater
        Dim options As New List(Of OptionItem) From {
            New OptionItem With {.OptionName = "Product Alpha", .OptionValue = "ALPHA"},
            New OptionItem With {.OptionName = "Product Beta", .OptionValue = "BETA"},
            New OptionItem With {.OptionName = "Product Gamma", .OptionValue = "GAMMA"}
        }

        rptOptions.DataSource = options
        rptOptions.DataBind()
    End Sub

    ' Simple class to hold option data
    Public Class OptionItem
        Public Property OptionName As String
        Public Property OptionValue As String
    End Class

End Class
