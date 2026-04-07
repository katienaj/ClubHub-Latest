# Culver ClubHub Requirements Document

## Hosting & Authentication
- The web app must authenticate users with **Azure AD B2C**.
- The app must be hosted on **Umbriel**.
- The URL must be: **sites.culver.org/clubhub**.

---

## User Roles & Permissions

### **Student**
- View Info tab  
- View Members tab  
- View Attendance tab  
- Click **“I’m Interested”** button  

### **Student Leader**
- View Info page  
- Click **“I’m Interested”**  
- Read/Write Info tab  
- Read/Write Members tab (approve members)  
- Read/Write Attendance tab (create attendance records)  

### **Adult Sponsor**
- View Info page  
- Read/Write Info tab  
- Read/Write Members tab (approve members)  
- Read/Write Attendance tab (create attendance records)  

### **Admin**
- Read/Write admin tables  
- Add/Remove clubs  
- Users come from AD group **ClubHub Admins**  

---

## Admin Pages

### **Manage Club List**
- Add/Remove clubs  
- **Create a Club** form:
  - Name (text, 50 char limit)
  - Club Type (dropdown)
    - Service Clubs  
    - Diversity Clubs  
    - Honor Orgs  
    - Spiritual Life  
    - Competition Clubs  
    - Publication Clubs  
    - Academics  
    - Hobby Clubs  
  - Description (text, 500 char limit)
  - Meeting Day (dropdown)
  - Meeting Times (dropdown)
  - Adult Sponsor (dropdown from `dbo_employees`)
  - Meeting Location (text)
  - Veracross Internal Class ID (int)

### **Manage Club Types**
- Add/remove club types

---

## Home Page
Displays a tile for each club:

- Picture  
- Name  

Filters at the top:

- Meeting Day  
- Club Type  
- Search by Name  

Admin link appears only for Admin users.

Clicking a tile opens the **Club Page**.

---

## Club Page Structure

### **Header**
- Picture  
- Club Name  

### **Tabs**
- **Info**
- **Members**
- **Attendance**

Students can click **“I’m Interested”**.

---

## Info Tab
Editable by Student Leaders, Adult Sponsors, and Admins.  
Students have read‑only access.

Fields:
- Name (50 char limit)
- Club Type (dropdown)
- Description (500 char limit)
- Meeting Day (dropdown)
- Meeting Times (dropdown)
- Adult Sponsor (from `dbo_employees`)
- Meeting Location (text)

---

## Members Tab
Displays list of people:

- Name (Preferred or First + Last)
- Status (Interested, Member, Student Leader)
- Actions:
  - **Remove**
  - **Make Leader**

---

## Attendance Tab

### **Meeting Records**
- Meeting Date  
- Action: **Take Attendance**  
- Attendance Status:
  - Taken  
  - Pending  

### **Add a Meeting**
Form includes:
- Meeting Date  

### **Taking Attendance**
Displays list of members:

- Name  
- Action: **Present** / **Absent**  
- Timestamp  
- Recorded By (logged‑in user)

---

## Phase 2 (Future Features)

### **Email Notifications**
Send email to:
- Hunley  
- Adult Sponsor  
- Student Leader  
When a student clicks **“I’m Interested”**.

### **Posts Tab**
Student Leader / Adult Sponsor can create posts:
- Date  
- Header  
- Text  
- Toggle “Send Email”  
  - If on, email all club members the post content  

---

## Phase 3 (Possible Future Work)
- Quarterly 
