ET3 Delivery Planner

A simple and maintainable ASP.NET Core Web API for planning delivery trips based on delivery priority, destination area, and vehicle capacity.

The main goal of the solution is to organize deliveries into trips while ensuring that no trip exceeds the maximum vehicle capacity of 10 kg.

The solution uses a greedy approach with clearly defined preferences for selecting an existing trip.

---

1. Problem Overview

Each delivery contains:

- "Id"
- "Area"
- "Priority"
- "PackageWeight"

The vehicle has a maximum capacity of:

10 kg per trip

The planner should:

- Process more urgent deliveries first.
- Prefer grouping deliveries according to priority and area.
- Never exceed the 10 kg trip capacity.
- Assign every valid delivery exactly once.
- Create a new trip when no existing trip can accommodate the delivery.

---

2. Main Business Rules

The solution follows these rules:

Vehicle Capacity

A trip must never exceed:

10 kg

A delivery with a weight greater than 10 kg cannot be assigned to a trip.

Priority

Lower priority numbers represent more urgent deliveries.

Therefore, deliveries are processed in ascending priority order:

Priority 1
Priority 2
Priority 3
...

The delivery "Id" is used as a secondary ordering criterion to make the processing order deterministic.

Area

Deliveries going to the same area are preferred to be grouped together where possible.

Trip Selection

When processing a delivery, the planner searches for a suitable trip using the following order of preference:

1. Same Priority + Same Area + Enough Capacity
2. Same Priority + Enough Capacity
3. Same Area + Enough Capacity
4. Any Trip + Enough Capacity
5. Create a New Trip

This ordering is a deliberate design decision used to balance priority, area grouping, and capacity utilization.

---

3. Algorithm

The application uses a greedy algorithm.

For each delivery, the planner makes the best available decision based on the current trips instead of trying every possible combination.

High-Level Flow

Get all deliveries
        ↓
Filter invalid deliveries
        ↓
Sort by Priority
        ↓
Process deliveries one by one
        ↓
Same Priority + Same Area?
        ↓
       Yes → Use Trip
        ↓ No
Same Priority?
        ↓
       Yes → Use Trip
        ↓ No
Same Area?
        ↓
       Yes → Use Trip
        ↓ No
Any Trip with enough capacity?
        ↓
       Yes → Use Trip
        ↓ No
Create New Trip

---

4. Detailed Trip Selection Strategy

For every delivery, the planner checks the following conditions in order.

4.1 Same Priority + Same Area

This is the first preference.

The planner looks for an existing trip that:

- Has enough remaining capacity.
- Contains a delivery with the same priority.
- Contains a delivery from the same area.

Example:

Existing Trip:
Area = Maadi
Priority = 1
Weight = 6 kg

New Delivery:
Area = Maadi
Priority = 1
Weight = 3 kg

Since:

6 + 3 = 9 kg

the delivery is added to the existing trip.

---

4.2 Same Priority

If no trip satisfies both priority and area, the planner looks for a trip containing a delivery with the same priority and enough capacity.

Example:

Trip 1:
Priority = 1
Weight = 6 kg

New Delivery:
Priority = 1
Weight = 3 kg

The delivery can be added:

6 + 3 = 9 kg

---

4.3 Same Area

If no same-priority trip is available, the planner looks for a trip serving the same area with enough capacity.

Example:

Trip 1:
Area = Maadi
Weight = 5 kg

New Delivery:
Area = Maadi
Weight = 4 kg
Priority = 3

Even if the priorities are different, the delivery can be grouped with the same area:

5 + 4 = 9 kg

---

4.4 Any Trip With Enough Capacity

If there is no suitable trip matching priority or area, the planner reuses any existing trip that can accommodate the delivery.

This avoids creating a new trip unnecessarily when available capacity already exists.

---

4.5 Create a New Trip

If no existing trip can accommodate the delivery, a new trip is created.

Example:

Existing Trip:
Weight = 8 kg

New Delivery:
Weight = 4 kg

Since:

8 + 4 = 12 kg

the delivery cannot be added to that trip.

If no other trip can accommodate it, a new trip is created.

---

5. Choosing Between Multiple Suitable Trips

If more than one trip satisfies the current rule, the planner selects the trip with the highest current total weight.

