@imports System.Data
@code
    PageData("pagina") = "user-mensajes"
    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Mensajes"
    PageData("opmens") = "active"

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Dim objmsg As New poolin_class.cMensajes
    Dim dthead As System.Data.DataTable = objmsg.Mensaje_Recibidos(idemp, strConn)
    'Dim dtDetalle As System.Data.DataTable
    Dim idmsg As Long = 0
    'If Not IsNothing(Request.Form("idmsg")) Then
    '    idmsg = Request.Form("idmsg")
    'Else
    '    If dthead.Rows.Count <> 0 Then
    '        idmsg = dthead.Rows(0)("id")
    '    End If
    'End If
    Dim msgsend As String = ""
    If Not IsNothing(Request.Form("idmsgresp")) Then
        'Try
        '    objmsg.Responde_Mensaje(Request.Form("idmsgresp"),
        '                    Request.Form("tbasunto"),
        '                    Request.Form("tbresp"), strConn)
        '    idmsg = Request.Form("idmsgresp")
        '    Dim objsend As New poolin_class.cSendGrid
        '    'objsend.sendgrid(ConfigurationManager.AppSettings("sendgridkey").ToString,                             "Tienes mensaje en POOLIN", "<strong>Tienes una notificación en Poolin</strong>", Request.Form("msgemail"), "", strConn)
        '    msgsend = "OK"
        'Catch ex As Exception
        '    msgsend = "ERR"
        'End Try
    End If
    'dtDetalle = objmsg.Mensaje_Detalle(idmsg, strConn)

    Dim objComun As New poolin_class.cComunes
    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-mensajes", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
End Code
@section head
    <link href="~/assets/plugins/icon/themify-icons/themify-icons.css" rel="stylesheet" />
    <style>
        a:hover{background-color:transparent !important; }
        .bnum {
            float: right;
            background: #007aff;
            color: #fff;
            padding: 3px 5px 3px 7px;
            -webkit-border-radius: 0.375rem;
            -moz-border-radius: 0.375rem;
            border-radius: 0.375rem;
            font-size: 11px;
            line-height: 1rem;
            margin-top: 1px;
        }

    </style>
