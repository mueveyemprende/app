@functions
    Protected Sub Envia_NotiXMail(ByVal idreceptor As String,
      ByVal asunto As String, ByVal strConn As String)

        Dim htmlmsg As String

        Dim cEmp As New poolin_class.cEmprendedor
        Dim dt As System.Data.DataTable = cEmp.Datos_Emprendedor(idreceptor, strConn)
        Dim emailemp As String = ""
        Dim idioma As String = ""
        Dim tipo As String = ""
        If dt.Rows.Count <> 0 Then
            emailemp = dt.Rows(0)("email")
            idioma = dt.Rows(0)("lang")
            tipo = dt.Rows(0)("tipo")
        End If

        Dim fnoti As New StreamReader(Server.MapPath("poolin_arch/txt/notifica.txt"))
        htmlmsg = fnoti.ReadToEnd
        fnoti.Close()
        htmlmsg = htmlmsg.Replace("[URL]", "https://mueveyemprende.io/")

        Dim objSend As New poolin_class.cSendGrid

        objSend.Correo_SMTP(asunto, htmlmsg, emailemp, "", strConn)

    End Sub

End Functions

@Code
    PageData("pagina") = "user-pool-emp"
    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim msgsave = ""

    Validation.RequireField("mensaje", "<br> Requerido")

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    'If Request.Form("msg") = "OK" Then
    '    Dim msg As New poolin_class.cMensajes
    '    Dim asunto = Mid(Request.Form("mensaje"), 1, 15) & "..."
    '    Try
    '        msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), 0, asunto, Request.Form("mensaje"), strConn, 0)
    '        Envia_NotiXMail(Request.Form("idreceptor"), asunto, strConn)
    '        msgsave = "MS"
    '    Catch ex As Exception
    '        msgsave = "ERR"
    '    End Try
    'End If

    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Contrata servicios"
    PageData("oppool2") = "active"


    Dim objCat As New poolin_class.cCategorias
    Dim dtCat As System.Data.DataTable = objCat.Carga_Categorias(strConn)
    Dim dtSubCat As System.Data.DataTable

    Dim ttiempo As Int16 = -1
    Dim tiempo As String = ""

    Dim objAdmin As New poolin_class.cAdmin

    Dim subcat() As String = Request.Form.GetValues("tsubcat[]")
    Dim dtEmp As System.Data.DataTable = objAdmin.Pool_Emprendedores(subcat,
                                                                  Request.Form("presupuesto"),
                                                                  Request.Form("pais"),
                                                                  Request.Form("puntuacion"), strConn)

    Dim objEmp As New poolin_class.cEmprendedor
    Dim dtPaises As System.Data.DataTable = objEmp.Paises_Emprendedores(strConn)

    Dim objperfil As New poolin_class.cEmprendedor
    Dim dtPerfil As System.Data.DataTable = objperfil.Datos_Perfil(idemp, strConn)
    PageData("nom-val") = dtPerfil.Rows(0)("emp").ToString.Trim


    Dim idscat As String = ""
    Dim subcatsel As String = ""
    If Not IsNothing(subcat) Then
        For Each t As String In subcat
            idscat &= "," & t
        Next
        Dim dt1 As System.Data.DataTable = objCat.Carga_SubCategorias("", strConn, Mid(idscat, 2))
        For Each dr As System.Data.DataRow In dt1.Rows
            subcatsel &= "<a id='subcat-" & dr("id") & "' class='btn-categorias' value='" & dr("id") & "' onclick='delsel(this.id);'> x " & dr("subcategoria") & "</a> <input type='hidden' id='tsub-" & dr("id") & "' name='tsubcat[]' value='" & dr("id") & "'>"
        Next
    End If

    Dim idioma As String = Request.Form("idioma")
    Dim objComun As New poolin_class.cComunes
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-pool-emp", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

End Code

@section head

    <link href="~/assets/ekko/ekko-lightbox.css" rel="stylesheet">
    @*<link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?19.2">*@
    <link rel="stylesheet" href="assets/videojs/css/jquery.popVideo.css">
    
    <style>
        .optionGroup {
            font-weight: bold;
            font-style: italic;
        }

        .optionChild {
            padding-left:20px;
        }
    </style>
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

