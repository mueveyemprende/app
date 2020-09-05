@code
    PageData("pagina") = "micuenta"
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
    PageData("Title") = "Mi Cuenta"
    PageData("hiddemenu") = ""

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString
    Dim objEmp As New poolin_class.cEmprendedor
    Dim objComun As New poolin_class.cComunes

    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "micuenta", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    Dim msgsave As String = ""
    Try
        Select Case Request.Form("save")
            Case "del-tc"
                objEmp.Borra_tdc(Request.Form("idtc"), strConn)
                msgsave = "OK-DEL"
            Case "del-depo"
                objEmp.Borra_Ctadeposito(idemp, strConn)
                msgsave = "OK-DEL"
            Case "datos"
                objEmp.Update_MiCuenta(idemp, Trim(Request.Form("nombres")),
                                       Request.Form("pwd"), Request.Form("pais"),
                                       Request.Form("moneda"), strConn)
                msgsave = "OK"
            Case "fact"
                Dim tyfact As New poolin_class.ctypedatosfact
                tyfact.id = Request.Form("idfact")
                tyfact.calle = Request.Form("direccion")
                tyfact.RFC = Request.Form("rfc")
                tyfact.razonsocial = Request.Form("razon")
                tyfact.email = Request.Form("email")
                tyfact.idemprendedor = idemp
                tyfact.nomcomercial = ""

                tyfact.numext = ""
                tyfact.numint = ""
                tyfact.colonia = ""
                tyfact.localidad = ""
                tyfact.referencia = ""
                tyfact.municipio = ""
                tyfact.idpais = 0
                tyfact.idestado = 0
                tyfact.codigopostal = ""
                objEmp.Grabar_DatosFact(strConn, tyfact)
                msgsave = "OK"
            Case "TC"
                Dim tipo As String = ""
                Dim numtc As String = Request.Form("numtc1") & "|" & Request.Form("numtc2") & "|" & Request.Form("numtc3") & "|" & Request.Form("numtc4")
                Select Case Mid(numtc, 1, 1)
                    Case "3"
                        Select Case Mid(numtc, 1, 2)
                            Case "34", "35", "36", "37"
                                tipo = "amex"

                        End Select
                    Case "4"
                        tipo = "visa"
                    Case "5"
                        Select Case Mid(numtc, 1, 2)
                            Case "51", "52", "53", "54", "55"
                                tipo = "mastercard"
                        End Select
                End Select
                objEmp.tc(0, idemp, Request.Form("nomtc"), numtc, "", Request.Form("nummes"), Request.Form("numano"), tipo, strConn)
                msgsave = "OK"
            Case "cerrar"
                Dim objAdmin As New poolin_class.cAdmin
                Dim dtPwd As System.Data.DataTable = objEmp.Datos_Emprendedor(idemp, strConn)
                Dim cPWD As String = objComun.Decrypt(dtPwd.Rows(0)("pwd"))
                If Request.Form("passclose") = cPWD Then
                    objAdmin.Emprendedor_Status(idemp, "E", strConn)
                    Response.Redirect("https://mueveyemprende.io/eliminada", False)
                Else
                    msgsave = "ERRPWD"
                End If
            Case "depos"
                objEmp.Ctadeposito(0, idemp,
                                   Request.Form("nomdepos"),
                                   Request.Form("numcard"),
                                   Request.Form("tipodepos"),
                                   Request.Form("banco"), strConn)
                msgsave = "OK"
        End Select

    Catch ex As Exception
        msgsave = "ERR"
    End Try

    Dim dtEmp As System.Data.DataTable = objEmp.MiCuenta_Emp(idemp, strConn)
    If dtEmp.Rows.Count = 0 Then
        Response.Redirect("https://mueveyemprende.io/", False)
    End If
    Dim pwd As String = objComun.Decrypt(dtEmp.Rows(0)("pwd"))
    Dim dtpais As System.Data.DataTable = objComun.Carga_Paises(strConn)
    Dim dtMonedas As System.Data.DataTable = objComun.Carga_Monedas(strConn)
    Dim dtFact As System.Data.DataTable = objEmp.MiCuenta_Fact(idemp, strConn)
    Dim dtTC As System.Data.DataTable = objEmp.Carga_TC(idemp, strConn)
    Dim dtBancos As System.Data.DataTable = objEmp.Bancos(strConn)
    Dim dtCtaDepos As System.Data.DataTable = objEmp.Carga_CtaDeposito(idemp, strConn)

    Validation.RequireField("nombres", "<br>" & PageData("val-campo1"))
    Validation.RequireField("pwd", "<br>" & PageData("val-campo1"))
    Validation.RequireField("conf-pwd", "<br>" & PageData("val-campo1"))
    Validation.Add("conf-pwd", Validator.EqualsTo("pwd", PageData("val-campo2")))

    Validation.RequireField("rfc", "<br>" & PageData("val-campo1"))
    Validation.RequireField("razon", "<br>" & PageData("val-campo1"))
    Validation.RequireField("direccion", "<br>" & PageData("val-campo1"))
    Validation.RequireField("email", "<br>" & PageData("val-campo1"))

    Validation.RequireField("numtc1", "<br>" & PageData("val-campo1"))
    Validation.Add("numtc1", Validator.Integer("Solo numeros"))

    Validation.RequireField("numtc2", "<br>" & PageData("val-campo1"))
    Validation.Add("numtc2", Validator.Integer("Solo numeros"))
    Validation.RequireField("numtc3", "<br>" & PageData("val-campo1"))
    Validation.Add("numtc3", Validator.Integer("Solo numeros"))
    Validation.RequireField("numtc4", "<br>" & PageData("val-campo1"))
    Validation.Add("numtc4", Validator.Integer("Solo numeros"))

    Validation.RequireField("nomtc", "<br>" & PageData("val-campo1"))
    Validation.RequireField("nummes", PageData("val-campo1"))
    Validation.RequireField("numano", PageData("val-campo1"))

    Validation.RequireField("tipodepos", PageData("val-campo1"))
    Validation.RequireField("banco", PageData("val-campo1"))
    Validation.RequireField("nomdepos", PageData("val-campo1"))
    Validation.RequireField("numcard", PageData("val-campo1"))

    Validation.RequireField("passclose", PageData("val-campo1"))

    Dim rfc = ""
    Dim razon = ""
    Dim direccion = ""
    Dim email = ""
    Dim idfact = 0

    Dim nomdepos = ""
    Dim numdepos = ""
    Dim tipodepos = ""
    Dim banco = ""
    Dim nombanco = ""
    Dim tipoctadepos = ""

    If dtCtaDepos.Rows.Count <> 0 Then
        nomdepos = dtCtaDepos.Rows(0)("nombre")
        numdepos = dtCtaDepos.Rows(0)("numcuenta")
        tipodepos = dtCtaDepos.Rows(0)("tipo")
        banco = dtCtaDepos.Rows(0)("idbanco")
        nombanco = dtCtaDepos.Rows(0)("banco")
        Select Case tipodepos
            Case "CLABE"
                tipoctadepos = "CLABE INTERBANCARIA"
            Case "CRED"
                tipoctadepos = "TARJETA CRÉDITO"
            Case "DEBI"
                tipoctadepos = "TARJETA DÉBITO"
            Case "PAYPAL"
                tipoctadepos = "PAYPAL"
            Case "MPAGO"
                tipoctadepos = "MERCADO PAGO"
        End Select
    End If

    If dtFact.Rows.Count <> 0 Then
        idfact = dtFact.Rows(0)("id")
        rfc = dtFact.Rows(0)("rfc")
        razon = dtFact.Rows(0)("razonsocial")
        direccion = dtFact.Rows(0)("calle")
        email = "" & dtFact.Rows(0)("email")
    End If