End Section
<div id="content">
    <h3 class="user_title">Centro de notificaciones y mensajes.<br /><small >Utiliza este centro para gestionar tus mensajes, avances y entregas.</small></h3>
    
    @* <button id="btnok" class="success btn btn-success">Success</button>
        <button class="error btn btn-danger">Error</button>
        <button class="info btn btn-info">Info</button>
        <button class="warning btn btn-warning">Warning</button>*@

    <div class="chat_container">
        <div class="col-xs-3 col-sm-3 chat_sidebar">
            <div class="row">
                <div id="custom-search-input" class="hidden">
                    <div class="input-group col-md-12">
                        <input type="text" class="search-query form-control" placeholder="@PageData("pholder-1")" />
                        <button class="btn btn-danger" type="button">
                            <span class="fas fa-search"></span>
                        </button>
                    </div>
                </div>
                <div class="chat-list" style="padding:10px">
                    <form id="frmmsg" action="~/user-mensajes" method="post">
                        <input type="hidden" id="idmsg" name="idmsg" value="@idmsg" />
                        <ul class="list-unstyled" id="listhead">
                            @For Each dr As DataRow In dthead.Rows
                                @<li class="msg-emp" onclick="vermensajes(@dr("idemp"), @dr("idproyecto"));" id="li-@String.Format("{0}_{1}", dr("idemp"), dr("idproyecto"))">
                                    <div Class="hidden-xs hidden-sm hidden-md">
                                        <span Class="chat-img pull-left">
                                            <img src="poolin_arch/images/png/@String.Format("_avatar_{0}.png", dr("idemp"))" onerror="this.src='img/logo-carr.jpg';this.onerror='';">
                                        </span>
                                    </div>
                                    <div Class="clearfix">
                                        <div Class="chat-list-header">
                                            @dr("Emisor")
                                            <div Class="pull-right text-gray-dark" style="font-size:9px">@String.Format("{0:yyyy-MM-dd h:mm}", dr("fecha"))</div>
                                        </div>
                                        <div Class="chat-list-text">
                                            @if IsDBNull(dr("Proyecto")) Then
                                                If dr("nolee") <> 0 Then
                                                @<span>Mensaje Directo <b class="bnum leido-@String.Format("{0}_{1}", dr("idemp"), dr("idproyecto"))">@dr("nolee")</b></span>
                                                Else
                                                    @<span>Mensaje Directo </span>
                                                End If
                                                    Else
                                                    If dr("nolee") <> 0 Then
                                                    @<span>@Html.Raw(dr("Proyecto")) <b class="bnum leido-@String.Format("{0}_{1}", dr("idemp"), dr("idproyecto"))">@dr("nolee")</b></span>
                                                Else
                                                    @<span>@Html.Raw(dr("Proyecto")) </span>
                                                End If
                                                    End If
                                        </div>
                                        @*<span Class="pull-right chat-time">
                                            <strong>' + tiempotrans($(this).find("fecha_creacion").text()) +  '</strong>
                                        </span>*@
                                    </div>
                                </li>

                            Next
                        </ul>
                    </form>

                </div>
            </div>
        </div>
        <div class="col-xs-9 col-sm-9">
            <ul class="email-list" id="div-mensajes"  style="overflow-y:scroll; height:410px">
            </ul>
            <form id="frm-upload" enctype="multipart/form-data" method="post">
			    <div class="form-group">
				    <div data-format="alias" class="input-group colorpicker-component">
					    <input type="text" id="tbmsg" name="tbmsg" placeholder="Escribe aquí tu mensaje y/o adjunta un archivo." class="form-control" />
                        <span class="input-group-addon" id="btnaddfile"><i class="fas fa-paperclip"></i></span>
					    <span class="input-group-addon" id="btnsendmsg"><i class="fas fa-send"></i></span>
				    </div>
			    </div>
                <input type="hidden" id="idproyecto" />
                <input type="hidden" id="idreceptor" />
                <input type="file" class="hidden" id="uparchivos" multiple />
                <div class="form-group">
                    <div id="divarch"></div>
                </div>
                <div class="progress" >
                    <div id="prog-upporta" class="progress-bar" role="progressbar" style="width: 0%;"  aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
            </form>
        </div>
    </div>
</div>

