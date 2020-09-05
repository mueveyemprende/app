@functions
    Private Sub Enviar_Correo(ByVal idproyecto As Long,
                                ByVal asunto As String,
                                ByVal strConn As String)
        Dim P As New poolin_class.cProyectos
        Dim dt As System.Data.DataTable = P.Proyecto_Propuesta(idproyecto, strConn)
        If dt.Rows.Count <> 0 Then
            Dim http As String = "https://mueveyemprende.io"
            Dim htmlmsg As String

            Using f As New StreamReader(Server.MapPath("poolin_arch/txt/acepta_proy.txt"))
                htmlmsg = f.ReadToEnd
                f.Close()
            End Using

            If dt.Rows(0).Item("empresa") <> "" Then
                htmlmsg = htmlmsg.Replace("[URL]", http).Replace("[WEB]", http).Replace("[CLIENTE]", dt.Rows(0).Item("empresa"))
            Else
                htmlmsg = htmlmsg.Replace("[URL]", http).Replace("[WEB]", http).Replace("[CLIENTE]", dt.Rows(0).Item("cliente"))
            End If
            htmlmsg = htmlmsg.Replace("[PROYECTO]", dt.Rows(0).Item("proyecto"))

            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString

            Dim objSend As New poolin_class.cSendGrid
            objSend.Correo_SMTP(asunto, htmlmsg, dt.Rows(0).Item("email"), "", strConn)

        End If
    End Sub

    Protected Sub Envia_NotiXMail(ByVal idreceptor As String,
      ByVal asunto As String, ByVal strConn As String)
        'Dim c As New poolin_class.cComunes
        'Dim _smtp As String = ""
        'Dim user As String = ""
        'Dim pwd As String = ""
        'Dim puerto As Long = 0
        'Dim ssl As Boolean = False
        'Dim http As String = ""

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

        'c.config_SMTP(_smtp, user, pwd, puerto, ssl, http, strConn)

        Dim objSend As New poolin_class.cSendGrid

        objSend.Correo_SMTP(asunto, htmlmsg, emailemp, "", strConn)

        'Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString
        'Dim _client As New SendGridClient(apkey)
        'Dim _from As New EmailAddress(user, "Equipo M&E")
        'Dim _subject As String = "Tienes notificaciones en Poolin"

        'Dim _msg = New SendGridMessage()
        '_msg.AddTo(emailemp)
        '_msg.From = _from
        '_msg.Subject = _subject
        '_msg.HtmlContent = htmlmsg
        'Dim _bbc As New List(Of EmailAddress)

        'Dim trymsg = _client.SendEmailAsync(_msg)

        'If trymsg.IsFaulted Then
        '    Err.Raise(trymsg.Exception.HResult, trymsg.Exception.Message)
        'End If

    End Sub

    'Private Sub Actualiza_EstatusProyecto(ByVal idproyecto As Long,
    '                      ByVal idemprendedor As Long,
    '                      ByVal estatus As String,
    '                      ByVal idpropuesta As Long,
    '                      ByVal strConn As String)
    '    Dim p As New poolin_class.cProyectos
    '    Dim m As New poolin_class.cComunes
    '    'FP = Finalizado por Persona
    '    'FE = Finalizado por Empresa (Acepto Finalizacion)
    '    'P = Se vuelve a poner en Proceso
    '    'CV = Controversia se envía mensaje al Administrador de POOLIN 
    '    If estatus = "FE" Then 'Final Empresa (Cliente)

    '    ElseIf estatus = "CV" Then 'Controversia
    '        p.Actualiza_Proyecto(Request.Cookies("idemprendedor").Value, idproyecto, estatus, strConn, 0, Request.Cookies("idemprendedor").Value)
    '        'Se debe de indicar quien puso el proyecto en controversia
    '        'Envia_Msg_AdminPoolin(idproyecto, Request.Form("asunto"), Request.Form("mensaje"))
    '        Dim cmm As New poolin_class.cComunes
    '        Response.Redirect("enejecucion.aspx?msg=msgcv&success=" & cmm.Encrypt(idproyecto), False)

    '    ElseIf estatus = "FINAL" Then
    '        'Dim dt As System.Data.DataTable = m.Preguntas("E", "ESP", strConn)
    '        'Dim dtEva As New System.Data.DataTable
    '        'dtEva.Columns.Add("idpregunta", GetType(Integer))
    '        'dtEva.Columns.Add("calif", GetType(Integer))
    '        'Dim drEva As System.Data.DataRow
    '        'For Each drow As System.Data.DataRow In dt.Rows
    '        '    drEva = dtEva.NewRow
    '        '    drEva("idpregunta") = drow("id")
    '        '    drEva("calif") = 0
    '        '    If Not IsNothing(Request.Form("starscte-" & drow("id"))) Then
    '        '        drEva("calif") = Request.Form("starscte-" & drow("id")) ' : Exit For
    '        '    End If
    '        '    dtEva.Rows.Add(drEva)
    '        'Next
    '        'p.Evaluacion(idproyecto, idemprendedor, dtEva, strConn)
    '        'Dim cmm As New poolin_class.cComunes
    '        'Response.Redirect("enejecucion.aspx?msg=msgfinal&success=" & cmm.Encrypt(idproyecto), False)

    '    Else

    '        'Dim cmm As New poolin_class.cComunes
    '        'Response.Redirect("enejecucion.aspx?msg=msgelse&success=" & cmm.Encrypt(idproyecto), False)

    '    End If
    'End Sub


    Private Sub Agenda_Cita(ByVal idemp As Long, ByVal strConn As String)

        Dim objProy As New poolin_class.cProyectos
        Dim dt As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(0, strConn, Request.Form("idproyecto"), "%%")

        If dt.Rows.Count = 0 Then
            Exit Sub
        End If

        Dim c As New poolin_class.cAgenda
        c.id = 0
        c.fechahora = Request.Form("fechahora")
        c.duracion = 60
        c.motivo = Mid("CITA / " & Request.Form("proyecto"), 1, 50)
        c.idemprendedor = idemp

        If dt.Rows(0)("idcliente") <> idemp Then
            c.idreceptor = dt.Rows(0)("idcliente")
        Else
            c.idreceptor = dt.Rows(0)("idemprendedor")
        End If

        c.idproyecto = Request.Form("idproyecto")
        c.descripcion = Request.Form("asunto")
        c.estatus = ""
        c.Agenda_Cita(strConn)
    End Sub

End Functions

