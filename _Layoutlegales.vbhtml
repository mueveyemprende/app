<!DOCTYPE html>
<html lang="es">
<head>
    <title>M&E - @PageData("Title")</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="keywords" content="marketing, freelancer, freelance, marketing online, proyecto de empresa, trabajo desde casa por internet, proyectos emprendedores, freelance marketing, trabajos en internet, marketing freelance, freelancer online, escritor freelance, consultoria marketing digital, freelancers, los mejores freelancers, trabaja como freelancer, mejores freelancers de, estrategias de marketing, crea un proyecto, mejores freelancers de habla, de los mejores freelancers, publica tu proyecto, y gana dinero, trabajos freelance en ,publicar un proyecto como, freelance en línea, trabajos freelance en línea, publicar un proyecto como ">
    <meta name="title" content="Trabaja con los mejores emprendedores de latinoamérica – M&E.">
    <meta name="description" content="Publica vacantes, ideas o proyectos y trabaja con los mejores emprendedores de diseño gráfico, diseño web, programadores, marketing digital y escritores de latinoamérica.">
    <meta name="author" content="Mueve y Emprende Co.">

    <meta http-equiv="X-UA-Compatible" content="IE=8; IE=9; IE=10; IE=edge">

    <meta property="og:title" content="Trabaja con los mejores emprendedores de latinoamérica – M&E" />
    <meta property="og:type" content="website" />
    <meta property="og:url" content="https://mueveyemprende.io" />
    <meta property="og:image" content="https://mueveyemprende.io/images/logofb.gif" />
    <meta property="og:description" content="Publica vacantes, ideas o proyectos y trabaja con los mejores emprendedores de diseño gráfico, diseño web, programadores, marketing digital y escritores de latinoamérica." />


    <link rel="shortcut icon" type="image/png" href="images/favicon.png" />

	<link rel="stylesheet" href="css/bootstrap.min.css?2.0">
	<link rel="stylesheet" href="css/style.css?2.1">
	<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.3.1/css/all.css" integrity="sha384-mzrmE5qonljUremFsqc01SB46JvROS7bZs3IO2EmfFsd15uHvIt+Y8vEf7N7fWAU" crossorigin="anonymous">

    @code
    End Code
    @RenderSection("headpool", False)
</head>
<body>

    @RenderBody()

<script src="https://code.jquery.com/jquery-3.1.1.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js"></script>
    @RenderSection("script", False)
<script>
    $(document).ready(function () {
        $('#sidecollapse').on('click', function () {
            $('#sidebar').toggleClass('active');
        });

        $('#idioma').click(function(){
            $('#ul-idiomas').show();
        });
        $('#a-close').click(function () {
            $('#ul-idiomas').hide();
        });
    });

    function cambiaidioma(valor) {
        var faq = "@PageData("pagina")";
        if (faq == "") {
            document.getElementById("frmidioma-" + valor).submit();
        }
        else {
            document.getElementById("frmidioma-" + valor).action = faq + "-" + valor;
            document.getElementById("frmidioma-" + valor).submit();
        }

    }
</script>
    <!-- JavaScript -->
	<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script>
    <script src="js/bootstrap.min.js"></script>
</body>
</html>