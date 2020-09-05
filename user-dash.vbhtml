@functions
    Private Sub Envia_Msg_AdminPoolin(ByVal idemp As Long, ByVal idproyecto As Long, ByVal asunto As String,
                                      ByVal mensaje As String, ByVal strConn As String)
        Dim cProy As New poolin_class.cProyectos
        Dim dt As System.Data.DataTable = cProy.Proyecto_Propuesta(idproyecto, strConn)
        Dim cEmp As New poolin_class.cEmprendedor
        Dim dtEmp As System.Data.DataTable = cEmp.Datos_Emprendedor(idemp, strConn)
        If dt.Rows.Count <> 0 Then
            'Using smtp As New SmtpClient
            '    Dim correo As New MailMessage

            Dim c As New poolin_class.cComunes
            Dim _smtp As String = ""
            Dim user As String = ""
            Dim pwd As String = ""
            Dim puerto As Long = 0
            Dim ssl As Boolean = False
            Dim http As String = ""

            Dim htmlmsg As String = ""

            If dtEmp.Rows.Count <> 0 Then
                htmlmsg &= "<h1>Proyecto puesto en controversia por el Emprendedor: " & dtEmp.Rows(0).Item("nombres") & " " & dtEmp.Rows(0).Item("apellidos")
                If "" & dtEmp.Rows(0).Item("empresa") <> "" Then
                    htmlmsg &= " (" & dtEmp.Rows(0).Item("empresa") & ")"
                End If
                htmlmsg &= "</h1>"
            End If

            htmlmsg &= "<p>"
            htmlmsg &= "ASUNTO: " & asunto
            htmlmsg &= "</p>"
            htmlmsg &= "<p>"
            htmlmsg &= "MENSAJE: " & mensaje
            htmlmsg &= "</p>"
            htmlmsg &= " <hr />"
            htmlmsg &= "PROYECTO <strong>" & dt.Rows(0).Item("Proyecto") & "</strong>"
            htmlmsg &= "<h2>Cliente</h2>"
            htmlmsg &= "<p>"
            htmlmsg &= "Nombre: " & dt.Rows(0).Item("cliente").ToString.ToUpper & "<br />"
            If "" & dt.Rows(0).Item("empresa") <> "" Then
                htmlmsg &= "Empresa: " & dt.Rows(0).Item("empresa").ToString.ToUpper & "<br />"
            End If
            htmlmsg &= "Email 1: " & dt.Rows(0).Item("mail") & "<br />"
            htmlmsg &= "Email 2: " & dt.Rows(0).Item("mail2") & "<br />"
            htmlmsg &= "</p>"
            'htmlmsg &= "<h2>ENTRO EN CONTROVERSIA CON EL EMPRENDEDOR</h2>"
            htmlmsg &= "<p>"
            htmlmsg &= "<h2>Proveedor</h2>"
            htmlmsg &= "Nombre: " & dt.Rows(0).Item("proveedor").ToString.ToUpper & "<br />"
            If "" & dt.Rows(0).Item("empproveedor") <> "" Then
                htmlmsg &= "Empresa: " & dt.Rows(0).Item("empproveedor").ToString.ToUpper & "<br />"
            End If
            htmlmsg &= "Email 1: " & dt.Rows(0).Item("email") & "<br />"
            htmlmsg &= "Email 2: " & dt.Rows(0).Item("email2") & "<br />"
            htmlmsg &= "</p>"
            htmlmsg &= "<p>Pongase en contacto con los emprendedores para solucionar la Controversia."
            htmlmsg &= "</p>"


            Dim dtMails As System.Data.DataTable = c.Datos_Config(strConn)
            If dtMails.Rows.Count = 0 Then Exit Sub
            Dim correos As String = dtMails.Rows(0).Item("mailcontro")
            correos = correos.Replace(";", ",")
            Dim objproy As New poolin_class.cProyectos
            Dim dtProy As System.Data.DataTable = objproy.Carga_Proyectos_EnEjecucion(0, strConn, idproyecto)
            Dim objSend As New poolin_class.cSendGrid
            If dtProy.Rows.Count <> 0 Then
                objSend.Correo_SMTP("PROYECTO EN CONTROVERSIA", htmlmsg, correos, String.Format("{0};{1}", dtProy.Rows(0)("email"), dtProy.Rows(0)("email_proy")), strConn)
            Else
                objSend.Correo_SMTP("PROYECTO EN CONTROVERSIA", htmlmsg, correos, "", strConn)
            End If

        End If

    End Sub
End Functions