<div class="modal fade" id="modaladdmsg"  tabindex="-1" role="dialog" aria-labelledby="modaladdmsg"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modaladdmsg-1")</h3>
            </div>
            <div class="modal-body">
                <form id="formmsg" method="post" action="user-pool-emp">
                    <input type="hidden" name="msg" value="OK"/>
                    <input type="hidden" name="idreceptor" id="idreceptor" value="0" />
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                @*<label>@PageData("modaladdmsg-2")</label>*@
                                <span class="text-azul-navy" id="msg-emp"></span> <em class="text-danger"> * </em>
                                <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                <textarea type="text" Class="form-control" id="mensaje" name="mensaje" @Validation.For("mensaje") rows="4"></textarea>
                            </div>
                        </div>
                    </div>                
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                <button id="btnsendproxy" type="button" Class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                <button type="button" id="btncloseproxy" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>
                    </div>
                </form>

            </div>
        </div>
    </div>
</div>

<div id="content">
    <input type="hidden" id="iduser" value="@idemp" />

    <form action="~/publicar-proyectos" id="frmgoproy">
    </form>

    <form id="frmbuscar" method="post" action="~/user-pool-emp" class="pool-form">
        <div class="row">
            <div class="col-lg-6">
                <h3 class="user_title">@PageData("titulo")<br />
                    <button style="background-color: #b98a00; color: white;" type="button" id="btngoproy" class="btn mostaza-color" onclick="goproyecto()">Publica tu Proyecto</button>               
                </h3>
                <div class="pool-form">
                    <span id="selcat">
                        @Html.Raw(subcatsel)
                    </span>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="form-group pull-right">
                    <h3 class="inline"><i class="fas fa-sliders-h"></i> @PageData("filtro-titulo1")</h3>
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <select id="categoria" name="categoria" class="form-control" onchange="goselcat(this.value)">
                                <option value="" disabled selected>@PageData("filtro-op1")</option>
                                <option value="-1">TODAS LAS CATEGORíAS</option>
                                @for Each drCat As System.Data.DataRow In dtCat.Rows
                                    dtSubCat = objCat.Carga_SubCategorias(drCat("id"), strConn)
                                    @<option value="cat-@drCat("id")" class="optionGroup">@drCat("categoria")</option>
                                    @for Each drSubCat As System.Data.DataRow In dtSubCat.Rows
                                        @<option value="@drSubCat("id")" class="optionChild">&nbsp;&nbsp;&nbsp;@drSubCat("subcategoria")</option>
                                    Next
                                
                                Next
                            </select>
                        </div>
                    </div>
                    <div class="row hide">
                        <div class="col-lg-12 col-md-12 ">
                            <select id="presupuesto" name="presupuesto" class="form-control">
                                <option value="" selected>@PageData("filtro-op2")</option>
                                <option value="0 AND 200">$1 - $200</option>
                                <option value="201 AND 500">$201 - $500</option>
                                <option value="501 AND 1000">$501 - $1,000</option>
                                <option value="1001 AND 9999999">> $1,001</option>
                            </select>

                            <script>
                                document.getElementById("presupuesto").value = "@Request.Form("presupuesto")";
                            </script>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <select id="pais" name="pais" class="form-control">
                                <option value="" selected>@PageData("filtro-op3")</option>
                                @for Each dr As System.Data.DataRow In dtPaises.Rows
                                    @<option value="@dr("id")">@Html.Raw(dr("pais"))</option>
                                Next

                            </select>
                            <script>
                                document.getElementById("pais").value = "@Request.Form("pais")";
                            </script>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12">
                            <select id="puntuacion" name="puntuacion" class="form-control">
                                <option value="" selected>@PageData("filtro-op4")</option>
                                <option value="1">1</option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                            </select>
                        </div>
                        <script>
                            document.getElementById("puntuacion").value = "@Request.Form("puntuacion")";
                        </script>
                    </div>
                    <div class="row">
                        <div class="col-lg-6 pull-right">
                            <button type="submit" class="btn btn-primary pull-right">@PageData("btnaplicar")</button>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </form>
    <div Class="row">
        @For Each drow As System.Data.DataRow In dtEmp.Rows
            Dim propMoneda As String = ""
            Dim propMonto As Double = 0
            Dim propObserv As String = ""
            Dim propDias As Integer = 0
            Dim foto As String = ""
            Dim diPorta As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & drow("idemp") & "/images/"))
            Dim diVideo As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & drow("idemp") & "/video/"))
            Dim dtCatego As System.Data.DataTable
            Dim dtHab As System.Data.DataTable

            foto = "_avatar_" & drow("idemp") & ".png"
            Dim fiFoto As New FileInfo(Server.MapPath("poolin_arch/images/png/" & foto))
            If Not fiFoto.Exists Then
                foto = "_avatar_.png"
            End If
            dtCatego = objEmp.SubCategos_Emp(drow("idemp"), strConn)
            dtHab = objEmp.Datos_Habilidades(drow("idemp"), strConn)
            If IsDBNull(drow("calif")) Then drow("calif") = 0
            If drow("emprendedor").ToString.Trim = "" Then drow("emprendedor") = drow("email")
            'Dim di As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/proyectos/" & drow("id") & "/docs/"))
            'Dim tmpfiles As String = ""
            'Try
            '    For Each fi In di.GetFiles()
            '        tmpfiles &= "<a href='poolin_arch/proyectos/" & drow("id") & "/docs/" & fi.Name & "' target='_blank'>" & fi.Name.ToLower & "</a> | "
            '    Next
            'Catch ex As Exception

            'End Try

            @<div class="col-md-3 col-lg-3 col-xl-3">
                <div class="panel panel-default">
                    <div class="panel-body">

                        <div class="row">
                            <div class="col-lg-12 col-md-12" style="height:260px">
                                <div class="pool-box-emp" id="emp-@drow("idemp")" >
                                    <div class="">
                                        <img src="~/poolin_arch/images/png/@foto?1.1" class="misdatos-foto-modal">
                                        <div class="title-pool-emp text-capitalize" id="pubs-@drow("idemp")">@Html.Raw(drow("emprendedor")) </div>
                                        @if "" & drow("slogan") <>"" Then
                                            @<small class="subtitle-pool-emp">@Html.Raw(drow("slogan"))</small>
                                        End if
                                        @If drow("calif") = 0 Then
                                            @<div class="calification">
                                                <i class="fas fa-star"></i>
                                                <i class="fas fa-star"></i>
                                                <i class="fas fa-star"></i>
                                                <i class="fas fa-star"></i>
                                                <i class="fas fa-star"></i>
                                            </div>
                                        Else
                                            @<div class="calification">
                                                @For i As Int16 = 1 To 5
                                                    If i <= drow("calif") Then
                                            @<i class="fas fa-star"></i>
                                                    End If
                                                Next
                                            </div>
                                        End If
                                        <span class="subtitle-pool-emp">
                                        @PageData("pool-tit_1")<br />
                                        @Format(drow("fecha_creacion"), "dd/MM/yyyy")
                                        &nbsp;&nbsp;<img class="flag @drow("icno")" src="img/blank.gif" alt="@Html.Raw(drow("pais"))"> 
                                        @if drow("website").ToString <> "" Then
                                            @<a href="@drow("website")" target="_blank" title="Sitio Web">&nbsp;&nbsp;<i class="fa fa-globe"></i></a>
                                        End If
                                        </span>
                                    </div>
                                    @If drow("descripcion").ToString <> "" Then
                                        @<p class="description">@Html.Raw(Mid(drow("descripcion").ToString, 1, 100).Replace(vbCrLf, "<br />"))...</p>
                                    End If
                                </div>
                            </div>
                        </div>
                            <div Class="row">
                                <div class="col-lg-12 col-md-12">
                                    <a href="javascript:;" class="btn btn-primary btn-block" onclick="goshowperfil(@drow("idemp"))">Perfil</a>
                                    @If PageData("nom-val") <> "" Then
                                        @<a href = "#" Class="btn btn-primary btn-block" data-id="@drow("idemp")" onclick="setemp('@Html.Raw(drow("emprendedor"))', '@drow("idemp")')" data-toggle="modal"  data-target="#modaladdmsg">@PageData("pool-tit_3")</a>
                                    Else
                                        @<a href="javascript:;" class="btn btn-primary btn-block" data-target="#modal-incompleto" data-toggle="modal" >@Html.Raw(PageData("pool-tit_3"))</a>
                                    End If
                                </div>
                            </div>

                    </div>
                </div>
            </div>
        Next
        </div>
