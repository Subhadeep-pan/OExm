<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminViolations.aspx.cs" Inherits="OExm.AdminViolations" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

```
<title>Exam Violations | OExm Portal</title>

<meta name="viewport"
      content="width=device-width, initial-scale=1.0" />

<link rel="stylesheet"
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

<style>

    body{
        margin:0;
        font-family:'Segoe UI';
        background:#f3f4f6;
    }

    .container{
        max-width:1300px;
        margin:auto;
        padding:30px;
    }

    .header{
        background:linear-gradient(
            135deg,
            #dc2626,
            #991b1b);
        color:white;
        padding:25px;
        border-radius:15px;
        margin-bottom:25px;
    }

    .header h1{
        margin:0;
    }

    .card{
        background:white;
        padding:25px;
        border-radius:15px;
        box-shadow:0 2px 10px rgba(0,0,0,.1);
    }

    .gridview{
        width:100%;
        border-collapse:collapse;
    }

    .gridview th{
        background:#2563eb;
        color:white;
        padding:12px;
        text-align:left;
    }

    .gridview td{
        padding:12px;
        border-bottom:1px solid #e5e7eb;
    }

    .gridview tr:hover{
        background:#f9fafb;
    }

    .stats{
        display:grid;
        grid-template-columns:
        repeat(3,1fr);

        gap:20px;
        margin-bottom:25px;
    }

    .stat-card{
        background:white;
        padding:20px;
        border-radius:15px;
        text-align:center;
        box-shadow:0 2px 10px rgba(0,0,0,.1);
    }

    .stat-card h2{
        color:#dc2626;
        margin:10px 0;
    }

</style>
```

</head>

<body>

<form id="form1" runat="server">

<div class="container">

```
<div class="header">

    <h1>
        🚨 Exam Violations Monitor
    </h1>

    <p>
        Monitor suspicious activities and exam misconduct
    </p>

</div>

<div class="stats">

    <div class="stat-card">

        <i class="fa-solid fa-triangle-exclamation fa-2x"></i>

        <h2>
            <asp:Label ID="lblTotalViolations"
                runat="server">
            </asp:Label>
        </h2>

        <div>Total Violations</div>

    </div>

    <div class="stat-card">

        <i class="fa-solid fa-user-secret fa-2x"></i>

        <h2>
            <asp:Label ID="lblStudents"
                runat="server">
            </asp:Label>
        </h2>

        <div>Students Flagged</div>

    </div>

    <div class="stat-card">

        <i class="fa-solid fa-shield-halved fa-2x"></i>

        <h2>
            <asp:Label ID="lblToday"
                runat="server">
            </asp:Label>
        </h2>

        <div>Today's Violations</div>

    </div>

</div>

<div class="card">

    <h2>
        Violation Records
    </h2>

    <br />

    <asp:GridView
        ID="gvViolations"
        runat="server"
        CssClass="gridview"
        AutoGenerateColumns="true"
        GridLines="None"
        Width="100%">
    </asp:GridView>

</div>
```

</div>

</form>

</body>

</html>
