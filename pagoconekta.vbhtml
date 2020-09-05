@Imports Conekta
@functions
    Dim idrecibo_View As String = ""
    Dim idplan_view As String = ""
    Dim msgerror As String = ""

    Private Sub Genera_OrdendePago(ByVal idpropuesta As Long, ByVal idproyecto As Long, ByVal mpago As String,
                              ByVal coddesc As String,
                              ByVal monto As Double, ByVal mcomision As Double,
                              ByVal montopagar As Double, ByVal moneda As String,
                              ByVal descpago As String,
                              ByVal idemp As Long, ByVal strConn As String)
        Conekta.Api.apiKey = ConfigurationManager.AppSettings("conekta_private").ToString
        Conekta.Api.version = "2.0.0"
        Dim _mpago As String = mpago
        Dim idrecibo As Long = 0


        Dim e As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = e.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count = 0 Then
        End If

        Dim drow As System.Data.DataRow = dt.Rows(0)
        Dim tel As String = ""
        Dim nombre As String = ""
        Dim email As String = ""
        Dim idcust As String = drow("idcust_conekta")
        If Not IsDBNull(drow("telefono")) Then
            tel = drow("telefono")
            tel = drow("telefono").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "")
        End If
        If tel = "" Then tel = "5555555555"
        nombre = drow("nombres") & " " & drow("apellidos")
        email = drow("email")
        Dim stridCust As String = ""
        If idcust <> "K" Then
            stridCust = " ""customer_id"":""" & idcust & """," & vbCrLf
        End If
        Dim customer As Conekta.Customer
        Dim strCustomer As String

        If _mpago.ToLower = "oxxo" Then
            _mpago = "oxxo_cash"
        End If

        strCustomer = "{" & vbCrLf &
                        stridCust &
                        " ""name"":""" & nombre & """," & vbCrLf &
                        "  ""email"":""" & email & """," & vbCrLf &
                        "  ""phone"":""" & tel & """," & vbCrLf &
                        "  ""payment_methods"":[{" & vbCrLf &
                        "    ""type"": """ & _mpago & """" & vbCrLf &
                        "  }]" & vbCrLf &
                       "}"
        'mpago oxxo o spei
        customer = New Conekta.Customer().create(strCustomer)

        e.Update_IDConekta(idemp, customer.id, strConn)

        Dim m As New poolin_class.cMembresia

        'm.Genera_Recibo_Plan(idrecibo, idPlan, idemp, UCase(_mpago), coddesc,
        '                "E", strConn, idpropuesta)

        m.Genera_Pago(idrecibo, idemp, idpropuesta, idproyecto,
                      monto, mcomision, montopagar, moneda,
                      descpago, "", mpago, "", "", "", _mpago.ToLower, strConn)

        Dim idioma As String = "ESP"

        Dim dtRecibo As System.Data.DataTable = m.Datos_Pago(idrecibo, strConn)
        If dtRecibo.Rows.Count = 0 Then
            Response.Redirect("nopago")
        End If
        drow = dtRecibo.Rows(0)
        Dim orden As Conekta.Order = New Conekta.Order().create("{" & vbCr & vbLf &
                                                        "  ""currency"":""" & drow("moneda") & """," & vbCr & vbLf &
                                                        "  ""customer_info"": {" & vbCr & vbLf &
                                                        stridCust &
                                                        "    ""name"": """ & nombre & """," & vbCr & vbLf &
                                                        "    ""phone"": """ & tel & """," & vbCr & vbLf &
                                                        "    ""email"": """ & email & """" & vbCr & vbLf &
                                                        "  }," & vbCr & vbLf &
                                                        "  ""line_items"": [{" & vbCr & vbLf &
                                                        "    ""name"": """ & drow("descripcion") & """," & vbCr & vbLf &
                                                        "    ""unit_price"": " & CStr((Math.Round(drow("monto") + drow("mcomision"), 2)) * 100) & "," & vbCr & vbLf &
                                                        "    ""quantity"": 1" & vbCr & vbLf & "  }]," & vbCr & vbLf &
                                                        "  ""charges"": [{" & vbCr & vbLf &
                                                        "    ""payment_method"": {" & vbCr & vbLf &
                                                        "      ""type"": """ & _mpago & """" & vbCr & vbLf &
                                                        "    }" & vbCr & vbLf &
                                                        "  }]" & vbCr & vbLf &
                                                        "}")

        If orden.payment_status.ToUpper = "PENDING_PAYMENT" Then
            If mpago = "oxxo" Then
                'm.Activa_Plan(idrecibo, idemp, orden.charges.data(0)("payment_method")("reference"), "CONEKTA OXXO",
                '          orden.payment_status.ToUpper,
                '          "",
                '          "",
                '          upfecha, strConn, 0, orden.id, idpropuesta)
                m.Activa_Pago(idrecibo, idemp,
                          orden.charges.data(0)("payment_method")("reference"),
                          "CONECTA OXXO", orden.payment_status.ToUpper, "", "", "EN PROCESO", strConn, 0, orden.id)
            Else
                'm.Activa_Plan(idrecibo, idemp, orden.charges.data(0)("payment_method")("receiving_account_number"), "CONEKTA SPEI",
                '          orden.payment_status.ToUpper,
                '          "",
                '          "",
                '          upfecha, strConn, 0, orden.id, idpropuesta)
                m.Activa_Pago(idrecibo, idemp,
                          orden.charges.data(0)("payment_method")("receiving_account_number"),
                          "CONECTA SPEI", orden.payment_status.ToUpper, "", "", "EN PROCESO", strConn, 0, orden.id)

            End If
            Enviar_Recibo_OXXO_SPEI(idrecibo, idemp, mpago, strConn)
        End If
    End Sub

    Private Sub Aplica_Pago(ByVal idemp As Long, ByVal token As String, ByVal idPlan As Long,
                ByVal idpropuesta As Long, ByVal idproyecto As Long, ByVal coddesc As String,
                ByVal monto As Double, ByVal mcomision As Double, ByVal montopago As Double,
                ByVal moneda As String, ByVal descripcion As String,
                ByVal asunto As String, ByVal mensaje As String,
                ByVal strConn As String)
        Conekta.Api.apiKey = ConfigurationManager.AppSettings("conekta_private").ToString
        Conekta.Api.version = "2.1.14"

        'token = "tok_test_visa_4242"
        idrecibo_View = "0"
        Dim e As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = e.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count = 0 Then

        End If
        Dim drow As System.Data.DataRow = dt.Rows(0)
        Dim tel As String = ""
        Dim nombre As String = ""
        Dim email As String = ""
        Dim idcust As String = drow("idcust_conekta")
        If Not IsDBNull(drow("telefono")) Then
            tel = drow("telefono")
            tel = tel.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "")
        End If
        If tel = "" Then tel = "5555555555"
        nombre = drow("nombres") & " " & drow("apellidos")
        email = drow("email")
        Dim stridCust As String = ""
        If idcust <> "K" Then
            stridCust = " ""customer_id"":""" & idcust & """," & vbCrLf
        End If
        Dim customer As Conekta.Customer
        Dim strCustomer As String
        strCustomer = "{" & vbCrLf &
                        stridCust &
                        " ""name"":""" & nombre & """," & vbCrLf &
                        "  ""email"":""" & email & """," & vbCrLf &
                        "  ""phone"":""" & tel & """," & vbCrLf &
                        "  ""payment_methods"":[{" & vbCrLf &
                        "    ""type"": ""card""," & vbCrLf &
                        "    ""token_id"":""" & token & """" & vbCrLf &
                        "  }]" & vbCrLf &
                       "}"
        customer = New Conekta.Customer().create(strCustomer)

        e.Update_IDConekta(idemp, customer.id, strConn)

        If customer.id <> "" Then
            stridCust = " ""customer_id"":""" & customer.id & """," & vbCr & vbLf
        End If
        Dim m As New poolin_class.cMembresia
        Dim idrecibo As Long = 0
        'm.Genera_Recibo_Plan(idrecibo, idPlan, "E", "", coddesc,
        '                     "E", strConn, idpropuesta)

        m.Genera_Pago(idrecibo, idemp, idpropuesta, idproyecto,
                      monto, mcomision, montopago, moneda,
                      descripcion, "", "", "", "", "", "", strConn, False)

        Dim idioma As String = "ESP"

        Dim dtRecibo As System.Data.DataTable = m.Datos_Pago(idrecibo, strConn)
        If dtRecibo.Rows.Count = 0 Then

        End If
        drow = dtRecibo.Rows(0)

        Dim strorden As String
        strorden = "{"
        strorden &= "  ""line_items"": [{"
        strorden &= "    ""name"": """ & drow("descripcion") & ""","
        strorden &= "    ""unit_price"": " & CStr(Math.Round(drow("montopagar"), 2) * 100) & ","
        strorden &= "    ""quantity"": 1"
        strorden &= "    }],"
        strorden &= "  ""currency"":""" & drow("moneda") & ""","
        strorden &= "  ""customer_info"": {"
        strorden &= "    ""customer_id"": """ & customer.id & """"
        strorden &= "  },"
        strorden &= "  ""charges"": [{"
        strorden &= "    ""payment_method"": {"
        strorden &= "      ""type"": ""card"","
        strorden &= "      ""token_id"": """ & token & """"
        strorden &= "    }"
        strorden &= "  }]"
        strorden &= "}"
        Dim orden As Conekta.Order = New Conekta.Order().create(strorden)

        If orden.payment_status.ToUpper = "PAID" Then
            'orden.charges.data(0)("payment_method")("name") & " - " & 
            m.Activa_Pago(idrecibo, idemp,
                      orden.charges.data(0)("id"),
                      orden.charges.data(0)("payment_method")("object"),
                      orden.charges.data(0)("payment_method")("auth_code"),
                      orden.charges.data(0)("payment_method")("type"),
                      orden.charges.data(0)("payment_method")("last4") & " - " & orden.charges.data(0)("payment_method")("brand"), "EN mueve y emprende", strConn, 1)
            '  m.Activa_Plan(idrecibo, idemp, orden.id, "CONEKTA",
            'orden.charges.data(0)("payment_method")("auth_code"),
            'orden.charges.data(0)("payment_method")("object"),
            'orden.charges.data(0)("payment_method")("name") & " - " & orden.charges.data(0)("payment_method")("last4") & " - " & orden.charges.data(0)("payment_method")("brand") & " - " & orden.charges.data(0)("payment_method")("issuer") & " - " & orden.charges.data(0)("payment_method")("type"),
            'upfecha, strConn, , , idpropuesta)
            'If idpropuesta <> 0 Then
            '    Dim pPro As New poolin_class.cPropuestas
            '    Dim dtPro As System.Data.DataTable = pPro.Carga_PropuestaPersona(0, 0, strConn, idpropuesta)
            '    If dtPro.Rows.Count <> 0 Then
            '        Dim p As New poolin_class.cProyectos
            '        p.AceptaPropuesta(idemp, dtPro.Rows(0).Item("idemprendedor"),
            '                      dtPro.Rows(0).Item("idproyecto"), idpropuesta, strConn, asunto, mensaje)
            '    End If
            'End If
            idplan_view = idPlan
            idrecibo_View = CStr(idrecibo)
            Enviar_Recibo(idrecibo, idemp, asunto, strConn)
        Else
            msgerror = orden.payment_status.ToUpper
        End If


    End Sub


    Protected Sub Enviar_Recibo_OXXO_SPEI(ByVal idrecibo As Long,
                      ByVal idemp As Long,
                      ByVal mpago As String,
                      ByVal strConn As String)
        Dim P As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = P.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count <> 0 Then
            'Using smtp As New SmtpClient
            '    Dim correo As New MailMessage
            Dim http As String = "https://mueveyemprende.io"

            Dim htmlmsg As String

            If mpago = "oxxo" Then
                Using f As New StreamReader(Server.MapPath("poolin_arch/oxxo/opps_es.txt"))
                    htmlmsg = f.ReadToEnd
                    f.Close()
                End Using
            Else
                Using f As New StreamReader(Server.MapPath("poolin_arch/spei/spei_es.txt"))
                    htmlmsg = f.ReadToEnd
                    f.Close()
                End Using
            End If

            htmlmsg = htmlmsg.Replace("[URL]", http).Replace("[WEB]", http)
            Dim m As New poolin_class.cMembresia
            Dim idioma As String = "ESP"

            Dim dtRecibo As System.Data.DataTable = m.Datos_Pago(idrecibo, strConn)
            If dtRecibo.Rows.Count <> 0 Then
                htmlmsg = htmlmsg.Replace("[REFERENCIA]", dtRecibo.Rows(0).Item("pagoid"))
                htmlmsg = htmlmsg.Replace("[MONTO]", Format(dtRecibo.Rows(0).Item("montopagar"), PageData("moneda")))
                htmlmsg = htmlmsg.Replace("[MONEDA]", dtRecibo.Rows(0).Item("moneda"))
                htmlmsg = htmlmsg.Replace("[POOLIN]", "RED DE SERVICIOS EMPRESARIALES Y PROFESIONALES mueve y emprende, S.C.")
            End If

            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString

            Dim _subject As String '= "POOLIN - Recibo de Pago"
            If mpago = "spei" Then
                _subject = "mueve y emprende - Orden para Pago vía SPEI"
            Else
                _subject = "mueve y emprende - Orden para Pago en OXXO"
            End If

            Dim objsend As New poolin_class.cSendGrid
            objsend.Correo_SMTP(_subject, htmlmsg, dt.Rows(0)("email"), "", strConn)

        End If

    End Sub

    Protected Sub Enviar_Recibo(ByVal idrecibo As Long, ByVal idemp As Long,
                            ByVal asunto As String, ByVal strConn As String)
        Dim P As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = P.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count <> 0 Then
            'Using smtp As New SmtpClient
            '    Dim correo As New MailMessage
            Dim ssl As Boolean = False
            Dim http As String = ""

            Dim htmlmsg As String
            Using f As New StreamReader(Server.MapPath("poolin_arch/txt/recibopago.txt"))
                htmlmsg = f.ReadToEnd
                f.Close()
            End Using

            htmlmsg = htmlmsg.Replace("[URL]", http).Replace("[WEB]", http)
            Dim m As New poolin_class.cMembresia
            Dim idioma As String = "ESP"

            Dim dtRecibo As System.Data.DataTable = m.Datos_Recibo_vm(idrecibo, strConn)
            If dtRecibo.Rows.Count <> 0 Then
                htmlmsg = htmlmsg.Replace("[NUMRECIBO]", dtRecibo.Rows(0).Item("id"))
                htmlmsg = htmlmsg.Replace("[IDTRANS]", dtRecibo.Rows(0).Item("pagoid"))
                htmlmsg = htmlmsg.Replace("[FECHA]", Format(dtRecibo.Rows(0).Item("fecha"), "dd-MM-yyyy"))
                htmlmsg = htmlmsg.Replace("[FPAGO]", "" & dtRecibo.Rows(0).Item("fpago"))
                htmlmsg = htmlmsg.Replace("[MPAGO]", dtRecibo.Rows(0).Item("metodo"))
                htmlmsg = htmlmsg.Replace("[CODAUT]", dtRecibo.Rows(0).Item("auto_code"))
                htmlmsg = htmlmsg.Replace("[PLAN]", dtRecibo.Rows(0).Item("descripcion"))
                htmlmsg = htmlmsg.Replace("[MONTO]", Format(dtRecibo.Rows(0).Item("precio"), PageData("moneda")))
                htmlmsg = htmlmsg.Replace("[MONEDA]", dtRecibo.Rows(0).Item("moneda"))
            End If

            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString

            Dim objsend As New poolin_class.cSendGrid
            objsend.Correo_SMTP(asunto, htmlmsg, dt.Rows(0)("email"), "", strConn)

        End If
    End Sub


End Functions

@Code
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Orden de Pago"

    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If


    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    Dim idemp As Long = 0
    Dim strconn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try
    Try
        Select Case Request.Form("mpago")
            Case "oxxo", "spei"
                Genera_OrdendePago(Request.Form("idpropuesta"), Request.Form("idproyecto"),
                              Request.Form("mpago"), Request.Form("descripcion"), Request.Form("monto"), Request.Form("mcomision"),
                              Request.Form("montopagar"), Request.Form("moneda"),
                              Request.Form("descripcion"), idemp, strconn)
                Dim objComun As New poolin_class.cComunes
                Response.Redirect("user-proyecto-seguimiento?tipo=empleador&viewpagos=poolin-orden&idproyecto=" & objComun.Encrypt(Request.Form("idproyecto")), False)
            Case Else
                Aplica_Pago(idemp, Request.Form("tokenid"),
                            Request.Form("idplan"), Request.Form("idpropuesta"),
                            Request.Form("idproyecto"), Request.Form("descripcion"),
                            Request.Form("monto"), Request.Form("mcomision"), Request.Form("montopagar"),
                            Request.Form("moneda"), Request.Form("descripcion"),
                            "PAGO mueve y emprende - TARJETA", "SE APLICÓ PAGO CON TARJETA", strconn)
                Dim objComun As New poolin_class.cComunes
                Response.Redirect("user-proyecto-seguimiento?tipo=empleador&viewpagos=poolin-tc&idproyecto=" & objComun.Encrypt(Request.Form("idproyecto")), False)
        End Select

    Catch ex As Exception
        msgerror = ex.Message
        Dim objComun As New poolin_class.cComunes
        Response.Redirect("user-proyecto-seguimiento?tipo=empleador&viewpagos=poolin-error&idproyecto=" & objComun.Encrypt(Request.Form("idproyecto")), False)
    End Try
End Code

