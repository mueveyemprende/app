@code
    If HttpContext.Current.Request.IsSecureConnection.Equals(False) Then
        'Response.Redirect("https://" + Request.ServerVariables("HTTP_HOST") + HttpContext.Current.Request.RawUrl)
    End If

    Layout = "_Layoutlegales.vbhtml"
    PageData("Title") = "Oferta"
    PageData("idpage") = "info"
    PageData("pagina") = "me-oferta"
End Code


<section>
	<div class="container">
		<div class="row justify-content-center">
			<div class="col-10">
				<h2 class="blue-1 center">Una nueva forma de trabajo en puerta</h2>
				<p>Mueve y Emprende está compuesto por un equipo de emprendedores mexicanos expertos en diferentes áreas, que trabajan en conjunto para ofrecer una plataforma web fácil y segura, donde más personas como nosotros puedan encontrar oportunidades de trabajo y llevar sus proyectos a otro nivel de la mano de asesores expertos de forma ágil y sencilla.</p>
				<h3 class="blue-3">Nuestro ideal</h3>
				<p>En Mueve y Emprende soñamos con encontrar el talento de cada persona y así, unirlos en un Pool donde puedan ser proyectados a miles de personas que requieren de sus habilidades, profesionalidad y compromiso, abriendo muchas oportunidades de negocios exitosos.</p>
                <p>Compartimos el ideal de ser un potenciadores de ideas y proyectos a través de asesorías personalizadas promoviendo a los emprendedores a trabajar para generar un impacto positivo y benéfico a la sociedad que le rodea.</p>
				<h3 class="blue-3">La nueva cultura de trabajo a distancia</h3>
				<p>¡La distancia ya no es pretexto!  Las fronteras de espacio y tiempo son parte del siglo XX. El Internet, y ahora los dispositivos móviles, permiten dos personas a estar en comunicación -incluso en vivo- a miles de kilómetros de distancia e incluso en otro uso horario. </p>
                <p>Existen diversas aplicaciones que permiten hacer una videollamada al mismo tiempo que se comparte la pantalla de la computadora para darle seguimiento a un proyecto.</p>
                <p>En Mueve y Emprende nos sumamos a esta nueva cultura, y buscamos aportar nuevas herramientas para fomentar en los emprendedores a adquirir nuevas oportunidades sin fronteras.</p>
                <p>¡Bienvenido a la cultura Mueve y Emprende!</p>
				<h3 class="blue-3">Oportunidades sin fronteras</h3>
				<p>A través de Mueve y Emprende, conectamos con profesionales independientes para trabajar en proyectos desde el desarrollo de aplicaciones web y móviles hasta SEO, marketing en redes sociales, redacción de contenido, diseño gráfico, ayuda de administración y miles de otros proyectos. Mueve y Emprende hace que sea rápido, simple y rentable buscar, contratar, trabajar y pagar a los mejores profesionales en cualquier lugar, en cualquier momento.</p>
                <h3 class="blue-3">Visión de Mueve y Emprende</h3>
                <p>Potenciar ideas, proyectos y negocios a través de una red de grandes talentos.</p>
			</div>
		</div>
	</div>
</section>