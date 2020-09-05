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

            Dim objSend As New poolin_class.cSendGrid
            objSend.Correo_SMTP("PROYECTO EN CONTROVERSIA", htmlmsg, correos, "", strConn)

        End If

    End Sub
End Functions

@code
    PageData("pagina") = "user-proyectos"
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Proyectos"
    PageData("opproy") = "active"
    PageData("hiddemenu") = ""

    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Dim msgsave = ""
    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim objProy As New poolin_class.cProyectos

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
    Validation.RequireField("mensaje", "<br> Requerido")

    Dim dtEmprendedor As System.Data.DataTable = objProy.Proyecto_Emprendedor(idemp, strConn)
    Dim dtEmpleador As System.Data.DataTable = objProy.Proyecto_Empleador(idemp, strConn)
    Dim objComun As New poolin_class.cComunes

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
            Dim dtProy As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(idemp, strConn, idproyecto, "%%")
            If dtProy.Rows.Count <> 0 Then
                objProy.Evaluacion(idproyecto, idemp, dtProy.Rows(0)("idcliente"), dtEva, strConn)
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

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-proyectos", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
End Code
@section head

        <!-- ================== BEGIN BASE CSS STYLE ================== -->
    <link href="assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/plugins/icon/themify-icons/themify-icons.css" rel="stylesheet" />
    <link href="assets/css/animate.min.css" rel="stylesheet" />
    <!-- ================== END BASE CSS STYLE ================== -->
    <!-- ================== BEGIN PAGE CSS STYLE ================== -->
    <link href="assets/plugins/table/DataTables/DataTables-1.10.15/css/dataTables.bootstrap.min.css" rel="stylesheet" />
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
    <link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?5.7">
    <link href="assets/css/style.css?2.1" rel="stylesheet" />
    

    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>

End Section

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

<div class="modal fade" id="modaleliminar"  tabindex="-1" role="dialog" aria-labelledby="modaleliminar"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modaleliminar-1")</h3>
                <div class="modal-body">
                    <form id="formNOfinemp" method="post" action="">
                        <input type="hidden" name="DELPROY" value="OK"/>
                        <input type="hidden" id="idproydel" name="idproyecto"  />
                        <div Class="row">
                            <div class="col-lg-12">
                                <button type = "submit" Class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                <button type = "button" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>

                    </form>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="modal fade" id="modalcontroversia"  tabindex="-1" role="dialog" aria-labelledby="modalcontroversia"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modalcontroversia-1")</h3>
                <div class="modal-body">
                    <form id="formNOfinemp" method="post" action="">
                        <input type="hidden" name="FINNOPROY" value="OK"/>
                        <input type="hidden" id="idproycontro" name="idproyecto"  />
                        <input type="hidden" name="idreceptor" value="0" />
                        <div class="fields_wrap">
                            <div Class="row">
                                <div Class="col-lg-12">
                                    <label>@PageData("modalcontroversia-2")</label>
                                    <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                    <textarea type="text" Class="form-control" name="mensaje" @Validation.For("mensaje") placeholder="@PageData("modalcontroversia-3")" rows="4"></textarea>
                                </div>
                            </div>
                        </div>                
                        <div class="fields_wrap">
                            <div Class="row">
                                <div Class="col-lg-12">
                                    <button type = "submit" Class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                    <button type = "button" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                                </div>
                            </div>
                        </div>
                    </form>

                </div>
            </div>
        </div>
    </div>
</div>