End Code
@section head
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
End Section

    <div class="modal fade" id="modaldeposito"  tabindex="-1" role="dialog" aria-labelledby="modaldeposito"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <span class="tabs-title">@PageData("modaldeposito1")</span>
                </div>
                <form id="formaddtc" method="post" action="~/micuenta">
                    <input type="hidden" name="save" value="TC" />
                    <div class="modal-body">
                        <div Class="row">
                            <div class="col-lg-12">
                                <label>@PageData("modaldeposito2")</label>
                                <select id="tipocta" num="tipocta" class="form-control">
                                    <option value="" disabled selected></option>
                                    <option value="CRED">T. Crédito</option>
                                    <option value="DEBI">T. Débito</option>
                                    <option value="CLABE">CLABE Inter.</option>
                                </select>
                            </div>
                            <div class="col-lg-12">
                                <label>@PageData("modaldeposito3")</label>
                            </div>
                            <div class="col-lg-12">
                                <label>@PageData("modaldeposito4")</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <button type="submit" class="save-btn pull-right">@PageData("btnsave")</button>
                                <button class="cancel-btn pull-right" data-dismiss="modal">@PageData("btncancel")</button>
                            </div>
                        </div>

                    </div>
                </form>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modaltc"  tabindex="-1" role="dialog" aria-labelledby="modaltc"  aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <span class="tabs-title">@PageData("modaltc1")</span>
                </div>
                <form id="formaddtc" method="post" action="~/micuenta">
                    <input type="hidden" name="save" value="TC" />
                    <div class="modal-body">
                        <div Class="row">
                            <div class="col-lg-12">
                                <label>@PageData("modaltc2")</label>
                                <span class="msg-val">@Html.ValidationMessage("nomtc")</span>
                                <input type="text" name="nomtc" id="nomtc" class="form-control" @Validation.For("nomtc")>
                            </div>
                            <div class="col-lg-6">
                                <label>@PageData("modaltc3")</label>
                                <span class="msg-val">@Html.ValidationMessage("numtc1")</span>
                                <span class="msg-val">@Html.ValidationMessage("numtc2")</span>
                                <span class="msg-val">@Html.ValidationMessage("numtc3")</span>
                                <span class="msg-val">@Html.ValidationMessage("numtc4")</span>
                                <div class="form-inline">
                                    <input type="text" name="numtc1" id="numtc1" maxlength="4" size="4" onchange="changetype(1)" onfocus="fchangetype(1)" onfocusout="changetype(1)" class="form-control" @Validation.For("numtc1")>
                                    <input type="text" name="numtc2" id="numtc2" maxlength="4" size="4" onchange="changetype(2)" onfocus="fchangetype(2)" onfocusout="changetype(2)" class="form-control" @Validation.For("numtc2")>
                                    <input type="text" name="numtc3" id="numtc3" maxlength="4" size="4" onchange="changetype(3)" onfocus="fchangetype(3)" onfocusout="changetype(3)" class="form-control" @Validation.For("numtc3")>
                                    <input type="text" name="numtc4" id="numtc4" maxlength="4" size="4"  class="form-control" @Validation.For("numtc4")>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <img id="tc-mc" src="images/mastercard.png" class="inline" style="width:25%">
                                <img id="tc-vi" src="images/visa.png" class="inline" style="width:25%">
                                <img id="tc-am" src="images/amex.png" class="inline" style="width:25%">
                            </div>
                            <div class="col-lg-3">
                                @*<input type="number" name="nummes" maxlength="2" id="nummes" class="form-control" @Validation.For("nummes")>*@
                                <label>@PageData("modaltc4")</label>
                                <select name="nummes" class="form-control" @Validation.For("nummes")>
                                    <option value="" selected disabled>MES</option>
                                    <option value="01">01</option>
                                    <option value="02">02</option>
                                    <option value="03">03</option>
                                    <option value="04">04</option>
                                    <option value="05">05</option>
                                    <option value="06">06</option>
                                    <option value="07">07</option>
                                    <option value="08">08</option>
                                    <option value="09">09</option>
                                    <option value="10">10</option>
                                    <option value="11">11</option>
                                    <option value="12">12</option>
                                </select>
                                <span class="msg-val">@Html.ValidationMessage("nummes")</span>
                            </div>
                            <div class="col-lg-3">
                                <label>@PageData("modaltc5")</label>
                                <select name="numano" class="form-control" @Validation.For("numano")>
                                    <option value="" selected disabled>AÑO</option>
                                    @for inti As Integer = Now.Year To Now.AddYears(23).Year
                                        @<option value="@inti">@inti</option>
                                    Next
                                </select>
                                <span class="msg-val">@Html.ValidationMessage("numano")</span>
                            </div>
                            <div class="col-lg-12">
                                <button type="submit" class="save-btn pull-right">@PageData("btnsave")</button>
                                <button class="cancel-btn pull-right" data-dismiss="modal">@PageData("btncancel")</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
