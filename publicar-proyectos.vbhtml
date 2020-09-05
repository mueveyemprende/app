@functions
    Private Sub Grabar_Proyecto(ByVal idemp As Long, ByRef idproyecto As Long, ByVal strConn As String)

        'Try
        Dim p As New poolin_class.cProyectos

        p.idemprendedor = idemp
        p.tipo = "P" 'Request.Form("tipoproyecto")
        p.nombre = "" & Request.Form("proyecto")
        p.nombre = p.nombre.Replace("<", "(").Replace(">", ")").Replace("{", "(").Replace("}", ")").Replace("/", " ")
        p.descripcion = "" & Request.Form("proy-desc")
        p.descripcion = p.descripcion.Replace("<", "(").Replace(">", ")").Replace("{", "(").Replace("}", ")").Replace("/", " ")
        p.duracion = "0"
        p.tipopago = "P"
        p.pago = Request.Form("monto")
        p.moneda = Request.Form("moneda")
        p.estatus = "" & Request.Form("estatus")

        Dim subcat() As String = Request.Form.GetValues("tsubcat[]")
        Dim idscat As String = ""
        If Not IsNothing(subcat) Then
            For Each t As String In subcat
                idscat &= "," & t
            Next
        End If

        p.idsubcatego = idscat

        'If Request.Form("subcategootra") = "@" Then
        p.subcategootros = ""
        'Else
        '    p.subcategootros = Request.Form("subcategootra")
        'End If

        p.poolinseguro = True
        p.poolinsegtyc = True

        p.Grabar_ProyVac(idproyecto, Server.MapPath("~/poolin_arch/proyectos/tmp/" & idemp), Server.MapPath("~/poolin_arch/proyectos"), strConn)

        'If Not Directory.Exists(Server.MapPath("poolin_arch/proyectos/" & idproyecto & "/docs")) Then
        '    Directory.CreateDirectory(Server.MapPath("poolin_arch/proyectos/" & idproyecto & "/docs"))
        'End If

        'For i As Integer = 0 To Request.Files.Count - 1
        '    Dim archivo As HttpPostedFileBase = Request.Files(i)
        '    If archivo.FileName <> "" Then
        '        archivo.SaveAs(Server.MapPath("poolin_arch/proyectos/" & idproyecto & "/docs/" & archivo.FileName))
        '    End If
        'Next

        'Dim filesdel = Request.Form("deletefiles")
        'Dim fdel() = filesdel.Split("/")

        'For Each fborra As String In fdel
        '    If fborra <> "" Then
        '        File.Delete(Server.MapPath("poolin_arch/proyectos/" & idproyecto & "/docs/" & fborra))
        '    End If
        'Next

        'Muevo las fotos al folder del proyecto
        'Crea_Folder(idproyecto)
        'Mueve_Fotos(idproyecto)
        'Carga_Fotos(idproyecto)
        'Graba_Fotos_DB(idproyecto)
        'Dim cmm As New cComunes
        'Response.Redirect("misproyectos.aspx?success=" & cmm.Encrypt(idproyecto), False)
        'Catch ex As Exception
        'divAlerta.Style.Add("display", "normal")
        'divAlerta.InnerHtml = m.Msg_Alertas(cComunes._alerta._error, ViewState("errapp"), ex.Message, False)
        'End Try
    End Sub

End Functions

