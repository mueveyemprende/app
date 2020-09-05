<!DOCTYPE html>
<html lang="es">
<head>
    <title>M&E - Perfil </title>
    <meta http-equiv=”Expires” content=”0″>
    <meta http-equiv=”Last-Modified” content=”0″>
    <meta http-equiv=”Cache-Control” content=”no-cache, mustrevalidate”>
    <meta http-equiv=”Pragma” content=”no-cache”>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="shortcut icon" type="image/png" href="img/logo-carr.jpg" />
    @RenderSection("head", False)

    <link rel="stylesheet" type="text/css" href="css/bootstrap.css?1.5">
    <link rel="stylesheet" type="text/css" media="screen" href="css/font-awesome.min.css">
    <link rel="stylesheet" type="text/css" href="css/Poolin-styles.css?24.4">
    <link href="https://gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">

</head>
<body>
    <div class="wrapper">            
        @RenderBody()
    </div>

    @RenderSection("Scripts", required:=False)

	<!-- ================== BEGIN BASE JS ================== -->
	<script src="assets/plugins/jquery/jquery-3.2.1.min.js"></script>
	<script src="assets/plugins/jquery-ui/jquery-ui.min.js"></script>
	<script src="assets/plugins/cookie/js/js.cookie.js"></script>
	<script src="assets/plugins/tooltip/popper/popper.min.js"></script>
	<script src="assets/plugins/bootstrap/bootstrap4/js/bootstrap.min.js"></script>
	<script src="assets/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>
	<!-- ================== END BASE JS ================== -->

    <script>
        $(document).ready(function () {

        });

        var idioma = "ESP";

        switch (idioma) {
            case "ESP":
                document.getElementById("a-esp").innerText = "* Español";
                document.getElementById("a-ing").innerText = "Inglés";
                break;
            case "ING":
                document.getElementById("a-esp").innerText = "Spanish";
                document.getElementById("a-ing").innerText = "* English";
                break;
        }

    </script>

</body>
</html>