For example:

Trip 1 = 5 kg
Trip 2 = 7 kg
New Delivery = 2 kg

Both trips can accommodate the delivery.

The planner chooses:

Trip 2

7 + 2 = 9 kg

This helps use existing capacity more efficiently and reduces fragmented unused space.

---

6. Example

Input

[
  {
    "id": 1,
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 2
  },
  {
    "id": 2,
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 4
  },
  {
    "id": 3,
    "area": "Maadi",
    "priority": 2,
    "packageWeight": 3
  },
  {
    "id": 4,
    "area": "Zamalek",
    "priority": 1,
    "packageWeight": 7
  }
]

Processing Order

The deliveries are processed according to priority:

ID 1 → Priority 1
ID 2 → Priority 1
ID 4 → Priority 1
ID 3 → Priority 2

Processing

Delivery 1

No trips exist.

Trip 1 = 2 kg

Delivery 2

Same priority + same area exists.

Trip 1 = 2 + 4 = 6 kg

Delivery 4

No suitable Maadi/Priority 1 trip has enough capacity:

6 + 7 = 13 kg

A new trip is created.

Trip 2 = 7 kg

Delivery 3

Trip 1 has:

Area = Maadi
Priority = 1
Weight = 6 kg

The delivery has:

Area = Maadi
Priority = 2
Weight = 3 kg

The first two conditions do not match because the priority is different.

The third condition matches:

Same Area + Enough Capacity

Therefore:

Trip 1 = 6 + 3 = 9 kg

Final Result

Trip 1
---------
ID 1 → 2 kg
ID 2 → 4 kg
ID 3 → 3 kg

Total = 9 kg


Trip 2
---------
ID 4 → 7 kg

Total = 7 kg

Every valid delivery is assigned exactly once, and no trip exceeds 10 kg.

---

7. Validation and Edge Cases

The planner handles the following cases.

Empty Delivery List

Input:

[]

Result:

[]

No trip is created.

---

Package Weight Greater Than 10 kg

Example:

{
  "id": 5,
  "area": "Nasr City",
  "priority": 1,
  "packageWeight": 12
}

This delivery cannot be transported by a vehicle with a 10 kg maximum capacity.

It is excluded from trip planning.

---

Zero or Negative Weight

Examples:

0 kg
-2 kg

These values are considered invalid and are excluded from planning.

---

Multiple Deliveries With the Same Priority

When multiple deliveries have the same priority, the "Id" is used as a secondary sorting criterion.

This provides deterministic processing.

---

Delivery Does Not Fit

If:

Current Trip = 8 kg
New Delivery = 3 kg

then:

8 + 3 = 11 kg

The delivery cannot be added to that trip.

The planner continues searching for another suitable trip.

If no suitable trip exists, a new trip is created.

---

8. Why Greedy?

The problem can be viewed as a constrained packing problem because deliveries need to be placed into trips with a maximum capacity.

A fully optimized solution could try to find a globally optimal distribution of deliveries.

However, the challenge also contains other requirements:

- Priority order.
- Same-area grouping.
- Capacity constraints.
- Reasonable and explainable decisions.

Therefore, a greedy strategy was selected.

The algorithm makes a decision for each delivery based on the best currently available option.

Advantages

- Simple to understand.
- Easy to trace and debug.
- Deterministic.
- Respects the 10 kg capacity constraint.
- Considers priority.
- Attempts to group deliveries intelligently.
- Easy to extend.

Limitation

The greedy approach does not guarantee the globally minimum number of trips.

An earlier decision can sometimes leave unused capacity that cannot be efficiently used by later deliveries.

Finding the globally optimal arrangement would require a more complex optimization strategy.

For this challenge, the greedy approach provides a reasonable balance between correctness, simplicity, and explainability.

---

9. Complexity and Scalability

The current implementation keeps the deliveries and generated trips in memory.

For each delivery, the planner may scan the existing trips multiple times.

Therefore, as the number of deliveries and trips grows, the number of comparisons can also grow.

For a very large dataset such as 1 million deliveries, the current implementation would need optimization.

Possible improvements include:

