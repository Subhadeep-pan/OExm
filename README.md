<div align="center">

# 🎓 OExm — Online Examination System

**A full-featured web-based examination platform built with ASP.NET Web Forms, C#, SQL Server, and ADO.NET.**

Designed for educational institutions to manage exams, question banks, and student results — all from a clean admin panel.

![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-blue?style=flat-square)
![CSharp](https://img.shields.io/badge/C%23-.NET%204.7-purple?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-red?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

</div>

---

## 📋 Table of Contents

- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Project Structure](#-project-structure)
- [Database Design](#-database-design)
- [Getting Started](#-getting-started)
- [Default Login](#-default-login)
- [Bulk Upload Format](#-bulk-upload-excel-format)
- [Exam Workflow](#-exam-workflow)
- [Screenshots](#-screenshots)

---

## ✨ Features

### 👨‍💼 Admin
| Feature | Description |
|---|---|
| **Exam Builder** | Create exams with name, duration, dates, passing marks, per-question positive/negative marks |
| **Question Bank** | Each bank (Java, DBMS, SQL...) is its own SQL table — add, edit, delete questions |
| **Bulk Upload** | Upload hundreds of questions at once via `.xlsx` |
| **Manual & Random Mode** | Pick questions by hand, or let the system pull N random questions from a bank |
| **Publish Control** | Set exam status (Draft / Published / Closed) with start and end dates |
| **Admin Dashboard** | Live stats — total students, exams, average score, violations flagged |
| **Violations Tracking** | Tab-switch violations logged and shown on the dashboard |
| **Database Manager** | Super-admin panel to view, add and delete rows in any table; run custom SQL |

### 🧑‍🎓 Student
| Feature | Description |
|---|---|
| **Instructions Page** | See full exam details before starting — duration, questions, marks, dates |
| **Live Exam Portal** | Color-coded question navigator, countdown timer, Mark for Review, Clear Response |
| **One Attempt Only** | Students cannot re-attempt a completed exam |
| **Auto Submit** | Exam auto-submits when the timer hits zero |
| **Result Page** | Instant score, percentage, grade (A+/A/B/C/F), pass/fail, download PDF certificate |

### 🔐 Auth & Security
- Login / Register with role-based access (Admin / Student)
- Forgot Password → OTP via Gmail SMTP → Verify → Reset (OTP stored in DB with 10-minute expiry)
- Session-based authentication with browser cache prevention
- Tab-switch violation detection and logging during exams

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Web Forms, HTML5, CSS3, Font Awesome 6 |
| Backend | C# (.NET Framework 4.7) |
| Database | SQL Server / LocalDB |
| Data Access | ADO.NET (SqlConnection, SqlCommand, SqlDataAdapter) |
| Excel | EPPlus |
| PDF | iTextSharp |
| Email | System.Net.Mail — Gmail SMTP |
| Auth | Session-based |

---

## 🗂️ Project Structure

```
OExm/
│
├── Pages (Admin)
│   ├── AdminDashboard.aspx          # Stats overview + violations table
│   ├── ManageExam.aspx              # Create / edit / delete exams
│   ├── QuestionBank.aspx            # Add questions; create new bank tables
│   ├── BulkUpload.aspx              # Upload .xlsx question files
│   └── DatabaseManager.aspx         # Super-admin: view/edit/delete any table + run SQL
│
├── Pages (Student)
│   ├── Instructions.aspx            # Exam selector + pre-exam details
│   ├── ExamPortal.aspx              # Live exam — navigator, timer, answers
│   └── Result.aspx                  # Score, grade, pass/fail, PDF download
│
├── Pages (Auth)
│   ├── Login.aspx
│   ├── Register.aspx
│   ├── ForgotPassword.aspx          # Request OTP
│   ├── VerifyOtp.aspx               # Enter OTP
│   └── ResetPassword.aspx           # Set new password
│
├── Helpers
│   ├── BankHelper.cs                # All dynamic bank-table operations
│   ├── DatabaseHelpe.cs             # ADO.NET wrapper (ExecuteQuery/Scalar/NonQuery)
│   └── EmailHelper.cs               # SMTP email sender
│
├── Site.Master                      # Shared admin layout and sidebar
└── Web.config                       # Connection string + SMTP settings
```

---

## 🗄️ Database Design

### Core Tables

| Table | Purpose |
|---|---|
| `Users` | Admin and student accounts |
| `Exams` | Exam definitions — marks, dates, mode, bank reference |
| `ExamQuestions` | Stores hand-picked question SlNos for Manual mode exams |
| `StudentExams` | Each student's attempt record (start time, end time, score, status) |
| `StudentResponses` | Each answer saved during the exam (A/B/C/D + status) |
| `ActivityLogs` | User action history |
| `ExamViolations` | Tab-switch violations logged per attempt |
| `PasswordResetOTP` | OTP records with expiry time and used flag |
| `QuestionBanks` | Registry of all banks (name + ID) |

### Dynamic Bank Tables

Every Question Bank you create gets its own physical SQL table:


## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022
- SQL Server or LocalDB
- .NET Framework 4.7+
- Gmail account with an [App Password](https://myaccount.google.com/apppasswords)

### 1. Clone the repository
```bash
git clone https://github.com/yourusername/OExm.git
cd OExm
```

### 2. Create the database

Open **SSMS** or **LocalDB**, create a new database called `OExam`, then run the single script:

```
Database/OExam_Setup.sql
```

This one script creates all tables, inserts default data, seeds 10 question banks, and creates their physical bank tables — the app is ready to use immediately after.

> The script is self-contained. No need to run multiple SQL files in a specific order.

### 3. Configure the connection string

Open `Web.config` and update:

```xml
<connectionStrings>
    <add name="ExamDbConnection"
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OExam;Integrated Security=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

Change `Data Source` and `Initial Catalog` to match your SQL Server instance.

### 4. Configure Gmail SMTP (for OTP password reset)

```xml
<system.net>
    <mailSettings>
        <smtp from="youremail@gmail.com">
            <network
                host="smtp.gmail.com"
                port="587"
                userName="youremail@gmail.com"
                password="your-app-password"
                enableSsl="true" />
        </smtp>
    </mailSettings>
</system.net>
```

> Use a **Gmail App Password**, not your regular password.
> Generate one at: [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)

### 5. Open and run

```
OExm.sln  →  Visual Studio 2022  →  F5
```

---

## 🔑 Default Login

| Field | Value |
|---|---|
| Email | `admin@oexm.com` |
| Password | `admin123` |
| Role | Admin |

> Change this password after first login.

---

## 📊 Bulk Upload Excel Format

Questions can be uploaded via `.xlsx` with exactly these columns in this order:

| SlNo | Question | M1 | M2 | M3 | M4 | Ans |
|---|---|---|---|---|---|---|
| 1 | What is JVM? | Java Virtual Machine | Java Variable Machine | Java Vendor Machine | Java Visual Machine | Java Virtual Machine |
| 2 | What is OOP? | Object Oriented Programming | Object Operator Program | Open Oriented Program | Object Output Process | Object Oriented Programming |

**Rules:**
- `SlNo` is auto-generated — leave it blank or put any number, it's ignored
- `Ans` must be the **exact text** of the correct option (not a letter like A or B)
- Row 1 is treated as a header and skipped automatically
- Blank rows are skipped silently
- Any row with a missing field or an `Ans` that doesn't match M1–M4 is reported and skipped

---

## 🔄 Exam Workflow

```
ADMIN SIDE                          STUDENT SIDE
──────────────────────────────      ─────────────────────────────
 1. Create Question Bank              1. Login
    └─ Physical SQL table created      2. Instructions page
                                           └─ See duration, marks,
 2. Add Questions                             dates, question count
    ├─ Manual entry (one by one)
    └─ Bulk Upload (.xlsx)            3. Start Exam
                                           └─ One attempt only
 3. Create Exam
    ├─ Set name, duration             4. Answer Questions
    ├─ Set dates (start / end)             ├─ Navigate via palette
    ├─ Set marks (passing,            ├─ Mark for Review
    │  positive, negative)            ├─ Clear Response
    ├─ Random mode:                   └─ Save & Next
    │   └─ Pick bank + count
    └─ Manual mode:                   5. Submit / Auto-Submit
        └─ Tick questions                  └─ Redirects to Result

 4. Publish Exam                      6. View Result
    └─ Status = Published                  ├─ Score, percentage, grade
    └─ Students can now see it            ├─ Pass / Fail
                                          └─ Download PDF Certificate
```

---

## 📦 NuGet Packages

```xml
EPPlus                                        <!-- Excel read/write -->
iTextSharp                                    <!-- PDF generation  -->
Portable.BouncyCastle                         <!-- Crypto support  -->
Microsoft.CodeDom.Providers.DotNetCompilerPlatform
```

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Subhadeep Pan**

- GitHub: [@Subhadeep-pan](https://github.com/Subhadeep-pan)
- Email: subhadeeppan047@gmail.com

---

<div align="center">

Made with ❤️ using ASP.NET Web Forms · C# · SQL Server

⭐ Star this repo if it helped you!

</div>
