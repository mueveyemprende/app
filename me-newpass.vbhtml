@Code
    Layout = "_LoginLayout.vbhtml"
    PageData("idpage") = "login"
    PageData("Title") = "Cambio de Contraseña"

    Validation.RequireField("pwd", "Indica la contraseña")
    Validation.RequireField("conf-pwd", "Indica la confirmación")
    Validation.Add("conf-pwd", Validator.EqualsTo("pwd", "La confirmación no coincide"))
    Dim ViewMsg As String = ""
    Dim objEmp As New poolin_class.cEmprendedor
    Dim objComun As New poolin_class.cComunes
    Dim idEmp As Long = 0
    Dim token = Request.QueryString("token")
    If Request.Form("token") <> "" And Request.Form("idemp") <> "" Then
        objEmp.Actualiza_Pwd(Request.Form("idemp"),
                             "NEW",
                             Request.Form("pwd"), ConfigurationManager.ConnectionStrings("SQLConn").ToString, "A")
        Response.Redirect("login-me", False)
    Else
        If Request.QueryString("token") = "" Then
            Response.Redirect("https://mueveyemprende.io", False)
        Else
            Try
                'If token = "" Then
                ' Response.Redirect("https://mueveyemprende.io", False)
                'End If
                idEmp = objEmp.Get_AutoToken(Request.QueryString("token"), ConfigurationManager.ConnectionStrings("SQLConn").ToString)
                If idEmp <> 0 Then
                    Dim dt As System.Data.DataTable = objEmp.Datos_Emprendedor(idEmp, ConfigurationManager.ConnectionStrings("SQLConn").ToString)
                    If dt.Rows.Count = 0 Then
                        Response.Redirect("https://mueveyemprende.io", False)
                    End If
                Else
                    Response.Redirect("https://mueveyemprende.io", False)
                End If
            Catch ex As Exception
                Response.Redirect("https://mueveyemprende.io", False)
            End Try

        End If

    End If



End Code


<section class="full no-padding">
	<div class="container-fluid">
		<div class="row align-items-center justify-content-center">
			<div class="col-sm-4 signin-login">
				<div class="logo">
                    <a href="/"><img src="~/img/logo-layout.png?1.1" alt="Poolin Logo"></a>
                </div>
                <div id="pwd-send">
                <div class="col-12 center">
					<h2 class="white">Cambio de Contraseña</h2>
					<p class="white">Ingresa tu nueva contraseña y la confirmación.</p>
				</div>
                <form id="formlogin" method="post" action="me-newpass">
                    <input name="token" value="@token" type="hidden" />
                    <input name="idemp" value="@idEmp" type="hidden" />
                    <div class="row">
                        <div class="col-md-12">
                            <span  class="text-danger" style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("pwd")</strong></span>
                            <input type="password" class="form-control"  id="pwd" name="pwd" placeholder="Contraseña" @Validation.For("pwd")>
                            <span  class="text-danger"  style="font-weight:bold; line-height:30px;"><strong>@Html.ValidationMessage("conf-pwd")</strong></span>
                            <input type="password" class="form-control"  id="conf-pwd" name="conf-pwd" placeholder="Confirmación" @Validation.For("conf-pwd") >
                        </div>
                    </div>
					<div class="row justify-content-end">
						<input type="submit" class="btn white bg-blue-2 up pull-right" onclick="" value="Enviar">
					</div>
                    <div class="clearfix"></div>
                </form>
                </div>
                <div id="pwd-msg" style="display:none">
                    <h3>@ViewMsg</h3>
                    <a href="/" class="btn white bg-blue-2 up pull-right">Regresar</a>
                </div>
			</div>
		</div>
	</div>
</section>	
