<!DOCTYPE html>
<html lang="es">
<head>
    <title>M&E @PageData("Title")</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="google-signin-client_id" content="221237620599-bf0ebljub3godpd02792dtjgd688qnnr.apps.googleusercontent.com">
    <link rel="shortcut icon" type="image/png" href="images/favicon.png" />


    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css" integrity="sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB" crossorigin="anonymous">
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">
    <link rel="stylesheet" type="text/css" href="css/poolin-stylesheet.css?2.2">
    <link rel="stylesheet" type="text/css" href="css/landing.css?3.2">

    <!--Script para funcionamiento Poolin-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <link href="https://use.fontawesome.com/releases/v5.0.8/css/all.css" rel="stylesheet">
    <!--Script para funcionamiento Poolin-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    
    <script src="~/Scripts/jquery.validate.min.js"></script>
    <script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
    @*<script src="https://apis.google.com/js/platform.js" async defer></script>*@
    <script src="https://apis.google.com/js/client:platform.js?onload=startApp"></script>

    @RenderSection("head", required:=False)
</head>
<body id="@PageData("idpage")">
    @RenderSection("featured", required:=False)
    @RenderBody()
    @RenderSection("Scripts", required:=False)
</body>
</html>
