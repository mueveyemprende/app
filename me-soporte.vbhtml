
@code
    'If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
    '    Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    'End If

    Layout = "_Layoutlegales.vbhtml"
    PageData("Title") = "Preguntas Frecuentes"
    PageData("pagina") = "me-soporte"

    Dim viewmsg = ""
    If Request.Form("soporte") = "ok" Then
        Try
            viewmsg = "Gracias por usar el soporte Mueve y Emprende. Tu mensaje fue enviado con éxito. En breve uno de nuestros asesores se pondra en contacto contigo."
            Dim objSend As New Mueve y Emprende_class.cSendGrid
            Dim apkey As String = ConfigurationManager.AppSettings("sendgridkey").ToString
            Dim htmlmsg = "Nombre: " & Request.Form("nombre") & "<br>"
            htmlmsg &= "Email: " & Request.Form("mail") & "<br>"
            htmlmsg &= "Mensaje: " & Request.Form("mensaje") & "<br>"
            objSend.Correo_SMTP("SOPORTE M&E", htmlmsg, "soporte@mueveyemprende.io", "", ConfigurationManager.ConnectionStrings("SQLConn").ToString)
        Catch ex As Exception
            viewmsg = ex.Message
        End Try

    End If
End Code

<section>
    <div class="container">
        <div class="row">
            <div class="col-12">
                <h2 class="blue-1">Preguntas frecuentes</h2>
                @*<h3 class="blue-3"></h3>*@
                <div class="custom-tabs">
                    <nav class="nav nav-tabs nav-justified">
                        <a class="nav-link active" href="#Mueve y Emprende" data-toggle="tab">Mueve y Emprende</a>
                        <a class="nav-link" href="#contratar" data-toggle="tab">CONTRATAR</a>
                        <a class="nav-link" href="#trabajar" data-toggle="tab">TRABAJAR</a>
                    </nav>
                    <div class="tab-content">
                        <div class="tab-pane show active" id="Mueve y Emprende">                        
                            <div class="accordion-container">
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué tipo de proyectos se pueden publicar en Mueve y Emprende?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>El objetivo principal de Mueve y Emprende es ofrecerte nuestra plataforma para que puedas ofrecer tus servicios y solicitar apoyo de otros emprendedores a desarrollar tus proyectos, por lo que el tipo de proyectos a publicar son tan variados que no existe categorías definidas, ni exclusivas.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cómo gana la plataforma de Mueve y Emprende?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Al ser un intermediario entre usuarios, Mueve y Emprende solicita una comisión por cada transacción realizada a través de <a href="~/me-seguro">M&E seguro</a>. </p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué es M&E seguro?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Para garantizar tu seguridad, desarrollamos un módulo de pago con todos los protocolos de seguridad al que llamamos <em>M&E seguro</em>. </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="contratar">
                            <div class="accordion-container">
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué debo hacer después de registrarme?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Define tu proyecto. Describe los objetivos, las habilidades que está buscando, productos a entregar, un presupuesto y la fecha límite deseada. Una vez que estés listo, publica el proyecto y nuestro sistema instantáneamente lo agregará a nuestro Pool donde los profesionales independientes podrán presentarte sus propuestas.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cómo le hago para que mi proyecto sea atractivo?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Un proyecto es atractivo cuando se detallan las necesidades con claridad, permitiendo que profesionales independientes comprendan el alcance lo más exacto posible y puedan presentar una propuesta más precisa.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cómo realizo el pago y cuánto cuesta?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Mueve y Emprende ofrece su módulo de pago entre usuarios que le llamamos <a href="~/me-seguro">M&E seguro</a>. El monto por proyecto lo podrás negociar directamente con el emprendedor de acuerdo al alcance, tiempo y resultados esperados.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Puedo solicitar factura?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Depende. Por un lado, deberás revisar directamente con el emprendedor que contrates si te puede generar factura por sus servicios. Mueve y Emprende, por su lado, podrá emitir factura -si así lo solicitas- en relación a la comisión que recibe. </p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué pasa si no me gustan los resultados?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>La mayoría de los proyectos en Mueve y Emprende se completan a tiempo y forma, pero si es necesario, te ofreceremos ayuda para que puedas negociar con tu emprendedor. Con <a href="~/me-seguro">M&E seguro</a>, te damos el control para liberar tu dinero hasta que estés satisfecho con los resultados.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿A quién le pertenecen los derechos legales de los proyectos finalizados? <i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Los derechos legales por los proyectos finalizados para a ser exclusivamente del usuario que solicita y paga por el proyecto cuando este ha finalizado.</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="trabajar">
                            <div class="accordion-container">
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué hago después de registrarme?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Llena tu perfil emprendedor y deja que los demás conozcan lo que haces. Después, entra al Pool y comienza a buscar proyectos en donde puedas aplicar con una propuesta competente.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cómo encuentro los proyectos apropiados para mi perfil?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>En nuestro Pool, tendrás la opción de buscar dentro del listado de proyectos recientes o realizar búsquedas por palabras clave, incluso filtrar los resultados de la búsqueda por categorías, <a href="#">______________</a>. .</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cómo me ayuda Mueve y Emprende a administrar mi trabajo?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Mueve y Emprende ofrece a sus usuarios herramientas de trabajo que le ayudan a administrar sus proyectos de manera eficiente y de manera transparente a los usuarios contratantes, como calendario de citas, mensajes privados y chat.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Qué tipo de clientes puedo encontrar en Mueve y Emprende?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Los usuarios de Mueve y Emprende son personas emprendedoras que buscan desarrollarse y crecer haciendo lo que más les gusta: trabajar. El talento y el compromiso son grandes características de los usuarios que forman parte de la comunidad Mueve y Emprende, por lo que estamos seguros que clientes de diversos lugares, edades e incluso idiomas formarán también parte de la comunidad Mueve y Emprende.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cuánto podré ganar?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p><em>El cielo es el límite.</em> Puedes ganar, tanto como estés dispuesto a trabajar. No hay límites.</p>
                                    </div>
                                </div>
                                <div class="content-entry">
                                    <h4 class="article-title">
                                        ¿Cuánto me cuesta trabajar en Mueve y Emprende?<i class="fas"></i>
                                    </h4>
                                    <div class="accordion-content">
                                        <p>Podrás registrarte sin costo alguno y comenzar a ofrecer tus servicios. Ahora bien, Mueve y Emprende no cobra membresías por pertenecer a la comunidad de emprendedores, sino que obtiene una comisión pequeña por cada transacción que recibes a través de M&E.</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
