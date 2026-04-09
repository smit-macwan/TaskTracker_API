# TaskManager - Client Overview

TaskManager is a lightweight task management application where users can create and manage their own tasks, and admins can view all tasks.

## What This Demo Shows

- Secure login and registration
- Role-based access (`user`, `admin`)
- Task lifecycle with workflow validation
- Data persistence using SQLite
- Simple web UI for day-to-day task operations

## Core Features

- **Authentication**
  - Register a new user
  - Login with email and password
  - JWT-based session handling
- **Task Management**
  - Create task with title, description, and optional due date
  - View and update own tasks
  - Change status through valid workflow only
- **Admin View**
  - Admin can view all users' tasks

## Business Rules Enforced

- Only the owner (or admin) can access a task.
- Allowed transitions:
  - `ToDo -> InProgress`
  - `InProgress -> Done`
- Invalid status transitions are rejected.
- A task cannot be marked `Done` if description is empty.
- Due date cannot be in the past when creating a task.

## Quick Demo Flow (5-8 minutes)

1. Register a normal user and login.
2. Create a task and show it appears in "My tasks".
3. Move task from `ToDo` to `InProgress`, then to `Done`.
4. Show validation by trying to mark a task done with empty description.
5. Login as admin and open "All tasks" to show cross-user visibility.

## Default Demo Admin

- Email: `admin@local`
- Password: `Admin123!`

## Technology Snapshot

- Backend: ASP.NET Core Web API
- Frontend: React
- Database: SQLite
- Security: JWT authentication and role-based authorization
