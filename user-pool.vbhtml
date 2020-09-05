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

    Private Sub Grabar_Propuesta(ByVal idproyecto As Long, ByVal idemp As Long, ByVal strConn As String)
        Dim p As New poolin_class.cPropuestas
        Dim idprop As Long = 0

        If Not IsNothing(Request.Form("idpropuesta")) Then
            idprop = Request.Form("idpropuesta")
        End If

        p.Crear_Propuesta(idprop, idproyecto, idemp, Request.Form("observ"),
                  CDbl(Request.Form("monto")), Request.Form("moneda"), True, strConn, Request.Form("dias"))

        Envia_NotiXMail(idemp, idproyecto, Request.Form("observ"), strConn)
    End Sub

    Protected Sub Envia_NotiXMail(ByVal idemp As Long,
                                  ByVal idproyecto As String,
                                  ByVal msg As String,
                                  ByVal strConn As String,
                                  Optional ByVal prop As Boolean = False)
        Dim http As String = ""

        Dim htmlmsg As String

        Dim objProy As New poolin_class.cProyectos
        Dim objComun As New poolin_class.cComunes
        Dim dtProy As System.Data.DataTable = objProy.Datos_Proyecto(idproyecto, strConn)

        If dtProy.Rows.Count = 0 Then
            Exit Sub
        End If

        Dim idreceptor As Long = dtProy.Rows(0)("idemprendedor")

        'Tenemos que buscar quien publica el proyecto para enviarle notificación por correo
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
        'If Not prop Then
        'htmlmsg = htmlmsg.Replace("[URL]", "https://mueveyemprende.io/inbox_per.aspx?e=" & c.Encrypt(idreceptor) & "&t=" & c.Encrypt(tipo) & "&i=" & c.Encrypt(idioma) & "&p=inbox")
        htmlmsg = htmlmsg.Replace("[URL]", "https://mueveyemprende.io/")
        'Else
        'htmlmsg = htmlmsg.Replace("[URL]", "http://app.poolin.com.mx/propuestas_recibidas.aspx?e=" & c.Encrypt(idreceptor) & "&t=" & c.Encrypt(tipo) & "&i=" & c.Encrypt(idioma) & "&p=inbox")
        'End If

        'htmlmsg = "<p><b>Tienes una notificación nueva.</b></p>"
        'htmlmsg &= "<p>Para acceder a ella presiona la dirección URL que aparece debajo, o cópiala y págala en la dirección de tu navegador.</p>"
        'htmlmsg &= "<a href='http://app.poolin.com.mx/propuestas_recibidas.aspx?e=" & c.Encrypt(idreceptor) & "&t=" & c.Encrypt(tipo) & "&i=" & c.Encrypt(idioma) & "'>http://app.poolin.com.mx/propuestas_recibidas.aspx?e=" & c.Encrypt(idreceptor) & "&t=" & c.Encrypt(tipo) & "&i=" & c.Encrypt(idioma) & "</a>"
        'htmlmsg &= "<p>Equipo Poolin</p>"
        Dim objMsg As New poolin_class.cMensajes

        msg &= "<p>Para revisar mi propuesta presiona <a href='user-proyecto-seguimiento?idempview" & objComun.Encrypt(idemp) & "=&idproyecto=" & objComun.Encrypt(idproyecto) & "&tipo=empleador'>AQUÍ</a></p>"

        objMsg.Envia_Mensaje(idemp, idreceptor, idproyecto, "NUEVA PROPUESTA", msg, strConn)

        Dim objmails As New poolin_class.cSendGrid
        objmails.Correo_SMTP("NUEVA PROPUESTA EN M&E", htmlmsg, emailemp, "", strConn)


    End Sub
