<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="OExm.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OExm Assessment Portal</title>

<meta name="viewport" content="width=device-width, initial-scale=1" />

<link rel="stylesheet"
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

<style>

    * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
        font-family: 'Segoe UI', sans-serif;
    }

    body {
        min-height: 100vh;
        display: flex;
        justify-content: center;
        align-items: center;
        background: linear-gradient(
            135deg,
            #0F172A,
            #1E3A8A,
            #14B8A6);
        padding: 20px;
    }

    .container {
        width: 1050px;
        max-width: 100%;
        display: flex;
        overflow: hidden;
        border-radius: 24px;
        background: rgba(255,255,255,.97);
        box-shadow: 0 25px 60px rgba(0,0,0,.25);
        animation: fadeIn .6s ease;
    }

    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: translateY(25px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .left-panel {
        width: 50%;
        padding: 60px;
        color: white;
        background: linear-gradient(
            135deg,
            #1E3A8A,
            #0F172A);
        display: flex;
        flex-direction: column;
        justify-content: center;
    }

    .brand {
        display: flex;
        align-items: center;
        gap: 15px;
        margin-bottom: 25px;
    }

    .brand i {
        font-size: 48px;
    }

    .brand h1 {
        font-size: 38px;
        font-weight: 700;
    }

    .tagline {
        font-size: 17px;
        line-height: 1.8;
        opacity: .95;
        margin-bottom: 30px;
    }

    .features {
        list-style: none;
    }

    .features li {
        margin-bottom: 15px;
        font-size: 16px;
    }

    .features i {
        color: #14B8A6;
        margin-right: 10px;
    }

    .right-panel {
        width: 50%;
        padding: 50px;
        background: #ffffff;
    }

    .login-header {
        text-align: center;
        margin-bottom: 30px;
    }

    .login-header i {
        font-size: 60px;
        color: #2563EB;
        margin-bottom: 10px;
    }

    .login-header h2 {
        color: #111827;
        margin-bottom: 8px;
    }

    .login-header p {
        color: #6B7280;
    }

    .success-box {
        background: #DCFCE7;
        color: #15803D;
        padding: 12px;
        border-radius: 10px;
        text-align: center;
        margin-bottom: 15px;
    }

    .error-box {
        background: #FEE2E2;
        color: #DC2626;
        padding: 12px;
        border-radius: 10px;
        text-align: center;
        margin-bottom: 15px;
    }

    .input-group {
        position: relative;
        margin-bottom: 18px;
    }

    .input-group i {
        position: absolute;
        top: 17px;
        left: 15px;
        color: #6B7280;
    }

    .textbox {
        width: 100%;
        padding: 14px 14px 14px 45px;
        border: 1px solid #D1D5DB;
        border-radius: 12px;
        font-size: 15px;
        transition: .3s;
    }

    .textbox:focus {
        outline: none;
        border-color: #2563EB;
        box-shadow: 0 0 0 4px rgba(37,99,235,.15);
    }

    .password-row {
        display: flex;
        justify-content: flex-end;
        margin-bottom: 20px;
    }

    .show-password {
        font-size: 14px;
        color: #4B5563;
        cursor: pointer;
    }

    .btn {
        width: 100%;
        padding: 14px;
        border: none;
        border-radius: 12px;
        background: linear-gradient(
            135deg,
            #2563EB,
            #1E3A8A);
        color: white;
        font-size: 16px;
        font-weight: 600;
        cursor: pointer;
        transition: .3s;
    }

    .btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 10px 25px rgba(37,99,235,.30);
    }

    .links {
        text-align: center;
        margin-top: 25px;
    }

    .links a {
        text-decoration: none;
        color: #2563EB;
        font-weight: 600;
    }

    .links a:hover {
        text-decoration: underline;
    }

    .divider {
        margin: 15px 0;
        color: #9CA3AF;
    }

    @media(max-width:768px) {

        .container {
            flex-direction: column;
        }

        .left-panel,
        .right-panel {
            width: 100%;
        }

        .left-panel {
            padding: 35px;
            text-align: center;
        }

        .right-panel {
            padding: 35px;
        }

        .brand {
            justify-content: center;
        }

        .brand h1 {
            font-size: 30px;
        }
    }

</style>

<script>

    function togglePassword() {

        var txt =
            document.getElementById('<%= txtPassword.ClientID %>');

        txt.type =
            txt.type === "password"
            ? "text"
            : "password";
    }

    window.addEventListener("pageshow", function (event) {

        if (event.persisted) {
            window.location.reload();
        }

    });

    document.addEventListener("keydown", function (e) {

        if (e.key === "Enter") {

            e.preventDefault();

            document.getElementById(
                '<%= btnLogin.ClientID %>'
            ).click();
        }

    });

</script>


</head>

<body>

<form id="form1"
      runat="server"
      autocomplete="off">

<div class="container">

    <div class="left-panel">

        <div class="brand">
            <i class="fa-solid fa-graduation-cap"></i>
            <h1>OExm</h1>
        </div>

        <p class="tagline">
            Secure, Scalable and Intelligent Online Examination Platform
            for schools, colleges, training institutes and professional assessments.
        </p>

        <ul class="features">

            <li>
                <i class="fa-solid fa-circle-check"></i>
                Secure Authentication System
            </li>

            <li>
                <i class="fa-solid fa-circle-check"></i>
                Timer Based Online Exams
            </li>

            <li>
                <i class="fa-solid fa-circle-check"></i>
                Anti-Cheating Monitoring
            </li>

            <li>
                <i class="fa-solid fa-circle-check"></i>
                Automated Evaluation
            </li>

            <li>
                <i class="fa-solid fa-circle-check"></i>
                Detailed Performance Analytics
            </li>

        </ul>

    </div>

    <div class="right-panel">

        <div class="login-header">

            <i class="fa-solid fa-user-shield"></i>

            <h2>Welcome Back</h2>

            <p>Sign in to continue to your account</p>

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

            <i class="fa-regular fa-user"></i>

            <asp:TextBox ID="txtUsername"
                         runat="server"
                         CssClass="textbox"
                         placeholder="Enter Username"
                         autocomplete="off">
            </asp:TextBox>

        </div>

        <div class="input-group">

            <i class="fa-solid fa-lock"></i>

            <asp:TextBox ID="txtPassword"
                         runat="server"
                         CssClass="textbox"
                         TextMode="Password"
                         placeholder="Enter Password"
                         autocomplete="new-password">
            </asp:TextBox>

        </div>

        <div class="password-row">

            <label class="show-password">

                <input type="checkbox"
                       onclick="togglePassword()" />

                Show Password

            </label>

        </div>

        <asp:Button ID="btnLogin"
                    runat="server"
                    Text="Sign In"
                    CssClass="btn"
                    OnClick="btnLogin_Click" />

        <div class="links">

            <a href="ForgotPassword.aspx">
                Forgot Password?
            </a>

            <div class="divider">or</div>

            <a href="Register.aspx">
                Create New Account
            </a>

        </div>

    </div>

</div>

</form>

</body>
</html>
