@functions
    Private Sub Agenda_Cita(ByVal idemp As Long, ByVal strConn As String)

        Dim objProy As New poolin_class.cProyectos
        Dim dt As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(0, strConn, Request.Form("proyecto"))

        If dt.Rows.Count = 0 Then
            Exit Sub
        End If


        Dim c As New poolin_class.cAgenda
        c.id = 0
        c.fechahora = Request.Form("fechahora")
        c.duracion = 60
        c.motivo = Mid("CITA / " & Request.Form("proyecto"), 1, 100)
        c.idemprendedor = idemp

        If dt.Rows(0)("idcliente") <> idemp Then
            c.idreceptor = dt.Rows(0)("idcliente")
        Else
            c.idreceptor = dt.Rows(0)("idemprendedor")
        End If

        c.idproyecto = Request.Form("proyecto")
        c.descripcion = Request.Form("asunto")
        c.estatus = ""
        c.Agenda_Cita(strConn)
    End Sub

End Functions

@code
    PageData("pagina") = "user-calendario"
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Calendario"
    PageData("opcal") = "active"

    Dim idemp As Long = 0
    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try


    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Dim objagenda As New poolin_class.cAgenda
    Dim msgsave As String = ""

    Validation.RequireField("proyecto", "<br>Requerido")
    Validation.RequireField("asunto", "<br>Requerido")
    Validation.RequireField("fechahora", "<br>Requerido")

    If Request.Form("save") = "OK" Then
        Try
            Agenda_Cita(idemp, strConn)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End If

    If Request.Form("idagenda") <> "" Then
        Try
            objagenda.Actualiza_Estatus(Request.Form("idagenda"), Request.Form("estatus"), strConn)
            msgsave = "UP"
        Catch ex As Exception
            msgsave = "ERR"
        End Try

    End If

    Dim agenda As String = objagenda.Carga_Agenda_Vm(idemp, strConn)

    Dim objProy As New poolin_class.cProyectos
    Dim dtProy As System.Data.DataTable = objProy.Carga_Proyectos_EnEjecucion(idemp, strConn)
    Dim idagenda As Long = 0
    Dim programada As String = ""
    Dim fechahora As String = ""
    Dim asunto As String = ""
    Dim proyecto As String = ""
    Dim idempagendo As Long = 0
    Dim objComun As New poolin_class.cComunes
    If Request.QueryString("idagenda") <> "" And Request.Form("idagenda") = "" Then
        Try
            idagenda = objComun.Decrypt(Request.QueryString("idagenda"))
            Dim dtcita As System.Data.DataTable = objagenda.Carga_Cita(idagenda, idemp, strConn)
            If dtcita.Rows.Count <> 0 Then
                programada = dtcita.Rows(0)("agendo")
                proyecto = dtcita.Rows(0)("proyecto")
                fechahora = Format(dtcita.Rows(0)("fecha"), "dd/MM/yyyy HH:mm")
                asunto = dtcita.Rows(0)("descripcion")
                idempagendo = dtcita.Rows(0)("idemprendedor")
            End If
        Catch ex As Exception
            idagenda = 0
        End Try
    End If

    'Dim fstrem As New System.IO.StreamWriter(Server.MapPath("poolin_arch/calendario/" & idemp & "/eventos.json.php").ToString)
    'fstrem.Write(agenda)
    'fstrem.Close()

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-calendario", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

End Code
@section head
    <link rel="stylesheet" href="css/calendar.css">

End Section
<div class="modal fade" id="modaledit" tabindex="-1" role="dialog" aria-labelledby="modaleditcita" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modaledit-1") - @Html.Raw(proyecto)</h3>
            </div>
            <div class="modal-body">
                <form id="formedit" method="post" action="">
                    <input type="hidden" name="edit" value="OK" />
                    <input type="hidden" name="idagenda" value="@idagenda"/>
                    <input type="hidden" id="estatus" name="estatus" />
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-6">
                                <label>@PageData("modaledit-2")</label>
                                <input readonly type="text" class="form-control" value="@programada" />
                            </div>
                            <div class="col-lg-6">
                                <label>@PageData("modaledit-3")</label>
                                <input readonly type="text" class="form-control" value="@fechahora" />
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <label>@PageData("modaledit-4")</label>
                                <textarea readonly type="text" class="form-control" rows="3">@Html.Raw(asunto)</textarea>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                @if idempagendo<>idemp Then
                                    @<button type="button" class="btn btn-primary pull-right" onclick="updatecita('O')">@PageData("btnaceptar")</button>     
                                End if
                                <button type="button" class="btn btn-danger pull-right" onclick="updatecita('C')">@PageData("btnocupado")</button>     
                                <button type="button" class="btn btn-default pull-right"  data-dismiss="modal">@PageData("btncerrar")</button>     
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="modaladd" tabindex="-1" role="dialog" aria-labelledby="modaladdcita"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3 class="modal-title">@PageData("modaladd-1")</h3>
            </div>
            <div class="modal-body">
                <form id="formcita" method="post" action="">
                    <input type="hidden" name="save" value="OK" />
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-lg-12">
                                <label>@PageData("modaladd-2")</label>
                                <span class="msg-val">@Html.ValidationMessage("proyecto")</span>
                                <select name="proyecto" class="form-control" @Validation.For("proyecto") >
                                    <option value="" selected disabled>@PageData("modaladd-3")</option>
                                    @for Each dr As System.Data.DataRow In dtProy.Rows
                                        @<option value="@dr("idproyecto")">@Html.Raw(dr("nombre"))</option>
                                    Next
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                <label>@PageData("modaladd-4")</label>
                                <span class="msg-val">@Html.ValidationMessage("asunto")</span>
                                <textarea type="text" Class="form-control" name="asunto" @Validation.For("asunto")></textarea>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-6">
                                <label>@PageData("modaladd-5")</label>
                                <span class="msg-val">@Html.ValidationMessage("fechahora")</span>
                                <input type = "datetime-local" Class="form-control" id="fechahora" name="fechahora" value="@Format(Now.AddHours(1), "yyyy-MM-ddTHH:00")"  @Validation.For("fechahora")/>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div Class="row">
                            <div Class="col-lg-12">
                                <Button type = "submit" Class="btn btn-primary pull-right">@PageData("btnenviar")</button>&nbsp;&nbsp;      
                                <Button type = "button" Class="btn btn-default pull-right" data-dismiss="modal">@PageData("btncerrar")</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>


