# Hotel Booking Platform - Graduation Project

Welcome to the **Hotel Booking Platform**!  
This is a full-stack graduation project that provides a modern hotel booking system, including user authentication, hotel/room management, personalized recommendations, and more.

---

## 📂 Project Structure
```
├── BackEnd/
│   └── AuthJWT/                # ASP.NET Core Web API (main backend)
│       ├── Controllers/
│       ├── Domain/
│       ├── Services/
│       ├── Infrastructure/
│       ├── Program.cs
│       ├── appsettings.json
│       └── ...
└── FrontEnd/
    └── bookingapp/            # Next.js (React) Frontend
        ├── app/
        ├── components/
        ├── Services/
        ├── public/
        └── ...
```

---

## 🚀 Features

- **User Authentication** (JWT, OAuth)
- **Hotel & Room Management** (CRUD, images, amenities)
- **Booking System** (search, filter, book, cancel)
- **Personalized Recommendation System** (vector-based, cosine similarity)
- **Email Notification** (Kafka event-driven, background email sender)
- **Admin Panel** (manage hotels, rooms, users)
- **Responsive Frontend** (Next.js, TailwindCSS)
- **Caching** (Redis, in-memory)
- **API Documentation** (Swagger/OpenAPI)

---

## 🛠️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18+)](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Redis](https://redis.io/) (for caching)
- [Kafka](https://kafka.apache.org/) (for email event queue)
- [Docker](https://www.docker.com/) (optional, for running dependencies)

---

## ⚙️ Backend Setup (`BackEnd/AuthJWT`)

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/hotel-booking-platform.git
   cd hotel-booking-platform/BackEnd/AuthJWT
   ```

2. **Configure the database and services:**
   - Update appsettings.json with your SQL Server, Redis, Kafka, and Mail settings.

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the backend:**
   ```bash
   dotnet run
   ```
   The API will be available at https://localhost:5001 (or as configured).

5. **API Documentation:**
   - Visit https://localhost:5001/swagger for Swagger UI.

---

## 💻 Frontend Setup (`FrontEnd/bookingapp`)

1. **Install dependencies:**
   ```bash
   cd ../../FrontEnd/bookingapp
   npm install
   ```

2. **Configure environment variables:**
   - Copy .env.example to .env and update API URLs as needed.

3. **Run the frontend:**
   ```bash
   npm run dev
   ```
   The app will be available at http://localhost:3000.

---

## 📧 Email Sending via Kafka

When a booking or important event occurs, the backend publishes an email event to Kafka.

- A background service consumes the Kafka topic and sends emails using the configured SMTP server.
- Configure Kafka and Mail settings in appsettings.json.

**Example Kafka Email Event Flow:**
1. User books a room.
2. Backend publishes an EmailEvent to Kafka topic (e.g., email-events).
3. Background service (Kafka consumer) listens to the topic and sends the email.

---

## 🧠 Recommendation System

The backend uses vector encoding and cosine similarity to recommend hotels/rooms based on user booking history.
- Features include price, room type, amenities, rating, and more.
- See `Services/Implements/RecommendationService.cs` for details.

---

## 📝 Additional Notes

- **Seeding Data:** The backend seeds initial data for hotels, rooms, amenities, etc. on first run.
- **Caching:** Room/hotel data is cached in Redis for performance.
- **Security:** JWT authentication is required for most API endpoints.
- **Extensibility:** The system is modular and can be extended for new features (payment, reviews, etc.).
