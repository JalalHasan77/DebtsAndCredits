Imports Microsoft.VisualBasic
Imports System.Net.Mail
Imports System

Public Module SendEmailClass
    'Dim udf As New GUDF
    Public Sub SendEmailMessage(ByVal strFrom As String, ByVal strTo() _
     As String, ByVal strSubject _
     As String, ByVal strMessage _
     As String, ByVal fileList() As String)
        'This procedure takes string array parameters for multiple recipients and files
        Try
            For Each item As String In strTo
                'For each to address create a mail message
                Dim MailMsg As New MailMessage(New MailAddress(strFrom.Trim()), New MailAddress(item))
                MailMsg.BodyEncoding = Encoding.Default
                MailMsg.Subject = strSubject.Trim()
                MailMsg.Body = strMessage.Trim() & vbCrLf
                MailMsg.Priority = MailPriority.High
                MailMsg.IsBodyHtml = True

                'attach each file attachment
                For Each strfile As String In fileList
                    If Not strfile = "" Then
                        Dim MsgAttach As New Attachment(strfile)
                        MailMsg.Attachments.Add(MsgAttach)
                    End If
                Next

                'Smtpclient to send the mail message
                Dim SmtpMail As New SmtpClient
                SmtpMail.Host = "192.168.1.9"
                SmtpMail.Send(MailMsg)
            Next
            'Message Successful
        Catch ex As Exception
            'Message Error
        End Try
    End Sub

    Public Sub SendEmailMessage(ByVal strFrom As String, ByVal strTo _
    As String, ByVal strSubject _
    As String, ByVal strMessage _
    As String, ByVal file As String)
        'This procedure overrides the first procedure and accepts a single
        'string for the recipient and file attachement 
        Try
            Dim MailMsg As New MailMessage(New MailAddress(strFrom.Trim()), New MailAddress(strTo))
            MailMsg.BodyEncoding = Encoding.Default
            MailMsg.Subject = strSubject.Trim()
            MailMsg.Body = strMessage.Trim() & vbCrLf
            MailMsg.Priority = MailPriority.High
            MailMsg.IsBodyHtml = True

            If Not file = "" Then
                Dim MsgAttach As New Attachment(file)
                MailMsg.Attachments.Add(MsgAttach)
            End If

            'Smtpclient to send the mail message
            Dim SmtpMail As New SmtpClient
            SmtpMail.Host = "192.168.1.9"
            SmtpMail.Port = 25
            SmtpMail.Send(MailMsg)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub SendingSMTPEmails(ByVal lcFrom As String, ByVal clToCC As Collection, ByVal lcSubject As String, ByVal lcBody As String)
        '4.11
        '        Dim objSMTPClient As New Net.Mail.SmtpClient("192.168.1.9", 25) chnaged on 2014-02-04 due to email server upgrade

        'IO.File.AppendAllText("LogFile", lcBody)


        Dim objSMTPClient As New Net.Mail.SmtpClient("192.168.4.111", 25)
        Dim RecerverEmails() As String = Split(lcFrom, "; ")
        Dim objMailMsg As New Net.Mail.MailMessage()
        Dim x As String
        If clToCC.Count = 0 Then Exit Sub
        objMailMsg.From = New MailAddress(lcFrom)
        For Each x In clToCC
            objMailMsg.To.Add(New MailAddress(x.ToString))
            'If Left(x, 4) = "<TO>" Then objMailMsg.To.Add(New MailAddress(Mid(x, 5)))
            'If Left(x, 4) = "<CC>" Then objMailMsg.CC.Add(New MailAddress(Mid(x, 5)))
        Next
        objMailMsg.Subject = lcSubject
        objMailMsg.Body = lcBody
        objMailMsg.Priority = Net.Mail.MailPriority.High
        objMailMsg.IsBodyHtml = True
        Try
            '     objSMTPClient.Send(objMailMsg)
        Catch ex As Exception
            'Throw ex
            'udf.SaveSessionVariable("", "ERROR", ex.ToString)
        End Try
        objMailMsg = Nothing
        objSMTPClient = Nothing
    End Sub
End Module
