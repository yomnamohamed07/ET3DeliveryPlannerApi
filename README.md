ET3 Delivery Planner

A simple and deterministic delivery trip planner built with ASP.NET Core Web API and Entity Framework Core.

The system organizes deliveries into vehicle trips while respecting the vehicle's maximum capacity, delivery priority, and area grouping requirements.

The solution focuses on being simple, readable, explainable, and easy to maintain, as required by the technical challenge.

---

📌 Project Overview

The application receives a list of deliveries and organizes them into trips.

Each delivery contains:

- Area — destination area
- Priority — lower numbers represent higher urgency
- Package Weight — package weight in kilograms
- ID — generated automatically by the database

The vehicle has a maximum capacity of:

«10 kg per trip»

The planner must ensure that no trip exceeds this limit.

---

🎯 Challenge Requirements

The solution follows the main requirements of the challenge:

- A trip must never exceed 10 kg.
- Lower priority numbers are processed first.
- Deliveries going to the same area should be grouped together where reasonably possible.
- Every valid delivery should appear exactly once in the generated trips.
- Deliveries that cannot be included because of invalid weight are not planned.
- The algorithm should behave consistently and be easy to explain.
- The solution should handle important edge cases.
- The project should remain simple rather than using unnecessary complex algorithms or frameworks.

---

🛠 Technologies

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- REST APIs
- Repository Pattern
- Service Layer
- Custom Exception Middleware

---

🧠 Trip Planning Approach

The planner uses a Greedy Algorithm.

Instead of trying every possible combination of deliveries, the algorithm processes deliveries one by one and chooses the best currently available trip according to a fixed set of rules.

This makes the solution:

- Simple
- Deterministic
- Easy to understand
- Easy to test
- Efficient enough for the scope of the challenge

The algorithm does not guarantee the mathematically minimum possible number of trips. It is a practical heuristic that balances capacity, priority, and area grouping.

---

🚚 Trip Selection Strategy

For every delivery, the planner checks the existing trips in the following order:

1. Same Priority + Same Area

First, try to place the delivery into a trip that:

- Has enough remaining capacity
- Contains deliveries with the same priority
- Contains deliveries going to the same area

This gives the strongest grouping.

---

2. Same Priority

If no suitable trip is found, look for a trip that:

- Has enough capacity
- Already contains a delivery with the same priority

---

3. Same Area

If there is still no suitable trip, look for a trip that:

- Has enough capacity
- Already contains a delivery going to the same area

This keeps deliveries to the same destination together where possible.

---

4. Any Trip With Enough Capacity

If none of the previous conditions can be satisfied, the delivery can be placed in any existing trip that has enough remaining capacity.

This prevents unnecessary trips when capacity is available.

---

5. Create a New Trip

If no existing trip can hold the delivery, a new trip is created.

The new trip starts with the current delivery.

---

📊 Choosing Between Multiple Suitable Trips

If more than one trip satisfies the current rule, the planner chooses the trip with the highest current total weight.

For example:

Trip 1 → 4 kg
Trip 2 → 7 kg
New delivery → 2 kg

Both trips have enough capacity.

The planner chooses:

Trip 2 → 7 + 2 = 9 kg

instead of:

Trip 1 → 4 + 2 = 6 kg

This helps use existing capacity efficiently and reduces fragmented unused space.

---

🔢 Delivery Processing Order

Before creating trips, valid deliveries are ordered by:

1. Priority — lower number first
2. Area — deterministic tie-breaking
3. ID — deterministic ordering when needed

Example:

Priority 1
    ↓
Priority 2
    ↓
Priority 3

This ensures that more urgent deliveries are considered before less urgent ones.

---

🧮 Example

Suppose we have:

ID| Area| Priority| Weight
1| Maadi| 1| 2 kg
2| Maadi| 1| 4 kg
3| Maadi| 2| 3 kg
4| Zamalek| 1| 7 kg

The planner processes deliveries according to priority and deterministic tie-breaking.

Possible result:

