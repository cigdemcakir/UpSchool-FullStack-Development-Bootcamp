   # CQRS in .NET

CQRS (Command Query Responsibility Segregation) is an architectural pattern that separates the responsibility of handling commands and queries in a system. This separation allows for more efficient scaling and optimization of each responsibility, as well as increased flexibility in the design and implementation of the system.

![](https://user-images.githubusercontent.com/102484836/231033571-585b89cc-4e0c-4b57-bb97-6475efef3f70.png)

---

CQRS allows an application to work with different models, One model that has data needed to update a record, another model to insert a record, yet another to query a record. This gives us flexibility with varying and complex scenarios. You don’t have to rely on just one DTO for the entire CRUD Operations by implementing CQRS.

<p align="center">
  <img src="https://user-images.githubusercontent.com/102484836/231038336-23243102-9e75-4a8a-b93f-91c91135b708.png" />
</p>

---

### Implementation in .NET
CQRS is commonly implemented in .NET using a combination of patterns and technologies, including:

- Command Handlers and Query Handlers,
- MediatR,
- Domain Entities and Domain Services,
- Event Sourcing

Command Handlers and Query Handlers are responsible for handling incoming commands and queries, respectively. MediatR is a library that provides an implementation of the mediator pattern, making it easy to register and resolve handlers. Domain Entities and Domain Services represent the business logic of the system, while Event Sourcing is a pattern for persisting the state of the system as a sequence of events.

---
__With this repo, the following statements were made by me;__

👩‍💻 Configured the fields in our "Address", "Note", "NoteCategory" entities correctly for the database.

👩‍💻 Configured the relation between User and Address. One User can have many Addresses (one-to-many)

👩‍💻 Configured the relation between Note and Category. One Note can have many Category and One Category can have many Note.

👩‍💻 Created Add, Update, Delete and HardDelete Commands For "Address", in the "CQRS" structure.

👩‍💻 Created GetById and GetAll Query for "Address", in "CQRS" structure.
