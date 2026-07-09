<%@ Page Title="Question Bank" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="QuestionBank.aspx.cs" Inherits="OExm.QuestionBank" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h2 style="margin-bottom:6px;"><i class="fa-solid fa-layer-group"></i> Question Bank</h2>
<p style="color:var(--text-secondary); margin-top:0; margin-bottom:24px;">
    Each bank is its own table in the database (<b>Java</b>, <b>SQL</b>, etc.).
    Add a question here or use <a href="BulkUpload.aspx">Bulk Upload</a> for many at once.
</p>

<div style="display:grid; grid-template-columns:400px 1fr; gap:32px; align-items:start;">

    <!-- LEFT: FORM -->
    <div>

        <!-- CREATE NEW BANK -->
        <div class="card" style="margin-bottom:20px;">
            <h3 class="card-title"><i class="fa-solid fa-plus"></i> Create New Bank</h3>
            <p style="color:var(--text-secondary); font-size:13px; margin-top:-8px; margin-bottom:14px;">
                This will create a SQL table with columns: SlNo, Question, M1, M2, M3, M4, Ans.
            </p>

            <div class="form-group">
                <label class="form-label">Bank Name</label>
                <asp:TextBox ID="txtNewBank" runat="server" CssClass="textbox" placeholder="e.g. Java, Physics, SQL"></asp:TextBox>
            </div>

            <asp:Button ID="btnCreateBank" runat="server" Text="Create Bank Table" CssClass="btn btn-primary"
                Style="width:100%;" OnClick="btnCreateBank_Click" />

            <asp:Label ID="lblBankMsg" runat="server" Style="display:block; margin-top:10px; font-weight:600;"></asp:Label>
        </div>

        <!-- ADD QUESTION -->
        <div class="card">
            <h3 class="card-title"><i class="fa-solid fa-circle-question"></i>
                <asp:Label ID="lblFormTitle" runat="server" Text="Add a Question"></asp:Label>
            </h3>

            <div class="form-group">
                <label class="form-label">Question Bank</label>
                <asp:DropDownList ID="ddlBank" runat="server" CssClass="dropdown"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlBank_Changed"></asp:DropDownList>
            </div>

            <div class="form-group">
                <label class="form-label">Question</label>
                <asp:TextBox ID="txtQuestion" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="3" placeholder="Enter question text"></asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Option M1</label>
                <asp:TextBox ID="txtM1" runat="server" CssClass="textbox" placeholder="Option 1"></asp:TextBox>
            </div>
            <div class="form-group">
                <label class="form-label">Option M2</label>
                <asp:TextBox ID="txtM2" runat="server" CssClass="textbox" placeholder="Option 2"></asp:TextBox>
            </div>
            <div class="form-group">
                <label class="form-label">Option M3</label>
                <asp:TextBox ID="txtM3" runat="server" CssClass="textbox" placeholder="Option 3"></asp:TextBox>
            </div>
            <div class="form-group">
                <label class="form-label">Option M4</label>
                <asp:TextBox ID="txtM4" runat="server" CssClass="textbox" placeholder="Option 4"></asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Correct Answer (must match one option exactly)</label>
                <asp:TextBox ID="txtAns" runat="server" CssClass="textbox" placeholder="e.g. Java Virtual Machine"></asp:TextBox>
            </div>

            <div style="display:flex; gap:10px; margin-top:10px;">
                <asp:Button ID="btnSave" runat="server" Text="Save Question" CssClass="btn btn-primary"
                    Style="flex:1;" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
                    OnClick="btnCancel_Click" />
            </div>

            <asp:HiddenField ID="hfEditSlNo" runat="server" Value="" />
            <asp:Label ID="lblMsg" runat="server" Style="display:block; margin-top:10px; font-weight:600;"></asp:Label>
        </div>

    </div>

    <!-- RIGHT: QUESTIONS LIST -->
    <div class="card">
        <h3 class="card-title"><i class="fa-solid fa-folder-open"></i> Questions in This Bank</h3>

        <asp:TextBox ID="txtSearch" runat="server" CssClass="textbox" placeholder="Filter questions..."
            Style="margin-bottom:12px;" AutoPostBack="true" OnTextChanged="txtSearch_Changed"></asp:TextBox>

        <div class="table-container" style="margin-top:0;">
            <asp:GridView ID="gvQuestions" runat="server" AutoGenerateColumns="false"
                CssClass="gridview" DataKeyNames="SlNo" AllowPaging="true" PageSize="10"
                OnRowEditing="gvQuestions_RowEditing"
                OnRowDeleting="gvQuestions_RowDeleting"
                OnPageIndexChanging="gvQuestions_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="SlNo"     HeaderText="#"        HeaderStyle-Width="50px" />
                    <asp:BoundField DataField="Question" HeaderText="Question" />
                    <asp:BoundField DataField="M1"       HeaderText="M1"       HeaderStyle-Width="100px" />
                    <asp:BoundField DataField="Ans"      HeaderText="Answer"   HeaderStyle-Width="100px" />
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true"
                        HeaderText="Actions" HeaderStyle-Width="120px" />
                </Columns>
                <EmptyDataTemplate>
                    <div style="padding:24px; text-align:center; color:var(--text-secondary);">
                        No questions yet. Add one on the left, or use Bulk Upload.
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

</div>

</asp:Content>
