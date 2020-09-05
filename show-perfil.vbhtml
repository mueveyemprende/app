@code
    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim idemp = Request.QueryString("idemp")

    Layout = "_ShowLayout.vbhtml"

    Dim objComun As New poolin_class.cComunes
    Dim objEmp As New poolin_class.cEmprendedor

    Dim avatar As String = "avatar_"

    Dim pais As String = "México"

    Dim dtGral As System.Data.DataTable
    Dim dtServ As System.Data.DataTable 'Utilizamos la tabla emp_trabajos
    Dim dtPais As System.Data.DataTable
    Dim dtIdiomas As System.Data.DataTable
    Dim dtCursos As System.Data.DataTable
    Dim dtHabEmp As System.Data.DataTable
    Dim dtHab As System.Data.DataTable = objComun.Carga_Habilidades(strConn)

    Dim subcatsel As String = ""

    Dim objCat As New poolin_class.cCategorias
    Dim dtCat As System.Data.DataTable = objCat.Carga_Categorias(strConn)
    Dim dtSubCat As System.Data.DataTable

    If Not Directory.Exists(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images")) Then
        Directory.CreateDirectory(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images"))
    End If

    dtServ = objEmp.Datos_Trabajos(idemp, strConn)
    dtGral = objEmp.Datos_Emprendedor(idemp, strConn)
    dtIdiomas = objEmp.Datos_Idiomas(idemp, strConn)
    dtCursos = objEmp.Datos_Cursos(idemp, strConn)
    dtHabEmp = objEmp.Datos_Habilidades(idemp, strConn)
    If dtGral.Rows.Count = 0 Then
    Else
        dtPais = objComun.Carga_Paises(strConn, dtGral.Rows(0)("idpais"))
        If dtPais.Rows.Count <> 0 Then
            pais = dtPais.Rows(0)("nombre")
        End If
    End If

    Dim di As String = Dir(Server.MapPath("poolin_arch/images/png/_avatar_" & idemp & ".png"))
    If di <> "" Then
        avatar = di & "?" & Format(Now, "m.ss")
    End If

    Dim dtCatego As System.Data.DataTable = objEmp.SubCategos_Emp(idemp, strConn)

    For Each dr As System.Data.DataRow In dtCatego.Rows
        subcatsel &= "<a id='subcat-" & dr("id") & "' class='btn-categorias' value='" & dr("id") & "'> " & dr("subcategoria") & "</a> <input type='hidden' id='tsub-" & dr("id") & "' name='tsubcat[]' value='" & dr("id") & "'>"
    Next

    Dim habsel = ""

    Dim diPorta As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images/"))
    Dim diVideo As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/"))

    If Not diVideo.Exists Then
        Directory.CreateDirectory(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/"))
    End If

    dtPais = objComun.Carga_Paises(strConn)
    Dim filesporta As String = ""

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-perfil", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    Dim idporta = 0
    Dim checkemp = ""
    If Request.Form("checkemp") = "on" Then
        checkemp = "on"
    End If
    Dim idaccion = dtGral.Rows(0)("idaccion")
 End Code
@section head
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">

    <link href="assets/plugins/form/jquery-file-upload/css/jquery.fileupload.css" rel="stylesheet" />

End Section

<div id="content" class="container">
    <div class="row">
        <div class="col-sm-8 col-md-8 ">
            @*<h5 class="user_title inline">@PageData("titulo")</h5>*@

            <div class="row">
                <div class="col-lg-12">
                    <div class="misdatos-box">
                        <div class="row">
                            <div id="showfoto">
                                <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6 centrar">
                                    <img src="poolin_arch/images/png/@avatar" class="misdatos-foto">
                                </div>
                            </div>
                            <div id="showdata">
                                <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                    
                                    <div class="text_align">
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(IIf(dtGral.Rows(0)("nombres") = "", "<span class='text-danger'>Indica tu nombre<span>", dtGral.Rows(0)("nombres")))</div>    
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(dtGral.Rows(0)("email"))</div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @if "" & dtGral.Rows(0)("slogan") <> "" Then
                                                    @<span>@Html.Raw(dtGral.Rows(0)("slogan"))</span>
                                                Else
                                                    @<span class="text-gray-light">Profesión</span>
                                                End if
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(pais)</div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <span>@Html.Raw(String.Format("{0}, {1}", dtGral.Rows(0)("ciudad"), dtGral.Rows(0)("estado")))</span>    
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <div id="show-datos">
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="profile-box">
                                    @*<h2> @PageData("sub-titulo_2")</h2>*@
                                    <form class="" accept-charset="UTF-8" action="" method="post">
                                        <input type="hidden" name="s-emp" value="on" />
                                        <input type="hidden" name="idaccion" value="B"/>
                                        <div class="row">
                                            <div class="col-lg-8">
                                                <h3  class="user_subtitle inline">Información sobre mis servicios</h3>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <span id="selcat">
                                                </span>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h4>Servicios que ofrezco:</h4>
                                                <span class="msg-val">@Html.ValidationMessage("empresa")</span>
                                                <input type="text" name="empresa" readonly class="form-control" placeholder="@PageData("miperfil-label_1")" value="@Html.Raw(dtGral.Rows(0)("empresa"))" @Validation.For("empresa")>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <input type="text" class="form-control" readonly name="website" placeholder="@PageData("miperfil-label_3")" value="@Html.Raw(dtGral.Rows(0)("website"))">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <label></label>
                                                <textarea name="servgrales" maxlength="2000" rows="5" readonly class="form-control" placeholder="@PageData("miperfil-label_2")">@Html.Raw(dtGral.Rows(0)("servicios"))</textarea>
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>
                                    </form>
                                </div>
                            </div>
                        </div>
                        <div class="profile-box">
                            <h3 class="user_subtitle"> @PageData("sub-titulo_4") </h3>
                            <div class="row">
                                <div class="col-lg-6">
                                    <form id="frmporta" method="post" action="~/user-perfil#frmporta" enctype="multipart/form-data">
                                        <input type="hidden" name="saveporta" value="OK" />
                                        <input id="file-delete" name="file-delete" type="hidden" value="" />
                                        <div class="portfolio-wrap">
                                            <div class="row">
                                                @For Each fi In diPorta.GetFiles()
                                                    idporta += 1
                                                    @<div class="col-md-4  col-sm-4 col-xs-4">
                                                        <input id="file-@idporta" type="hidden" value="@Html.Raw(fi.Name)" />
                                                        <img id="img-@idporta" src="poolin_arch/portafolio/@idemp/images/@Html.Raw(fi.Name)" class="img-responsive" style="padding-bottom:10px;">
                                                    </div>
                                                Next
                                            </div>
                                        </div>
                                    </form>
                                </div>
                                <div class="col-lg-6">
                                    <form id="frmvideo" method="post" action="~/user-perfil" enctype="multipart/form-data">
                                        <div class="portfolio-wrap p-asesoria" style="margin-top:-20px">
                                                @if diVideo.Exists Then
                                                    For Each fi In diVideo.GetFiles()
                                                        @<div class="row">
                                                            <div class="col-md-10">
                                                                <input type="hidden" value="@Html.Raw(fi.Name)" />
                                                                @*<video id="img-@idporta" src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" class="img-responsive" style="float: right; padding:2px;">*@
                                                                <video controls style="width:100%;" id="video-pool" >
                                                                  <source src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" type="video/mp4">
                                                                  <source src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" type="video/ogg">
                                                                  Your browser does not support the video tag.
                                                                </video>
                                                            </div>
                                                        </div>
                                                    Next
                                                End If

                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-sm-4 col-md-4">
            <h4 class="user_title inline">Mi experiencia profesional </h4>
            <div class="row">
                <div class="col-lg-12">
                    <div id="exp-prof" class="expprof-box">
                        <span>a&b Integral Solutions</span> 
                        <ul>
                            <li>Sistemas</li>
                            <li>2016 a la Fecha</li>
                        </ul>
                    </div>
                </div>
            </div>
            <h4 class="user_title inline">Información académica </h4>
            <div class="row">
                <div class="col-lg-12">
                    <div id="info-aca" class="cursos-box">
                    </div>
                </div>
            </div>
        </div>
    </div>
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

    <script src="https://gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>
    <Script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></Script>
    <Script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></Script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
    <Script src="js/jquery.jeditable.min.js"></Script>
    <script src="js/slick.min.js" type="text/javascript" charset="utf-8"></script>
    
    <script>

        $.ajax({
            type: 'POST',
            url: 'wsperfil.asmx/Carga_ExpProf',
            data: "{idemp: " + "@idemp" + "}",
            contentType: 'application/json; utf-8',
            dataType: 'json',
            success: carga_exp,
            failure: function (response) {
                alert("1 - " + response.responseText);
            },
            error: function (response) {
                alert("2 - " + response.responseText);
            }
        });

        $.ajax({
            type: 'POST',
            url: 'wsperfil.asmx/Carga_Cursos',
            data: "{idemp: " + "@idemp" + "}",
            contentType: 'application/json; utf-8',
            dataType: 'json',
            success: carga_cur,
            failure: function (response) {
                alert("1 - " + response.responseText);
            },
            error: function (response) {
                alert("2 - " + response.responseText);
            }
        });



        var selcat = document.getElementById("selcat");
        var selsubcat = "@Html.Raw(subcatsel)";
        if (selsubcat != "") {
            selcat.innerHTML = "<div style='padding-top:15px; padding-bottom:15px;'>" + selsubcat + "<div>"; 
        }

        function carga_exp(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("expprof");      
            var idata = 0;
            $("#exp-prof").html("");
            var html = "";
            $(detalle).each(function () {
                html += "<span>" + $(this).find("proyecto").text() + "</span>";
                html += "<ul>"; 
                html += "<li>" + $(this).find("funcion").text() + "</li>";
                html += "<li>" + $(this).find("periodo").text() + "</li>";
                if ($(this).find("descripcion").text() != "") {
                    html += "<li>" + $(this).find("descripcion").text() + "</li>"
                }
                html += "</ul>";
            });
            $("#exp-prof").html(html);
        }

        function carga_cur(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("cursos");      
            var idata = 0;
            $("#info-aca").html("");
            var html = "";
            $(detalle).each(function () {
                html += "<span>" + $(this).find("curso").text() + "</span>";
                html += "<ul>";
                html += "<li>" + $(this).find("escuela").text() + "</li>";
                html += "<li>" + $(this).find("periodo").text() + "</li>";
                html += "</ul>";
            });
            $("#info-aca").html(html);
        }
    </script>
End Section