<section>
    <div class="container">
        <div class="row justify-content-center center">
            <div class="col-12">
                <h2 class="blue-1">¿No has encontrado la respuesta a tu pregunta?</h3>
                <p>Escríbenos a <a href="mailto:soporte@mueveyemprende.io">soporte@mueveyemprende.io</a> o déjanos un mensaje a través de nuestro formulario.</p>
            </div>
            <div class="clearfix"></div>
            <div class="col-sm-6">
                <form class="soporte" accept-charset="UTF-8" action="" method="post">
                    @*<input type="text" name="nombre"  placeholder="Nombre">
                    <input type="text" name="mail"  placeholder="E-mail">
                    <textarea name="mensaje"  placeholder="Tu mensaje"></textarea>
                    <input type="submit" value="Enviar" class="btn white bg-blue-2 pull-right">*@

                    <input type="hidden" name="soporte" value="ok" />
                    <input type="text" name="nombre" placeholder="Nombre" required />
                    <input type="email" name="mail" placeholder="E-mail" required />
                    <textarea name="mensaje" placeholder="Tu mensaje" required></textarea>
                    <input type="submit" value="Enviar" class="btn btn-primary pull-right">

                </form>
            </div>
            <div class="clearfix"></div>
            <div class="col-sm-12">
                <p>Oficina registrada:<br>Centro Comercial Espacio Esmeralda<br>Av. Doctor Jiménez Cantú, Col. Bosque Esmeralda, C.P. 52930<br>Atizapan de Zaragoza, Estado de México</p>
            </div>
        </div>
    </div>
