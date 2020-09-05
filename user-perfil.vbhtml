@functions

    Dim strConn As String = ConfigurationManager.ConnectionStrings("SQLConn").ToString

    Protected Sub Grabar_Habilidades(ByVal idemp As Long, ByRef msgsave As String)
        Try

            Dim subHab() = Request.Form.GetValues("idhab[]")
            Dim idhabs As String = ""
            For inti As Int16 = 0 To UBound(subHab)
                idhabs &= "," & subHab(inti)
            Next
            Dim objE As New poolin_class.cEmprendedor
            If idhabs <> "" Then
                idhabs = Mid(idhabs, 2)
                objE.Actualiza_Habilidades(idemp, idhabs, strConn)
            End If
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End Sub

    Protected Sub Grabar_Datos_Emprendedor(ByVal idemp As Long, ByRef msgsave As String)
        Try

            msgsave = ""
            'Dim serv() = Request.Form.GetValues("servicios[]")
            Dim idserv() = Request.Form.GetValues("idserv[]")
            Dim e As New poolin_class.cEmprendedor
            e.idemp = idemp
            e.website = Request.Form("website")
            e.honorario = Request.Form("presupuesto")
            e.empresa = Request.Form("empresa")
            e.servicios = Request.Form("servgrales")
            e.idaccion = Request.Form("idaccion")
            e.tipoingreso = Request.Form("tipoingreso")
            e.Update_DatosEmprendedor(idemp, strConn)
            'For inti As Int16 = 0 To UBound(serv)
            '    e.Agrega_Trabajos(idemp, serv(inti), "", "", "", strConn, idserv(inti))
            'Next
            Dim idioma() = Request.Form.GetValues("idiomas[]")
            Dim ididioma() = Request.Form.GetValues("ididiomas[]")
            For inti As Int16 = 0 To UBound(idioma)
                If idioma(inti) <> "" Then
                    e.Agrega_Idiomas(idemp, idioma(inti), "", "", "", strConn, ididioma(inti))
                End If
            Next
            Dim curso() = Request.Form.GetValues("cursos[]")
            Dim idcurso() = Request.Form.GetValues("idcursos[]")
            For inti As Int16 = 0 To UBound(curso)
                If curso(inti) <> "" Then
                    e.Agrega_Cursos(idemp, curso(inti), "", "", "", strConn, idcurso(inti))
                End If
            Next

            Dim subcatego() = Request.Form.GetValues("tsubcat[]")
            Dim idsubcat As String = ""
            For inti As Int16 = 0 To UBound(subcatego)
                idsubcat &= "," & subcatego(inti)
            Next
            If idsubcat <> "" Then
                idsubcat = Mid(idsubcat, 2)
                e.Actualiza_Categoria(idemp, idsubcat, "", strConn)
            End If
            Grabar_Habilidades(idemp, msgsave)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try

    End Sub
    Protected Sub Graba_Datos(ByVal idemp As Long, ByRef msgsave As String)
        Dim m As New poolin_class.cComunes
        msgsave = ""
        Try
            Dim objEmp As New poolin_class.cEmprendedor
            objEmp.idemp = idemp
            objEmp.nombres = Request.Form("nombres")
            objEmp.apellidos = "" ' Request.Form("lname")
            objEmp.email = Request.Form("email")
            'objEmp.pwd = Request.Form("pwd")
            objEmp.empresa = ""
            objEmp.idpais = Request.Form("pais")
            objEmp.fecha_modificacion = Now
            objEmp.email2 = ""
            objEmp.telefono = ""
            objEmp.slogan = Request.Form("slogan")
            objEmp.idaccion = "A"
            objEmp.estado = Request.Form("estado")
            If IsNothing(Request.Form("estado")) Then objEmp.estado = ""
            objEmp.ciudad = Request.Form("ciudad")
            If IsNothing(Request.Form("ciudad")) Then objEmp.ciudad = ""

            objEmp.codpostal = ""
            objEmp.calle = ""
            objEmp.numext = ""
            objEmp.numint = ""
            objEmp.colonia = ""
            objEmp.servicios = ""

            objEmp.website = Request.Form("website")
            If IsNothing(Request.Form("website")) Then objEmp.website = ""

            objEmp.facebook = Request.Form("facebook")
            If IsNothing(Request.Form("facebook")) Then objEmp.facebook = ""

            objEmp.twitter = Request.Form("twitter")
            If IsNothing(Request.Form("twitter")) Then objEmp.twitter = ""

            objEmp.instagram = Request.Form("instagram")
            If IsNothing(Request.Form("instagram")) Then objEmp.instagram = ""

            objEmp.tipoingreso = "H"
            'If IsNothing(Request.Form("ddltipohono")) Then objEmp.tipoingreso = ""

            objEmp.moneda = "MXN"

            'If Request.Form("moneda") = "" Then
            '    objEmp.moneda = "USD"
            'Else
            '    objEmp.moneda = Request.Form("moneda")
            'End If
            objEmp.honorario = 0
            'If IsNothing(Request.Form("honorario")) Then objEmp.honorario = 0
            'objEmp.honorario = Request.Form("honorario")

            objEmp.avatar = "_avatar_" & idemp & ".png"

            'Dim blob As String = "" & Request.Form("blobfoto")
            'If blob <> "" Then
            '    blob = blob.Replace("data:image/png;base64,", "")
            '    Dim b As Byte() = Convert.FromBase64String(blob)
            '    Dim img As Image
            '    Using ms As New MemoryStream(b)
            '        img = Image.FromStream(ms)
            '        img.Save(Server.MapPath("poolin_arch/images/png/_" & e.avatar))
            '    End Using
            'End If
            'Using f As New StreamWriter(Server.MapPath("poolin_arch/images/png/" & e.avatar))
            'f.Write(blobfile.Value)
            'f.Write(Request.Form("blobfoto"))
            'cropfile.InnerHtml = "<img src=" & Request.Form("blobfoto") & ">"
            'f.Close()
            'End Using

            objEmp.Actualiza_Datos(strConn)
            msgsave = "OK"
        Catch ex As Exception
            msgsave = "ERR"
        End Try
    End Sub
