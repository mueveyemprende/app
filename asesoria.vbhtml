@Code
    Layout = "_PoolinLayout.vbhtml"
    PageData("Title") = "Asesoría para tu negocio"

    Validation.RequireField("nom-proyecto", "Este campo es obligatorio")
    Validation.RequireField("desc-proyecto", "Este campo es obligatorio")
    Validation.RequireField("fecha-proyecto", "Este campo es obligatorio")
    Validation.RequireField("num-proyecto", "Este campo es obligatorio")
    Validation.RequireField("ciudad-proyecto", "Este campo es obligatorio")
    Validation.RequireField("email-proyecto", "Este campo es obligatorio")

    If Request.Form("send") = "ok" Then
        Try

            Dim objSend As New poolin_class.cSendGrid
            Dim msghtml = "NOMBRE DEL PROYECTO: " & Request.Form("nom-proyecto") & "<br>"
            msghtml &= " AYUDA EN: <br><ul>"
            If Request.Form("idea") = "on" Then
                msghtml &= "<li>IDEA</li>"
            End If
            If Request.Form("proyecto") = "on" Then
                msghtml &= "<li>PROYECTO</li>"
            End If
            If Request.Form("negocio") = "on" Then
                msghtml &= "<li>NEGOCIO</li>"
            End If
            If Request.Form("empresa") = "on" Then
                msghtml &= "<li>EMPRESA</li>"
            End If
            msghtml &= " </ul><br>"
            msghtml &= "DESCRIPCIÓN: " & Request.Form("desc-proyecto") & "<br>"
            msghtml &= "FECHA PROYECTO: " & Request.Form("fecha-proyecto") & "<br>"
            msghtml &= "# PERSONAS: " & Request.Form("num-proyecto") & "<br>"
            msghtml &= "CIUDAD: " & Request.Form("ciudad-proyecto") & "<br>"
            msghtml &= "EMAIL: " & Request.Form("email-proyecto") & "<br>"
            msghtml &= "TEL: " & Request.Form("tel-proyecto") & "<br>"
            msghtml &= "HORARIO CONTACTO"
            msghtml &= "<ul>"
            If Request.Form("loc-manana") = "on" Then
                msghtml &= "<li>MAÑANA</li>"
            End If
            If Request.Form("loc-tarde") = "on" Then
                msghtml &= "<li>TARDE</li>"
            End If
            If Request.Form("loc-noche") = "on" Then
                msghtml &= "<li>NOCHE</li>"
            End If
            msghtml &= "</ul>"
            objSend.Correo_SMTP("SOLICITUD DE COACHING", msghtml, "coaching@mueveyemprende.io", "vicente@mueveyemprende.io", ConfigurationManager.ConnectionStrings("SQLConn").ToString)
            Response.Redirect("user-perfil?msgsend=ok", False)
        Catch ex As Exception
            Response.Redirect("user-perfil?msgsend=err", False)
        End Try

    End If
End Code
@section head
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
    