- Processing deliveries in batches.
- Avoiding loading the entire dataset into memory.
- Maintaining trips grouped by area.
- Using more efficient lookup structures.
- Reducing repeated scans of the trip list.
- Moving filtering and sorting operations to the database when appropriate.
- Using a more advanced packing/optimization strategy if minimizing the number of trips becomes a strict requirement.

The current implementation intentionally prioritizes clarity and maintainability.

---

10. Architecture

The application uses a simple layered structure.

Controller
    ↓
Service
    ↓
Repository
    ↓
Entity Framework Core
    ↓
SQL Server

Controller

Responsible for handling HTTP requests and returning HTTP responses.

Service

Contains the business logic, including the trip planning algorithm.

Repository

Responsible for accessing delivery data.

Infrastructure

Contains the EF Core "DbContext", database configuration, and repository implementations.

This separation keeps the trip planning logic independent from the API and database access code.

---

11. Error Handling

The API uses global exception handling middleware.

Custom exceptions include:

BadRequestException
NotFoundException
ConflictException

The middleware converts these exceptions into consistent HTTP responses.

Example:

{
  "statusCode": 404,
  "message": "Delivery with ID 10 was not found."
}

This avoids repeating "try/catch" blocks across controllers.

---

12. Database

The application uses:

- SQL Server
- Entity Framework Core
- EF Core Migrations

The "Delivery" entity is configured using "IEntityTypeConfiguration<Delivery>".

Important constraints include:

Area → Required
Priority → Required
PackageWeight → Required
PackageWeight → Greater than 0
PackageWeight → Less than or equal to 10

---

13. API Endpoints

Deliveries

Get all deliveries

GET /api/Deliveries

Get delivery by ID

GET /api/Deliveries/{id}

Add delivery

POST /api/Deliveries

Example:

{
  "id": 1,
  "area": "Maadi",
  "priority": 1,
  "packageWeight": 4.5
}

---

Trips

Generate delivery trips

GET /api/Trips

This endpoint runs the trip planning algorithm and returns the generated trips.

Get trip summary

GET /api/Trips/summary

The summary endpoint provides additional information such as:

- Total deliveries.
- Valid deliveries.
- Invalid deliveries.
- Total trips.
- Total weight.
- Average deliveries per trip.

The summary endpoint is an additional feature implemented beyond the core trip-planning logic.

---

14. Input Data

The challenge requires the solution to work with delivery input data.

The project can use a JSON input file for test data, for example:

[
  {
    "id": 1,
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 2
  },
  {
    "id": 2,
    "area": "Zamalek",
    "priority": 2,
    "packageWeight": 5
  }
]

The input data can then be loaded into the application/database before generating trips.

---

15. Running the Project

Clone the repository

git clone <repository-url>

Configure the database

Update the connection string in:

appsettings.json

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ET3DeliveryPlannerDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

Apply migrations

Using Package Manager Console:

Update-Database

Or using the .NET CLI:

dotnet ef database update

Run the application

dotnet run

The API can then be tested through Swagger.

---

16. Testing the Algorithm

The most important scenarios to test are:

Scenario| Expected behavior
Empty input| No trips
Same priority + same area| Prefer the matching trip
Same priority| Prefer a matching-priority trip
Same area| Prefer a matching-area trip
Different priority and area| Use another available trip
Package does not fit| Try another trip
No trip can fit| Create a new trip
Weight > 10 kg| Ignore/reject invalid delivery
Weight <= 0| Ignore/reject invalid delivery
Multiple equal priorities| Process deterministically by ID

---

17. Future Improvements

Possible future improvements include:

- Unit tests for the trip planning rules.
- Integration tests for API endpoints.
- Pagination for delivery listing.
- Batch processing for large datasets.
- More efficient trip lookup.
- Persistent trip storage if trips need to be saved.
- More advanced optimization if minimum trip count becomes a strict business requirement.
- A dedicated input-file import service.

---

18. Summary

The project implements a deterministic greedy delivery planner based on five levels of preference:

Same Priority + Same Area
            ↓
Same Priority
            ↓
Same Area
            ↓
Any Available Trip
            ↓
New Trip

while always enforcing:

Trip Weight <= 10 kg

The solution intentionally favors clarity, predictable behavior, and explainability over a complex global optimization algorithm.

The main business logic is contained in the trip planning service, making it straightforward to understand, test, and extend.
