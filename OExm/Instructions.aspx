<%@ Page Title="Exam Instructions" Language="C#" AutoEventWireup="true"
    CodeBehind="Instructions.aspx.cs"
    Inherits="OExm.Instructions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Exam Instructions | OExm</title>

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link href="Content/style.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

    <style>

        body {
            background: #f1f5f9;
            margin: 0;
        }

        /* ---------- TOP HEADER BAR ---------- */

        .top-header {
            background: linear-gradient(135deg, #1e3a8a, #2563eb);
            color: white;
            padding: 18px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 20px;
            font-weight: 700;
        }

        .brand small {
            display: block;
            font-weight: 400;
            font-size: 12px;
            opacity: .85;
        }

        .logout-link {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 8px 16px;
            border-radius: 8px;
            background: rgba(255,255,255,.15);
            color: white;
            text-decoration: none;
            font-weight: 600;
            font-size: 14px;
        }

        .logout-link:hover {
            background: rgba(255,255,255,.28);
        }

        /* ---------- PAGE LAYOUT ---------- */

        .page-container {
            max-width: 950px;
            margin: 0 auto;
            padding: 30px 20px 60px;
        }

        .page-title h1 {
            margin: 0 0 4px;
            font-size: 26px;
        }

        .page-title p {
            color: #64748b;
            margin: 0 0 25px;
        }

        .step-heading {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 18px;
            font-size: 17px;
            font-weight: 700;
        }

        .step-number {
            width: 26px;
            height: 26px;
            border-radius: 50%;
            background: #2563eb;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
        }

        /* ---------- STAT MINI-CARDS (row of quick facts) ---------- */

        .stat-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
            gap: 14px;
            margin-top: 18px;
        }

        .stat-box {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 14px;
        }

        .stat-box .stat-label {
            color: #64748b;
            font-size: 13px;
            margin-bottom: 4px;
        }

        .stat-box .stat-value {
            font-size: 18px;
            font-weight: 700;
            color: #1e293b;
        }

        /* ---------- EXAM DETAILS SECTION ---------- */

        .details-grid {
            display: grid;
            grid-template-columns: 1fr 1.3fr;
            gap: 20px;
            margin-top: 18px;
        }

        .exam-name-box {
            background: #eff6ff;
            border-radius: 12px;
            padding: 18px;
        }

        .exam-name-box h3 {
            margin: 0 0 8px;
        }

        .exam-name-box p {
            color: #475569;
            margin: 0;
        }

        .detail-list .detail-row {
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #e2e8f0;
        }

        .detail-list .detail-row:last-child {
            border-bottom: none;
        }

        .detail-row .detail-label {
            color: #64748b;
            font-size: 13px;
        }

        .detail-row .detail-value {
            font-weight: 700;
        }

        @media (max-width: 700px) {
            .details-grid {
                grid-template-columns: 1fr;
            }
        }

        /* ---------- INSTRUCTION CARDS ---------- */

        .instruction-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
            gap: 14px;
            margin-top: 18px;
        }

        .instruction-card {
            border-radius: 12px;
            padding: 16px;
            text-align: center;
        }

        .instruction-card i {
            font-size: 22px;
            margin-bottom: 10px;
        }

        .instruction-card h4 {
            margin: 0 0 6px;
            font-size: 14px;
        }

        .instruction-card p {
            margin: 0;
            font-size: 12px;
            color: #475569;
        }

        .instruction-card.blue { background: #eff6ff; }
        .instruction-card.orange { background: #fff7ed; }
        .instruction-card.purple { background: #f5f3ff; }
        .instruction-card.green { background: #ecfdf5; }
        .instruction-card.red { background: #fef2f2; }

        /* ---------- DECLARATION ---------- */

        .declaration-list {
            margin: 15px 0;
            padding-left: 5px;
            list-style: none;
        }

        .declaration-list li {
            margin-bottom: 8px;
            color: #334155;
        }

        .declaration-list i {
            color: #16a34a;
            margin-right: 8px;
        }

        .agree-row {
            display: flex;
            gap: 10px;
            align-items: flex-start;
            margin-top: 10px;
        }

        .start-row {
            display: flex;
            gap: 16px;
            margin-top: 20px;
            align-items: stretch;
        }

        .start-row .btn {
            flex: 2;
        }

        .secure-note {
            flex: 1;
            background: #eff6ff;
            border-radius: 12px;
            padding: 12px 16px;
            display: flex;
            gap: 10px;
            align-items: center;
            font-size: 12px;
            color: #475569;
        }

        .secure-note i {
            color: #2563eb;
            font-size: 20px;
        }

        .message {
            display: block;
            margin-top: 15px;
            padding: 12px;
            border-radius: 10px;
            font-weight: 600;
        }

        .page-footer {
            text-align: center;
            color: #94a3b8;
            font-size: 13px;
            padding: 20px;
        }

    </style>

    <script>

        function checkAndStart(e) {
            var chk = document.getElementById("chkAgree");

            if (!chk.checked) {
                alert("Please agree to the declaration before starting.");
                e.preventDefault();
                return false;
            }

            return true;
        }

    </script>

</head>

<body>

<form id="form1" runat="server">

    <!-- TOP HEADER -->
    <div class="top-header">
        <div class="brand">
            <i class="fa-solid fa-graduation-cap fa-lg"></i>
            <div>
                OExm
                <small>Online Examination System</small>
            </div>
        </div>

        <a href="Login.aspx?logout=true" class="logout-link">
            <i class="fa-solid fa-right-from-bracket"></i> Logout
        </a>
    </div>

    <div class="page-container">

        <div class="page-title">
            <h1><i class="fa-solid fa-clipboard-list"></i> Exam Instructions</h1>
            <p>Please read all instructions carefully before starting the examination.</p>
        </div>

        <!-- STEP 1: SELECT EXAM -->
        <div class="card">

            <div class="step-heading">
                <span class="step-number">1</span> Select Examination
            </div>

            <asp:DropDownList
                ID="ddlExams"
                runat="server"
                CssClass="dropdown"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlExams_SelectedIndexChanged">
            </asp:DropDownList>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>

            <div class="stat-row">
                <div class="stat-box">
                    <div class="stat-label">Duration</div>
                    <div class="stat-value"><asp:Label ID="lblDuration" runat="server"></asp:Label> min</div>
                </div>
                <div class="stat-box">
                    <div class="stat-label">Questions</div>
                    <div class="stat-value"><asp:Label ID="lblQuestionCount" runat="server"></asp:Label></div>
                </div>
                <div class="stat-box">
                    <div class="stat-label">Passing Marks</div>
                    <div class="stat-value"><asp:Label ID="lblPassingMarks" runat="server"></asp:Label></div>
                </div>
                <div class="stat-box">
                    <div class="stat-label">Negative Marking</div>
                    <div class="stat-value"><asp:Label ID="lblNegativeMarks" runat="server"></asp:Label></div>
                </div>
                <div class="stat-box">
                    <div class="stat-label">Mode</div>
                    <div class="stat-value"><asp:Label ID="lblMode" runat="server"></asp:Label></div>
                </div>
            </div>

        </div>

        <br />

        <!-- STEP 2: EXAM DETAILS -->
        <div class="card">

            <div class="step-heading">
                <span class="step-number">2</span> Examination Details
            </div>

            <div class="details-grid">

                <div class="exam-name-box">
                    <h3><asp:Label ID="lblExamName" runat="server"></asp:Label></h3>
                    <p><asp:Label ID="lblDescription" runat="server"></asp:Label></p>
                </div>

                <div class="detail-list">
                    <div class="detail-row">
                        <span class="detail-label">Available From</span>
                        <span class="detail-value"><asp:Label ID="lblAvailableFrom" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Available Until</span>
                        <span class="detail-value"><asp:Label ID="lblAvailableUntil" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Total Marks</span>
                        <span class="detail-value"><asp:Label ID="lblTotalMarks" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Question Bank</span>
                        <span class="detail-value"><asp:Label ID="lblBankName" runat="server"></asp:Label></span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Attempts Allowed</span>
                        <span class="detail-value">1</span>
                    </div>
                </div>

            </div>

        </div>

        <br />

        <!-- STEP 3: INSTRUCTIONS -->
        <div class="card">

            <div class="step-heading">
                <span class="step-number">3</span> Important Instructions
            </div>

            <div class="instruction-cards">

                <div class="instruction-card blue">
                    <i class="fa-solid fa-laptop"></i>
                    <h4>No Refresh / Close</h4>
                    <p>Do not refresh or close the browser during the examination.</p>
                </div>

                <div class="instruction-card orange">
                    <i class="fa-solid fa-expand"></i>
                    <h4>Stay in Fullscreen</h4>
                    <p>Stay in fullscreen mode throughout the exam.</p>
                </div>

                <div class="instruction-card purple">
                    <i class="fa-solid fa-stopwatch"></i>
                    <h4>Timer &amp; Auto Submit</h4>
                    <p>The timer will automatically submit your exam when time is over.</p>
                </div>

                <div class="instruction-card green">
                    <i class="fa-solid fa-floppy-disk"></i>
                    <h4>Save Your Answers</h4>
                    <p>Save answers before moving to another question.</p>
                </div>

                <div class="instruction-card red">
                    <i class="fa-solid fa-shield-halved"></i>
                    <h4>Violations</h4>
                    <p>Any suspicious activity may be recorded as a violation.</p>
                </div>

            </div>

        </div>

        <br />

        <!-- STEP 4: DECLARATION -->
        <div class="card">

            <div class="step-heading">
                <span class="step-number">4</span> Declaration
            </div>

            <ul class="declaration-list">
                <li><i class="fa-solid fa-circle-check"></i> I will not copy answers or attempt to cheat.</li>
                <li><i class="fa-solid fa-circle-check"></i> I will not switch tabs or open any other windows.</li>
                <li><i class="fa-solid fa-circle-check"></i> I understand my exam will be auto-submitted if I violate any rules.</li>
            </ul>

            <div class="agree-row">
                <input type="checkbox" id="chkAgree" />
                <label for="chkAgree">I agree to follow all the instructions and rules stated above.</label>
            </div>

            <div class="start-row">

                <asp:Button
                    ID="btnStart"
                    runat="server"
                    Text="Start Examination"
                    CssClass="btn btn-primary"
                    OnClientClick="return checkAndStart(event);"
                    OnClick="btnStart_Click" />

                <div class="secure-note">
                    <i class="fa-solid fa-shield-halved"></i>
                    <span><b>Secure Exam Environment</b><br />Your exam session is monitored and protected for your integrity.</span>
                </div>

            </div>

        </div>

    </div>

    <div class="page-footer">&copy; 2026 OExm. All rights reserved.</div>

</form>

</body>
</html>