End Functions
@code
    PageData("pagina") = "user-pool"
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
    PageData("oppool") = "active"

    PageData("curr") = "#.00"
    PageData("moneda") = "#,##0.00"

    Validation.RequireField("monto", "<br>Requerido")
    Validation.RequireField("dias", "<br>Requerido")
    Validation.RequireField("observ", "<br>Requerido")
    Validation.RequireField("mensaje", "<br> Requerido")

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim objadmin As New poolin_class.cAdmin

    Dim objProy As New poolin_class.cProyectos
    Dim objMsg As New poolin_class.cMensajes
    Dim objComun As New poolin_class.cComunes
    Dim objProp As New poolin_class.cPropuestas

    Dim objCat As New poolin_class.cCategorias
    Dim dtCat As System.Data.DataTable = objCat.Carga_Categorias(strConn)
    Dim dtSubCat As System.Data.DataTable

    Dim foto As String = ""
    Dim prom As Int16 = 0

    Dim dtProy As System.Data.DataTable
    Dim dtCatego As System.Data.DataTable
    Dim ttiempo As Int16 = -1
    Dim tiempo As String = ""

    Dim subcat() As String = Request.Form.GetValues("tsubcat[]")
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
    Dim param As String = ""
    Dim idproy As Long = -1
    Dim pres As String = Request.Form("presupuesto")
    Dim tpago As String = Request.Form("tipopago")
    Dim puntua As String = Request.Form("puntuacion")
    Dim tperfil As String = "1"

    If Request.QueryString("idproyecto") <> "" Then
        Dim dttmp As System.Data.DataTable = objProy.Datos_Proyecto(objComun.Decrypt(Request.QueryString("idproyecto")), strConn)
        If dttmp.Rows.Count <> 0 Then
            param = dttmp.Rows(0)("nombre")
        End If
    End If
    PageData("nom-val") = ""
    If idemp <> 0 Then
        If Not IsNothing(Request.Form("opperfil")) Then
            tperfil = Request.Form("opperfil")
        End If
        dtProy = objadmin.Proyectos_Activos_V2(idemp,
            param,
            strConn, "'P','V'",
            idproy,,, idscat, pres, puntua, tpago, tperfil)
        Dim objperfil As New poolin_class.cEmprendedor
        Dim dtPerfil As System.Data.DataTable = objperfil.Datos_Perfil(idemp, strConn)
        PageData("nom-val") = dtPerfil.Rows(0)("emp").ToString.Trim
    Else
        Response.Redirect("https://mueveyemprende.io")
    End If
    Dim msgsave As String = ""

    If Request.Form("msg") = "OK" Then
        'Dim msg As New poolin_class.cMensajes
        'Dim asunto = Mid(Request.Form("mensaje"), 1, 15) & "..."
        Try
            'Dim dtproymsg As System.Data.DataTable = objProy.Datos_Proyecto(Request.Form("idproyecto"), strConn)
            'If dtproymsg.Rows.Count <> 0 Then
            '    msg.Envia_Mensaje(idemp, dtproymsg.Rows(0)("idemprendedor"), 0, asunto, Request.Form("mensaje"), strConn, 0)
            '    Envia_NotiXMail(dtproymsg.Rows(0)("idemprendedor"), asunto, strConn)
            msgsave = "MS"
            'Else
            '    msgsave = "ERR"
            'End If
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If

    If Request.Form("idproy") <> "" Then
        Try
            'Grabar_Propuesta(Request.Form("idproy"), idemp, strConn)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If
    Dim cat(0) As String
    Dim objEmp As New poolin_class.cEmprendedor

    'Dim dtEmp As System.Data.DataTable = objadmin.Emprendedores_m(cat, "", "", "", strConn, Val(Request.Form("idempview")))

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-pool", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next
    PageData("Title") = PageData("titulo")

End Code
@section head
    <link href="~/assets/ekko/ekko-lightbox.css" rel="stylesheet">
    @*<link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?19.2">*@
    <link rel="stylesheet" href="assets/videojs/css/jquery.popVideo.css">

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