End Section
<div id="content">
    <h3 class="user_title" style="display:inline-block">Cuéntale a un asesor sobre tu proyecto.</h3>
    <div class="clearfix"></div>
    <div class="row">
        <div class="col-md-12">
            <div class="asesoria-box">
                <p class="fields_wrap">Recuerda que en Poolin contamos con un equipo de expertos que buscan apoyarte para llevar tu idea o proyecto al siguiente nivel. Al llenar este formulario, un asesor de negocios se pondrá en contacto contigo para revisar el estatus de tu proyecto.</p>
                <form class="asesoria" method="post" onsubmit="return checkloc(this);">
                    <input type="hidden" name="send" value="ok" />
                    <div class="fields_wrap">
                        <div class="row">
                        <div class="col-md-12">
                            <div class="light-blue">¿Con qué necesitas ayuda?</div>
                            <span id="valayuda" class="msg-val"></span>
                                <ul id="recomend">
                                    <li>
                                        <input type="checkbox" name="idea" id="idea"  onclick="checkloc(this);"   />
                                        <label for="Idea">Idea</label>
                                    </li>
                                    <li>
                                        <input type="checkbox" name="proyecto" id="proyecto"   onclick="checkloc(this);"  />
                                        <label for="Proyecto">Proyecto</label>
                                    </li>
                                    <li>
                                        <input type="checkbox" name="negocio" id="negocio" onclick="checkloc(this);"   />
                                        <label for="Negocio">Negocio</label>
                                    </li>
                                    <li>
                                        <input type="checkbox" name="empresa" id="empresa" onclick="checkloc(this);"   />
                                        <label for="Empresa">Empresa</label>
                                    </li>
                                </ul>
                        </div>
                    </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="light-blue">Nombre del proyecto / Negocio:</div>
                                <span class="msg-val">@Html.ValidationMessage("nom-proyecto")</span>
                                <input type="text" name="nom-proyecto"  class="form-control" @Validation.For("nom-proyecto")>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="light-blue">Describe tu idea o proyecto:</div>
                                <span class="msg-val">@Html.ValidationMessage("desc-proyecto")</span>
                                <textarea class="form-control" name="desc-proyecto" @Validation.For("desc-proyecto")></textarea>
                            </div>
                        </div>
                    </div>
                    <div class="fields_wrap">
                        <div class="row">
                            <div class="col-md-3">
                                <div class="light-blue">Fecha de inicio:</div>
                                <span class="msg-val">@Html.ValidationMessage("fecha-proyecto")</span>
                                <input type="date" name="fecha-proyecto"  class="form-control" @Validation.For("fecha-proyecto") >
                            </div>
                            <div class="col-md-5">
                                <div class="light-blue">Número de personas en el equipo:</div>
                                <span class="msg-val">@Html.ValidationMessage("num-proyecto")</span>
                                <input type="number" name="num-proyecto" class="form-control" @Validation.For("num-proyecto") >
                            </div>
                            <div class="col-md-4">
                                <div class="light-blue">Ciudad:</div>
                                <span class="msg-val">@Html.ValidationMessage("ciudad-proyecto")</span>
                                <input type="text" name="ciudad-proyecto" class="form-control" @Validation.For("ciudad-proyecto") >
                            </div>
                        </div>
                    </div>
                    <div class="row fields_wrap">
                        <div class="clearfix"></div>
                        <div class="col-md-6">
                            <div class="light-blue">Correo de contacto:</div>
                            <span class="msg-val">@Html.ValidationMessage("email-proyecto")</span>
                            <input type="email" name="email-proyecto" class="form-control" @Validation.For("email-proyecto") >
                        </div>
                        <div class="col-md-6">
                            <div class="light-blue">Teléfono de contacto:</div>
                            <input type="text" name="tel-proyecto" class="form-control" >
                        </div>
                    </div>
                    <div class="row fields_wrap">
                        <div class="col-md-12">
                            <div class="light-blue">Horario preferencial para contactarte:</div>
                            <span id="valhorario" class="msg-val"></span>
                            <ul id="recomend">
                                <li>
                                    <input type="checkbox" name="loc-manana" id="manana" onclick="checkloc(this);"  />
                                    <label for="mañana">Mañana</label>
                                </li>
                                <li>
                                    <input type="checkbox" name="loc-tarde" id="tarde"  onclick="checkloc(this);"  />
                                    <label for="tarde">Tarde</label>
                                </li>
                                <li>
                                    <input type="checkbox" name="loc-noche" id="noche" onclick="checkloc(this);"  />
                                    <label for="noche">Noche</label>
                                </li>
                            </ul>
                        </div>                            
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <button type="submit" class="btn btn-primary pull-right">ENVIAR</button>
                        </div>
                    </div>

                </form>
            </div>
        </div>
    </div>
</div>
@section Scripts
    <script>
        function checkloc(valor) {
            var flag1 = true;
            var flag2 = true;
            //idea, negocio, empresa, proyecto
            // && document.getElementById("tarde").checked == "false" && document.getElementById("noche").checked == "false"
            if (document.getElementById("manana").checked == false && document.getElementById("tarde").checked == false && document.getElementById("noche").checked == false) {
                document.getElementById("valhorario").innerText = "Este campo es obligatorio";
                flag1 = false;
            }
            else {
                document.getElementById("valhorario").innerText = "";
            }
            

            if (document.getElementById("idea").checked == false && document.getElementById("negocio").checked == false && document.getElementById("empresa").checked == false && document.getElementById("proyecto").checked == false) {
                document.getElementById("valayuda").innerText = "Este campo es obligatorio";
                flag2 = false;
            }
            else {
                document.getElementById("valayuda").innerText = "";
            }

            if (flag1 == false) {
                document.getElementById("manana").focus;
                return false;
            }

            if (flag2 == false) {
                document.getElementById("idea").focus;
                return false;
            }

            return true;
        }
    </script>
End Section