<div id="cerrar-cuenta" class="modal fade centered-modal" role="dialog"  aria-labelledby="cerrar-cuenta"  aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h3>@PageData("cerrarcuenta1")<br>@PageData("cerrarcuenta2")</h3>
            </div>
            <div class="modal-body">
                <form id="frmclose" action="~/micuenta" method="post">
                    <input type="hidden" name="save" value="cerrar" />
                    <div class="row">
                        <div class="col-lg-3">
                        </div>
                        <div class="col-lg-6">
                            <input type="password" name="passclose" placeholder="@PageData("pholdercerrar1")" class="form-control" @Validation.For("passclose")>
                            <span class="msg-val">@Html.ValidationMessage("passclose")</span>
                        </div>
                        <div class="col-lg-3">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <button type="submit" class="save-btn pull-right">@PageData("btncerrarcta")</button>
                            <button class="cancel-btn pull-right" data-dismiss="modal">@PageData("btncancel")</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
<div id="content">
    <div class="container">
        <h3 class="cuenta_title">@PageData("titulo")</h3>
                <div class="row">
                    <div class="col-md-12">
        <div class="tabbable">
            <div class="tab-content">
            <ul class="nav nav-tabs">
                <li class="active"><a href="#info" data-toggle="tab">@PageData("li-1")</a></li>
                <li class="hide"><a href="#membresia" data-toggle="tab">@PageData("li-2")</a></li>
                <li><a href="#pago" data-toggle="tab">Cuenta para depósitos</a></li>
                <li><a href="#factura" data-toggle="tab">@PageData("li-4")</a></li>
                <li><a href="#seguridad" data-toggle="tab">@PageData("li-5")</a></li>
            </ul>

                <div class="tab-pane active" id="info">
                    <div class="cuenta-box">
                        <h2 class="tabs-title"> @PageData("subtitulo-1")  <a href="javascript:;" id="edit-datos"><i class="fas fa-edit" style="font-size:.5em; vertical-align:top;"></i></a></h2>
                        
                        <form id="frm-datos" method="post" action="~/micuenta">
                            <input type="hidden" name="save" value="datos" />
                            <div class="fields_wrap">
                                <div class="row border-btm">
                                    <div class="col-md-4 col-lg-4 col-xl-4">
                                        <div class="title-info-fields"><label>@PageData("info-1")</label></div>
                                        <div class="info-fields">@Html.Raw(dtEmp.Rows(0)("email"))</div>
                                    </div>
                                    <div class="col-md-2 col-lg-2 col-xl-2">
                                        <div class="title-info-fields"><label>@PageData("info-2")</label> </div>
                                        <span class="msg-val">@Html.ValidationMessage("pwd")</span>
                                        <div class="info-fields">
                                            <input class="info-contra" id="pwd" name="pwd" type="password"  readonly value="@Html.Raw(pwd)" @Validation.For("pwd")>
                                        </div>
                                    </div>
                                    <div class="col-md-2 col-lg-2 col-xl-2">
                                        <div class="title-info-fields"><label>@PageData("info-3")</label> </div>
                                        <span class="msg-val">@Html.ValidationMessage("conf-pwd")</span>
                                        <div class="info-fields">
                                            <input class="info-contra" id="conf-pwd" name="conf-pwd" type="password"  readonly value="@Html.Raw(pwd)" @Validation.For("conf-pwd")>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="fields_wrap">
                                <div class="row">
                                    <div class="col-md-4 col-lg-4 col-xl-6">
                                        <div class="title-info-fields"><label>@PageData("info-4")</label></div>
                                        <span class="msg-val">@Html.ValidationMessage("nombres")</span>
                                        <div class="info-fields">
                                            <input class="info-contra" id="nombres" name="nombres" readonly type="text" value="@Html.Raw(dtEmp.Rows(0)("nombres"))" @Validation.For("nombres") >
                                        </div>
                                    </div>
                                    <div class="col-md-2 col-lg-2 col-xl-2">
                                        <div class="title-info-fields"><label>@PageData("info-5")</label></div>
                                        <div class="info-fields">
                                            <select class="info-contra" id="pais" name="pais" disabled>
                                                @for Each dr As System.Data.DataRow In dtpais.Rows
                                                    @<option value="@dr("id")" selected>@Html.Raw(dr("nombre"))</option>
                                                Next
                                            </select>
                                            <script>
                                                document.getElementById("pais").value="@dtEmp.Rows(0)("idpais")";
                                            </script>
                                        </div>
                                    </div>
                                    <div Class="col-md-2 col-lg-2 col-xl-2" >
                                        <div Class="title-info-fields"><label>@PageData("info-6")</label> </div>
                                        <div Class="info-fields">
                                            <select class="info-contra" id="moneda" name="moneda" disabled>
                                                @for Each dr As System.Data.DataRow In dtMonedas.Rows
                                                    @<option value="@dr("moneda")">@dr("moneda")</option>
                                                Next
                                            </select>
                                            <script>
                                                document.getElementById("moneda").value = "@dtEmp.Rows(0)("moneda")";
                                            </script>
                                        </div>
                                    </div>
                                    <div id="save-datos" class="hidden">
                                        <div class="col-md-3 col-lg-3 col-xl-3">
                                            <button type="button" class="btn-defsave" onclick="noedit('datos')">@PageData("btncancel")</button>
                                            <button type="submit" id="btnsave" class="btn-prisave">@PageData("btnsave")</button>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </form>
                    </div>
                </div>
                <div class="tab-pane" id="membresia">
                    <div class="cuenta-box">
                        <div class="col-md-3">
                            <h2 class="tabs-title"> @PageData("subtitulo-2")</h2>
                            <p>@PageData("mem-1"): Gratis $0 / mes<br>@PageData("mem-2"): -<br></p>
                            <p>@PageData("mem-3") @Format(dtEmp.Rows(0)("fecha_creacion"), "d - MM - yyyy")</p>
                        </div>
                        <div class="col-md-9">
                            <img src="~/Images/membresia.png" alt="Membresía" class="img-membresia" />
                            <span class="text-membresia"    >Estamos trabajando<br />para ofrecerte nuevos servicios.</span>
                        </div>
                    </div>
                </div>
                <div Class="tab-pane" id="pago">
                    <div Class="cuenta-box hide">
                        @*<div Class="historial pull-right"><a href="#">Ver Historial de movimientos</a></div>*@
                        <h2 Class="tabs-title"> @PageData("subtitulo-3")</h2>
                        @for Each drtc As System.Data.DataRow In dtTC.Rows
                            Dim tipotc = drtc("tipo") & ".png"
                            @<div Class="col-md-4">
                                <div Class="pay-box">
                                    <p> @PageData("fpago-1") </p>
                                    <div Class="gateway">
                                        @drtc("nombre")<br />
                                        <img src = "images/@tipotc" width="50" Class="inline">
                                        <input class="info-contra" type="password" value="@drtc("numero").ToString.Replace("|", "")" readonly>
                                    </div>
                                    <form id="frmdel-tc" method="post" action="~/micuenta">
                                        <input type="hidden" name="idtc" value="@drtc("id")" />
                                        <input type="hidden" name="save" value="del-tc" />
                                        <a href="javascript:;" onclick="document.getElementById('frmdel-tc').submit();"  class="pull-right"><i Class="fas fa-trash"></i></a>
                                    </form>
                                </div>
                            </div>

                        Next
                        <a href = "javascript:;" Class="save-btn pull-right"  data-toggle="modal" data-target="#modaltc">@PageData("btnaddtarj")</a>
                    </div>
                    <div class="cuenta-box">
                        <h2 Class="tabs-title"> @PageData("subtitulo-3.1") <a href="javascript:;" id="edit-depos"><i Class="fas fa-edit"  style="font-size:.5em; vertical-align:top;"></i></a></h2>
                        <div class="row">
                            <div class="col-lg-6 col-md-6">
						        <div id="div-depos" class="pay-box" style="overflow-x:auto;">
                                    <p><label> @PageData("depo-1")</label><span class="info-fields">@Html.Raw(nomdepos)</span></p>
                                    <p> <Label> @PageData("depo-2")</label> <span class="info-fields">@Html.Raw(nombanco)</span></p>
							        <p> <Label> @PageData("depo-3")</label> <span class="info-fields">@Html.Raw(tipoctadepos)</span></p>
							        <p> <Label> @PageData("depo-4")</label> <span class="info-fields">@Html.Raw(numdepos)</span></p>
                                    <form method="post" action="~/micuenta" id="frmdel-depo">
                                        <input type="hidden" name="save" value="del-depo"/>
                                        <a href="javascript:;" onclick="document.getElementById('frmdel-depo').submit();" class="pull-right"><i Class="fas fa-trash"></i></a>
                                    </form>
						        </div>
                            </div>
                        </div>
                        <form id="frm-depos" action="micuenta" method="post" class="hidden">
                            <input type="hidden" name="save" value="depos" />
                            <div class="row form-group">
                                <div class="col-lg-3">
                                    <div Class="title-info-fields"><label>@PageData("depolabel-1")</label></div>
                                    <span class="msg-val">@Html.ValidationMessage("tipodepos")</span>
                                    <select name="tipodepos" id="tipodepos" class="form-control"  onchange="cambiaformato()" @Validation.For("tipodepos")>
                                        <option value="" disabled selected></option>
                                        <option value="CRED">@PageData("optarj-1")</option>
                                        <option value="DEBI">@PageData("optarj-2")</option>
                                        <option value="CLABE">@PageData("optarj-3")</option>
                                        <option value="PAYPAL">Paypal</option>
                                        <option value="MPAGO">Mercado Pago</option>
                                    </select>
                                </div>
                            </div>
                            <div class="row form-group">
                                <div class="col-lg-3">
                                    <div Class="title-info-fields"><label>@PageData("depolabel-2")</label></div>
                                    <span class="msg-val">@Html.ValidationMessage("banco")</span>
                                    <select name="banco" id="banco" class="form-control" @Validation.For("banco")>
                                        <option value="" disabled selected></option>
                                        @for Each drBancos As System.Data.DataRow In dtBancos.Rows
                                            @<option value="@drBancos("id")">@Html.Raw(drBancos("nombre"))</Option>
                                        Next
                                    </select>
                                </div>
                            </div>
                            <div class="row form-group">
                                <div class="col-lg-6">
                                    <div Class="title-info-fields"><label>@PageData("depolabel-3")</label></div>
                                    <span class="msg-val">@Html.ValidationMessage("nomdepos")</span>
                                    <input type="text" class="form-control" name="nomdepos" id="nomdepos" @Validation.For("nomdepos") />
                                </div>
                            </div>

                            <div class="row form-group">
                                <div class="col-lg-3">
                                    <div Class="title-info-fields"><label>Numero de Cuenta</label></div>
                                    <span class="msg-val">@Html.ValidationMessage("numcard")</span>
                                    <input type="text" name="numcard" id="numcard" size="16" maxlength="16" class="form-control" placeholder="Numero de Cuenta" value="" @Validation.For("numcard")>
                                </div>
                            </div>

                            <div class="row form-group">
                                <div Class="col-lg-3">
                                    <Button type="button" Class="btn-defsave" tabindex="0" onclick="noedit('depos')" >Cerrar</button>
                                    <Button type="submit" id="btm-tax" Class="btn-prisave" tabindex="0">@PageData("btnsave")</button>
                                </div>
                            </div>
                            <script>
                                document.getElementById("tipodepos").value = "@tipodepos";
                                document.getElementById("banco").value = "@banco";
                                document.getElementById("numcard").value = "@numdepos";
                                document.getElementById("nomdepos").value = "@nomdepos";
                            </script>

                        </form>
                    </div>
                </div>
                <div Class="tab-pane" id="factura">
                    <div Class="cuenta-box">
                        <h2 Class="tabs-title"> Facturación <a href="javascript:;" id="edit-tax"><i Class="fas fa-edit"  style="font-size:.5em; vertical-align:top;"></i></a></h2>
                        <div style="padding-bottom:20px"><small>Mueve y Emprende facturará la comisión por administración. Revisa <a href="~/me-terminos" target="_blank">Términos y Condiciones</a>.</small></div> 
						<div id = "div-tax"  style="overflow-x:auto">
                            <p><label>@PageData("factlabel-1")</label><span class="info-fields">@Html.Raw(razon)</span></p>
                             <p> <Label> @PageData("factlabel-2")  </label> <span class="info-fields">@Html.Raw(rfc)</span></p>
							<p> <Label> @PageData("factlabel-3")</label> <span class="info-fields">@Html.Raw(direccion)</span></p>
							<p> <Label> @PageData("factlabel-4")</label> <span class="info-fields">@Html.Raw(email)</span></p>
						</div>
                        <form id="frm-tax" action="micuenta" method="post" Class="hidden">
                            <input type = "hidden" name="save" value="fact" />
                            <input type = "hidden" name="idfact" value="@idfact" />
                            <div Class="row">
                                <div Class="col-lg-6">
                                    <Label> @PageData("factlabel-1")</label>
                                    <span Class="msg-val">@Html.ValidationMessage("razon")</span>
                                    <span Class="info-fields"><input type="text" name="razon" id="razon" class="form-control" placeholder="Razón Social"  value="@Html.Raw(razon)"  @Validation.For("razon")/></span>
                                </div>
                            </div>
                            <div Class="row">
                                <div Class="col-lg-4">
                                    <Label> @PageData("factlabel-2")</label> 
                                    <span Class="msg-val">@Html.ValidationMessage("rfc")</span>
                                    <span Class="info-fields"><input type="text" name="rfc" id="rfc" class="form-control" placeholder="RFC" value="@Html.Raw(rfc)"  @Validation.For("rfc") /></span>
                                </div>
                            </div>
                            <div Class="row">
                                <div Class="col-lg-6">
                                    <Label> @PageData("factlabel-3")</label>
                                    <span Class="msg-val">@Html.ValidationMessage("direccion")</span>
                                    <span Class="info-fields"><textarea name="direccion"  id="direccion" class="form-control" rows="4" placeholder="Dirección" @Validation.For("direccion")>@Html.Raw(direccion)</textarea></span>
                                </div>
                            </div>
                            <div Class="row">
                                <div Class="col-lg-3">
                                    <Label> @PageData("factlabel-4")</label>
                                    <span Class="msg-val">@Html.ValidationMessage("email")</span>
                                    <span Class="info-fields"><input type="email" name="email" id="email" class="form-control"  value="@Html.Raw(email)" placeholder="E-mail"  @Validation.For("email") /></span>
                                </div>
                                <div Class="col-lg-3">
                                    <Button type = "submit" id="btm-tax" Class="btn-prisave" tabindex="0">@PageData("btnsave")</button>
                                    <Button type = "button" Class="btn-defsave" tabindex="0" onclick="noedit('fact')" >@PageData("btncancel")</button>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
                <div Class="tab-pane" id="seguridad">
                    <div Class="cuenta-box">
                        <h2 Class="tabs-title"> @PageData("subtitulo-5")</h2>
                        <p>@PageData("seglabel-1")<br>@PageData("seglabel-2")</p>
                        <a href="javascript:;" class="btn btn-primary" data-toggle="modal" data-target="#cerrar-cuenta">@PageData("btncerrarcta")</a>
                    </div>
                </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
