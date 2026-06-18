<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageExams.aspx.cs" Inherits="OExm.ManageExams" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Exams | Online Examination System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="Content/style.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <script src="Scripts/app.js"></script>
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
                        <i class="fa-solid fa-chart-line"></i>Dashboard
                    </a>
                </li>
                <% if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    { %>
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
                    <a href="BulkUpload.aspx" class="nav-item">
                        <i class="fa-solid fa-file-excel"></i>
                        Bulk Upload
                    </a>
                </li>

                <li>
                    <a href="ManageQuestions.aspx" class="nav-item">
                        <i class="fa-solid fa-circle-question"></i>Questions
                        </a>
                </li>
                <% }
                    else
                    { %>
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
                <h1 class="header-title">Manage Exams</h1>
                <div class="header-right">
                    <span style="font-weight: 500; font-size: 14px; opacity: 0.8;">
                        <i class="fa-solid fa-user-shield"></i>Admin Control
                    </span>
                </div>
            </header>

            <!-- CONTENT -->
            <div class="content-container">
                <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 32px; align-items: start;">

                    <!-- FORM CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-pen-to-square"></i>Exam Editor</h3>

                        <div class="form-group">
                            <label class="form-label">Exam Name</label>
                            <asp:TextBox ID="txtExamName" runat="server" CssClass="textbox" placeholder="e.g. Midterm Physics"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Duration (Minutes)</label>
                            <asp:TextBox ID="txtDuration"
                                runat="server"
                                CssClass="textbox"
                                TextMode="Number"
                                placeholder="e.g. 60">
                            </asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Start Date & Time</label>

                            <asp:TextBox ID="txtStartDate"
                                runat="server"
                                CssClass="form-control"
                                TextMode="DateTimeLocal">
                            </asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>End Date & Time</label>

                            <asp:TextBox ID="txtEndDate"
                                runat="server"
                                CssClass="form-control"
                                TextMode="DateTimeLocal">
                            </asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Exam Status</label>

                            <asp:DropDownList
                                ID="ddlExamStatus"
                                runat="server"
                                CssClass="dropdown">

                                <asp:ListItem Text="Draft" Value="Draft"></asp:ListItem>
                                <asp:ListItem Text="Published" Value="Published"></asp:ListItem>
                                <asp:ListItem Text="Closed" Value="Closed"></asp:ListItem>

                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Question Mode</label>

                            <asp:DropDownList
                                ID="ddlQuestionMode"
                                runat="server"
                                CssClass="dropdown"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlQuestionMode_SelectedIndexChanged">

                                <asp:ListItem Text="Random" Value="Random"></asp:ListItem>
                                <asp:ListItem Text="Manual" Value="Manual"></asp:ListItem>

                            </asp:DropDownList>
                        </div>

                        <asp:Panel
                            ID="pnlQuestionCount"
                            runat="server">

                            <div class="form-group">
                                <label class="form-label">
                                    Question Count
                                </label>

                                <asp:TextBox
                                    ID="txtQuestionCount"
                                    runat="server"
                                    CssClass="textbox"
                                    TextMode="Number"
                                    placeholder="e.g. 20">
                                </asp:TextBox>
                            </div>

                        </asp:Panel>

                        <div class="form-group">
                            <label class="form-label">Question Bank</label>

                            <asp:DropDownList
                                ID="ddlQuestionBank"
                                runat="server"
                                CssClass="dropdown"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlQuestionBank_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>

                        <div class="form-group">

                            <label class="form-label">
                                Manual Question Selection
                            </label>

                            <asp:Panel
                                ID="pnlManualQuestions"
                                runat="server">

                                <div style="max-height: 250px; overflow-y: auto; border: 1px solid #ddd; padding: 10px; border-radius: 8px;">

                                    <asp:CheckBoxList
                                        ID="cblQuestions"
                                        runat="server">
                                    </asp:CheckBoxList>

                                </div>

                            </asp:Panel>

                        </div>

                        <div class="form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="4" placeholder="Enter exam details..."></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="chk-label">
                                <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                                Is Active & Accessible
                           
                            </label>
                        </div>

                        <asp:Button ID="btnSave" runat="server" Text="Save Exam" CssClass="btn btn-primary" Style="width: 100%; margin-top: 10px;" OnClick="btnSave_Click" />
                        <asp:HiddenField ID="hfExamId" runat="server" />
                    </div>

                    <!-- GRID CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-list-check"></i>Registered Exams</h3>
                        <div class="table-container" style="margin-top: 0;">
                            <asp:GridView ID="gvExams"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="gridview"
                                DataKeyNames="ExamId"
                                OnRowEditing="gvExams_RowEditing"
                                OnRowDeleting="gvExams_RowDeleting">
                                <Columns>

                                    <asp:BoundField DataField="ExamId" HeaderText="ID" HeaderStyle-Width="60px" />
                                    <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />
                                    <asp:BoundField DataField="DurationMinutes" HeaderText="Mins" HeaderStyle-Width="80px" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" HeaderStyle-Width="80px" />
                                    <asp:CommandField
                                        ShowEditButton="true"
                                        ShowDeleteButton="true"
                                        HeaderText="Actions"
                                        HeaderStyle-Width="120px" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="padding: 24px; text-align: center; color: var(--text-secondary); font-weight: 500;">
                                        No exams created yet. Add one on the left!
                                   
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>

                </div>
            </div>

        </div>

    </form>
</body>
</html>
