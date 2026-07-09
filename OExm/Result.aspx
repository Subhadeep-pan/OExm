<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Result.aspx.cs"
    Inherits="OExm.Result" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Exam Result</title>

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />

    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI';
        }

        body {
            background: #f1f5f9;
            padding: 30px;
        }

        .result-card {
            max-width: 1000px;
            margin: auto;
            background: white;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 10px 30px rgba(0,0,0,.1);
        }

        .result-header {
            background: linear-gradient(135deg,#2563eb,#1e40af);
            color: white;
            text-align: center;
            padding: 25px;
        }

            .result-header h1 {
                font-size: 32px;
                margin-bottom: 10px;
            }

            .result-header p {
                opacity: .9;
            }

        .student-info {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            padding: 25px;
            border-bottom: 1px solid #e5e7eb;
        }

        .info-item {
            font-size: 16px;
        }

            .info-item strong {
                color: #1e293b;
            }

        .result-table {
            width: 100%;
            border-collapse: collapse;
        }

            .result-table td {
                padding: 18px;
                border: 1px solid #e5e7eb;
            }

                .result-table td:first-child {
                    font-weight: 600;
                    background: #f8fafc;
                }

        .score-row {
            background: #eff6ff;
            font-size: 18px;
            font-weight: 700;
        }

        .result-footer {
            padding: 25px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 15px;
        }

        .pass {
            color: #16a34a;
            font-size: 22px;
            font-weight: 700;
        }

        .fail {
            color: #dc2626;
            font-size: 22px;
            font-weight: 700;
        }

        .btn {
            border: none;
            padding: 12px 20px;
            border-radius: 10px;
            cursor: pointer;
            font-size: 15px;
        }

        .btn-primary {
            background: #2563eb;
            color: white;
        }

            .btn-primary:hover {
                background: #1d4ed8;
            }

        .btn-danger {
            background: #dc2626;
            color: white;
        }

        @media(max-width:768px) {

            .student-info {
                grid-template-columns: 1fr;
            }

            .result-footer {
                flex-direction: column;
            }
        }
    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="result-card">

            <!-- HEADER -->

            <div class="result-header">

                <h1>
                    <i class="fa-solid fa-graduation-cap"></i>
                    OExm Examination Result
                </h1>

                <p>
                    Online Examination System
                </p>

            </div>

            <!-- STUDENT INFO -->

            <div class="student-info">

                <div class="info-item">
                    <strong>Student Name :</strong>
                    <asp:Label ID="lblStudentName"
                        runat="server" />
                </div>

                <div class="info-item">
                    <strong>Exam :</strong>
                    <asp:Label ID="lblExamName"
                        runat="server" />
                </div>

                <div class="info-item">
                    <strong>Date :</strong>
                    <asp:Label ID="lblDate"
                        runat="server" />
                </div>

                <div class="info-item">
                    <strong>Grade :</strong>
                    <asp:Label ID="lblGrade"
                        runat="server" />
                </div>

            </div>

            <!-- RESULT TABLE -->

            <table class="result-table">

                <tr>
                    <td>Total Questions</td>
                    <td>
                        <asp:Label ID="lblTotalQuestions"
                            runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>Attempted Questions</td>
                    <td>
                        <asp:Label ID="lblAttempted"
                            runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>Unattempted Questions</td>
                    <td>
                        <asp:Label ID="lblUnattempted"
                            runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>Correct Answers</td>
                    <td>
                        <asp:Label ID="lblCorrect"
                            runat="server" />
                    </td>
                </tr>

                <tr>
                    <td>Wrong Answers</td>
                    <td>
                        <asp:Label ID="lblWrong"
                            runat="server" />
                    </td>
                </tr>

                <tr class="score-row">

                    <td>Final Score</td>

                    <td>

                        <asp:Label ID="lblScore"
                            runat="server" />

                        |

                <asp:Label ID="lblPercentage"
                    runat="server" />

                        %

                    </td>

                </tr>

            </table>

            <!-- FOOTER -->

            <div class="result-footer">

                <asp:Label ID="lblResultStatus"
                    runat="server"
                    CssClass="pass" />

                <div>

                    <button type="button"
                        onclick="window.print();"
                        class="btn btn-primary">

                        <i class="fa-solid fa-print"></i>
                        Print

                    </button>

                    <asp:Button ID="btnDownloadPDF"
                        runat="server"
                        Text="Download PDF"
                        CssClass="btn btn-primary"
                        OnClick="btnDownloadPDF_Click" />

                    <a href="Login.aspx?logout=true" class="nav-item logout-btn">
                        <i class="fa-solid fa-right-from-bracket"></i>Logout</a>

                </div>

            </div>

        </div>

    </form>

</body>

</html>
