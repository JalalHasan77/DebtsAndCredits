Imports System
Imports System.Web
Imports Microsoft.AspNet.SignalR

Namespace SignalRChat
    Public Class ChatHub
        Inherits Hub

        Public Sub Send(ByVal name As String, ByVal message As String)
            'Clients.All.broadcastMessage(name, message)
            'name = Guid.NewGuid
            name = "any thing"

            Clients.Others.broadcastMessage(name, message)
        End Sub
    End Class
End Namespace
