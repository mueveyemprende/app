@Code
    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    'Dim email As String = "vicentegalindo@hotmail.com" ' Request.QueryString("e")

    'Dim idemp As Long = 148 '214 '148
    Dim idemp As Long = 0
    Dim qstring = Request.QueryString("e")
    Dim eidemp As String

    'Se puso por un error en la conversión
    If InStr(qstring, "®") <> 0 Then
        eidemp = Mid(Request.QueryString("e"), 1, InStr(qstring, "®") - 1)
    Else
        eidemp = Request.QueryString("e")
    End If

    If eidemp = "" Then
        Response.Redirect("https://mueveyemprende.io", False)
    End If

    Dim c As New poolin_class.cComunes

    idemp = Val(c.Decrypt(eidemp))

    If idemp <> 0 Then
        Response.Cookies("idemp").Value = idemp
        Dim objemp As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = objemp.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count <> 0 Then
            Response.Cookies("emp").Value = Trim(dt.Rows(0)("nombres") & " " & dt.Rows(0)("apellidos"))
        End If
        If Request.QueryString("token") <> "" Then
            If objemp.autoreg(idemp, Request.QueryString("token"), strConn) Then
                Response.Redirect("user-perfil", False)
            Else
                Response.Redirect("https://mueveyemprende.io", False)
            End If
        Else
            If dt.Rows(0)("autoreg") = 0 Then
                Response.Redirect("https://mueveyemprende.io", False)
            End If
            Response.Redirect("user-dash", False)
        End If
    End If
End Code