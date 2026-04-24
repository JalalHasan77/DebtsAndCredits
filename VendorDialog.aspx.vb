Imports System
Imports System.Collections.Generic

Partial Class VendorPopup
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadVendorOptions()
        End If
    End Sub

    Private Sub LoadVendorOptions()
        Dim options As New List(Of OptionItem) From {
            New OptionItem With {.OptionName = "Product Alpha", .OptionValue = "ALPHA"},
            New OptionItem With {.OptionName = "Product Beta", .OptionValue = "BETA"},
            New OptionItem With {.OptionName = "Product Gamma", .OptionValue = "GAMMA"}
        }

        rptVendorOptions.DataSource = options
        rptVendorOptions.DataBind()
    End Sub

    Public Class OptionItem
        Public Property OptionName As String
        Public Property OptionValue As String
    End Class
End Class