@Code
    PageData("pagina") = "publicar-proyectos"
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Publica tus proyectos"
    PageData("oppub") = "active"



    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Dim objComun As New poolin_class.cComunes

    Dim objperfil As New poolin_class.cEmprendedor
    Dim dtPerfil As System.Data.DataTable = objperfil.Datos_Perfil(idemp, strConn)
    PageData("nom-val") = dtPerfil.Rows(0)("emp").ToString.Trim


    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "publicar-proyectos", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
    Validation.RequireField("proyecto", "<br>" & PageData("val-msg"))
    Validation.RequireField("proy-desc", "<br>" & PageData("val-msg"))
    Validation.RequireField("moneda", PageData("val-msg"))
    Validation.RequireField("monto", PageData("val-msg"))
    Validation.RequireField("categoria", PageData("val-msg"))


    Dim objCat As New poolin_class.cCategorias
    Dim dtCat As System.Data.DataTable = objCat.Carga_Categorias(strConn)
    Dim dtSubCat As System.Data.DataTable
    Dim idproyecto As Long = 0
    If Request.QueryString("idproyecto") <> "" Then
        idproyecto = objComun.Decrypt(Request.QueryString("idproyecto"))
    End If
    Dim msgsave As String = ""

    If Request.Form("save") = "OK" Then
        Try
            idproyecto = Request.Form("idproyecto")
            Grabar_Proyecto(idemp, idproyecto, strConn)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If
    Dim dtProy As New System.Data.DataTable
    Dim objProy As New poolin_class.cProyectos
    If idproyecto <> 0 Then
        dtProy = objProy.Datos_Proyecto(idproyecto, strConn)
    End If

    Dim proyecto = ""
    Dim proy_desc = ""
    Dim monto As Double = 0
    Dim moneda = ""
    Dim tmpsel As String = ""
    Dim subcatsel As String = ""
    Dim estatus As String = "A"
    Dim dirFiles As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/proyectos/" & idproyecto & "/docs/"))
    If idproyecto = 0 Then
        Dim dirtmp As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/proyectos/tmp/" & idemp))
        If dirtmp.Exists Then
            For Each fi In dirtmp.GetFiles()
                File.Delete(Server.MapPath("poolin_arch/proyectos/tmp/" & idemp & "/" & fi.Name))
            Next
        End If
    End If
    Dim idproyencry = objComun.Encrypt(idproyecto)
    For Each dr As System.Data.DataRow In dtProy.Rows
        Dim dtTMP As System.Data.DataTable
        dtTMP = objProy.Carga_CategoProyecto(idproyecto, strConn)
        For Each drCat As System.Data.DataRow In dtTMP.Rows
            tmpsel = drCat("id")
            subcatsel &= "<a id='subcat-" & drCat("id") & "' class='btn-categorias' value='" & drCat("id") & "' onclick='delsel(this.id);'> x " & drCat("subcategoria") & "</a> <input type='hidden' id='tsub-" & drCat("id") & "' name='tsubcat[]' value='" & drCat("id") & "'>"
        Next
        proyecto = dr("nombre")
        proy_desc = dr("descripcion")
        monto = dr("pago")
        moneda = dr("moneda")
        estatus = dr("estatus")
    Next
End Code



@section head
    <link href="assets/plugins/form/jquery-file-upload/css/jquery.fileupload.css" rel="stylesheet" />
End Section

<div class="modal fade" id="modal-incompleto"  tabindex="-1" role="dialog" aria-labelledby="modal-incompleto"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">
                <i class="fa fa-info-circle"></i>
                Tu perfil está incompleto</h3>
            </div>
            <div class="modal-body">
                <p>Para poder proccesar esta opción debes completar los datos de tu perfil, lo podrás hacer entrando a <a href="~/user-perfil">Mi Perfil</a>.
                </p>
            </div>
            <div class="modal-footer">
                <button type="button" Class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>

