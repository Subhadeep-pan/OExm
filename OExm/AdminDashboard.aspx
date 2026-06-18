<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="OExm.AdminDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Admin Dashboard | OExm Portal</title>

    <link href="Content/style.css" rel="stylesheet" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

    <style>
        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit,minmax(220px,1fr));
            gap: 20px;
            margin-bottom: 25px;
        }

        .stat-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            text-align: center;
            box-shadow: 0 2px 10px rgba(0,0,0,.1);
            transition: .3s;
        }

            .stat-card:hover {
                transform: translateY(-5px);
            }

            .stat-card i {
                font-size: 30px;
                color: #2563eb;
                margin-bottom: 15px;
            }

            .stat-card h2 {
                margin: 10px 0;
                font-size: 28px;
                color: #111827;
            }

        .welcome-card {
            background: linear-gradient( 135deg, #2563eb, #4f46e5);
            color: white;
            padding: 25px;
            border-radius: 15px;
            margin-bottom: 25px;
        }
    </style>

</head>

<body>

    <form id="form1" runat="server" class="app-container">
        ```
        <!-- SIDEBAR -->

        <div class="sidebar">

            <div class="brand-section">

                <div class="brand-icon">
                    <i class="fa-solid fa-graduation-cap"></i>
                </div>

                <span class="brand-name">OExm Portal
                </span>

            </div>

            <div class="user-profile">

                <span class="user-name">
                    <%= Session["FullName"] %>
                </span>

                <span class="user-role">
                    <%= Session["Role"] %>
                </span>

            </div>

            <ul class="nav-menu">

                <li>
                    <a href="AdminDashboard.aspx" class="nav-item">
                        <i class="fa-solid fa-chart-line"></i>
                        Dashboard
                    </a>
                </li>

                <li>
                    <a href="ManageExams.aspx" class="nav-item">
                        <i class="fa-solid fa-file-signature"></i>
                        Exams
                    </a>
                </li>

                <li>
                    <a href="ManageSections.aspx" class="nav-item">
                        <i class="fa-solid fa-layer-group"></i>
                        Sections
                    </a>
                </li>

                <li>
                    <a href="ManageQuestions.aspx" class="nav-item">
                        <i class="fa-solid fa-circle-question"></i>
                        Questions
                    </a>
                </li>

                <li>
                    <a href="BulkUpload.aspx" class="nav-item">
                        <i class="fa-solid fa-file-excel"></i>
                        Bulk Upload
                    </a>
                </li>
                <li>
                    <a href="QuestionBank.aspx" class="nav-item">
                        <i class="fa-solid fa-database"></i>
                        Question Banks
                    </a>
                </li>

                <li>
                    <a href="AdminViolations.aspx" class="nav-item">
                        <i class="fa-solid fa-shield-halved"></i>
                        Violations
                    </a>
                </li>

                <li>
                    <a href="Leaderboard.aspx" class="nav-item">
                        <i class="fa-solid fa-trophy"></i>
                        Leaderboard
                    </a>
                </li>

                <li>
                    <a href="Login.aspx?logout=true" class="nav-item">
                        <i class="fa-solid fa-right-from-bracket"></i>
                        Logout
                    </a>
                </li>

                <li>
                    <a href="DatabaseManager.aspx"
                        class="nav-item">
                        <i class="fa-solid fa-database"></i>
                        Database
                    </a>
                </li>



            </ul>

        </div>

        <!-- MAIN AREA -->

        <div class="main-area">

            <header class="app-header">

                <h1 class="header-title">Admin Dashboard
                </h1>

            </header>

            <div class="content-container">

                <!-- WELCOME CARD -->

                <div class="welcome-card">

                    <h2>Welcome,
                <%= Session["FullName"] %>
                    </h2>

                    <br />

                    <p>
                        Manage examinations, students,
                question banks and monitor
                platform activities.
                    </p>

                </div>

                <!-- STATISTICS -->

                <div class="dashboard-grid">

                    <div class="stat-card">
                        <i class="fa-solid fa-users"></i>
                        <h2>
                            <asp:Label ID="lblStudents"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Total Students</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-file-signature"></i>
                        <h2>
                            <asp:Label ID="lblExams"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Total Exams</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-circle-question"></i>
                        <h2>
                            <asp:Label ID="lblQuestions"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Total Questions</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-clipboard-check"></i>
                        <h2>
                            <asp:Label ID="lblAttempts"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Total Attempts</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-triangle-exclamation"></i>
                        <h2>
                            <asp:Label ID="lblViolations"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Total Violations</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-bullhorn"></i>
                        <h2>
                            <asp:Label ID="lblPublishedExams"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Published Exams</p>
                    </div>


                    <div class="stat-card">
                        <i class="fa-solid fa-chart-pie"></i>
                        <h2>
                            <asp:Label ID="lblPassRate"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Pass Percentage</p>
                    </div>

                    <div class="stat-card">
                        <i class="fa-solid fa-star"></i>
                        <h2>
                            <asp:Label ID="lblAverageScore"
                                runat="server">
                            </asp:Label>
                        </h2>
                        <p>Average Score</p>
                    </div>

                </div>

                <!-- RECENT ACTIVITIES -->

                <div class="card">

                    <h3>Recent Activities
                    </h3>

                    <br />

                    <asp:GridView
                        ID="gvLogs"
                        runat="server"
                        CssClass="gridview">
                    </asp:GridView>

                </div>

                <br />

                <div class="card">

                    <h3>Top 5 Performers</h3>

                    <br />

                    <asp:GridView
                        ID="gvTopPerformers"
                        runat="server"
                        CssClass="gridview">
                    </asp:GridView>

                </div>

                <br />
                <!-- RECENT VIOLATIONS -->

                <div class="card">

                    <h3>Recent Violations
                    </h3>

                    <br />

                    <asp:GridView
                        ID="gvViolations"
                        runat="server"
                        CssClass="gridview">
                    </asp:GridView>

                </div>

            </div>

        </div>
        ```

    </form>

</body>
</html>