@section Scripts
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <!-- JQUERY MASKED INPUT -->
	<script src="assets/plugin/masked-input/jquery.maskedinput.min.js"></script>

    <script src="js/jquery.jeditable.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            //Editable Fields
            $('#edit-datos').on('click', function () {
                $('.info-fields').toggleClass('edit');
                $("#nombres").attr("readonly", false);
                $("#pwd").attr("readonly", false);
                $("#conf-pwd").attr("readonly", false);
                $("#pais").attr("disabled", false);
                $("#moneda").attr("disabled", false);
                $("#edit-datos").toggleClass('hidden');
                $("#save-datos").removeClass('hidden');
                //$('.edit').editable('http://www.example.com/save.php'); dirigir a la base de datos
            });

            $('#edit-tax').on('click', function () {
                $('.info-fields').toggleClass('edit');
                $("#edit-tax").toggleClass('hidden');
                $("#frm-tax").removeClass('hidden');
                $("#div-tax").toggleClass('hidden');

                //$("#rfc").attr("readonly", false);
                //$("#razon").attr("readonly", false);
                //$("#email").attr("readonly", false);
                //$("#direccion").attr("readonly", false);
                //$("#edit-tax").toggleClass('hidden');
                //$("#btm-tax").removeClass('hidden');
                //$("#btm-tax").toggleClass('btn btn-primary');
                //$('.edit').editable('http://www.example.com/save.php'); dirigir a la base de datos
            });

            $('#edit-depos').on('click', function () {
                $("#edit-depos").toggleClass('hidden');
                $("#frm-depos").removeClass('hidden');
                $("#div-depos").toggleClass('hidden');
            });

        });

        function noedit(valor) {

            switch (valor) {
                case "datos":
                    $('.info-fields').removeClass('edit');
                    $("#nombres").attr("readonly", true);
                    $("#pwd").attr("readonly", true);
                    $("#conf-pwd").attr("readonly", true);
                    $("#pais").attr("disabled", 'disabled');
                    $("#moneda").attr("disabled", 'disabled');
                    $("#edit-datos").removeClass('hidden');
                    $("#save-datos").toggleClass('hidden');
                    break;
                case "fact":
                    $('.info-fields').removeClass('edit');
                    $("#edit-tax").removeClass('hidden');
                    $("#frm-tax").toggleClass('hidden');
                    $("#div-tax").removeClass('hidden');
                    break;
                case "depos":
                    $("#edit-depos").removeClass('hidden');
                    $("#frm-depos").toggleClass('hidden');
                    $("#div-depos").removeClass('hidden');
                    break;
            }

        }

        var msgsend = "@msgsave";
        switch (msgsend) {
            case "OK":
                $.bootstrapGrowl('@PageData("msg-ok")', {
                    type: 'success',
                    delay: 4000,
                    width: '100%'
                });
                break;
            case "OK-DEL":
                $.bootstrapGrowl('@PageData("msg-ok-del")', {
                    type: 'success',
                    delay: 4000,
                    width: '100%'
                });
                break;
            case "ERR":
                $.bootstrapGrowl('@PageData("msg-err")', {
                    type: 'danger',
                    delay: 4000,
                    width: '100%'
                });
                break;
            case "ERRPWD":
                $.bootstrapGrowl('@PageData("msg-errpwd")', {
                    type: 'danger',
                    delay: 4000,
                    width: '100%'
                });
                break;
        }

        function checkconf(valor) {
            var pwd = document.getElementById("pwd").value;
            if (pwd != valor) {
                $.bootstrapGrowl('La confirmación del a CONTRASEÑA es incorrecta.', {
                    type: 'danger',
                    delay: 4000,
                    width: '100%'
                });
                document.getElementById("conf-pwd").focus();
                document.getElementById("btnsave").disabled = true;
            }
            else {
                document.getElementById("btnsave").disabled = false;
            }
        }

        function fchangetype(valor) {
            document.getElementById("numtc" + valor).type = "text";
        }
        function changetype(valor) {
            document.getElementById("numtc" + valor).type = "password";
            if (valor = 1)
            {
                document.getElementById("tc-mc").style.display = "none";
                document.getElementById("tc-vi").style.display = "none";
                document.getElementById("tc-am").style.display = "none";
                var num1 = document.getElementById("numtc" + valor).value;
                var ini = num1.substr(0, 1);
                switch (ini) {
                    case "3":
                        ini = num1.substr(0, 2);
                        switch (ini) {
                            case "34":
                                document.getElementById("tc-am").style.display = "";
                                break;
                            case "35":
                                document.getElementById("tc-am").style.display = "";
                                break;
                            case "36":
                                document.getElementById("tc-am").style.display = "";
                                break;
                            case "37":
                                document.getElementById("tc-am").style.display = "";
                                break;
                        }
                        break;
                    case "4":
                        document.getElementById("tc-vi").style.display = "";
                        break;
                    case "5":
                        ini = num1.substr(0, 2);
                        switch (ini) {
                            case "51":
                                document.getElementById("tc-mc").style.display = "";
                                break;
                            case "52":
                                document.getElementById("tc-mc").style.display = "";
                                break;
                            case "53":
                                document.getElementById("tc-mc").style.display = "";
                                break;
                            case "54":
                                document.getElementById("tc-mc").style.display = "";
                                break;
                            case "55":
                                document.getElementById("tc-mc").style.display = "";
                                break;
                        }
                        break;
                }
            }

        }

        function cambiaformato() {
            var d = document.getElementById("tipodepos").value;
            var numcard = document.getElementById("numcard");
            switch (d) {
                case "CRED":
                    numcard.setAttribute("maxlength", "16");
                    numcard.setAttribute("size", "16");
                    //$("#numcard").mask("9999-9999-9999-9999");
                    break;
                case "DEBI":
                    numcard.setAttribute("maxlength", "16");
                    numcard.setAttribute("size", "16");
                    //$("#numcard").mask("9999-9999-9999-9999");
                    break;
                case "CLABE":
                    numcard.setAttribute("maxlength", "18");
                    numcard.setAttribute("size", "18");
                    //$("#numcard").mask("999999999999999999");
                    break;
            }
        }

    </script>
End Section