@Code
    PageData("pagina") = "user-proyecto-seguimiento"
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Seguimiento al Proyecto"
    PageData("opproy") = "active"
    PageData("hiddemenu") = ""

    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim errapp = ""

    Validation.RequireField("asunto", "<br> Requerido")
    Validation.RequireField("fechahora", "<br> Requerido")
    Validation.RequireField("mensaje", "<br> Requerido")

    Validation.RequireField("descripcion", "<br> Requerido")

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim msgsave = ""
    'Agenda citas
    If Request.Form("save") = "OK" Then
        Try
            'Agenda_Cita(idemp, strConn)
            Envia_NotiXMail(Request.Form("idrece-cita"), Request.Form("proyecto") & vbCrLf & Request.Form("asunto"), strConn)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try
    End If

    Dim objComun As New poolin_class.cComunes
    Dim idProyecto As Long = 0
    If Request.QueryString("idproyecto") = "" Then
        If Request.Form("idproyecto") <> "" Then
            If IsNumeric(Request.Form("idproyecto")) Then
                idProyecto = Request.Form("idproyecto")
            Else
                idProyecto = objComun.Decrypt(Request.Form("idproyecto"))
            End If
        End If
    Else
        idProyecto = objComun.Decrypt(Request.QueryString("idproyecto"))
    End If

    Dim objProp As New poolin_class.cPropuestas
    Dim objProy As New poolin_class.cProyectos

    Dim pemp = 0 'Porcentaje de comision por, para Empleador.
    Dim iva = 0
    Dim moneda = ""
    objComun.config_comisiones(pemp, 0, iva, strConn)

    If Request.Form("liberado") = "OK" Then
        Dim objMem As New poolin_class.cMembresia
        Try
            objMem.Libera_Pago(Request.Form("idrecibo"), idemp, strConn)
            msgsave = "LIBOK"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try

    End If

    If Request.Form("confirma") = "OK" Then
        Try
            'objProy.AceptaPropuesta(idemp, Request.Form("idemprendedor"),
            '        idProyecto, Request.Form("idpropuesta"), strConn,
            '"PROPUESTA ACEPTADA", "PONTE EN CONTACTO CON TU CLIENTE. RECUERDA QUE PUEDES AGENDAR CITA Y ENVIAR MENSAJES MEDIANTE .")
            Enviar_Correo(idProyecto, "PROPUESTA ACEPTADA", strConn)
            msgsave = "CONFOK"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try

    End If

    'Envia mensaje por poolin
    If Request.Form("msg") = "OK" Then

        Dim msg As New poolin_class.cMensajes
        Dim asunto = Mid(Request.Form("mensaje"), 1, 15) & "..."
        Dim idrecmsg As Long
        Try
            If IsNothing(Request.Form("idpropuesta")) Then
                idrecmsg = Request.Form("idreceptor")
                'msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), idProyecto, asunto, Request.Form("mensaje"), strConn, 0)
            Else
                Dim dtsend As System.Data.DataTable = objProp.Carga_PropuestaPersona(0, 0, strConn, Request.Form("idpropuesta"))
                If dtsend.Rows.Count <> 0 Then
                    If idemp = dtsend.Rows(0)("idemprendedor") Then
                        idrecmsg = Request.Form("idreceptor")
                        '    msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), idProyecto, asunto, Request.Form("mensaje"), strConn, 0)
                    Else
                        idrecmsg = dtsend.Rows(0)("idemprendedor")
                        'msg.Envia_Mensaje(idemp, dtsend.Rows(0)("idemprendedor"), idProyecto, asunto, Request.Form("mensaje"), strConn, 0)
                    End If
                End If
            End If
            Envia_NotiXMail(idrecmsg, asunto, strConn)
            msgsave = "MS"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try
    End If

    Dim totProp = objProp.Total_Propuestas(idProyecto, strConn)
    Dim dtProy As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(0, strConn, idProyecto, "%%")
    Dim idreceptor As Long = 0
    If dtProy.Rows.Count = 0 Then
        Response.Redirect("user-dash")
    Else
        If idemp = dtProy.Rows(0)("idcliente") Then
            If Not IsDBNull(dtProy.Rows(0)("idemprendedor")) Then
                idreceptor = dtProy.Rows(0)("idemprendedor")
            End If
        Else
            idreceptor = dtProy.Rows(0)("idcliente")
        End If
    End If

    If Request.Form("FINNOPROY") = "OK" Then
        Try
            Dim msg As New poolin_class.cMensajes
            objProy.Actualiza_Proyecto(idemp, idProyecto, "P", strConn, 0, 0)
            msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), idProyecto, "PROYECTO NO FINALIZADO", Request.Form("mensaje"), strConn, 0)
            Envia_NotiXMail(Request.Form("idreceptor"), "PROYECTO NO FINALIZADO", strConn)
            msgsave = "MSNOFIN"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try

    End If

    If Request.Form("FINCTE") = "OK" Then
        Try
            Dim msg As New poolin_class.cMensajes
            objProy.Actualiza_Proyecto(idemp, idProyecto, "FP", strConn, 0, 0)
            msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), idProyecto, "PROYECTO FINALIZADO POR QUIEN OFRECE EL SERVICIO", Request.Form("mensaje"), strConn, 0)
            Envia_NotiXMail(Request.Form("idreceptor"), "PROYECTO FINALIZADO POR QUIEN OFRECE EL SERVICIO", strConn)
            msgsave = "MSFP"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try
    End If
    If Request.Form("FINEMP") = "OK" Then
        Try
            objProy.Actualiza_Proyecto(idemp, idProyecto, "FE", strConn, Request.Form("idpropuesta"), 0)
            'Graba evaluación
            Dim dt As System.Data.DataTable = objComun.Preguntas("P", "ESP", strConn)
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
            objProy.Evaluacion(idProyecto, idemp, dtProy.Rows(0)("idemprendedor"), dtEva, strConn)
            Dim msg As New poolin_class.cMensajes
            msg.Envia_Mensaje(idemp, Request.Form("idreceptor"), idProyecto, "PROYECTO FINALIZADO POR CONTRATANTE", "Su cliente finalizó el proyecto satisfactoriamente, <a href='user-proyecto-seguimiento?idproyecto=" & Request.QueryString("idproyecto") & "&FE=EVALUA'>INGRESE AQUÍ</a> para evaluar la colaboración con su cliente.", strConn, 0)
            Envia_NotiXMail(Request.Form("idreceptor"), "PROYECTO FINALIZADO POR CONTRATANTE", strConn)

            msgsave = "FINFE"
        Catch ex As Exception
            msgsave = "ERR"
            errapp = ex.Message.Replace(vbLf, "").Replace(vbCr, "")
        End Try
    End If


    'Si no hay pagos inicializo en ceros
    If IsDBNull(dtProy.Rows(0)("LIBERADO")) Then
        dtProy.Rows(0)("LIBERADO") = 0
    End If

    If IsDBNull(dtProy.Rows(0)("ENM&E")) Then
        dtProy.Rows(0)("ENM&E") = 0
    End If

    If IsDBNull(dtProy.Rows(0)("monto")) Then
        dtProy.Rows(0)("monto") = dtProy.Rows(0)("pago")
    End If

    Dim estatus = ""
    Dim showFE = False
    Select Case dtProy.Rows(0)("estatus")
        Case "A" 'Activo (Publicado)
            estatus = "<span class=""in-progress"">Publicado</span>"
        Case "P" 'En Prodceso
            estatus = "<span class=""in-progress"">En curso</span>"
        Case "CV" 'En Controversia
            estatus = "<span class=""canceled"">En Controversia</span>"
        Case "E" 'Eliminado
            estatus = "<span class=""canceled"">Eliminado</span>"
        Case "FE" 'Finalizado finish
            estatus = "<span class=""finish"">Finalizado</span>"
        Case "FP" 'Finalizado Emprendedor finish
            showFE = True
            estatus = "<span class=""finish"">Fin por quien ofrece el servicio</span>"
    End Select

    Dim dtTMP = objProy.Carga_CategoProyecto(idProyecto, strConn)
    Dim subcatsel = ""
    For Each drCat As System.Data.DataRow In dtTMP.Rows
        subcatsel &= "<a id='subcat-" & drCat("id") & "' href = 'javascript:;' class='btn-categorias' value='" & drCat("id") & "' >" & drCat("subcategoria") & "</a> <input type='hidden' id='tsub-" & drCat("id") & "' name='tsubcat[]' value='" & drCat("id") & "'>"
    Next

    'idcliente > Empleador
    'Emprendedor ES EMPLEADOR

    'idemprendedor > Emprendedor
    'Asignado

    Dim foto = "_avatar_" & dtProy.Rows(0)("idemprendedor") & ".png"
    Dim prom = 0
    Dim usuario = ""
    Dim quientrabaja = "está trabajando en este proyecto"

    If idemp = dtProy.Rows(0)("idcliente") Then
        If Not IsDBNull(dtProy.Rows(0)("idemprendedor")) Then
            prom = objComun.PromEvaluaciones(dtProy.Rows(0)("idemprendedor"), strConn)
            usuario = dtProy.Rows(0)("asignado")
        End If
    Else
        quientrabaja = " (Cliente)"
        usuario = dtProy.Rows(0)("Emprendedor")
        prom = objComun.PromEvaluaciones(dtProy.Rows(0)("idcliente"), strConn)
        foto = "_avatar_" & dtProy.Rows(0)("idcliente") & ".png"

    End If

    Dim fiFoto As New FileInfo(Server.MapPath("poolin_arch/images/png/" & foto))
    If Not fiFoto.Exists Then
        foto = "_avatar_.png"
    End If

    Dim ifiles As Integer = 0
    Dim filesporta As String = ""

    Dim diPorta As New DirectoryInfo(Server.MapPath("poolin_arch/proyectos/" & idProyecto & "/docs/"))

    Dim dtPregParaEmpleado As System.Data.DataTable = objComun.Preguntas("P", "ESP", strConn)
    Dim dtPregParaCliente As System.Data.DataTable = objComun.Preguntas("E", "ESP", strConn)
    Dim dtPropuestas As System.Data.DataTable = objProp.Carga_PropuestasXProy(idProyecto, strConn)
    Dim dtPagos As System.Data.DataTable = objProy.Pagos_Proyecto(idemp, idProyecto, strConn)
    moneda = dtProy.Rows(0)("mon_proy")
    Dim viewpagos = Request.QueryString("viewpagos")
    If viewpagos <> "" Then
        viewpagos = viewpagos.Replace("poolin-", "")
    End If
    Dim activepagos = "active"
    Dim activeprop = "active"
    Dim nomproyecto = dtProy.Rows(0)("nombre")
    'If viewpagos <> "" Then
    '    activeproy = ""
    'Else
    '    activepagos = ""
    'End If
    Dim montopagado As Double = 0
    If Not IsDBNull(dtProy.Rows(0)("monto")) Then
        montopagado = dtProy.Rows(0)("monto")
    End If

    For Each dr As System.Data.DataRow In dtPagos.Rows
        Select Case dr("estatus")
            Case "CANCELADO"
            Case Else
                montopagado -= dr("monto")
        End Select
    Next

    Validation.Add("monto", Validator.Required("<br> Requerido"), Validator.Range(1, montopagado, "<br> Monto a pagar excede el presupuesto."))

    Dim evalcte As String = ""

    If Request.QueryString("FE") <> "" Then
        If Request.QueryString("FE") = "EVALUA" And dtProy.Rows(0)("estatus") = "FE" Then
            If Not objProy.Ya_Evaluo(idProyecto, idemp, strConn) Then
                evalcte = "EVAL"
            Else
                Response.Redirect("user-dash?yaeval=" & Request.QueryString("idproyecto"))
            End If
        End If
    End If

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-proyecto-seguimiento", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    Dim objEmp As New poolin_class.cEmprendedor
    Dim dtEmp As New System.Data.DataTable
    If Request.QueryString("idempview") <> "" Then
        Dim objAdmin As New poolin_class.cAdmin
        Dim cat(0) As String
        dtEmp = objAdmin.Emprendedores_m(cat, "", "", "", strConn, objComun.Decrypt(Request.QueryString("idempview")))
    End If
    Dim tipoemprendedor = Request.QueryString("tipo")
    If Request.QueryString("tipo") = "" Then
        tipoemprendedor = Request.Form("tipo")
    End If

    If Request.Form("msg") = "PAGO" Then
        msgsave = "PAGO"
        If Request.Form("errpago") <> "OK" Then
            msgsave = "ERR"
            errapp = Request.Form("errpago")
        End If
    End If

