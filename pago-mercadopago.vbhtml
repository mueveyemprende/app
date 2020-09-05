@Code
    Layout = "_PoolinLayout.vbhtml"
    Dim wspago = New wsseguimiento
    Dim pagoid As String = Request.Form("payment_id")
    Dim metodo As String = Request.Form("payment_status")
    Dim auto_code As String = Request.Form("preference_id")
    Dim fpago As String = "MERCADO PAGO"
    Dim pago_info As String = Request.Form("merchant_order_id")
    Dim tipohook As String = ""
    Dim pagado As Int16 = 0

    If metodo.ToUpper = "APPROVED" Then
        pagado = 1
        metodo = "mercadopago"
    End If

    Dim estpago = wspago.Genera_Pago(Request.Form("idproyecto"),
            Request.Form("monto"),
            Request.Form("mcomision"),
            Request.Form("mcomisioniva"),
            Request.Form("montopagar"),
            Request.Form("moneda"),
            Request.Form("descripcion"), pagoid, metodo, auto_code, fpago, pago_info, tipohook, pagado)
End Code

<div>
    <form id="frmregresa" method="post" action="~/user-proyecto-seguimiento">
        <input  name="idproyecto" value="@Request.Form("idproyecto")" type="hidden" />
        <input name="msg" id="msg" value="PAGO" type="hidden" />
        <input name="errpago" value="@estpago" type="hidden"  />
        Redireccionando a Seguimientos... 
    </form>
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
	<script>
		$(document).ready(function () {
            $("#frmregresa").submit();
		});
	</script>
End Section