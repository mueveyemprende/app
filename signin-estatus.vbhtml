@code
    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim ViewMsg As String = ""
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "_LoginLayout.vbhtml"
    PageData("Title") = "Registro Estatus"
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
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_Landing(idioma, "signin-poolin", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

End Code

@section head
    <!-- Style full screen-->
    <style type="text/css">

        html, body {
            margin: auto;
            height: 100%;
        }

    </style>

End Section

<section class="full no-padding">
	<div class="container-fluid">
		<div class="row align-items-center justify-content-center">
            <div class="col-lg-4 col-md-10 signin-login">
				<div class="logo">
                    <a href="/"><img src="~/img/logo-layout.png?1.1" alt="M&E logo"></a>
                </div>
				<div class="clearfix"></div>
                <div class="row">
                    <div class="col-md-12">
                        <p class="text-white">
                            <h3 class="text-white" >
                                El Pre-Registro se realizó con éxito. 
                            </h3>
                            <span class="text-white">
                            En breve te llegará un correo con la confirmación de tu registro con las instrucciones 
                            para que actives tu cuenta. 
                            En caso de que no te llegue, por favor verifica en la bandeja de correos no deseados
                            y verifica nuestro dominio (mueveyemprende.io) para que los correos que te enviemos no vuelvan a irse ahí.
                            </span>
                        </p>
                        <form action="/">
                            <p>
                                <button type="submit" class="btn white bg-blue-2 up pull-right"><span class="text-capitalize"   >Inicio</span></button>
                            </p>
                        </form>
                    </div>
                </div>
			</div>
		</div>
	</div>
</section>	

