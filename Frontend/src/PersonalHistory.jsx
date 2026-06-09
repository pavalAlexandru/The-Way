import { useState, useEffect } from "react";
import axios from "axios";
import "./LeaderBoard.css";

function PersonalHistory({ token }) {
  const [history, setHistory] = useState([]);

  const fetchHistory = async () => {
    try {
      const res = await axios.get("http://localhost:5231/api/game/history", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setHistory(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    fetchHistory();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  return (
    <div className="leaderboard-wrapper">
      <h3>Your Personal Match History</h3>
      <button
        onClick={fetchHistory}
        style={{
          marginBottom: "15px",
          padding: "8px 16px",
          backgroundColor: "#3498db",
          color: "white",
          border: "none",
          borderRadius: "5px",
          cursor: "pointer",
        }}
      >
        Refresh History
      </button>

      {history.length === 0 ? (
        <p className="no-data">You haven't finished any games yet.</p>
      ) : (
        <table className="leaderboard-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Status</th>
              <th>Score</th>
              <th>Time (s)</th>
            </tr>
          </thead>
          <tbody>
            {history.map((game, index) => (
              <tr key={index}>
                <td>{new Date(game.startTime).toLocaleString()}</td>
                <td
                  style={{
                    color: game.status === "Won" ? "#2ecc71" : "#e74c3c",
                    fontWeight: "bold",
                  }}
                >
                  {game.status}
                </td>
                <td>{game.score}</td>
                <td>{game.durationInSeconds}s</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default PersonalHistory;