@code 
    PageData("pagina") = "user-dash"
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
    Dim objProy As New poolin_class.cProyectos
    Dim msgsave As String = ""

    If Request.Form("FINNOPROY") = "OK" Then
        Try
            objProy.Actualiza_Proyecto(idemp, Request.Form("idproyecto"), "CV", strConn, 0, idemp)
            Envia_Msg_AdminPoolin(idemp, Request.Form("idproyecto"), "PROYECTO EN CONTROVERSIA", Request.Form("mensaje"), strConn)
            msgsave = "CV"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If
    If Request.Form("delproy") = "OK" Then
        Try
            objProy.Actualiza_Proyecto(idemp, Request.Form("idproyecto"), "E", strConn, 0, 0)
            msgsave = "PYDEL"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If

    Dim idioma As String = Request.Form("idioma")
    Dim dtEmprendedor As System.Data.DataTable = objProy.Proyecto_Emprendedor(idemp, strConn)
    Dim dtEmpleador As System.Data.DataTable = objProy.Proyecto_Empleador(idemp, strConn)
    Dim objComun As New poolin_class.cComunes
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-dash", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    Dim mes(12) As String
    mes(1) = PageData("mes1")
    mes(2) = PageData("mes2")
    mes(3) = PageData("mes3")
    mes(4) = PageData("mes4")
    mes(5) = PageData("mes5")
    mes(6) = PageData("mes6")
    mes(7) = PageData("mes7")
    mes(8) = PageData("mes8")
    mes(9) = PageData("mes9")
    mes(10) = PageData("mes10")
    mes(11) = PageData("mes11")
    mes(12) = PageData("mes12")
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Dashboard"
    PageData("opdash") = "active"

    Dim objDash As New poolin_class.cDashboard
    Dim objAdmin As New poolin_class.cAdmin

    Dim dtMis As System.Data.DataTable = objDash.Resumen_Proyectos(idemp, strConn)
    Dim drMis As System.Data.DataRow = dtMis.Rows(0)

    dtMis = Nothing
    dtMis = objDash.Resumen_Mis_Proyectos(idemp, strConn)
    Dim drMisProy = dtMis.Rows(0)

    Dim dtProy As System.Data.DataTable = objDash.Proyectos(idemp, strConn)
    Dim dtMisProy As System.Data.DataTable = objProy.Carga_Proyectos(idemp, strConn,,,,, "%%")
    Dim dtProy2 = objAdmin.Proyectos_Activos_V2(idemp, "", strConn, "'P','V'")
    Dim dtEmpIgual = objDash.Emprendedor_Afin(idemp, strConn)

    Dim dtEstEmpleado As System.Data.DataTable = objDash.Pagos_Recibidos(idemp, Year(Now), strConn, False)
    Dim objEmp As New poolin_class.cEmprendedor
    Dim cat(0) As String
    Dim dtEmp As System.Data.DataTable = objAdmin.Emprendedores_m(cat, "", "", "", strConn, Val(Request.Form("idempview")))

    Dim lblmes As String = ""
    Dim monmes(1) As String
    monmes(0) = ""
    monmes(1) = ""
    For i As Int16 = 1 To Month(Now)
        lblmes &= ",'" & mes(i) & "'"
        monmes(0) &= ",@" & i
        monmes(1) &= ",@" & i
    Next

    If lblmes <> "" Then
        lblmes = Mid(lblmes, 2)
        monmes(0) = Mid(monmes(0), 2)
        monmes(1) = Mid(monmes(1), 2)
    End If

    For Each dr As System.Data.DataRow In dtEstEmpleado.Rows
        monmes(0) = monmes(0).Replace("@" & dr("mes"), String.Format("{0:0.00}", dr("monto")))
    Next

    For i As Int16 = 1 To Month(Now)
        monmes(0) = monmes(0).Replace("@" & i, "0")
    Next

    dtEstEmpleado = Nothing
    dtEstEmpleado = objDash.Pagos_Recibidos(idemp, Year(Now), strConn, True)

    For Each dr As System.Data.DataRow In dtEstEmpleado.Rows
        monmes(1) = monmes(1).Replace("@" & dr("mes"), String.Format("{0:0.00}", dr("monto")))
    Next

    For i As Int16 = 1 To Month(Now)
        monmes(1) = monmes(1).Replace("@" & i, "0")
    Next

    Dim dtdetalle1 As System.Data.DataTable = objDash.Pagos_Detalles(idemp, Now.Year, strConn, False)
    Dim dtdetalle2 As System.Data.DataTable = objDash.Pagos_Detalles(idemp, Now.Year, strConn, True)

    Dim total1 As Double = 0
    Dim total2 As Double = 0

    'Control de proyectos
    Dim idproyecto As Long = 0
    If Request.QueryString("idproyecto") <> "" Then
        idproyecto = objComun.Decrypt(Request.QueryString("idproyecto"))
    End If

    If Request.Form("EVALCTE") = "OK" Then
        'Graba evaluación
        Try
            Dim dt As System.Data.DataTable = objComun.Preguntas("E", "ESP", strConn)
            Dim dtEva As New System.Data.DataTable
            dtEva.Columns.Add("idpregunta", GetType(Integer))
            dtEva.Columns.Add("calif", GetType(Integer))
            Dim drEva As System.Data.DataRow
            For Each drow As System.Data.DataRow In dt.Rows
                drEva = dtEva.NewRow
                drEva("idpregunta") = drow("id")
                drEva("calif") = 0
                If Not IsNothing(Request.Form("resp-" & drow("id"))) Then
                    drEva("calif") = Request.Form("resp-" & drow("id")) ' : Exit For
                End If
                dtEva.Rows.Add(drEva)
            Next
            Dim dtProyCtrl As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(idemp, strConn, idproyecto, "%%")
            If dtProyCtrl.Rows.Count <> 0 Then
                objProy.Evaluacion(idproyecto, idemp, dtProyCtrl.Rows(0)("idcliente"), dtEva, strConn)
                msgsave = "FINFE"
            Else
                msgsave = "ERR"
            End If
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If
    If Request.QueryString("yaeval") <> "" Then
        msgsave = "YAEVAL"
    End If

    dtEt = objComun.Etiquetas_m(idemp, idioma, "user-proyectos", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
    Validation.RequireField("mensaje", "<br> Requerido")
    PageData("Title") = "Mi Escritorio"
End Code
@section head
        <link rel="stylesheet" type="text/css" href="css/bootstrap.css?1.5">
    <link rel="stylesheet" type="text/css" media="screen" href="css/font-awesome.min.css">
    
    <link href="https://gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">

	<!-- ================== BEGIN PAGE CSS STYLE ================== -->
    <link href="assets/plugins/table/DataTables-1.10.18/datatables.css" rel="stylesheet"/>
	<link href="assets/plugins/table/DataTables/DataTables-1.10.16/css/dataTables.bootstrap4.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/AutoFill-2.2.0/css/autoFill.bootstrap.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/Buttons-1.3.1/css/buttons.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/ColReorder-1.3.3/css/colReorder.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/FixedColumns-3.2.2/css/fixedColumns.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/FixedHeader-3.1.2/css/fixedHeader.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/KeyTable-2.2.1/css/keyTable.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/Responsive-2.1.1/css/responsive.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/RowGroup-1.0.0/css/rowGroup.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/RowReorder-1.2.0/css/rowReorder.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/Scroller-1.4.2/css/scroller.bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/table/DataTables/Select-1.2.2/css/select.bootstrap.min.css" rel="stylesheet" />
	<!-- ================== END PAGE CSS STYLE ================== -->
    <!-- ================== END PAGE CSS STYLE ================== -->
    <link href="css/resCarousel.css?5.0" rel="stylesheet" type="text/css">

    @*<link rel="stylesheet" href="lib/fullcalendar/fullcalendar_gebo.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/bxslider/4.2.12/jquery.bxslider.css">*@
    <style>
        
        .p0 {
            padding: 0;
        }
        
        .resCarousel-inner .item {
            /*border: 4px solid #eee;*/
            /*vertical-align: top;*/
            text-align: left;
        }
        
        .resCarousel-inner .item .tile div,
        .banner .item div {
            display: table;
            width: 80%;
            min-height: 250px;
            text-align: left;
            /*box-shadow: 0 1px 1px rgba(0, 0, 0, .1);*/
        }
        
        .resCarousel-inner .item h1 {
            display: table-cell;
            vertical-align: middle;
            color: white;
        }
        
        /*.banner .item div {
            background: url('demoImg.jpg') center top no-repeat;
            background-size: cover;
            min-height: 550px;
        }*/
        
        /*.item .tile div {
            background: url('demoImg.jpg') center center no-repeat;
            background-size: cover;
            height: 200px;
            color: white;
        }*/
        
        .item div h1 {
            background: rgba(0, 0, 0, .4);
        }
    </style>
    
End Section
<div class="modal fade" id="modal-emp" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-body">
                
                <div class="pool-box">
                    <div class="row">
                        <div class="col-lg-4 col-md-4">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div class="publish">
                                        <img id="img-modal" class="misdatos-foto-modal">
                                        <div id="calif-modal" class="calification">
                                            <i class="fas fa-star"></i>
                                            <i class="fas fa-star"></i>
                                            <i class="fas fa-star"></i>
                                            <i class="fas fa-star"></i>
                                            <i class="fas fa-star"></i>
                                        </div>
                                        <div id="fcrea-modal"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12 col-md-12 demo-icon-font">
                                    <img id="flag-modal" class="flag" src="img/blank.gif" alt=""> 
                                    <br />
                                    <a id="mail-modal" href="mailto:"><i class="fa fa-envelope"></i></a>
                                    @*<a id="celular-modal" href="javascript:;" title=""><i class="fa fa-phone"></i></a>*@
                                    <a id="website-modal" href="" target="_blank" title="Sitio Web"><i class="fa fa-globe"></i></a>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-8 col-md-8">
                            <div class="title-pool-emp" id="empnom-modal"></div>
                            <h5 class="category">
                                <span id="catego-modal"></span>
                            </h5>
                            <p class="description" id="desc-modal"></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>


    <div class="modal fade" id="modaldetalle-1"  tabindex="-1" role="dialog" aria-labelledby="modaldetalle-1"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="title">@PageData("modaldetalle-1_1") @Html.Raw(Now.Year)</h3>
                </div>
                <div class="modal-body">
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <table id="dt-est1" class="table table-condensed table-bordered" style="width:100%">
                                    <thead>
                                        <tr>
                                            <th>@PageData("modaldetalle-1_th1")</th>
                                            <th>@PageData("modaldetalle-1_th2")</th>
                                            <th>@PageData("modaldetalle-1_th3")</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @for Each dr As System.Data.DataRow In dtdetalle1.Rows
                                            total1 += dr("monto")
                                            @<tr>
                                                <td>@Html.Raw(dr("proyecto"))</td>
                                                <td>@Html.Raw(Format(dr("fecha"), "dd/MMM/yyyy").Replace(".", ""))</td>
                                                <td>@Html.Raw(Format(dr("monto"), PageData("moneda")))</td>
                                            </tr>
                                        Next
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <td>@PageData("modaldetalle-1_ft1")</td>
                                            <td>
                                            </td>
                                            <td>@Html.Raw(Format(total1, PageData("moneda")))</td>
                                        </tr>
                                    </tfoot>

                                </table>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <button type="button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modaldetalle-2"  tabindex="-1" role="dialog" aria-labelledby="modaldetalle-2"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="title">@PageData("modaldetalle-2_1") @Html.Raw(Now.Year)</h3>
                </div>
                <div class="modal-body">
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <table id="dt-est2" class="table table-condensed table-bordered" style="width:100%">
                                    <thead>
                                        <tr>
                                            <th>@PageData("modaldetalle-2_th1")</th>
                                            <th>@PageData("modaldetalle-2_th2")</th>
                                            <th>@PageData("modaldetalle-2_th3")</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @for Each dr As System.Data.DataRow In dtdetalle2.Rows
                                            total2 += dr("monto")
                                            @<tr>
                                                <td>@Html.Raw(dr("proyecto"))</td>
                                                <td>@Html.Raw(Format(dr("fecha"), "dd/MMM/yyyy").Replace(".", ""))</td>
                                                <td>@Html.Raw(Format(dr("monto"), PageData("moneda")))</td>
                                            </tr>
                                        Next
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <td>@PageData("modaldetalle-2_ft1")</td>
                                            <td>
                                            </td>
                                            <td>@Html.Raw(Format(total2, PageData("moneda")))</td>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <button type="button" class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

<div class="modal fade" id="modalcancel"  tabindex="-1" role="dialog" aria-labelledby="modalcancel"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modalcancel-1")</h3>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <span>@PageData("modalcancel-2")<br />@PageData("modalcancel-3")</span>
                        </div>
                    </div>
                    <div Class="row">
                        <div class="col-lg-12">
                            <button type = "button" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modaldeleteproy"  tabindex="-1" role="dialog" aria-labelledby="modaldeleteproy"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modaleliminar-1")</h3>
                <form id="formNOfinemp" method="post" action="">
                    <div class="modal-body">
                        <input type="hidden" name="DELPROY" value="OK"/>
                        <input type="hidden" id="idproydel" name="idproyecto"  />
                    </div>
                    <div class="modal-footer">
                        <button type="button" Class="btn btn-def" data-dismiss="modal">@PageData("btncerrar")</button>
                        <button type="submit" Class="btn btn-pri">@PageData("btnenviar")</button>      
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="modalcontroversia"  tabindex="-1" role="dialog" aria-labelledby="modalcontroversia"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">@PageData("modalcontroversia-1")</h4>
            </div>
            <form id="formNOfinemp" method="post" action="">
                <div class="modal-body">
                    <input type="hidden" name="FINNOPROY" value="OK"/>
                    <input type="hidden" id="idproycontro" name="idproyecto"  />
                    <input type="hidden" name="idreceptor" value="0" />
                    <div class="fields_wrap">
                        <div class="form-group row">
                            <div class="col-lg-12">
                                <label class="control-label">Proyecto</label>
                                <input id="proy-cv" class="form-control form-control-danger" readonly />
                            </div>
                        </div>
                        <div Class="form-group row">
                            <div Class="col-lg-12">
                                <label>Un proyecto es puesto en controversia cuando quien contrata no está satisfecho con el servicio recibido y solicita algún tipo de intervención por parte de Conecta. </label>
                                <label>@PageData("modalcontroversia-2")</label>
                                <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                <textarea type="text" Class="form-control" id="mensaje" name="mensaje" @Validation.For("mensaje") placeholder="@PageData("modalcontroversia-3")" rows="4"></textarea>
                            </div>
                        </div>
                    </div>                
                </div>
                <div class="modal-footer">
                    <div class="form-group">
                        <button type="button" class="btn btn-def" data-dismiss="modal">@PageData("btncerrar")</button>
                        <button type="submit" class="btn btn-pri" >@PageData("btnenviar")</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>


