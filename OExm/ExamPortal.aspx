<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamPortal.aspx.cs" Inherits="OExm.ExamPortal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Live Exam Portal | OExm</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

    <style>

        * { box-sizing: border-box; }

        body {
            margin: 0;
            font-family: 'Segoe UI', Arial, sans-serif;
            background: #eef2f9;
        }

        /* ---------- HEADER ---------- */

        .exam-header-bar {
            background: linear-gradient(135deg, #1e3a8a, #2563eb);
            color: white;
            padding: 14px 30px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 14px;
            position: sticky;
            top: 0;
            z-index: 50;
        }

        .brand-block {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .brand-icon {
            width: 42px;
            height: 42px;
            background: rgba(255,255,255,.15);
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
        }

        .brand-block h1 {
            font-size: 17px;
            margin: 0;
        }

        .brand-block small {
            display: block;
            font-size: 11px;
            opacity: .8;
            font-weight: 400;
        }

        .exam-meta {
            background: rgba(255,255,255,.12);
            border-radius: 10px;
            padding: 8px 18px;
            display: flex;
            gap: 18px;
            font-size: 14px;
            font-weight: 600;
        }

        .exam-meta span i { margin-right: 6px; opacity: .85; }

        .timer-box {
            background: rgba(220, 38, 38, .18);
            border: 1px solid rgba(255,255,255,.25);
            border-radius: 10px;
            padding: 6px 18px;
            text-align: center;
        }

        .timer-box .timer-label {
            font-size: 11px;
            opacity: .85;
        }

        .timer-box .timer-value {
            font-size: 20px;
            font-weight: 700;
        }

        .student-block {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 600;
        }

        .student-avatar {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background: rgba(255,255,255,.25);
        }

        /* ---------- LAYOUT ---------- */

        .exam-layout {
            max-width: 1300px;
            margin: 24px auto;
            padding: 0 20px 40px;
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 24px;
            align-items: start;
        }

        .panel {
            background: white;
            border-radius: 14px;
            padding: 24px;
            box-shadow: 0 4px 14px rgba(0,0,0,.05);
        }

        /* ---------- QUESTION CARD ---------- */

        .q-top-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 1px solid #e5e7eb;
            padding-bottom: 14px;
            margin-bottom: 20px;
        }

        .q-number {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 17px;
            font-weight: 700;
            color: #1e293b;
        }

        .q-number .icon-circle {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            background: #2563eb;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .mark-badges span {
            display: inline-block;
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 700;
            margin-left: 8px;
        }

        .badge-positive { background: #dcfce7; color: #16a34a; }
        .badge-negative { background: #fee2e2; color: #dc2626; }

        .q-text {
            font-size: 19px;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 22px;
        }

        .q-text .q-mark { color: #2563eb; margin-right: 6px; }

        /* Options rendered by the RadioButtonList */
        .option-list table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0 12px;
        }

        .option-list td {
            display: block;
            border: 2px solid #e5e7eb;
            background: #fff;
            border-radius: 10px;
            padding: 14px 18px;
            cursor: pointer;
            transition: .15s;
        }

        .option-list td:hover {
            border-color: #93c5fd;
            background: #f8fafc;
        }

        .option-list td:has(input:checked) {
            border-color: #2563eb;
            background: #eff6ff;
        }

        .option-list input[type="radio"] {
            width: 18px;
            height: 18px;
            margin-right: 12px;
            accent-color: #2563eb;
            vertical-align: middle;
        }

        .option-list label {
            font-size: 15px;
            font-weight: 600;
            color: #1e293b;
            vertical-align: middle;
            cursor: pointer;
        }

        .info-note {
            background: #eff6ff;
            border-radius: 10px;
            padding: 12px 16px;
            font-size: 14px;
            color: #334155;
            margin: 20px 0;
        }

        .info-note i { color: #2563eb; margin-right: 8px; }

        .action-row {
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 10px;
        }

        .action-row .btn {
            padding: 12px 22px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 14px;
            border: none;
            cursor: pointer;
        }

        .btn-outline {
            background: white;
            border: 1px solid #cbd5e1 !important;
            color: #334155;
        }

        .btn-blue {
            background: #2563eb;
            color: white;
        }

        .action-group { display: flex; gap: 10px; flex-wrap: wrap; }

        /* ---------- SIDEBAR ---------- */

        .side-title {
            display: flex;
            align-items: center;
            gap: 8px;
            font-weight: 700;
            font-size: 15px;
            margin-bottom: 16px;
            color: #1e293b;
        }

        .side-title i { color: #2563eb; }

        .nav-grid {
            display: grid;
            grid-template-columns: repeat(5, 1fr);
            gap: 8px;
            margin-bottom: 18px;
        }

        .nav-btn {
            width: 100%;
            aspect-ratio: 1;
            border-radius: 8px;
            border: 1px solid #e2e8f0;
            background: #f8fafc;
            color: #334155;
            font-weight: 700;
            font-size: 14px;
            cursor: pointer;
        }

        .nav-btn.current { background: #2563eb; color: white; border-color: #2563eb; }
        .nav-btn.answered { background: #dcfce7; color: #16a34a; border-color: #bbf7d0; }
        .nav-btn.review { background: #fef9c3; color: #a16207; border-color: #fde68a; }
        .nav-btn.not-answered { background: #fee2e2; color: #dc2626; border-color: #fecaca; }
        .nav-btn.not-visited { background: #f1f5f9; color: #64748b; border-color: #e2e8f0; }

        .legend {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 8px;
            font-size: 13px;
            color: #475569;
            margin-bottom: 22px;
        }

        .legend-item { display: flex; align-items: center; gap: 8px; }

        .legend-dot {
            width: 12px;
            height: 12px;
            border-radius: 3px;
        }

        .overview-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px solid #f1f5f9;
            font-size: 14px;
        }

        .overview-row:last-child { border-bottom: none; }

        .overview-row .label { color: #64748b; display: flex; align-items: center; gap: 8px; }
        .overview-row .value { font-weight: 700; color: #1e293b; }

        .important-box {
            border: 1px solid #fecaca;
            background: #fef2f2;
            border-radius: 12px;
            padding: 16px;
            margin-top: 20px;
        }

        .important-box h4 {
            color: #dc2626;
            margin: 0 0 6px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .important-box p {
            font-size: 13px;
            color: #7f1d1d;
            margin: 0 0 14px;
        }

        .btn-submit {
            width: 100%;
            background: #dc2626;
            color: white;
            border: none;
            padding: 14px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 15px;
            cursor: pointer;
        }

        .secure-footer {
            max-width: 1300px;
            margin: 0 auto 30px;
            padding: 0 20px;
            text-align: center;
            color: #64748b;
            font-size: 13px;
        }

        .secure-footer i { color: #16a34a; margin-right: 6px; }

        @media (max-width: 950px) {
            .exam-layout { grid-template-columns: 1fr; }
        }

    </style>

    <script type="text/javascript">

        // Traps the browser Back/Forward buttons for the whole time the
        // student is inside the exam -- pushes a dummy history entry, and
        // whenever "back" is pressed (fires popstate), immediately pushes
        // forward again so they just stay on this page instead of landing
        // back on Instructions.aspx.
        history.pushState(null, null, location.href);
        window.addEventListener("popstate", function () {
            history.pushState(null, null, location.href);
        });

        // Server calculates the real time left (see GetSecondsLeft in code-behind);
        // this just counts it down on screen and auto-submits at zero.
        var remainingSeconds = 0;
        var timerIntervalId = null;
        var examAlreadySubmitted = false;

        // How many tab switches are allowed before the exam is force-submitted.
        var maxTabSwitches = 3;
        var tabSwitchCount = 0;

        function startTimer() {
            var hiddenField = document.getElementById('<%= hfRemainingSeconds.ClientID %>');
            remainingSeconds = parseInt(hiddenField.value) || 0;
            updateTimerDisplay();

            timerIntervalId = setInterval(function () {
                remainingSeconds--;
                updateTimerDisplay();

                if (remainingSeconds <= 0) {
                    // Stop ticking immediately so this never fires again,
                    // even if the submit postback takes a moment to leave the page.
                    clearInterval(timerIntervalId);
                    forceSubmitExam("Time is up! Submitting your exam now...");
                }
            }, 1000);
        }

        function updateTimerDisplay() {
            var m = Math.max(0, Math.floor(remainingSeconds / 60));
            var s = Math.max(0, remainingSeconds % 60);
            var display = (m < 10 ? '0' + m : m) + ':' + (s < 10 ? '0' + s : s);
            document.getElementById('<%= lblTimer.ClientID %>').innerHTML = display;
        }

        // Single place that actually ends the exam -- used by both the
        // timer running out and by exceeding the tab-switch limit.
        function forceSubmitExam(message) {
            // Belt-and-braces: even if something calls this twice, only submit once.
            if (examAlreadySubmitted) return;
            examAlreadySubmitted = true;

            allowLeavingPage();
            showBanner(message, "#dc2626");

            // Trigger the postback directly instead of .click()-ing the button.
            // .click() would also run btnSubmit's own OnClientClick handler,
            // which pops up a "Are you sure?" confirm() dialog -- fine for a
            // real manual click, but wrong here: there's nothing left to
            // confirm, and it just gets in the way of an automatic submit.
            __doPostBack('<%= btnSubmit.UniqueID %>', '');
        }

        // Non-blocking banner -- a blocking alert()/confirm() here is exactly
        // what let earlier bugs fire repeatedly or ask pointless questions
        // during an automatic action. color is any valid CSS color.
        function showBanner(text, color) {
            var notice = document.createElement('div');
            notice.textContent = text;
            notice.style.cssText =
                "position:fixed; top:20px; left:50%; transform:translateX(-50%); " +
                "background:" + color + "; color:#fff; padding:12px 24px; border-radius:8px; " +
                "font-family:Segoe UI,Arial,sans-serif; font-weight:600; z-index:9999; " +
                "box-shadow:0 4px 12px rgba(0,0,0,.3);";
            document.body.appendChild(notice);
            setTimeout(function () {
                if (notice.parentNode) notice.parentNode.removeChild(notice);
            }, 4000);
        }

        window.onbeforeunload = function () {
            return "Your progress is saved, but the timer keeps running. Are you sure you want to leave?";
        };

        function allowLeavingPage() {
            window.onbeforeunload = null;
        }

        // Tell the server whenever the student switches away from this tab,
        // so every switch gets logged -- and once they come back, either
        // warn them (still under the limit) or force-submit (limit hit).
        document.addEventListener("visibilitychange", function () {
            if (examAlreadySubmitted) return;

            if (document.hidden) {
                tabSwitchCount++;
                __doPostBack('TabSwitch', tabSwitchCount.toString());
            } else if (tabSwitchCount >= maxTabSwitches) {
                forceSubmitExam("Too many tab switches detected. Submitting your exam now...");
            } else if (tabSwitchCount > 0) {
                showBanner(
                    "Tab switch detected (" + tabSwitchCount + "/" + maxTabSwitches +
                    "). Switching away again may auto-submit your exam.",
                    "#a16207"
                );
            }
        });

        window.addEventListener('load', startTimer);

    </script>
</head>
<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- HEADER -->
        <div class="exam-header-bar">

            <div class="brand-block">
                <div class="brand-icon"><i class="fa-solid fa-graduation-cap"></i></div>
                <div>
                    <h1>OExm Exam Portal</h1>
                    <small>Online Examination System</small>
                </div>
            </div>

            <div class="exam-meta">
                <span><i class="fa-solid fa-file-lines"></i>Exam: <asp:Label ID="lblExamNameHeader" runat="server"></asp:Label></span>
                <span><i class="fa-solid fa-layer-group"></i>Subject: <asp:Label ID="lblSubjectHeader" runat="server"></asp:Label></span>
            </div>

            <div class="timer-box">
                <div class="timer-label"><i class="fa-solid fa-stopwatch"></i> Time Left</div>
                <div class="timer-value"><asp:Label ID="lblTimer" runat="server" Text="--:--"></asp:Label></div>
            </div>

            <div class="student-block">
                <div class="student-avatar"></div>
                <span>Hi, <%= Session["FullName"] %></span>
            </div>

        </div>

        <!-- MAIN LAYOUT -->
        <!-- btnSubmit is set as a PostBackTrigger below (not an async
             trigger) because it navigates away to Result.aspx and should
             behave as a normal full postback. -->
        <asp:UpdatePanel ID="upExam" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div class="exam-layout">

            <!-- LEFT: QUESTION PANEL -->
            <div class="panel">

                <div class="q-top-row">
                    <div class="q-number">
                        <span class="icon-circle"><i class="fa-solid fa-file-lines"></i></span>
                        Question <%= Convert.ToInt32(hfQuestionIndex.Value) + 1 %> of <%= (Session["Questions"] != null) ? ((System.Data.DataTable)Session["Questions"]).Rows.Count : 0 %>
                    </div>
                    <div class="mark-badges">
                        <span class="badge-positive">+<asp:Label ID="lblPositiveMark" runat="server"></asp:Label> Mark</span>
                        <span class="badge-negative">-<asp:Label ID="lblNegativeMark" runat="server"></asp:Label> Negative</span>
                    </div>
                </div>

                <div class="q-text"><span class="q-mark">Q.</span><asp:Label ID="lblQuestion" runat="server"></asp:Label></div>

                <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="option-list">
                    <asp:ListItem Value="A"></asp:ListItem>
                    <asp:ListItem Value="B"></asp:ListItem>
                    <asp:ListItem Value="C"></asp:ListItem>
                    <asp:ListItem Value="D"></asp:ListItem>
                </asp:RadioButtonList>

                <div class="info-note">
                    <i class="fa-solid fa-circle-info"></i>
                    Select an answer and click <b>"Save &amp; Next"</b> to continue.
                </div>

                <div class="action-row">

                    <asp:Button ID="btnPrevious" runat="server" Text="&#8249; Previous" CssClass="btn btn-outline"
                        OnClientClick="allowLeavingPage();" OnClick="btnPrevious_Click" />

                    <div class="action-group">
                        <asp:Button ID="btnMarkForReview" runat="server" Text="Mark for Review" CssClass="btn btn-outline"
                            OnClientClick="allowLeavingPage();" OnClick="btnMarkForReview_Click" />

                        <asp:Button ID="btnClearResponse" runat="server" Text="Clear Response" CssClass="btn btn-outline"
                            OnClientClick="allowLeavingPage();" OnClick="btnClearResponse_Click" />

                        <asp:Button ID="btnNext" runat="server" Text="Save &amp; Next &#8250;" CssClass="btn btn-blue"
                            OnClientClick="allowLeavingPage();" OnClick="btnNext_Click" />
                    </div>

                </div>

            </div>

            <!-- RIGHT: SIDEBAR -->
            <div>

                <div class="panel" style="margin-bottom: 20px;">

                    <div class="side-title"><i class="fa-solid fa-table-cells"></i> Question Navigator</div>

                    <div class="nav-grid">
                        <asp:Repeater ID="rpPalette" runat="server" OnItemCommand="rpPalette_ItemCommand">
                            <ItemTemplate>
                                <asp:Button runat="server"
                                    OnClientClick="allowLeavingPage();"
                                    Text='<%# Container.ItemIndex + 1 %>'
                                    CommandName="JumpTo"
                                    CommandArgument='<%# Container.ItemIndex %>'
                                    CssClass='<%# GetPaletteClass(Eval("QuestionId")) %>' />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="legend">
                        <div class="legend-item"><span class="legend-dot" style="background:#16a34a;"></span>Answered</div>
                        <div class="legend-item"><span class="legend-dot" style="background:#a16207;"></span>Review</div>
                        <div class="legend-item"><span class="legend-dot" style="background:#dc2626;"></span>Not Answered</div>
                        <div class="legend-item"><span class="legend-dot" style="background:#94a3b8;"></span>Not Visited</div>
                    </div>

                </div>

                <div class="panel">

                    <div class="side-title"><i class="fa-solid fa-square-poll-vertical"></i> Exam Overview</div>

                    <div class="overview-row">
                        <span class="label"><i class="fa-regular fa-square"></i> Total Questions</span>
                        <span class="value"><%= (Session["Questions"] != null) ? ((System.Data.DataTable)Session["Questions"]).Rows.Count : 0 %></span>
                    </div>
                    <div class="overview-row">
                        <span class="label"><i class="fa-solid fa-check" style="color:#16a34a;"></i> Attempted</span>
                        <span class="value"><asp:Label ID="lblAttemptedCount" runat="server">0</asp:Label></span>
                    </div>
                    <div class="overview-row">
                        <span class="label"><i class="fa-solid fa-bookmark" style="color:#a16207;"></i> To Review</span>
                        <span class="value"><asp:Label ID="lblReviewCount" runat="server">0</asp:Label></span>
                    </div>
                    <div class="overview-row">
                        <span class="label"><i class="fa-solid fa-xmark" style="color:#dc2626;"></i> Not Attempted</span>
                        <span class="value"><asp:Label ID="lblNotAttemptedCount" runat="server">0</asp:Label></span>
                    </div>

                    <div class="important-box">
                        <h4><i class="fa-solid fa-shield-halved"></i> Important</h4>
                        <p>You can submit your exam at any time. Once submitted, answers cannot be modified.</p>

                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Examination" CssClass="btn-submit"
                            OnClientClick="allowLeavingPage(); return confirm('Are you sure you want to end and submit this examination?');"
                            OnClick="btnSubmit_Click" />
                    </div>

                </div>

            </div>

        </div>

        <!-- Hidden fields live inside the UpdatePanel so their server-updated
             values (new question index, recalculated seconds left) actually
             reach the browser after a partial postback -- outside the panel,
             the DOM copy would go stale after the very first click. -->
        <asp:HiddenField ID="hfQuestionIndex" runat="server" Value="0" />
        <asp:HiddenField ID="hfRemainingSeconds" runat="server" Value="0" />

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubmit" />
        </Triggers>
        </asp:UpdatePanel>

        <div class="secure-footer">
            <i class="fa-solid fa-shield-halved"></i>Secure Exam Environment: your session is monitored for fairness and integrity.
        </div>

    </form>
</body>
</html>