@section Scripts
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>

	 <!-- ================== BEGIN BASE JS ================== -->
	<script src="assets/plugins/jquery/jquery-3.2.1.min.js"></script>
	<script src="assets/plugins/jquery-ui/jquery-ui.min.js"></script>
	<script src="assets/plugins/cookie/js/js.cookie.js"></script>
	<script src="assets/plugins/tooltip/popper/popper.min.js"></script>
	<script src="assets/plugins/bootstrap/bootstrap4/js/bootstrap.min.js"></script>
	<script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>
	<!-- ================== END BASE JS ================== -->

    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script>
        $(document).ready(function () {
            $("#btnaddfile").click(function () {
                $("#uparchivos").click();    
            });
            
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
                                    $("#divarch").append('<span class="upfilesdel" id="spanfile-' + i + '"><a href="' + item + '" target="_blank" ><i class="fas fa-paperclip"></i>  <small>Archivo #' + (i + 1) + '<small></a><a href="javascript:;" onclick="delarchivo(\'spanfile-' + i + '\', \'' + item + '\')">   <i class="fas fa-trash"></i></a><br><span>');
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

        function gotoresp() {
            document.getElementById("form-send").submit;
        }

        function vermensajes(idemisor, idproy) {
            $(".msg-emp").removeClass("bg-success");
            $("#li-" + idemisor + "_" + idproy).toggleClass("bg-success");
            $("#idproyecto").val(idproy);
            $("#idreceptor").val(idemisor);
            $.ajax({
                type: 'POST',
                url: 'wsmensajes.asmx/Mensaje_Head',
                data: "{idemisor: " + idemisor + ", idreceptor: @idemp, idproyecto: " + idproy  + ", updateleido: 1}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: function (response) {
                    $('.leido-' + idemisor + '_' + idproy).remove();
                    $("#noti-i").css("color", "");
                    Carga_Detalle_Mensajes(response);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Error: " + textStatus);
                    return;
                }
            });
        }

        function Carga_Detalle_Mensajes(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("head");
            $('#div-mensajes').empty();
            var strli = "";
            var idtag = "";
            $(detalle).each(function () {
                //unread has-attachment
                var unread = "";
                if ($(this).find("estatus").text() == "N") {
                    unread = "unread"
                }
                var idmsg = $(this).find("id").text();
                if ($(this).find("idemisor").text() == $("#iduser").val() && $(this).find("asunto").text() == 'PROPUESTA ACEPTADA') {
                    unread = "hidden";
                }
                strli += ' 			<li class="' + unread + '" id="li-' + $(this).find("id").text() + '">'
                    + ' 				<div class="email-message">'
                    + ' 					<div onclick="javascript:;">'
                    + ' 						<div class="email-sender">'
                    + ' 							<span class="email-time text-right">' + tiempotrans($(this).find("fecha_creacion").text()) + ' - ' + $(this).find("fecha_creacion").text().split("T")[0] + '</span>'
                    + ' 							' + $(this).find("Emisor").text()
                    + ' 						</div>'
                    + ' 						<div class="email-desc">'
                    + ' 					    ' + $(this).find("mensaje").text();
                if ($(this).find("arch").text() != "") {
                    strli += '<br>';
                    var arch = $(this).find("arch").text().split("|");
                    $.each(arch, function (i, item) {
                        if (item != "") {
                            strli += '<a href="poolin_arch/msg/' + idmsg + '/' + item + '" target="_blank"><small><i class="fas fa-paperclip"></i> ' + item + '</small></a>  |  ';
                        }
                    });
                }
                strli += ' 						</div>'
                    + ' 					</div>'
                    + ' 				</div>'
                    + '             </li>';
                idtag = $(this).find("id").text();
            });
            
            $("#div-mensajes").html(strli);
            var elmnt = document.getElementById('li-' + idtag);
            elmnt.scrollIntoView();

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

        var msgsend = "@msgsend";
        if (msgsend == "OK") {
            $.bootstrapGrowl('@PageData("msg-ok")', {
                type: 'success',
                delay: 2000,
                width: '100%'
            });
        } else if (msgsend == "ERR") {
            $.bootstrapGrowl('@PageData("msg-err")', {
                type: 'danger',
                delay: 2000,
                width: '100%'
            });

        }

       function fotodefault(idfoto) {
            $("#" + idfoto).attr("src", "poolin_arch/images/png/_avatar_.png");
        }
        function tiempotrans(fecha) {
            var nacimiento = new Date(fecha)
            var hoy = new Date()

            var tiempoPasado= hoy - nacimiento
            var segs = 1000;
            var mins = segs * 60;
            var hours = mins * 60;
            var days = hours * 24;
            var months = days * 30.416666666666668;
            var years = months * 12;

            //calculo 
            var anos = Math.floor(tiempoPasado / years);

            tiempoPasado = tiempoPasado - (anos * years);
            var meses = Math.floor(tiempoPasado / months)

            tiempoPasado = tiempoPasado - (meses * months);
            var dias = Math.floor(tiempoPasado / days)

            tiempoPasado = tiempoPasado - (dias * days);
            var horas = Math.floor(tiempoPasado / hours)

            tiempoPasado = tiempoPasado - (horas * hours);
            var minutos = Math.floor(tiempoPasado / mins)

            tiempoPasado = tiempoPasado - (minutos * mins);
            var segundos = Math.floor(tiempoPasado / segs);
            // Mostrar el resultado.
            if (anos != 0) {
                return anos + " año(s)";
            }

            if (dias != 0) {
                return dias + " dia(s)";
            }
            if (horas != 0) {
                return horas + " hr(s)";
            }
            if (minutos != 0) {
                return minutos + " min(s)";
            }
            return segundos + " seg(s)";
            //Output: 164 días, 23 horas, 0 minutos, 0 segundos.
        }

    </script>

    <script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            var username = $("#iduser").val();
            var proxy = $.connection.messageHub;

            $.connection.hub.start().done(function () {
                proxy.server.connect(username);
            });

            $.connection.hub.start();

            //proxy.client.recibenoti = function (message, user, userenvio) {
            //    console.log("EMP " + user);
            //    console.log("EMP ENVIO + : " + userenvio);
            //    if (document.getElementById("iduser").value == user) {
            //        alert("Mensaje recibido");
            //        carga_headmsg();
            //        make_msg(user);
            //    }
            //};
            //proxy.client.updateUsers = function (userCount, userList) {
            //    //$('#onlineUsersCount').text('Online users: ' + userCount);
            //    //$('#userList').empty();
            //    userList.forEach(function (username) {
            //        //$('#userList').append('<li>' + username + '</li>');
            //        //$('#idcol-' + username).removeClass('text-success');
            //        //$('#idcol-' + username).toggleClass('text-success');
            //    });
            //};

            $("#btnsendmsg").click(function () {
                if ($("#tbmsg").val() == "") {
                    $("#tbmsg").focus();
                    return false;
                }
                $.ajax({
                    type: 'POST',
                    url: 'wsmensajes.asmx/Enviar_Mensaje',
                    data: "{idemisor: @idemp, idreceptor: " + $("#idreceptor").val() + ", asunto: '', mensaje:'" + $("#tbmsg").val() + "', idproyecto: " + $("#idproyecto").val() + "}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: function (response) {
                        //$('.leido-' + idemisor + '_' + idproy).remove();
                        proxy.server.senmessage($('#tbmsg').val(), $('#idreceptor').val(), $('#iduser').val());
                        $("#tbmsg").val("");
                        $("#tbmsg").focus();
                        $(".upfilesdel").remove();
                        Carga_Detalle_Mensajes(response);
                        alert("Mensaje Enviado.");
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("Error: " + textStatus);
                        return;
                    }
                });
            });
            $.connection.hub.logging = true;
            //proxy.client.updateUsers = function (userCount, userList) {
            //    //$('#onlineUsersCount').text('Online users: ' + userCount);
            //    //$('#userList').empty();
            //    userList.forEach(function (username) {
            //        //$('#userList').append('<li>' + username + '</li>');
            //        //$('#idcol-' + username).removeClass('text-success');
            //        //$('#idcol-' + username).toggleClass('text-success');
            //    });
            //};

            //$.connection.hub.start().done(function () {
            //    console.log("POOL-EMP  username: " + username);
            //    proxy.server.connect(username);
            //    console.log("POOL-EMP  connect: " + username);
            //});

            ////myHub = $.connection.messagehub;
            //try {
            //    //$.connection.hub.logging = true;
            //    console.log("POOL-EMP  Conecta ON");
            //    $.connection.hub.start();
            //    console.log("POOL-EMP  Conecta OK");
            //}
            //catch (e)
            //{
            //    console.log("POOL-EMP  " + e.message);
            //}

            //$("#btnsendproxy").click(function () {
            //    if ($('#tbresp').val() == "") {
            //        $('#tbresp').focus();
            //        return;
            //    }
            //    try {
            //        proxy.server.senmessage($('#mensaje').val(), $('#idreceptor').val(), $('#iduser').val());
            //        $.ajax({
            //            type: 'POST',
            //            url: 'wsmensajes.asmx/Responde_Mensaje',
            //            data: "{idemail: '" + $("#idmsgresp").val() + "', asunto:'" + $("#tbasunto").val() + "', mensaje: '" + $('#tbresp').val() + "'}",
            //            contentType: 'application/json; utf-8',
            //            dataType: 'json',
            //            success: function (data) {
            //                $('#tbresp').val("");
            //                $('#tbresp').focus();
            //            },
            //            error: function (jqXHR, textStatus, errorThrown) {
            //                alert("Error: " + textStatus);
            //                return; 
            //            }
            //        });
            //        console.log("ENVIAdo..." + $("#iduser").val());
            //    }
            //    catch (e)
            //    {
            //        alert("Mensaje NO enviado. " + e.message);
            //    }
            //});
        });
        
    </script>
End Section