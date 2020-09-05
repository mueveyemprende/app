@functions
    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim ViewMsg As String = ""

    Protected Sub Grabar_PreRegistro()
        Dim c As New poolin_class.cComunes
        Dim r As New poolin_class.cPreRegistro

        If Not r.Existe_Email(Request.Form("email"), strConn) Then
            Try
                Dim tipo As String = Request.Form("tipo")

                If tipo = "" Then
                    tipo = "P"
                End If

                r.id = 0
                If IsNothing(Request.Form("nombres")) Then
                    r.nombres = ""
                Else
                    r.nombres = Request.Form("nombres")
                End If

                r.apellidos = "" ' Request.Form("lastname")
                r.email = Request.Form("email")
                r.pwd = Request.Form("pwd")
                r.idpais = 0
                r.tipo = tipo
                r.termycond = True
                r.enviomails = False
                r.empresa = ""
                r.fbid = ""
                r.fb = ""
                r.idaccion = "A"
                r.PreRegistoEmprendedor(strConn)
                Dim objSend As New clanding
                objSend.Envio_PreRegistro(r.id, r.email, r.pwd, ConfigurationManager.AppSettings("app-me").ToString, ConfigurationManager.AppSettings("sendgridkey").ToString, strConn)
                objSend = Nothing
                Response.Redirect("signin-estatus", False)
                'Response.Redirect(ConfigurationManager.AppSettings("app-me").ToString & "login?e=" & c.Encrypt(r.id) & "&signin=" & c.Encrypt("Poolin"))
            Catch ex As Exception
                If r.id = -1001 Then
                    ViewMsg = "El correo electrónico ya está siendo utilizado en una cuenta."
                Else
                    ViewMsg = ex.Message.ToString & ": Hubo algún error. Estamos trabajando para solucionar estos inconvenientes."
                End If
            End Try
        Else
            ViewMsg = "El correo electrónico ya está siendo utilizado en una cuenta."
        End If

    End Sub


    Private Sub Graba_Login(ByVal idemprendedor As Long, ByVal usuario As String)

        Dim strHostName As String = Dns.GetHostName()
        Dim ipEntry As IPHostEntry = Dns.GetHostEntry(strHostName)

        Dim IPAddress As String = Convert.ToString(ipEntry.AddressList(ipEntry.AddressList.Length - 1))
        Dim HostName As String = Convert.ToString(ipEntry.HostName)

        Dim IPBehindProxy As String = ""

        'Find IP Address Behind Proxy Or Client Machine In ASP.NET
        Dim IPAdd As String = String.Empty

        IPAdd = Request.ServerVariables("HTTP_X_FORWARDED_FOR")

        If String.IsNullOrEmpty(IPAdd) Then
            IPAdd = Request.ServerVariables("REMOTE_ADDR")
            IPBehindProxy = IPAdd
        End If
        Dim c As New poolin_class.cComunes
        c.Graba_Ingreso(idemprendedor, usuario, IPAddress, HostName, IPBehindProxy, ConfigurationManager.ConnectionStrings("SQLConn").ToString)
    End Sub
