<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="OExm.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" runat="server">

<style>

:root{
    --primary:#2563EB;
    --primary-dark:#1D4ED8;

    --bg:#0F172A;
    --card:#1E293B;
    --card-hover:#243247;

    --border:#334155;

    --text:#F8FAFC;
    --text-light:#94A3B8;

    --success:#22C55E;
    --warning:#F59E0B;
    --danger:#EF4444;
    --purple:#8B5CF6;
    --teal:#14B8A6;
}

.dashboard-container{
    width:100%;
}

/* ===========================
        WELCOME CARD
===========================*/

.welcome-card{

    background:linear-gradient(135deg,#1E3A8A,#2563EB);

    color:white;

    padding:30px;

    border-radius:18px;

    margin-bottom:25px;

    box-shadow:0 15px 35px rgba(37,99,235,.25);

}

.welcome-card h2{

    margin:0 0 10px;

    font-size:28px;

}

.welcome-card p{

    color:#E2E8F0;

    margin:0;

}

/* ===========================
        DASHBOARD GRID
===========================*/

.dashboard-grid{

    display:grid;

    grid-template-columns:repeat(auto-fit,minmax(220px,1fr));

    gap:20px;

    margin-bottom:30px;

}

/* ===========================
        STAT CARD
===========================*/

.stat-card{

    background:var(--card);

    border:1px solid var(--border);

    border-radius:18px;

    padding:25px;

    text-align:center;

    transition:.3s;

    box-shadow:0 10px 25px rgba(0,0,0,.20);

}

.stat-card:hover{

    transform:translateY(-6px);

    border-color:var(--primary);

    box-shadow:0 20px 40px rgba(37,99,235,.20);

}

.stat-card i{

    font-size:32px;

    margin-bottom:15px;

}

.stat-card h2{

    margin:0;

    font-size:30px;

    color:white;

}

.stat-card p{

    margin-top:10px;

    color:var(--text-light);

    font-size:15px;

}

/* ===========================
      ICON COLORS
===========================*/

.students i{
    color:#3B82F6;
}

.exams i{
    color:#22C55E;
}

.questions i{
    color:#F59E0B;
}

.attempts i{
    color:#8B5CF6;
}

.violations i{
    color:#EF4444;
}

.published i{
    color:#14B8A6;
}

.passrate i{
    color:#3B82F6;
}

.average i{
    color:#F59E0B;
}

/* ===========================
        SECTION CARD
===========================*/

.section-card{

    background:var(--card);

    border:1px solid var(--border);

    border-radius:18px;

    padding:25px;

    margin-bottom:25px;

    box-shadow:0 10px 25px rgba(0,0,0,.20);

    transition:.3s;

}

.section-card:hover{

    border-color:var(--primary);

    box-shadow:0 15px 35px rgba(37,99,235,.15);

}

/* ===========================
        TITLE
===========================*/

.section-title{

    display:flex;

    align-items:center;

    gap:12px;

    margin-bottom:20px;

    color:white;

}

.section-title i{

    color:#3B82F6;

    font-size:22px;

}

.section-title h3{

    margin:0;

}

/* ===========================
        GRIDVIEW
===========================*/

.gridview{

    width:100%;

    border-collapse:collapse;

    border-radius:12px;

    overflow:hidden;

}

.gridview th{

    background:#1D4ED8;

    color:white;

    padding:14px;

    text-align:left;

    font-weight:600;

    border:none;

}

.gridview td{

    padding:14px;

    background:#1E293B;

    color:#F8FAFC;

    border-bottom:1px solid #334155;

}

.gridview tr:nth-child(even) td{

    background:#243247;

}

.gridview tr:hover td{

    background:#334155;

    transition:.2s;

}

/* ===========================
      ASP.NET SELECTED ROW
===========================*/

.gridview .selected td{

    background:#2563EB !important;

    color:white;

    font-weight:bold;

}

/* ===========================
        SCROLLBAR
===========================*/

.table-container{

    overflow-x:auto;

    border-radius:14px;

}

/* ===========================
        RESPONSIVE
===========================*/

@media(max-width:768px){

.dashboard-grid{

    grid-template-columns:1fr;

}

.welcome-card{

    padding:20px;

}

.welcome-card h2{

    font-size:22px;

}

.stat-card{

    padding:20px;

}

.section-card{

    padding:20px;

}

.gridview{

    font-size:14px;

}

}
</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="dashboard-container">

<!-- Welcome -->

<div class="welcome-card">

    <h2>
        Welcome Back,
        <%= Session["FullName"] %> 👋
    </h2>

    <p>
        Manage exams, students, questions and monitor platform activities from one place.
    </p>

</div>

<!-- Statistics -->

<div class="dashboard-grid">

    <div class="stat-card students">
        <i class="fa-solid fa-users"></i>
        <h2><asp:Label ID="lblStudents" runat="server"></asp:Label></h2>
        <p>Total Students</p>
    </div>

    <div class="stat-card exams">
        <i class="fa-solid fa-file-signature"></i>
        <h2><asp:Label ID="lblExams" runat="server"></asp:Label></h2>
        <p>Total Exams</p>
    </div>

    <div class="stat-card attempts">
        <i class="fa-solid fa-clipboard-check"></i>
        <h2><asp:Label ID="lblAttempts" runat="server"></asp:Label></h2>
        <p>Total Attempts</p>
    </div>

    <div class="stat-card published">
        <i class="fa-solid fa-bullhorn"></i>
        <h2><asp:Label ID="lblPublishedExams" runat="server"></asp:Label></h2>
        <p>Published Exams</p>
    </div>

    <div class="stat-card passrate">
        <i class="fa-solid fa-chart-pie"></i>
        <h2><asp:Label ID="lblPassRate" runat="server"></asp:Label></h2>
        <p>Pass Percentage</p>
    </div>

    <div class="stat-card average">
        <i class="fa-solid fa-star"></i>
        <h2><asp:Label ID="lblAverageScore" runat="server"></asp:Label></h2>
        <p>Average Score</p>
    </div>

    <div class="stat-card violations">
        <i class="fa-solid fa-triangle-exclamation"></i>
        <h2><asp:Label ID="lblTotalViolations" runat="server"></asp:Label></h2>
        <p>Total Violations</p>
    </div>

    <div class="stat-card violations">
        <i class="fa-solid fa-user-secret"></i>
        <h2><asp:Label ID="lblStudentsFlagged" runat="server"></asp:Label></h2>
        <p>Students Flagged</p>
    </div>

</div>

<!-- Recent Activities -->

<div class="section-card">

    <div class="section-title">
        <i class="fa-solid fa-clock-rotate-left"></i>
        <h3>Recent Activities</h3>
    </div>

    <asp:GridView ID="gvLogs"
        runat="server"
        CssClass="gridview">
    </asp:GridView>

</div>

<!-- Top Performers -->

<div class="section-card">

    <div class="section-title">
        <i class="fa-solid fa-trophy"></i>
        <h3>Top 5 Performers</h3>
    </div>

    <asp:GridView ID="gvTopPerformers"
        runat="server"
        CssClass="gridview">
    </asp:GridView>

</div>

<!-- Violations -->

<div class="section-card">

    <div class="section-title">
        <i class="fa-solid fa-shield-halved"></i>
        <h3>Exam Violations</h3>
    </div>

    <p style="color: #6B7280; margin-top: -10px; margin-bottom: 15px;">
        Every logged tab-switch during an exam, most recent first.
    </p>

    <asp:GridView ID="gvViolations"
        runat="server"
        CssClass="gridview"
        AutoGenerateColumns="true"
        GridLines="None"
        Width="100%">
    </asp:GridView>

</div>

</div>

</asp:Content>
