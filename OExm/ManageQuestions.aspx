<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageQuestions.aspx.cs" Inherits="OExm.ManageQuestions" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Questions | Online Examination System</title>
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
                <h1 class="header-title">Manage Questions</h1>
                <div class="header-right">
                    <span style="font-weight: 500; font-size: 14px; opacity: 0.8;">
                        <i class="fa-solid fa-user-shield"></i> Admin Control
                    </span>
                </div>
            </header>

            <!-- CONTENT -->
            <div class="content-container">
                <div style="display: grid; grid-template-columns: 380px 1fr; gap: 32px; align-items: start;">
                    
                    <!-- FORM CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-circle-question"></i> Question Editor</h3>
                        
                       <div class="form-group">
    <label class="form-label">Select Exam</label>

    <asp:DropDownList
        ID="ddlExams"
        runat="server"
        CssClass="dropdown"
        AutoPostBack="true"
        OnSelectedIndexChanged="ddlExams_SelectedIndexChanged">
    </asp:DropDownList>
</div>

<div class="form-group">
    <label class="form-label">Question Bank</label>

    <asp:DropDownList
        ID="ddlBank"
        runat="server"
        CssClass="dropdown">
    </asp:DropDownList>
</div>



                        <div class="form-group">
                            <label class="form-label">Select Section</label>
                            <asp:DropDownList ID="ddlSections" runat="server" CssClass="dropdown"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Question Text</label>
                            <asp:TextBox ID="txtQuestion" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="4" placeholder="Enter question description..."></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Option A</label>
                            <asp:TextBox ID="txtOptionA" runat="server" CssClass="textbox" placeholder="Option A text"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Option B</label>
                            <asp:TextBox ID="txtOptionB" runat="server" CssClass="textbox" placeholder="Option B text"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Option C</label>
                            <asp:TextBox ID="txtOptionC" runat="server" CssClass="textbox" placeholder="Option C text"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Option D</label>
                            <asp:TextBox ID="txtOptionD" runat="server" CssClass="textbox" placeholder="Option D text"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Correct Option</label>
                            <asp:DropDownList ID="ddlCorrect" runat="server" CssClass="dropdown">
                                <asp:ListItem Text="Option A" Value="A"></asp:ListItem>
                                <asp:ListItem Text="Option B" Value="B"></asp:ListItem>
                                <asp:ListItem Text="Option C" Value="C"></asp:ListItem>
                                <asp:ListItem Text="Option D" Value="D"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px;">
                            <div class="form-group">
                                <label class="form-label">Positive Marks</label>
                                <asp:TextBox ID="txtPositive" runat="server" CssClass="textbox" placeholder="e.g. 4"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Negative Marks</label>
                                <asp:TextBox ID="txtNegative" runat="server" CssClass="textbox" placeholder="e.g. 1"></asp:TextBox>
                            </div>
                        </div>

                        <asp:Button ID="btnSave" runat="server" Text="Save Question" CssClass="btn btn-primary" style="width: 100%; margin-top: 10px;" OnClick="btnSave_Click" />
                        <asp:HiddenField ID="hfQuestionId" runat="server" />
                    </div>

                    <!-- GRID CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-folder-open"></i> Question Bank</h3>
                        <div class="table-container" style="margin-top: 0;">
                            <asp:GridView ID="gvQuestions"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="gridview"
                                DataKeyNames="QuestionId"
                                OnRowEditing="gvQuestions_RowEditing"
                                OnRowDeleting="gvQuestions_RowDeleting">
                                <Columns>
                                    <asp:BoundField DataField="QuestionId" HeaderText="ID" HeaderStyle-Width="60px" />
                                    <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                                    <asp:BoundField DataField="CorrectOption" HeaderText="Key" HeaderStyle-Width="60px" />
                                    <asp:BoundField DataField="PositiveMarks" HeaderText="[+]" HeaderStyle-Width="60px" />
                                    <asp:BoundField DataField="NegativeMarks" HeaderText="[-]" HeaderStyle-Width="60px" />
                                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" HeaderText="Actions" HeaderStyle-Width="120px" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="padding: 24px; text-align: center; color: var(--text-secondary); font-weight: 500;">
                                        No questions in the database yet. Add one on the left!
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