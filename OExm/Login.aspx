<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="OExm.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>OExm Assessment Portal</title>

    <meta name="viewport" content="width=device-width, initial-scale=1" />

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
            font-size:40px;
            margin-bottom:20px;
        }

        .left-panel p{
            line-height:1.8;
            opacity:.9;
        }

        .right-panel{
            width:50%;
            padding:50px;
            background:white;
        }

        .logo{
            text-align:center;
            margin-bottom:25px;
        }

        .logo i{
            font-size:55px;
            color:#2563eb;
        }

        .logo h2{
            margin-top:10px;
            color:#111827;
        }

        .textbox{
            width:100%;
            padding:14px 14px 14px 45px;
            border:1px solid #d1d5db;
            border-radius:10px;
            margin-top:8px;
            margin-bottom:18px;
            font-size:15px;
        }

        .input-group{
            position:relative;
        }

        .input-group i{
            position:absolute;
            top:18px;
            left:15px;
            color:#6b7280;
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

        .links{
            text-align:center;
            margin-top:20px;
        }

        .links a{
            text-decoration:none;
            color:#2563eb;
            font-weight:600;
        }

        .error-box{
            background:#fee2e2;
            color:#dc2626;
            padding:10px;
            border-radius:8px;
            text-align:center;
            margin-bottom:15px;
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
        }

    </style>

    <script>

        function togglePassword()
        {
            var txt =
                document.getElementById('<%= txtPassword.ClientID %>');

            if(txt.type === "password")
                txt.type = "text";
            else
                txt.type = "password";
        }
       
            window.addEventListener("pageshow", function (event)
            {
    if (event.persisted)
            {
                window.location.reload();
    }
});
    

        document.addEventListener("keypress", function (e) {

            if (e.key === "Enter") {

                document.getElementById('<%= btnLogin.ClientID %>').click();

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

        <h1>OExm Portal</h1>

        <p>
            Secure Online Examination Platform
            for educational institutions,
            training centres and assessments.
        </p>

        <br />

        <p>

            ✓ Secure Authentication<br />
            ✓ Timer Based Exams<br />
            ✓ Auto Evaluation<br />
            ✓ Result Analytics

        </p>

    </div>

    <div class="right-panel">

        <div class="logo">

            <i class="fa-solid fa-graduation-cap"></i>

            <h2>Sign In</h2>

        </div>


        <asp:Panel ID="pnlSuccess"
    runat="server"
    Visible="true"
    style="
        background:#dcfce7;
        color:#15803d;
        padding:10px;
        border-radius:8px;
        text-align:center;
        margin-bottom:15px;">

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
    autocomplete="off"
                runat="server"
                CssClass="textbox"
                placeholder="Username">
            </asp:TextBox>

        </div>

        <div class="input-group">

            <i class="fa-solid fa-lock"></i>

           <asp:TextBox ID="txtPassword"
    autocomplete="new-password"
                runat="server"
                CssClass="textbox"
                TextMode="Password"
                placeholder="Password">
            </asp:TextBox>

        </div>

        <div class="show-pass">

            <input type="checkbox"
                   onclick="togglePassword()" />

            Show Password

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

            <br /><br />

            <a href="Register.aspx">
                Create New Account
            </a>

        </div>

    </div>

</div>

</form>

</body>

</html>