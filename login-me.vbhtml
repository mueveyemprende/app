@Functions
    Dim ViewMsg = ""
    Protected Sub Grabar_PreRegistro(ByVal nombres As String, ByVal email As String, ByVal pwd As String)
        Dim c As New poolin_class.cComunes
        Dim r As New poolin_class.cPreRegistro
        Try
            Dim tipo As String = Request.Form("tipo")

            If tipo = "" Then
                tipo = "P"
            End If

            r.id = 0
            r.nombres = Request.Form("nombres")
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
            r.PreRegistoEmprendedor(ConfigurationManager.ConnectionStrings("SQLConn").ToString)
            Response.Redirect("login?e=" & c.Encrypt(r.id) & "&signin=" & c.Encrypt("Poolin"))
        Catch ex As Exception
            If r.id = -1001 Then
                ViewMsg = "El correo electrónico ya está siendo utilizado en una cuenta."
            Else
                ViewMsg = ex.Message
            End If
        End Try
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


@Code
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "~/_LoginLayout.vbhtml"
    PageData("Title") = "Login"
    PageData("idpage") = "login"

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
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_Landing(idioma, "login-poolin", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    Validation.RequireField("email", PageData("msgval-1"))
    Validation.RequireField("pwd", PageData("msgval-2"))

    'Validation.Add("pwd",
    '    Validator.StringLength(
    '    maxLength:=Int32.MaxValue,
    '    minLength:=6,
    '    errorMessage:="La contraseña debe tener al menos seis caracteres"))
    Dim ViewMsg As String = ""
    If Request.Form("login") = "on" Then
        Dim p As New poolin_class.cPreRegistro
        Dim idemp As Long = 0
        Dim tipo As String = "" 'Persona o Empresa
        Dim estatus As String = ""

        Dim pwd As String = p.recupera_pwd(idemp, Request.Form("email"), ConfigurationManager.ConnectionStrings("SQLConn").ToString, tipo, estatus)
        If idemp = 0 Then
            'Usuario no encontrado
            If Request.Form("fb") = "on" Then
                If Request.Form("chk-recordar") = "on" Then
                    Response.Cookies("check-poolin").Value = "ok"
                    Response.Cookies("pwdpool").Value = pwd
                    Response.Cookies("usrpool").Value = Request.Form("email")
                    Response.Cookies("pwdpool").Expires = Now.AddYears(20)
                    Response.Cookies("usrpool").Expires = Now.AddYears(20)
                Else
                    Response.Cookies("check-poolin").Value = ""
                    Response.Cookies("pwdpool").Value = ""
                    Response.Cookies("usrpool").Value = ""
                    Response.Cookies("pwdpool").Expires = Now.AddYears(-20)
                    Response.Cookies("usrpool").Expires = Now.AddYears(-20)
                End If
                Grabar_PreRegistro(Request.Form("nombres"), Request.Form("email"), Request.Form("pwd"))
            End If

            ViewMsg = PageData("msgval-3")
            'ViewMsg &= "Si no te acuerdas de tu contraseña solicita sea reestablecida y te mandaremos un link para que puedas acceder a Poolin, entrar y cambiarla."
            'ViewState("login") = "0"
            'Response.Redirect("~/login", False)
        Else
            Dim pwdform As String = Request.Form("pwd")
            If Request.Form("fb") = "on" Then
                pwdform = pwd
            End If
            'If fb = "fb" Then
            '    pwdform = pwd
            'End If
            If pwd <> pwdform Then
                'Password incorecto
                ViewMsg = PageData("msgval-3")
                'ViewState("err501") = "Si no te acuerdas de tu contraseña solicita sea reestablecida y te mandaremos un link para que puedas acceder a Poolin, entrar y cambiarla."
                'Response.Redirect("~/login", False)
            Else
                If estatus = "C" Then
                    ViewMsg = PageData("msgval-4")
                    'ViewState("err501") = "Pongase en contacto con un asesor Poolin."
                    'Response.Redirect("~/login", False)
                Else
                    If Request.Form("chk-recordar") = "on" Then
                        Response.Cookies("check-poolin").Value = "ok"
                        Response.Cookies("pwdpool").Value = pwd
                        Response.Cookies("usrpool").Value = Request.Form("email")
                        Response.Cookies("pwdpool").Expires = Now.AddYears(20)
                        Response.Cookies("usrpool").Expires = Now.AddYears(20)
                    Else
                        Response.Cookies("check-poolin").Value = ""
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
                    Response.Redirect("login?e=" & cm.Encrypt(idemp))

                End If

            End If
        End If

    End If
    Dim check = ""
    Dim rec_usuario = ""
    Dim rec_pwd = ""
    If Not IsNothing(Response.Cookies("check-poolin")) Then
        If Request.Cookies("check-poolin").Value = "ok" Then
            check = "checked"
            rec_usuario = Request.Cookies("usrpool").Value
            rec_pwd = Request.Cookies("pwdpool").Value
        End If
    End If
End Code

@section head

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
                document.getElementById('email').value = response.email;
                document.getElementById('pwd').value = response.id;
                document.getElementById("nombres").value = response.first_name & " " & response.last_name;
                document.getElementById('login').value = "on";
                document.getElementById('fb').value = "on";
                document.getElementById('formlogin').submit();
            });
        }

        function SignupAPI() {
            FB.api('/me', { fields: 'name,email,first_name,last_name,currency,id,locale,link' }, function (response) {

                //console.log('Successful login for: ' + response.name + response.id + response.email + response.email);
                //document.getElementById('status').innerHTML = 'Thanks for logging in, ' + response.name + response.first_name + response.last_name + response.email + '!';
                document.getElementById("email").value = response.email;
                document.getElementById("pwd").value = response.id;
                document.getElementById("nombres").value = response.first_name & " " & response.last_name;
                document.getElementById("fb").value = "on";
                document.getElementById('formlogin').submit();
                //document.getElementById('email2').value = response.email;
                //document.getElementById('emailppal').value = response.email;
                //document.getElementById('fbid').value = response.id;
                //document.getElementById('fblink').value = response.link;
                //document.getElementById('firstname').value = response.first_name;
                //document.getElementById('lastname').value = response.last_name;
                //document.getElementById('passwordppal').value = response.id;
                //document.getElementById('passconfirm').value = response.id;
                //document.getElementById('secpass1').style.display = 'none';
                //document.getElementById('secpass2').style.display = 'none';
                //document.getElementById('secemail').style.display = 'none';
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
            document.getElementById("nombres").value = profile.getName();
            document.getElementById("fb").value = "on";
            signOut();
            document.getElementById('formlogin').submit();
        }

        function signOut() {
            var auth2 = gapi.auth2.getAuthInstance();
            auth2.signOut().then(function () {
                    console.log('User signed out.');
            });
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
    </script>

End Section

<section class="full no-padding">
	<div class="container-fluid">
		<div class="row align-items-center justify-content-center">
			<div class="col-lg-4 col-md-10 col-sm-10 signin-login">
				<div class="logo">
                    <a href="/"><img src="~/img/logo-layout.png?1.1" alt="poolin-logo"></a>
                </div>
				<form id="formlogin" method="post" action="login-me">
                    <input type="hidden" id="login" name="login" value="on" />
                    <input type="hidden" id="fb" name="fb" value="" />
                    <input type="hidden" id="nombres" name="nombres" value="" />
                    <span  class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("email")</strong></span>
                    <input type="email" id="email" name="email" class="form-control" value="@rec_usuario" placeholder="@Html.Raw(PageData("place-1"))" @Validation.For("email")>
                    <span  class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("pwd")</strong></span>
                    <input type="password" id="pwd" name="pwd" class="form-control" value="@rec_pwd" placeholder="@Html.Raw(PageData("place-2"))" @Validation.For("pwd")>

                    @If ViewMsg <> "" Then
                        @<div Class="row align-items-center">
                            <span class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@ViewMsg</strong></span>
                        </div>
                    End If
                    <div class="row align-items-center">
						<div class="col-xs-7 col-sm-7 col-md-7 col-lg-7 col-xl-7 no-padding">
							<small>
								<input id="recordar" type="checkbox" name="chk-recordar" @check>
								<label class="white" for="recordar"> @Html.Raw(PageData("text-1")) | <a href="pwd-me" class="white">@Html.Raw(PageData("text-2"))</a></label>
							</small>
						</div>
						<div class="col-xs-5 col-sm-5 col-md-5 col-lg-5 col-xl-5 no-padding">
							<input type="submit" class="btn white bg-blue-2 pull-right" onclick="" value="@Html.Raw(PageData("text-3"))">
						</div>						
					</div>
				</form>
				<div class="clearfix"></div>
				<div class="registro-redes">
			        <table style="width:100%;">
				        <tr>
                            <td align="center" style="padding-bottom:15px;">
                                <div id="fb-root"></div>
                                <script>
                                    (function (d, s, id) {
                                        var js, fjs = d.getElementsByTagName(s)[0];
                                        if (d.getElementById(id)) return;
                                        js = d.createElement(s); js.id = id;
                                        js.src = 'https://connect.facebook.net/es_LA/sdk.js#xfbml=1&autoLogAppEvents=1&version=v3.0&appId=1656096664695973';
                                        fjs.parentNode.insertBefore(js, fjs);
                                    }(document, 'script', 'facebook-jssdk'));
                                </script>
                                <div class="fb-login-button" data-max-rows="1" data-size="large" data-button-type="login_with" data-show-faces="false" data-auto-logout-link="false" data-use-continue-as="false" onlogin="SignupFB();"></div>
                                @*<fb:login-button  scope="public_profile, email" onlogin="SignupFB();">
                                </fb:login-button>*@
                            </td>
                        </tr>
                        <tr>
                            <td align="center" style="padding-bottom:30px;">
                                <div id="my-signin2"></div>
                                @*<div class="g-signin2" data-onsuccess="onSignIn"></div>
                                    *@
                                
                            </td>
                        </tr>

                    </table>
                     <script src="https://apis.google.com/js/platform.js?onload=renderButton" async defer></script>
					@*<ul class="no-padding">
						<li class="nostyle">
                            <div style="width:100%; height:40px; background-color:#3b5998; vertical-align:middle; border-radius:200px;">
                            </div>
                        </li>
						<li class="nostyle">
                            <div style="width:100%; height:40px; background-color:#fff; text-align:center; vertical-align:middle; border-radius:200px;">
                            </div>
                        </li>
					</ul>*@
				</div>
				<div class="col-12 center">
					<small class="white">@Html.Raw(PageData("text-4")) <a href="signin-me" >@Html.Raw(PageData("text-5"))</a></small>
				</div>
			</div>
		</div>
	</div>
</section>	