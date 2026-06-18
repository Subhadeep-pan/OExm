<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageSections.aspx.cs" Inherits="OExm.ManageSections" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Sections | Online Examination System</title>
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
                <h1 class="header-title">Manage Sections</h1>
                <div class="header-right">
                    <span style="font-weight: 500; font-size: 14px; opacity: 0.8;">
                        <i class="fa-solid fa-user-shield"></i> Admin Control
                    </span>
                </div>
            </header>

            <!-- CONTENT -->
            <div class="content-container">
                <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 32px; align-items: start;">
                    
                    <!-- FORM CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-layer-group"></i> Section Editor</h3>
                        
                        <div class="form-group">
                            <label class="form-label">Select Exam</label>
                            <asp:DropDownList ID="ddlExams" runat="server" CssClass="dropdown"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Section Name</label>
                            <asp:TextBox ID="txtSectionName" runat="server" CssClass="textbox" placeholder="e.g. Physics, Chemistry"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Order Index</label>
                            <asp:TextBox ID="txtOrderIndex"
    runat="server"
    CssClass="textbox"
    TextMode="Number"
    placeholder="e.g. 1">
</asp:TextBox>
                        </div>
                        <asp:Label ID="lblMessage"
    runat="server"
    ForeColor="Green"
    Font-Bold="true">
</asp:Label>

<br />
<br />
                        <asp:Button ID="btnSave" runat="server" Text="Save Section" CssClass="btn btn-primary" style="width: 100%; margin-top: 10px;" OnClick="btnSave_Click" />
                        <asp:HiddenField ID="hfSectionId" runat="server" />
                    </div>

                    <!-- GRID CARD -->
                    <div class="card">
                        <h3 class="card-title"><i class="fa-solid fa-bars-staggered"></i> Registered Sections</h3>
                        <div class="table-container" style="margin-top: 0;">
                            <asp:GridView ID="gvSections"
                                runat="server"
                                AutoGenerateColumns="false"
                                CssClass="gridview"
                                DataKeyNames="SectionId"
                                OnRowEditing="gvSections_RowEditing"
                                OnRowDeleting="gvSections_RowDeleting">
                                <Columns>
                                    <asp:BoundField DataField="SectionId" HeaderText="ID" HeaderStyle-Width="60px" />
                                    <asp:BoundField DataField="ExamName" HeaderText="Exam Title" />
                                    <asp:BoundField DataField="SectionName" HeaderText="Section Name" />
                                    <asp:BoundField DataField="OrderIndex" HeaderText="Order Index" HeaderStyle-Width="100px" />
                                    <asp:TemplateField HeaderText="Actions">
    <ItemTemplate>

        <asp:LinkButton
            ID="btnEdit"
            runat="server"
            CommandName="Edit"
            Text="Edit">
        </asp:LinkButton>

        &nbsp;

        <asp:LinkButton
            ID="btnDelete"
            runat="server"
            CommandName="Delete"
            Text="Delete"
            OnClientClick="return confirm('Delete this question?');">
        </asp:LinkButton>

    </ItemTemplate>
</asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="padding: 24px; text-align: center; color: var(--text-secondary); font-weight: 500;">
                                        No sections created yet. Add one on the left!
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