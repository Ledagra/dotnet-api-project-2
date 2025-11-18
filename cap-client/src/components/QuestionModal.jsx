import { useEffect, useState } from "react";
import { createQuestion, deleteQuestion, updateQuestion, fetchSurvey } from "../api/apiClient";
import styles from "../styles/QuestionModal.module.css";

function QuestionModal({ survey, onClose, onAdded }) {
  const [text, setText] = useState("");
  const [type, setType] = useState("0");
  const [answers, setAnswers] = useState([{ text: "", weight: 0 }]);
  const [loading, setLoading] = useState(false);
  const [questions, setQuestions] = useState([]);
  const [editingId, setEditingId] = useState(null);

  useEffect(() => {
    if (survey?.questions) {
      setQuestions(survey.questions);
    }
  }, [survey]);

  function handleAddAnswer() {
    setAnswers([...answers, { text: "", weight: 0 }]);
  }

  function handleAnswerChange(index, field, value) {
    const updated = [...answers];
    updated[index][field] = value;
    setAnswers(updated);
  }

  function resetForm() {
    setText("");
    setType("0");
    setAnswers([{ text: "", weight: 0 }]);
    setEditingId(null);
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);

    try {
      const formattedAnswers =
        type === "2" ? answers : answers.filter((a) => a.text.trim() !== "");

      if (editingId) {
        await updateQuestion(editingId, {
          surveyId: survey.id,
          text,
          type: parseInt(type),
          answers: formattedAnswers,
        });
      } else {
        await createQuestion({
          surveyId: survey.id,
          text,
          type: parseInt(type),
          answers: formattedAnswers,
        });
      }

      const newSurvey = await fetchSurvey(survey.id);
      setQuestions(newSurvey.questions || []);

      resetForm();
      onAdded();
    } catch (err) {
      console.error("Error saving question:", err);
    } finally {
      setLoading(false);
    }
  }

  async function handleDeleteQuestion(id) {
    if (!window.confirm("Delete this question?")) return;
    try {
      await deleteQuestion(id);
      setQuestions((prev) => prev.filter((q) => q.id !== id));
      onAdded();
    } catch (err) {
      console.error("Error deleting question:", err);
    }
  }

  function handleEditQuestion(q) {
    setEditingId(q.id);
    setText(q.text);
    setType(String(q.type));
    setAnswers(q.answers?.length ? q.answers : [{ text: "", weight: 0 }]);
  }

  function getTypeLabel(typeVal) {
    switch (typeVal) {
      case 0:
        return "Single Choice";
      case 1:
        return "Multiple Choice";
      case 2:
        return "Free Text";
      default:
        return "Unknown";
    }
  }

  const allAnswersFilled =
  type === "2" || answers.every((a) => a.text.trim() !== "");

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <h2 className={styles.title}>
            Manage Questions for "{survey.title}"
          </h2>
          <button
            className={styles.closeButton}
            onClick={onClose}
            aria-label="Close"
          >
            ✖
          </button>
        </div>

        {questions.length >= 10 && !editingId && (
          <p className={styles.notice}>
            ⚠️ This survey already has 10 questions. You must delete one before
            adding another.
          </p>
        )}

        <form onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Question text"
            value={text}
            onChange={(e) => setText(e.target.value)}
            className={styles.input}
            required
          />

          <label className={styles.label}>Question Type:</label>
          <select
            value={type}
            onChange={(e) => {
              setType(e.target.value);
              if (e.target.value !== "1")
                setAnswers([{ text: "", weight: 0 }]);
            }}
            className={styles.input}
          >
            <option value="0">Single Choice</option>
            <option value="1">Multiple Choice</option>
            <option value="2">Free Text</option>
          </select>

          <div>
            <h4>{type === "2" ? "Free Text Scoring:" : "Answers:"}</h4>

            {type !== "2" &&
              answers.map((ans, idx) => (
                <div key={idx} className={styles.answerRow}>
                  <input
                    type="text"
                    placeholder={`Answer ${idx + 1}`}
                    value={ans.text}
                    onChange={(e) =>
                      handleAnswerChange(idx, "text", e.target.value)
                    }
                    className={styles.input}
                  />
                  <label className={styles.label}>Weight:</label>
                  <input
                    type="number"
                    placeholder="Weight"
                    value={ans.weight}
                    onChange={(e) =>
                      handleAnswerChange(
                        idx,
                        "weight",
                        parseFloat(e.target.value) || 0
                      )
                    }
                    className={styles.input}
                    style={{ width: "100px" }}
                  />
                </div>
              ))}

            {type === "2" && (
              <div>
                <label className={styles.label}>Weight:</label>
                <input
                  type="number"
                  placeholder="Weight"
                  value={answers[0]?.weight ?? 0}
                  onChange={(e) =>
                    handleAnswerChange(
                      0,
                      "weight",
                      parseFloat(e.target.value) || 0
                    )
                  }
                  className={styles.input}
                  style={{ width: "100px" }}
                />
              </div>
            )}

            {type === "1" && (
              <button
                type="button"
                onClick={handleAddAnswer}
                className={`${styles.button} ${styles.buttonAdd}`}
              >
                + Add Answer
              </button>
            )}
          </div>

          <div style={{ marginTop: "1rem" }}>
            <button
              type="submit"
              className={styles.button}
              disabled={
                loading ||
                !text.trim() ||
                !allAnswersFilled ||
                (!editingId && questions.length >= 10)
              }
            >
              {editingId ? "Update Question" : "Save Question"}
            </button>

            {editingId && (
              <button
                type="button"
                onClick={resetForm}
                className={`${styles.button} ${styles.buttonCancel}`}
              >
                ✖ Cancel Edit
              </button>
            )}
          </div>
        </form>

        {questions.length > 0 && (
          <div style={{ marginTop: "2rem" }}>
            <h3 className={styles.title}>Existing Questions</h3>
            <table className={styles.table}>
              <thead>
                <tr>
                  <th className={styles.th}>Question</th>
                  <th className={styles.th}>Type</th>
                  <th className={styles.th}>Weight</th>
                  <th className={styles.th}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {questions.map((q) => {
                  let weightDisplay = "—";
                  if (q.type === 2) {
                    weightDisplay = q.answers?.[0]?.weight ?? 0;
                  } else if (q.answers && q.answers.length > 0) {
                    const maxWeight = Math.max(
                      ...q.answers.map((a) => a.weight || 0)
                    );
                    weightDisplay = maxWeight.toFixed(0);
                  }

                  return (
                    <tr key={q.id}>
                      <td className={styles.td}>{q.text}</td>
                      <td className={styles.td}>{getTypeLabel(q.type)}</td>
                      <td className={`${styles.td} ${styles.center}`}>
                        {weightDisplay}
                      </td>
                      <td className={styles.td}>
                        <button
                          onClick={() => handleEditQuestion(q)}
                          className={`${styles.button} ${styles.buttonEdit}`}
                          style={{
                            padding: "0.3rem 0.6rem",
                            fontSize: "0.85rem",
                          }}
                        >
                          ✏️ Edit
                        </button>
                        <button
                          onClick={() => handleDeleteQuestion(q.id)}
                          className={`${styles.button} ${styles.buttonDelete}`}
                          style={{
                            padding: "0.3rem 0.6rem",
                            fontSize: "0.85rem",
                            marginLeft: "0.5rem",
                          }}
                        >
                          🗑️ Delete
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

export default QuestionModal;
