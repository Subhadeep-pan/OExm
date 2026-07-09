<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="OExm.ForgotPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Forgot Password | OExm Portal</title>

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

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

        .card{
            width:450px;
            max-width:95%;

            background:white;

            border-radius:20px;

            padding:40px;

            box-shadow:
                0 20px 50px rgba(0,0,0,.2);

            text-align:center;
        }

        .icon-box{
            width:80px;
            height:80px;

            margin:auto;

            border-radius:50%;

            background:#eef2ff;

            display:flex;
            justify-content:center;
            align-items:center;
        }

        .icon-box i{
            font-size:35px;
            color:#4f46e5;
        }

        h2{
            margin-top:20px;
            color:#111827;
        }

        .subtitle{
            margin-top:10px;
            margin-bottom:25px;

            color:#6b7280;

            line-height:1.6;
            font-size:14px;
        }

        .input-group{
            position:relative;
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

            outline:none;

            font-size:15px;
        }

        .textbox:focus{
            border-color:#4f46e5;
            box-shadow:0 0 0 4px rgba(79,70,229,.15);
        }

        .btn{
            width:100%;

            margin-top:20px;

            padding:14px;

            border:none;

            border-radius:10px;

            background:#2563eb;

            color:white;

            font-size:16px;

            font-weight:600;

            cursor:pointer;

            transition:.3s;
        }

        .btn:hover{
            background:#1d4ed8;
        }

        .message{
            margin-top:20px;

            color:green;

            font-weight:600;
        }

        .footer{
            margin-top:25px;

            font-size:13px;

            color:#6b7280;
        }

        .footer a{
            text-decoration:none;
            color:#2563eb;
            font-weight:600;
        }

    </style>

</head>

<body>

<form id="form1" runat="server">

    <div class="card">

        <div class="icon-box">

            <i class="fa-solid fa-key"></i>

        </div>

        <h2>Forgot Password</h2>

        <div class="subtitle">

            Enter your registered email address.
            We will send a One-Time Password (OTP)
            to verify your identity.

        </div>

        <div class="input-group">

            <i class="fa-solid fa-envelope"></i>

            <asp:TextBox ID="txtEmail"
                runat="server"
                CssClass="textbox"
                TextMode="Email"
                placeholder="Enter your email address">
            </asp:TextBox>

        </div>

        <asp:Button ID="btnSendOtp"
            runat="server"
            Text="Send OTP"
            CssClass="btn"
            OnClick="btnSendOtp_Click" />

        <div class="message">

            <asp:Label ID="lblMessage"
                runat="server">
            </asp:Label>

        </div>

        <div class="footer">

            Remember your password?

            <a href="Login.aspx">

                Back to Login

            </a>

        </div>

    </div>

</form>

</body>
</html>