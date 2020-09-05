
@functions
    Dim logger As ILog = LogManager.GetLogger("PoolinPayPal")

    Shared Sub New()
        ' Load the log4net configuration settings from Web.config or App.config
        log4net.Config.XmlConfigurator.Configure()
    End Sub

    'Private Shared logger As log4net.ILog = log4net.LogManager.GetLogger(GetType(getpagopaypal))
    Private Sub getpagopaypal_Load()
        Dim CurrContext As HttpContext = HttpContext.Current

        ' Create the GetExpressCheckoutDetailsResponseType object
        Dim responseGetExpressCheckoutDetailsResponseType As New GetExpressCheckoutDetailsResponseType()
        Try
            ' Create the GetExpressCheckoutDetailsReq object
            Dim getExpressCheckoutDetails As New GetExpressCheckoutDetailsReq()
            ' A timestamped token, the value of which was returned by `SetExpressCheckout` API response
            Dim EcToken As String = DirectCast(Session("EcToken"), String)
            Dim getExpressCheckoutDetailsRequest As New GetExpressCheckoutDetailsRequestType(EcToken)
            getExpressCheckoutDetails.GetExpressCheckoutDetailsRequest = getExpressCheckoutDetailsRequest
            ' Create the service wrapper object to make the API call
            Dim service As New PayPalAPIInterfaceServiceService()
            ' # API call
            ' Invoke the GetExpressCheckoutDetails method in service wrapper object
            responseGetExpressCheckoutDetailsResponseType = service.GetExpressCheckoutDetails(getExpressCheckoutDetails)
            If responseGetExpressCheckoutDetailsResponseType IsNot Nothing Then
                ' Response envelope acknowledgement
                Dim acknowledgement As String = "GetExpressCheckoutDetails API Operation - "
                acknowledgement += responseGetExpressCheckoutDetailsResponseType.Ack.ToString()
                logger.Info(acknowledgement & Convert.ToString(vbLf))
                System.Diagnostics.Debug.WriteLine(acknowledgement & Convert.ToString(vbLf))
                ' # Success values
                If responseGetExpressCheckoutDetailsResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS") Then
                    ' Unique PayPal Customer Account identification number. This
                    ' value will be null unless you authorize the payment by
                    ' redirecting to PayPal after `SetExpressCheckout` call.
                    Dim PayerId As String = responseGetExpressCheckoutDetailsResponseType.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerID
                    ' Store PayerId in session to be used in DoExpressCheckout API operation
                    Session("PayerId") = PayerId

                    Dim paymentDetails As List(Of PaymentDetailsType) = responseGetExpressCheckoutDetailsResponseType.GetExpressCheckoutDetailsResponseDetails.PaymentDetails
                    For Each paymentdetail As PaymentDetailsType In paymentDetails
                        Dim ShippingAddress As AddressType = paymentdetail.ShipToAddress
                        If ShippingAddress IsNot Nothing Then
                            Session("Address_Name") = ShippingAddress.Name
                            Session("Address_Street") = ShippingAddress.Street1 + " " + ShippingAddress.Street2
                            Session("Address_CityName") = ShippingAddress.CityName
                            Session("Address_StateOrProvince") = ShippingAddress.StateOrProvince
                            Session("Address_CountryName") = ShippingAddress.CountryName
                            Session("Address_PostalCode") = ShippingAddress.PostalCode
                        End If
                        Session("Currency_Code") = paymentdetail.OrderTotal.currencyID
                        Session("Order_Total") = paymentdetail.OrderTotal.value
                        Session("Shipping_Total") = paymentdetail.ShippingTotal.value
                        Dim itemList As List(Of PaymentDetailsItemType) = paymentdetail.PaymentDetailsItem
                        For Each item As PaymentDetailsItemType In itemList
                            Session("Product_Quantity") = item.Quantity

                            Session("Product_Name") = item.Name
                        Next
                    Next
                Else
                    ' # Error Values
                    Dim errorMessages As List(Of ErrorType) = responseGetExpressCheckoutDetailsResponseType.Errors
                    Dim errorMessage As String = ""
                    For Each [error] As ErrorType In errorMessages
                        logger.Debug("API Error Message : " + [error].LongMessage)
                        System.Diagnostics.Debug.WriteLine("API Error Message : " + [error].LongMessage + vbLf)
                        errorMessage = errorMessage + [error].LongMessage
                    Next
                    'Redirect to error page in case of any API errors
                    CurrContext.Items.Add("APIErrorMessage", errorMessage)
                    Server.Transfer("~/cancelado.aspx")
                End If
            End If
            'Redirect to DoExpressCheckoutPayment.aspx page if the method chosen is MarkExpressCheckout
            'The buyer need not review the shipping address and shipping method as it's already provided
            Dim ecMethod As String = DirectCast(Session("ExpressCheckoutMethod"), String)
            If ecMethod.Equals("MarkExpressCheckout") Then
                Response.Redirect("dopagopaypal", False)
                Context.ApplicationInstance.CompleteRequest()

            End If
            ' # Exception log
        Catch ex As System.Exception
            ' Log the exception message
            logger.Debug("Error Message : " + ex.Message)
            System.Diagnostics.Debug.WriteLine("Error Message : " + ex.Message)
        End Try
    End Sub

End Functions
@Code
    PageData("Title") = "Pagos PAYPAL"
    getpagopaypal_Load()
End Code
<div id="content">

</div>
