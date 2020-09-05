
@functions
    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim BNCode As String

    'Load values from web.config (configuration file)

    Dim logger As ILog = LogManager.GetLogger("dopagopaypal")

    Shared Sub New()
        ' Load the log4net configuration settings from Web.config or App.config
        log4net.Config.XmlConfigurator.Configure()
    End Sub

    Function GetConfig() As Dictionary(Of String, String)
        Return PayPal.Manager.ConfigManager.Instance.GetProperties()
    End Function

    Protected Sub Page_Load(ByVal idemp As Long)
        Dim config = GetConfig()

        BNCode = config("SBN_CODE")

        Dim CurrContext As HttpContext = HttpContext.Current

        ' Create the DoExpressCheckoutPaymentResponseType object
        Dim responseDoExpressCheckoutPaymentResponseType As New DoExpressCheckoutPaymentResponseType()

        Dim idpropuesta As Long = 0
        If Session("idpropuesta") IsNot Nothing Then
            idpropuesta = Session("idpropuesta")
        End If
        ' Create the DoExpressCheckoutPaymentReq object
        Dim doExpressCheckoutPayment As New DoExpressCheckoutPaymentReq()
        Dim doExpressCheckoutPaymentRequestDetails As New DoExpressCheckoutPaymentRequestDetailsType()
        ' The timestamped token value that was returned in the
        ' `SetExpressCheckout` response and passed in the
        ' `GetExpressCheckoutDetails` request.
        doExpressCheckoutPaymentRequestDetails.Token = DirectCast(Session("EcToken"), String)
        ' Unique paypal buyer account identification number as returned in
        ' `GetExpressCheckoutDetails` Response
        doExpressCheckoutPaymentRequestDetails.PayerID = DirectCast(Session("PayerId"), String)

        ' # Payment Information
        ' list of information about the payment
        Dim paymentDetailsList As New List(Of PaymentDetailsType)()
        ' information about the payment
        Dim paymentDetails As New PaymentDetailsType()
        Dim currency_code_type As CurrencyCodeType = DirectCast(Session("currency_code_type"), CurrencyCodeType)
        Dim payment_action_type As PaymentActionCodeType = DirectCast(Session("payment_action_type"), PaymentActionCodeType)
        'Pass the order total amount which was already set in session
        Dim total_amount As String = DirectCast(Session("Total_Amount"), String)
        Dim orderTotal As New BasicAmountType(currency_code_type, total_amount)
        paymentDetails.OrderTotal = orderTotal
        paymentDetails.PaymentAction = payment_action_type

        'BN codes to track all transactions
        paymentDetails.ButtonSource = BNCode

        ' Unique identifier for the merchant. 
        Dim sellerDetails As New SellerDetailsType()
        sellerDetails.PayPalAccountID = DirectCast(Session("SellerEmail"), String)
        paymentDetails.SellerDetails = sellerDetails

        paymentDetailsList.Add(paymentDetails)
        doExpressCheckoutPaymentRequestDetails.PaymentDetails = paymentDetailsList

        Dim doExpressCheckoutPaymentRequest As New DoExpressCheckoutPaymentRequestType(doExpressCheckoutPaymentRequestDetails)
        doExpressCheckoutPayment.DoExpressCheckoutPaymentRequest = doExpressCheckoutPaymentRequest
        ' Create the service wrapper object to make the API call
        Dim service As New PayPalAPIInterfaceServiceService()
        ' # API call
        ' Invoke the DoExpressCheckoutPayment method in service wrapper object
        responseDoExpressCheckoutPaymentResponseType = service.DoExpressCheckoutPayment(doExpressCheckoutPayment)
        If responseDoExpressCheckoutPaymentResponseType IsNot Nothing Then

            ' Response envelope acknowledgement
            Dim acknowledgement As String = "DoExpressCheckoutPayment API Operation - "
            acknowledgement += responseDoExpressCheckoutPaymentResponseType.Ack.ToString()
            logger.Info(acknowledgement & Convert.ToString(vbLf))
            System.Diagnostics.Debug.WriteLine(acknowledgement & Convert.ToString(vbLf))
            ' # Success values
            If responseDoExpressCheckoutPaymentResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS") Then
                ' Transaction identification number of the transaction that was
                ' created.
                ' This field is only returned after a successful transaction
                ' for DoExpressCheckout has occurred.
                If responseDoExpressCheckoutPaymentResponseType.DoExpressCheckoutPaymentResponseDetails.PaymentInfo IsNot Nothing Then
                    Dim paymentInfoIterator As IEnumerator(Of PaymentInfoType) = responseDoExpressCheckoutPaymentResponseType.DoExpressCheckoutPaymentResponseDetails.PaymentInfo.GetEnumerator()
                    While paymentInfoIterator.MoveNext()
                        Dim paymentInfo As PaymentInfoType = paymentInfoIterator.Current
                        logger.Info("Transaction ID : " + paymentInfo.TransactionID + vbLf)

                        Session("Transaction_Id") = paymentInfo.TransactionID
                        Session("Transaction_Type") = paymentInfo.TransactionType
                        Session("Payment_Status") = paymentInfo.PaymentStatus
                        Session("Payment_Type") = paymentInfo.PaymentType
                        Session("Payment_Total_Amount") = paymentInfo.GrossAmount.value

                        System.Diagnostics.Debug.WriteLine("Transaction ID : " + paymentInfo.TransactionID + vbLf)
                        Aplica_Pago(paymentInfo.TransactionID.ToString,
                                    paymentInfo.TransactionType.ToString,
                                    paymentInfo.PaymentStatus.ToString,
                                    paymentInfo.PaymentType.ToString,
                                    Session("idplanpaypal"),
                                    Session("coddesc"),
                                    idpropuesta,
                                    Session("idproyecto"),
                                    Session("monto"),
                                    Session("mcomision"),
                                    Session("montopago"),
                                    Session("moneda"),
                                    idemp)
                    End While
                End If
            Else
                ' # Error Values
                Dim errorMessages As List(Of ErrorType) = responseDoExpressCheckoutPaymentResponseType.Errors
                Dim errorMessage As String = ""
                For Each [error] As ErrorType In errorMessages
                    logger.Debug("API Error Message : " + [error].LongMessage)
                    System.Diagnostics.Debug.WriteLine("API Error Message : " + [error].LongMessage + vbLf)
                    errorMessage = errorMessage + [error].LongMessage
                Next
                'Redirect to error page in case of any API errors
                CurrContext.Items.Add("APIErrorMessage", errorMessage)
                Server.Transfer("~/Response.aspx")
            End If
        End If

    End Sub

    Private Sub Aplica_Pago(ByVal pagoid As String,
                            ByVal auto_code As String,
                            ByVal paymenttype As String,
                            ByVal pagostatus As String,
                            ByVal idPlan As Long,
                            ByVal coddesc As String,
                            ByVal idpropuesta As Long,
                            ByVal idproyecto As Long,
                            ByVal monto As Double,
                            ByVal mcomision As Double,
                            ByVal montopago As Double,
                            ByVal moneda As String,
                            ByVal idemp As Long)

        Session("idrecibo") = 0
        Session("idioma") = "ESP"

        Dim e As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = e.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count = 0 Then
            Session("idioma") = dt.Rows(0).Item("lang")
        End If
        Dim drow As System.Data.DataRow = dt.Rows(0)
        Dim tel As String = ""
        Dim nombre As String = ""
        Dim email As String = ""

        If Not IsDBNull(drow("telefono")) Then
            tel = tel.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "")
        End If
        If tel = "" Then tel = "5555555555"
        nombre = drow("nombres") & " " & drow("apellidos")
        email = drow("email")

        Dim m As New poolin_class.cMembresia
        Dim idrecibo As Long
        m.Genera_Pago(idrecibo, idemp, idpropuesta, idproyecto, monto, mcomision,
                      montopago, moneda, coddesc, pagoid, "paypal", auto_code,
                      paymenttype, pagostatus, "", strConn, True)


        Dim dtRecibo As System.Data.DataTable = m.Datos_Pago(idrecibo,
                                                 strConn)
        If dtRecibo.Rows.Count = 0 Then

        End If
        drow = dtRecibo.Rows(0)

        'm.Activa_Plan(idrecibo, idemp, transid, "PAYPAL",
        '              transtype,
        '              "CUENTA PAYPAL",
        '              paymenttype,
        '              Now, strConn, , , idpropuesta)

        Session("idplan") = idPlan
        Session("idrecibo") = idrecibo
        Session("idpropuesta") = idpropuesta

        'If idpropuesta <> 0 Then
        '    Dim pPro As New poolin_class.cPropuestas
        '    Dim dtPro As System.Data.DataTable = pPro.Carga_PropuestaPersona(0, 0, strConn, idpropuesta)
        '    If dtPro.Rows.Count <> 0 Then
        '        Dim p As New poolin_class.cProyectos
        '        'Dim cMsg As New cComunes
        '        Dim asunto As String = "PROPUESTA ACEPTADA"
        '        Dim mensaje = "PONTE EN CONTACTO CON TU CLIENTE. Puedes utilizar POOLIN para enviar mensajes y/o agendar citas."
        '        p.AceptaPropuesta(idemp, dtPro.Rows(0).Item("idemprendedor"),
        '                          dtPro.Rows(0).Item("idproyecto"), idpropuesta, strConn,
        '                          asunto, mensaje)
        '    End If
        'End If
        Enviar_Recibo(idrecibo, idemp)


    End Sub

    Protected Sub Enviar_Recibo(ByVal idrecibo As Long, ByVal idemp As Long)
        Dim P As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = P.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count <> 0 Then
            Dim idioma As String = dt.Rows(0).Item("lang")
            Dim http As String = ""
            Dim htmlmsg As String
            Using f As New StreamReader(Server.MapPath("poolin_arch/txt/recibopago.txt"))
                htmlmsg = f.ReadToEnd
                f.Close()
            End Using
            htmlmsg = htmlmsg.Replace("[URL]", http).Replace("[WEB]", http)
            Dim m As New poolin_class.cMembresia

            Dim dtRecibo As System.Data.DataTable = m.Datos_Recibo_vm(idrecibo, strConn)
            If dtRecibo.Rows.Count <> 0 Then
                htmlmsg = htmlmsg.Replace("[NUMRECIBO]", dtRecibo.Rows(0).Item("id"))
                htmlmsg = htmlmsg.Replace("[IDTRANS]", dtRecibo.Rows(0).Item("pagoid"))
                htmlmsg = htmlmsg.Replace("[FECHA]", Format(dtRecibo.Rows(0).Item("fecha"), "dd-MM-yyyy"))
                htmlmsg = htmlmsg.Replace("[FPAGO]", "" & dtRecibo.Rows(0).Item("fpago"))
                htmlmsg = htmlmsg.Replace("[MPAGO]", dtRecibo.Rows(0).Item("metodo"))
                htmlmsg = htmlmsg.Replace("[CODAUT]", "" & dtRecibo.Rows(0).Item("auto_code"))
                htmlmsg = htmlmsg.Replace("[PLAN]", dtRecibo.Rows(0).Item("descripcion"))
                htmlmsg = htmlmsg.Replace("[MONTO]", Format(dtRecibo.Rows(0).Item("precio"), PageData("moneda")))
                htmlmsg = htmlmsg.Replace("[MONEDA]", dtRecibo.Rows(0).Item("moneda"))
            End If

            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString

            Dim objsend As New poolin_class.cSendGrid
            objsend.Correo_SMTP("M&E - Recibo de Pago", htmlmsg, dt.Rows(0)("email"), "", strConn)

        End If
    End Sub

End Functions

@Code
    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim objComun As New poolin_class.cComunes
    Try
        Page_Load(idemp)
        Response.Redirect("user-proyecto-seguimiento?tipo=empleador&viewpagos=poolin-paypal&idproyecto=" & objComun.Encrypt(Session("idproyecto")), False)
    Catch ex As System.Exception
        ' Log the exception message
        Response.Redirect("user-proyecto-seguimiento?tipo=empleador&viewpagos=poolin-error&idproyecto=" & objComun.Encrypt(Session("idproyecto")), False)
        logger.Debug("Error Message : " + ex.Message)
        System.Diagnostics.Debug.WriteLine("Error Message : " + ex.Message)
    End Try

    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

End Code
