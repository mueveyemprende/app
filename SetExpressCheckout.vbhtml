@Functions
    Dim ReturnUrl As String
    Dim CancelUrl As String
    Dim LogoUrl As String
    Dim SellerEmail As String
    Dim RedirectUrl As String

    Dim logger As ILog = LogManager.GetLogger("SetExpressCheckout")

    Protected Function GetConfig() As Dictionary(Of String, String)
        Return PayPal.Manager.ConfigManager.Instance.GetProperties()
    End Function

    Protected Sub Page_Load()
        Dim CurrContext As HttpContext = HttpContext.Current

        ' Create the SetExpressCheckoutResponseType object
        Session("ExpressCheckoutMethod") = "MarkExpressCheckout"
        Dim responseSetExpressCheckoutResponseType As New SetExpressCheckoutResponseType()
        Try
            ' Check if the EC methos is shorcut or mark
            Dim ecMethod As String = ""
            If Request.QueryString("ExpressCheckoutMethod") IsNot Nothing Then
                ecMethod = Request.QueryString("ExpressCheckoutMethod")
            ElseIf DirectCast(Session("ExpressCheckoutMethod"), String) IsNot Nothing Then
                ecMethod = DirectCast(Session("ExpressCheckoutMethod"), String)
            End If
            Dim item_name As String = ""
            Dim item_id As String = ""
            Dim item_desc As String = ""
            Dim item_quantity As String = ""
            Dim item_amount As String = ""
            Dim tax_amount As String = ""
            Dim shipping_amount As String = ""
            Dim handling_amount As String = ""
            Dim shipping_discount_amount As String = ""
            Dim insurance_amount As String = ""
            Dim total_amount As String = ""
            Dim currency_code As String = ""
            Dim payment_type As String = ""

            ' From Marck EC Page
            Dim shipping_rate As String = ""
            Dim first_name As String = ""
            Dim last_name As String = ""
            Dim street1 As String = ""
            Dim street2 As String = ""
            Dim city As String = ""
            Dim state As String = ""
            Dim postal_code As String = ""
            Dim country As String = ""
            Dim phone As String = ""
            Session("idpropuesta") = 0
            Session("fecha") = Request.Form("fecha")
            Dim new_total_rate As [Double] = 0.0
            Dim shipToAddress As New AddressType()
            If Not IsNothing(Request.Form("idpropuesta")) Then
                Session("idpropuesta") = Request.Form("idpropuesta")
            End If

            If ecMethod IsNot Nothing AndAlso ecMethod = "ShorcutExpressCheckout" Then
                ' Get parameters from index page (shorcut express checkout)
                item_name = Request.Form("item_name")
                item_id = Request.Form("item_id")
                'item_desc = Request.Form("item_desc")
                item_quantity = "1" 'Request.Form("item_quantity")
                item_amount = Request.Form("item_amount")
                'tax_amount = Request.Form("tax_amount")
                'shipping_amount = Request.Form("shipping_amount")
                'handling_amount = Request.Form("handling_amount")
                'shipping_discount_amount = Request.Form("shipping_discount_amount")
                'insurance_amount = Request.Form("insurance_amount")
                'total_amount = Request.Form("item_amount")
                total_amount = item_amount 'Request.Form("total_amount")
                currency_code = Request.Form("currency_code_type")
                payment_type = "Sale" 'Request.Form("payment_type")
                Session("Total_Amount") = total_amount
            ElseIf ecMethod IsNot Nothing AndAlso ecMethod = "MarkExpressCheckout" Then
                ' Get parameters from mark ec page 
                shipping_rate = "0" ' Request.Form("shipping_method").ToString()

                item_name = Request.Form("item_name")
                item_id = Request.Form("item_id")
                item_desc = "0" 'Request.Form("item_desc")
                item_quantity = "1" 'Request.Form("item_quantity")
                item_amount = Request.Form("item_amount")
                tax_amount = "0" 'Request.Form("tax_amount")
                shipping_amount = "0" ' Request.Form("shipping_amount")
                handling_amount = "0" ' Request.Form("handling_amount")
                shipping_discount_amount = "0" '  Request.Form("shipping_discount_amount")
                insurance_amount = "0" ' Request.Form("insurance_amount")
                total_amount = item_amount 'Request.Form("total_amount")
                currency_code = Request.Form("currency_code_type")
                payment_type = "Sale" ' Request.Form("payment_type")

                'first_name = Request.Form("FIRST_NAME")
                'last_name = Request.Form("LAST_NAME")
                'street1 = Request.Form("STREET_1")
                'street2 = Request.Form("STREET_2")
                'city = Request.Form("CITY")
                'state = Request.Form("STATE")
                'postal_code = Request.Form("POSTAL_CODE")
                'country = Request.Form("COUNTRY")
                'phone = Request.Form("PHONE")

                ' Set the details of new shipping address                  
                shipToAddress.Name = Nothing ' Convert.ToString(first_name & Convert.ToString(" ")) & last_name
                shipToAddress.Street1 = Nothing ' street1
                shipToAddress.Street2 = Nothing
                If Not street2.Equals("") Then
                End If
                shipToAddress.CityName = Nothing ' city
                shipToAddress.StateOrProvince = Nothing ' state
                Dim countrycode As String = Request.Form("countrycode") ' "MX" 'country
                Dim countryCodeType As CountryCodeType = DirectCast([Enum].Parse(GetType(CountryCodeType), countrycode, True), CountryCodeType)
                shipToAddress.Country = countryCodeType
                shipToAddress.PostalCode = "" ' postal_code
                If Not phone.Equals("") Then
                    shipToAddress.Phone = phone
                End If

                Dim total_rate As [Double] = Convert.ToDouble(total_amount)
                Dim old_shipping_rate As [Double] = Convert.ToDouble(shipping_amount)
                Dim new_shipping_rate As [Double] = Convert.ToDouble(shipping_rate)

                ' Calculate new order total based on shipping method selected
                new_total_rate = total_rate - old_shipping_rate + new_shipping_rate
                Session("Total_Amount") = new_total_rate.ToString()
                total_amount = new_total_rate.ToString()
                shipping_amount = new_shipping_rate.ToString()
            End If

            Session("SellerEmail") = SellerEmail
            Dim currencyCode_Type As CurrencyCodeType = DirectCast([Enum].Parse(GetType(CurrencyCodeType), currency_code, True), CurrencyCodeType)
            Session("currency_code_type") = currencyCode_Type
            Dim payment_ActionCode_Type As PaymentActionCodeType = DirectCast([Enum].Parse(GetType(PaymentActionCodeType), payment_type, True), PaymentActionCodeType)
            Session("payment_action_type") = payment_ActionCode_Type
            ' SetExpressCheckoutRequestDetailsType object
            Dim setExpressCheckoutRequestDetails As New SetExpressCheckoutRequestDetailsType()
            setExpressCheckoutRequestDetails.cppLogoImage = "https://mueveyemprende.io/img/logo.png"
            setExpressCheckoutRequestDetails.NoShipping = 1
            ' (Required) URL to which the buyer's browser is returned after choosing to pay with PayPal.
            setExpressCheckoutRequestDetails.ReturnURL = ReturnUrl
            '(Required) URL to which the buyer is returned if the buyer does not approve the use of PayPal to pay you
            setExpressCheckoutRequestDetails.CancelURL = CancelUrl
            ' A URL to your logo image. Use a valid graphics format, such as .gif, .jpg, or .png
            setExpressCheckoutRequestDetails.cppLogoImage = LogoUrl
            ' To display the border in your principal identifying color, set the "cppCartBorderColor" parameter to the 6-digit hexadecimal value of that color
            ' setExpressCheckoutRequestDetails.cppCartBorderColor = "0000CD";

            'Item details
            Dim itemDetails As New PaymentDetailsItemType()
            itemDetails.Name = item_name
            itemDetails.Amount = New BasicAmountType(currencyCode_Type, item_amount)
            itemDetails.Quantity = Convert.ToInt32(item_quantity)
            itemDetails.Description = item_desc
            itemDetails.Number = item_id

            'Add more items if necessary by using the class 'PaymentDetailsItemType'

            ' Payment Information
            Dim paymentDetailsList As New List(Of PaymentDetailsType)()

            Dim paymentDetails As New PaymentDetailsType()
            paymentDetails.PaymentAction = payment_ActionCode_Type
            paymentDetails.ItemTotal = New BasicAmountType(currencyCode_Type, item_amount)
            'item amount
            paymentDetails.TaxTotal = New BasicAmountType(currencyCode_Type, tax_amount)
            'tax amount;
            paymentDetails.ShippingTotal = New BasicAmountType(currencyCode_Type, shipping_amount)
            'shipping amount
            paymentDetails.HandlingTotal = New BasicAmountType(currencyCode_Type, handling_amount)
            'handling amount
            paymentDetails.ShippingDiscount = New BasicAmountType(currencyCode_Type, shipping_discount_amount)
            'shipping discount
            paymentDetails.InsuranceTotal = New BasicAmountType(currencyCode_Type, insurance_amount)
            'insurance amount
            paymentDetails.OrderTotal = New BasicAmountType(currencyCode_Type, total_amount)
            ' order total amount
            paymentDetails.PaymentDetailsItem.Add(itemDetails)

            ' Unique identifier for the merchant. 
            Dim sellerDetails As New SellerDetailsType()
            sellerDetails.PayPalAccountID = SellerEmail
            paymentDetails.SellerDetails = sellerDetails

            If ecMethod IsNot Nothing AndAlso ecMethod = "MarkExpressCheckout" Then
                paymentDetails.ShipToAddress = shipToAddress
            End If
            paymentDetailsList.Add(paymentDetails)
            setExpressCheckoutRequestDetails.PaymentDetails = paymentDetailsList

            ' Collect Shipping details if MARK express checkout

            Dim setExpressCheckout As New SetExpressCheckoutReq()
            Dim setExpressCheckoutRequest As New SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails)
            setExpressCheckout.SetExpressCheckoutRequest = setExpressCheckoutRequest

            ' Create the service wrapper object to make the API call
            Dim service As New PayPalAPIInterfaceServiceService()

            ' API call
            ' Invoke the SetExpressCheckout method in service wrapper object
            responseSetExpressCheckoutResponseType = service.SetExpressCheckout(setExpressCheckout)

            If responseSetExpressCheckoutResponseType IsNot Nothing Then
                ' Response envelope acknowledgement
                Dim acknowledgement As String = "SetExpressCheckout API Operation - "
                acknowledgement += responseSetExpressCheckoutResponseType.Ack.ToString()
                logger.Debug(acknowledgement & Convert.ToString(vbLf))
                System.Diagnostics.Debug.WriteLine(acknowledgement & Convert.ToString(vbLf))
                ' # Success values
                If responseSetExpressCheckoutResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS") Then
                    ' # Redirecting to PayPal for authorization
                    ' Once you get the "Success" response, needs to authorise the
                    ' transaction by making buyer to login into PayPal. For that,
                    ' need to construct redirect url using EC token from response.
                    ' Express Checkout Token
                    Dim EcToken As String = responseSetExpressCheckoutResponseType.Token
                    logger.Info((Convert.ToString("Express Checkout Token : ") & EcToken) + vbLf)
                    System.Diagnostics.Debug.WriteLine((Convert.ToString("Express Checkout Token : ") & EcToken) + vbLf)
                    ' Store the express checkout token in session to be used in GetExpressCheckoutDetails & DoExpressCheckout API operations
                    Session("EcToken") = EcToken
                    Response.Redirect(RedirectUrl + HttpUtility.UrlEncode(EcToken), False)
                    Context.ApplicationInstance.CompleteRequest()
                Else
                    ' # Error Values
                    Dim errorMessages As List(Of ErrorType) = responseSetExpressCheckoutResponseType.Errors
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
        Catch ex As System.Exception
            ' Log the exception message
            logger.Debug("Error Message : " + ex.Message)
            System.Diagnostics.Debug.WriteLine("Error Message : " + ex.Message)
        End Try
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

    log4net.Config.XmlConfigurator.Configure()

    Dim schemeHost As String = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)

    'Load values from web.config (configuration file)
    Dim config = GetConfig()
    ReturnUrl = schemeHost + config("ReturnUrl")
    CancelUrl = schemeHost + config("CancelUrl")
    LogoUrl = schemeHost + config("LogoUrl")
    SellerEmail = config("SellerEmail")
    RedirectUrl = config("RedirectUrl")

    Page_Load()
    '<input type = "hidden" name="item_name" value="Poolin Seguro - @descripcion" />
    '<input type = "hidden" name="item_id" value="9" />
    '<input type = "hidden" name="item_amount" value="@montopagar" />
    '<input type = "hidden" name="currency_code_type" value="@moneda" />
    '<input type = "hidden" name="countrycode" value="@countrycode" />
    '<input type = "hidden" name="idpropuesta" value="@idpropuesta" />
    '<input type = "hidden" name="idproyecto" value="@idproyecto" />
    '<input type = "hidden" name="monto" value="@monto" />
    '<input type = "hidden" name="mcomision" value="@mcomision" />
    '<input type = "hidden" name="moneda" value="@moneda"/>    
    Session("idplanpaypal") = 9
    Session("coddesc") = Request.Form("item_name")
    Session("idpropuesta") = Request.Form("idpropuesta")
    Session("idproyecto") = Request.Form("idproyecto")
    Session("monto") = Request.Form("monto")
    Session("mcomision") = Request.Form("mcomision")
    Session("moneda") = Request.Form("moneda")
    Session("montopago") = Request.Form("item_amount")


End Code