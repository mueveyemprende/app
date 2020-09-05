@Code
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Pago a Proyecto"

    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    If Request.Form("idproyecto") = "" Then
        Response.Redirect("user-dash", True)
    End If

    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Dim monto = CDbl(Request.Form("monto"))
    Dim mcomision = CDbl(Request.Form("mcomision"))
    Dim mivacomision = CDbl(Request.Form("mivacomision"))
    Dim montopagar = CDbl(Request.Form("montopagar"))
    Dim moneda = Request.Form("moneda")

    Dim descripcion = Request.Form("descripcion")
    Dim countrycode = "MX"
    Dim idpropuesta = Request.Form("idpropuesta")
    Dim idproyecto = Request.Form("idproyecto")
    Dim idreciboi = Request.Form("idrecibo")
    Dim nombre = ""
    Dim tarjeta = ""
End Code

@section head
    <style>
        button.mercadopago-button {
            width: 100%;
            font-size: 16px;
        }

    </style>    
End Section

<div id="content">
    <h3 class="user_title">Pagos</h3>
    <form action="~/user-proyecto-seguimiento" method="post">
        <input type="hidden" name="idproyecto" value="@idproyecto" />
        <button type="submit" class="btn btn-primary">Regresar al Seguimiento</button>
    </form>
            <div class="row">
                <div class="col-md-6">
                    <form id="frm-paypal" method="post" action="~/user-proyecto-seguimiento">
                        <input type="hidden" name="idproyecto" value="@idproyecto" />
                        <input type="hidden" name="msg" value="PAGO" />
                        <input type="hidden" name="errpago" id="errpago" />
                        <div class="cuenta-box-pagos">
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <span class="resumen-pago">Resume del pago (@moneda)</span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">
                                    <label>Monto</label>
                                </div>
                                <div class="col-lg-6">
                                    <span class="pull-right">@Html.Raw(Format(monto, PageData("moneda")))</span>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">
                                    <label>Comisión</label>
                                </div>
                                <div class="col-lg-6">
                                    <span class="pull-right">@Html.Raw(Format(mcomision, PageData("moneda")))</span>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">
                                    <label>IVA Comisión</label>
                                </div>
                                <div class="col-lg-6">
                                    <span class="pull-right">@Html.Raw(Format(mivacomision, PageData("moneda")))</span>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">
                                    <label>Por pagar</label>
                                </div>
                                <div class="col-lg-6">
                                    <span class="pull-right">@Html.Raw(Format(montopagar, PageData("moneda")))</span>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
                <div class="col-md-6 text-center">
                    <div class="fields_wrap form-group">
                        <div id="paypal-button"></div>
                    </div>
                    <div class="fields_wrap form-group">
                        <form id="frm-mercadopago" method="post" action="~/pago-mercadopago" >
                            <input type="hidden" name="idproyecto" value="@idproyecto" />
                            <input type="hidden" name="monto" value="@monto" />
                            <input type="hidden" name="mcomision" value="@mcomision" />
                            <input type="hidden" name="mcomisioniva" value="@mivacomision" />
                            <input type="hidden" name="moneda" value="@moneda" />
                            <input type="hidden" name="montopagar" value="@montopagar" />
                            <input type="hidden" name="descripcion" value="@descripcion" />
                                <script src="https://www.mercadopago.com.mx/integrations/v1/web-payment-checkout.js"
                                        data-preference-id="@Request.Form("data-id")" 
                                        data-button-label="Pagar mediante Mercado Pago"></script>
                            <img src="~/img/mercadopago.png" title="Mercado Pago" />
                            @*<script src="https://www.mercadopago.com.mx/integrations/v1/web-payment-checkout.js"></script>
                            <a href="@Request.Form("data-id")">Pagar mediante Mercado Pago</a>*@

                        </form>
                    </div>
                </div>
            </div>
    @*<div class="row">
        <div class="col-md-6"></div>
        <div class="col-md-6">
            <span id="siteseal"><script async type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=HIs6ZOa26sFuHx2AvWvK9d6S3BWWU7XqzyouWO7UaR9gocIVWJKtB9hTX1qf"></script></span>
        </div>
    </div>*@
</div>


@section Scripts
	<!-- ================== BEGIN BASE JS ================== -->
	<script src="assets/plugins/jquery/jquery-3.2.1.min.js"></script>
	<script src="assets/plugins/jquery-ui/jquery-ui.min.js"></script>
	<script src="assets/plugins/cookie/js/js.cookie.js"></script>
	<script src="assets/plugins/tooltip/popper/popper.min.js"></script>
	<script src="assets/plugins/bootstrap/bootstrap4/js/bootstrap.min.js"></script>
	<script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>
	<!-- ================== END BASE JS ================== -->

    <script src="https://www.paypalobjects.com/api/checkout.js"></script>
    <script>
        $(document).ready(function () {
            var total = Number(@montopagar);
            paypal.Button.render({
                env: 'sandbox',
                client: {
                    sandbox: 'AaPlw7KCPPAeTmBUE5kgz6U3eoOnvNMgETBhLRRBkrsqAAe1iG5ZuV2ZxRqMrLzjzSWxvVhKhO0-_BgE',
                    production: ''
                },
                commit: true,
                style: {
                    size: 'responsive',
                    color: 'blue',
                    shape: 'pill',
                    label: 'checkout',
                    tagline: 'true',
                },
                payment: function (data, actions) {
                    return actions.payment.create({
                        transactions: [{
                            amount: {
                                total: total,
                                currency: '@moneda'
                            },
                            description: 'Pago Proyecto M&E - @descripcion', 
                            custom: 'Pago Proyecto M&E - @idproyecto'
                        }]
                    });
                },
                onAuthorize: function (details, actions) {
                    return actions.payment.execute().then(function () {
                        // Show a success message to the buyer
                        $.ajax({
                            type: "POST",
                            url: "wsseguimiento.asmx/Genera_Pago",
                            dataType: "json",
                            contentType: 'application/json; utf-8',
                            data: "{idproyecto: @idproyecto,"
                                + " monto: @monto,"
                                + " mcomision: @mcomision,"
                                + " mcomisioniva: @mivacomision,"
                                + " montopagar: @montopagar,"
                                + " moneda: '@moneda',"
                                + " descripcion: 'Pago Proyecto M&E - @descripcion',"
                                + " pagoid: '" + details.orderID + "',"
                                + " metodo: 'paypal',"
                                + " auto_code: '',"
                                + " fpago: '',"
                                + " pago_info: '" + details.payerID + ' ' + details.paymentID + "',"
                                + " tipohook: '',"
                                + " pagado: 1}",
                            success: function (data) {
                                if (data.d != "OK") {
                                    alert(data.d);
                                } else {
                                    alert('PAGO APLICADO');
                                    $("#errpago").val("OK");
                                    $("#frm-paypal").submit();
                                    //setTimeout(function () { $("#frm-contrata").submit(); }, 3000);
                                }
                            }, //End of AJAX Success function  
                            failure: function (data) {
                                alert("Fail: " + data.responseText);
                            }, //End of AJAX failure function  
                            error: function (data) {
                                alert("Error: " + data.responseText);
                                //$("#msgpay").html(data.responseText);
                            } //End of AJAX error function  
                        });
                    });
                },
                onCancel: function (data) {
                    alert("Tu pago fue Cancelado");
                },
                onError: function (err) {
                    alert("ERROR al procesar pago " + err);
                    //$("#msgpay").html(err);
                    // Show an error page here, when an error occurs
                }
                //,
                //note_to_payer: 'Contact us for any questions on your order.'
            }, '#paypal-button');

        });
    </script>
End Section