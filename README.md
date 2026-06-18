# OExm - Online Examination System

## Overview

OExm is a full-stack Online Examination System developed using ASP.NET Web Forms, C#, SQL Server, HTML, CSS, JavaScript, and Bootstrap.

The platform provides a secure environment for conducting online examinations with role-based access control, question bank management, automated result generation, anti-cheating mechanisms, PDF report generation, email notifications, and exam analytics.

---

## Features

### Authentication & Authorization

* Secure Login System
* Role-Based Access Control

  * Admin
  * Student
* Session Management
* Activity Logging

### Exam Management

* Create Exams
* Manage Exam Duration
* Assign Questions to Exams
* Multiple Sections Support
* Question Bank Integration

### Question Management

* Create Questions
* Edit Questions
* Delete Questions
* Question Banks
* Section-wise Question Organization
* Positive and Negative Marking
* Bulk Question Upload (Excel)

### Student Examination Portal

* Live Exam Interface
* Question Navigation Palette
* Previous / Next Navigation
* Mark for Review
* Auto Save Answers
* Auto Submit on Time Expiry
* Fullscreen Monitoring

### Anti-Cheating Features

* Tab Switch Detection
* Fullscreen Exit Detection
* Violation Tracking
* Exam Activity Logging

### Result & Analytics

* Automatic Evaluation
* Score Calculation
* Percentage Calculation
* Correct / Wrong Analysis
* Unattempted Questions Analysis
* Section-wise Performance Tracking
* Weak Area Identification

### Reporting

* PDF Result Certificate
* Email Result Notification
* Student Performance Summary

### Administration

* Dashboard Analytics
* User Management
* Database Manager
* Activity Log Monitoring
* Exam Monitoring

---

## Technology Stack

### Frontend

* ASP.NET Web Forms
* HTML5
* CSS3
* JavaScript
* Bootstrap
* Font Awesome

### Backend

* C#
* ASP.NET Framework 4.8

### Database

* Microsoft SQL Server

### Libraries

* EPPlus
* iTextSharp
* BouncyCastle
* System.Net.Mail

---

## Database Design

Main Tables:

* Users
* Exams
* Sections
* QuestionBanks
* Questions
* ExamQuestions
* StudentExams
* StudentResponses
* ActivityLogs
* ExamViolations
* StudentExamAttempts
* SectionAnalysis

---

## Project Architecture

```text
Users
 ├── StudentExams
 │      ├── StudentResponses
 │      ├── ExamViolations
 │      └── SectionAnalysis
 │
 └── ActivityLogs

Exams
 ├── Sections
 │      └── Questions
 │
 └── ExamQuestions
        └── Questions

QuestionBanks
      └── Questions
```

---

## Key Functionalities

### Admin

* Create and Manage Exams
* Create Sections
* Manage Questions
* Manage Question Banks
* Monitor Exam Activities
* View Results
* Upload Questions in Bulk

### Student

* Attend Online Exams
* View Results
* Download PDF Report
* Receive Email Notifications

---

## Security Features

* Password Hashing
* Session-Based Authentication
* Role-Based Authorization
* Activity Tracking
* Anti-Cheating Monitoring
* Secure Database Operations using Parameterized Queries

---

## Future Enhancements

* AI Proctoring
* Webcam Monitoring
* Face Recognition
* Excel Result Export
* REST API Integration
* Mobile Application
* Real-Time Analytics Dashboard

---



---

## Author

**Subhadeep Pan**

Bachelor of Technology (Computer Science & Engineering)

---

## License

This project is developed for educational and academic purposes.