End Functions
@code
    PageData("pagina") = "user-perfil"
    If IsNothing(Request.Cookies("idemp")) Then
        Response.Redirect("https://mueveyemprende.io")
    End If

    Dim idemp As Long = 0

    Try
        idemp = Request.Cookies("idemp").Value
    Catch ex As Exception
        Response.Redirect("https://mueveyemprende.io")
    End Try

    Dim msgsave As String = ""

    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Perfil"
    PageData("opperfil") = "active"

    'Validation.RequireField("empresa", "<br>Debes indicar el nombre de tu empresa/negocio.")
    'Validation.RequireField("presupuesto", "<br>Debes indicar el presupuesto por hora.")
    'Validation.RequireField("servicios[]", "<br>Debes indicar al menos un servicio.")

    Dim objComun As New poolin_class.cComunes
    Dim objEmp As New poolin_class.cEmprendedor

    Dim avatar As String = "avatar_"

    Dim pais As String = "México"

    Dim dtGral As System.Data.DataTable
    Dim dtServ As System.Data.DataTable 'Utilizamos la tabla emp_trabajos
    Dim dtPais As System.Data.DataTable
    Dim dtIdiomas As System.Data.DataTable
    Dim dtCursos As System.Data.DataTable
    Dim dtHabEmp As System.Data.DataTable
    Dim dtHab As System.Data.DataTable = objComun.Carga_Habilidades(strConn)

    Dim subcatsel As String = ""

    Dim objCat As New poolin_class.cCategorias
    Dim dtCat As System.Data.DataTable = objCat.Carga_Categorias(strConn)
    Dim dtSubCat As System.Data.DataTable

    If Not Directory.Exists(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images")) Then
        Directory.CreateDirectory(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images"))
    End If
    Try

        If Request.Form("saveporta") = "OK" Then
            Dim filesdel() As String = Request.Form("file-delete").Split("|")
            For Each fdel As String In filesdel
                If fdel <> "" Then
                    File.Delete(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images/" & fdel))
                End If
            Next

            For i As Integer = 0 To Request.Files.Count - 1
                Dim archivo As HttpPostedFileBase = Request.Files(i)
                If archivo.FileName <> "" Then
                    archivo.SaveAs(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images/" & archivo.FileName))
                End If
                msgsave = "OK"
            Next
        End If

        If Request.Form("savevideo") = "OK" Then
            Dim dirVid As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/"))
            If dirVid.Exists Then
                For Each fvid As FileInfo In dirVid.GetFiles
                    File.Delete(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/" & fvid.Name))
                Next
            End If

            For i As Integer = 0 To Request.Files.Count - 1
                Dim archivo As HttpPostedFileBase = Request.Files(i)
                If archivo.FileName <> "" Then
                    archivo.SaveAs(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/" & archivo.FileName))
                End If
                msgsave = "OK"
            Next
        End If


        If Request.Form("NOEMPRENDE") = "RUN" Then
            Try
                Dim check = Request.Form("checkemp")
                'Request.Form("checkemp") = "on"
                If check = "on" Then
                    objEmp.update_idaccion(idemp, "B", strConn)
                    msgsave = "OK-EMP-VER"
                Else
                    objEmp.update_idaccion(idemp, "A", strConn)
                    msgsave = "OK-EMP"
                End If
            Catch ex As Exception
                msgsave = "ERR"
            End Try
        End If

        'If Request.Form("savehab") = "OK" Then
        '    Grabar_Habilidades(idemp, msgsave)
        'End If

        If Request.Form("s-datos") = "on" Then
            For Each f As String In Request.Files
                Dim foto As HttpPostedFileBase = Request.Files(f)
                If foto.FileName <> "" Then
                    foto.SaveAs(Server.MapPath("poolin_arch/images/png/_avatar_" & idemp & ".png"))
                End If
            Next
            Graba_Datos(idemp, msgsave)
        End If
        If Request.Form("s-foto") = "on" Then
            For Each f As String In Request.Files
                Dim foto As HttpPostedFileBase = Request.Files(f)
                If foto.FileName <> "" Then
                    foto.SaveAs(Server.MapPath("poolin_arch/images/png/_avatar_" & idemp & ".png"))
                End If
            Next
        End If

        If Request.Form("s-emp") = "on" Then
            Grabar_Datos_Emprendedor(idemp, msgsave)
        End If

    Catch ex As Exception
        msgsave = "ERR"
    End Try

    dtServ = objEmp.Datos_Trabajos(idemp, strConn)
    dtGral = objEmp.Datos_Emprendedor(idemp, strConn)
    dtIdiomas = objEmp.Datos_Idiomas(idemp, strConn)
    dtCursos = objEmp.Datos_Cursos(idemp, strConn)
    dtHabEmp = objEmp.Datos_Habilidades(idemp, strConn)
    If dtGral.Rows.Count = 0 Then
        'Redireccionar a una página de ERROR
        Response.Redirect("https://mueveyemprende.io")
    Else
        dtPais = objComun.Carga_Paises(strConn, dtGral.Rows(0)("idpais"))
        If dtPais.Rows.Count <> 0 Then
            pais = dtPais.Rows(0)("nombre")
        End If
    End If

    Dim di As String = Dir(Server.MapPath("poolin_arch/images/png/_avatar_" & idemp & ".png"))
    If di <> "" Then
        avatar = di & "?" & Format(Now, "m.ss")
    End If

    Dim dtCatego As System.Data.DataTable = objEmp.SubCategos_Emp(idemp, strConn)

    For Each dr As System.Data.DataRow In dtCatego.Rows
        subcatsel &= "<a id='subcat-" & dr("id") & "' class='btn-categorias' value='" & dr("id") & "' onclick='delsel(this.id);'> x " & dr("subcategoria") & "</a> <input type='hidden' id='tsub-" & dr("id") & "' name='tsubcat[]' value='" & dr("id") & "'>"
    Next

    Dim habsel = ""


    Dim diPorta As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & idemp & "/images/"))
    Dim diVideo As DirectoryInfo = New DirectoryInfo(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/"))

    If Not diVideo.Exists Then
        Directory.CreateDirectory(Server.MapPath("poolin_arch/portafolio/" & idemp & "/video/"))
    End If

    'For Each favatar In di.GetFiles()
    'avatar = favatar.filename
    ' Next

    dtPais = objComun.Carga_Paises(strConn)
    Dim filesporta As String = ""


    Dim idioma As String = Request.Form("idioma")
    Dim dtEt As System.Data.DataTable = objComun.Etiquetas_m(idemp, idioma, "user-perfil", strConn)
    For Each dr As System.Data.DataRow In dtEt.Rows
        PageData(dr("nomobj")) = dr("valor")
    Next

    If Request.QueryString("msgsend") <> "" Then
        msgsave = Request.QueryString("msgsend").ToUpper
    End If
    Dim idporta = 0
    Dim checkemp = ""
    If Request.Form("checkemp") = "on" Then
        checkemp = "on"
    End If
    Dim idaccion = dtGral.Rows(0)("idaccion")
 End Code
@section head
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">

    @*<link rel="stylesheet" href="~/components/Croppie-2.6.2/croppie.css" />
    <script src="~/components/Croppie-2.6.2/croppie.js"></script>*@
    <link href="assets/plugins/form/jquery-file-upload/css/jquery.fileupload.css" rel="stylesheet" />

End Section

<div id="content" class="container">

    <div class="modal fade" id="modal-expprof" role="dialog" aria-labelledby="modal-expprof" >
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Experiencia profesional</h3>
                    <div class="modal-body">
                        <div class="row form-group">
                            <div class="col-md-12">
                                <label class="control-label">Empresa/Proyecto <em class="text-danger">*</em></label>
                                <input type="text" id="exp-empresa" class="form-control" required />
                            </div>
                        </div>
                        <div Class="row form-group">
                            <div class="col-md-8">
                                <label class="control-label">Funciones <em class="text-danger">*</em></label>
                                <input type="text" id="exp-funcion" class="form-control" required />
                            </div>
                            <div class="col-md-4">
                                <label class="control-label">Período <em class="text-danger">*</em></label>
                                <input type="text" id="exp-periodo" class="form-control" required />
                            </div>
                        </div>
                        <div class="row form-group">
                            <div class="col-md-12">
                                <label class="control-label">Breve descripción de funciones </label>
                                <textarea id="exp-desc" class="form-control" rows="3" required></textarea>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" Class="btn btn-def" data-dismiss="modal">Cerrar</button>
                        <button id="btnaddexpprof" type="button" Class="btn btn-pri" >Agregar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modal-cursos" role="dialog" aria-labelledby="modal-expprof" >
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Información académica</h3>
                    <div class="modal-body">
                        <div class="row form-group">
                            <div class="col-md-12">
                                <label class="control-label">Nombre del programa, estudio o titulación  <em class="text-danger">*</em></label>
                                <input type="text" id="cur-curso" class="form-control" required />
                            </div>
                        </div>
                        <div Class="row form-group">
                            <div class="col-md-8">
                                <label class="control-label">Institución <em class="text-danger">*</em></label>
                                <input type="text" id="cur-instituto" class="form-control" required />
                            </div>
                            <div class="col-md-4">
                                <label class="control-label">Período <em class="text-danger">*</em></label>
                                <input type="text" id="cur-periodo" class="form-control" required />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" Class="btn btn-def" data-dismiss="modal">Cerrar</button>
                        <button id="btnaddcurso" type="button" Class="btn btn-pri" >Agregar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    
    <div class="row">
        <div class="col-lg-8 col-xl-8">
            <h5 class="user_title inline">@PageData("titulo")</h5>

            <div class="row">
                <div class="col-lg-12">
                    <div class="misdatos-box">
                        <div class="row">
                            <div id="showfoto">
                                <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6 centrar">
                                    <a href="javascript:;" id="edit-foto" class="pull-right" style="margin-top:15px;"><i class="fas fa-edit"></i></a>
                                    <img src="poolin_arch/images/png/@avatar" class="misdatos-foto">
                                </div>
                            </div>
                            <div id="editfoto" class="edit-box">
                                <form enctype="multipart/form-data" id="formsave-foto" method="post" action="user-perfil">
                                    <input type="hidden" name="s-foto" value="on" />
                                    <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6 centrar">
                                        <img id="demo-basic" src="poolin_arch/images/png/@avatar" class="misdatos-foto"><br />
                                        @*<span class="btn btn-defaulf">
                                            <input id="idfoto-upload" name="foto" type="file" />
                                        </span>*@
								        <span class="btn btn-primary fileinput-button btn-sm m-r-3 m-b-3">
									        <i class="glyphicon glyphicon-plus"></i>
									        <span>Selecciona archivo</span>
									        <input id="idfoto-upload" type="file" name="foto">
								        </span>
                                    </div>
                                </form>
                            </div>
                            <div id="showdata">
                                <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                    <a href="javascript:;" id="edit-profile" class="pull-right"><i class="fas fa-edit"></i></a>
                                    <div class="text_align">
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(IIf(dtGral.Rows(0)("nombres") = "", "<span class='text-danger'>Indica tu nombre<span>", dtGral.Rows(0)("nombres")))</div>    
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(dtGral.Rows(0)("email"))</div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @if "" & dtGral.Rows(0)("slogan") <> "" Then
                                                    @<span>@Html.Raw(dtGral.Rows(0)("slogan"))</span>
                                                Else
                                                    @<span class="text-gray-light">Profesión</span>
                                                End if
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">@Html.Raw(pais)</div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <span>@Html.Raw(String.Format("{0}, {1}", dtGral.Rows(0)("ciudad"), dtGral.Rows(0)("estado")))</span>    
                                                @*@if "" & dtGral.Rows(0)("ciudad") <> "" Then
                                                Else
                                                    @<span class="text-gray-light">Ciudad</span>
                                                end if
                                                @if "" & dtGral.Rows(0)("estado") <> "" Then
                                                    @Html.Raw(", " & dtGral.Rows(0)("estado"))
                                                Else
                                                    @<span class="text-gray-light">, Estado</span>
                                                end if*@
                                            
                                            </div>
                                        </div>
                                        <div class="row form-group">
                                            <div class="col-md-12"><a href="~/micuenta" class="pull-right">Mi Cuenta</a></div>
                                        </div>
                                        @*<div class="profile-fields profesion">@Html.Raw(dtGral.Rows(0)("slogan"))</div>*@
                                    
                                    </div>
                                </div>
                            </div>
                            <div id="editdata" class="edit-box">
                                <form id="formsave-datos" method="post" action="user-perfil">
                                    <input type="hidden" name="s-datos" value="on" />
                                    <div class="col-sm-6 col-md-6 col-lg-6 col-xl-6">
                                        <div class="text_align">
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <input type="text" name="nombres" class="form-control" placeholder="@PageData("miperfil-pholder_2")" value="@Html.Raw(dtGral.Rows(0)("nombres"))" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <input type="text" name="email" class="form-control" placeholder="@PageData("miperfil-pholder_3")" disabled value="@Html.Raw(dtGral.Rows(0)("email"))" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <input type="tel" name="slogan" class="form-control" placeholder="@PageData("miperfil-pholder_4")" value="@Html.Raw("" & dtGral.Rows(0)("slogan"))" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <select id="selpais" name="pais" class="form-control">
                                                        <option value="" disabled selected>@PageData("miperfil-pholder_7")</option>
                                                        <option value="0">POR DEFINIR</option>
                                                        @for Each dr As System.Data.DataRow In dtPais.Rows
                                                            @<option value="@dr("id")">@dr("nombre")</option>
                                                        Next
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <input type="text" name="ciudad" class="form-control" placeholder="@PageData("miperfil-pholder_5")" value="@Html.Raw(dtGral.Rows(0)("ciudad"))" />
                                                </div>
                                                <div class="col-lg-6">
                                                    <input type="text" name="estado" class="form-control" placeholder="@PageData("miperfil-pholder_6")" value="@Html.Raw(dtGral.Rows(0)("estado"))" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <input type="submit" value="@PageData("miperfil-btn")" class="btn btn-primary pull-right">
                                                </div>
                                            </div>

                                            <script>
                                                document.getElementById("selpais").value = "@dtGral.Rows(0)("idpais")";
                                            </script>
                                            <div class="clearfix"></div>

                                        </div>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <div class="row">
                        <div class="col-lg-12">
                            <form id="frm-emprendedor" method="post" action="~/user-perfil">
                                <input name="NOEMPRENDE" type="hidden" value="RUN"/>
                                <input type="hidden" name="idaccion" value="A"/>
                                <h3 class="user_subtitle inline">¿Deseas ofrecer tus servicios en Conecta?</h3>
                                <div class="pull-right" style="margin-top:-10px;">
                                    <input id="checkemp" name="checkemp" type="checkbox" data-toggle="toggle" data-size="small" data-on="SI" data-off="NO" onchange="gotoperfil(this.checked)">
                                </div>
                            </form>
                        </div>
                    </div>
                    <div id="show-datos">
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="profile-box">
                                    @*<h2> @PageData("sub-titulo_2")</h2>*@
                                    <form class="" accept-charset="UTF-8" action="" method="post">
                                        <input type="hidden" name="s-emp" value="on" />
                                        <input type="hidden" name="idaccion" value="B"/>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3  class="user_subtitle inline">Información sobre mis servicios</h3>
                                                <h4>Ayúdanos a clasificar tus servicios en una o varias áreas que estén relacionadas.</h4>
                                                <select id="categoria" name="categoria" class="form-control" onchange="goselcat(this.value)">
                                                    <option value="" disabled selected>@PageData("miperfil-label_6")</option>
                                                    @for Each drCat As System.Data.DataRow In dtCat.Rows
                                                        dtSubCat = objCat.Carga_SubCategorias(drCat("id"), strConn)
                                                        @<optgroup label="@drCat("categoria")">
                                                            @for Each drSubCat As System.Data.DataRow In dtSubCat.Rows
                                                                @<option value="@drSubCat("id")">@drSubCat("subcategoria")</option>
                                                            Next
                                                        </optgroup>
                                                    Next
                                                </select>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <span id="selcat">
                                                </span>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h4>Cuéntanos más sobre los servicios que ofreces:</h4>
                                                <span class="msg-val">@Html.ValidationMessage("empresa")</span>
                                                <input type="text" name="empresa" class="form-control" placeholder="@PageData("miperfil-label_1")" value="@Html.Raw(dtGral.Rows(0)("empresa"))" @Validation.For("empresa")>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <input type="text" class="form-control" name="website" placeholder="Sitio web o página/perfil de red social" value="@Html.Raw(dtGral.Rows(0)("website"))">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <label></label>
                                                <textarea name="servgrales" maxlength="2000" rows="5" class="form-control" placeholder="@PageData("miperfil-label_2")">@Html.Raw(dtGral.Rows(0)("servicios"))</textarea>
                                            </div>
                                        </div>

                                        @*<div class="row">
                                            <div class="col-lg-12">
                                                <div class="fields_wrap servicios">
                                                    <Label> @PageData("miperfil-label_4") </Label>
                                                    <span  class="msg-val">@Html.ValidationMessage("servicios[]")</span>

                                                    @if dtServ.Rows.Count = 0 Then
                                                        @<div class="row">
                                                            <div class="col-lg-2">
                                                                <input type="hidden" name="idserv[]" value="0" />
                                                                <input type="text" class="form-control" style="width:50%" name="servicios[]" @Validation.For("servicios[]")>
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <a href="javascript:;" class="add_field servicios"><i class="fas fa-plus"></i></a>
                                                            </div>
                                                        </div>
                                                    Else
                                                        For Each dr As System.Data.DataRow In dtServ.Rows
                                                            If dtServ.Rows.IndexOf(dr) = 0 Then
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" name="idserv[]" value="@dr("id")" />
                                                                        <input type="text" class="form-control" name="servicios[]" value="@dr("proyecto")" @Validation.For("servicios[]") />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" class="add_field servicios"><i class="fas fa-plus"></i></a>
                                                                    </div>
                                                                </div>
                                                            Else
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" id="serv-i-@dr("id")" name="idserv[]" value="@dr("id")" />
                                                                        <input type="text" id="serv-s-@dr("id")" class="form-control" name="servicios[]" value="@dr("proyecto")" @Validation.For("servicios[]") />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" id="serv-@dr("id")" class="remove_field" onclick="marcaserv(this.id)"> <i id="serv-f-@dr("id")" class="fas fa-times"></i></a>
                                                                    </div>
                                                                </div>
                                                            End If
                                                        Next
                                                    End If
                                                </div>

                                            </div>
                                        </div>*@
                                        @*<div class="row">
                                            <div class="col-lg-4">
                                                <div class="fields_wrap">
                                                    <Label> @PageData("miperfil-label_5") (@dtGral.Rows(0)("moneda")) </Label>
                                                    <span  class="msg-val">@Html.ValidationMessage("presupuesto")</span>
                                                    <input type="number" step="0.01" name="presupuesto" class="form-control" value="@Html.Raw(dtGral.Rows(0)("honorario"))" @Validation.For("presupuesto")>
                                                </div>
                                            </div>
                                        </div>*@


                                        <h3 class="user_subtitle hide">Habilidades destacadas</h3>

                                        <div class="row hide">
                                            <div class="col-lg-3">
                                                Por proyecto <input type="radio" value="P" name="tipoingreso" id="xproyecto" />
                                            </div>
                                            <div class="col-lg-3">
                                                Por hora <input type="radio" value="H" name="tipoingreso" id="xhonorario" />
                                            </div>
                                        </div>
                                        <script>
                                            var tipoingres = "@dtGral.Rows(0)("tipoingreso")";
                                            if (tipoingres == "P") { document.getElementById("xproyecto").checked = true; }
                                            else { document.getElementById("xhonorario").checked = true;}
                                        </script>
                                        <label></label>
                                        <div class="row">
                                            <div class="col-lg-12">
                                                @*<div class="row">
                                                    <div class="col col-lg-12">
                                                        <h2> @PageData("sub-titulo_3") <a href="javascript:;" onclick="addmishab()" class="pull-right"><i class="fas fa-edit" id="edit-hab"></i></a> </h2>
                                                    </div>
                                                </div>*@
                                                <div id="mishab" class="row  hide">
                                                    <div class="col-lg-8">
                                                        <select id="habilidades" class="form-control" onchange="goselhab(this.value)">
                                                            <option value="" disabled selected>@PageData("sub-titulo_3")</option>
                                                            @for Each dr As System.Data.DataRow In dtHab.Rows
                                                                @<option value="@dr("id")">@Html.Raw(dr("nombre"))</option>
                                                            Next
                                                        </select>
                                                    </div>
                                                </div>
                                                <div class="row  hide">
                                                    <div class="col-lg-12">
                                                        <div id="selhab" style="padding-top:15px; padding-bottom:15px;">
                                                            @For Each dr As System.Data.DataRow In dtHabEmp.Rows
                                                                @<a id="hab-@dr("idhabilidad")" class="btn-categorias" value="@dr("idhabilidad")" onclick="delhab(this.id);"> x @dr("nombre")</a> 
                                                                @<input type="hidden" id="thab-@dr("idhabilidad")" name="idhab[]" value="@dr("idhabilidad")">
                                                            Next
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row hide">
                                            <div class="col-lg-12">
                                                <div class="fields_wrap idiomas">
                                                    @if dtIdiomas.Rows.Count = 0 Then
                                                        @<div class="row">
                                                            <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                <input type="hidden" name="ididiomas[]" value="0" />
                                                                <input type="text" class="form-control" placeholder="@PageData("miperfil-label_8")" name="idiomas[]">
                                                            </div>
                                                            <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                <a href="javascript:;" class="add_field idiomas"><i class="fas fa-plus"></i></a>
                                                            </div>
                                                        </div>
                                                    Else
                                                        For Each dr As System.Data.DataRow In dtIdiomas.Rows
                                                            If dtIdiomas.Rows.IndexOf(dr) = 0 Then
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" name="ididiomas[]" value="@dr("id")" />
                                                                        <input type="text" class="form-control" placeholder="@PageData("miperfil-label_8")" name="idiomas[]" value="@dr("idioma")" />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" class="add_field idiomas"><i class="fas fa-plus"></i></a>
                                                                    </div>
                                                                </div>
                                                            Else
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" id="idioma-i-@dr("id")" name="ididiomas[]" value="@dr("id")" />
                                                                        <input type="text" class="form-control" id="idioma-s-@dr("id")" name="idiomas[]" value="@dr("idioma")" />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" id="idioma-@dr("id")" class="remove_field" onclick="marcaidiomas(this.id)"> <i id="idioma-f-@dr("id")" class="fas fa-times"></i></a>
                                                                    </div>
                                                                </div>
                                                            End If
                                                        Next
                                                    End If
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row hide">
                                            <div class="col-lg-12">
                                                <div class="fields_wrap cursos">
                                                    @if dtCursos.Rows.Count = 0 Then
                                                        @<div class="row">
                                                            <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                <input type="hidden" name="idcursos[]" value="0" />
                                                                <input type="text" placeholder="@PageData("miperfil-label_7")" class="form-control" name="cursos[]">
                                                            </div>
                                                            <div class="col-lg-1  col-lg-1 col-sm-1 col-xs-1">
                                                                <a href="javascript:;" class="add_field cursos"><i class="fas fa-plus"></i></a>
                                                            </div>
                                                        </div>
                                                    Else
                                                        For Each dr As System.Data.DataRow In dtCursos.Rows
                                                            If dtCursos.Rows.IndexOf(dr) = 0 Then
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" name="idcursos[]" value="@dr("id")" />
                                                                        <input type="text" placeholder="@PageData("miperfil-label_7")" class="form-control" name="cursos[]" value="@dr("curso")" />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" class="add_field cursos"><i class="fas fa-plus"></i></a>
                                                                    </div>
                                                                </div>
                                                            Else
                                                                @<div class="row">
                                                                    <div class="col-lg-11 col-lg-11 col-sm-11 col-xs-11">
                                                                        <input type="hidden" id="curso-i-@dr("id")" name="idcursos[]" value="@dr("id")" />
                                                                        <input type="text" class="form-control" id="curso-s-@dr("id")" name="cursos[]" value="@dr("curso")" />
                                                                    </div>
                                                                    <div class="col-lg-1 col-lg-1 col-sm-1 col-xs-1">
                                                                        <a href="javascript:;" id="curso-@dr("id")" class="remove_field" onclick="marcacursos(this.id)"> <i id="curso-f-@dr("id")" class="fas fa-times"></i></a>
                                                                    </div>
                                                                </div>
                                                            End If
                                                        Next
                                                    End If
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12">
                                                <input type="submit" value="@PageData("miperfil-btn")" class="btn btn-primary pull-right">
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>
                                    </form>
                                </div>
                            </div>
                        </div>
                        <div class="profile-box">
                            <h3 class="user_subtitle"> @PageData("sub-titulo_4") </h3>
                            <div class="row">
                                <div class="col-lg-6">
                                    <form id="frmporta" method="post" action="~/user-perfil#frmporta" enctype="multipart/form-data">
                                        <input type="hidden" name="saveporta" value="OK" />
                                        <input id="file-delete" name="file-delete" type="hidden" value="" />
                                        <div id="porta" class="row">
                                            <div class="col-lg-12  p-asesoria">
                                                <input type="file" multiple id="fporta" name="fporta" style="display:none"  accept="image/x-png,image/gif,image/jpeg" onchange="loadimg('frmporta')" />
                                                <img src="~/Images/fileupload.png" id="upfile1" style="cursor:pointer" /><br />
                                                Cargar Galería
                                            </div>
                                        </div>
                                        <div class="portfolio-wrap">
                                            <div class="row">
                                                @For Each fi In diPorta.GetFiles()
                                                    idporta += 1
                                                    @<div class="col-md-4 col-sm-4 col-xs-4">
                                                        <input id="file-@idporta" type="hidden" value="@Html.Raw(fi.Name)" />
                                                        <a href="javascript:;" title="Eliminar imagen"  id="porta-@idporta" onclick="delporta('@idporta', 'poolin_arch/portafolio/@idemp/images/@Html.Raw(fi.Name)')" ><i class="fas fa-trash text-danger"></i> <small>Eliminar</small></a>
                                                        <img id="img-@idporta" src="poolin_arch/portafolio/@idemp/images/@Html.Raw(fi.Name)" class="img-responsive" style="padding-bottom:10px;">
                                                    </div>
                                                Next
                                            </div>
                                        </div>
                                        <div id="btnporta" Class="row hide">
                                            <div Class="col col-lg-12">
                                                <Button Class="btn btn-primary pull-right">@PageData("miperfil-btn")</Button>
                                            </div>
                                        </div>

                                    </form>
                                </div>
                                <div class="col-lg-6">
                                    <form id="frmvideo" method="post" action="~/user-perfil" enctype="multipart/form-data">
                                        <input type="hidden" name="savevideo" value="OK" />
                                        <div class="row">
                                            <div class="col-lg-12 p-asesoria">
                                                <input type="file" id="fvideo" name="fvideo" style="display:none" accept="video/*" onchange="loadimg('frmvideo')" />
                                                <img src="~/Images/uploadvideo.png"  id="upfile2"  alt="Subir Video" style="cursor:pointer" /><br />
                                                Cargar Video
                                            </div>
                                        </div>
                                        <div class="portfolio-wrap p-asesoria" style="margin-top:-20px">
                                                @if diVideo.Exists Then
                                                    For Each fi In diVideo.GetFiles()
                                                        @<div class="row">
                                                            <div class="col-md-10">
                                                                <input type="hidden" value="@Html.Raw(fi.Name)" />
                                                                @*<video id="img-@idporta" src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" class="img-responsive" style="float: right; padding:2px;">*@
                                                                <video controls style="width:100%;" id="video-pool" >
                                                                  <source src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" type="video/mp4">
                                                                  <source src="poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)" type="video/ogg">
                                                                  Your browser does not support the video tag.
                                                                </video>
                                                            </div>
                                                            <div class="col-md-2 pull-right">
                                                                <a href = "javascript:;"  onclick="delvideo('video-pool', 'poolin_arch/portafolio/@idemp/video/@Html.Raw(fi.Name)')" title="Eliminar video" Class="btn btn-link"><img src="~/Images/cancel.png" /></a>
                                                            </div>
                                                        </div>
                                                    Next
                                                End if

                                        </div>
                                        <div class="row hidden">
                                            <div class="col col-lg-12">
                                                <Button class="btn btn-primary pull-right">@PageData("miperfil-btn")</Button>
                                            </div>
                                        </div>

                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-4 col-xl-4">
            <h4 class="user_title inline">Mi experiencia profesional <a href="javascript:;" data-toggle="modal" data-target="#modal-expprof"><i class="fas fa-plus"></i></a></h4>
            <div class="row">
                <div class="col-lg-12">
                    <div id="exp-prof" class="expprof-box">
                        <span>a&b Integral Solutions</span> <a href="javascript:;" onclick="borraexpprof(0)"><i class="fas fa-times"></i></a>
                        <ul>
                            <li>Sistemas</li>
                            <li>2016 a la Fecha</li>
                        </ul>
                    </div>
                </div>
            </div>
            <h4 class="user_title inline">Información académica <a href="javascript:;" data-toggle="modal" data-target="#modal-cursos"><i class="fas fa-plus"></i></a></h4>
            <div class="row">
                <div class="col-lg-12">
                    <div id="info-aca" class="cursos-box">
                    </div>
                </div>
            </div>
        </div>
    </div>
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

    <script src="https://gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>
    <Script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></Script>
    <Script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></Script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-growl/1.0.0/jquery.bootstrap-growl.min.js"></script>

    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
    <Script src="js/jquery.jeditable.min.js"></Script>
    <script src="js/slick.min.js" type="text/javascript" charset="utf-8"></script>
    
    <Script>

        $(document).ready(function () {
            //Editable Fields
            $('#idfoto-upload').change(function () {
                $('#formsave-foto').submit();
            });

            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/Carga_ExpProf',
                data: "{idemp: " + "@idemp" + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: carga_exp,
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }
            });

            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/Carga_Cursos',
                data: "{idemp: " + "@idemp" + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: carga_cur,
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }
            });

            $("#btnaddcurso").click(function () {
                var idemp = "@idemp";
                var curso = $("#cur-curso").val();
                var instituto = $("#cur-instituto").val();
                var periodo = $("#cur-periodo").val();
                if (curso == '') {
                    $("#cur-curso").focus();
                    return
                }
                if (instituto == '') {
                    $("#cur-instituto").focus();
                    return
                }
                if (periodo == '') {
                    $("#cur-periodo").focus();
                    return
                }
                $.ajax({
                    type: 'POST',
                    url: 'wsperfil.asmx/Agregar_Cursos',
                    data: "{idemp: " + idemp + ", curso: '" + curso + "', escuela: '" + instituto + "', periodo: '" + periodo +  "', tipo: ''}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: carga_cur,
                    failure: function (response) {
                        alert("1 - " + response.responseText);
                    },
                    error: function (response) {
                        alert("2 - " + response.responseText);
                    }
                }); 
                $("#cur-curso").val("");
                $("#cur-instituto").val("");
                $("#cur-periodo").val("");
                $("#cur-curso").focus();
            });


            $("#btnaddexpprof").click(function () {
                var idemp = "@idemp";
                var empresa = $("#exp-empresa").val();
                var funcion = $("#exp-funcion").val();
                var periodo = $("#exp-periodo").val();
                var desc = $("#exp-desc").val();
                if (empresa == '') {
                    $("#exp-empresa").focus();
                    return
                }
                if (funcion == '') {
                    $("#exp-funcion").focus();
                    return
                }
                if (periodo == '') {
                    $("#exp-periodo").focus();
                    return
                }
                $.ajax({
                    type: 'POST',
                    url: 'wsperfil.asmx/Agregar_ExpProf',
                    data: "{idemp: " + idemp + ", empresa: '" + empresa + "', funcion: '" + funcion + "', periodo: '" + periodo +  "', descripcion: '" +  desc + "'}",
                    contentType: 'application/json; utf-8',
                    dataType: 'json',
                    success: carga_exp,
                    failure: function (response) {
                        alert("1 - " + response.responseText);
                    },
                    error: function (response) {
                        alert("2 - " + response.responseText);
                    }
                }); 
                $("#exp-empresa").val("");
                $("#exp-funcion").val("");
                $("#exp-periodo").val("");
                $("#exp-desc").val("");
                $("#exp-empresa").focus();
            });

            $('#editdata').hide();
            $('#editfoto').hide();
            $('#edit-profile').on('click', function () {
                //$('.profile-fields').toggleclass('edit');
                //$('.edit').editable(''); //dirigir a la base de datos
                $('#showdata').hide();
                $('#editdata').show();
            });

            $('#edit-foto').on('click', function () {
                //$('.profile-fields').toggleclass('edit');
                //$('.edit').editable(''); //dirigir a la base de datos
                $('#showfoto').hide();
                $('#editfoto').show();
            });

            //Form add fields
            var max_fields = 10;
            var wrapper = $(".fields_wrap.servicios");
            var wrapper2 = $(".fields_wrap.cursos");
            var wrapper3 = $(".fields_wrap.idiomas");
            var add_button = $(".add_field.servicios");
            var add_button2 = $(".add_field.cursos");
            var add_button3 = $(".add_field.idiomas");
            var x = 1;

            $(add_button).click(function (e) {
                var html = "";
                e.preventDefault();
                if (x < max_fields) {
                    x++;
                    html = '<div class="row"><div class="col-lg-11 col-mg-11 col-sm-11"><input type="hidden" name="idserv[]" value="0" /><input type="text" class="form-control" name="servicios[]" @@Validation.For("servicios[]") /></div><div class="col-lg-1 col-mg-1 col-sm-1"><a href="javascript:;" class="remove_field"> <i class="fas fa-times"></i></a></div></div>';
                    $(wrapper).append(html);
                }
            });
            //$(wrapper).on("click", ".remove_field", function (e) {
            //    e.preventDefault(); $(this).parent.parent('div.row').remove(); x--;
            //})
            $(add_button2).click(function (e) {
                e.preventDefault();
                if (x < max_fields) {
                    x++;
                    $(wrapper2).append('<div class="row"><div class="col-lg-11 col-mg-11 col-sm-11"><input type="hidden" name="idcursos[]" value="0" /><input type="text" class="form-control" name="cursos[]"/></div><div class="col-lg-1 col-mg-1 col-sm-1"><a href="javascript:;" class="remove_field"> <i class="fas fa-times"></i></a></div></div>');
                }
            });
            //$(wrapper2).on("click", ".remove_field", function (e) {
            //    e.preventDefault(); $(this).parent('div').remove(); x--;
            //})
            $(add_button3).click(function (e) {
                e.preventDefault();
                if (x < max_fields) {
                    x++;
                    $(wrapper3).append('<div class="row"><div class="col-lg-11 col-mg-11 col-sm-11"><input type="hidden" name="ididiomas[]" value="0" /><input type="text" class="form-control" name="idiomas[]"/></div><div class="col-lg-1 col-mg-1 col-sm-1"><a href="javascript:;" class="remove_field"> <i class="fas fa-times"></i></a></div></div>');
                }
            });
            //$(wrapper3).on("click", ".remove_field", function (e) {
            //    e.preventDefault(); $(this).parent('div').remove(); x--;
            //})

            $("#upfile1").click(function () {
                $("#fporta").trigger('click');
            });

            $("#upfile2").click(function () {
                $("#fvideo").trigger('click');
            });

            $('#fvideo').change(function () {
                $('#frmvideo').submit();
            });

            //portafolio
            //$(".portfolio-wrap").slick({
            //    dots: false,
            //    vertical: true,
            //    centerMode: true,
            //    rows: 7,
            //    slidesPerRow: 2,
            //    infinite: true,
            //    nextArrow: '<i class="fas fa-angle-right"></i>',
            //    prevArrow: '<i class="fas fa-angle-left"></i>',
            //    lazyLoad: 'ondemand',
            //    responsive: [
            //        {
            //            breakpoint: 600,
            //            settings: {
            //                slidesToShow: 2,
            //                slidesToScroll: 2
            //            }
            //        },
            //        {
            //            breakpoint: 480,
            //            settings: {
            //                slidesToShow: 1,
            //                slidesToScroll: 1
            //            }
            //        }
            //    ]
            //});

        });

        var selhab = document.getElementById("selhab");
        //style="overflow-y:scroll;height:120px"
        //selhab.style.overflowY = "scroll";
        //selhab.style.height = "200px";

        function borraexpprof(idexpprof) {
            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/Borra_ExpProf',
                data: "{idexpprof: " + idexpprof + ", idemp: " + "@idemp" + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: carga_exp,
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }
            });
        }

        function borracurso(idexpprof) {
            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/Borra_Curso',
                data: "{idcurso: " + idexpprof + ", idemp: " + "@idemp" + "}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: carga_cur,
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }
            });
        }

        function loadimg(frm) {
            $("#" + frm).submit();
        }

        function carga_exp(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("expprof");      
            var idata = 0;
            $("#exp-prof").html("");
            var html = "";
            $(detalle).each(function () {
                html += "<span>" + $(this).find("proyecto").text() + "</span> <a href='javascript:;' onclick='borraexpprof(" + $(this).find("id").text() + ")'><i class='fas fa-times'></i></a>";
                html += "<ul>"; 
                html += "<li>" + $(this).find("funcion").text() + "</li>";
                html += "<li>" + $(this).find("periodo").text() + "</li>";
                if ($(this).find("descripcion").text() != "") {
                    html += "<li>" + $(this).find("descripcion").text() + "</li>"
                }
                html += "</ul>";
            });
            $("#exp-prof").html(html);
        }

        function carga_cur(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var detalle = xml.find("cursos");      
            var idata = 0;
            $("#info-aca").html("");
            var html = "";
            $(detalle).each(function () {
                html += "<span>" + $(this).find("curso").text() + "</span> <a href='javascript:;' onclick='borracurso(" + $(this).find("id").text() + ")'><i class='fas fa-times'></i></a>";
                html += "<ul>";
                html += "<li>" + $(this).find("escuela").text() + "</li>";
                html += "<li>" + $(this).find("periodo").text() + "</li>";
                html += "</ul>";
            });
            $("#info-aca").html(html);
        }

        function marcaserv(valor) {
            valor = valor.replace("serv-", "");
            var id = document.getElementById("serv-i-" + valor).value;
            if (id.substring(0, 1) == "-") {
                document.getElementById("serv-s-" + valor).style.textDecoration = "";
                document.getElementById("serv-s-" + valor).style.borderColor = "lightgray";
                document.getElementById("serv-i-" + valor).value = document.getElementById("serv-i-" + valor).value.replace("-", "");
                document.getElementById("serv-f-" + valor).setAttribute("class", "fas fa-times");
            }
            else {
                document.getElementById("serv-s-" + valor).style.textDecoration = "line-through";
                document.getElementById("serv-s-" + valor).style.borderColor = "red";
                document.getElementById("serv-i-" + valor).value = "-" + document.getElementById("serv-i-" + valor).value;
                document.getElementById("serv-f-" + valor).setAttribute("class", "fas fa-check");
            }
        }
        function marcacursos(valor) {
            valor = valor.replace("curso-", "");
            var id = document.getElementById("i-" + valor).value;
            if (id.substring(0, 1) == "-") {
                document.getElementById("curso-s-" + valor).style.textDecoration = "";
                document.getElementById("curso-s-" + valor).style.borderColor = "lightgray";
                document.getElementById("curso-i-" + valor).value = document.getElementById("curso-i-" + valor).value.replace("-", "");
                document.getElementById("curso-f-" + valor).setAttribute("class", "fas fa-times");
            }
            else {
                document.getElementById("curso-s-" + valor).style.textDecoration = "line-through";
                document.getElementById("curso-s-" + valor).style.borderColor = "red";
                document.getElementById("curso-i-" + valor).value = "-" + document.getElementById("curso-i-" + valor).value;
                document.getElementById("curso-f-" + valor).setAttribute("class", "fas fa-check");
            }
        }
        function marcaidiomas(valor) {
            valor = valor.replace("idioma-", "")
            var id = document.getElementById("i-" + valor.replace("idioma-", "")).value;
            if (id.substring(0, 1) == "-") {
                document.getElementById("idioma-s-" + valor).style.textDecoration = "";
                document.getElementById("idioma-s-" + valor).style.borderColor = "lightgray";
                document.getElementById("idioma-i-" + valor).value = document.getElementById("idioma-i-" + valor).value.replace("-", "");
                document.getElementById("idioma-f-" + valor).setAttribute("class", "fas fa-times");
            }
            else {
                document.getElementById("idioma-s-" + valor).style.textDecoration = "line-through";
                document.getElementById("idioma-s-" + valor).style.borderColor = "red";
                document.getElementById("idioma-i-" + valor).value = "-" + document.getElementById("idioma-i-" + valor).value;
                document.getElementById("idioma-f-" + valor).setAttribute("class", "fas fa-check");
            }
        }


        function goselhab(valor, texto) {
            var selcat = document.getElementById("selhab");
            var cate = document.getElementById("habilidades");
            var arr = document.getElementsByName("idhab[]");
            for (i = 0; i < arr.length; i++) {
                if (valor == arr.item(i).value) {
                    return;
                }
            }
            selcat.innerHTML = selcat.innerHTML + "<a id='hab-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delhab(this.id);'> x " + cate.options[cate.selectedIndex].text + "</a> <input type='hidden' id='thab-" + valor + "' name='idhab[]' value='" + valor + "'>";
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
            //selcat.innerHTML = selcat.innerHTML + "<select name='lstcat' multiple id='subcatego' size='1' ><option value='"+ valor + "'> x " + cate.options[cate.selectedIndex].text + "</option></select> ";
            selcat.innerHTML = selcat.innerHTML + "<a id='subcat-" + valor + "' class='btn-categorias' value='" + valor + "' onclick='delsel(this.id);'> x " + cate.options[cate.selectedIndex].text + "</a> <input type='hidden' id='tsub-" + valor + "' name='tsubcat[]' value='" + valor + "'>";
            //cate.selectedIndex = 0;
        }

        function delsel(valor) {
            document.getElementById(valor).remove();
            document.getElementById("tsub-" + valor.replace("subcat-", "")).remove();
        }

        function delhab(valor) {
            //addmishab();
            document.getElementById(valor).remove();
            document.getElementById("thab-" + valor.replace("hab-", "")).remove();
        }

        function delvideo(valor, path) {
            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/delfile',
                data: "{ruta: '" + path + "'}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: function (response) {
                    document.getElementById(valor).remove();
                },
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }

            });
        } 

        function delporta(valor, path) {
            $.ajax({
                type: 'POST',
                url: 'wsperfil.asmx/delfile',
                data: "{ruta: '" + path + "'}",
                contentType: 'application/json; utf-8',
                dataType: 'json',
                success: function (response) {
                    addportafolio();
                    //var file1 = document.getElementById("file-delete").value;
                    //file1 += document.getElementById("file-" + valor).value + "|";
                    //document.getElementById("file-delete").value = file1;
                    document.getElementById("img-" + valor).remove();
                    document.getElementById("porta-" + valor).remove();
                },
                failure: function (response) {
                    alert("1 - " + response.responseText);
                },
                error: function (response) {
                    alert("2 - " + response.responseText);
                }

            });
        } 

        var selcat = document.getElementById("selcat");
        var selsubcat = "@Html.Raw(subcatsel)";
        if (selsubcat != "") {
            selcat.innerHTML = "<div style='padding-top:15px; padding-bottom:15px;'>" + selsubcat + "<div>"; 
        }

        //$(".success").click(function () {
        //    $.bootstrapGrowl('We do have the Kapua suite available.', {
        //        type: 'success',
        //        delay: 2000,
        //    });
        //});


        //$(".error").click(function () {
        //    $.bootstrapGrowl('You Got Error', {
        //        type: 'danger',
        //        delay: 2000,
        //    });
        //});


        //$(".info").click(function () {
        //    $.bootstrapGrowl('It is for your kind information', {
        //        type: 'info',
        //        delay: 2000,
        //    });
        //});


        //$(".warning").click(function () {
        //    $.bootstrapGrowl('It is for your kind warning', {
        //        type: 'warning',
        //        delay: 2000,
        //    });
        //});

        var msgsend = "@msgsave";
        switch (msgsend) {
            case "OK":
                $.bootstrapGrowl('@PageData("msg-ok")', {
                    type: 'success',
                    delay: 4000,
                    width: '100%'
                });
                break;
            case "OK-EMP-VER":
                $.bootstrapGrowl('Tu perfil podrá ser revisado por otros emprendedores en la sección Contrata', {
                    type: 'success',
                    delay: 4000,
                    width: '100%'
                });
                break;
            case "OK-EMP":
                $.bootstrapGrowl('@PageData("msg-ok-emp")', {
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


        function addmishab()
        {
            var selhab = document.getElementById("selhab");
            //style="overflow-y:scroll;height:120px"
            //selhab.style.overflowY = "scroll";
            //selhab.style.height = "120px";

            document.getElementById("mishab").setAttribute("class", "row");
            document.getElementById("btnmishab").setAttribute("class", "row");
            document.getElementById("edit-hab").style.display = "none";
        }

      //  function addhab(valor) {
        //    var arr = document.getElementsByName("idhab[]");
        //    for (i = 0; i < arr.length; i++) {
        //        if (valor == arr.item(i).value) {
        //            return;
        //        }
        //    }

        //    var selectElement = document.getElementById("selhab");
        //    var li = document.createElement('LI');
        //    li.innerHTML = "" + selectElement.options[selectElement.selectedIndex].text + "<input type='hidden' name='idhab[]' value='" + valor +  "'>";
        //    document.getElementById('ulhab').appendChild(li);
        //}

        function borrar() {
            lis = document.getElementById('ulhab').getElementsByTagName('li');
            for (var i = 0; i < lis.length; i++) {
                lis[i].onclick = function () {
                    this.parentNode.removeChild(this);
                };
            }
        }

        function addportafolio() {
            document.getElementById("porta").setAttribute("class", "row");
            document.getElementById("btnporta").setAttribute("class", "row");
            //document.getElementById("edit-porta").style.display = "none";
        }

        function gotoperfil(valor) {
            if (valor == true) {
                document.getElementById("show-datos").style.display = "";
            }
            else {
                document.getElementById("show-datos").style.display = "none";
            }
            document.getElementById("frm-emprendedor").submit();

        }
        var idaccion = "@idaccion";
        if (idaccion == "B") {
            document.getElementById("show-datos").style.display = "";
            document.getElementById("checkemp").checked = true;
        }
        else
        {
            document.getElementById("show-datos").style.display = "none";
            document.getElementById("checkemp").checked = false;
        }

        var checkemp = "@checkemp";
        if (checkemp == "on") {
            document.getElementById("checkemp").checked = "checked";
        }
        
    </Script>
End Section
