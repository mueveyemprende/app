<!DOCTYPE html>
<html lang="es">
<head>
    <title>M&E - @PageData("Title") </title>
    <meta http-equiv=”Expires” content=”0″>
    <meta http-equiv=”Last-Modified” content=”0″>
    <meta http-equiv=”Cache-Control” content=”no-cache, mustrevalidate”>
    <meta http-equiv=”Pragma” content=”no-cache”>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="shortcut icon" type="image/png" href="img/logo-carr.jpg" />
    @RenderSection("head", False)

    @*<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css?2.0">*@
    <link rel="stylesheet" type="text/css" href="css/bootstrap.css?1.6">
    <link rel="stylesheet" type="text/css" media="screen" href="css/font-awesome.min.css">
    <link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?25.01">
    <link href="https://gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">

</head>
<body>
    @code
        Dim objmsg As New poolin_class.cMensajes
        Dim idemp As Long = Request.Cookies("idemp").Value
        Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
        'Dim dt As System.Data.DataTable = objmsg.Genera_Mensaje_Preview_vm(idemp, strConn)
        Dim noti As Byte = 0

        If "" & Request.Form("idioma") <> "" Then
            Dim objEmp As New poolin_class.cEmprendedor
            objEmp.Update_LANG(idemp, Request.Form("idioma"), strConn)
        End If

        Dim objComun As New poolin_class.cComunes
        Dim idioma As String = Request.Form("idioma")
        Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "_PoolinLayout", strConn)
        For Each dr As System.Data.DataRow In dtEt.Rows
            PageData(dr("nomobj")) = dr("valor")
        Next
        Dim avatar As String = "avatar_"
        Dim di As String = Dir(Server.MapPath("poolin_arch/images/png/_avatar_" & idemp & ".png"))
        If di <> "" Then
            avatar = di & "?" & Format(Now, "d.ss")
        End If

    End Code

    

    <div class="modal fade" id="modaleliminar"  tabindex="-1" role="dialog" aria-labelledby="modaleliminar"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-4">
                                <img src="Images/membresia.png" title="En Construcción" alt="M&E En Construcción" style="width:100%" />
                            </div>
                            <dib class="col-md-8 mt-5">
                                <span class="title-pool">Estos increíbles servicios aún se están diseñando, por el momento no están disponibles.</span>
                            </dib>
                        </div>
                    </div>
            </div>
        </div>
    </div>
