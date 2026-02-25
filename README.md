# Mini Dating App Prototype

This project is a simplified dating application built as part of the Web Developer Intern technical test for Clique83.

The goal of this project is to demonstrate the ability to design and implement a complete feature end-to-end, including data modeling, business logic, and user interaction.

---

## Live Demo

[https://mini-dating-app.onrender.com]

---

## Tech Stack

* .NET 9
* Blazor Server
* Entity Framework Core (Code First)
* SQLite

---

## Data Storage

The application uses SQLite as a lightweight database.

All data is persisted using Entity Framework Core, including:

* Profiles
* Likes
* Matches
* Availability

The database is automatically created and migrated on application startup.

---

## System Architecture

The project is structured into clear and maintainable layers:

### Models

* Profile: Stores user information (Name, Age, Gender, Bio, Email)
* Like: Represents a one-way like from one user to another
* Match: Represents a mutual relationship between two users
* Availability: Stores a user's available time range (linked to a Match)

### Data

* AppDbContext: Handles database access using Entity Framework Core

### Services

ProfileService:

* Handles profile creation
* Validates input
* Prevents duplicate emails

MatchService:

* Handles like logic
* Detects mutual likes and creates matches
* Prevents duplicate likes and matches
* Stores availability per match
* Finds overlapping time slots

### Pages (UI)

* CreateProfile: Create a new profile
* Profiles: View profiles and like users
* Matches: View matches and availability status
* AvailabilityPage: Select availability and find overlapping time

---

## User Handling

Authentication is not implemented to keep the scope focused on core logic.

Instead, a "current user" is selected from the profile list. This allows testing of like, match, and availability features without requiring a login system.

---

## Match Logic

A match is created when two users like each other (mutual like).

### Flow

1. User A likes User B → a Like record is created
2. The system checks if User B has liked User A
3. If both conditions are true → a Match is created

### Key Points

* Users cannot like themselves
* Duplicate likes are prevented
* The system checks for existing matches before creating a new one
* Each pair of users can only have one match

This ensures data consistency and prevents duplicate relationships.

---

## Availability and Overlap Logic

After a match is created, both users can select their availability.

### Design Decision

Availability is linked to a specific Match instead of being global per user.

This ensures:

* Availability is scoped per relationship
* Different matches can have different schedules
* The system is flexible for future features

---

### Rules

* Users select a time range (Start → End)
* Time must be within the next 3 weeks
* End time must be after start time
* Minimum duration is 15 minutes

---

### Overlap Logic

The system finds the first overlapping time slot between two users.

All availability entries are sorted by start time. The system compares each pair of time slots and returns the earliest valid overlap.

```
start = max(userA.start, userB.start)
end   = min(userA.end, userB.end)

if (start < end) → overlap exists
```

### Reasoning

* Ensures the earliest possible meeting time
* Keeps the logic simple and efficient for small datasets
* Easy to understand and debug

---

### Result

* If a common slot is found:
  → "You have a date at [date time]"

* If no overlap exists:
  → "No common time found"

* If only one user has availability:
  → "Waiting for the other user"

---

### Additional Notes

* Time is stored in UTC and converted to local time for display
* Only future time slots are considered
* Multiple availability slots per user are supported

---

## Key Implementation Details

* Validation using DataAnnotations and custom logic
* Email uniqueness enforced at database level
* Business logic separated into services
* UI state managed to ensure consistency

### Edge Cases

* Prevent self-like
* Prevent duplicate likes and matches
* Validate time ranges (start < end)
* Ensure availability is within 3 weeks
* Handle missing availability from one user
* Handle no overlapping time

---

## Design Decisions

* Use Guid for user identification
* Use match-based availability
* Store time in UTC to avoid timezone issues
* Separate business logic into services

These decisions prioritize simplicity, correctness, and maintainability.

---

## Future Improvements

* Prevent overlapping availability within the same user
* Optimize overlap detection (e.g., two-pointer algorithm)
* Add authentication
* Improve UI/UX
* Add database constraints for Like and Match
* Use transactions for concurrency handling

---

## Feature Proposals

1. Chat system between matched users
2. Location-based matching
3. Smart time suggestions

---

## How to Run

```bash
git clone https://github.com/TTTam0711/MiniDatingApp.git
cd MiniDatingApp
dotnet run
```

Open:

```
https://localhost:7065/
```

---

## Summary

This project demonstrates:

* Implementation of core dating app logic
* Data consistency handling
* Clear separation of concerns
* End-to-end feature delivery

The focus is on correctness, clarity, and maintainability.