<div class="modal fade" id="modalview" tabindex="-1" role="dialog" aria-labelledby="modalview">
    <div class="modal-dialog" role="document">

        <div class="modal-content">
            <div class="modal-body" id="bodyview">
                    @*@For Each drow As System.Data.DataRow In dtEmp.Rows
                        Dim propMoneda As String = ""
                        Dim propMonto As Double = 0
                        Dim propObserv As String = ""
                        Dim propDias As Integer = 0
                        Dim fotoview As String = ""
                        fotoview = "_avatar_" & drow("idemp") & ".png"
                        Dim fiFoto As New FileInfo(Server.MapPath("poolin_arch/images/png/" & fotoview))
                        If Not fiFoto.Exists Then
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

                        'End Try*@
                    @*Next*@
            </div>
            <div class="modal-footer">
                <a href="#" class="btn btn-link pull-right" data-dismiss="modal">Cerrar</a>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalpropuesta" tabindex="-1" role="dialog" aria-labelledby="modaladdpropuesta"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modalpropuesta-1") <br /><span class="text-capitalize text-success" id="proyecto"></span></h3>
                <h5>@PageData("pubspor") <strong id="publicado" class="text-uppercase text-danger"></strong></h5>
            </div>
            <div class="modal-body">
                <form id="formpropuesta" class="form-control-static" method="post" action="user-pool" >
                    <input type="hidden" id="idproy" name="idproy" />
                    <input  type="hidden" id="idpropuesta" name="idpropuesta" />
                    <input type="hidden" id="idemp-proy" />
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-4">
                                <label>@PageData("modalpropuesta-2") <em class="text-danger">*</em></label>
                                <span class="msg-val">@Html.ValidationMessage("monto")</span>
                                <div class="inline">
                                    <input type="number" id="monto" name="monto" placeholder="@PageData("modalpropuesta-pholder-1")" class="form-control" @Validation.For("monto") />
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <label>Moneda</label>
                                <input type="text" id="moneda" name="moneda" class="form-control" readonly />
                            </div>
                            <div class="col-lg-6">
                                <label>Días para entrega (si aplica)</label>
                                <span class="msg-val">@Html.ValidationMessage("dias")</span>
                                <input type="number" id="dias" name="dias" placeholder="@PageData("modalpropuesta-pholder-2")" class="form-control" @Validation.For("dias")  />
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="fields_wrap">
                                <div class="col-lg-12">
                                    <label>@PageData("modalpropuesta-5")  <em class="text-danger">*</em></label>
                                    <span class="msg-val">@Html.ValidationMessage("observ")</span>
                                    <textarea class="form-control" placeholder="Incluye ligas que muestren tu experiencia para desarrollar este proyecto." name="observ" id="observ" rows="4"  @Validation.For("observ") ></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <button type="button" id="btnsendprop" class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                <button type="button" class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modalmsg"  tabindex="-1" role="dialog" aria-labelledby="modalmsg"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title text-capitalize" id="msg-proyecto"></h3>
                <small>Proyecto</small>
            </div>
            <div class="modal-body">
                <form id="formmsg" method="post" action="">
                    <input type="hidden" name="msg" value="OK"/>
                    <input type="hidden" name="idproyecto" id="msg-idproyecto" />
                    <input type="hidden" id="idreceptor" />
                    
                    <label>Para: </label> <span id="msg-emp"></span>
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                <label>@PageData("modaladdmsg-2") <em class="text-danger">*</em></label>
                                <span class="msg-val">@Html.ValidationMessage("mensaje")</span>
                                <textarea type="text" Class="form-control" id="mensaje" name="mensaje" @Validation.For("mensaje") rows="4"></textarea>
                            </div>
                        </div>
                    </div>                
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                <button type="button" id="btnsendmsg"  Class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                <button type="button" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>
                    </div>
                </form>

            </div>
        </div>
    </div>
</div>

