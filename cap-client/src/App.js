import { useEffect, useState } from "react";
import { createSurvey, fetchSurveys } from "./api/apiClient";
import styles from "./App.module.css";
import AnswerModal from "./components/AnswerModal";
import QuestionModal from "./components/QuestionModal";
import SurveyCard from "./components/SurveyCard";
import ResponsesModal from "./components/ResponsesModal";

function App() {
  const [surveys, setSurveys] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 4;
  const totalPages = Math.ceil(surveys.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentSurveys = surveys.slice(startIndex, startIndex + itemsPerPage);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [loading, setLoading] = useState(true);
  const [activeSurvey, setActiveSurvey] = useState(null);
  const [answerSurvey, setAnswerSurvey] = useState(null);
  const [showResponses, setShowResponses] = useState(false);


  useEffect(() => {
    loadSurveys();
  }, []);

  async function loadSurveys() {
    try {
      setLoading(true);
      const data = await fetchSurveys();
      setSurveys(data);
      setCurrentPage(1);
    } catch (err) {
      console.error("Error loading surveys:", err);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!title.trim() || !description.trim()) return;

    try {
      await createSurvey({ title, description });
      setTitle("");
      setDescription("");
      await loadSurveys();
    } catch (err) {
      console.error("Error creating survey:", err);
    }
  }

  function handleOpenQuestionModal(survey) {
    setActiveSurvey(survey);
  }

  function handleCloseModal() {
    setActiveSurvey(null);
  }

  return (
    <div className={styles.app}>
      <header className={styles.header}>
        <h1 className={styles.title}>Survey Dashboard</h1>
        <p className={styles.subtitle}>Create and manage surveys easily</p>
        <button className={styles.button} onClick={() => setShowResponses(true)}>📊 View Responses</button>
      </header>

      <section className={`${styles.card} ${styles.newSurvey}`}>
        <h2 className={styles.sectionTitle}>Add New Survey</h2>
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Survey Title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className={styles.input}
          />
          <textarea
            placeholder="Survey Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className={styles.input}
            style={{ minHeight: "80px" }}
          />
          <button
            type="submit"
            className={styles.button}
            disabled={!title.trim() || !description.trim()}
          >
            + Add Survey
          </button>
        </form>
      </section>

      <section>
        <h2 className={styles.sectionTitle} style={{ textAlign: "center" }}>
          Existing Surveys
        </h2>

        {loading ? (
          <p className={styles.textCenter}>Loading surveys...</p>
        ) : surveys.length === 0 ? (
          <p className={styles.textCenter}>No surveys found.</p>
        ) : (
          <ul className={styles.surveyList}>
            {currentSurveys.map((s) => (
              <SurveyCard
                key={s.id}
                survey={s}
                onUpdated={loadSurveys}
                onAnswer={(survey) => setAnswerSurvey(survey)}
                onCardClick={(survey) => handleOpenQuestionModal(survey)}
              />
            ))}

            {surveys.length === 0 && (
              <p className={styles.textEmpty}>No surveys found.</p>
            )}
          </ul>
        )}

        {totalPages > 1 && (
          <div className={styles.pagination}>
            <button
              onClick={() => setCurrentPage((p) => Math.max(p - 1, 1))}
              disabled={currentPage === 1}
              className={styles.pageButton}
            >
              ⬅ Prev
            </button>

            <span className={styles.pageInfo}>
              Page {currentPage} of {totalPages}
            </span>

            <button
              onClick={() => setCurrentPage((p) => Math.min(p + 1, totalPages))}
              disabled={currentPage === totalPages}
              className={styles.pageButton}
            >
              Next ➡
            </button>
          </div>
        )}
      </section>

      {activeSurvey && (
        <QuestionModal
          survey={activeSurvey}
          onClose={handleCloseModal}
          onAdded={loadSurveys}
        />
      )}

      {answerSurvey && (
        <AnswerModal
          survey={answerSurvey}
          onClose={() => setAnswerSurvey(null)}
          onAnswered={loadSurveys}
        />
      )}

      {showResponses && <ResponsesModal onClose={() => setShowResponses(false)} />}
    </div>
  );
}

export default App;
