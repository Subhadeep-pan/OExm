# 🎓 OExm — Online Examination System

A full-featured web-based examination platform built with **ASP.NET Web Forms**, **C#**, **SQL Server**, and **ADO.NET**. Designed for educational institutions to manage exams, question banks, and student results — all from a clean admin panel.

---

## 📸 Screenshots

> Login → Instructions → Exam Portal → Result

---

## ✨ Features

### 👨‍💼 Admin Side
- **Exam Builder** — Create exams with name, duration, dates, passing marks, positive/negative marks per question
- **Question Bank** — Each bank (Java, DBMS, SQL etc.) is its own SQL table. Add, edit, delete questions
- **Bulk Upload** — Upload hundreds of questions at once via Excel (`.xlsx`) in `SlNo | Question | M1 | M2 | M3 | M4 | Ans` format
- **Manual & Random Modes** — Pick exact questions by hand, or let the system pull N random questions from a bank
- **Publish Control** — Set exam status (Draft / Published / Closed) and start/end dates
- **Admin Dashboard** — Live stats: total students, exams, average score, violations flagged
- **Exam Violations** — Tab-switch violations logged and shown on the dashboard
- **Database Manager** — Super-admin panel to view, add, delete rows in any table; run custom SQL; create/delete bank tables

### 🧑‍🎓 Student Side
- **Exam Instructions Page** — See exam details (duration, questions, marks, dates) before starting
- **Live Exam Portal** — Question navigator palette, color-coded status (Answered / Not Answered / Review / Not Visited), countdown timer, Mark for Review, Clear Response
- **One Attempt Only** — Students cannot re-attempt a completed exam
- **Auto Submit** — Exam auto-submits when time runs out
- **Result Page** — Instant score, percentage, grade (A+/A/B/C/F), pass/fail, download PDF certificate

### 🔐 Auth & Security
- Login / Register with role-based access (Admin / Student)
- Forgot Password with OTP via email (Gmail SMTP), OTP stored in database with 10-minute expiry
- Session-based authentication with browser cache prevention
- Tab-switch violation detection and logging

---

## 🗂️ Project Structure

```
OExm/
├── AdminDashboard.aspx       # Admin home — stats + violations
├── ManageExam.aspx           # Exam builder (create/edit/delete exams)
├── QuestionBank.aspx         # Add questions to any bank; create new banks
├── BulkUpload.aspx           # Upload Excel file of questions
├── DatabaseManager.aspx      # Super-admin table viewer + SQL runner
├── ExamPortal.aspx           # Live exam page for students
├── Instructions.aspx         # Pre-exam instructions & exam selector
├── Result.aspx               # Score, grade, PDF download
├── Login.aspx                # Login page
├── Register.aspx             # Student registration
├── ForgotPassword.aspx       # Email OTP request
├── VerifyOtp.aspx            # OTP verification
├── ResetPassword.aspx        # New password entry
├── BankHelper.cs             # Shared helper for all bank-table operations
├── DatabaseHelpe.cs          # ADO.NET helper (ExecuteQuery, ExecuteScalar, ExecuteNonQuery)
├── EmailHelper.cs            # SMTP email sender
└── Site.Master               # Shared admin layout/sidebar
```

---

## 🗄️ Database Design

### Core Tables (unchanged)
| Table | Purpose |
|---|---|
| `Users` | Admin and student accounts |
| `Exams` | Exam definitions, marks, dates, mode |
| `StudentExams` | Each student's attempt record |
| `StudentResponses` | Each answer saved per question |
| `ExamQuestions` | Links exams to manually selected questions |
| `ActivityLogs` | User action history |
| `ExamViolations` | Tab-switch violations |
| `PasswordResetOTP` | OTP records with expiry |

### Dynamic Bank Tables
Every Question Bank creates its own physical SQL table:

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022
- SQL Server (LocalDB or full instance)
- .NET Framework 4.7+
- Gmail account with App Password for SMTP

### 1. Clone the repository
```bash
git clone https://github.com/yourusername/OExm.git
cd OExm
```

### 2. Set up the database
Open SSMS and run these scripts in order:

```sql
-- 1. Run the full schema (Users, Exams, StudentExams etc.)
-- (paste the contents of Database.sql)

-- 2. Make SectionId optional
ALTER TABLE Questions ALTER COLUMN SectionId INT NULL;

-- 3. Add per-exam marks columns
ALTER TABLE Exams
ADD PositiveMarksPerQuestion DECIMAL(10,2) NULL,
    NegativeMarksPerQuestion DECIMAL(10,2) NULL;

-- 4. Remove foreign key blocking bank-table SlNos
ALTER TABLE ExamQuestions
DROP CONSTRAINT FK__ExamQuest__Quest__398D8EEE;

-- 5. Create physical tables for each default question bank
-- (run 03_bank_tables_migration.sql)
```


> **Note:** Use a Gmail App Password, not your regular Gmail password. Generate one at [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords).

### 5. Run the project
Open `OExm.sln` in Visual Studio → Press **F5**

### Default Admin Login
```
Email:    admin@oexm.com
Password: admin123
```

---

## 📊 Excel Bulk Upload Format

Questions can be uploaded via `.xlsx` with this exact column order:

| SlNo | Question | M1 | M2 | M3 | M4 | Ans |
|---|---|---|---|---|---|---|
| 1 | What is JVM? | Java Virtual Machine | Java Variable Machine | Java Vendor Machine | Java Visual Machine | Java Virtual Machine |
| 2 | What is OOP? | Object Oriented Programming | Object Operator Program | Open Oriented Program | Object Output Process | Object Oriented Programming |

- **SlNo** is auto-generated and ignored during upload
- **Ans** must exactly match one of M1–M4 (the correct option's full text)
- Row 1 (header) is skipped automatically

---

## 🔄 Exam Workflow

```
Admin                              Student
  │                                   │
  ├─ Create Question Bank             │
  ├─ Add Questions (manual/bulk)      │
  ├─ Create Exam                      │
  │   ├─ Set duration, marks, dates   │
  │   ├─ Random: pick bank + count    │
  │   └─ Manual: tick questions       │
  └─ Publish Exam                     │
                                      ├─ Login
                                      ├─ See Instructions
                                      ├─ Start Exam
                                      ├─ Answer Questions
                                      ├─ Submit / Auto-submit
                                      └─ View Result + PDF
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Web Forms, HTML5, CSS3, Font Awesome |
| Backend | C# (.NET Framework 4.7) |
| Database | SQL Server (ADO.NET — SqlConnection, SqlCommand) |
| Excel | EPPlus |
| PDF | iTextSharp |
| Email | System.Net.Mail (Gmail SMTP) |
| Auth | Session-based |

---

## 📦 NuGet Packages

```
EPPlus
iTextSharp
BouncyCastle
Microsoft.CodeDom.Providers.DotNetCompilerPlatform
```

---

## 🤝 Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

---

## 📄 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**Subhadeep Pan**
- GitHub: [@yourusername](https://github.com/yourusername)

---

<div align="center">
  Made with ❤️ using ASP.NET Web Forms
</div>
