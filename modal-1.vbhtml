<!DOCTYPE html>
<html lang="en">

<head>
    <title>POOLIN</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" type="text/css" href="css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="css/Poolin-styles.css">
    
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">
</head>

<body>
    <header class="clearfix">
        <div id="Poolin-logo">
            <a href="#"><img src="images/Poolin-logo.svg"></a>
        </div>
        <div id="menu-Poolin-account">
            <ul>
                <li><a class="fas fa-exclamation-circle"></a></li>
                <li><a class="fas fa-user-circle"></a></li>
            </ul>
        </div>
    </header>
    <div id="content">
        <div class="container modal-1">
            <h3 class="user_title">Modal</h3>

            <!-- Estos son los botones que tienes en tu archivo -->
            <a href="#" id="reportar" class="btn btn-primary">REPORTAR<br>USUARIO</a>
            <a href="#" id="falla" class="btn btn-primary">REPORTAR<br>FALLA DEL SISTEMA</a>

            <!-- Este es el codigo para los popups -->
            <div id="reportar-usuario" class="modal fade centered-modal" role="dialog">
			    <div class="modal-dialog">
			        <div class="modal-content">
			            <div class="modal-body">
			            <button type="button" class="close" data-dismiss="modal">&times;</button>
			                <h3>Reportar usuario</h3>
			                <form class="form-modal" accept-charset="UTF-8" action="" method="post">
			                	<p><label>Usuario a reportar:</label>
		                        <input type="text" name="usuario-fail">
		                    	</p>
		                    	<p><label>Descripción de la falla:</label>
		                        <textarea name="descripcion"></textarea></p>
		                        <p><label>Tu usuario:</label>
		                        <input type="text" name="usuario">
		                    	</p>
		                        <input type="submit" value="Enviar" class="btn btn-primary pull-right">
		                        <div class="clearfix"></div>
		                    </form>
			            </div>
			        </div>
			    </div>
			</div>

			<div id="falla-sistema" class="modal fade centered-modal" role="dialog">
			    <div class="modal-dialog">
			        <div class="modal-content">
			            <div class="modal-body">
			            <button type="button" class="close" data-dismiss="modal">&times;</button>
			                <h3>Reportar falla de sistema</h3>
			                <form class="form-modal" accept-charset="UTF-8" action="" method="post">
			                	<p><label>Tu usuario:</label>
		                        <input type="text" name="usuario">
		                    	</p>
		                    	<p><label>Descripción de la falla:</label>
		                        <textarea name="descripcion"></textarea></p>
		                        <input type="submit" value="Enviar" class="btn btn-primary pull-right">
		                        <div class="clearfix"></div>
		                    </form>
			            </div>
			        </div>
			    </div>
			</div>


        </div>
    </div>
    </div>
    <footer class="clearfix">
        <div class="columna-footer" id="redes-poolin" style="width: 30%">
            <a href="/"><img src="images/Poolin-logo-bco.svg"></a>
            <a href="https://www.youtube.com/channel/UCzRwLD4g7JYHEEK0MCBa-yg"><i class="fab fa-youtube" style="color: #ADCED5; text-align: center;"></i></a>
            <a href="https://www.linkedin.com/company/poolinco"><i class="fab fa-linkedin-in" style="color: #ADCED5; text-align: center;"></i></a>
            <a href="https://www.facebook.com/emprendedorespoolin/?"><i class="fab fa-facebook-f" style="color: #ADCED5; text-align: center;"></i></a>
            <p style="font-family: Helvetica, Arial, sans-serif; font-size: 10px"> Copyright © 2017 Poolin. Todos los derechos reservados.</p>
        </div>
        <div class="columna-footer" style="width: 70%">
            <ul class="vinculos-footer">
                <li><a href="quienes-somos">Quiénes Somos</a></li>
                <li><a href="codigo-conducta-poolin">Código de Conducta</a></li>
                <li><a href="aviso-privacidad-poolin">Aviso de Privacidad</a></li>
            </ul>
            <ul class="vinculos-footer">
                <li><a href="terminos-poolin">Términos y Condiciones</a></li>
                <li><a href="poolin-seguro">Poolin Seguro</a></li>
                <li><a href="soporte-poolin">Soporte</a></li>
            </ul>
            <ul class="vinculos-footer">
                <li><a href="signin-me">Registrarse</a></li>
                <li><a href="login-me">Inicio de Sesión</a></li>
                <li><a href="contacto">Contacto</a></li>
            </ul>
        </div>
    </footer>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script src="js/jquery.jeditable.min.js"></script>
    <script type="text/javascript">
	    $(document).ready(function () {
		    $("#falla").click(function() {
		        $('#falla-sistema').modal('show');
		    });
		    $("#reportar").click(function() {
		        $('#reportar-usuario').modal('show');
		    });
		});
	</script>
</body>

</html>