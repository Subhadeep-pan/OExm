<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="OExm.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

<title>Register | OExm Portal</title>

<meta name="viewport" content="width=device-width, initial-scale=1.0" />

<link rel="stylesheet"
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

<style>

    *{
        margin:0;
        padding:0;
        box-sizing:border-box;
        font-family:'Segoe UI';
    }

    body{
        min-height:100vh;
        display:flex;
        justify-content:center;
        align-items:center;
        background:linear-gradient(
            135deg,
            #2563eb,
            #4f46e5,
            #7c3aed);
    }

    .container{
        width:1000px;
        max-width:95%;
        display:flex;
        overflow:hidden;
        border-radius:20px;
        background:white;
        box-shadow:0 20px 60px rgba(0,0,0,.2);
    }

    .left-panel{
        width:50%;
        background:linear-gradient(
            135deg,
            #2563eb,
            #1e40af);
        color:white;
        padding:60px;
        display:flex;
        flex-direction:column;
        justify-content:center;
    }

    .left-panel h1{
        font-size:42px;
        margin-bottom:20px;
    }

    .left-panel p{
        line-height:1.8;
        opacity:.9;
    }

    .right-panel{
        width:50%;
        padding:50px;
    }

    .logo{
        text-align:center;
        margin-bottom:20px;
    }

    .logo i{
        font-size:55px;
        color:#2563eb;
    }

    .logo h2{
        margin-top:10px;
        color:#111827;
    }

    .input-group{
        position:relative;
        margin-bottom:15px;
    }

    .input-group i{
        position:absolute;
        left:15px;
        top:17px;
        color:#6b7280;
    }

    .textbox{
        width:100%;
        padding:14px 14px 14px 45px;
        border:1px solid #d1d5db;
        border-radius:10px;
        font-size:15px;
        outline:none;
    }

    .textbox:focus{
        border-color:#2563eb;
        box-shadow:0 0 0 4px rgba(37,99,235,.15);
    }

    .btn{
        width:100%;
        padding:14px;
        border:none;
        border-radius:10px;
        background:#2563eb;
        color:white;
        font-size:16px;
        cursor:pointer;
        transition:.3s;
    }

    .btn:hover{
        background:#1d4ed8;
    }

    .success-box{
        background:#dcfce7;
        color:#15803d;
        padding:10px;
        border-radius:8px;
        text-align:center;
        margin-bottom:15px;
    }

    .error-box{
        background:#fee2e2;
        color:#dc2626;
        padding:10px;
        border-radius:8px;
        text-align:center;
        margin-bottom:15px;
    }

    .links{
        text-align:center;
        margin-top:20px;
    }

    .links a{
        color:#2563eb;
        text-decoration:none;
        font-weight:600;
    }

    .show-pass{
        margin-bottom:15px;
        font-size:14px;
    }

    @media(max-width:768px){

        .container{
            flex-direction:column;
        }

        .left-panel,
        .right-panel{
            width:100%;
        }

        .left-panel{
            padding:35px;
        }

        .right-panel{
            padding:35px;
        }
    }

</style>

<script>

    function togglePassword()
    {
        var p1 =
            document.getElementById('<%= txtPassword.ClientID %>');

        var p2 =
            document.getElementById('<%= txtConfirmPassword.ClientID %>');

        if(p1.type === "password")
        {
            p1.type = "text";
            p2.type = "text";
        }
        else
        {
            p1.type = "password";
            p2.type = "password";
        }
    }

</script>

</head>

<body>

<form id="form1" runat="server" autocomplete="off">

<div class="container">

<div class="left-panel">

    <h1>Join OExm</h1>

    <p>
        Create your account and access a secure,
        professional online examination platform.
    </p>

    <br />

    <p>
        ✓ Secure Authentication<br />
        ✓ Online Exams<br />
        ✓ Instant Results<br />
        ✓ Performance Analytics
    </p>

</div>

<div class="right-panel">

    <div class="logo">

        <i class="fa-solid fa-user-plus"></i>

        <h2>Create Account</h2>

    </div>

    <asp:Panel ID="pnlSuccess"
        runat="server"
        Visible="false"
        CssClass="success-box">

        <asp:Label ID="lblSuccess"
            runat="server">
        </asp:Label>

    </asp:Panel>

    <asp:Panel ID="pnlError"
        runat="server"
        Visible="false"
        CssClass="error-box">

        <asp:Label ID="lblError"
            runat="server">
        </asp:Label>

    </asp:Panel>

    <div class="input-group">
        <i class="fa-solid fa-user"></i>

        <asp:TextBox ID="txtFullName"
    runat="server"
    CssClass="textbox"
    placeholder="Full Name"
    autocomplete="off">
</asp:TextBox>
    </div>

    <div class="input-group">
        <i class="fa-solid fa-id-card"></i>

       <asp:TextBox ID="txtUsername"
    runat="server"
    CssClass="textbox"
    placeholder="Choose Username"
    autocomplete="off">
</asp:TextBox>
    </div>

    <div class="input-group">
        <i class="fa-solid fa-envelope"></i>

        <asp:TextBox ID="txtEmail"
            runat="server"
            CssClass="textbox"
            TextMode="Email"
            placeholder="Email Address">
        </asp:TextBox>
    </div>

    <div class="input-group">
        <i class="fa-solid fa-lock"></i>

        <asp:TextBox ID="txtPassword"
    runat="server"
    CssClass="textbox"
    TextMode="Password"
    placeholder="Create Password"
    autocomplete="new-password">
</asp:TextBox>
    </div>

    <div class="input-group">
        <i class="fa-solid fa-lock"></i>

        <asp:TextBox ID="txtConfirmPassword"
    runat="server"
    CssClass="textbox"
    TextMode="Password"
    placeholder="Confirm Password"
    autocomplete="new-password">
</asp:TextBox>

        <input type="text"
       style="display:none" />

<input type="password"
       style="display:none" />
    </div>

    <div class="show-pass">

        <input type="checkbox"
               onclick="togglePassword()" />

        Show Password

    </div>

    <asp:Button ID="btnRegister"
        runat="server"
        Text="Create Account"
        CssClass="btn"
        OnClick="btnRegister_Click" />

    <div class="links">

        <a href="Login.aspx">
            Already have an account? Login
        </a>

    </div>

</div>

</div>

</form>

</body>
</html>