</div>

@section scripts

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

    <script src="//cdnjs.cloudflare.com/ajax/libs/tether/1.2.0/js/tether.min.js" integrity="sha384-Plbmg8JY28KFelvJVai01l8WyZzrYWG825m+cZ0eDDS1f7d/js6ikvy1+X+guPIB" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>
    <script src="assets/ekko/ekko-lightbox.js"></script>
    <script src="assets/videojs/js/jquery.popVideo.js"></script>

    <script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>

    <script>


            //$('#modaladdmsg').on('show.bs.modal', function (e) {
            //    var idreceptor = e.relatedTarget.dataset.id;
            //    document.getElementById("idreceptor").value = idreceptor;
            //});


            // delegate calls to data-toggle="lightbox"
            //$(document).on('click', '[data-toggle="lightbox"]:not([data-gallery="navigateTo"])', function (event) {
            //    event.preventDefault();
            //    return $(this).ekkoLightbox({
            //        onShown: function () {
            //            if (window.console) {
            //                return console.log('Checking our the events huh?');
            //            }
            //        },
            //        onNavigate: function (direction, itemIndex) {
            //            if (window.console) {
            //                return console.log('Navigating ' + direction + '. Current item: ' + itemIndex);
            //            }
            //        }
            //    });
            //});


            //Programmatically call
            $('#open-image').click(function (e) {
                e.preventDefault();
                $(this).ekkoLightbox();
            });
            $('#open-youtube').click(function (e) {
                e.preventDefault();
                $(this).ekkoLightbox();
            });
    
            // navigateTo
            $(document).on('click', '[data-toggle="lightbox"][data-gallery="navigateTo"]', function (event) {
                event.preventDefault();

                return $(this).ekkoLightbox({
                    onShown: function () {

                        this.modal().on('click', '.modal-footer a', function (e) {

                                    e.preventDefault();
                            this.navigateTo(2);

                        }.bind(this));

                    }
                });
            });

            /**
             * Documentation specific - ignore this
             */
            //anchors.options.placement = 'left';
            //anchors.add('h3');
            $('code[data-code]').each(function () {

                var $code = $(this),
                    $pair = $('div[data-code="' + $code.data('code') + '"]');

                $code.hide();
                var text = $code.text($pair.html()).html().trim().split("\n");
                var indentLength = text[text.length - 1].match(/^\s+/)
                indentLength = indentLength ? indentLength[0].length : 24;
                var indent = '';
                for (var i = 0; i < indentLength; i++)
                    indent += ' ';
                if ($code.data('trim') == 'all') {
                    for (var i = 0; i < text.length; i++)
                        text[i] = text[i].trim();
                } else {
                    for (var i = 0; i < text.length; i++)
                        text[i] = text[i].replace(indent, '    ').replace('    ', '');
                }
                text = text.join("\n");
                $code.html(text).show();

            });
                //$(".announce").click(function () { // Click to only happen on announce links
                //    $("#idproducto").val($(this).data('id'));
                //    $("#idreceptor").val($(this).data('idreceptor'));
                //    $('#MyModal').modal('show');
                //});

        var selcat = document.getElementById("selcat");
        var selsubcat = "@Html.Raw(subcatsel)";
        if (selsubcat != "") {
            selcat.innerHTML = selsubcat;
        }

        function goshowperfil(idemp) {
            window.open("show-perfil?idemp=" + idemp,'popup','width=980,height=600');
        }

        function setemp(nombre, idreceptor) {
            $("#msg-emp").html("<b>" + nombre + "</b>");
            $("#idreceptor").val(idreceptor);
            //$("#modaladdmsg").modal("show");
        }

        function showvideo(valor) {
            $('#video-' + valor).popVideo({
                playOnOpen: true,
                // close on end
                closeOnEnd: false,
                // pause on close
                pauseOnClose: true,
                // (String || Boolean) 
                closeKey: 'esc',
  // callback functions
                callback: {
                    onOpen: function (self) { },
                    onClose: function (self) { },
                    onPlay: function (self) { },
                    onPause: function (self) { },
                    onEnd: function (self) { }
                }
            }).open()

        }

        function goselcat(valor, texto) {
            var selcat = document.getElementById("selcat");
            var cate = document.getElementById("categoria");
            var arr = document.getElementsByName("tsubcat[]");
            for (i = 0; i < arr.length; i++) {
                if (valor == arr.item(i).value) {
                    return;
                }
            }
            var n = valor.search("cat");
            if (n != 0) {
                selcat.innerHTML = selcat.innerHTML + "<a id='subcat-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delsel(this.id);'> x " + cate.options[cate.selectedIndex].text + "</a> <input type='hidden' id='tsub-" + valor + "' name='tsubcat[]' value='" + valor + "'>";
            }
            else {
                selcat.innerHTML = selcat.innerHTML + "<a id='subcat-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delsel(this.id);'> x <i><b>" + cate.options[cate.selectedIndex].text + "</b></i></a> <input type='hidden' id='tsub-" + valor + "' name='tsubcat[]' value='" + valor + "'>";
            }
        }

        function delsel(valor) {
            document.getElementById(valor).remove();
            document.getElementById("tsub-" + valor.replace("subcat-", "")).remove();
        }

        var msgsend = "@msgsave";
        switch (msgsend)
        {
            case "MS" : 
                $.bootstrapGrowl('@PageData("msg-ms")', {
                    type: 'success',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "ERR" : 
                $.bootstrapGrowl('@PageData("msg-err")', {
                    type: 'danger',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
        }

        function goproyecto() {
            $("#frmgoproy").submit();
        }

    </script>

    <script type="text/javascript">

        $(function () {
            var username = $("#iduser").val();
            console.log("POOL-EMP Inicio del servicio");
            var proxy = $.connection.messageHub;
            proxy.client.recibenoti = function (message, user, userenvio) {
                console.log("POOL-EMP " + user);
                console.log("POOL-EMP ENVIO + : " + userenvio);
                if (document.getElementById("iduser").value == user) {
                    console.log("POOL-EMP  "+  message);
                    //make_msg(user);
                }
            };

            proxy.client.updateUsers = function (userCount, userList) {
                //$('#onlineUsersCount').text('Online users: ' + userCount);
                //$('#userList').empty();
                userList.forEach(function (username) {
                    //$('#userList').append('<li>' + username + '</li>');
                    //$('#idcol-' + username).removeClass('text-success');
                    //$('#idcol-' + username).toggleClass('text-success');
                });
            };

            $.connection.hub.start().done(function () {
                console.log("POOL-EMP  username: " + username);
                proxy.server.connect(username);
                console.log("POOL-EMP  connect: " + username);
            });

            //myHub = $.connection.messagehub;
            try {
                //$.connection.hub.logging = true;
                console.log("POOL-EMP  Conecta ON");
                $.connection.hub.start();
                console.log("POOL-EMP  Conecta OK");
            }
            catch (e)
            {
                console.log("POOL-EMP  " + e.message);
            }

            $("#btnsendproxy").click(function () {
                if ($('#mensaje').val() == "") {
                    $('#mensaje').focus();
                    return;
                }
                console.log("MSG: " + $('#mensaje').val());
                console.log("UsuarioID: " + $('#idreceptor').val());
                try {
                    console.log("POOLEMP " + $('#mensaje').val() + " " + $('#idreceptor').val() + " " + $('#iduser').val());
                    proxy.server.senmessage($('#mensaje').val(), $('#idreceptor').val(), $('#iduser').val());
                    $.ajax({
                        type: 'POST',
                        url: 'wsmensajes.asmx/grabamsg',
                        data: "{user: '" + $('#iduser').val() + "', userenvia: '"+ $('#idreceptor').val() + "', asunto:'', mensaje: '" + $('#mensaje').val() + "', idmsgpadre: '0', idproyecto:0}",
                        contentType: 'application/json; utf-8',
                            dataType: 'json',
                            success: function (data) {
                            $('#formmsg').attr('action', 'user-pool-emp#emp-' + $("#idreceptor").val());
                            alert("MENSAJE ENVIADO CON EXITO.");
                            $("#mensaje").val("");
                            $("#msg-emp").html("");
                            $('#formmsg').submit();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert("Error: " + textStatus);
                            return; 
                        }
                    });
                    console.log("ENVIAdo..." + $("#iduser").val());
                }
                catch (e)
                {
                    alert("Mensaje NO enviado. " + e.message);
                }
            });


            //$("#btnleido").click(function () {
            //    $.ajax({
            //        type: 'POST',
            //        url: 'wsmensajes.asmx/leidomsg',
            //        data: "{usersent: '"+ $('#iduser').val() + "', idmsgpadre: '"+ $('#idmsgpadre').val()  + "'}",
            //        contentType: 'application/json; utf-8',
            //        dataType: 'json',
            //        success: function (data) {
            //            $("#txtmsg").val('');
            //            $("#sendmessage").modal('toggle'); 
            //            carga_msg(data);
            //        },
            //        error: function (jqXHR, textStatus, errorThrown) {
            //        }
            //    });

            //});



        });
        //make_msg($("#iduser").val());
    </script>
End Section