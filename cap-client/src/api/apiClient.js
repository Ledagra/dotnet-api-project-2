const API_BASE = "http://localhost:5163/api";

// -------------------- SURVEYS --------------------

export async function fetchSurveys() {
  const res = await fetch(`${API_BASE}/Surveys`);
  if (!res.ok) throw new Error("Failed to fetch surveys");
  return await res.json();
}

export async function createSurvey(data) {
  const res = await fetch(`${API_BASE}/Surveys`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to create survey");
  return await res.json();
}

export async function updateSurvey(id, data) {
  const res = await fetch(`${API_BASE}/Surveys/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to update survey");
}

export async function deleteSurvey(id) {
  const res = await fetch(`${API_BASE}/Surveys/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete survey");
}

export async function fetchSurvey(id) {
  const res = await fetch(`${API_BASE}/Surveys/${id}`);
  if (!res.ok) throw new Error("Failed to fetch survey");
  return await res.json();
}

// -------------------- QUESTIONS --------------------

export async function createQuestion(data) {
  const res = await fetch(`${API_BASE}/Questions`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to create question");
  return await res.json();
}

export async function updateQuestion(id, data) {
  const res = await fetch(`${API_BASE}/Questions/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to update question");

  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

export async function deleteQuestion(id) {
  const res = await fetch(`${API_BASE}/Questions/${id}`, {
    method: "DELETE",
  });
  if (!res.ok) throw new Error("Failed to delete question");
}

// -------------------- RESPONSES --------------------

export async function submitResponse(data) {
  const res = await fetch(`${API_BASE}/Responses`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error("Failed to submit response");
  return await res.json();
}

export async function fetchAllResponses() {
  const res = await fetch(`${API_BASE}/Responses`);
  if (!res.ok) throw new Error("Failed to fetch responses");
  return await res.json();
}

