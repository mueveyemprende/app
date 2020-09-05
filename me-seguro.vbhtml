@code
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "_Layoutlegales.vbhtml"
    PageData("Title") = "Mueve y Emprende Seguro"
    PageData("idpage") = "seguro"
    PageData("pagina") = "me-seguro"
End Code
<section>
	<div class="container">
		<div class="row justify-content-center">
			<div class="col-8">
				<div class="col-12 center">
					<h2 class="blue-1">Mueve y Emprende seguro</h2>
					<p>En Mueve y Emprende queremos que te sientas seguro. Te presentamos la forma en la que nos encargamos de tu seguridad:</p>
				</div>
				<div class="row align-items-center pasos">
					<div class="col-sm-6">
						<h4 class="blue-3">Antes de comenzar a trabajar</h4>
						<ul>
							<li>La reputación es importante: conoce con quién vas a trabajar.</li>
							<li>Recibe distintas propuestas y elije la que más te guste.</li>
							<li>Revisa las calificaciones de tus candidados freelancers a elegir.</li>
							<li>Platica con ellos por inbox para acordar detalles y/o entrevistarlos.</li>
						</ul>
					</div>
					<div class="col-sm-6">
						<img src="images/Mueve y Emprende-seguro-1.svg">
					</div>
				</div>
				<div class="row align-items-center pasos">
					<div class="col-sm-6">
						<h4 class="blue-3">Cuando ya estás trabajando</h4>
						<ul>
							<li>Protección de pago seguro: Siéntete tranquilo; si no te ha gustado la propuesta que te han entregado, y no has quedado satisfecho, siempre puede regresar tu dinero a ti.</li>
							<li>Fondos congelados: Los fondos depositados no serán retirables hasta después de concluído el proyecto.</li>
						</ul>
					</div>
					<div class="col-sm-6">
						<img src="images/Mueve y Emprende-seguro-2.svg">
					</div>
				</div>
				<div class="row align-items-center pasos">
					<div class="col-sm-6">
						<h4 class="blue-3">Durante toda tu estancia en Mueve y Emprende</h4>
						<ul>
							<li>¿No estás conforme con alguna situación o usuario? Te ofrecemos asitencia a reclamos 24/7.</li>
							<li>¿Notas alguna vulnerabilidad en el sistema? Pónte en contacto con nosotros para brindarte asistencia.</li>
						</ul>
					</div>
					<div class="col-sm-6">
						<img src="images/Mueve y Emprende-seguro-3.svg">
					</div>
				</div>
				<div class="row center pasos">
					<div class="col-sm-12">
						<h4 class="blue-3">Ayúdanos a hacer de Mueve y Emprende una mejor experiencia.</h4>
						<ul class="justify-content-center nav nav-tabs">
		                    <li><a href="#" id="reportar" class="btn white bg-blue-2 up">Reportar usuario</a></li>
		                    <li><a href="#" id="falla" class="btn white bg-blue-2 up">Reportar falla del sistema</a></li>
		                </ul>
		                <div id="reportar-usuario" class="modal fade" role="dialog">
						    <div class="modal-dialog modal-dialog-centered">
						        <div class="modal-content">
						            <div class="modal-body">
						                <h3 class="blue-1">Reportar usuario</h3>
						                <form class="form-modal" accept-charset="UTF-8" action="" method="post">
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Usuario a reportar:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<input type="text" name="usuario-fail">
						                		</div>
						                	</div>
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Descripción del problema:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<textarea name="descripcion"></textarea>
						                		</div>
						                	</div>
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Tu usuario:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<input type="text" name="usuario">
						                		</div>
						                	</div>
						                	<input type="submit" value="Cancelar" class="cancel-btn pull-right">
						                	<input type="submit" value="Enviar" class="save-btn pull-right">
						                    <div class="clearfix"></div>
						                </form>
						            </div>
						        </div>
						    </div>
						</div>
						<div id="falla-sistema" class="modal fade" role="dialog">
						    <div class="modal-dialog modal-dialog-centered">
						        <div class="modal-content">
						            <div class="modal-body">
						                <h3 class="blue-1">Reportar falla de sistema</h3>
						                <form class="form-modal" accept-charset="UTF-8" action="" method="post">
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Tu usuario:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<input type="text" name="usuario">
						                		</div>
						                	</div>
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Descripción de la falla:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<textarea name="descripcion"></textarea>
						                		</div>
						                	</div>
						                	<div class="row">
						                		<div class="col-md-4">
						                			<p>Tu usuario:</p>
						                		</div>
						                		<div class="col-md-8">
						                			<input type="text" name="usuario">
						                		</div>
						                	</div>
						                	<input type="submit" value="Cancelar" class="cancel-btn pull-right">
						                	<input type="submit" value="Enviar" class="save-btn pull-right">
						                    <div class="clearfix"></div>
						                </form>
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

    <script type="text/javascript">
        $(document).ready(function () {
            $("#falla").click(function () {
                $('#falla-sistema').modal('show');
            });
            $("#reportar").click(function () {
                $('#reportar-usuario').modal('show');
            });
        });
    </script>

