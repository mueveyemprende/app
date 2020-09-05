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

    Validation.RequireField("tbresp", "<br>Debes indicar el mensaje.")

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString


    Dim objmsg As New poolin_class.cMensajes
    Dim dthead As System.Data.DataTable = objmsg.Mensaje_Head(idemp, strConn)
    Dim dtDetalle As System.Data.DataTable
    Dim idmsg As Long = 0
    If Not IsNothing(Request.Form("idmsg")) Then
        idmsg = Request.Form("idmsg")
    Else
        If dthead.Rows.Count <> 0 Then
            idmsg = dthead.Rows(0)("id")
        End If
    End If
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
    dtDetalle = objmsg.Mensaje_Detalle(idmsg, strConn)

    Dim objComun As New poolin_class.cComunes
    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-mensajes", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
End Code
@section head

End Section
<div id="content">
    <h3 class="user_title">@PageData("titulo")</h3>

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
                <div class="chat-list">
                    <form id="frmmsg" action="~/user-mensajes" method="post">
                        <input type="hidden" id="idmsg" name="idmsg" value="@idmsg" />
                        <ul class="list-unstyled" id="listhead">
                        </ul>
                    </form>

                </div>
            </div>
        </div>
        <div Class="col-xs-9 col-sm-9 message_section">
            @For Each dr As System.Data.DataRow In dtDetalle.Rows
                @<div Class="row">
                    <div class="message_head">
                        <div Class="pull-left">
                            @Html.Raw(dr("asunto")) <span><small>@Html.Raw(Format(dr("fecha_creacion"), "dd/MMM/yyyy HH:mm").Replace(".", ""))</small></span><br />
                            <small>@Html.Raw(dr("nombre")) <<a href="mailto:@dr("email")">@dr("email")</a>></small>
                        </div>
                    </div>
                    <div Class="chat_area">
                        <ul Class="list-unstyled">
                            <li Class="left clearfix">
                                <div Class="chat-body clearfix">
                                    <p> @Html.Raw(dr("mensaje"))</p>
                                    @*<div Class="chat_time pull-right">09:40PM</div>*@
                                </div>
                            </li>
                        </ul>
                    </div>
                    <div Class="message_write">
                        <form id="form-send" method="post" action="~/user-mensajes">
                            <input name="msgemail" type="hidden" value="@Html.Raw(dr("email"))" />
                            <input id="idmsgresp" name="idmsgresp" type="hidden" value="@idmsg" />
                            <input id="tbasunto" name="tbasunto" type="hidden" value="RE: @Html.Raw(dr("asunto"))" />
                            <input id="idreceptor" name="idreceptor" type="hidden" value="@dr("idemisor")" />
                            <div>
                                <span class="msg-val">@Html.ValidationMessage("tbresp")</span>
                                <input type="text" class="form-control" id="tbresp" name="tbresp" placeholder="@PageData("pholder-2")" @Validation.For("tbresp") />
                            </div>
                            <div Class="pull-right">
                                <button type="button" id="btnsendproxy"><i class="fa fa-send"></i></button>
                            </div>

                            <div Class="clearfix"></div>
                        </form>
                    </div>
                </div>
            Next
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

    @*<script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>*@

    <script>
        $(document).ready(function () {
            carga_headmsg();
        });

        function gotoresp() {

            document.getElementById("form-send").submit;
        }
        function gotomsg(valor) {
            document.getElementById("idmsg").value = valor;
            document.getElementById("frmmsg").submit();
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

        function carga_headmsg() {
            $.ajax({
                type: 'POST',
                url: 'wsmensajes.asmx/Mensaje_Head',
                data: "{idemp: " + @idemp + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: function (response) {
                    var xmlDoc = $.parseXML(response.d);
                    var xml = $(xmlDoc);
                    var detalle = xml.find("head");
                    $('#listhead').empty();
                    var li = "";
                    var activo = "";
                    var fotofile = "";
                    $(detalle).each(function () {
                        fotofile = "_avatar_" + $(this).find("idemisor").text() + ".png";
                        activo = "";
                        if ($(this).find("id").text() == $("#idmsg").val()) {
                            activo = "active";
                        }
                        //alert($(this).find("Proyecto").text());
                        li = '';
                        li += '       <li id="' + $(this).find("id").text() + '" class="' + activo + '" onclick="gotomsg(this.id)">';
                        li += '            <div class="hidden-xs hidden-sm hidden-md">';
                        li += '                <span class="chat-img pull-left">';
                        li += '                    <img id="foto-' + $(this).find("id").text() + '" src="poolin_arch/images/png/' + fotofile + '" onerror="fotodefault(this.id)">';
                        li += '                </span>';
                        li += '            </div>';
                        li += '            <div class="clearfix">';
                        li += '                <div class="chat-list-header">';
                        li += '                    ' + $(this).find("nombre").text();
                        li += '                </div>';
                        li += '                <div class="chat-list-text">';
                        li += '                    ' + $(this).find("asunto").text();
                        li += '                </div>';
                        li += '                <span Class="pull-right chat-time">';
                        li += '                    <strong>' + tiempotrans($(this).find("fecha_creacion").text()) +  '</strong>';
                        li += '                </span>';
                        li += '            </div>';
                        li += '        </li>';
                        
                        $("#listhead").append(li);
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Error: " + textStatus);
                    return;
                }
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
            proxy.client.recibenoti = function (message, user, userenvio) {
                console.log("POOLXXX EMP " + user);
                console.log("POOLYYY EMP ENVIO + : " + userenvio);
                if (document.getElementById("iduser").value == user) {
                    carga_headmsg();
                    make_msg(user);
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
                proxy.server.connect(username);
            });
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

            $("#btnsendproxy").click(function () {
                if ($('#tbresp').val() == "") {
                    $('#tbresp').focus();
                    return;
                }
                try {
                    proxy.server.senmessage($('#mensaje').val(), $('#idreceptor').val(), $('#iduser').val());
                    $.ajax({
                        type: 'POST',
                        url: 'wsmensajes.asmx/Responde_Mensaje',
                        data: "{idemail: '" + $("#idmsgresp").val() + "', asunto:'" + $("#tbasunto").val() + "', mensaje: '" + $('#tbresp').val() + "'}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
                            $('#tbresp').val("");
                            $('#tbresp').focus();
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
        });
        
    </script>
End Section