<div id="content">
    <h3 class="user_title line">@PageData("titulo")</h3>
    <ul class="nav nav-pills">
        <li id="empr" class="active"><a data-toggle="pill" href="#emprendedor">@PageData("sub-titulo_1")</a></li>
        <li id="empl"><a data-toggle="pill" href="#empleador">@PageData("sub-titulo_2")</a></li>
    </ul>
    <div class="tab-content">
        <div id="emprendedor" class="tab-pane fade in active">
            <div class="proyect-box">
                <table id="dt-emprendedor" class="table table-responsive">
                    <thead>
                        <tr>
                            <th>@PageData("th-emp1")</th>
                            <th>@PageData("th-emp2")</th>
                            <th>@PageData("th-emp3")</th>
                            <th>@PageData("th-emp4")</th>
                            <th>@PageData("th-emp5")</th>
                            <th>@PageData("th-emp6")</th>
                            <th>@PageData("th-emp7")</th>
                            <th>@PageData("th-emp8")</th>
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
                                            @<a href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("th-emp1")/@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
                                    End Select
                                </td>
                                <td title="@PageData("th-emp2")">@Html.Raw(dr("empleador"))</td>
                                <td title="@PageData("th-emp3")">@Html.Raw(Format(CDate(dr("fecha_inicio")).AddDays(dr("dias")), "dd/MM/yyyy"))</td>
                                <td title="@PageData("th-emp4")">@Html.Raw(estatus) </td>
                                <td title="@PageData("th-emp5")">@Html.Raw(Format(dr("monto"), PageData("moneda")) & " " & dr("moneda")) </td>
                                <td title="@PageData("th-emp6")">@Html.Raw(Format(dr("liberado"), PageData("moneda")) & " " & dr("moneda")) </td>
                                <td title="@PageData("th-emp7")">@Html.Raw(Format(dr("pagos"), PageData("moneda")) & " " & dr("moneda"))</td>
                                <td title="@PageData("th-emp8")">
                                    @Select Case dr("estatus")
                                        Case "CV"
                                        Case "FE"
                                            @<a href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i></a>
                                        Case Else
                                            @<a href="user-proyecto-seguimiento?tipo=emprendedor&idproyecto=@objComun.Encrypt(dr("idproyecto"))"  title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i></a>
                                            @<a href="#" data-id="@dr("idproyecto")" title="@PageData("btn-titulo2")" data-toggle="modal" data-target="#modalcontroversia" ><i Class="fas fa-exclamation-triangle"></i></a>
                                            @*<a href="javascript:;"><i class="fas fa-trash"></i></a>*@
                                    end select
                                </td>
                            </tr>
                        Next
                    </tbody>
                </table>
                @*<a href = "#" class="pull-right">Ver historial</a>*@
            </div>
        </div>
        <div id="empleador" class="tab-pane fade">
            <div class="proyect-box">
                <table id="dt-empleador" class="table table-condensed table-bordered" style="width:100%">
                    <thead>
                       <tr  class="hidden-sm hidden-xs">
                            <th>@PageData("th-empl1")</th>
                            <th>@PageData("th-empl2")</th>
                            <th>@PageData("th-empl3")</th>
                            <th>@PageData("th-empl4")</th>
                            <th>@PageData("th-empl5")</th>
                            <th>@PageData("th-empl6")</th>
                            <th>@PageData("th-empl7")</th>
                            <th>@PageData("th-empl8")</th>
                            <th>@PageData("th-empl9")</th>
                            <th>@PageData("th-empl10")</th>
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
                                                @<a href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
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
                                                @<a href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")">@Html.Raw(dr("nombre"))</a>
                                        End Select
                                    End If

                                </td>
                                <td title="@PageData("th-empl2")">@Html.Raw(Format(CDate(dr("fecha_creacion")), "dd/MM/yyyy"))</td>
                                <td title="@PageData("th-empl3")">@Html.Raw(Format(dr("pago"), PageData("moneda")) & " " & dr("moneda"))</td>
                                <td title="@PageData("th-empl4")">@Html.Raw(IIf(dr("NUM_PROP") = 0, "-", CStr(dr("NUM_PROP"))))</td>
                                <td title="@PageData("th-empl5")">@Html.Raw(emp)</td>
                                <td title="@PageData("th-empl6")" style="text-wrap:none;">@Html.Raw(monto)</td>
                                <td title="@PageData("th-empl7")">@Html.Raw(finicio)</td>
                                <td title="@PageData("th-empl8")">@Html.Raw(estatus)</td>
                                <td title="@PageData("th-empl9")" style="text-wrap:none;">@Html.Raw(Format(dr("pagos"), PageData("moneda")) & " " & dr("moneda"))</td>
                                <td title="@PageData("th-empl10")">
                                    @if emp = "-" Then
                                        @<a href="publicar-proyectos?idproyecto=@objComun.Encrypt(dr("idproyecto"))"><i class="fas fa-edit"></i></a>
                                        If dr("estatus") <> "E" Then
                                            @<a href="#" title="@PageData("btn-titulo3")" data-id="@dr("idproyecto")"  data-toggle="modal" data-target="#modaleliminar"><i class="fas fa-trash"></i></a>
                                            If dr("estatus") = "A" And dr("NUM_PROP") <> 0 Then
                                                @<a href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i></a>
                                            End If
                                        End If
                                    Else
                                        Select Case dr("estatus")
                                            Case "CV"
                                            Case "FE"
                                                @<a href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i></a>
                                            Case Else
                                                @<a href="user-proyecto-seguimiento?tipo=empleador&idproyecto=@objComun.Encrypt(dr("idproyecto"))" title="@PageData("btn-titulo1")"><i Class="fas fa-folder"></i></a>
                                                @<a href="#" data-id="@dr("idproyecto")"  title="@PageData("btn-titulo2")" data-toggle="modal" data-target="#modalcontroversia"><i Class="fas fa-exclamation-triangle"></i></a>
                                        End Select
                                    End If
                                </td>

                            </tr>
                        Next
                    </tbody>
                </table>
                @*<a href = "#" class="pull-right">Ver historial</a>
                <i class="icon-filter"></i>*@
            </div>
        </div>
    </div>        	
