<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamPortal.aspx.cs" Inherits="OExm.ExamPortal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Live Exam Portal | Online Examination System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Content/style.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <script src="Scripts/app.js"></script>

    <style>
        /* Custom overrides for ASP.NET RadioButtonList layout inside cards */
        .option-list table {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0 12px;
        }

        .option-list td {
            display: block;
            border: 1px solid var(--border-color);
            background-color: var(--bg-primary);
            border-radius: 8px;
            padding: 14px 18px;
            transition: all 0.2s ease;
            cursor: pointer;
        }

            .option-list td:hover {
                border-color: var(--primary);
                background-color: var(--primary-light);
            }

            .option-list td input[type="radio"] {
                margin-right: 12px;
                width: 18px;
                height: 18px;
                accent-color: var(--primary);
                cursor: pointer;
                vertical-align: middle;
            }

            .option-list td label {
                font-size: 15px;
                font-weight: 500;
                color: var(--text-primary);
                cursor: pointer;
                vertical-align: middle;
                display: inline-block;
                width: calc(100% - 35px);
            }
    </style>

    <script>
        document.addEventListener("contextmenu",
            function (e) {
                e.preventDefault();
            });

        document.addEventListener("copy",
            function (e) {
                e.preventDefault();
            });

        document.addEventListener("paste",
            function (e) {
                e.preventDefault();
            });
    </script>

    <script type="text/javascript">
        let countdownTimer;
        let remainingSeconds = 0;

        function initTimer() {
            const hfSecs = document.getElementById('<%= hfRemainingSeconds.ClientID %>');
            let serverSecs = parseInt(hfSecs.value) || 0;

            const studentExamId = "<%= Session["StudentExamId"] %>";
            const storageKey = 'exam_timer_' + studentExamId;

            let localSecs = parseInt(localStorage.getItem(storageKey));

            if (!isNaN(localSecs) && localSecs > 0) {
                // Cheat proof validation: take the minimum of local storage and server remaining seconds
                remainingSeconds = Math.min(serverSecs, localSecs);
            } else {
                remainingSeconds = serverSecs;
            }

            updateDisplay();

            countdownTimer = setInterval(() => {
                if (remainingSeconds <= 0) {
                    clearInterval(countdownTimer);
                    localStorage.removeItem(storageKey);
                    triggerAutoSubmit();
                } else {
                    remainingSeconds--;
                    localStorage.setItem(storageKey, remainingSeconds);
                    updateDisplay();
                }
            }, 1000);
        }

        function updateDisplay() {
            const minutes = Math.floor(remainingSeconds / 60);
            const seconds = remainingSeconds % 60;

            const displayStr =
                (minutes < 10 ? '0' + minutes : minutes) + ':' +
                (seconds < 10 ? '0' + seconds : seconds);

            const lblTimer = document.getElementById('<%= lblTimer.ClientID %>');
            if (lblTimer) {
                lblTimer.innerHTML = displayStr;
            }
        }

        function triggerAutoSubmit() {
            alert("⏰ TIME IS UP! Your examination answers are being submitted automatically.");
            const btnSubmit = document.getElementById('<%= btnSubmit.ClientID %>');
            if (btnSubmit) {
                btnSubmit.click();
            }
        }

        // Prompt user to stay on page during accidental unload
        window.onbeforeunload = function () {
            return "Are you sure you want to leave the exam? Your progress will be saved, but time continues ticking on the server!";
        };

        // Suppress warning on deliberate form submits
        function disableUnloadWarning() {
            window.onbeforeunload = null;
        }
        window.addEventListener('load', function () {

            document.querySelectorAll(
                'input[type=radio], input[type=submit], button')
                .forEach(function (element) {

                    element.addEventListener(
                        'click',
                        function () {

                            disableUnloadWarning();
                        });
                });
        });
        window.addEventListener('load', () => {
            initTimer();
            initFullscreenListeners();
        });
        document.addEventListener("visibilitychange", function () {
            if (document.hidden) {
                __doPostBack('TabSwitch', '');
            }
        });
    </script>
