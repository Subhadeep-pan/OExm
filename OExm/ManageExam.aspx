<%@ Page Title="Exam Builder" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageExam.aspx.cs" Inherits="OExm.ExamBuilder" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .context-bar {
            display: flex;
            align-items: center;
            gap: 16px;
            background: var(--bg-secondary);
            border: 1px solid var(--border-color);
            border-radius: var(--border-radius);
            padding: 16px 20px;
            margin-bottom: 20px;
            box-shadow: var(--shadow);
            flex-wrap: wrap;
        }

        .context-bar-label {
            font-weight: 600;
            font-size: 13px;
            color: var(--text-secondary);
            text-transform: uppercase;
            letter-spacing: .04em;
            white-space: nowrap;
        }

        .context-bar select.dropdown {
            min-width: 260px;
        }

        .search-box {
            width: 100%;
            padding: 8px 12px;
            margin-bottom: 8px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            font-family: var(--font-family);
            background: var(--bg-primary);
            color: var(--text-primary);
        }

        #obToastContainer {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .ob-toast {
            display: flex;
            align-items: center;
            gap: 10px;
            min-width: 260px;
            max-width: 380px;
            padding: 14px 16px;
            border-radius: var(--border-radius);
            background: var(--bg-secondary);
            color: var(--text-primary);
            box-shadow: var(--shadow-hover);
            border-left: 4px solid var(--primary);
            opacity: 0;
            transform: translateX(30px);
            transition: opacity .25s ease, transform .25s ease;
            font-size: 14px;
            font-weight: 500;
        }

        .ob-toast-show { opacity: 1; transform: translateX(0); }
        .ob-toast-success { border-left-color: var(--success); }
        .ob-toast-success i { color: var(--success); }
        .ob-toast-error { border-left-color: var(--danger); }
        .ob-toast-error i { color: var(--danger); }
        .ob-toast-info i { color: var(--primary); }
    </style>

    <div id="obToastContainer"></div>

    <h2 style="margin-bottom: 6px;"><i class="fa-solid fa-pen-to-square"></i> Exam Builder</h2>
    <p style="color: var(--text-secondary); margin-top: 0; margin-bottom: 20px;">
        Set up the exam, then attach questions from the
        <a href="QuestionBank.aspx">Question Bank</a> &mdash; either a random pull or a hand-picked list.
    </p>

    <!-- CONTEXT BAR -->
    <div class="context-bar">
        <span class="context-bar-label"><i class="fa-solid fa-crosshairs"></i> Currently Editing</span>

        <asp:DropDownList ID="ddlContextExam" runat="server" CssClass="dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlContextExam_SelectedIndexChanged">
        </asp:DropDownList>

        <asp:Button ID="btnNewExam" runat="server" Text="+ New Exam" CssClass="btn btn-primary" OnClick="btnNewExam_Click" />
    </div>

    <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 32px; align-items: start;">

        <!-- LEFT: EXAM DETAILS FORM -->
        <div class="card">
            <h3 class="card-title"><i class="fa-solid fa-circle-info"></i> Exam Details</h3>

            <div class="form-group">
                <label class="form-label">Exam Name</label>
                <asp:TextBox ID="txtExamName" runat="server" CssClass="textbox" placeholder="e.g. Midterm Physics"></asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="3" placeholder="Enter exam details..."></asp:TextBox>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px;">
                <div class="form-group">
                    <label class="form-label">Duration (Minutes)</label>
                    <asp:TextBox ID="txtDuration" runat="server" CssClass="textbox" TextMode="Number" placeholder="e.g. 60"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Exam Status</label>
                    <asp:DropDownList ID="ddlExamStatus" runat="server" CssClass="dropdown">
                        <asp:ListItem Text="Draft" Value="Draft"></asp:ListItem>
                        <asp:ListItem Text="Published" Value="Published"></asp:ListItem>
                        <asp:ListItem Text="Closed" Value="Closed"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px;">
                <div class="form-group">
                    <label class="form-label">Passing Marks</label>
                    <asp:TextBox ID="txtPassingMarks" runat="server" CssClass="textbox" TextMode="Number" placeholder="e.g. 40"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">&nbsp;</label>
                    <label class="chk-label">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                        Is Active &amp; Accessible
                    </label>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px;">
                <div class="form-group">
                    <label class="form-label">Marks per Correct Answer</label>
                    <asp:TextBox ID="txtPositivePerQuestion" runat="server" CssClass="textbox" TextMode="Number" placeholder="e.g. 4"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Marks Deducted per Wrong Answer</label>
                    <asp:TextBox ID="txtNegativePerQuestion" runat="server" CssClass="textbox" TextMode="Number" placeholder="e.g. 1"></asp:TextBox>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px;">
                <div class="form-group">
                    <label class="form-label">Start Date &amp; Time</label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="textbox" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">End Date &amp; Time</label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="textbox" TextMode="DateTimeLocal"></asp:TextBox>
                </div>
            </div>

            <hr style="border-color: var(--border-color); margin: 16px 0;" />

            <div class="form-group">
                <label class="form-label">How should questions be added to this exam?</label>
                <asp:DropDownList ID="ddlQuestionMode" runat="server" CssClass="dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestionMode_SelectedIndexChanged">
                    <asp:ListItem Text="Pick Randomly From a Bank" Value="Random"></asp:ListItem>
                    <asp:ListItem Text="Manually Choose Questions" Value="Manual"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Panel ID="pnlQuestionCount" runat="server">
                <div class="form-group">
                    <label class="form-label">Question Bank</label>
                    <asp:DropDownList ID="ddlQuestionBank" runat="server" CssClass="dropdown"></asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">How many questions to pull?</label>
                    <asp:TextBox ID="txtQuestionCount" runat="server" CssClass="textbox" TextMode="Number" placeholder="e.g. 20"></asp:TextBox>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlManualQuestions" runat="server">
                <div class="form-group">
                    <label class="form-label">Question Bank</label>
                    <asp:DropDownList ID="ddlManualBank" runat="server" CssClass="dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlManualBank_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Select Questions <span id="manualCount" style="font-weight: 400; color: var(--text-secondary);"></span></label>
                    <asp:TextBox ID="txtManualSearch" runat="server" CssClass="search-box" placeholder="Type to filter questions..."></asp:TextBox>
                    <div style="max-height: 260px; overflow-y: auto; border: 1px solid var(--border-color); padding: 10px; border-radius: 8px;">
                        <asp:CheckBoxList ID="cblManualQuestions" runat="server"></asp:CheckBoxList>
                    </div>
                </div>
            </asp:Panel>

            <asp:Button ID="btnSaveExam" runat="server" Text="Save Exam" CssClass="btn btn-primary" Style="width: 100%; margin-top: 10px;" OnClick="btnSaveExam_Click" />
            <asp:HiddenField ID="hfExamId" runat="server" />
        </div>

        <!-- RIGHT: ALL EXAMS -->
        <div class="card">
            <h3 class="card-title"><i class="fa-solid fa-list-check"></i> All Exams</h3>
            <div class="table-container" style="margin-top: 0;">
                <asp:GridView ID="gvExams" runat="server" AutoGenerateColumns="false" CssClass="gridview"
                    DataKeyNames="ExamId" AllowPaging="true" PageSize="8"
                    OnRowEditing="gvExams_RowEditing" OnRowDeleting="gvExams_RowDeleting" OnPageIndexChanging="gvExams_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="ExamId" HeaderText="ID" HeaderStyle-Width="50px" />
                        <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                        <asp:BoundField DataField="ExamStatus" HeaderText="Status" HeaderStyle-Width="90px" />
                        <asp:BoundField DataField="DurationMinutes" HeaderText="Mins" HeaderStyle-Width="70px" />
                        <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" HeaderText="Actions" HeaderStyle-Width="120px" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="padding: 24px; text-align: center; color: var(--text-secondary); font-weight: 500;">
                            No exams yet. Fill the form on the left to create your first one.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <script>
        function showToast(msg, type) {
            var container = document.getElementById('obToastContainer');
            if (!container) return;
            var toast = document.createElement('div');
            toast.className = 'ob-toast ob-toast-' + (type || 'info');
            var icon = type === 'error' ? 'fa-circle-exclamation' : (type === 'success' ? 'fa-circle-check' : 'fa-circle-info');
            toast.innerHTML = '<i class="fa-solid ' + icon + '"></i><span>' + msg + '</span>';
            container.appendChild(toast);
            setTimeout(function () { toast.classList.add('ob-toast-show'); }, 10);
            setTimeout(function () {
                toast.classList.remove('ob-toast-show');
                setTimeout(function () { toast.remove(); }, 300);
            }, 4500);
        }

        document.addEventListener('DOMContentLoaded', function () {
            var searchBox = document.getElementById('<%= txtManualSearch.ClientID %>');
            var list = document.getElementById('<%= cblManualQuestions.ClientID %>');
            if (searchBox && list) {
                searchBox.addEventListener('keyup', function () {
                    var filter = this.value.toLowerCase();
                    var rows = list.querySelectorAll('tr');
                    rows.forEach(function (row) {
                        row.style.display = row.textContent.toLowerCase().indexOf(filter) > -1 ? '' : 'none';
                    });
                });
            }
        });
    </script>

</asp:Content>
