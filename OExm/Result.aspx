<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Result.aspx.cs" Inherits="OExm.Result" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Exam Result Analysis | Online Examination System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Content/style.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <script src="Scripts/app.js"></script>
    
    <style>
        @media print {
            .sidebar, .app-header, .action-buttons-card {
                display: none !important;
            }
            .main-area {
                margin-left: 0 !important;
            }
            .content-container {
                padding: 0 !important;
            }
            .card {
                box-shadow: none !important;
                border: 1px solid #ccc !important;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="app-container">

        <!-- SIDEBAR -->
        <div class="sidebar">
            <div class="brand-section">
                <div class="brand-icon">
                    <i class="fa-solid fa-graduation-cap"></i>
                </div>
                <span class="brand-name">OExm Portal</span>
            </div>

            <div class="user-profile">
                <span class="user-name"><%= Session["FullName"] %></span>
                <span class="user-role"><%= Session["Role"] %></span>
            </div>

            <ul class="nav-menu">
                <li>
                    <a href="AdminDashboard.aspx" class="nav-item">
                        <i class="fa-solid fa-chart-line"></i> Dashboard
                    </a>
                </li>
                <% if (Session["Role"] != null && Session["Role"].ToString() == "Admin") { %>
                    <li>
                        <a href="ManageExams.aspx" class="nav-item">
                            <i class="fa-solid fa-file-signature"></i> Exams
                        </a>
                    </li>
                    <li>
                        <a href="ManageSections.aspx" class="nav-item">
                            <i class="fa-solid fa-layer-group"></i> Sections
                        </a>
                    </li>
                    <li>
                        <a href="ManageQuestions.aspx" class="nav-item">
                            <i class="fa-solid fa-circle-question"></i> Questions
                        </a>
                    </li>
                <% } else { %>
                    <li>
                        <a href="Instructions.aspx" class="nav-item">
                            <i class="fa-solid fa-circle-play"></i> Start Exam
                        </a>
                    </li>
                <% } %>
                <li>
                    <a href="Result.aspx" class="nav-item">
                        <i class="fa-solid fa-square-poll-vertical"></i> Results
                    </a>
                </li>
            </ul>

            <div class="sidebar-footer">
                <button id="themeToggleBtn" type="button" class="theme-toggle-btn">🌙 Dark Mode</button>
                <a href="Login.aspx?logout=true" class="nav-item logout-btn">
                    <i class="fa-solid fa-right-from-bracket"></i> Logout
                </a>
            </div>
        </div>

        <!-- MAIN AREA -->
        <div class="main-area">

            <!-- HEADER -->
            <header class="app-header">
                <h1 class="header-title">Exam Result Analytics</h1>
                <div class="header-right">
                    <span style="font-weight: 500; font-size: 14px; opacity: 0.8;">
                        <i class="fa-solid fa-square-poll-vertical"></i> Performance Review
                    </span>
                </div>
            </header>

            <!-- CONTENT -->
            <div class="content-container">

                <!-- TOP SUMMARY METRICS GRID — two equal columns -->
                <div style="
                    display: grid;
                    grid-template-columns: 1fr 1fr;
                    gap: 25px;
                    margin-bottom: 30px;">

                    <!-- LEFT: SCORE CARD -->
                    <div class="score-display">

                        <span class="score-lbl">
                            Cumulative Score
                        </span>

                        <div class="score-value">
                            <asp:Label ID="lblScore" runat="server"></asp:Label>
                        </div>

                        <div style="margin-top:12px; font-size:15px; font-weight:600;">
                            Percentage :
                            <asp:Label ID="lblPercentage" runat="server" style="color:#2563eb;"></asp:Label>
                        </div>

                        <div style="
                            margin-top:20px;
                            display:flex;
                            justify-content:center;
                            gap:10px;
                            flex-wrap:wrap;">

                            <asp:Label ID="lblResultStatus" runat="server"
                                style="
                                padding:10px 20px;
                                border-radius:30px;
                                font-weight:700;
                                font-size:14px;">
                            </asp:Label>

                            <asp:Label ID="lblGrade" runat="server"
                                style="
                                background:#dbeafe;
                                color:#1d4ed8;
                                padding:10px 20px;
                                border-radius:30px;
                                font-weight:700;
                                font-size:14px;">
                            </asp:Label>

                        </div>

                    </div>

                    <!-- RIGHT: DETAILED STATS LIST -->
                    <div class="card" style="display: flex; flex-direction: column; justify-content: center;">
                        <h3 class="card-title" style="font-size: 16px; margin-bottom: 16px;">
                            <i class="fa-solid fa-chart-simple"></i> Attempt Metrics
                        </h3>

                        <ul class="list-group">
                            <li class="list-group-item">
                                <span>Correct Responses</span>
                                <span style="font-weight: 700; color: var(--success);">
                                    <asp:Label ID="lblCorrect" runat="server"></asp:Label>
                                </span>
                            </li>
                            <li class="list-group-item">
                                <span>Wrong Responses</span>
                                <span style="font-weight: 700; color: var(--danger);">
                                    <asp:Label ID="lblWrong" runat="server"></asp:Label>
                                </span>
                            </li>
                            <li class="list-group-item">
                                <span>Unanswered Questions</span>
                                <span style="font-weight:700; color:#f59e0b;">
                                    <asp:Label ID="lblUnattempted" runat="server"></asp:Label>
                                </span>
                            </li>
                            
                        </ul>
                    </div>

                </div>
                <!-- END TWO-COLUMN GRID -->

                <!-- CONGRATULATIONS BANNER -->
                <div class="card"
                    style="background:linear-gradient(135deg,#2563eb,#4f46e5);
                           color:white;
                           margin-bottom:25px;">
                    <h2>Congratulations!</h2>
                    <p>Your performance has been evaluated successfully.</p>
                </div>

               

                <!-- ACTION BUTTONS CARD -->
                <div class="card action-buttons-card" style="display: flex; gap: 16px; align-items: center; justify-content: space-between;">
                    <a href="AdminDashboard.aspx" class="btn btn-secondary" style="font-size: 15px; padding: 12px 24px;">
                        <i class="fa-solid fa-circle-chevron-left"></i> Go To Dashboard
                    </a>

                    <div style="display: flex; gap: 12px;">
                        <button type="button" onclick="window.print();" class="btn btn-secondary" style="font-size: 15px; padding: 12px 20px;">
                            <i class="fa-solid fa-print"></i> Print Page
                        </button>

                        <asp:Button ID="btnDownloadPDF"
                            runat="server"
                            Text="Download PDF Certificate"
                            CssClass="btn btn-primary"
                            style="font-size: 15px; padding: 12px 24px;"
                            OnClick="btnDownloadPDF_Click" />
                    </div>
                </div>

            </div>
            <!-- END content-container -->

        </div>
        <!-- END main-area -->

    </form>
</body>
</html>