</head>
<body class="exam-body" style="background-color: var(--bg-primary);">
    <form id="form1" runat="server">

        <!-- Top Fullscreen Warning Notice -->
        <div id="fullscreenAlert" class="fullscreen-alert"></div>

        <!-- TOP BAR HEADER -->
        <header class="app-header" style="position: fixed; width: 100%; top: 0; z-index: 100; padding: 0 40px; box-shadow: var(--shadow);">
            <div class="brand-section" style="margin-bottom: 0;">
                <div class="brand-icon">
                    <i class="fa-solid fa-graduation-cap"></i>
                </div>
                <span class="brand-name">OExm Exam Portal</span>
            </div>

            <div class="header-right" style="gap: 24px;">
                <span style="font-weight: 500; font-size: 15px; color: var(--text-secondary);">Attempting: <strong style="color: var(--text-primary);"><%= Session["FullName"] %></strong>
                </span>

                <div style="background-color: rgba(239, 68, 68, 0.08); border: 1px solid var(--danger); padding: 8px 16px; border-radius: 8px; font-weight: 700; color: var(--danger); font-size: 16px; display: flex; align-items: center; gap: 8px;">
                    <i class="fa-solid fa-stopwatch fa-spin"></i>Time Left:

                   

                    <asp:Label ID="lblTimer" runat="server" Text="--:--"></asp:Label>
                </div>
            </div>
        </header>

        <!-- CONTAINER BODY -->
        <div class="content-container" style="margin-top: 90px; padding: 32px 40px;">
            <div class="exam-layout">

                <!-- LEFT PANEL (Question & Options) -->
                <div class="exam-left">

                    <div class="card" style="min-height: 480px; display: flex; flex-direction: column;">

                        <div class="exam-header">
                            <span class="exam-title">
                                <i class="fa-solid fa-circle-info" style="color: var(--primary);"></i>
                                <asp:Label ID="lblSectionName" runat="server" Text="Section: General"></asp:Label>
                            </span>
                            <span style="font-size: 14px; font-weight: 600; color: var(--text-secondary); background-color: var(--bg-primary); padding: 4px 12px; border-radius: 20px; border: 1px solid var(--border-color);">Question <%= Convert.ToInt32(hfQuestionIndex.Value) + 1 %> of <%= (Session["Questions"] != null) ? ((System.Data.DataTable)Session["Questions"]).Rows.Count : 0 %>
                            </span>
                        </div>

                        <!-- QUESTION TEXT -->
                        <div style="font-size: 18px; font-weight: 600; line-height: 1.6; margin-bottom: 24px; color: var(--text-primary); flex-grow: 0;">
                            Q.
                           
                            <asp:Label ID="lblQuestion" runat="server"></asp:Label>
                        </div>

                        <!-- OPTIONS RADIO BUTTON LIST -->
                        <div style="flex-grow: 1;">
                            <asp:RadioButtonList
                                ID="rblOptions"
                                runat="server"
                                CssClass="option-list">
                                <asp:ListItem Value="A"></asp:ListItem>
                                <asp:ListItem Value="B"></asp:ListItem>
                                <asp:ListItem Value="C"></asp:ListItem>
                                <asp:ListItem Value="D"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>

                        <!-- NAVIGATION BUTTONS -->
                        <div style="border-top: 1px solid var(--border-color); padding-top: 24px; margin-top: 24px; display: flex; justify-content: space-between; align-items: center;">

                            <asp:Button ID="btnPrevious"
                                runat="server"
                                Text="Previous"
                                CssClass="btn btn-secondary"
                                Style="font-size: 15px; padding: 12px 24px;"
                                OnClientClick="disableUnloadWarning();"
                                OnClick="btnPrevious_Click" />

                            <div style="display: flex; gap: 12px;">
                                <asp:Button ID="btnMarkForReview"
                                    runat="server"
                                    Text="Mark For Review"
                                    CssClass="btn btn-secondary"
                                    Style="font-size: 15px; padding: 12px 20px;"
                                    OnClientClick="disableUnloadWarning();"
                                    OnClick="btnMarkForReview_Click" />

                                <asp:Button ID="btnClearResponse"
                                    runat="server"
                                    Text="Clear Response"
                                    CssClass="btn btn-secondary"
                                    OnClientClick="disableUnloadWarning();"
                                    OnClick="btnClearResponse_Click" />

                                <asp:Button ID="btnNext"
                                    runat="server"
                                    Text="Save &amp; Next"
                                    CssClass="btn btn-primary"
                                    Style="font-size: 15px; padding: 12px 28px;"
                                    OnClientClick="disableUnloadWarning();"
                                    OnClick="btnNext_Click" />
                            </div>

                        </div>

                    </div>

                </div>

                <!-- RIGHT PANEL (Question Palette & Legends) -->
                <div class="exam-right">

                    <!-- PALETTE CARD -->
                    <div class="card">
                        <h3 class="card-title" style="font-size: 16px; margin-bottom: 16px;"><i class="fa-solid fa-grip"></i>Question Palette</h3>

                        <div class="palette-grid">
                            <asp:Repeater ID="rpPalette" runat="server" OnItemCommand="rpPalette_ItemCommand">
                                <ItemTemplate>
                                    <asp:Button ID="btnPaletteQ"
                                        runat="server"
                                        OnClientClick="disableUnloadWarning();"
                                        Text='<%# Container.ItemIndex + 1 %>'
                                        CommandName="JumpTo"
                                        CommandArgument='<%# Container.ItemIndex %>'
                                        CssClass='<%# GetPaletteClass(Eval("QuestionId")) %>' />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- LEGEND -->
                        <div class="legend" style="border-top: 1px solid var(--border-color); padding-top: 16px; margin-top: 20px;">
                            <div class="legend-item">
                                <div class="legend-color" style="background-color: var(--success);"></div>
                                <span>Answered</span>
                            </div>
                            <div class="legend-item">
                                <div class="legend-color" style="background-color: var(--danger);"></div>
                                <span>Not Answered</span>
                            </div>
                            <div class="legend-item">
                                <div class="legend-color" style="background-color: var(--warning);"></div>
                                <span>For Review</span>
                            </div>
                            <div class="legend-item">
                                <div class="legend-color" style="background-color: var(--gray-palette);"></div>
                                <span>Not Visited</span>
                            </div>
                        </div>

                    </div>

                    <!-- SUBMIT ACTIONS -->
                    <div class="card">
                        <p style="font-size: 12px; color: var(--text-secondary); margin-bottom: 16px; line-height: 1.4;">
                            You can submit your exam at any time. Once submitted, answers cannot be modified.
                       
                        </p>

                        <asp:Button ID="btnSubmit"
                            runat="server"
                            Text="Submit Examination"
                            CssClass="btn btn-danger"
                            Style="width: 100%; font-size: 15px; padding: 14px 20px;"
                            OnClientClick="disableUnloadWarning(); return confirm('Are you sure you want to end and submit this examination?');"
                            OnClick="btnSubmit_Click" />
                    </div>

                </div>

            </div>
        </div>

        <!-- HIDDEN FIELDS -->
        <asp:HiddenField ID="hfQuestionIndex" runat="server" Value="0" />
        <asp:HiddenField ID="hfRemainingSeconds" runat="server" Value="0" />

    </form>
</body>
</html>