Trip 1
---------
Maadi - Priority 1 - 2 kg
Maadi - Priority 1 - 4 kg
Total = 6 kg

Then:

Trip 2
---------
Maadi - Priority 2 - 3 kg
Total = 3 kg

And:

Trip 3
---------
Zamalek - Priority 1 - 7 kg
Total = 7 kg

Every trip remains within the 10 kg limit.

---

⚠️ Edge Cases

The application handles the following cases:

Empty Input

If there are no deliveries:

Trips = 0

No unnecessary trip is created.

---

Package Weight Greater Than 10 kg

A package heavier than the vehicle capacity cannot be delivered in a single trip.

For example:

Package = 12 kg
Vehicle Capacity = 10 kg

The delivery is considered invalid and is not included in trip planning.

When adding deliveries through the API, the request is rejected with a 400 Bad Request.

---

Zero or Negative Weight

Weights such as:

0 kg
-2 kg

are invalid and rejected by the API.

---

Package Does Not Fit in Current Trip

Example:

Current Trip = 8 kg
New Delivery = 4 kg
Capacity = 10 kg

Because:

8 + 4 = 12 kg

the delivery cannot be added to that trip.

The planner continues searching for another suitable trip.

If none exists, it creates a new trip.

---

Multiple Deliveries With the Same Priority

When several deliveries have the same priority, the planner continues using the area and capacity rules to determine where they should be placed.

---

💡 Why Greedy?

A greedy approach was selected because the challenge does not require a mathematically optimal packing algorithm.

The algorithm makes a reasonable local decision for each delivery while respecting the important business rules.

It also has several practical advantages:

- Easy to understand
- Easy to explain during an interview
- Predictable behavior
- No unnecessary complexity
- Good performance compared with trying every possible combination

However, it is important to note that:

«The algorithm does not guarantee the minimum possible number of trips.»

For example, a different ordering or packing strategy could sometimes produce fewer trips.

The challenge allows reasonable decisions as long as the behavior is consistent and the decision is clearly explained.

---

📈 Scalability

For "N" deliveries and "T" trips, the current approach may inspect existing trips for each delivery.

In the worst case, the planning process can approach:

O(N × T)

Since the number of trips can grow with the number of deliveries, the approach can become expensive for very large datasets.

For a dataset containing 1 million deliveries, I would consider:

- Processing data in batches
- Reducing repeated searches through trips
- Using dictionaries/grouping structures for areas and priorities
- Avoiding unnecessary database calls
- Separating data loading from trip calculation
- Profiling the actual bottlenecks before optimizing

The current implementation intentionally favors clarity because the challenge emphasizes understandable and explainable code.

---

🏗️ Project Structure

The project uses a simple layered structure:

ET3DeliveryPlanner
│
├── ET3DeliveryPlanner.Data
│   ├── Entities
│   ├── Repositories
│   ├── Services
│   └── MappingProfiles
│
├── ET3DeliveryPlanner.Infrastructure
│   ├── Data
│   └── Repositories
│
├── ET3DeliveryPlanner.Services
│   ├── Services
│   └── Exceptions
│
└── ET3DeliveryPlannerApi
    ├── Controllers
    ├── Errors
    └── MiddleWares

The goal is to keep responsibilities separated without introducing unnecessary complexity.

---

🗄️ Database

The application uses SQL Server with Entity Framework Core.

The "Delivery" table contains:

Id
Area
Priority
PackageWeight

The "Id" is generated automatically by the database.

Example request:

{
  "area": "Maadi",
  "priority": 1,
  "packageWeight": 4
}

The database generates the ID automatically.

The database also validates the package weight so that invalid values cannot be stored.

---

🔌 API Endpoints

Deliveries

Get all deliveries

GET /api/Deliveries

Get delivery by ID

GET /api/Deliveries/{id}

Add a delivery

POST /api/Deliveries

Example:

{
  "area": "Maadi",
  "priority": 1,
  "packageWeight": 4
}

---

Trips