<div id="content">
    <h3 Class="user_title">@PageData("titulo")</h3>
    <div class="btn-group">
        <button Class="btn btn-primary" data-calendar-nav="prev">< @PageData("subtit-1")</button>
        <button Class="btn btn-default" data-calendar-nav="today">@PageData("subtit-2")</button>
        <button Class="btn btn-primary" data-calendar-nav="next">@PageData("subtit-3") ></button>
        <!-- <button class="btn btn-warning" data-calendar-view="year">Año</button> -->
        <button Class="btn btn-warning" data-calendar-view="day">@PageData("subtit-4")</button>
        <button Class="btn btn-warning" data-calendar-view="week">@PageData("subtit-5")</button>
        <button Class="btn btn-warning active" data-calendar-view="month">@PageData("subtit-6")</button>
        <button class="btn btn-primary" type="button" data-toggle="modal" data-target="#modaladd" data-calendar-view="month">@PageData("btnagendar")</button>
    </div>
    <h3 Class="titlecalendar"></h3>
    <div id="calendar"></div>
</div>
@section Scripts

    <script src="Scripts/jquery.validate.min.js"></script>
    <script src="Scripts/jquery.validate.unobtrusive.min.js"></script>

    <Script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></Script>
    <Script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></Script>

    <script type="text/javascript" src="components/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="components/underscore/underscore-min.js"></script>
    <script type="text/javascript" src="components/bootstrap3/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="components/jstimezonedetect/jstz.min.js"></script>
    <script type="text/javascript" src="js/language/es-MX.js"></script>
    <script type="text/javascript" src="js/calendar.js"></script>

    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>


    @*<script type="text/javascript" src="js/bootstrap-datetimepicker.js"></script>*@
    <script>

        var options = {
            events_source: [
                @Html.Raw(agenda)
            ],
            view: 'month',
            tmpl_path: 'tmpls/',
            tmpl_cache: false,
            language: 'es-MX',
            //day: '2018-04-01',
            onAfterEventsLoad: function (events) {
                if (!events) {
                    return;
                }
                var list = $('#eventlist');
                list.html('');

                $.each(events, function (key, val) {
                    $(document.createElement('li'))
                        .html('<a href="' + val.url + '">' + val.title + '</a>')
                        .appendTo(list);
                });
            },
            onAfterViewLoad: function (view) {
                $('h3.titlecalendar').text(this.getTitle());
                $('.btn-group button').removeClass('active');
                $('button[data-calendar-view="' + view + '"]').addClass('active');
            },
            classes: {
                months: {
                    general: 'label'
                }
            }
        };

        var calendar = $('#calendar').calendar(options);

        $('.btn-group button[data-calendar-nav]').each(function () {
            var $this = $(this);
            $this.click(function () {
                calendar.navigate($this.data('calendar-nav'));
            });
        });

        $('.btn-group button[data-calendar-view]').each(function () {
            var $this = $(this);
            $this.click(function () {
                calendar.view($this.data('calendar-view'));
            });
        });

        $('#first_day').change(function () {
            var value = $(this).val();
            value = value.length ? parseInt(value) : null;
            calendar.setOptions({ first_day: value });
            calendar.view();
        });

        $('#language').change(function () {
            calendar.setLanguage($(this).val());
            calendar.view();
        });

        $('#events-in-modal').change(function () {
            var val = $(this).is(':checked') ? $(this).val() : null;
            calendar.setOptions({ modal: val });
        });
        $('#format-12-hours').change(function () {
            var val = $(this).is(':checked') ? true : false;
            calendar.setOptions({ format12: val });
            calendar.view();
        });
        $('#show_wbn').change(function () {
            var val = $(this).is(':checked') ? true : false;
            calendar.setOptions({ display_week_numbers: val });
            calendar.view();
        });
        $('#show_wb').change(function () {
            var val = $(this).is(':checked') ? true : false;
            calendar.setOptions({ weekbox: val });
            calendar.view();
        });
        $('#modaladd .modal-header, #modaladd .modal-footer').click(function (e) {
            //e.preventDefault();
            //e.stopPropagation();
        });

        var msgsend = "@msgsave";
        switch (msgsend)
        {
            case "OK":
                $.bootstrapGrowl('@PageData("msg-ok")', {
                    type: 'success',
                    delay: 2000,
                    width: '100%',
                    showProgressbar: true
                });
                break;
            case "UP":
                $.bootstrapGrowl('@PageData("msg-up")', {
                    type: 'info',
                    delay: 2000,
                    width: '100%',
                    showProgressbar: true
                    });
        
                break;
            case "ERR":
                $.bootstrapGrowl('@PageData("msg-err")', {
                    type: 'danger',
                    delay: 2000,
                    width: '100%',
                    showProgressbar: true
                });
                break;
        }

        var showmodal = "@idagenda";
        if (showmodal != "0") {
            $('#modaledit').modal('show');
        }

        function updatecita(valor)
        {
            document.getElementById("estatus").value = valor;
            document.getElementById("formedit").submit();
        }

    </script>

End Section