End Functions
@code
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "_LoginLayout.vbhtml"
    PageData("Title") = "Registro"
    PageData("idpage") = "signin"

    Dim flag = "mx"
    Dim idioma = "ESP"
    Dim extIdioma = ""
    If Request.Form("idioma") <> "" Then
        idioma = Request.Form("idioma")
        If Request.Form("idioma") = "ING" Then
            flag = "us"
        End If
        Response.Cookies("Poolin-flag").Value = flag
        Response.Cookies("Poolin-idioma").Value = Request.Form("idioma")
    Else
        If Not IsNothing(Request.Cookies("Poolin-idioma")) Then
            flag = Request.Cookies("Poolin-flag").Value
            idioma = Request.Cookies("Poolin-idioma").Value
        End If
    End If
    extIdioma = idioma

    Dim objComun As New poolin_class.cComunes
    Dim strConn = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_Landing(idioma, "signin-poolin", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    Validation.RequireField("email", PageData("msgval-1"))
    Validation.RequireField("pwd", PageData("msgval-2"))
    Validation.RequireField("conf-pwd", PageData("msgval-3"))
    Validation.Add("conf-pwd", Validator.EqualsTo("pwd", PageData("msgval-4")))
    If Request.Form("signin") = "on" Then
        If Request.Form("fb") = "on" Then
            Dim objPre As New poolin_class.cPreRegistro
            Dim idemp = 0
            Dim tipo = ""
            Dim estatus = ""
            Dim pwd As String = objPre.recupera_pwd(idemp, Request.Form("email"), ConfigurationManager.ConnectionStrings("SQLConn").ToString, tipo, estatus)
            If idemp = 0 Then
                Grabar_PreRegistro()
            Else
                If estatus = "C" Then
                    ViewMsg = PageData("msgval-5")
                    'ViewState("err501") = "Pongase en contacto con un asesor Poolin."
                    'Response.Redirect("~/login", False)
                Else
                    If Request.Form("recordarpwd") = "on" Then
                        Response.Cookies("pwdpool").Value = pwd
                        Response.Cookies("usrpool").Value = Request.Form("email")
                        Response.Cookies("pwdpool").Expires = Now.AddYears(20)
                        Response.Cookies("usrpool").Expires = Now.AddYears(20)
                    Else
                        Response.Cookies("pwdpool").Value = ""
                        Response.Cookies("usrpool").Value = ""
                        Response.Cookies("pwdpool").Expires = Now.AddYears(-20)
                        Response.Cookies("usrpool").Expires = Now.AddYears(-20)
                    End If
                    'Ingreso correcto
                    Graba_Login(idemp, Request.Form("email"))
                    Response.Cookies("tipo").Value = tipo
                    Dim e As New poolin_class.cEmprendedor
                    Dim dt As System.Data.DataTable = e.Datos_Emprendedor(idemp, ConfigurationManager.ConnectionStrings("SQLConn").ToString)
                    Dim cm As New poolin_class.cComunes
                    Response.Redirect("https://mueveyemprende.io/login?e=" & cm.Encrypt(idemp))
                End If
            End If
        Else
            Grabar_PreRegistro()
        End If
    End If
    Dim email As String = ""
    If Not IsNothing(Request.Form("email")) Then
        email = Request.Form("email")
    End If
End Code

@section head
    @*<script src="https://apis.google.com/js/platform.js" async defer></script>*@
    
    @*6LeOjlsUAAAAAI2bGabGSLc30oXmUDK7vqM3Bvkm*@
    <script type="text/javascript">
      var onloadCallback = function() {
        grecaptcha.render('html_element', {
            'sitekey': '6LfjnsEZAAAAAAM5DT7DXG4kEqus9MALs1D2xyis',
            'theme': 'light'
        });
      };
    </script>
    <!-- Style full screen-->
    <style type="text/css">

        html, body {
            margin: auto;
            height: 100%;
        }

.g-recaptcha {
    position: relative;
    width: 100%;
    background: #f9f9f9;
    overflow: hidden;
}

.g-recaptcha > * {
    float: right;
    right: 0;
    margin: -2px -2px -10px;/*remove borders*/ 
}

.g-recaptcha::after{
    display: block;
    content: "";
    position: absolute;
    left:0;
    right:150px;
    top: 0;
    bottom:0;
    background-color: #f9f9f9;
    clear: both;
}
    </style>

End Section

<section class="full no-padding">
	<div class="container-fluid">
		<div class="row align-items-center justify-content-center">
            <div class="col-lg-4 col-md-10 signin-login">
				<div class="logo">
                    <a href="/"><img src="~/img/logo-layout.png?1.1" alt="poolin-logo"></a>
                </div>
				<form id="frmsignin" action="~/signin-me" method="post" onsubmit="return correctCaptcha();">
                    <input type="hidden" name="signin" value="on"/>
                    <input type="hidden" id="fb" name="fb" value=""/>
                    <input type="hidden" id="nombres" name="nombres" value=""/>
                    <input type="hidden" id="tipo" name="tipo" value="" />
                    <div class="row">
                        <div class="col-md-12">
                            <span class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("email")</strong></span>
                            <input type="email" class="form-control"  id="email" name="email" placeholder="@Html.Raw(PageData("place-1"))" value="@email" @Validation.For("email")>
                            <span  class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("pwd")</strong></span>
                            <input type="password" class="form-control"  id="pwd" name="pwd" placeholder="@Html.Raw(PageData("place-2"))" @Validation.For("pwd")>
                            <span  class="text-danger"  style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("conf-pwd")</strong></span>
                            <input type="password" class="form-control"  id="conf-pwd" name="conf-pwd" placeholder="@Html.Raw(PageData("place-3"))" @Validation.For("conf-pwd") >
                            <div id="html_element" class="pull-right"></div>
                        </div>
                    </div>
                    @If ViewMsg <> "" Then
                        @<div Class="row align-items-center">
                            <span class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@ViewMsg</strong></span>
                        </div>
                    End If

					<div class="row align-items-center">
						<div class="col-xl-7 no-padding">
							<small class="white">Al registrarte, confirmas que aceptas los <a href="me-terminos" target="_blank">Términos y Condiciones</a> y que conoces el <a href="~/me-aviso-privacidad" target="_blank">Aviso de Privacidad</a></small>
						</div>
						<div class="col-xl-5 no-padding center">
							<button id="btnsubmit" type="submit" class="btn white bg-blue-2 up pull-right"><span style="text-transform:capitalize">@Html.Raw(PageData("text-5"))</span></button>
						</div>						
					</div>
                    @*<script>onload();</script>*@
				    <div class="clearfix"></div>
				    <div class="registro-redes">
					    @*<ul class="no-padding">
						    <li class="nostyle fb-btn"><a href="#"><i class="fab fa-facebook-f"></i>Registrarse con Facebook</a></li>
						    <li class="nostyle gmail-btn"><a href="#"><i class="fab fa-google"></i>Registrarse con Gmail</a></li>
					    </ul>*@
			            <table style="width:100%;">
				            <tr>
                                <td align="center" style="padding-bottom:15px;">
                                @*<div style="width:100%; height:40px; background-color:#3b5998; vertical-align:middle; border-radius:200px;">
                                <fb:login-button  scope="public_profile, email" size="large" onlogin="SignupFB();" style="padding-top:8px;">
                                    Registrarme
                                </fb:login-button>
                                </div>*@
                                <div class="fb-login-button" data-max-rows="1" data-size="large" data-button-type="login_with" data-show-faces="false" data-auto-logout-link="false" data-use-continue-as="false" onlogin="SignupFB();"></div>
                                </td>
                            </tr>
				            <tr>
                                <td align="center" style="padding-bottom:15px;">
                                <div id="my-signin2"></div>
                                    @*<div class="g-signin2" data-onsuccess="onSignIn" data-longtitle="false" style="padding-left:50px;" ></div>*@
                                </td>
                            </tr>
			            </table>
                        <script src="https://apis.google.com/js/platform.js?onload=renderButton" async defer></script>
				    </div>
				    <div class="col-12 center">
					    <small class="white">@Html.Raw(PageData("text-6")) <a href="login-me" >@Html.Raw(PageData("text-7"))</a></small>
				    </div>
				</form>
                <script src="https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit" async defer></script>

			</div>
		</div>
	</div>
</section>	

@section Scripts
    
    <script>
        window.fbAsyncInit = function () {
            FB.init({
                appId: '1656096664695973',
                cookie: true,
                xfbml: true,
                version: 'v2.8'
            });
            FB.AppEvents.logPageView();
        };

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = 'https://connect.facebook.net/es_LA/sdk.js#xfbml=1&autoLogAppEvents=1&version=v3.0&appId=1656096664695973';
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));


        // This is called with the results from from FB.getLoginStatus().
        function statusChangeCallback(response, signup) {
            console.log('statusChangeCallback');
            console.log(response);

            // The response object is returned with a status field that lets the
            // app know the current login status of the person.
            // Full docs on the response object can be found in the documentation
            // for FB.getLoginStatus().
            if (response.status === 'connected') {
                // Logged into your app and Facebook.

                if (signup == 0) {
                    LoginAPI();
                } else {
                    SignupAPI();
                }

            } else {
                // The person is not logged into your app or we are unable to tell.
                document.getElementById('status').innerHTML = 'Please log ' +
                    'into this app.';
            }
        }

        // This function is called when someone finishes with the Login
        // Button.  See the onlogin handler attached to it in the sample
        // code below.
        function LoginFB() {
            FB.getLoginStatus(function (response) {
                statusChangeCallback(response, 0);
            });
        }

        function SignupFB() {
            FB.getLoginStatus(function (response) {
                statusChangeCallback(response, 1);
            });
        }

        // Here we run a very simple test of the Graph API after login is
        // successful.  See statusChangeCallback() for when this call is made.
        function LoginAPI() {
            console.log('Welcome!  Fetching your information.... ');
            FB.api('/me', { fields: 'name,email,first_name,last_name,currency,id,locale,link' }, function (response) {
                //console.log('Successful login for: ' + response.name + response.id + response.email + response.email);
                //document.getElementById('status').innerHTML = 'Thanks for logging in, ' + response.name + response.first_name + response.last_name + response.email + '!';
                document.getElementById("email").value = response.email;
                document.getElementById("pwd").value = response.id;
                document.getElementById("conf-pwd").value = response.id;
                document.getElementById("nombres").value = response.first_name & " " & response.last_name;
                document.getElementById("login").value = "on";
                document.getElementById("fb").value = "on";
                document.getElementById("frmsignin").submit();
            });
        }

        function SignupAPI() {
            FB.api('/me', { fields: 'name,email,first_name,last_name,currency,id,locale,link' }, function (response) {

                //console.log('Successful login for: ' + response.name + response.id + response.email + response.email);
                //document.getElementById('status').innerHTML = 'Thanks for logging in, ' + response.name + response.first_name + response.last_name + response.email + '!';
                document.getElementById("email").value = response.email;
                document.getElementById("pwd").value = response.id;
                document.getElementById("conf-pwd").value = response.id;
                document.getElementById("nombres").value = response.first_name & " " & response.last_name;
                document.getElementById("fb").value = "on";
                document.getElementById("btnsubmit").click();
            });
        }

        function onSignIn(googleUser) {
            var profile = googleUser.getBasicProfile();
            //console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
            //console.log('Name: ' + profile.getName());
            //console.log('Image URL: ' + profile.getImageUrl());
            //console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
            document.getElementById("email").value = profile.getEmail();
            document.getElementById("pwd").value = profile.getId();
            document.getElementById("conf-pwd").value = profile.getId();
            document.getElementById("nombres").value = profile.getName();
            document.getElementById("fb").value = "on";
            //document.getElementById("frmsignin").submit();
            document.getElementById("btnsubmit").click();
            signOut();
        }

        function signOut() {
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut().then(function () {
                console.log('User signed out.');
            });
        }
    </script>

    <script>
      function onSubmit(token) {
          document.getElementById("frmsignin").submit();
      }

        function onSuccess(googleUser) {
            console.log('Logged in as: ' + googleUser.getBasicProfile().getName());
        }
        function onFailure(error) {
            console.log(error);
        }
        function renderButton() {
            gapi.signin2.render('my-signin2', {
                'scope': 'profile email',
                'width': 282,
                'height': 40,
                'longtitle': true,
                'theme': 'dark',
                'onsuccess': onSignIn,
                'onfailure': onFailure
            });
        }

        function correctCaptcha() {
            var response = grecaptcha.getResponse();
            if (response.length == 0) {
                alert("Captcha no verificado");
                return false
            }


        }
    </script>

End Section