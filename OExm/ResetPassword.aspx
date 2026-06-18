<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="OExm.ResetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Reset Password | OExm Portal</title>

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

            text-align:center;

            box-shadow:
                0 20px 50px rgba(0,0,0,.2);
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
            font-size:14px;
            line-height:1.6;
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

            outline:none;

            font-size:15px;
        }

        .textbox:focus{
            border-color:#4f46e5;
            box-shadow:0 0 0 4px rgba(79,70,229,.15);
        }

        .showpass{
            text-align:left;
            margin-bottom:15px;
            font-size:14px;
            color:#374151;
        }

        .btn{
            width:100%;

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
            font-weight:600;
            color:#16a34a;
        }

    </style>

    <script>

        function togglePassword()
        {
            var p1 =
                document.getElementById('<%= txtPassword.ClientID %>');

            var p2 =
                document.getElementById('<%= txtConfirmPassword.ClientID %>');

            if (p1.type === "password")
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

<form id="form1" runat="server">

    <div class="card">

        <div class="icon-box">

            <i class="fa-solid fa-lock"></i>

        </div>

        <h2>Reset Password</h2>

        <div class="subtitle">

            Create a strong password for your account.
            Make sure it is easy for you to remember
            and difficult for others to guess.

        </div>

        <div class="input-group">

            <i class="fa-solid fa-key"></i>

            <asp:TextBox ID="txtPassword"
                runat="server"
                CssClass="textbox"
                TextMode="Password"
                placeholder="New Password">
            </asp:TextBox>

        </div>

        <div class="input-group">

            <i class="fa-solid fa-key"></i>

            <asp:TextBox ID="txtConfirmPassword"
                runat="server"
                CssClass="textbox"
                TextMode="Password"
                placeholder="Confirm Password">
            </asp:TextBox>

        </div>

        <div class="showpass">

            <input type="checkbox"
                   onclick="togglePassword()" />

            Show Password

        </div>

        <asp:Button ID="btnReset"
            runat="server"
            Text="Reset Password"
            CssClass="btn"
            OnClick="btnReset_Click" />

        <div class="message">

            <asp:Label ID="lblMessage"
                runat="server">
            </asp:Label>

        </div>

    </div>

</form>

</body>
</html>