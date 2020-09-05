@Functions
    Dim ViewMsg = ""
    Dim Titmsg = "Listo."
    Protected Sub Recupera_Contrasena()

        'Using smtp As New SmtpClient
        '    Dim correo As New MailMessage

        Dim htmlmsg As String
        Using f As New StreamReader(Server.MapPath("~/poolin_arch/txt/envio_pwd.txt"))
            htmlmsg = f.ReadToEnd
            f.Close()
        End Using

        Dim oPreReg As New poolin_class.cPreRegistro
        Dim objComun As New poolin_class.cComunes
        Dim user_pwd As String
        Dim idemp As Long = 0

        user_pwd = oPreReg.recupera_pwd(idemp, Request.Form("email"), ConfigurationManager.ConnectionStrings("SQLConn").ToString)

        If idemp <> 0 Then
            Dim token = oPreReg.Token_NewPass(idemp, ConfigurationManager.ConnectionStrings("SQLConn").ToString)
            If token = "" Then
                Titmsg = "ERROR"
                ViewMsg = "Error al generar el reenvío, vuelve a intentarlo."
            End If
            htmlmsg = htmlmsg.Replace("[LINKNEWPWD]", "https://mueveyemprende.io/me-newpass?token=" & token)
            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString
            Dim objSend As New poolin_class.cSendGrid
            objSend.Correo_SMTP("Recuperación de Contraseña", htmlmsg, Request.Form("email"), "", ConfigurationManager.ConnectionStrings("SQLConn").ToString)
            ViewMsg = "Te hemos enviado un correo para que recuperes tu cuenta M&E."
        Else
            Titmsg = "ERROR"
            ViewMsg = "Tu correo no existe en nuestros registros, por favor verifícalo y vuelve a intentar."
        End If
    End Sub


End Functions


@Code
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "_LoginLayout.vbhtml"
    PageData("idpage") = "login"
    PageData("Title") = "Recuperación de Contraseña"

    Validation.RequireField("email", "* Debes indicar tu correo electrónico.")
    'Validation.Add("pwd",
    '    Validator.StringLength(
    '    maxLength:=Int32.MaxValue,
    '    minLength:=6,
    '    errorMessage:="La contraseña debe tener al menos seis caracteres"))
    If Request.Form("login") = "on" Then
        Try
            Recupera_Contrasena()
        Catch ex As Exception
            Titmsg = "ERROR."
            ViewMsg = ex.Message
        End Try
    End If

End Code

<section class="full no-padding">
	<div class="container-fluid">
		<div class="row align-items-center justify-content-center">
			<div class="col-sm-4 signin-login">
				<div class="logo">
                    <a href="/"><img src="~/img/logo-layout.png?1.1" alt="poolin-logo"></a>
                </div>
                <div id="pwd-send">
                <div class="col-12 center">
					<h2 class="white">¿Olvidaste tu contraseña?</h2>
					<p class="white">Ingresa aquí tu correo registrado.</p>
				</div>
                <form id="formlogin" method="post" action="pwd-me">
                    <input type="hidden" id="login" name="login" value="on" />
                    <span  class="msgval">@Html.ValidationMessage("email")</span>
                    <input type="email" class="form-control" id="email" name="email" placeholder="correo electrónico" @Validation.For("email")>
					<div class="row justify-content-end">
						<input type="submit" class="btn white bg-blue-2 up pull-right" onclick="" value="Enviar">
					</div>
                    <div class="clearfix"></div>
                </form>
                </div>
                <div id="pwd-msg" style="display:none">
                    <h3 class="text-white">@ViewMsg</h3>
                    <a href="/" class="btn white bg-blue-2 up pull-right"><span class="text-capitalize">Regresar</span></a>
                </div>
			</div>
		</div>
	</div>
</section>	

@section Scripts
    @*<script>
        var login = "@Request.Form("login")";
        if (login == "on")
        {
            $('#ok-poolin').removeClass("pwd-poolin hidden");
            $('#ok-poolin').toggleClass("pwd-poolin");
            $('#pwd-poolin').removeClass("pwd-poolin");
            $('#pwd-poolin').toggleClass("hidden");
        }
    </script>*@
    <script>
        var viewmsg = "@ViewMsg"
        if (viewmsg != "") {
            document.getElementById("pwd-send").style.display = "none";
            document.getElementById("pwd-msg").style.display = "";
        }
    </script>
End Section