<div id="content-dash">
    @*<h3 class="user_title line">@PageData("titulo")</h3>*@
    <div class="row">
        <div class="col-md-12 col-lg-12 col-xl-12">
            <h3> @PageData("sub-titulo_1-4") </h3>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="resCarousel centrar" data-items="1-3-3-3" data-slide="3">
                        <div class="resCarousel-inner">
                            @for Each drow As System.Data.DataRow In dtProy2.rows
                                Dim dtCatego As System.Data.DataTable = objProy.Carga_CategoProyecto(drow("id"), strConn)
                                Dim car_cat As String = ""
                                If Not IsNothing(dtCatego) Then
                                    For Each dCat As System.Data.DataRow In dtCatego.Rows
                                        car_cat &= ", " & dCat("subcategoria")
                                    Next
                                    car_cat = Mid(car_cat, 3)
                                Else
                                    car_cat = PageData("slider1-tit_1")
                                End If

                                @<div class="item" >
                                    <div class="tile col-md-11 col-xs-11 col-lg-11 col-xl-11">
                                        <h4 class="heading text-gradient"><a href="user-pool#@drow("id")">@drow("nombre")  @*<small><i class="fa fa-flash"></i></small>*@</a></h4>
                                        <h5 class="category hidden-sm hidden-xs" style="height:50px; overflow-y:auto" >
                                            @Html.Raw(car_cat)
                                        </h5>
                                        <p class="description" >@Html.Raw(Mid(drow("descripcion"), 1, 50).ToString.Replace(vbCrLf, "<br />"))</p>
                                        <p class="especifications">
                                            @PageData("slider1-tit_2") <span>@Html.Raw(Format(drow("pago"), PageData("moneda")) & drow("moneda"))</span>
                                            | @PageData("slider1-tit_3") <span>
                                                @Select Case drow("tipopago")
                                                    Case "H" :@<span> @PageData("slider1-tit_3-1")</span>
                                                    Case "M" : @<span> @PageData("slider1-tit_3-2")</span>
                                                    Case "P" : @<span> @PageData("slider1-tit_3-3")</span>
                                                    Case "I" : @<span> @PageData("slider1-tit_3-4")</span>
                                                    Case "R" : @<span> @PageData("slider1-tit_3-5")</span>
                                                End Select
                                            </span> 
                                            @If Val(drow("duracion")) <> 0 Then
                                                @Html.Raw(String.Format("| {0} <span>{1}</span>", PageData("slider1-tit_4"), drow("duracion")))
                                            End If
                                        </p>
                                    </div>
                                </div>
                            Next
                        </div>
                        <button class='btn btn-default leftRs' style="text-align:center"><i class="fa fa-arrow-left"></i></button>
                        <button class='btn btn-default rightRs' style="text-align:center"><i class="fa fa-arrow-right"></i></button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row m-t-10">
        <div class="col-md-12 col-lg-12 col-xl-12">
                <h3> Control de mis proyectos </h3>
                <div class="form-group">
                    <ul class="nav nav-pills">
                        <li id="empr" class="active"><a data-toggle="pill" href="#emprendedor">@PageData("sub-titulo_1")</a></li>
                        <li id="empl"><a data-toggle="pill" href="#empleador">@PageData("sub-titulo_2")</a></li>
                    </ul>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="tab-content">
                            <div id="emprendedor" class="tab-pane fade in active">
                                <div class="proyect-box">
                                    <table id="dt-emprendedor" class="table table-responsive"  cellspacing="0" width="100%">
                                        <thead>
                                            <tr>
                                                <th>@PageData("th-emp1")</th>
                                                <th>@PageData("th-emp8")</th>
                                                <th>@PageData("th-emp2")</th>
                                                <th>@PageData("th-emp3")</th>
                                                <th>@PageData("th-emp4")</th>
                                                <th>@PageData("th-emp5")</th>
                                                <th>@PageData("th-emp6")</th>
                                                <th>@PageData("th-emp7")</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            @for Each dr As System.Data.DataRow In dtEmprendedor.Rows
                                                Dim estatus As String = ""
                                                Select Case dr("estatus")
                                                    Case "A" 'Activo (Publicado)
                                                        estatus = "<span class=""in-progress"">" & PageData("estatus-a") & "</span>"
                                                    Case "P" 'En Prodceso
                                                        estatus = "<span class=""in-progress"">" & PageData("estatus-p") & "</span>"
                                                    Case "CV" 'En Controversia
                                                        estatus = "<span class=""canceled"">" & PageData("estatus-cv") & "</span>"
                                                    Case "E" 'Eliminado
                                                        estatus = "<span class=""canceled"">" & PageData("estatus-e") & "</span>"
                                                    Case "FE" 'Finalizado finish
                                                        estatus = "<span class=""finish"">" & PageData("estatus-fe") & "</span>"
                                                    Case "FP" 'Finalizado Emprendedor finish
                                                        estatus = "<span class=""finish"">" & PageData("estatus-fp") & "</span>"
                                                End Select
                                                @<tr>
                                                    <td>
                                                        @Select Case dr("estatus")
                                                            Case "CV" ', "FE"
                                                                @Html.Raw(dr("nombre"))
                                                            Case Else
                                                                @<a style="color:#0c264a !important" href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("th-emp1")/@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
                                                        End Select
                                                    </td>
                                                    <td title="@PageData("th-emp8")" nowrap>
                                                        @Select Case dr("estatus")
                                                            Case "CV"
                                                            Case "FE"
                                                                @<a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("btn-titulo1")"><i style="color:#7f8889;" class="fas fa-folder"></i> Seguimiento</a>
                                                            Case Else
                                                                @<p><a href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i> Seguimiento</a></p>
                                                                @<a style="color: #0c264a !important;" href="#" data-id="@dr("idproyecto")" title="@PageData("btn-titulo2")" data-toggle="modal" data-target="#modalcontroversia" ><i style="color:#66b8b7;" Class="fas fa-exclamation-triangle"></i> Controversia</a>
                                                                @*<a href="javascript:;"><i class="fas fa-trash"></i></a>*@
                                                        end select
                                                    </td>
                                                    <td title="@PageData("th-emp2")">@Html.Raw(dr("empleador"))</td>
                                                    <td title="@PageData("th-emp3")">@Html.Raw(Format(CDate(dr("fecha_inicio")).AddDays(dr("dias")), "yyyy-MM-dd"))</td>
                                                    <td title="@PageData("th-emp4")">@Html.Raw(estatus) </td>
                                                    <td title="@PageData("th-emp5")">@Html.Raw(Format(dr("monto"), PageData("moneda")) & " " & dr("moneda")) </td>
                                                    <td title="@PageData("th-emp6")">@Html.Raw(Format(dr("liberado"), PageData("moneda")) & " " & dr("moneda")) </td>
                                                    <td title="@PageData("th-emp7")">@Html.Raw(Format(dr("pagos"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                </tr>
                                            Next
                                        </tbody>
                                    </table>
                                    @*<a href = "#" class="pull-right">Ver historial</a>*@
                                </div>
                                <div class="proyect-box m-t-15">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <h3>@PageData("sub-titulo_1-1")</h3>
                                            <div class="dash-box">
                                                <ul>
                                                    <li class="stats"><i class="fas fa-star"></i> @PageData("sub-titulo_1-op1") <span id="proyectos" class="pull-right">@Html.Raw(drMis("exito"))</span></li>
                                                    <li class="stats"><i class="fas fa-bell"></i> @PageData("sub-titulo_1-op2") <span id="proyectos" class="pull-right">@Html.Raw(drMis("curso"))</span></li>
                                                    <li class="stats"><i class="fas fa-bolt"></i> @PageData("sub-titulo_1-op3") <span id="proyectos" class="pull-right">@Html.Raw(drMis("aplic"))</span></li>
                                                    <li class="stats"><i class="fas fa-eye"></i> @PageData("sub-titulo_1-op4") <span id="proyectos" class="pull-right">@Html.Raw(drMis("miperf"))</span></li>
                                                    <li class="stats"><i class="fas fa-gem"></i> @PageData("sub-titulo_1-op5") <span id="proyectos" class="pull-right">@Html.Raw(drMis("punt"))</span></li>
                                                    <li class="stats"><i class="fas fa-money-bill-alt"></i> @PageData("sub-titulo_1-op6") <span id="proyectos" class="pull-right">@Html.Raw(drMis("ingresos"))</span></li>
                                                </ul>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <h3>@PageData("sub-titulo_1-2")</h3>
                                            <div class="">
                                                @*<div id="curve_chart"></div>*@
                                                <canvas id="lineChart"></canvas>
                                                <a href="javascript:;" data-toggle="modal" data-target="#modaldetalle-1" class="pull-right"><small>@PageData("verdetalle")</small></a>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-5">
                                            <h3>@PageData("sub-titulo_1-3")</h3>
                                            <div class="dash-box project">
                                                <ul>
                                                    @for Each dr As System.Data.DataRow In dtProy.Rows
                                                        Dim estatus As String = "-"
                                                        Dim tagest As String = ""
                                                        Select Case dr("estatus")
                                                            Case "P"
                                                                estatus = PageData("estatus-5")
                                                                tagest = "in-progress"
                                                            Case "CV"
                                                                estatus = PageData("estatus-2")
                                                                tagest = "canceled"
                                                            Case "FP"
                                                                estatus = PageData("estatus-6")
                                                                tagest = "finish"
                                                            Case "FE"
                                                                estatus = PageData("estatus-4")
                                                                tagest = "finish"
                                                        End Select
                                                        @<li>@Html.Raw(dr("nombre")) - <span class="project-status @tagest"><strong>@estatus</strong></span></li>
                                                    Next
                                                    @*<li>Proyecto UNO <span class="pull-right project-status in-progress">En curso</span></li>
                                                        <li>Proyecto DOS <span class="pull-right project-status finish">Finalizado</span></li>
                                                        <li>Proyecto TRES <span class="pull-right project-status canceled">Cancelado</span></li>
                                                        <li>Proyecto CUATRO <span class="pull-right project-status in-progress">En curso</span></li>
                                                        <li>Proyecto CINCO <span class="pull-right project-status finish">Finalizado</span></li>
                                                        <li>Proyecto SEIS <span class="pull-right project-status canceled">Cancelado</span></li>
                                                        <li>Proyecto SIETE <span class="pull-right project-status in-progress">En curso</span></li>
                                                        <li>Proyecto OCHO <span class="pull-right project-status finish">Finalizado</span></li>
                                                        <li>Proyecto NUEVE <span class="pull-right project-status canceled">Cancelado</span></li>
                                                        <li>Proyecto DIEZ <span class="pull-right project-status in-progress">En curso</span></li>*@
                                                </ul>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div id="empleador" class="tab-pane fade">
                                <div class="proyect-box">
                                    <table id="dt-empleador" class="table table-responsive"  cellspacing="0" width="100%">
                                        <thead>
                                           <tr  class="hidden-sm hidden-xs">
                                                <th>@PageData("th-empl1")</th>
                                                <th>@PageData("th-empl10")</th>
                                                <th>@PageData("th-empl2")</th>
                                                <th>@PageData("th-empl3")</th>
                                                <th>@PageData("th-empl4")</th>
                                                <th>@PageData("th-empl5")</th>
                                                <th>@PageData("th-empl6")</th>
                                                <th>@PageData("th-empl7")</th>
                                                <th>@PageData("th-empl8")</th>
                                                <th>@PageData("th-empl9")</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            @for Each dr As System.Data.DataRow In dtEmpleador.Rows
                                                Dim estatus As String = ""
                                                Dim emp As String = "-"
                                                Dim monto As String = "-"
                                                Dim finicio As String = "-"
                                                If Not IsDBNull(dr("emprendedor")) Then
                                                    emp = dr("emprendedor")
                                                    monto = Format(dr("monto"), PageData("moneda")) & " " & dr("moneda")
                                                    finicio = Format(CDate(dr("fecha_inicio")), "dd/MM/yyyy")
                                                End If
                                                Select Case dr("estatus")
                                                    Case "A" 'Activo (Publicado)
                                                        estatus = "<span class=""in-progress"">" & PageData("estatus-a") & "</span>"
                                                    Case "P" 'En Prodceso
                                                        estatus = "<span class=""in-progress"">" & PageData("estatus-p") & "</span>"
                                                    Case "CV" 'En Controversia
                                                        estatus = "<span class=""canceled"">" & PageData("estatus-cv") & "</span>"
                                                    Case "E" 'Eliminado
                                                        estatus = "<span class=""canceled"">" & PageData("estatus-e") & "</span>"
                                                    Case "FE" 'Finalizado finish
                                                        estatus = "<span class=""finish"">" & PageData("estatus-fe") & "</span>"
                                                    Case "FP" 'Finalizado Emprendedor finish
                                                        estatus = "<span class=""finish"">" & PageData("estatus-fp") & "</span>"
                                                End Select
                                                Dim style As String = ""
                                                If idproyecto = dr("idproyecto") Then
                                                    style = "border: 1px solid black; background-color:lightblue; font-style:italic; "
                                                End If
                                                @<tr style="@style">
                                                    <td title="@PageData("th-empl1")">
                                    
                                                        @if emp = "-" Then
                                                            If dr("estatus") <> "E" Then
                                                                If dr("estatus") = "A" And dr("NUM_PROP") <> 0 Then
                                                                    @<a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
                                                                Else
                                                                    @Html.Raw(dr("nombre"))
                                                                End If
                                                            Else
                                                                @Html.Raw(dr("nombre"))
                                                            End If
                                                        Else
                                                            Select Case dr("estatus")
                                                                Case "CV" ', "FE"
                                                                    @Html.Raw(dr("nombre"))
                                                                Case Else
                                                                    @<a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
                                                            End Select
                                                        End If

                                                    </td>
                                                    <td title="@PageData("th-empl10")" nowrap style="text-align:left">
                                                        @if emp = "-" Then
                                                            @<p><a style="color: #0c264a !important;" href="publicar-proyectos?idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="Modificar información del proyecto"><i style="color:#46808c;" class="fas fa-edit"></i> Modificar proyecto</a></p>

                                                            If dr("estatus") <> "E" Then
                                                                @<p><a style="color: #0c264a !important;" href="javascript:;" title="@PageData("btn-titulo3")" data-id="@dr("idproyecto")"  data-toggle="modal" data-target="#modaldeleteproy"><i style="color:#b98a00" class="fas fa-trash"></i> Eliminar</a></p>
                                                                If dr("estatus") = "A" And dr("NUM_PROP") <> 0 Then
                                                                    @<p><a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i style="color:#7f8889;" class="fas fa-folder"></i> Seguimiento</a></p>
                                                                End If
                                                            End If
                                                        Else
                                                            Select Case dr("estatus")
                                                                Case "CV"
                                                                Case "FE"
                                                                    @<p><a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i style="color:#7f8889;" class="fas fa-folder"></i> Seguimiento</a></p>
                                                                Case Else
                                                                    @<p><a style="color: #0c264a !important;" href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i style="color:#7f8889;" class="fas fa-folder"></i> Seguimiento</a></p>
                                                                    @<p><a style="color: #0c264a !important;" href="#" data-id="@dr("idproyecto")"  title="@PageData("btn-titulo2")" data-toggle="modal" data-target="#modalcontroversia"><i style="color:#46808c;" class="fas fa-exclamation-triangle"></i> Controversia</a></p>
                                                            End Select
                                                        End If
                                                    </td>

                                                    <td title="@PageData("th-empl2")">@Html.Raw(Format(CDate(dr("fecha_creacion")), "yyyy-MM-dd"))</td>
                                                    <td title="@PageData("th-empl3")">@Html.Raw(Format(dr("pago"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                    <td title="@PageData("th-empl4")">@Html.Raw(IIf(dr("NUM_PROP") = 0, "-", "<a href='user-proyecto-seguimiento?tipo=empleador&idproyecto=" & objComun.Encrypt(dr("idproyecto")) & "'>" & CStr(dr("NUM_PROP")) & "</a>"))</td>
                                                    <td title="@PageData("th-empl5")">@Html.Raw(emp)</td>
                                                    <td title="@PageData("th-empl6")" style="text-wrap:none;">@Html.Raw(monto)</td>
                                                    <td title="@PageData("th-empl7")">@Html.Raw(finicio)</td>
                                                    <td title="@PageData("th-empl8")">@Html.Raw(estatus)</td>
                                                    <td title="@PageData("th-empl9")" style="text-wrap:none;">@Html.Raw(Format(dr("pagos"), PageData("moneda")) & " " & dr("moneda"))</td>

                                                </tr>
                                            Next
                                        </tbody>
                                    </table>
                                </div>
                                <div class="proyect-box m-t-5">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <h3> @PageData("sub-titulo_2-1")</h3>
                                            <div class="dash-box">
                                                <ul>
                                                    <li class="stats"><i class="fas fa-star"></i> @PageData("sub-titulo_2-op1") <span id="proyectos" class="pull-right">@drMisProy("exito")</span></li>
                                                    <li class="stats"><i class="fas fa-bell"></i> @PageData("sub-titulo_2-op2") <span id="proyectos" class="pull-right">@drMisProy("curso")</span></li>
                                                    <li class="stats"><i class="fas fa-gem"></i> @PageData("sub-titulo_2-op3") <span id="proyectos" class="pull-right">@drMisProy("punt")</span></li>
                                                    <li class="stats"><i class="fas fa-money-bill-alt"></i> @PageData("sub-titulo_2-op4") <span id="proyectos" class="pull-right">@drMisProy("ingresos")</span></li>
                                                </ul>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <h3> @PageData("sub-titulo_2-2")</h3>
                                            @*<div id="curve_chart2"></div>*@
                                            <canvas id="lineChart2"></canvas>
                                            <a href="javascript:;" data-toggle="modal" data-target="#modaldetalle-2" class="pull-right"><small>@PageData("verdetalle")</small></a>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <h3> @PageData("sub-titulo_2-3")</h3>
                                            <div class="dash-box project">
                                                <ul>
                                                    @for Each drow As System.Data.DataRow In dtMisProy.Rows
                                                        Dim estatus As String = "-"
                                                        Dim tagest As String = ""
                                                        Select Case drow("estatus")
                                                            Case "A"
                                                                estatus = PageData("estatus-1")
                                                                tagest = "in-progress"
                                                            Case "CV"
                                                                estatus = PageData("estatus-2")
                                                                tagest = "canceled"
                                                            Case "E"
                                                                estatus = PageData("estatus-3")
                                                                tagest = "canceled"
                                                            Case "FE"
                                                                estatus = PageData("estatus-4")
                                                                tagest = "finish"
                                                            Case "P"
                                                                estatus = PageData("estatus-5")
                                                                tagest = "in-progress"
                                                            Case "FP"
                                                                estatus = PageData("estatus-6")
                                                                tagest = "finish"
                                                        End Select
                                                        @<li> @drow("nombre") - <span class="project-status @tagest"><strong>@estatus</strong></span></li>
                                                    Next
                                                </ul>
                                                <!-- <ol>
                                                    <li> Proyecto UNO <span class="project-status in-progress">En curso</span></li>
                                                    <li> Proyecto DOS <span class="project-status finish">Finalizado</span></li>
                                                    <li> Proyecto TRES <span class="project-status canceled">Cancelado</span></li>
                                                    <li> Proyecto CUATRO <span class="project-status in-progress">En curso</span></li>
                                                    <li> Proyecto CINCO <span class="project-status finish">Finalizado</span></li>
                                                    <li> Proyecto SEIS <span class="project-status canceled">Cancelado</span></li>
                                                    <li> Proyecto SIETE <span class="project-status in-progress">En curso</span></li>
                                                    <li> Proyecto OCHO <span class="project-status finish">Finalizado</span></li>
                                                    <li> Proyecto NUEVE <span class="project-status canceled">Cancelado</span></li>
                                                    <li> Proyecto DIEZ <span class="project-status in-progress">En curso</span></li>
                                                </ol> -->
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <h3> @PageData("sub-titulo_2-4") </h3>
                                            <div class="dash-box project">
                                                
                                                    @for Each dr As System.Data.DataRow In dtEmpIgual.Rows
                                                        '<li style="margin-bottom:15px" id="dashemp-@dr("idemprendedor")">
                                                             @<span style="font-size:14px;">
                                                                <a style="color: #0c264a" href='javascript:;' onclick='godata(@dr("idemprendedor"))' data-toggle='modal' data-target='#modal-emp'>@dr("nombres")</a>
                                                                 <b style="font-size:12px;">@dr("categoria")</b><br />
                                                             </span>
                                                        '</li>
                                                    Next
                                                
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>         

        </div>
    </div>
    <div class="row m-t-10">
        <div class="col-md-12 col-lg-12 col-xl-12">
            <div class="dash-box quote">
                <h3 class="yoda">@PageData("slogan")<br><small>@PageData("slogan-autor")</small></h3>
            </div>
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

        @*<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
        <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>*@


	 <!-- ================== BEGIN BASE JS ================== -->
	@*<script src="assets/plugins/jquery/jquery-3.2.1.min.js"></script>
	<script src="assets/plugins/jquery-ui/jquery-ui.min.js"></script>
	<script src="assets/plugins/cookie/js/js.cookie.js"></script>
	<script src="assets/plugins/tooltip/popper/popper.min.js"></script>
	<script src="assets/plugins/bootstrap/bootstrap4/js/bootstrap.min.js"></script>
	<script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>*@
	<!-- ================== END BASE JS ================== -->

    <!-- ================== BEGIN PAGE LEVEL JS ================== -->
    <script src="assets/plugins/table/DataTables-1.10.18/datatables.js"></script>
	<script src="assets/plugins/table/DataTables/JSZip-3.1.3/jszip.min.js"></script>
	<script src="assets/plugins/table/DataTables/pdfmake-0.1.27/build/pdfmake.min.js"></script>
	<script src="assets/plugins/table/DataTables/pdfmake-0.1.27/build/vfs_fonts.js"></script>
	<script src="assets/plugins/table/DataTables/DataTables-1.10.16/js/jquery.dataTables.min.js"></script>
	<script src="assets/plugins/table/DataTables/DataTables-1.10.16/js/dataTables.bootstrap4.min.js"></script>
	<script src="assets/plugins/table/DataTables/AutoFill-2.2.0/js/dataTables.autoFill.min.js"></script>
	<script src="assets/plugins/table/DataTables/AutoFill-2.2.0/js/autoFill.bootstrap.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/dataTables.buttons.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/buttons.bootstrap.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/buttons.colVis.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/buttons.flash.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/buttons.html5.min.js"></script>
	<script src="assets/plugins/table/DataTables/Buttons-1.3.1/js/buttons.print.min.js"></script>
	<script src="assets/plugins/table/DataTables/ColReorder-1.3.3/js/dataTables.colReorder.min.js"></script>
	<script src="assets/plugins/table/DataTables/FixedColumns-3.2.2/js/dataTables.fixedColumns.min.js"></script>
	<script src="assets/plugins/table/DataTables/FixedHeader-3.1.2/js/dataTables.fixedHeader.min.js"></script>
	<script src="assets/plugins/table/DataTables/KeyTable-2.2.1/js/dataTables.keyTable.min.js"></script>
	<script src="assets/plugins/table/DataTables/Responsive-2.1.1/js/dataTables.responsive.min.js"></script>
	<script src="assets/plugins/table/DataTables/Responsive-2.1.1/js/responsive.bootstrap.min.js"></script>
	<script src="assets/plugins/table/DataTables/RowGroup-1.0.0/js/dataTables.rowGroup.min.js"></script>
	<script src="assets/plugins/table/DataTables/RowReorder-1.2.0/js/dataTables.rowReorder.min.js"></script>
	<script src="assets/plugins/table/DataTables/Scroller-1.4.2/js/dataTables.scroller.min.js"></script>
	<script src="assets/plugins/table/DataTables/Select-1.2.2/js/dataTables.select.min.js"></script>


	<!-- ================== BEGIN PAGE LEVEL JS ================== -->
	<script src="assets/plugins/chart/chart-js/Chart.min.js"></script>
	<script src="assets/js/page/chart.demo.js"></script>
	<script src="assets/js/apps.js"></script>
	<!-- ================== END PAGE LEVEL JS ================== -->

    <script src="https://cdn.jsdelivr.net/bxslider/4.2.12/jquery.bxslider.min.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            App.init();
            $('#modalcontroversia').on('show.bs.modal', function (e) {
                var idproy = e.relatedTarget.dataset.id;
                document.getElementById("idproycontro").value = idproy;
                $.ajax({
                    type: 'POST',
                    url: 'wsdashboard.asmx/Datos_Proy',
                    data: "{idproyecto: " + idproy + "}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: function (response) {
                        $("#proy-cv").val(response.d);
                    },
                    failure: function (response) {
                        alert("1 - " + response.responseText);
                    },
                    error: function (response) {
                        alert("2 - " + response.responseText);
                    }
                });
            });
            $('#modaldeleteproy').on('show.bs.modal', function (e) {
                var idproy = e.relatedTarget.dataset.id;
                document.getElementById("idproydel").value = idproy;
            });
        });


        var idproyecto = "@idproyecto";

        if (idproyecto != "0") {
            $("#empr").removeClass("active");
            $("#empl").toggleClass("active");

            $("#emprendedor").removeClass("tab-pane fade in active");
            $("#empleador").removeClass("tab-pane fade");

            $("#emprendedor").toggleClass("tab-pane fade");
            $("#empleador").toggleClass("tab-pane fade in active");

        }
        $('#dt-emprendedor').DataTable({
            "lengthChange": true,
            "order": [[3, "desc"]],
            "iDisplayLength": 10,
            "scrollCollapse": true,
            language: {
                search: "Buscar:",
                "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
                "infoEmpty": "Mostrando 0 a 0 de 0 registros",
                "lengthMenu": "Muestra _MENU_ registros",
                paginate: {
                    first: "Primero",
                    previous: "Anterior",
                    next: "Siguiente",
                    last: "Último"
                }
            }
        });
        $('#dt-empleador').dataTable({
            "lengthChange": true,
            "order": [[2, "desc"]],
            "iDisplayLength": 10,
            "scrollCollapse": true,
            language: {
                search: "Buscar:",
                "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
                "infoEmpty": "Mostrando 0 a 0 de 0 registros",
                "lengthMenu": "Muestra _MENU_ registros",
                paginate: {
                    first: "Primero",
                    previous: "Anterior",
                    next: "Siguiente",
                    last: "Último"
                }
            }
        });



        //App.init();
        //var rowReorderOption = ($(window).width() > 767) ? true : false;
        //var table = $('#dt-est1').DataTable({
        //    language: {
        //        search: "Buscar:",
        //        "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
        //        "infoEmpty": "Mostrando 0 a 0 de 0 registros",
        //        "lengthMenu": "Muestra _MENU_ registros",
        //        paginate: {
        //            first: "Primero",
        //            previous: "Anterior",
        //            next: "Siguiente",
        //            last: "Último"
        //        }
        //    }
        //});

        //var table = $('#dt-est2').DataTable({
        //    language: {
        //        search: "Buscar:",
        //        "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
        //        "infoEmpty": "Mostrando 0 a 0 de 0 registros",
        //        "lengthMenu": "Muestra _MENU_ registros",
        //        paginate: {
        //            first: "Primero",
        //            previous: "Anterior",
        //            next: "Siguiente",
        //            last: "Último"
        //        }
        //    }
        //});

        //var idempview = "";
        //if (idempview != "") {
            //alert(idempview);
            //$("#emprendedor").removeclass("tab-pane fade in active");
            //$("#empleador").removeclass("tab-pane fade");

            //$("#emprendedor").toggleclass("tab-pane fade");
            //$("#empleador").toggleclass("tab-pane fade in active");

            //$("#modal-emp").modal('show');
       // }

        function gosubmit(valor) {
            document.getElementById("frm-" + valor).submit();
        }

        function godata(idemp) {
            $.ajax({
                type: 'POST',
                url: 'wsdashboard.asmx/Emprendedores_m',
                data: "{idempfind: " + idemp + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: function (response) {
                    carga_empview(response);
                },
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }
            });
        }

        function carga_empview(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("empview");      
            var calif = 0;
            $(detalle).each(function () {
                calif = Number($(this).find("calif").text());
                $("#img-modal").attr("src", "poolin_arch/images/png/_avatar_" + $(this).find("idemp").text() + ".png");
                $("#fcrea-modal").html("<small>" + $(this).find("fecha_creacion").text().substring(0, 10) + "</small>");
                $("#flag-modal").removeClass("");
                $("#flag-modal").addClass("flag " + $(this).find("icno").text());
                $("#email-modal").attr("href", "mailto:" + $(this).find("email").text());
                //$("#celular-modal").attr("title", $(this).find("celular").text());
                $("#website-modal").attr("href", $(this).find("website").text());
                $("#empnom-modal").html($(this).find("Emprendedor").text());
                $("#desc-modal").html($(this).find("descripcion").text());
                $("#catego-modal").text($(this).find("catego").text());
                if (calif != 0) {
                    $("#calif-modal").html("");
                    var stars = "";
                    for (i = 1; i <= calif; i++) {
                        stars += "<i class='fas fa-star'></i>";
                    }
                    $("#calif-modal").html(stars);
                }

            });
            //$("#modal-emp").modal();
        }
        //ChartJs.init();

	    Chart.defaults.global.defaultFontFamily = '-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
	    Chart.defaults.global.defaultFontColor = '#222';
	    Chart.defaults.global.tooltips.xPadding = 8;
	    Chart.defaults.global.tooltips.yPadding = 8;
        Chart.defaults.global.tooltips.multiKeyBackground = 'transparent';
	
        var ctx = document.getElementById('lineChart');
        var lineChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [@Html.Raw(lblmes)],
                datasets: [{
                    color: PRIMARY_COLOR,
                    backgroundColor: PRIMARY_TRANSPARENT_2_COLOR,
                    borderColor: PRIMARY_COLOR,
                    borderWidth: 1.5,
                    pointBackgroundColor: WHITE_COLOR,
                    pointBorderWidth: 1.5,
                    pointRadius: 4,
                    pointHoverBackgroundColor: PRIMARY_COLOR,
                    pointHoverBorderColor: WHITE_COLOR,
                    pointHoverRadius: 7,
                    label: 'Ingresos @Now.Year',
                    data: [@Html.Raw(monmes(0))]
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });

        var ctx2 = document.getElementById('lineChart2');
        var lineChart = new Chart(ctx2, {
            type: 'line',
            data: {
                labels: [@Html.Raw(lblmes)],
                datasets: [{
                    //color: PRIMARY_COLOR,
                    color: "#aeaebe",
                    //backgroundColor: PRIMARY_TRANSPARENT_2_COLOR,
                    backgroundColor: "#aeaebe",
                    borderColor: PRIMARY_COLOR,
                    borderWidth: 1.5,
                    pointBackgroundColor: WHITE_COLOR,
                    pointBorderWidth: 1.5,
                    pointRadius: 4,
                    pointHoverBackgroundColor: PRIMARY_COLOR,
                    pointHoverBorderColor: WHITE_COLOR,
                    pointHoverRadius: 7,
                    label: 'Inversion @Now.Year',
                    data: [@Html.Raw(monmes(1))]
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });

        var msgsend = "@msgsave";
        switch (msgsend) {
            case "OK":
                $.bootstrapGrowl('@PageData("msg-ok")', {
                    type: 'success',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "CV":
                $.bootstrapGrowl('@PageData("msg-cv")', {
                    type: 'info',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });

                break;
            case "MSFP":
                $.bootstrapGrowl('@PageData("msg-msfp")', {
                    type: 'info',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });

                break;
            case "FINFE":
                $.bootstrapGrowl('@PageData("msg-finfe")', {
                    type: 'info',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "PYDEL":
                $.bootstrapGrowl('@PageData("msg-pydel")', {
                    type: 'info',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "ERR":
                $.bootstrapGrowl('@PageData("msg-err")', {
                    type: 'danger',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "YAEVAL":
                $.bootstrapGrowl('@PageData("msg-yaeval")', {
                    type: 'info',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
        }

        var cancelado = "@Request.QueryString("cancelado")";
        if (cancelado == "True") {
            alert("PAGO CANCELADO");
            //$("#modalcancel").modal("show");
        }

    </script>
   
    <script>
        //ResCarouselCustom();
        var pageRefresh = true;

        function ResCarouselCustom() {
            var items = $("#dItems").val(),
                slide = $("#dSlide").val(),
                speed = $("#dSpeed").val(),
                interval = $("#dInterval").val()

            var itemsD = "data-items=\"" + items + "\"",
                slideD = "data-slide=\"" + slide + "\"",
                speedD = "data-speed=\"" + speed + "\"",
                intervalD = "data-interval=\"" + interval + "\"";


            var atts = "";
            atts += items != "" ? itemsD + " " : "";
            atts += slide != "" ? slideD + " " : "";
            atts += speed != "" ? speedD + " " : "";
            atts += interval != "" ? intervalD + " " : ""

            //console.log(atts);

            var dat = "";
            dat += '<h4 >' + atts + '</h4>'
            dat += '<div class=\"resCarousel\" ' + atts + '>'
            dat += '<div class="resCarousel-inner">'
            for (var i = 1; i <= 14; i++) {
                dat += '<div class=\"item\"><div><h1>' + i + '</h1></div></div>'
            }
            dat += '</div>'
            dat += '<button class=\'btn btn-default leftRs\'><i class=\"fa fa-fw fa-angle-left\"></i></button>'
            dat += '<button class=\'btn btn-default rightRs\'><i class=\"fa fa-fw fa-angle-right\"></i></button>    </div>'
            console.log(dat);
            $("#customRes").html(null).append(dat);

            if (!pageRefresh) {
                ResCarouselSize();
            } else {
                pageRefresh = false;
            }
            //ResCarouselSlide();
        }

        $("#eventLoad").on('ResCarouselLoad', function() {
            //console.log("triggered");
            var dat = "";
            var lenghtI = $(this).find(".item").length;
            if (lenghtI <= 30) {
                for (var i = lenghtI; i <= lenghtI + 10; i++) {
                    dat += '<div class="item"><div class="tile"><div><h1>' + (i + 1) + '</h1></div><h3>Title</h3><p>content</p></div></div>'
                }
                $(this).append(dat);
            }
        });


    </script>

    <script src="js/resCarousel.js"></script>

End Section