Generate delivery trips

GET /api/Trips

Returns the generated trips based on the current deliveries.

Get trip summary

GET /api/Trips/summary

The summary contains information such as:

- Total deliveries
- Valid deliveries
- Invalid deliveries
- Total trips
- Total weight
- Average deliveries per trip

---

🛡️ Error Handling

The API uses custom exceptions and centralized exception handling middleware.

Supported cases include:

Status Code| Meaning
400| Invalid request or invalid package weight
404| Delivery was not found
409| Request conflicts with the current data/state
500| Unexpected server error

Instead of writing repetitive "try/catch" blocks inside every controller, exceptions are handled centrally by the middleware.

This keeps controllers cleaner and provides a consistent error response.

---

🔐 Data Validation

Delivery weight is validated before being added.

A valid delivery must satisfy:

0 < PackageWeight ≤ 10

Invalid values are rejected with a clear error message.

The same business rule is also enforced at the database level using a check constraint.

---

🧪 Testing Scenarios

The following scenarios can be used to verify the planner.

1. Same Area

[
  {
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 2
  },
  {
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 4
  }
]

Expected:

Same area
Same priority
Same trip
Total = 6 kg

---

2. Capacity Limit

[
  {
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 6
  },
  {
    "area": "Maadi",
    "priority": 1,
    "packageWeight": 5
  }
]

Expected:

Trip 1 = 6 kg
Trip 2 = 5 kg

The two deliveries cannot share a trip because:

6 + 5 = 11 kg > 10 kg

---

3. Different Priorities

[
  {
    "area": "Maadi",
    "priority": 3,
    "packageWeight": 2
  },
  {
    "area": "Zamalek",
    "priority": 1,
    "packageWeight": 2
  },
  {
    "area": "Nasr City",
    "priority": 2,
    "packageWeight": 2
  }
]

The planner processes:

Priority 1
Priority 2
Priority 3

---

🚀 Running the Project

1. Clone the repository

git clone <repository-url>

2. Configure the database

Update the connection string in:

appsettings.json

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ET3DeliveryPlannerDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}

3. Apply migrations

Using Package Manager Console:

Add-Migration InitialCreate
Update-Database

Or using the .NET CLI:

dotnet ef migrations add InitialCreate
dotnet ef database update

4. Run the API

dotnet run

5. Open Swagger

Swagger can be used to test the API endpoints and inspect the generated responses.

---

🔮 Possible Improvements

The current implementation is intentionally simple.

Possible future improvements include:

Better Packing

Use a more advanced bin-packing strategy if minimizing the number of trips becomes the primary objective.

Improved Area Grouping

Create stronger area-based grouping before packing deliveries into trips.

Large Dataset Optimization

For very large datasets, optimize trip lookups and avoid repeatedly scanning all existing trips.

File Import

Add a dedicated import process for CSV/JSON input files so that large delivery datasets can be loaded directly into the system.

Automated Tests

Add unit tests for:

- Capacity rules
- Priority ordering
- Area grouping
- Invalid deliveries
- Empty input
- New trip creation
- Multiple suitable trips

---

📌 Design Decisions

A few decisions were intentionally made to keep the solution simple and explainable:

- Greedy algorithm instead of complex optimization.
- Lower priority number = higher urgency.
- Same priority + same area is preferred first.
- Existing trips with the highest current weight are preferred when multiple trips are suitable.
- Invalid deliveries are not included in trip planning.
- Delivery IDs are generated by the database.
- Exception handling is centralized in middleware.
- Database validation is used in addition to API validation.

These decisions make the behavior deterministic while keeping the implementation easy to understand and maintain.

---

📝 Conclusion

ET3 Delivery Planner provides a simple solution for organizing deliveries into vehicle trips while respecting:

- 10 kg maximum capacity
- Delivery priority
- Area grouping
- Data validation
- Deterministic behavior

The solution intentionally avoids unnecessary complexity and focuses on producing code that is clear, testable, maintainable, and easy to explain.