</section>

@*<div id="content">
    <div class="container">
        <h2 class="soporte-h2">Soporte</h2>
        <h3>Preguntas frecuentes</h3>

        <div class="tabbable">
            <ul class="nav nav-tabs">
                <li class="active"><a href="#Mueve y Emprende" data-toggle="tab">Acerca de Mueve y Emprende</a></li>
                <li><a href="#contratar" data-toggle="tab">Acerca de contratar</a></li>
                <li><a href="#trabajar" data-toggle="tab">Acerca de trabajar</a></li>
            </ul>
            <div class="tab-content">
                <div class="tab-pane active" id="Mueve y Emprende">
                    <div class="accordion-container">
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué tipo de proyectos se pueden publicar en Mueve y Emprende?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>El objetivo principal de Mueve y Emprende es ofrecerte nuestra plataforma para que puedas ofrecer tus servicios y solicitar apoyo de otros emprendedores a desarrollar tus proyectos, por lo que el tipo de proyectos a publicar son tan variados que no existe categorías definidas, ni exclusivas.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cómo gana la plataforma de Mueve y Emprende?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Al ser un intermediario entre usuarios, Mueve y Emprende solicita una comisión por cada transacción realizada a través de <a href="Mueve y Emprende-seguro.html">M&E</a>. Para revisar las comisiones, visita nuestra página de <a href="#">____________</a></p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué es M&E?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Para garantizar tu seguridad, desarrollamos un módulo de pago con todos los protocolos de seguridad al que llamamos <em>M&E</em>. Para conocer más sobre esto, visita <a href="#">____________</a></p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="contratar">
                    <div class="accordion-container">
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué debo hacer después de registrarme?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Define tu proyecto. Describe los objetivos, las habilidades que está buscando, productos a entregar, un presupuesto y la fecha límite deseada. Una vez que estés listo, publica el proyecto y nuestro sistema instantáneamente lo agregará a nuestro Pool donde los profesionales independientes podrán presentarte sus propuestas.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cómo le hago para que mi proyecto sea atractivo?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Un proyecto es atractivo cuando se detallan las necesidades con claridad, permitiendo que profesionales independientes comprendan el alcance lo más exacto posible y puedan presentar una propuesta más precisa.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cómo realizo el pago y cuánto cuesta?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p> Mueve y Emprende ofrece su módulo de pago entre usuarios que le llamamos <a href="Mueve y Emprende-seguro.html">M&E</a>. El monto por proyecto lo podrás negociar directamente con el emprendedor de acuerdo al alcance, tiempo y resultados esperados.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Puedo solicitar factura?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Depende. Por un lado, deberás revisar directamente con el emprendedor que contrates si te puede generar factura por sus servicios. Mueve y Emprende, por su lado, podrá emitir factura -si así lo solicitas- en relación a la comisión que recibe.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué pasa si no me gustan los resultados?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>La mayoría de los proyectos en Mueve y Emprende se completan a tiempo y forma, pero si es necesario, te ofreceremos ayuda para que puedas negociar con tu emprendedor. Con <a href="Mueve y Emprende-seguro.html">M&E</a>, te damos el control para liberar tu dinero hasta que estés satisfecho con los resultados.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿A quién le pertenecen los derechos legales de los proyectos finalizados?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Los derechos legales por los proyectos finalizados para a ser exclusivamente del usuario que solicita y paga por el proyecto cuando este ha finalizado.</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="tab-pane" id="trabajar">
                    <div class="accordion-container">
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué hago después de registrarme?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Llena tu perfil emprendedor y deja que los demás conozcan lo que haces. Después, entra al Pool y comienza a buscar proyectos en donde puedas aplicar con una propuesta competente.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cómo encuentro los proyectos apropiados para mi perfil?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>En nuestro Pool, tendrás la opción de buscar dentro del listado de proyectos recientes o realizar búsquedas por palabras clave, incluso filtrar los resultados de la búsqueda por categorías, <a href="#">______________</a>.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cómo me ayuda Mueve y Emprende a administrar mi trabajo?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Mueve y Emprende ofrece a sus usuarios herramientas de trabajo que le ayudan a administrar sus proyectos de manera eficiente y de manera transparente a los usuarios contratantes, como calendario de citas, mensajes privados y chat.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Qué tipo de clientes puedo encontrar en Mueve y Emprende?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Los usuarios de Mueve y Emprende son personas emprendedoras que buscan desarrollarse y crecer haciendo lo que más les gusta: trabajar. El talento y el compromiso son grandes características de los usuarios que forman parte de la comunidad Mueve y Emprende, por lo que estamos seguros que clientes de diversos lugares, edades e incluso idiomas formarán también parte de la comunidad Mueve y Emprende.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cuánto podré ganar?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p><em>El cielo es el límite.</em> Puedes ganar, tanto como estés dispuesto a trabajar. No hay límites.</p>
                            </div>
                        </div>
                        <div class="content-entry">
                            <h4 class="article-title">
                                ¿Cuánto me cuesta trabajar en Mueve y Emprende?<i class="fas"></i>
                            </h4>
                            <div class="accordion-content">
                                <p>Podrás registrarte sin costo alguno y comenzar a ofrecer tus servicios. Ahora bien, Mueve y Emprende no cobra membresías por pertenecer a la comunidad de emprendedores, sino que obtiene una comisión pequeña por cada transacción que recibes a través de M&E.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="not-found">
            <h3 class="user_title">¿No has encontrado la respuesta a tu pregunta?</h3>
            <p>Escríbenos a <a href="mailto:soporte@mueveyemprende.io">soporte@mueveyemprende.io</a> o déjanos un mensaje a través de nuestro formulario.</p>
            <div class="col-md-6 col-md-offset-3">
                <form class="soporte" accept-charset="UTF-8" action="soporte" method="post">
                    <input type="hidden" name="soporte" value="ok" />
                    <input type="text" name="nombre" placeholder="Nombre" required />
                    <input type="email" name="mail" placeholder="E-mail" required />
                    <textarea name="mensaje" placeholder="Tu mensaje" required></textarea>
                    <input type="submit" value="Enviar" class="btn btn-primary pull-right">
                    <div class="clearfix"></div>
                </form>
            </div>
            <div class="clearfix"></div>
            <div class="col-md-12">
                <p>Oficina registrada:<br>Centro Comercial Espacio Esmeralda<br>Av. Doctor Jiménez Cantú, Col. Bosque Esmeralda, C.P. 52930<br>Atizapan de Zaragoza, Estado de México</p>
            </div>
        </div>
    </div>
</div>*@

@section script
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script src="js/jquery.jeditable.min.js"></script>
    <script type="text/javascript">
        $(function () {
            var Accordion = function (el, multiple) {
                this.el = el || {};
                this.multiple = multiple || false;

                var links = this.el.find('.article-title');
                links.on('click', {
                    el: this.el,
                    multiple: this.multiple
                }, this.dropdown)
            }

            Accordion.prototype.dropdown = function (e) {
                var $el = e.data.el;
                $this = $(this),
                    $next = $this.next();

                $next.slideToggle();
                $this.parent().toggleClass('open');

                if (!e.data.multiple) {
                    $el.find('.accordion-content').not($next).slideUp().parent().removeClass('open');
                };
            }
            var accordion = new Accordion($('.accordion-container'), false);
        });
    </script>

End Section

