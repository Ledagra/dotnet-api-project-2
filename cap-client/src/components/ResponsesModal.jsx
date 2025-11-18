import { useEffect, useState } from "react";
import { fetchAllResponses } from "../api/apiClient";
import styles from "../styles/ResponsesModal.module.css";

function ResponsesModal({ onClose }) {
  const [responses, setResponses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  useEffect(() => {
    async function loadResponses() {
      try {
        setLoading(true);
        const res = await fetchAllResponses();
        setResponses(res);
        setCurrentPage(1);
      } catch (err) {
        console.error("Error loading responses:", err);
      } finally {
        setLoading(false);
      }
    }

    loadResponses();
  }, []);

  const totalPages = Math.ceil(responses.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const currentResponses = responses.slice(
    startIndex,
    startIndex + itemsPerPage
  );

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <div className={styles.header}>
          <h2 className={styles.title}>📊 Recorded Responses</h2>
          <button onClick={onClose} className={styles.closeButton}>
            ✖
          </button>
        </div>

        {loading ? (
          <p>Loading responses...</p>
        ) : responses.length === 0 ? (
          <p>No responses have been recorded yet.</p>
        ) : (
          <>
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Survey</th>
                  <th>Total Score</th>
                  <th>Selected Answers</th>
                  <th>Free Text</th>
                </tr>
              </thead>
              <tbody>
                {currentResponses.map((r) => (
                  <tr key={r.id}>
                    <td>{r.id}</td>
                    <td>{r.surveyTitle || "—"}</td>
                    <td>{r.totalScore.toFixed(0)}</td>
                    <td>{r.selectedAnswers?.join(", ") || "—"}</td>
                    <td>{r.freeText || "—"}</td>
                  </tr>
                ))}
              </tbody>
            </table>

            {totalPages > 1 && (
              <div className={styles.pagination}>
                <button
                  className={styles.pageButton}
                  onClick={() =>
                    setCurrentPage((p) => Math.max(p - 1, 1))
                  }
                  disabled={currentPage === 1}
                >
                  ⬅ Prev
                </button>

                <span className={styles.pageInfo}>
                  Page {currentPage} of {totalPages}
                </span>

                <button
                  className={styles.pageButton}
                  onClick={() =>
                    setCurrentPage((p) =>
                      Math.min(p + 1, totalPages)
                    )
                  }
                  disabled={currentPage === totalPages}
                >
                  Next ➡
                </button>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}

export default ResponsesModal;