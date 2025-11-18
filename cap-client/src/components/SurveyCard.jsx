import { useState } from "react";
import { deleteSurvey, updateSurvey } from "../api/apiClient";
import styles from "../styles/SurveyCard.module.css";

function SurveyCard({ survey, onUpdated, onAnswer, onCardClick }) {
  const [isEditing, setIsEditing] = useState(false);
  const [title, setTitle] = useState(survey.title);
  const [description, setDescription] = useState(survey.description);
  const [loading, setLoading] = useState(false);

  const hasQuestions = survey.questions && survey.questions.length > 0;

  async function handleDelete(e) {
    e.stopPropagation();
    if (!window.confirm(`Delete survey "${survey.title}"?`)) return;
    try {
      setLoading(true);
      await deleteSurvey(survey.id);
      onUpdated();
    } catch (err) {
      console.error("Error deleting survey:", err);
    } finally {
      setLoading(false);
    }
  }

  async function handleSave(e) {
    e.stopPropagation();
    try {
      setLoading(true);
      await updateSurvey(survey.id, { title, description });
      setIsEditing(false);
      onUpdated();
    } catch (err) {
      console.error("Error updating survey:", err);
    } finally {
      setLoading(false);
    }
  }

  function handleEditToggle(e) {
    e.stopPropagation();

    if (isEditing) {
      setTitle(survey.title);
      setDescription(survey.description);
    }

    setIsEditing((prev) => !prev);
  }

  function handleAnswerClick(e) {
    e.stopPropagation();
    onAnswer(survey);
  }

  function handleCardClick() {
    if (!isEditing) onCardClick(survey);
  }

  return (
    <li
      className={`${styles.card} ${isEditing ? styles.editing : ""}`}
      onClick={handleCardClick}
    >
      <div className={styles.left}>
        {isEditing ? (
          <div className={styles.editFields}>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className={styles.input}
              placeholder="Survey Title"
            />
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className={styles.textarea}
              placeholder="Survey Description"
            />
            <div className={styles.editActions}>
              <button
                className={`${styles.button} ${styles.save}`}
                onClick={handleSave}
                disabled={loading || !title.trim() || !description.trim()}
              >
                💾 Save
              </button>
              <button
                className={`${styles.button} ${styles.cancel}`}
                onClick={handleEditToggle}
                disabled={loading}
              >
                ✖ Cancel
              </button>
            </div>
          </div>
        ) : (
          <>
            <h3 className={styles.title}>{survey.title}</h3>
            <p className={styles.description}>{survey.description}</p>
          </>
        )}
      </div>

      <div className={styles.right}>
        {!isEditing && (
          <>
            {hasQuestions && (
              <button
                className={`${styles.button} ${styles.answer}`}
                onClick={handleAnswerClick}
                disabled={loading}
              >
                📝 Answer
              </button>
            )}
            <button
              className={styles.button}
              onClick={handleEditToggle}
              disabled={loading}
            >
              ✏️ Edit
            </button>
            <button
              className={`${styles.button} ${styles.delete}`}
              onClick={handleDelete}
              disabled={loading}
            >
              🗑️ Delete
            </button>
          </>
        )}
      </div>
    </li>
  );
}

export default SurveyCard;
