<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Leaderboard.aspx.cs" Inherits="OExm.Leaderboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Leaderboard | OExm Portal</title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link rel="stylesheet"
          href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

    <style>

        body{
            font-family:'Segoe UI';
            background:#f3f4f6;
            margin:0;
            padding:30px;
        }

        .container{
            max-width:1200px;
            margin:auto;
        }

        .header-card{
            background:linear-gradient(
                135deg,
                #2563eb,
                #4f46e5);
            color:white;
            padding:25px;
            border-radius:15px;
            margin-bottom:25px;
        }

        .header-card h1{
            margin:0;
        }

        .stats{
            display:grid;
            grid-template-columns:repeat(3,1fr);
            gap:20px;
            margin-bottom:25px;
        }

        .stat-card{
            background:white;
            padding:20px;
            border-radius:15px;
            box-shadow:0 2px 10px rgba(0,0,0,.1);
            text-align:center;
        }

        .stat-card h2{
            color:#2563eb;
            margin:10px 0;
        }

        .grid-card{
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
        }

        .gridview td{
            padding:12px;
            border-bottom:1px solid #e5e7eb;
        }

        .gridview tr:hover{
            background:#f9fafb;
        }

        @media(max-width:768px)
        {
            .stats{
                grid-template-columns:1fr;
            }
        }

    </style>

</head>

<body>

<form id="form1" runat="server">

<div class="container">

    <div class="header-card">

        <h1>
            🏆 Leaderboard
        </h1>

        <p>
            Top performing students across examinations
        </p>

    </div>

    <div class="stats">

        <div class="stat-card">

            <i class="fa-solid fa-users"></i>

            <h2>
                <asp:Label ID="lblStudents"
                    runat="server">
                </asp:Label>
            </h2>

            <div>Total Students</div>

        </div>

        <div class="stat-card">

            <i class="fa-solid fa-file-signature"></i>

            <h2>
                <asp:Label ID="lblAttempts"
                    runat="server">
                </asp:Label>
            </h2>

            <div>Total Attempts</div>

        </div>

        <div class="stat-card">

            <i class="fa-solid fa-trophy"></i>

            <h2>
                <asp:Label ID="lblTopScore"
                    runat="server">
                </asp:Label>
            </h2>

            <div>Highest Score</div>

        </div>

    </div>

    <div class="grid-card">

        <h2>
            Top Performers
        </h2>

        <br />

        <asp:GridView ID="gvLeaderboard"
            runat="server"
            CssClass="gridview"
            AutoGenerateColumns="false"
            GridLines="None">

            <Columns>

    <asp:BoundField
        DataField="Rank"
        HeaderText=" Rank" />

    <asp:BoundField
        DataField="FullName"
        HeaderText="Student Name" />

    <asp:BoundField
        DataField="Score"
        HeaderText="Highest Score" />

    <asp:BoundField
        DataField="ExamCount"
        HeaderText="Exams Attempted" />

</Columns>

        </asp:GridView>

    </div>

</div>

</form>

</body>
</html>