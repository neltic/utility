<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="neltic.web.test.Default" %>

<%@ Import Namespace="neltic.environ" %>
<!DOCTYPE html>
<html>
<head>
    <title>neltic.laboratory</title>
    <link href="style/api.all.css" rel="stylesheet" />
    <link href="style/default.css" rel="stylesheet" />
    <script src="script/jquery.min.js"></script>
    <script src="script/jquery.ui.min.js"></script>
    <script src="<%=neltic.environ.Label.GetScriptPath() %>"></script>
    <script src="script/all.js"></script>
    <script type="text/javascript">
        $(function () {
            var service = "Default.aspx/";

            $("#go").button();
            $("#go").on("click", function () {
                $.WebMethod(service + "Reload", function (result) {
                    if (result) {
                        document.location.reload();
                    }
                });

            })
        });
    </script>
</head>
<body>
    <h1 data-label="PageNotFound"></h1>
    <h2 data-label="NotEditable"></h2>
    <ul>
        <li data-label="Create"></li>
        <li data-label="New"></li>
        <li data-label="Change"></li>
        <li data-label="Apply"></li>
        <li data-label="Next"></li>
        <li data-label="Import"></li>
    </ul>
    <span runat="server" id="DisplayDemoData"></span>
    <br />
    <button id="go" data-label="Go"></button>
</body>
</html>