<div id="content">
    <div class="row">
        <div class="col-md-6">
            <h3 class="user_title">@PageData("titulo")</h3>
            <form id="frmPerfil" method="post" action="~/user-pool">
                <div class="recomendaciones">
                    <h4><i class="far fa-lightbulb"></i> @PageData("filtro-titulo1")</h4>
                    <ul id="recomend">
                        <li>
                            <input type="radio" id="miperfil" name="opperfil" checked="checked" value="1" onchange="goform()" />
                            <label for="miperfil" style="font-size:13px;">@PageData("filtro-op1")</label>
                        </li>
                        <li>
                            <input type="radio" id="otroperfil" name="opperfil" value="2"  onchange="goform()" />
                            <label for="otroperfil" style="font-size:13px;">@PageData("filtro-op2")</label>
                        </li>
                    </ul>
                </div>
                <script>
                    function goform() {
                        document.getElementById("frmPerfil").submit();
                    }
                </script>
            </form>

        </div>

        <div class="col-md-6">
            <form id="frmbuscar" method="post" action="" class="pool-form">
                <input type="hidden" name="opperfil" id="valperfil" />
                <script>
                    var valperfil = document.getElementById("miperfil").value;
                    document.getElementById("valperfil").value = valperfil;
                </script>
                <div class="">
                    <h4 class="inline"><i class="fas fa-sliders-h"></i> @PageData("filtro-titulo2")</h4>
                    <div class="form-group">
                        <div class="row">
                            <div class="col-lg-12 col-md-12">
                                <select id="categoria" name="categoria" class="form-control" onchange="goselcat(this.value)" >
                                    <option value="" disabled selected>@PageData("filtro-op3")</option>
                                    @for Each drCat As System.Data.DataRow In dtCat.Rows
                                        @<optgroup label="@drCat("categoria")">
                                            @code
                                                dtSubCat = objCat.Carga_SubCategorias(drCat("id"), strConn)
                                            End Code
                                            @for Each drSubCat As System.Data.DataRow In dtSubCat.Rows
                                                @<option value="@drSubCat("id")">@drSubCat("subcategoria")</option>
                                            Next
                                        </optgroup>
                                    Next
                                </select>
                            </div>
                            <div class="col-lg-12 col-md-12">
                                <select id="presupuesto" name="presupuesto" class="form-control">
                                    <option value="" selected>Presupuesto por proyecto (Todos)</option>
                                    <option value="1">$1 - $5,000</option>
                                    <option value="2">$5,001 - $10,000</option>
                                    <option value="3">$10,000 - $20,000</option>
                                    <option value="4">> $20,0000</option>
                                </select>
                            </div>
                            <div class="col-lg-12 col-md-12 hide">
                                <select id="tipopago" name="tipopago" class="form-control">
                                    <option value="" selected>Tipo de servicio (Todos)</option>
                                    <option value="H"> Por Hora</option>
                                    <option value="P"> Por Proyecto</option>
                                    @*<option value="M"> Mensual</option>*@
                                    @*<option value="R"> Por Producto</option>*@
                                </select>
                            </div>
                            <div class="col-lg-12 col-md-12">
                                <select id="puntuacion" name="puntuacion" class="form-control">
                                    <option value="" selected>@PageData("filtro-op6")</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                    <option value="4">4</option>
                                    <option value="5">5</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="pool-form">
                        <span id="selcat">
                        </span>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <input type="submit" value="@PageData("btnaplicar")" class="btn btn-primary pull-right">
                        <div class="clearfix"></div>
                    </div>
                </div>
            </form>

        </div>
    </div>


    @For Each drow As System.Data.DataRow In dtProy.Rows
        Dim idprop As Long = objProp.PropuestaPostulacion_Enviada_m(drow("id"), idemp, strConn)
        Dim propMoneda As String = ""
        Dim propMonto As Double = 0
        Dim propObserv As String = ""
        Dim propDias As Integer = 0
        If idprop <> 0 Then
            Dim dtProp As System.Data.DataTable = objProp.Carga_PropuestaPersona(0, 0, strConn, idprop)
            If dtProp.Rows.Count <> 0 Then
                propMoneda = dtProp.Rows(0)("moneda")
                propObserv = dtProp.Rows(0)("propuesta")
                propMonto = dtProp.Rows(0)("monto")
                propDias = dtProp.Rows(0)("dias")
            End If
        End If
        foto = "_avatar_" & drow("idemprendedor") & ".png"
        Dim fiFoto As New FileInfo(Server.MapPath("poolin_arch/images/png/" & foto))
        If Not fiFoto.Exists Then
            foto = "_avatar_.png"
        End If
        dtCatego = objProy.Carga_CategoProyecto(drow("id"), strConn)
        prom = objComun.PromEvaluaciones(drow("idemprendedor"), strConn)
        Dim di As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/proyectos/" & drow("id") & "/docs/"))
        Dim tmpfiles As String = ""
        Try
            Dim numfile = 1
            For Each fi In di.GetFiles()
                tmpfiles &= "<small><a href='poolin_arch/proyectos/" & drow("id") & "/docs/" & fi.Name & "' target='_blank' title='" & fi.Name & "'><i class='fas fa-paperclip'></i> Archivos adjuntos del proyecto</a></small> <br>"
                numfile += 1
            Next
        Catch ex As Exception

        End Try
        @<div class="row" id="@drow("id")">
            <div class="col-md-12">
                <div class="pool-box">
                    <div class="row">
                        <div class="col-md-2 col-lg-2 col-xl-2">
                            <div class="row">
                                <div class="col-lg-12 col-md-12">
                                    <div class="publish">
                                        <img src="~/poolin_arch/images/png/@foto?1.1" class="misdatos-foto-modal">
                                        <div>@PageData("pool-text_1")<br>
                                        <input type="hidden"  id="pubs-@drow("id")" value="@drow("nombres")"/>
                                        <form id="frm-@drow("id")"  method="post" action="user-pool#@drow("id")">
                                            <input type="hidden" name="idempview" value="@drow("idemprendedor")" />
                                            <a href="javascript:;"  data-id="@drow("idemprendedor")" data-toggle="modal" data-target="#modalview">@drow("nombres")</a>
                                        </form>
                                        </div>

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

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12 col-md-12 demo-icon-font">
                                    <img class="flag @drow("icno")" src="img/blank.gif" alt="@Html.Raw(drow("pais"))"> @Html.Raw(drow("pais"))
                                </div>
                            </div>
                        </div>
                        <div class="col-md-8 col-lg-8 col-xl-8 pool-project-view">
                            <input id="mon-@drow("id")" type="hidden" value="@drow("moneda")" />
                            <input id="idprop-@drow("id")" type="hidden" value="@idprop" />
                            <input id="propmoneda-@drow("id")" type="hidden" value="@propMoneda" />
                            <input id="propobserv-@drow("id")" type="hidden" value="@propObserv" />
                            <input id="propmonto-@drow("id")" type="hidden" value="@propMonto" />
                            <input id="propdias-@drow("id")" type="hidden" value="@propDias" />
                            <h4 class="heading" id="h4-@drow("id")">@drow("nombre")
                                @if idprop <> 0 Then
                                    @<span class="msg-val">  @PageData("pool-text_2")</span>
                                End If
                            </h4>
                            <span class="category">
                                @If Not IsNothing(dtCatego) Then
                                    Dim ttmp As String = ""
                                    For Each dCat As System.Data.DataRow In dtCatego.Rows
                                        ttmp &= " | " & dCat("subcategoria")
                                    Next
                                    @<span class="text-info">@Html.Raw(Mid(ttmp, 3)) </span>
                                Else
                                    @<span>@PageData("pool-text_3")</span>
                                End If
                            </span>
                            <div class="description" style="padding-top:10px; padding-bottom:10px; max-width:660px; overflow-x:auto">
                                @Html.Raw(drow("descripcion").ToString.Replace(vbCrLf, "<br />"))
                            </div>
                            @if tmpfiles <> "" Then
                                @<div style="padding-bottom:10px;">
                                    @Html.Raw(tmpfiles)
                                </div>
                            End If
                            <p class="especifications">
                                @PageData("pool-text_4") <span>@Html.Raw(Format(drow("pago"), PageData("moneda")) & drow("moneda"))</span> |
                                @PageData("pool-text_5")
                                @Select Case drow("tipopago")
                                    Case "H" :@<span> @PageData("pool-text_5-op1")</span>
                                    Case "M" : @<span> @PageData("pool-text_5-op2")</span>
                                    Case "P" : @<span> @PageData("pool-text_5-op3")</span>
                                    Case "I" : @<span> @PageData("pool-text_5-op4")</span>
                                    Case "R" : @<span> @PageData("pool-text_5-op5")</span>
                                End Select
                                |
                                @*Duración (días): <span>@drow("duracion")</span> |*@ @PageData("pool-text_6") <span class="publish date">@objMsg.TiempoTrans(idemp, drow("fecha_creacion"), ttiempo, strConn)</span>
                            </p>
                        </div>
                        <div class="col-md-2 col-lg-2 col-xl-2">
                            @If PageData("nom-val") <> "" Then
                                If drow("idemprendedor") <> idemp Then
                                    @<a class="btn btn-primary" onclick="showmodal('@drow("id")', '@drow("idemprendedor")')" data-target="#modalpropuesta" data-toggle="modal">
                                    @if idprop = 0 Then
                                        @PageData("Realizar")
                                    Else
                                        @PageData("Editar")
                                    End If
                                    <br /> @PageData("pool-text_7")</a>
                                    @<a class="btn btn-primary" onclick="showmodal_msg('@drow("id")', '@drow("nombre")', '@drow("nombres")', '@drow("idemprendedor")');"  data-target="#modalmsg" data-toggle="modal">Enviar mensaje</a>
                                End If
                            Else
                                @<a href="javascript:;" class="btn btn-primary pull-right" data-target="#modal-incompleto" data-toggle="modal" >@Html.Raw(PageData("Realizar"))</a>
                            End If
                        </div>
                    </div>
                </div>
            </div>
        </div>
    Next
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
    @*anchors.options.placement = 'left';*@    
    <script src="Scripts/jquery.validate.unobtrusive.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/tether/1.2.0/js/tether.min.js" integrity="sha384-Plbmg8JY28KFelvJVai01l8WyZzrYWG825m+cZ0eDDS1f7d/js6ikvy1+X+guPIB" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>
    <script src="assets/ekko/ekko-lightbox.js"></script>
    <script src="assets/videojs/js/jquery.popVideo.js"></script>

    <script src="Scripts/jquery.signalR-2.4.1.js"></script>
    <script src="signalr/hubs" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#modalview').on('show.bs.modal', function (e) {
                var idemp = e.relatedTarget.dataset.id;
                $.ajax({
                    type: 'POST',
                    url: 'wspropuestas.asmx/Emp_Data',
                    data: "{idemp: '" + idemp + "'}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: function (response) {
                        var xmlDoc = $.parseXML(response.d);
                        var xml = $(xmlDoc);
                        var detalle = xml.find("data");
                        var view = '';
                        $("#bodyview").html('');
                        $(detalle).each(function (i, item) {
                            var calif = Number($(item).find("calif").text());
                            var stars = "";
                            if (calif == 0) { calif = 5;}
                            for (var i = 1; i <= calif; i++) {
                                stars += '<i class="fas fa-star"></i>';
                            }
                            view = '<div class="pool-box"> '
                                + ' <div class="row">'
                                + '     <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 col-xl-4">'
                                + '         <div class="row">'
                                + '             <div class="col-lg-12 col-md-12">'
                                + '                 <div class="publish">'
                                + '                     <img src="poolin_arch/images/png/_avatar_' + idemp + '.png" class="img-circle img-responsive" onerror="this.src=\'img/logo-carr.jpg\'">'
                                + '                         <div class="calification">'
                                + '                             ' + stars
                                + '                         </div>'
                                + '                     <small>Miembre desde: ' + tiempotrans($(item).find("fecha_creacion").text()) + '</small>'
                                + '                 </div>'
                                + '             </div>'
                                + '         </div>'
                                + '         <div class="row">'
                                + '             <div class="col-lg-12 col-md-12 demo-icon-font">'
                                + '                 <img class="flag ' + $(item).find("icno").text() + '" src="img/blank.gif" alt="' + $(item).find("pais").text() + '"> ' + $(item).find("pais").text()
                                + '                 <br />'
                                + '                 <a href="mailto:' + $(item).find("email").text() + '"><i class="fa fa-envelope"></i></a>'
                                + '                 <a href="' + $(item).find("website").text() + '" target="_blank" title="Sitio Web"><i class="fa fa-globe"></i></a>'
                                + '             </div>'
                                + '         </div>'
                                + '     </div>'
                                + '     <div class="col-lg-8 col-md-8  col-sm-8 col-xs-8 col-xl-8">'
                                + '         <div class="title-pool-emp" style="padding-bottom:10px;">' + $(item).find("Emprendedor").text() + ' <br><small class="light-blue">' + $(item).find("slogan").text() + '</small></div>'
                                + '             <p class="description">' + $(item).find("descripcion").text() + '</p>'
                                + '     </div>'
                                + ' </div>'
                                + ' </div>';

                        });
                        console.log(view);
                        $("#bodyview").html(view);
                        //$('#modalview').modal('show');
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("Error: " + textStatus);
                        return;
                    }
                });
            });

        });

        function showmodal(idproy, idemprendedor, emprendedor) {
            var idprop = document.getElementById("idprop-" + idproy).value;
            $("#idemp-proy").val(idemprendedor);
            $("#idproy").val(idproy);
            $("#proyecto").html($("#h4-" + idproy).html());
            $("#moneda").val($("#mon-" + idproy).val());
            $("#publicado").html($("#pubs-" + idproy).val());
            if (idprop != "0") {
                $("#idpropuesta").val(idprop);
                $("#moneda").val($("#propmoneda-" + idproy).val());
                $("#observ").val($("#propobserv-" + idproy).val());
                $("#monto").val($("#propmonto-" + idproy).val());
                $("#dias").val($("#propdias-" + idproy).val());
            }
            else
            {
                $("#idpropuesta").val("0");
                $("#observ").val("");
                $("#monto").val("");
                $("#dias").val("0");
            }
        }

        function showmodal_msg(idproy, proyecto, emp, idreceptor) {
            $("#msg-proyecto").html(proyecto);
            $("#msg-emp").html(emp);
            $("#msg-idproyecto").val(idproy);
            $("#idreceptor").val(idreceptor);
        }

        function goselcat(valor, texto) {
            var selcat = document.getElementById("selcat");
            var cate = document.getElementById("categoria");
            var arr = document.getElementsByName("tsubcat[]");
            //alert(arr.length);
            for (i = 0; i < arr.length; i++) {
                if (valor == arr.item(i).value)
                {
                    return;
                }
            }

            //selcat.innerHTML = selcat.innerHTML + "<select name='lstcat' multiple id='subcatego' size='1' ><option value='"+ valor + "'> x " + cate.options[cate.selectedIndex].text + "</option></select> ";
            selcat.innerHTML = selcat.innerHTML + "<a id='subcat-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delsel(this.id);'> x " + cate.options[cate.selectedIndex].text + "</a> <input type='hidden' id='tsub-" + valor + "' name='tsubcat[]' value='" + valor + "'>";
            //cate.selectedIndex = 0;
        }
        function delsel(valor) {
            document.getElementById(valor).remove();
            document.getElementById("tsub-" + valor.replace("subcat-", "")).remove();
        }

        var perfil = "@tperfil";
        if (perfil == "2")
        {
            document.getElementById("otroperfil").checked = "checked";
        }
        var pres = "@pres";
        if (pres != "") {
            document.getElementById("presupuesto").value = pres;
        }
        var tpago = "@tpago";
        if (tpago != "") {
            document.getElementById("tipopago").value = tpago;
        }
        var punt = "@puntua";
        if (punt != "") {
            document.getElementById("puntuacion").value = punt;
        }
        var selcat = document.getElementById("selcat");
        var selsubcat = "@Html.Raw(subcatsel)";
        if (selsubcat != "") {
            selcat.innerHTML = selsubcat;
        }

        var msgsend = "@msgsave";
        switch (msgsend) {
            case "OK": 
                $.bootstrapGrowl('Tu propuesta se envió exitosamente', {
                    type: 'success',
                    delay: 2000,
                    width: '100%', 
                    showProgressbar: true
                });
                break;
            case "ERR":
                $.bootstrapGrowl('Hubo un Error. Intentalo mas tarde.', {
                    type: 'danger',
                    delay: 2000,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "MS":
                $.bootstrapGrowl('@PageData("msg-ms")', {
                    type: 'success',
                    delay: 3500,
                    width: '100%',
                    showProgressbar: true
                });
                break;
        }

        

        @*var idempview = "@Request.Form("idempview")"
        if (idempview != "") {
            $("#modalview").modal('show');
        }*@

        function gosubmit(idemp) {
            
            //$("#modalview").modal('show');
        }
    </script>

    <script type="text/javascript">

        $(function () {
            var username = $("#iduser").val();
            var proxy = $.connection.messageHub;
            proxy.client.recibenoti = function (message, user, userenvio) {
                if (document.getElementById("iduser").value == user) {
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
                proxy.server.connect(username);
            });

            //myHub = $.connection.messagehub;
            try {
                //$.connection.hub.logging = true;
                $.connection.hub.start();
            }
            catch (e)
            {
                console.log("POOL-EMP  " + e.message);
            }

            
            $("#btnsendprop").click(function () {
                if (Number($('#monto').val()) == "0") {
                    $('#monto').focus();
                    return;
                }
                if ($('#observ').val() == "") {
                    $('#observ').focus();
                    return;
                }
                if ($('#dias').val() == "") {
                    $('#dias').focus();
                    return;
                }
                try {
                    proxy.server.senmessage($('#observ').val(), $('#idemp-proy').val(), @idemp);
                    @*alert("{idpropuesta: " + Number($('#idpropuesta').val()) + "," +
                            "idproyecto: " + Number($('#idproy').val()) + ", " +
                            "idemprendedor: " + @idemp + ", " +
                            "propuesta: '" + $('#observ').val() + "'," +
                            "monto: " + Number($('#monto').val()) + "," +
                            "moneda: '" + $('#moneda').val() + "'," +
                            "tyc: 1," +
                            "dias:" + Number($('#dias').val()) + "}");*@
                    $.ajax({
                        type: 'POST',
                        url: 'wspropuestas.asmx/Crear_Propuesta',
                        data: "{idpropuesta: " + Number($('#idpropuesta').val()) + "," +
                            "idproyecto: " + Number($('#idproy').val()) + ", " +
                            "idemprendedor: " + @idemp + ", " +
                            "propuesta: '" + $('#observ').val() + "'," +
                            "monto: " + Number($('#monto').val()) + "," +
                            "moneda: '" + $('#moneda').val() + "'," +
                            "tyc: 1," +
                            "dias:" + Number($('#dias').val()) + "}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
                            //$('#formpropuesta').attr('action', 'user-pool#' + $("#idproy").val());
                            $('#formpropuesta').submit();
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
                if ($('#mensaje').val() == "") {
                    $('#mensaje').focus();
                    return;
                }

                try {
                    proxy.server.senmessage($('#mensaje').val(), $('#idreceptor').val(), $('#iduser').val());
                    $.ajax({
                        type: 'POST',
                        url: 'wsmensajes.asmx/grabamsg',
                        data: "{user: '" + @idemp + "', userenvia: '"+ $('#idreceptor').val() + "', asunto:'', mensaje: '" + $('#mensaje').val() + "', idmsgpadre: '0', idproyecto:'"+ $("#msg-idproyecto").val() + "'}",
                        contentType: 'application/json; utf-8',
                        dataType: 'json',
                        success: function (data) {
                            //$('#formmsg').attr('action', 'user-pool#' + $("#msg-idproyecto").val());
                            //alert("MENSAJE ENVIADO CON ÉXITO.");
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
        ///*make_msg*/($("#iduser").val());
    </script>


End Section
