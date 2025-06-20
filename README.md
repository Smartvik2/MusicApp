# 🎶 Music App

A modern, full-featured music collaboration and booking platform built with **ASP.NET Core Web API** and **SQL Server**, designed to connect users with talented artists. It supports **role-based access control** for **Admins**, **Artists**, and **Regular Users**, offering a seamless experience across music discovery, booking, communication, and payments.

---

## 🚀 Core Features

- 🔐 **Authentication & Role Management**  
  Secure login, registration, and JWT-based authentication system with support for **Admin**, **Artist**, and **User** roles.
- Users can apply to become an artist which can only be approved by Admins.

- 🧑‍🎤 **Artist Management System**  
  Artists can create and update profiles with personal bios, genres, experience, availability, and media-rich portfolios, and so-on.

- 🗂️ **Artist Dashboard**  
  Personalized dashboard for artists to manage bookings, edit portfolios, update availability, track reviews, delete account, Manage their appointment history and Payment, and more.

- 📅 **Appointment Booking**  
  Users can view artist availability and book appointments. Artists can accept, reject, or reschedule bookings, with saved history.

- 🎨 **Portfolios**  
  Artists showcase their work through audio clips, videos, and achievements, helping users choose the right match.

- 💬 **In-App Real-Time Chat** *(via SignalR)*  
  Enables seamless communication between users and artists — perfect for collaboration or planning sessions.

- 💵 **Payments via Stripe**  
  Secure Stripe integration for booking payments, with support for payment history, invoices, and receipts.

- ⭐ **Reviews & Ratings**  
  Users can leave feedback for artists after sessions, helping others make informed decisions.

- 🔔 **In-App Notification System**  
  Real-time alerts for important events — bookings, messages, payment confirmations, and more.

- 📊 **Admin Dashboard**  
  Central hub for platform moderation and analytics:  
  - Manage users and artists  
  - Approve/reject requests  
  - Assign or revoke admin privileges
  - Delete Users  
  - View platform usage insights

- 🔍 **Smart Search & Filtering**  
  Find artists easily by **name**, **genre**, **rating**, **availability**, or **experience** — with intuitive filters.

---

## 🧱 Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 8)  
- **Database:** SQL Server  
- **Real-time Communication:** SignalR  
- **Payments:** Stripe API  
- **Authentication:** JWT (JSON Web Tokens)  
- **Frontend:** React or .NET MAUI *(planned)*

---
