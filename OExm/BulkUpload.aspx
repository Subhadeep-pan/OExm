<%@ Page Title="Bulk Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BulkUpload.aspx.cs" Inherits="OExm.BulkUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadExtra" runat="server">

<style>

.upload-card{
    max-width:750px;
    margin:auto;
}
.selected-file{
    margin-top:15px;
    padding:12px;
    border-radius:10px;
    background:#EFF6FF;
    border:1px solid #BFDBFE;
    color:#1E3A8A;
    font-weight:600;
}
.upload-header{
    background:linear-gradient(135deg,#1E3A8A,#2563EB);
    color:#fff;
    padding:20px;
    border-radius:15px;
    margin-bottom:20px;
}

.upload-box{
    background:#F8FAFC;
    border:2px dashed #CBD5E1;
    border-radius:15px;
    padding:30px;
    text-align:center;
    margin-bottom:20px;
}

.upload-box:hover{
    border-color:#2563EB;
}

.info{
    color:#64748B;
    font-size:14px;
    margin-top:10px;
}

.message{
    display:block;
    margin-top:15px;
    font-weight:600;
    white-space: pre-line;
}

.template{
    margin-top:10px;
    font-size:13px;
    color:#64748B;
}

.bank-picker{
    text-align:left;
    margin-bottom:20px;
}
.browse-btn{
    display:inline-flex;
    align-items:center;
    gap:10px;
    padding:14px 30px;
    background:#2563EB;
    color:white;
    border-radius:12px;
    cursor:pointer;
    font-weight:600;
    transition:.3s;
}

.browse-btn:hover{
    background:#1D4ED8;
    transform:translateY(-2px);
}
@media(max-width:768px){
    .upload-box{
        padding:20px;
    }
}

</style>


    <script>

function showFileName(input)
{
    var fileBox =
        document.getElementById("selectedFile");

    if(input.files.length > 0)
    {
        var file = input.files[0];

        fileBox.innerHTML =
            "📄 <b>" + file.name +
            "</b><br><br>" +
            "Size : " +
            (file.size / 1024).toFixed(2) +
            " KB";
    }
    else
    {
        fileBox.innerHTML =
            "No file selected";
    }
}

</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="card upload-card">

<div class="upload-header">
    <h2>
        <i class="fa-solid fa-file-excel"></i>
        Bulk Upload Questions
    </h2>
    <p>Upload many questions at once straight from an Excel sheet.</p>
</div>

<div class="bank-picker form-group">
    <label class="form-label">Add these questions to which Question Bank?</label>
    <asp:DropDownList ID="ddlBank" runat="server" CssClass="dropdown"></asp:DropDownList>
</div>


<div class="upload-box">

    <i class="fa-solid fa-cloud-arrow-up fa-2x"></i>

    <br /><br />

    <asp:FileUpload
    ID="fuExcel"
    runat="server"
    Style="display:none;"
    onchange="showFileName(this);" />
    <label for="<%= fuExcel.ClientID %>" class="browse-btn">
    <i class="fa-solid fa-folder-open"></i>
    Browse Excel File
</label>

<br /><br />

<div id="selectedFile" class="selected-file">
    No file selected
</div>

    <div class="info">
        Supported format: .xlsx
    </div>

    <div class="template">
        Expected columns, in this order (a header row on row 1 is fine and skipped automatically):<br />
        <b>SlNo | Question | M1 | M2 | M3 | M4 | Ans</b><br />
        SlNo can be left blank &mdash; it's ignored. <b>Ans</b> must match one of M1-M4 exactly (the correct answer's text, not a letter).
    </div>

</div>

<asp:Button
    ID="btnUpload"
    runat="server"
    Text="Upload Questions"
    CssClass="btn btn-primary"
    OnClick="btnUpload_Click" />

<asp:Label
    ID="lblMessage"
    runat="server"
    CssClass="message">
</asp:Label>


</div>

</asp:Content>
