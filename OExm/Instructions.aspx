<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Instructions.aspx.cs" Inherits="OExm.Instructions" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Exam Instructions | Online Examination System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Content/style.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <script src="Scripts/app.js"></script>
    <script type="text/javascript">
        function checkAgreedAndStart(e) {
            const chk = document.getElementById('chkAgree');
            if (chk && !chk.checked) {
                alert('Please accept the declaration checkbox before starting the exam.');
                e.preventDefault();
                return false;
            }
            // Trigger fullscreen mode before entering the exam
            enterExamFullscreen();
            return true;
        }
    </script>
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
                    <%-- Fixed typo: AdminDasgboard → AdminDashboard --%>
                    <a href="AdminDashboard.aspx" class="nav-item">
                        <i class="fa-solid fa-chart-line"></i>Dashboard
                    </a>
                </li>
                <% if (Session["Role"] != null && Session["Role"].ToString() == "Admin") { %>
                <li>
                    <a href="ManageExams.aspx" class="nav-item">
                        <i class="fa-solid fa-file-signature"></i>Exams
                    </a>
                </li>
                <li>
                    <a href="ManageSections.aspx" class="nav-item">
                        <i class="fa-solid fa-layer-group"></i>Sections
                    </a>
                </li>
                <li>
                    <a href="ManageQuestions.aspx" class="nav-item">
                        <i class="fa-solid fa-circle-question"></i>Questions
                    </a>
                </li>
                <% } else { %>
                <li>
                    <a href="Instructions.aspx" class="nav-item">
                        <i class="fa-solid fa-circle-play"></i>Start Exam
                    </a>
                </li>
                <% } %>
                <li>
                    <a href="Result.aspx" class="nav-item">
                        <i class="fa-solid fa-square-poll-vertical"></i>Results
                    </a>
                </li>
            </ul>

            <div class="sidebar-footer">
                <button id="themeToggleBtn" type="button" class="theme-toggle-btn">🌙 Dark Mode</button>
                <a href="Login.aspx?logout=true" class="nav-item logout-btn">
                    <i class="fa-solid fa-right-from-bracket"></i>Logout
                </a>
            </div>
        </div>

        <!-- MAIN AREA -->
        <div class="main-area">

            <!-- HEADER -->
            <header class="app-header">
                <h1 class="header-title">Exam Setup &amp; Instructions</h1>
                <div class="header-right">
                    <span style="font-weight: 500; font-size: 14px; opacity: 0.8;">
                        <i class="fa-solid fa-clock"></i> Scheduled Exam
                    </span>
                </div>
            </header>

            <!-- CONTENT -->
            <div class="content-container" style="max-width: 900px; margin: 0 auto;">

                <!-- EXAM SELECTOR CARD -->
                <div class="card" style="margin-bottom: 24px;">
                    <h3 class="card-title"><i class="fa-solid fa-file-signature"></i> Select Your Examination</h3>

                    <div class="form-group">
                        <label class="form-label">Active Examinations Available</label>
                        <asp:DropDownList ID="ddlExams"
                            runat="server"
                            CssClass="dropdown"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlExams_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <!-- Inline error/info message label -->
                    <asp:Label ID="lblMessage" runat="server" Visible="false"
                        style="display:block; margin-top:12px; padding:10px 16px; border-radius:8px; font-size:14px; font-weight:600;">
                    </asp:Label>

                    <div style="background-color: var(--bg-primary); border: 1px solid var(--border-color); border-radius: 8px; padding: 20px; display: flex; flex-direction: column; gap: 12px; margin-top: 16px;">
                        <div style="display: flex; justify-content: space-between; font-weight: 600; font-size: 16px;">
                            <span>Duration Allocated:</span>
                            <span style="color: var(--primary);">
                                <asp:Label ID="lblDuration" runat="server"></asp:Label> Minutes
                            </span>
                        </div>
                        <hr style="border: none; border-top: 1px solid var(--border-color);" />
                        <div style="font-size: 14px; line-height: 1.6; color: var(--text-secondary);">
                            <strong>Exam Details:</strong><br />
                            <asp:Label ID="lblDescription" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <!-- INSTRUCTIONS CARD -->
                <div class="card" style="margin-bottom: 24px;">
                    <h3 class="card-title" style="color: var(--danger);">
                        <i class="fa-solid fa-triangle-exclamation"></i> Important Candidate Instructions
                    </h3>

                    <%-- Fixed: removed raw markdown syntax (**bold**, `code`) — use proper HTML tags instead --%>
                    <ul style="padding-left: 20px; font-size: 14px; line-height: 2; color: var(--text-primary); display: flex; flex-direction: column; gap: 8px;">
                        <li>
                            <i class="fa-solid fa-ban" style="color: var(--danger); margin-right: 8px;"></i>
                            <strong>Do not refresh, reload, or navigate away</strong> from the browser. Your exam progress will be locked or flagged.
                        </li>
                        <li>
                            <i class="fa-solid fa-percent" style="color: var(--warning); margin-right: 8px;"></i>
                            Questions have individual marking schemes. Check positive and negative marks carefully.
                        </li>
                        <li>
                            <i class="fa-solid fa-hourglass-half" style="color: var(--primary); margin-right: 8px;"></i>
                            The <strong>Countdown Timer</strong> is cheat-proof and runs on the server. If the timer hits <strong>00:00</strong>, your exam will <strong>automatically submit</strong>.
                        </li>
                        <li>
                            <i class="fa-solid fa-expand" style="color: var(--success); margin-right: 8px;"></i>
                            The exam must be taken in <strong>Fullscreen Mode</strong>. Exiting fullscreen is flagged as a potential cheating attempt.
                        </li>
                        <li>
                            <i class="fa-solid fa-floppy-disk" style="color: var(--primary); margin-right: 8px;"></i>
                            Make sure to select options and click <strong>Save &amp; Next</strong> or <strong>Mark for Review</strong> to record your answers.
                        </li>
                    </ul>
                </div>

                <!-- DECLARATION CARD -->
                <div class="card">
                    <h3 class="card-title"><i class="fa-solid fa-signature"></i> Candidate Declaration</h3>

                    <div class="form-group">
                        <label class="chk-label" style="align-items: flex-start;">
                            <input type="checkbox" id="chkAgree" style="margin-top: 4px; width: 18px; height: 18px; cursor: pointer;" />
                            <span style="font-size: 14px; line-height: 1.4; opacity: 0.9; cursor: pointer;">
                                I have read, understood, and agree to adhere to all the instructions and rules mentioned above. I am ready to begin the examination.
                            </span>
                        </label>
                    </div>

                    <asp:Button ID="btnStart"
                        runat="server"
                        Text="Start Examination"
                        CssClass="btn btn-primary"
                        Style="width: 100%; font-size: 16px; padding: 14px 24px;"
                        OnClientClick="return checkAgreedAndStart(event);"
                        OnClick="btnStart_Click" />
                </div>

            </div>
        </div>

    </form>
</body>
</html>