End Code

@section head
    <!-- ================== BEGIN BASE CSS STYLE ================== -->
    <link href="assets/plugins/jquery-ui/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/plugins/icon/themify-icons/themify-icons.css" rel="stylesheet" />
    <link href="assets/css/animate.min.css" rel="stylesheet" />
    <!-- ================== END BASE CSS STYLE ================== -->

    @*<!-- ================== BEGIN PAGE CSS STYLE ================== -->
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
    <!-- ================== END PAGE CSS STYLE ================== -->*@

    @*<script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>*@

    <link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?9.0">

End Section

<div id="content">
    <h3 class="user_title">@Html.Raw(dtProy.Rows(0)("nombre"))</h3>
    <div class="tabbable">
        <div class="row">
            <div class="col col-lg-8">
                <ul class="nav nav-tabs">
                    <li class="active"><a href="#info" data-toggle="tab">@PageData("li-t3")</a></li>
                    @If IsDBNull(dtProy.Rows(0)("fecha_inicio")) Then
                        activepagos = ""
                        @<li><a href="#propuesta" data-toggle="tab">@PageData("li-t1") (@totProp)</a></li>
                    Else
                        activeprop = ""
                        @<li><a href="#pagos" data-toggle="tab">@PageData("li-t2")</a></li>
                    End If
                    <li><a href="#mensajes" data-toggle="tab">Entregas y avances</a></li>
                </ul>
            </div>
            <div class="col col-lg-4">
                @if dtProy.Rows(0)("estatus") <> "FE" Then
                    If idemp <> dtProy.Rows(0)("idcliente") Then
                        @<button type="button" class="btn btn-primary btn-small" data-toggle="modal" data-target="#modalfinemp">@PageData("btnfin")</button>
                    ElseIf showFE Then
                        @<button class="btn btn-primary" type="button" data-toggle="modal" data-target="#modalconfirmar">@PageData("btnconf")<br />@PageData("tit-fin")</button>
                        @<button class="btn btn-danger" type="button" data-toggle="modal" data-target="#modalrechazar">@PageData("btnrech")<br />@PageData("tit-fin")</button>
                    End If
                End If
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div class="tab-content">
                    <div class="tab-pane" id="mensajes">
                        <div class="cuenta-box">
                            <ul class="email-list" id="div-mensajes"  style="overflow-y:scroll; height:410px">
                            </ul>
                            <form id="frm-upload" enctype="multipart/form-data" method="post">
			                    <div class="form-group">
				                    <div data-format="alias" class="input-group colorpicker-component">
					                    <input type="text" id="tbmsg" name="tbmsg" placeholder=" Escribe aquí tu mensaje y/o adjunta un archivo." class="form-control" />
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
                    <div class="tab-pane" id="propuesta">
                        @for Each drProp As System.Data.DataRow In dtPropuestas.Rows
                            Dim fotoemp = "_avatar_" & drProp("idemprendedor") & ".png"

                            fiFoto = New FileInfo(Server.MapPath("poolin_arch/images/png/" & fotoemp))
                            If Not fiFoto.Exists Then
                                fotoemp = "_avatar_.png"
                            End If

                            Dim promemp = objComun.PromEvaluaciones(drProp("idemprendedor"), strConn)
                            Dim location = "-"
                            If "" & drProp("ciudad") = "" And "" & drProp("estado") = "" Then
                            ElseIf "" & drProp("ciudad") <> "" Then
                                location = drProp("ciudad")
                            End If
                            If "" & drProp("estado") <> "" Then
                                location &= ", " & drProp("estado")
                            End If
                            @<div class="cuenta-box" id="propemp-@drProp("idemprendedor")">
                                <div class="row">
                                    <div class="col-lg-2 centrar">
                                            <img src="~/poolin_arch/images/png/@fotoemp" class="misdatos-foto-modal"> 
                                        @If promemp = 0 Then
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
                                                    If i <= promemp Then
                                                        @<i class="fas fa-star"></i>
                                                    End If
                                                Next
                                            </div>
                                        End If
                                        @PageData("tit-miem")<br />@Format(drProp("fechadesde"), "dd/MM/yyyy")<br />
                                        <img class="flag @drProp("icno")" src="img/blank.gif" alt="@Html.Raw(drProp("pais"))"> @Html.Raw(drProp("pais"))<br />
                                        <a href="mailto:@drProp("email")"><i class="fa fa-envelope"></i></a>
                                        <a href="javascript:;" title="@drProp("celular")"><i class="fa fa-phone"></i></a>
                                        <a href="@drProp("website")" target="_blank" title="@PageData("website")"><i class="fa fa-globe"></i></a>
                                    </div>
                                    <div class="col-lg-8">
                                        <h3>
                                        <form id="frm-@drProp("idemprendedor")"  method="get" action="user-proyecto-seguimiento?#propemp-@drProp("idemprendedor")">
                                            <input type="hidden" name="idempview" value="@objComun.Encrypt(drProp("idemprendedor"))" />
                                            <input type="hidden" name="idproyecto" value="@Request.QueryString("idproyecto")" />
                                            <input type="hidden" name="tipo" value="@tipoemprendedor" />
                                            <a href="javascript:;" onclick="gosubmit('@drProp("idemprendedor")')" >@Html.Raw(drProp("nombres") & " " & drProp("apellidos")) </a>
                                        </form>
                                        </h3>
                                        <p>@Html.Raw(drProp("propuesta"))</p>
                                    </div>
                                    <div class="col-lg-2 centrar">
                                        <label class="publish">@Html.Raw(Format(drProp("monto"), PageData("moneda")) & " " & drProp("moneda_prop")) </label>
                                            @If Val(drProp("dias")) <> 0 Then
                                                @<small class="text-gray-dark"><br />Entrego el proyecto<br />en @drProp("dias") días.</small>
                                            End If

                                    <div Class="row">
                                            <div Class="col-md-12">
                                                <Button type = "button" Class="btn btn-primary" data-id="@drProp("idpropuesta")" data-idemp="@drProp("idemprendedor")" data-nombre="@Html.Raw(drProp("nombres") & " " & drProp("apellidos"))" data-toggle="modal" data-target="#modalaceptaprop">@PageData("btncontratar")</button>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <button type="button" class="btn-def" data-id="@drProp("idpropuesta")" data-toggle="modal" data-target="#modaladdmsg">Mensaje</button>
                                            </div>
                                        </div>
                                        
                                    </div>
                                </div>
                            </div>
                        Next
                    </div>

                    <div class="tab-pane" id="pagos">
                        <h3>@PageData("tit-detpag")</h3>
                        <div class="cuenta-box">
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-3 col-xl-3 centrar">
                                        <span class="modal-title-money">@Html.Raw(Format(dtProy.Rows(0)("monto"), PageData("moneda")) & " " & dtProy.Rows(0)("mon_proy"))</span><br />
                                        <label class="lbl-pagos">@PageData("tit-pago1")</label>
                                    </div>
                                    <div class="col-lg-3 col-xl-3 centrar">
                                        <span class="modal-title-money">@Html.Raw(Format(dtProy.Rows(0)("ENM&E"), PageData("moneda")) & " " & dtProy.Rows(0)("mon_proy"))</span><br />
                                        <label class="lbl-pagos">@PageData("tit-pago2")</label>
                                    </div>
                                    <div class="col-lg-3 col-xl-3 centrar">
                                        <span class="modal-title-money">@Html.Raw(Format(dtProy.Rows(0)("LIBERADO"), PageData("moneda")) & " " & dtProy.Rows(0)("mon_proy"))</span><br />
                                        <label class="lbl-pagos">@PageData("tit-pago3")</label>
                                    </div>
                                    <div class="col-lg-3 col-xl-3 centrar">
                                        <span class="modal-title-money">@Html.Raw(Format(dtProy.Rows(0)("PAGADO"), PageData("moneda")) & " " & dtProy.Rows(0)("mon_proy"))</span><br />
                                        <label class="lbl-pagos">@PageData("tit-pago4")</label>
                                    </div>
                                </div>
                            </div>
                            @if idemp = dtProy.Rows(0)("idcliente") Then
                            @<div class="row">
                                <div class="col-lg-12 col-xl-12">
                                    <button type="button" class="btn btn-primary pull-right" data-target="#modalcrearpago" data-toggle="modal">@PageData("btnpago")</button>
                                </div>
                            </div>
                            End If
                            <div class="row">
                                <div class="col-lg-12">
                                    <table id="dt-pagos" class="table table-condensed table-bordered" style="width:100%">
                                        <thead>
                                            <tr>
                                                <th>@PageData("th-1")</th>
                                                <th>@PageData("th-2")</th>
                                                <th>@PageData("th-3")</th>
                                                <th>@PageData("th-3-1")</th>
                                                <th>@PageData("th-4")</th>
                                                <th>@PageData("th-5")</th>
                                                <th>@PageData("th-6")</th>
                                                @if idemp = dtProy.Rows(0)("idcliente") Then
                                                @<th>@PageData("th-7")</th>
                                                End If
                                            </tr>
                                        </thead>
                                        <tbody>
                                            @for Each dr As System.Data.DataRow In dtPagos.Rows
                                                Select Case dr("estatus")
                                                    Case "CANCELADO"
                                                    Case Else
                                                        montopagado += dr("monto")
                                                End Select
                                            @<tr>
                                                <td>@Html.Raw(dr("descripcion"))</td>
                                                <td>@Html.Raw(Format(dr("monto"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                <td>@Html.Raw(Format(dr("mcomision"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                <td>@Html.Raw(Format(dr("mivacomision"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                <td>@Html.Raw(Format(dr("montopagar"), PageData("moneda")) & " " & dr("moneda"))</td>
                                                <td>
                                                    @select Case dr("estatus").ToString.ToUpper
                                                        Case "CANCELADO"
                                                        Case "EN PROCESO"
                                                            @<span>@Html.Raw(Format(dr("fecha_creacion"), "dd/MMM/yyyy").Replace(".", "")) </span>
                                                        Case "EN M&E"
                                                            @<span>@Html.Raw(Format(dr("fecha_pago"), "dd/MMM/yyyy").Replace(".", "")) </span>
                                                        Case "LIBERADO"
                                                            @<span>@Html.Raw(Format(dr("fecha_pago"), "dd/MMM/yyyy").Replace(".", ""))</span>
                                                        Case "PAGADO"
                                                            @<span>@Html.Raw(Format(dr("fecha_aplicacion"), "dd/MMM/yyyy").Replace(".", "")) </span>
                                                    End Select
                                                    
                                                </td>
                                                <td>@Html.Raw(dr("estatus"))</td>
                                                @if idemp = dtProy.Rows(0)("idcliente") Then
                                                @<td>
                                                    @if dr("estatus").ToString.ToUpper = "EN M&E" Then
                                                        @<button type="submit" class="btn-liberar" data-id="@dr("id")" data-toggle="modal" data-target="#modalliberar">@PageData("btnliberar")</button>
                                                    End If
                                                </td>
                                                end if
                                            </tr>
                                            Next
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane active" id="info">
                        <div class="cuenta-box">
                            <div class="row">
                                <div class="col-lg-8">
                                    <div class="row hidden">
                                        <div class="col-lg-12">
                                            @Html.Raw(subcatsel)
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-12 col-xl-12">
                                            <h4>@PageData("tit-desc")</h4>
                                            <p>@Html.Raw(dtProy.Rows(0)("descripcion"))</p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-12 col-xl-12">
                                            @If Not IsDBNull(dtProy.Rows(0)("idemprendedor")) Then
                                                @<label class="seg-label">@PageData("tit-fecha1")</label>@<small> @Html.Raw(Format(dtProy.Rows(0)("fecha_inicio"), "dd/MM/yyyy"))</small>
                                            Else
                                                @<label class="seg-label">@PageData("tit-fecha2")</label>@<small>  @Html.Raw(Format(dtProy.Rows(0)("fecha_creacion"), "dd/MM/yyyy"))</small>
                                            End If
                                        </div>
                                        @If Not IsDBNull(dtProy.Rows(0)("fecha_inicio")) Then
                                        @<div class="cuenta-box">
                                            <div class="row">
                                                <div class="col-lg-3 col-xl-3 centrar">
                                                    <img src="~/poolin_arch/images/png/@foto" class="img-circle">
                                                
                                                    @If prom = 0 Then
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
                                                                If i <= prom Then
                                                                    @<i class="fas fa-star"></i>
                                                                End If
                                                            Next
                                                        </div>
                                                    End If
                                                </div>
                                                <div class="col-lg-9 col-xl-9">
                                                    <h4>@Html.Raw(usuario) <span style="font-size:16px; color:gray;">@Html.Raw(quientrabaja)</span></h4>
                                                    @If Not IsDBNull(dtProy.Rows(0)("idemprendedor")) Then
                                                        If Not IsDBNull(dtProy.Rows(0)("fecha_inicio")) Then
                                                            @<label>@PageData("tit-fecha1")</label> @Html.Raw(Format(dtProy.Rows(0)("fecha_inicio"), "dd/MM/yyyy"))
                                                        Else
                                                            @<label>@PageData("tit-fecha1")</label> 
                                                        End If
                                                    Else
                                                        @<label>@PageData("tit-fecha2")</label> @Html.Raw(Format(dtProy.Rows(0)("fecha_creacion"), "dd/MM/yyyy"))
                                                    End If
                                                </div>
                                            </div>
                                        </div>
                                        End if
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="row">
                                        <div class="cuenta-box">
                                            <div class="row">
                                                <div class="col-lg-12 col-xl-12">
                                                    <h3 class="tabs-subtitle pull-right">@Html.Raw(estatus)</h3>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12 col-xl-12">
                                                    @if Not IsDBNull(dtProy.Rows(0)("monto")) Then
                                                        @<label class="pull-right">@Html.Raw(Format(dtProy.Rows(0)("monto"), PageData("moneda")) & " " & dtProy.Rows(0)("moneda"))</label>
                                                    Else
                                                        @<label class="pull-right">@Html.Raw("Presupuesto " & Format(dtProy.Rows(0)("pago"), PageData("moneda")) & " " & dtProy.Rows(0)("mon_proy"))</label>
                                                    End If
                                                </div>
                                            </div>

                                            @If diPorta.Exists Then
                                            @<div class="row">
                                                <div class="col-lg-12 col-xl-12">
                                                    @if diPorta.GetFiles.Count <> 0 Then
                                                        @<label>@PageData("tit-down")</label>
                                                        @<ul>
                                                            @For Each fi In diPorta.GetFiles
                                                                @<li><a href = "poolin_arch/proyectos/@idProyecto/docs/@Html.Raw(fi.Name)"> <span class="small">@Html.Raw(fi.Name).ToString.ToLower</span></a></li>
                                                            Next
                                                        </ul>
                                                    End If
                                                </div>
                                            </div>
                                            End If

                                            @*If Not IsDBNull(dtProy.Rows(0)("fecha_inicio")) Then
                                            <div class="row" style="padding-top:10px;">
                                                <div class="col-md-12">
                                                    <button type="button" class="btn btn-primary btn-block" data-id="@dtProy.Rows(0)("idpropuesta")" data-toggle="modal" data-target="#modaladdmsg">@PageData("btn-msg1")<br /> @PageData("btn-msg2")</button>
                                                </div>
                                            </div>
                                            End If*@
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


<div class="modal fade" id="modalview" tabindex="-1" role="dialog" aria-labelledby="modalview">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-body">
                    @For Each drow As System.Data.DataRow In dtEmp.Rows
                        Dim propMoneda As String = ""
                        Dim propMonto As Double = 0
                        Dim propObserv As String = ""
                        Dim propDias As Integer = 0
                        Dim fotoview As String = ""
                        Dim dtCatego
                        fotoview = "_avatar_" & drow("idemp") & ".png"
                        Dim modalFoto As New FileInfo(Server.MapPath("poolin_arch/images/png/" & fotoview))
                        If Not modalFoto.Exists Then
                            fotoview = "_avatar_.png"
                        End If
                        dtCatego = objEmp.SubCategos_Emp(drow("idemp"), strConn)
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
                @<div class="pool-box">

                    <div class="row">
                        <div class="col-lg-4 col-md-4">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div class="publish">
                                        <img src="~/poolin_arch/images/png/@fotoview" class="misdatos-foto-modal">
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
                                        @PageData("modalview-1")<br />
                                        @Format(drow("fecha_creacion"), "dd/MM/yyyy")

                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12 col-md-12 demo-icon-font">
                                    <img class="flag @drow("icno")" src="img/blank.gif" alt="@Html.Raw(drow("pais"))"> @Html.Raw(drow("pais"))
                                    <br />
                                    <a href="mailto:@drow("email")"><i class="fa fa-envelope"></i></a>
                                    @if drow("celular").ToString <> "" Then
                                        @<a href="javascript:;" title="@drow("celular")"><i class="fa fa-phone"></i></a>
                                    End If
                                    @if drow("website").tostring<>"" then
                                        @<a href="@drow("website")" target="_blank" title="Sitio Web"><i class="fa fa-globe"></i></a>
                                    end if
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-8 col-md-8">
                            <div class="title-pool-emp">@Html.Raw(drow("emprendedor"))</div>
                            <span class="category">
                                @If Not IsNothing(dtCatego) Then
                                    For Each dCat As System.Data.DataRow In dtCatego.Rows
                                         @<span class="btn-categorias">@dCat("subcategoria") </span>
                                    Next
                                Else
                                        @<span class="btn-categorias">@PageData("slider1-tit_1")</span>
                                End If
                            </span>
                            @*<h3>@Html.Raw(drow("slogan").ToString.Replace(vbCrLf, "<br />"))</h3>*@
                            @If drow("descripcion").ToString <> "" Then
                                @<p class="description">@Html.Raw(drow("descripcion").ToString.Replace(vbCrLf, "<br />"))</p>
                            End If
                        </div>
                        @*<div class="col-lg-2 col-md-2">
                            <a href="javascript:;" class="btn-def" data-id="@drow("idemp")" data-toggle="modal" data-target="#modaladdmsg">Enviar mensaje</a>
                        </div>*@
                    </div>
                </div>
                    Next
            </div>
        </div>
    </div>
</div>




    <div class="modal fade" id="modalliberar"  tabindex="-1" role="dialog" aria-labelledby="modalliberar"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="modal-title">Liberar pago a Emprendedor</h2>
                </div>
                <form id="formliberar" method="post" action="~/user-proyecto-seguimiento">
                    <input type="hidden" name="liberado" value="OK" />
                    <input type="hidden" id="idrecibo" name="idrecibo"/>
                    <input type="hidden"  name="idproyecto" value="@idProyecto"/>
                    <input type="hidden" name="tipo" value="@tipoemprendedor" />
                    <div class="modal-body">
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <span>El monto seleccionado será liberado al Emprendedor que colabora contigo.<br />
                                    ¿Deseas continuar con la liberación de pago?.</span>
                                </div>
                            </div>
                        </div>
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <button type="submit" class="btn btn-primary pull-right">Enviar</button>
                                    <button type="button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalordenpago"  tabindex="-1" role="dialog" aria-labelledby="modalordenpago"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    @select Case viewpagos
                        Case "error"
                            @<h2 class="modal-title">¡Ocurrió un ERROR!</h2>
                        Case "tc"
                            @<h2 class="modal-title">Aplicado Pago con Tarjeta</h2>
                        Case "orden"
                            @<h2 class="modal-title">Generada su Orden de Pago</h2>
                        Case "paypal"
                            @<h2 class="modal-title">Pago realizado mediante PayPal</h2>
                    End Select
                     
                </div>
                <div class="modal-body">
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                @select Case viewpagos
                                    Case "error"
                                        @<span>Se generó un ERROR al realizar su solicitud.<br />
                                        Intentelo mas tarde.</span>
                                    Case "tc"
                                        @<span>Su pago con tarjeta de crédito/debido se aplico con éxito.<br />
                                        Revise su correo en donde le llegará información sobre su pago.</span>
                                    Case "orden"
                                        @<span>Su orden de pago fue generada con éxito.<br />
                                        Revise su correo en donde le llegará información para poder realizar su pago.</span>
                                    Case "paypal"
                                        @<span>Su pago mediante PayPal fue generado con éxito.<br />
                                        Revise su correo en donde le llegará información sobre su pago.</span>
                                End Select

                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
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

    <div class="modal fade" id="modalcrearpago"  tabindex="-1" role="dialog" aria-labelledby="modalcrearpago"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3 class="modal-title">Realizar Pago</h3>
                    <p>Puedes liberar éste pago una vez que estés 100% satisfecho con el trabajo que han realizado para ti.</p>
                    <form id="frm-pagos" method="post" action="~/pago-proyecto">
                        <input type="hidden" name="idproyecto" value="@idProyecto" />
                        <input type="hidden" name="idpropuesta" id="idpropuesta" value="@dtProy.Rows(0)("idpropuesta")" />
                        <input type="hidden" name="idemp" id="idemp" value="@idemp" />
                        <input type="hidden" name="moneda" value="@moneda"/>
                        <input type="hidden" name="msg" value="PAGO"/>
                        <input type="hidden" name="data-id" id="data-id" value="" />
                        <div class="modal-body">
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-4 form-group">
                                        <label>Importe (@dtProy.Rows(0)("moneda"))</label>
                                        <span class="msg-val">@Html.ValidationMessage("monto")</span>
                                        <input type="number" name="monto" class="form-control" placeholder="$0.00" @Validation.For("monto")  onchange="calmonto(this.value);" />
                                    </div>
                                    <div class="col-lg-8 form-group">
                                        <label>Descripción</label>
                                        <span class="msg-val">@Html.ValidationMessage("descripcion")</span>
                                        <input type="text" name="descripcion" class="form-control" placeholder="Ej. Pago inicial" @Validation.For("descripcion") />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-4  form-group">
                                    <label>Comisión</label>
                                    <input type="text" id="comision" name="mcomision" class="form-control" readonly />
                                </div>
                                <div class="col-lg-4  form-group">
                                    <label>I.V.A.</label>
                                    <input type="text" id="ivacomision" name="mivacomision" class="form-control" readonly />
                                </div>
                                <div class="col-lg-4  form-group">
                                    <label>Monto a Pagar</label>
                                    <input type="text" id="montopagar" name="montopagar" class="form-control" readonly />
                                </div>
                            </div>
                            <div class="fields_wrap">
                                <div class="row form-group">
                                    <div class="col-lg-12">
                                        <button type="submit" class="btn btn-primary pull-right">Generar Pago</button>&nbsp;&nbsp;      
                                    </div>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>


    <div class="modal fade" id="modalaceptaprop"  tabindex="-1" role="dialog" aria-labelledby="modalaceptaprop"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Aceptación de la Propuesta.</h3>
                    <div class="modal-body">
                        <form id="frmconfirma" method="post" action="~/user-proyecto-seguimiento">
                            <input type="hidden" id="idproyconfirm" name="idproyecto" />
                            <input type="hidden" id="idpropconfirm" name="idpropuesta" />
                            <input type="hidden" id="idempconfirm" name="idemprendedor" />
                            <input type="hidden" name="confirma" value="OK"/>
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-12">
                                        Esta confirmando colaborar con el emprendedor <span id="empleado"></span>.
                                    </div>
                                </div>
                            </div>
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <button type="button" id="btnpropacept" class="btn btn-primary pull-right">Aceptar</button>&nbsp;&nbsp;      
                                        <button type="button" class="btn btn-default pull-right" data-dismiss="modal">Cancelar</button>
                                    </div>
                                </div>
                            </div>

                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="modal fade" id="modalrechazar"  tabindex="-1" role="dialog" aria-labelledby="modalrechazar"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3 class="modal-title">Notificación de Proyecto NO Finalizado</h3>
                    <div class="modal-body">
                        <form id="formNOfinemp" method="post" action="">
                            <input type="hidden" name="FINNOPROY" value="OK"/>
                            <input type="hidden" name="idproyecto" value="@dtProy.Rows(0)("idproyecto")" />
                            <input type="hidden" name="idreceptor" value="@dtProy.Rows(0)("idemprendedor")" />
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <label>Mensaje</label>
                                        <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                        <textarea type="text" class="form-control" name="mensaje" @Validation.For("mensaje") placeholder="Enviale un mensaje a quien te ofreció el servicio para notificarle que ¡NO HA FINALIZADO EL PROYECTO!." rows="4"></textarea>
                                    </div>
                                </div>
                            </div>                
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <button type="submit" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                        <button type="button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                                    </div>
                                </div>
                            </div>
                        </form>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="modal-mensajes" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="modal-mensajes"  aria-hidden="true">
        <div class="modal-dialog"  role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h3 class="modal-title" id="title-mensajes"></h3>
                </div>
                <div class="modal-body">
                    <p id="body-mensajes" class="f-s-16"></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                </div>
            </div>
        </div>
    </div>
<div class="modal fade" id="modalevalcte"  tabindex="-1" role="dialog" aria-labelledby="modalevalcte"  aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Se finalizó el proyecto</h4>
                <h5 class="modal-title">@dtProy.Rows(0)("nombre")</h5>
                <div class="modal-body">
                    <form id="ratingsForm" method="post" action="user-dash?idproyecto=@Html.Raw(Request.QueryString("idproyecto"))" >
                        <input type="hidden" name="EVALCTE" value="OK" />
                        <input type="hidden" name="idpropuesta" value="@dtProy.Rows(0)("idpropuesta")" />
                        <input type="hidden" name="idproyecto" value="@idProyecto" />
                            <p>Es importante para nosotros que evalués a tu contratante <i>@dtProy.Rows(0)("Emprendedor")</i></p>                            
                            <small>1 = Totalmente en desacuerdo</small><br />
                            <small>2 = En desacuerdo</small><br />
                            <small>3 = Ni de acuerdo ni en desacuerdo</small><br />
                            <small>4 = De acuerdo</small><br />
                            <small>5 = Totalmente de acuerdo</small><br />
                            <p></p>
                            <div class="row form-group">
                                <div class="col-lg-7""></div>
                                <div class="col-lg-1"><small>1</small></div>
                                <div class="col-lg-1"><small>2</small></div>
                                <div class="col-lg-1"><small>3</small></div>
                                <div class="col-lg-1"><small>4</small></div>
                                <div class="col-lg-1"><small>5</small></div>
                            </div>
                        @for Each dr As System.Data.DataRow In dtPregParaCliente.Rows
                            @<div class="row form-group">
                                <div class="col-lg-7">@dr("pregunta")</div>
                                <div class="col-lg-1">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="1" />
                                </div>
                                <div class="col-lg-1">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="2" />
                                </div>
                                <div class="col-lg-1">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="3" />
                                </div>
                                <div class="col-lg-1">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="4" />
                                </div>
                                <div class="col-lg-1">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="5" />
                                </div>
                            </div>
                        Next
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <button type = "submit" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                    <button type = "button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                                </div>
                            </div>
                        </div>

                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalconfirmar"  tabindex="-1" role="dialog" aria-labelledby="modalconfirmar"  aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Confirma la finalización del proyecto</h4>
                <h5 class="modal-title">@dtProy.Rows(0)("nombre")</h5>
                <div class="modal-body">
                    <form id="ratingsForm" method="post" action="">
                        <input type="hidden" name="FINEMP" value="OK" />
                        <input type="hidden" name="idpropuesta" value="@dtProy.Rows(0)("idpropuesta")" />
                        <input type="hidden" name="idreceptor" value="@dtProy.Rows(0)("idemprendedor")" />
                        <input type="hidden" name="idproyecto" value="@idProyecto" />
                            <p>Vas a confirmar la finalización del proyecto, es importante para nosotros que evalúes a la persona que te ofreció el servicio <i>@dtProy.Rows(0)("Asignado")</i>.</p>
                            <p>Indica qué tanto estás de acuerdo con las siguientes afirmaciones sobre el servicio que te prestaron, en una escala donde 1 significa que estás totalmente en desacuerdo y 5 significa que estás totalmente de acuerdo.</p>
                        <small>1 = Totalmente en desacuerdo</small><br />
                        <small>2 = En desacuerdo</small><br />
                        <small>3 = Ni de acuerdo ni en desacuerdo</small><br />
                        <small>4 = De acuerdo</small><br />
                        <small>5 = Totalmente de acuerdo</small><br />
                            <div class="row form-group pt-5">
                                <div class="col-lg-7"></div>
                                <div class="col-lg-1 text-center">1</div>
                                <div class="col-lg-1 text-center">2</div>
                                <div class="col-lg-1 text-center">3</div>
                                <div class="col-lg-1 text-center">4</div>
                                <div class="col-lg-1 text-center">5</div>
                            </div>
                        @for Each dr As System.Data.DataRow In dtPregParaEmpleado.Rows
                            @<div class="row form-group">
                                <div class="col-lg-7">@dr("pregunta")</div>
                                <div class="col-lg-1 text-center">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="1" />
                                </div>
                                <div class="col-lg-1 text-center">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="2" />
                                </div>
                                <div class="col-lg-1 text-center">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="3" />
                                </div>
                                <div class="col-lg-1 text-center">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="4" />
                                </div>
                                <div class="col-lg-1 text-center">
                                    <input type="radio" id="resp-@dr("id")" name="resp-@dr("id")" value="5" />
                                </div>
                            </div>
                        Next
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <button type = "submit" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                    <button type = "button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                                </div>
                            </div>
                        </div>

                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalfinemp"  tabindex="-1" role="dialog" aria-labelledby="modalfinemp"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">Notificación Finalización de Proyecto</h3>
                <div class="modal-body">
                    <form id="formfinemp" method="post" action="">
                        <input type="hidden" name="FINCTE" value="OK"/>
                        <input type="hidden" name="idproyecto" value="@dtProy.Rows(0)("idproyecto")" />
                        <input type="hidden" name="idreceptor" value="@dtProy.Rows(0)("idcliente")" />
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <label>Mensaje</label>
                                    <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                    <textarea type="text" class="form-control" name="mensaje" @Validation.For("mensaje") placeholder="Enviale un mensaje a tu contratante para notificarle que ya finalizaste el proyecto." rows="4"></textarea>
                                </div>
                            </div>
                        </div>                
                        <div class="fields_wrap">
                            <div class="row">
                                <div class="col-lg-12">
                                    <button type = "submit" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                    <button type = "button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modaladdmsg"  tabindex="-1" role="dialog" aria-labelledby="modaladdmsg"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">Enviar mensaje</h3>
            </div>
            <div class="modal-body">
                <form id="formmsg" method="post" action="~/user-proyecto-seguimiento">
                    <input type="hidden" name="msg" value="OK"/>
                    <input type="hidden" name="idproyecto" id="idproy-msg" value="@dtProy.Rows(0)("idproyecto")" />
                    @If idemp <> dtProy.Rows(0)("idcliente") Then
                        @<input type="hidden" name="idreceptor" id="idrece-msg" value="@dtProy.Rows(0)("idcliente")" />
                    Else
                        @<input type="hidden" name="idreceptor" id="idrece-msg" value="@dtProy.Rows(0)("idemprendedor")" />
                    End If
                    <input type="hidden" name="idpropuesta" id="idpropmsg" />
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <label>Mensaje <em class="text-danger">*</em></label>
                                <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                <textarea type="text" class="form-control" id="mensaje" name="mensaje" @Validation.For("mensaje") rows="4"></textarea>
                            </div>
                        </div>
                    </div>                
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <button type = "button" id="btnsengmsg" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                <button type = "button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                            </div>
                        </div>
                    </div>
                </form>

            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modaladdcita" tabindex="-1" role="dialog" aria-labelledby="modaladdcita"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">Agendar Cita</h3>
            </div>
            <div class="modal-body">
                <form id="formcita" method="post" action="~/user-proyecto-seguimiento">
                    <input type="hidden" name="save" value="OK" />
                    <input type="hidden" id="idproyecto-cita" name="idproyecto" value="@dtProy.Rows(0)("idproyecto")" />
                    <input type="hidden" id="proyecto-cita" name="proyecto" value="@dtProy.Rows(0)("nombre")" />
                    <input type="hidden" id="idrece-cita" name="idrece-cita" value="@idreceptor" />
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <h3>@dtProy.Rows(0)("nombre")</h3>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <label>Asunto <em class="text-danger">*</em></label>
                                <span class="msg-val">@Html.ValidationMessage("asunto")</span>
                                <textarea type="text" class="form-control" id="asunto-cita" name="asunto" @Validation.For("asunto") rows="4"></textarea>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-6">
                                <label>Fecha y Hora</label>
                                <span class="msg-val">@Html.ValidationMessage("fechahora")</span>
                                <input type = "datetime-local" class="form-control" id="fechahora" name="fechahora" value="@Format(Now.AddHours(1), "yyyy-MM-ddTHH:00")"  @Validation.For("fechahora")/>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <button type="button" id="btnagendacita" class="btn btn-primary pull-right">Enviar</button>&nbsp;&nbsp;      
                                <button type="button" class="btn btn-default pull-right" data-dismiss="modal">Cerrar</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>


    </div>
<button id="btn-mensajes" class="hidden" data-target="#modal-mensajes" data-toggle="modal"> CLICK </button>
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

    <script src="//cdnjs.cloudflare.com/ajax/libs/tether/1.2.0/js/tether.min.js" integrity="sha384-Plbmg8JY28KFelvJVai01l8WyZzrYWG825m+cZ0eDDS1f7d/js6ikvy1+X+guPIB" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>

	<script src="assets/js/apps.js"></script>
    <script>
        $(document).ready(function () {
            App.init();
            //$("#btnrealizapago").click(function () {
            //    alert("PAGO");
            //    $("#modalcrearpago").modal("show");
            //});

            $("#btnaddfile").click(function () {
                $("#uparchivos").click();    
            });

            var msgsend = "@msgsave";
            switch (msgsend)
            {
                case "OK":
                    $("#title-mensajes").html("Cita agendada");
                    $("#body-mensajes").html("La cita se agendo satisfactoriamente.");
                    $("#btn-mensajes").click();
                    break;
                case "MS":
                    $("#title-mensajes").html("Mensaje");
                    $("#body-mensajes").html("El mensaje se envió satisfactoriamente.");
                    $("#btn-mensajes").click();
                    break;
                case "PAGO":
                    $("#title-mensajes").html("Pago Registrado");
                    $("#body-mensajes").html("El Pago se registro de forma satisfactoria.");
                    $("#btn-mensajes").click();
                    break;
                case "MSFP":
                    $("#title-mensajes").html("Finalización del Proyecto");
                    $("#body-mensajes").html("El mensaje notificando la finalización de proyecto, se envió satisfactoriamente.");
                    $("#btn-mensajes").click();
                    break;
                case "LIBOK":
                    $("#title-mensajes").html("Pago liberado");
                    $("#body-mensajes").html("El pago fue liberado y en próximos días depositado al quien te ofrece el servicio.");
                    $("#btn-mensajes").click();
                    break;
                case "FINFE":
                    $("#title-mensajes").html("Finalizacion del Proyecto");
                    $("#body-mensajes").html("La finalización y evaluación del proyecto se actualizó satisfactoriamente.");
                    $("#btn-mensajes").click();
                    break;
                case "MSNOFIN":
                    $("#title-mensajes").html("Notificación");
                    $("#body-mensajes").html("Se notificó al que te ofrece el servicio que no finalizo el Proyecto.");
                    $("#btn-mensajes").click();
                    break;
                case "ERR":
                    $("#title-mensajes").html("Error");
                    $("#body-mensajes").html("Hubo un Error. Intentalo mas tarde. @errapp");
                    $("#btn-mensajes").click();
                    break;
            }



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
            
            //handleRenderTableData();

            var viewpagos = "@viewpagos";

            if (viewpagos != "") {
                $("#modalordenpago").modal();
            }

            var handleRenderTableData = function () {
                var rowReorderOption = ($(window).width() > 767) ? true : false;
                var table = $('#dt-pagos').DataTable({
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

         
            $('#modaladdmsg').on('show.bs.modal', function (e) {
                var idprop = e.relatedTarget.dataset.id;
                document.getElementById("idpropmsg").value = idprop;
            });

            $('#modalliberar').on('show.bs.modal', function (e) {
                var idrec = e.relatedTarget.dataset.id;
                document.getElementById("idrecibo").value = idrec;
            });

            $('#modalaceptaprop').on('show.bs.modal', function (e) {
                var idprop = e.relatedTarget.dataset.id;
                var idemp = e.relatedTarget.dataset.idemp;
                var nom = e.relatedTarget.dataset.nombre;
                document.getElementById("idproyconfirm").value = '@idProyecto';
                document.getElementById("idpropconfirm").value = idprop;
                document.getElementById("idempconfirm").value = idemp;
                document.getElementById("empleado").innerHTML = " <strong><i>" + nom + "</i></strong>";
            });

            vermensajes(@idreceptor, @idProyecto)
        });

        function vermensajes(idemisor, idproy) {
            $(".msg-emp").removeClass("bg-success");
            $("#li-" + idemisor + "_" + idproy).toggleClass("bg-success");
            $("#idproyecto").val(idproy);
            $("#idreceptor").val(idemisor);
            $.ajax({
                type: 'POST',
                url: 'wsmensajes.asmx/Mensaje_Head',
                data: "{idemisor: " + idemisor + ", idreceptor: @idemp, idproyecto: " + idproy + ", updateleido: 1}",
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
            //elmnt.scrollIntoView();
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
        function calmonto(valor) {
            var monto = parseFloat(valor);
            var pemp = parseFloat("@pemp");
            var iva = parseFloat("@iva");
            var comision = monto * pemp / 100;
            var montoiva = comision * iva/100
            var total = monto + montoiva + comision;

            //const options = {style: 'currency', currency: '@moneda' };
            const options = {style: 'currency', currency: '@moneda' };
            const locale = "es-MX";
            const f = new Intl.NumberFormat(locale, options);
             //"$33,333.00"

            document.getElementById("comision").value = comision.toFixed(2); 
            document.getElementById("montopagar").value = total.toFixed(2);
            document.getElementById("ivacomision").value = montoiva.toFixed(2);

            if (valor == "") {
                return false;
            }
//MERCADO PAGO
            $.ajax({
                type: "POST",
                url: "wsseguimiento.asmx/Mercado_Pago",
                dataType: "json",
                contentType: 'application/json; utf-8',
                data: "{titulo: '@nomproyecto',"
                    + " monto: " + total + "}",
                success: function (data) {
                    if (data.d != "") {
                        $("#data-id").val(data.d);
                    } else {
                        alert('ERROR');
                        $("#frm-pagos").submit();
                        //setTimeout(function () { $("#frm-contrata").submit(); }, 3000);
                    }
                }, //End of AJAX Success function  
                failure: function (data) {
                    alert("Fail: " + data.responseText);
                }, //End of AJAX failure function  
                error: function (data) {
                    alert("Error: " + data.responseText);
                    //$("#msgpay").html(data.responseText);
                } //End of AJAX error function  
            });

        }

        var evalua = "@evalcte";
        
        if (evalua == "EVAL")
        {
            //$("#content").toggleclass("hidden");
            //
            //alert("evalua");
            $('#modalevalcte').on('show.bs.modal', function (e) {
                //var idprop = e.relatedTarget.dataset.id;
                //document.getElementById("idpropmsg").value = idprop;
            });
            $("#modalevalcte").modal("show");
        }

        var idempview = "@Request.QueryString("idempview")"
        if (idempview != "") {
            $("#modalview").modal('show');
        }

        function gosubmit(valor) {
            document.getElementById("frm-" + valor).submit();
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
    </script>

    <script type="text/javascript">

        $(function () {
            //var username = $("#iduser").val();
            //var proxy = $.connection.messageHub;
            //proxy.client.recibenoti = function (message, user, userenvio) {
            //    if (document.getElementById("iduser").value == user) {
            //        make_msg(user);
            //    }
            //};

            var username = $("#iduser").val();
            var proxy = $.connection.messageHub;

            $.connection.hub.start().done(function () {
                proxy.server.connect(username);
            });

            $.connection.hub.start();


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
            //    proxy.server.connect(username);
            //});

            ////myHub = $.connection.messagehub;
            //try {
            //    //$.connection.hub.logging = true;
            //    $.connection.hub.start();
            //}
            //catch (e)
            //{
            //    console.log("POOL-EMP  " + e.message);
            //}

            $("#btnpropacept").click(function () {
                $("idproyconfirm").val('@idProyecto');
                try {
                    proxy.server.senmessage('PROPUESTA ACEPTADA', $('#idempconfirm').val(), @idemp);
                    $.ajax({
                        type: 'POST',
                        url: 'wsseguimiento.asmx/AceptaPropuesta',
                        data: "{idemp_emisor: " + @idemp +
                            ", idemp_rec: " + $('#idempconfirm').val() +
                            ", idproyecto: " + @idProyecto +
                            ", idpropuesta:" + $('#idpropconfirm').val() +
                            ", asunto: 'PROPUESTA ACEPTADA'"  +
                            ", mensaje: 'Después de haber revisado tu propuesta he decidido aceptarla. Te pido contactarte conmigo a la brevedad para iniciar el proyecto.'}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
                            $('#frmconfirma').submit();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert("Error: " + textStatus);
                            return;
                        }
                    });
                }
                catch (e)
                {
                    alert("Cita NO agendada. " + e.message);
                }
            });

            $("#btnagendacita").click(function () {
                if ($('#asunto-cita').val() == "") {
                    $('#asunto-cita').focus();
                    return;
                }
                try {
                    proxy.server.senmessage($('#asunto-cita').val(), $('#idrece-cita').val(), $('#iduser').val());
                    $.ajax({
                        type: 'POST',
                        url: 'wsseguimiento.asmx/Agenda_Cita',
                        data: "{idemp: " + @idemp +
                            ", idrecibe: " + $('#idrece-cita').val() +
                            ", idproyecto: " + $('#idproyecto-cita').val() +
                            ", proyecto:'" + $('#proyecto-cita').val() +
                            "', fechahora: '" + $('#fechahora').val() +
                            "', asunto:'" + $("#asunto-cita").val() + "'}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
                            $('#formcita').submit();
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert("Error: " + textStatus);
                            return;
                        }
                    });
                    console.log("CITA ENVIADA..." + $("#iduser").val());
                }
                catch (e)
                {
                    alert("Cita NO agendada. " + e.message);
                }
            });

            $("#btnsengmsg").click(function () {
                if ($('#mensaje').val() == "") {
                    $('#mensaje').focus();
                    return;
                }

                try {
                    proxy.server.senmessage($('#mensaje').val(), $('#idrece-msg').val(), $('#iduser').val());
                    $.ajax({
                        type: 'POST',
                        url: 'wsseguimiento.asmx/send_msg',
                        data: "{user: '" + @idemp +
                            "', userenvia: '" + $('#idrece-msg').val() +
                            "', asunto:'', mensaje: '" + $('#mensaje').val() +
                            "', idmsgpadre: '0', idproyecto:'" + $("#idproy-msg").val() +
                            "', idpropuesta: " + Number($("#idpropmsg").val()) + "}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
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

            $("#btnsendmsg").click(function () {
                if ($("#tbmsg").val() == "") {
                    $("#tbmsg").focus();
                    return false;
                }
                $.ajax({
                    type: 'POST',
                    url: 'wsmensajes.asmx/Enviar_Mensaje',
                    data: "{idemisor: @idemp, idreceptor: @idreceptor, asunto: '', mensaje:'" + $("#tbmsg").val() + "', idproyecto: " + $("#idproyecto").val() + "}",
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
        ///*make_msg*/($("#iduser").val());
    </script>

End Section