<div id="content">
    <div class="profile-box">
        <h3 class="user_title">@PageData("titulo")</h3>
        <div class="row">
            <div class="col-lg-10">
                <p>@PageData("subtitulo1")</p>
            </div>
        </div>
        <form id="formproy" class="pool-form" enctype="multipart/form-data" method="post" action="">
            <input name="save" type="hidden" value="OK" />
            <input id="idproyecto" name="idproyecto" type="hidden" value="@idproyecto" />
            <div class="row">
                <div class="col col-lg-10">
                    <div class="fields_wrap">
                        <label>@PageData("label1") <em class="text-danger"> *</em></label>
                        <span class="msg-val">@Html.ValidationMessage("proyecto")</span>
                        <input name="proyecto" id="proyecto" class="form-control" value="@Html.Raw(proyecto)" placeholder="@PageData("pholder1")" @Validation.For("proyecto") />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col col-lg-10">
                    <div class="fields_wrap">
                        <label>@PageData("label2")<em class="text-danger"> *</em></label>
                        <span class="msg-val">@Html.ValidationMessage("proy-desc")</span>
                        <textarea name="proy-desc" id="proy-desc" class="form-control" placeholder="@PageData("pholder2")" rows="5" @Validation.For("proy-desc") ">@Html.Raw(proy_desc)</textarea>
                    </div>
                </div>
            </div>
            <div class="fields_wrap">
                <div class="row">
                    <div class="col-lg-10">
                        <Label> @PageData("label4") <em class="text-danger"> *</em></Label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-4">
                        <span class="msg-val">@Html.ValidationMessage("categoria")</span>
                        <select id="categoria" name="categoria" class="form-control" onchange="goselcat(this.value)" @Validation.For("categoria")>
                            <option value="" disabled selected>Categoría</option>
                            @For Each drCat As System.Data.DataRow In dtCat.Rows
                                dtSubCat = objCat.Carga_SubCategorias(drCat("id"), strConn)
                                @<optgroup label="@drCat("categoria")">
                                    @For Each drSubCat As System.Data.DataRow In dtSubCat.Rows
                                        @<option value="@drSubCat("id")">@drSubCat("subcategoria")</option>
                                    Next
                                </optgroup>
                            Next
                        </select>
                        <script>
                            var seltmp = "@tmpsel";
                            document.getElementById("categoria").value = seltmp;
                        </script>
                    </div>
                    <div class="col col-lg-6">
                        <span id="selcat">
                            @Html.Raw(subcatsel)
                        </span>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-8">
                    <div class="fields_wrap">
                        <Label>@PageData("label5")<em class="text-danger"> *</em></Label>
                        <div class="form-inline">
                            <span class="msg-val">@Html.ValidationMessage("moneda")</span>
                            <select name="moneda" id="moneda" class="form-control" @Validation.For("moneda")>
                                <option value="" selected disabled>MONEDA</option>
                                <option value="MXN">MXN</option>
                                <option value="USD">USD</option>
                            </select>
                            <script>
                                var moneda = "@moneda";
                                document.getElementById("moneda").value = moneda;
                            </script>
                            <span class="msg-val">@Html.ValidationMessage("monto")<em class="text-danger"> *</em></span>
                            <input name="monto" id="monto" type="number" step="0.01" class="form-control right" placeholder="@PageData("pholder3")" value="@Html.Raw(monto)" @Validation.For("monto")>
                        </div>
                    </div>
                </div>
            </div>
            @If idproyecto <> 0 Then
                @<div class="row">
                    <div class="col-lg-3">
                        <div class="fields_wrap">
                            <label>@PageData("label6")</label>
                            <select name="estatus" id="estatus" class="form-control">
                                <option selected value="A">@PageData("opestatus1")</option>
                                <option value="E">@PageData("opestatus2")</option>
                            </select>
                            <script>
                                var estatus = "@estatus";
                                document.getElementById("estatus").value = estatus;
                            </script>
                        </div>
                    </div>
                </div>
            End If
            <div class="fields_wrap hide" id="frmimg">
                <div class="row">
                    <div class="col-lg-10">
                        <Label> @PageData("label3")</Label>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-5">
                        @*<input type="file" class="form-control" name="fupload" multiple />*@
						<span class="btn btn-primary fileinput-button btn-sm m-r-3 m-b-3">
							<i class="glyphicon glyphicon-plus"></i>
							<span>Selecciona archivo(s)</span>
							<input id="uparchivos" type="file" name="fupload" multiple>
						</span>
                        <div class="progress" >
                            <div id="prog-upporta" class="progress-bar" role="progressbar" style="width: 0%;"  aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div>
                        </div>
                        <div id="divarch"></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <input name="deletefiles" id="deletefiles" type="hidden" />
                        @If dirFiles.Exists Then
                            Dim ifile As Int16 = 0
                            @<span id="archivos">
                                <ul>
                                @For Each fi In dirFiles.GetFiles()
                                    ifile += 1
                                    @<li class="list-unstyled" id="spanfile-@ifile">
                                        <a href="poolin_arch/proyectos/@idproyecto/docs/@fi.Name" target="_blank"><i class="fas fa-paperclip"></i> <small id="nfile-@ifile">@Html.Raw(fi.Name.ToUpper)</small></a> 
                                        <a href="javascript:;" onclick="delarchivo('spanfile-@ifile', 'poolin_arch/proyectos/@idproyecto/docs/@fi.Name')"> <small><i id="btn-fa-@ifile" class="fa fa-trash text-danger"></i></small></a>
                                        @*delarchivo(\'spanfile-' + i + '\', \'' + item + '\')*@
                                    </li>
                                Next
                                </ul>
                            </span>
                        End If

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-10">
                    @if PageData("nom-val") <> "" Then
                    @<button type="submit" class="btn btn-primary pull-right">
                        @if idproyecto = 0 Then
                            @Html.Raw(PageData("btnenviar1"))
                        Else
                            @Html.Raw(PageData("btnenviar2"))
                        End If
                    </button>
                        @if idproyecto<>0 Then
                            @<a href="publicar-proyectos" class="btn btn-default  pull-right">@PageData("btnlimpiar")</a>
                        End If
                    Else
                        @<a href="javascript:;" class="btn btn-primary pull-right" data-target="#modal-incompleto" data-toggle="modal" >@Html.Raw(PageData("btnenviar1"))</a>
                    End if
                </div>  
            </div>
        </form>
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

    <script src="Scripts/jquery.validate.min.js"></script>
    <script src="Scripts/jquery.validate.unobtrusive.min.js"></script>

    <Script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></Script>
    <Script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></Script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script>
        $(document).ready(function () {

            $("#uparchivos").change(function () {
                var fileUpload = $("#uparchivos").get(0);
                var files = fileUpload.files;

                // Create FormData object  
                var fileData = new FormData();

                // Looping over all files and add it to FormData object  
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }

                // Adding one more key to FormData object  
                fileData.append('idproyecto', $("#idproyecto").val());
                fileData.append('idemisor', '@idemp');
                fileData.append('origen', 'PROY');

                $("#prog-upporta").attr("style", "width: 0%");
                $("#prog-upporta").attr("aria-valuenow", "0");
                $(".porcentajeporta").html("0%");
                $.ajax(
                    {
                        xhr: function () {
                            var xhr = new window.XMLHttpRequest();
                            xhr.upload.addEventListener("progress", function (evt) {
                                if (evt.lengthComputable) {
                                    var percentComplete = evt.loaded / evt.total;
                                    percentComplete = parseInt(percentComplete * 100);
                                    //style="width: 50%;"  aria-valuenow="50"
                                    $("#prog-upporta").attr("style", "width:" + percentComplete + '%');
                                    $("#prog-upporta").attr("aria-valuenow", percentComplete);
                                    $(".porcentajeporta").html(percentComplete + "%");
                                    if (percentComplete === 100) {
                                    }
                                }
                            }, false);
                            return xhr;
                        },
                        url: "wsmensajes.asmx/UploadFile",
                        type: "POST",
                        contentType: false, // Not to set any content header  
                        processData: false, // Not to process data  
                        data: fileData,
                        dataType: 'text',
                        success: function (response) {
                            var xml = jQuery.parseXML(response);
                            $(xml).find("string").each(function () {
                                var archivos = $(this).text().split(",");
                                var divarch = "";
                                $.each(archivos, function (i, item) {
                                    $("#divarch").append('<span class="upfilesdel" id="spanfile-' + i + '"><a href="' + item + '" target="_blank" ><i class="fas fa-paperclip"></i>  <small>' + item + '<small></a><a href="javascript:;" onclick="delarchivo(\'spanfile-' + i + '\', \'' + item + '\')">   <i class="fas fa-trash"></i></a><br><span>');
                                });
                            });
                        }, //End of AJAX Success function  
                        failure: function (data) {
                            alert(data);

                        }, //End of AJAX failure function  
                        error: function (data) {
                            alert(data);
                        }
                    }
                );
            });
        });
        if (Number($("#idproyecto")) != 0) {
            if ($("#frmimg").hasClass("hide")) {
                $("#frmimg").toggleClass("hide");
            }
        }

        function delarchivo(obj, ruta) {
            $.ajax({
                type: "POST",
                url: "wsmensajes.asmx/Del_Archivo",
                datatype: 'json',
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: {
                    archivo: ruta
                },
                success: function (data) {
                    $("#" + obj).remove();
                },
                error: function (response) {
                    alert(response.d);
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        }



        function formsubmit() {
            $("#formproy").submit();
        }
    function goselcat(valor, texto) {
        var selcat = document.getElementById("selcat");
        var cate = document.getElementById("categoria");
        var arr = document.getElementsByName("tsubcat[]");
    //alert(arr.length);
        for (i = 0; i < arr.length; i++) {
            if (valor == arr.item(i).value) {
                return;
            }
        }
    //selcat.innerHTML = selcat.innerHTML + "<select name='lstcat' multiple id='subcatego' size='1'><option value='"+ valor + "'> x " + cate.options[cate.selectedIndex].text + "</option></select> ";
        selcat.innerHTML = selcat.innerHTML + "<a id='subcat-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delsel(this.id);'> x " + cate.options[cate.selectedIndex].text + "</a> <input type='hidden' id='tsub-" + valor + "' name='tsubcat[]' value='" + valor + "'>";
    //cate.selectedIndex = 0;
    }

    function delsel(valor) {
        document.getElementById(valor).remove();
        document.getElementById("tsub-" + valor.replace("subcat-", "")).remove();
    }

        function delfile(valor) {
            var btn = document.getElementById("btn-fa-" + valor).className
            var doc = document.getElementById("nfile-" + valor).innerText
            //= "fa fa-undo";
            
            if (btn == "fa fa-trash") {
                document.getElementById("deletefiles").value += doc + "/";
                document.getElementById("nfile-" + valor).style.color = "red";
                document.getElementById("btn-fa-" + valor).className = "fa fa-undo";
            }
            else {
                var archivos = document.getElementById("deletefiles").value
                archivos = archivos.replace(doc + "/", "");
                document.getElementById("nfile-" + valor).style.color = "green";
                document.getElementById("deletefiles").value = archivos;
                document.getElementById("btn-fa-" + valor).className = "fa fa-trash";
            }
            
        }

    var selcat = document.getElementById("selcat");
    var selsubcat = "@Html.Raw(subcatsel)";
    if (selsubcat != "") {
        selcat.innerHTML = selsubcat;
    }

        var msgsend = "@msgsave";
        if (msgsend == "OK") {
            if ($("#frmimg").hasClass("hide")) {
                $("#frmimg").toggleClass("hide");
            }

            $.bootstrapGrowl('Tu información se guardó satisfactoriamente', {
                type: 'success',
                delay: 2000,
                width: '100%'
            });
        }
    else if (msgsend == "ERR") {
        $.bootstrapGrowl('Hubo un Error. Intentalo mas tarde.', {
            type: 'danger',
            delay: 2000,
            width: '100%'
        });

        }
        if (msgsend == "OK") {
            //window.location.href = "user-proyectos?idproyecto=@idproyencry";
            //window.location.href = "user-pool?idproyecto=@idproyencry";
        }
    </script>
end section