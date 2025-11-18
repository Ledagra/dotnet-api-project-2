import { useEffect, useState } from "react";
import { submitResponse } from "../api/apiClient";
import styles from "../styles/AnswerModal.module.css";

function AnswerModal({ survey, onClose, onAnswered }) {
  const [responses, setResponses] = useState({});
  const [loading, setLoading] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [score, setScore] = useState(null);

  useEffect(() => {
    if (survey?.questions) {
      const init = {};
      survey.questions.forEach((q) => (init[q.id] = []));
      setResponses(init);
    }
  }, [survey]);

  if (!survey) return null;

  function handleSelect(questionId, answerId) {
    setResponses((prev) => ({ ...prev, [questionId]: [answerId] }));
  }

  function handleCheckboxSelect(questionId, answerId, isChecked) {
    setResponses((prev) => ({
      ...prev,
      [questionId]: isChecked ? [answerId] : [],
    }));
  }

  function handleFreeText(questionId, text) {
    setResponses((prev) => ({ ...prev, [questionId]: [text] }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);

    try {
      const selectedIds = Object.values(responses)
        .flat()
        .filter((val) => typeof val === "number");

      const freeTextAnswers = {};
      survey.questions.forEach((q) => {
        if (q.type === 2) {
          freeTextAnswers[q.id] = responses[q.id]?.[0] || "";
        }
      });

      const res = await submitResponse({
        surveyId: survey.id,
        selectedAnswerIds: selectedIds,
        freeTextAnswers,
      });

      if (res?.totalScore !== undefined) setScore(res.totalScore);
      setSubmitted(true);
      onAnswered();
    } catch (err) {
      console.error("Error submitting response:", err);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        {!submitted ? (
          <>
            <h2 className={styles.title}>
              Answer Survey: {survey.title}
            </h2>

            <form onSubmit={handleSubmit}>
              {survey.questions?.length > 0 ? (
                survey.questions.map((q) => (
                  <div key={q.id} className={styles.section}>
                    <strong>{q.text}</strong>
                    <div>
                      {q.type === 0 &&
                        q.answers.map((a) => (
                          <div key={a.id}>
                            <label>
                              <input
                                type="checkbox"
                                checked={responses[q.id]?.includes(a.id) || false}
                                onChange={(e) =>
                                  handleCheckboxSelect(q.id, a.id, e.target.checked)
                                }
                              />{" "}
                              {a.text}
                            </label>
                          </div>
                        ))}

                      {q.type === 1 &&
                        q.answers.map((a) => (
                          <div key={a.id}>
                            <label>
                              <input
                                type="radio"
                                name={`q${q.id}`}
                                value={a.id}
                                checked={responses[q.id]?.includes(a.id)}
                                onChange={() => handleSelect(q.id, a.id)}
                              />{" "}
                              {a.text}
                            </label>
                          </div>
                        ))}

                      {q.type === 2 && (
                        <textarea
                          placeholder="Your answer..."
                          className={styles.input}
                          onChange={(e) =>
                            handleFreeText(q.id, e.target.value)
                          }
                        />
                      )}
                    </div>
                  </div>
                ))
              ) : (
                <p>No questions found for this survey.</p>
              )}

              <div style={{ marginTop: "1.5rem" }}>
                <button
                  type="submit"
                  className={`${styles.button} ${styles.submit}`}
                  disabled={loading || !survey.questions?.length}
                >
                  ✅ Submit
                </button>
                <button
                  type="button"
                  onClick={onClose}
                  className={`${styles.button} ${styles.cancel}`}
                  disabled={loading}
                >
                  ✖ Cancel
                </button>
              </div>
            </form>
          </>
        ) : (
          <div className={styles.center}>
            <h2 style={{ color: "#2ecc71" }}>Survey Completed!</h2>
            {score !== null && (
              <p style={{ fontSize: "1.2rem" }}>
                Your total score: <strong>{score}</strong>
              </p>
            )}
            <button onClick={onClose} className={styles.button}>
              Close
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default AnswerModal;