</div>
@section Scripts
    <!-- ================== BEGIN BASE JS ================== -->
    <script src="assets/plugins/jquery/jquery-1.9.1.min.js"></script>
    <script src="assets/plugins/jquery/jquery-migrate-1.1.0.min.js"></script>
    <script src="assets/plugins/jquery-ui/ui/minified/jquery-ui.min.js"></script>
    <script src="assets/plugins/cookie/js/js.cookie.js"></script>
    <script src="assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>
    <script src="assets/js/apps.js"></script>

    <!-- ================== BEGIN PAGE LEVEL JS ================== -->
    <script src="assets/plugins/chart/chart-js/Chart.js"></script>
    <script src="assets/plugins/chart/chart-js/Chart.bundle.js"></script>
    <script src="assets/plugins/table/DataTables/JSZip-3.1.3/jszip.min.js"></script>
    <script src="assets/plugins/table/DataTables/pdfmake-0.1.27/build/pdfmake.min.js"></script>
    <script src="assets/plugins/table/DataTables/pdfmake-0.1.27/build/vfs_fonts.js"></script>
    <script src="assets/plugins/table/DataTables/DataTables-1.10.15/js/jquery.dataTables.min.js"></script>
    <script src="assets/plugins/table/DataTables/DataTables-1.10.15/js/dataTables.bootstrap.min.js"></script>
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
    <script src="https://codepen.io/anon/pen/aWapBE.js"></script>

    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script>
        $(document).ready(function () {
            App.init();
            handleRenderTableData();
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

        var handleRenderTableData = function () {
            var rowReorderOption = ($(window).width() > 767) ? true : false;
            var table = $('#dt-emprendedor').DataTable({
                "order": [[2, "desc"]],
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
            var tableemp = $('#dt-empleador').DataTable({
                "order": [[1, "desc"]],
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
        };

        var cancelado = "@Request.QueryString("cancelado")";
        if (cancelado == "True") {
            $("#modalcancel").modal("show");
        }

        $('#modalcontroversia').on('show.bs.modal', function (e) {
            var idproy = e.relatedTarget.dataset.id;
            document.getElementById("idproycontro").value = idproy;
        });

        $('#modaleliminar').on('show.bs.modal', function (e) {
            var idproy = e.relatedTarget.dataset.id;
            document.getElementById("idproydel").value = idproy;
        });

        var msgsend = "@msgsave";
        switch (msgsend)
        {
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
    </script>
End Section    
