# Backend-capstone-booking-api
Practicing C# focused on the backend of a booking app.

This repository follows a simple layered architecture under `src/`:

## Project structure

### Booking.Api
This is the ASP.NET Core Web API entry point. It contains the application startup, dependency injection setup, controller endpoints, and HTTP configuration. In this project, the API exposes the booking and resource endpoints, maps the health check route, and wires the `BookingApplication` contracts to the infrastructure implementations.

### Booking.Application
This layer contains the application logic and contracts that define what the system can do without depending on the concrete database or transport implementation. It includes the query interfaces used by the API and represents the business operations the application supports.

### Booking.Domain
This is the core business model of the application. It contains the domain entities such as `Resource`, `User`, and `BookingDetails`, which represent the main concepts of the booking domain. This layer should stay free from database and HTTP concerns.

### Booking.Infrastructure
This layer implements the application’s technical concerns, especially persistence and data access. It contains the EF Core context, database-related query implementations, and infrastructure code that connects the application layer to PostgreSQL or other external systems.

Local database with Docker Compose
----------------------------------

The root-level `docker-compose.yml` starts a PostgreSQL database for local
development. It does not start the .NET API; run the API separately.

### Start PostgreSQL

1. Install and start Docker Desktop (or another Docker Engine with the Compose
   plugin).
2. Open a terminal in the repository root, next to `docker-compose.yml`.
3. Start the database in the background:

   ```powershell
   docker compose up -d
   ```

4. Check that the container is running and healthy:

   ```powershell
   docker compose ps
   docker compose logs postgres
   ```

The database is available on your computer at `localhost:5432`. The default
development settings are database `bookingdb`, user `booking`, and password
`booking_dev_password`. For an API running directly on your computer, a
PostgreSQL connection string would be:

```text
Host=localhost;Port=5432;Database=bookingdb;Username=booking;Password=booking_dev_password
```

If you later run the API as another service in this Compose project, use
`postgres` as the host instead of `localhost`; Compose services can reach each
other by service name. The current project does not yet configure a database
provider or apply database migrations, so starting this container alone does
not connect the API to PostgreSQL or create application tables.

### Stop and retain the database

```powershell
docker compose down
```

This removes the container and network but keeps the named `postgres-data`
volume, so the database data is still there the next time you run
`docker compose up -d`.

To permanently delete the local database data as well, use
`docker compose down -v`. This is destructive: PostgreSQL will start empty
next time.

### Configuration and adding Redis later

The values in `docker-compose.yml` are convenient local-development defaults,
not production credentials. Compose also reads a `.env` file in the repository
root. To override the PostgreSQL settings locally, create `.env` (it is ignored
by Git) with, for example:

```dotenv
POSTGRES_DB=bookingdb
POSTGRES_USER=booking
POSTGRES_PASSWORD=choose_a_local_password
```

When the application needs Redis, add a `redis` service under `services` in
`docker-compose.yml`. For example:

```yaml
  redis:
    image: redis:7-alpine
    restart: unless-stopped
    ports:
      - "6379:6379"
```

Then run `docker compose up -d` again to start the added service. An API
container in the same Compose project would connect to Redis using hostname
`redis` and port `6379`.


API Contract
   - `GET /resources`
   - `GET /resources/{id}/availability`
   - `POST /resources`

### Getting an access token

The API seeds the Admin account from `Admin:Email` and `Admin:Password`.
Configure those values and a strong JWT signing key in the ignored repository
root `.env` file:

```dotenv
Admin__Email=admin@example.com
Admin__Password=replace-with-a-strong-admin-password
Jwt__SigningKey=replace-with-a-random-secret-at-least-32-bytes-long
```

The JWT issuer, audience, and token lifetime have defaults in `appsettings.json`;
override them with `Jwt__Issuer`, `Jwt__Audience`, and
`Jwt__AccessTokenMinutes` as appropriate for your deployment. Never commit the
signing key or production credentials.

Request a token with the seeded credentials:

```http
POST https://localhost:7214/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "replace-with-a-strong-admin-password"
}
```

Use the returned `accessToken` on protected requests:

```http
GET https://localhost:7214/bookings/confirmed
Authorization: Bearer <accessToken>
```

   - `POST /bookings`
   - `DELETE /bookings/{id}`

   - `GET /Users/{id}/bookings`
   - `POST /Users`
   - `DELETE /Users/{id}`