</div>

    <header class="clearfix">
        <div id="Poolin-logo">
            <a href="logout">
                <img class="img-lg" src="~/img/logo.png">
                <img class="img-sm" src="~/img/logo-carr.png">
            </a>
        </div>
        <div id="menu-Poolin-account">
            <ul>
                <li id="sidecollapse">
			        <a href="javascript:;"><i class="btn bg-white-1 blue fas fa-bars"></i></a>
                </li>
                <li class="idiomas hide" onclick="">
                    <a id="idioma"><i class="btn bg-blue-1 white fas fa-info" ></i></a>
                    <ul class="dropdown-content idioma" onclick="">
                        <li>
                            <form id="frmidioma-esp" action="@PageData("pagina")" method="post">
                                <input type="hidden" name="idioma" value="ESP" />
                                <a href="javascript:;" onclick="cambiaidioma('esp')" id="a-esp">Español</a>
                            </form>
                        </li>
                        <li>
                            <form id="frmidioma-ing" action="@PageData("pagina")" method="post">
                                <input type="hidden" name="idioma" value="ING" />
                                <a href="javascript:;" onclick="cambiaidioma('ing')" id="a-ing">Inglés</a>
                            </form>
                        </li>
                    </ul>
                </li>
                @*<li class="poolinapps" onclick="">
                    <a id="poolinapp"><i class="btn bg-white-1 blue fas icon-pool" style="margin-right:25px;"></i><img src="img/2000px-Black_triangle.svg.png"  style="margin-left:5px; width:8px;"/></a>
                    <ul class="dropdown-content poolinapp" onclick=""> 
                        <li Class="dropdown-title">Poolin</li>
                        <li><a href="javascript:;">CONECTA</a></li>
                        <li><a href="javascript:;" data-toggle="modal" data-target="#modaleliminar">EDU</a></li>
                        <li><a href="javascript:;" data-toggle="modal" data-target="#modaleliminar">MENTORÍA</a></li>
                        <li><a href="javascript:;" data-toggle="modal" data-target="#modaleliminar">RECURSOS</a></li>
                    </ul>
                </li>*@

                <li Class="notifications" onclick="">
                    <input type="hidden" value="@idemp" id="iduser" />
                    <a id="alert-bell"><i id="noti-i" class="fas fa-envelope-open" style="">
                         </i> <img src="img/2000px-Black_triangle.svg.png"  style="margin-left:-5px; width:8px;"/></a>
                    <ul id="msglist" class="dropdown-content notification" onclick=""> 

                    </ul>
                </li>
                <li Class="users-menu" onclick="">
                    <a><i class="btn bg-white-1 blue fas fa-user"></i> <img src="img/2000px-Black_triangle.svg.png"  style="margin-left:-5px; width:8px;"/></a>
                    <ul Class="dropdown-content user-menu"  onclick="">
                        <li><a href="micuenta"> @PageData("toolbar_menu_1")<br><span Class="mi-user">@Request.Cookies("emp").Value</span></a></li>
                        <li><a href="user-perfil">@PageData("toolbar_menu_2")</a></li>
                        <li><a href="logout">@PageData("toolbar_menu_3")</a></li>
                    </ul>
                </li>
            </ul>


        </div>

    </header>
    <div class="wrapper">            
        <nav id="sidebar" class="active">
            @*<div style="background-color:#071e6e !important; color:#fff; text-align:center; padding:20px; font-weight:bold; font-size:22px;">CONECTA</div>*@
            <div style="text-align:center; padding-top:30px;">
                <img src="poolin_arch/images/png/@avatar" class="misdatos-foto-bar" onerror="this.src='img/logo-carr.jpg';this.onerror='';">
            </div>
            <ul class="list-unstyled components">
                <li class="@PageData("opperfil")">
                    <a href="user-perfil" title="@PageData("sidebar_menu_2")"><i class="demo-icon icon-user"></i><span class="nav-title"> @PageData("sidebar_menu_2")</span></a>
                </li>
                <li class="@PageData("opdash")">
                    <a href="user-dash" title="@PageData("sidebar_menu_3")"><i class="demo-icon icon-dash"></i><span class="nav-title"> @PageData("sidebar_menu_3")</span></a>
                </li>
                <li class="@PageData("oppool")">
                    <a href="user-pool" title="@PageData("sidebar_menu_4")"><i class="demo-icon icon-dash"></i><span class="nav-title"> @PageData("sidebar_menu_4")</span></a>
                </li>
                <li class="@PageData("oppool2")">
                    <a href="user-pool-emp" title="@PageData("sidebar_menu_5")"><i class="demo-icon icon-dash"></i><span class="nav-title"> @PageData("sidebar_menu_5")</span></a>
                </li>
                @*<li class="@PageData("opproy")">
                    <a href="user-proyectos" title="@PageData("sidebar_menu_6")"><i class="demo-icon icon-proyectos"></i><span class="nav-title"> @PageData("sidebar_menu_6")</span></a>
                </li>
                <li class="@PageData("opmens")">
                    <a href="user-mensajes" title="@PageData("sidebar_menu_7")"><i class="demo-icon icon-mensaje"></i><span class="nav-title"> @PageData("sidebar_menu_7")</span></a>
                </li>*@
                @*<li class="@PageData("opcal")">
                    <a href="user-calendario" title="@PageData("sidebar_menu_8")"><i class="demo-icon icon-calendario"></i><span class="nav-title"> @PageData("sidebar_menu_8")</span></a>
                </li>*@
                <li class="proyecto-nuevo hide">
                    @*<a href="publicar-proyectos" title="@PageData("sidebar_menu_1")"></a>*@
                    <button type="button" id="btngoproy"><span class="nav-title">@PageData("sidebar_menu_1")</span></button>
                </li>
            </ul>
        </nav>
        @RenderBody()
    </div>

    <footer>
    <div>
        <div class="row align-items-center">
            <div class="col-sm-3">
                <a href="/">
                    <img class="logo-w" src="~/img/logo-layout.png">
                </a>
                <a href="https://www.youtube.com/" target="_blank"><i class="fab fa-youtube"></i></a>
                <a href="https://www.linkedin.com/company/mueveyemprende" target="_blank"><i class="fab fa-linkedin-in"></i></a>
                <a href="https://www.facebook.com/mueveyemprende/?" target="_blank"><i class="fab fa-facebook-f"></i></a>
                <p class="white"> Copyright © 2020 mueve y emprende. <br />@PageData("footer_op_1")</p>
            </div>
            <div class="col-sm-6">
                <div class="row">
                    <ul class="col-sm-4 nostyle">
                        <li><a class="white" target="_blank" href="me-oferta">@PageData("footer_op_2")</a></li>
                        <li><a class="white" target="_blank" href="me-seguro">@PageData("footer_op_3")</a></li>
                        <li><a class="white" target="_blank" href="#">@PageData("footer_op_4")</a></li>
                    </ul>
                    <ul class="col-sm-4 nostyle">
                        <li><a class="white" target="_blank" href="me-codigo-conducta">@PageData("footer_op_5")</a></li>
                        <li><a class="white" target="_blank" href="me-aviso-privacidad">@PageData("footer_op_6")</a></li>
                        <li><a class="white" target="_blank" href="me-terminos">@PageData("footer_op_7")</a></li>
                    </ul>
                    <ul class="col-sm-4 nostyle">
                        <li><a class="white" target="_blank" href="~/me-soporte">@PageData("footer_op_8")</a></li>
                    </ul>
                </div>
            </div>
            @*<div class="col-sm-3">
                <div class="row">
                    <div class="col-sm-12">
                        <h5 class="white">@PageData("footer_app")</h5>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6 ">
                        <a href="#"><img src="images/app-store.png" class="img-apps"></a>
                    </div>
                    <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6 ">
                        <a href="#"><img src="images/google-play.png" class="img-apps"></a>
                    </div>
                </div>
            </div>*@
        </div>
    </div>


    </footer>

    @RenderSection("Scripts", required:=False)


	<!-- ================== BEGIN BASE JS ================== -->
	<script src="assets/plugins/jquery/jquery-3.2.1.min.js"></script>
	<script src="assets/plugins/jquery-ui/jquery-ui.min.js"></script>
	<script src="assets/plugins/cookie/js/js.cookie.js"></script>
	<script src="assets/plugins/tooltip/popper/popper.min.js"></script>
	<script src="assets/plugins/bootstrap/bootstrap4/js/bootstrap.min.js"></script>
	<script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>
	<!-- ================== END BASE JS ================== -->


    <script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>

    <script>
        $(document).ready(function () {
            $('#sidecollapse').on('click', function () {
                $('#sidebar').toggleClass('active');
            });

            $("#btngoproy").on('click', function () {
                window.location.href = 'publicar-proyectos';
            });
        });


        function gotomsg(valor) {
            document.getElementById("frmmsg-" + valor).submit();
        }

        var noti = "@noti";
        if (noti == "1") {
            document.getElementById("noti-i").style.backgroundColor = "lightgreen";
            document.getElementById("noti-i").style.border = 0;
        }

        function cambiaidioma(valor) {
            document.getElementById("frmidioma-" + valor).submit();
        }

        var idioma = "@idioma";

        switch (idioma) {
            case "ESP":
                document.getElementById("a-esp").innerText = "* Español";
                document.getElementById("a-ing").innerText = "Inglés";
                break;
            case "ING":
                document.getElementById("a-esp").innerText = "Spanish";
                document.getElementById("a-ing").innerText = "* English";
                break;
        }

        var hiddemenu = "@PageData("hiddemenu")";
        if (hiddemenu == "hidden") {
            document.getElementById("sidebar").style.display="none";
        }
    </script>



    <script>
        function make_msg(user) {
            $.ajax({
                    type: 'POST',
                    url: 'wsmensajes.asmx/loadmsg',
                    data: "{user: '" + user + "'}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: carga_msg,
                    failure: function (response) {
                        alert("1 - " + response.responseText);
                    },
                    error: function (response) {
                        alert("2 - " + response.responseText);
                    }
                });
        }

        //function gomsg(userid, usuario, idmsgpadre, msgrv) {
        //    $("#userenviar").val(userid);
        //    $("#usermsg").text(usuario);
        //    $("#idmsgpadre").val(idmsgpadre);
        //    $("#sendmessage").modal();
        //    if (idmsgpadre == "0") {
        //        $("#rvmsg").hide();
        //    }
        //    else {
        //        $("#msgrv").val("RV: " + msgrv);
        //    }
        //}


        function carga_msg(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("msguser");      
            $('#msglist').empty();
            //$("#alert-bell").removeClass("bg-danger");
            //$("#noti-i").removeClass("text-danger");
            $("#noti-i").css("color", "");
            var bgcolor = "";
            if (detalle.length == 0) {
                $('#msglist').append("<li class='dropdown-title'>" +
                    "<a href='javascript:;'>" +
                    "<div class='notification-info'>" +
                    "<p class='notification-desc'>No hay nuevos mensajes." +
                    "</p>" +
                    "</div></a></li>");
            } else {
                $('#msglist').append('<li Class="dropdown-title">@PageData("titnoti")</li>');
            }

            $(detalle).each(function () {
                bgcolor = "";
                if ($(this).find("estatus").text() == "N") {
                    //if (!($("#alert-bell").hasClass("bg-danger"))) {
                    //    $("#alert-bell").toggleClass("bg-danger");
                    //}
                    $("#noti-i").css("color", "");
                    $("#noti-i").css("color", "#e5ba6a");
                    bgcolor = "bg-warning";
                }

                //$('#msglist').append("<li class='" + bgcolor + "'>" +
                //    "<a href='javascript:;' onclick=\"gomsg('" + $(this).find("idemisor").text() + "','" + $(this).find("nombres").text() + "','" + $(this).find("id").text() + "','" + $(this).find("asunto").text() + "')\">" +
                //    "<div class='notification-icon bg-muted'>" +
                //    "<i class='ti-user'></i>" +
                //    "</div>" +
                //    "<div class='notification-info'>" +
                //    "<h4 class='notification-title'>" + $(this).find("nombres").text() + " <span class='notification-time'>" + $(this).find("pubs").text() + "</span></h4>" +
                //    "<p class='notification-desc'>" +
                //    $(this).find("mensaje").text() +
                //    "</p>" +
                //    "</div></a></li>");
                var asunto = "Mensaje Directo";
                if ($(this).find("Proyecto").text() != "") {
                    asunto = $(this).find("Proyecto").text();
                }
                $("#msglist").append('<li id=' + $(this).find("id").text() + ' onclick="gotomsg(this.id)">' +
                    '<form id="frmmsg-' + $(this).find("id").text() + '" action="user-mensajes" method="post">' + 
                    "<a href='javascript:;' style='background-color: #d4d4d4;'>" +
                    '<input type="hidden"  name="idmsg" value="' + $(this).find("id").text() + '" />' +
                    '<div class="col-xs-3">' +
                    '     <img src="poolin_arch/images/png/' + $(this).find("foto").text() + '" class="img-responsive">' +
                    ' </div>' +
                    '<div class="col-xs-9"><strong>' + $(this).find("nombres").text() + '<br><small>' + asunto + '</small><br><span class="time">Hace ' + $(this).find("pubs").text() + '</span></strong></div>' +
                    '</a>' +
                    '</form> ' + 
                    '</li>');
            });

        }
    </script>
    <script type="text/javascript">

        $(function () {
            var username = $("#iduser").val();
            var proxy = $.connection.messageHub;
            proxy.client.recibenoti = function (message, user, userenvio) {
                if (document.getElementById("iduser").value == user) {
                    $("#noti-i").css("color", "");
                    $("#noti-i").css("color", "red");
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

            //myHub = $.connection.messagehub;
            try {
                //$.connection.hub.logging = true;
                $.connection.hub.start();
            }
            catch (e)
            {
                console.log(e.message);
            }
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
        make_msg($("#iduser").val());
    </script>

</body>
</html>