# Survey Project  
*A .NET 8 Web API + React Project*

---

![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![React](https://img.shields.io/badge/React-19-blue)
![EF Core](https://img.shields.io/badge/EF%20Core-InMemory-green)
![Swagger](https://img.shields.io/badge/Docs-Swagger-yellow)
![Tests](https://img.shields.io/badge/Tests-xUnit-brightgreen)

---

## 📁 Table of Contents

- [Running & Testing the API](#running--testing-the-api)
- [Architecture](#architecture)
  - [Technology Stack](#technology-stack)
  - [API Design](#api-design)
  - [Data Modeling](#data-modeling)
  - [Communication Layer](#communication-layer)
- [Scoring](#scoring)
- [Assumptions](#assumptions)
- [Unit Tests](#unit-tests)
- [Frontend Overview](#frontend-overview)

---

## 🚀 Running & Testing the API

```bash
cd CAPApi
dotnet restore
dotnet run
```

The API will run at:
```bash
http://localhost:5163
```

Open Swagger UI:
```bash
http://localhost:5163/swagger
```

🏗️ Architecture
<details> <summary><strong>Click to expand Architecture details</strong></summary>
Technology Stack

Backend: .NET 8 Web API

Frontend: React

Data Layer: EF Core (InMemory)

Documentation: Swagger / Swashbuckle

API Design

RESTful CRUD endpoints

Controllers:

SurveysController

QuestionsController

ResponsesController

Features:

Swagger annotations

Dependency injection

Consistent HTTP responses

Data Modeling

Core Entities:

Survey – contains multiple questions

Question – includes a QuestionType enum and list of answers

Answer – includes display text and weight

Response – includes selected answers, free text, survey title, and calculated score

Communication Layer

The React client uses a centralized module (apiClient.js) for:

All API calls

Centralizing endpoint definitions

Simplifying maintenance

Allowing backend changes without UI rewrites

</details>

🧮 Scoring
<details> <summary><strong>Click to expand Scoring details</strong></summary>

Each answer has a weight

Final score = sum of all answered weights

Unanswered questions = 0 weight

Free-text questions:

Full weight if text is entered

Zero if blank

Scoring performed server-side to avoid client tampering

</details>
