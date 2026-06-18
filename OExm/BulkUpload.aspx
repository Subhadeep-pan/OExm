<%@ Page Language="C#" AutoEventWireup="true"
CodeBehind="BulkUpload.aspx.cs"
Inherits="OExm.BulkUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Bulk Upload Questions</title>

    <style>

        body{
            font-family:'Segoe UI';
            background:#f5f7fb;
            margin:0;
            padding:0;
        }

        .container{
            width:700px;
            margin:50px auto;
        }

        .card{
            background:white;
            padding:30px;
            border-radius:12px;
            box-shadow:0 2px 10px rgba(0,0,0,.1);
        }

        .btn{
            background:#2563eb;
            color:white;
            border:none;
            padding:12px 20px;
            border-radius:8px;
            cursor:pointer;
        }

        .btn:hover{
            background:#1d4ed8;
        }

        .title{
            color:#2563eb;
            margin-bottom:20px;
        }

    </style>

</head>

<body>

<form id="form1" runat="server">

<div class="container">

<div class="card">

<h2 class="title">
📥 Bulk Upload Questions
</h2>

<p>
Upload Excel File (.xlsx)
</p>

<asp:FileUpload
    ID="fuExcel"
    runat="server" />

<br /><br />

<asp:Button
    ID="btnUpload"
    runat="server"
    Text="Upload Questions"
    CssClass="btn"
    OnClick="btnUpload_Click" />

<br /><br />

<asp:Label
    ID="lblMessage"
    runat="server"
    Font-Bold="true">
</asp:Label>

</div>

</div>

</form>

</body>
</html>