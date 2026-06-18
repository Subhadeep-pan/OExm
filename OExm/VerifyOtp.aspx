<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyOtp.aspx.cs" Inherits="OExm.VerifyOtp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Verify OTP | OExm Portal</title>

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
            width:430px;
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
            font-size:36px;
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

            font-size:14px;
            line-height:1.6;
        }

        .textbox{
            width:100%;

            padding:14px;

            border:1px solid #d1d5db;

            border-radius:10px;

            text-align:center;

            font-size:22px;

            letter-spacing:6px;

            outline:none;

            transition:.3s;
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

            color:#dc2626;

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

            <i class="fa-solid fa-shield-halved"></i>

        </div>

        <h2>Verify OTP</h2>

        <div class="subtitle">

            Enter the 6-digit OTP sent to your email address.

        </div>

        <asp:TextBox ID="txtOtp"
            runat="server"
            CssClass="textbox"
            MaxLength="6"
            placeholder="******">
        </asp:TextBox>

        <asp:Button ID="btnVerify"
            runat="server"
            Text="Verify OTP"
            CssClass="btn"
            OnClick="btnVerify_Click" />

        <div class="message">

            <asp:Label ID="lblMessage"
                runat="server">
            </asp:Label>

        </div>

        <div class="footer">

            Didn't receive OTP?

            <a href="ForgotPassword.aspx">

                Send Again

            </a>

        </div>

    </div>

</form>

</body>
</html>