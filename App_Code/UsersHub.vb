Imports Microsoft.AspNet.SignalR

Public Class UsersHub
    Inherits Hub

    ' Called from server-side (Users.aspx.vb) via GetHubContext
    ' This method is optional — direct server push via hubContext.Clients.All is enough.
    ' But you can keep it if you want clients to also be able to trigger updates directly.
    Public Sub UpdateUserStatus(ByVal id As String, ByVal isChecked As Boolean)
        Clients.All.userStatusChanged(id, isChecked)
    End Sub
End Class