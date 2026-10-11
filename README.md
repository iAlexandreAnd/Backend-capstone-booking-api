# Backend-capstone-booking-api
Practicing C# focused on the backend of a bookin app


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

   - `POST /bookings`
   - `DELETE /bookings/{id}`

   - `GET /Users/{id}/bookings`
   - `POST /Users`
   - `DELETE /Users/{id}`