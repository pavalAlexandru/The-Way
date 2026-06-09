import { useState } from "react";
import axios from "axios";
import "./GameBoard.css";

function GameBoard({ token, username, onLogout }) {
  const [sessionId, setSessionId] = useState(null);
  const [column, setColumn] = useState(1);
  const [msg, setMsg] = useState(
    "Click 'Start Game' to generate an empty board.",
  );
  const [endData, setEndData] = useState(null);

  const startGame = async () => {
    try {
      const res = await axios.post(
        "http://localhost:5231/api/game/start",
        {},
        {
          headers: { Authorization: `Bearer ${token}` },
        },
      );
      setSessionId(res.data.sessionId);
      setColumn(1);
      setEndData(null);
      setMsg("Game started! Choose a row for Column 1.");
    } catch (err) {
      console.error(err);
    }
  };

  const handleCellClick = async (row) => {
    if (endData || !sessionId) return;
    try {
      const res = await axios.post(
        "http://localhost:5231/api/game/move",
        { sessionId: sessionId, row: row },
        {
          headers: { Authorization: `Bearer ${token}` },
        },
      );
      const data = res.data;

      if (data.status === "Active") {
        setColumn((c) => c + 1);
        setMsg(
          `Correct! Score: ${data.score}. Choose row for Column ${column + 1}`,
        );
      } else {
        setEndData(data);
        setMsg(
          data.status === "Won"
            ? `YOU WON! Rank: ${data.rank} | Final Score: ${data.score}`
            : `GAME OVER! You hit an obstacle or deviated from the safe path.`,
        );
      }
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div className="game-board-wrapper">
      <div className="game-header">
        <h2>Playing as: {username}</h2>
        <button onClick={startGame}>Start New Game</button>
        {sessionId && <h3>{msg}</h3>}
      </div>

      {sessionId && (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(4, 80px)",
            gap: "10px",
            justifyContent: "center",
            margin: "20px 0",
          }}
        >
          {[1, 2, 3, 4].map((row) =>
            [1, 2, 3, 4].map((col) => (
              <button
                key={`${row}-${col}`}
                className="grid-cell"
                disabled={col !== column || endData}
                onClick={() => handleCellClick(row)}
                style={{
                  backgroundColor: col === column ? "#3498db" : "#bdc3c7",
                  color: "white",
                  padding: "20px",
                }}
              >
                R{row}C{col}
              </button>
            )),
          )}
        </div>
      )}

      {endData && (
        <div
          style={{
            padding: "15px",
            backgroundColor: "#ecf0f1",
            borderRadius: "5px",
          }}
        >
          <h4>Game Summary</h4>
          <p>
            <strong>Complete Safe Path:</strong> {endData.safePath}
          </p>
          <p>
            <strong>Obstacles Locations:</strong> {endData.obstacles}
          </p>
        </div>
      )}

      <button
        className="logout-btn"
        onClick={onLogout}
        style={{ marginTop: "20px" }}
      >
        Logout
      </button>
    </div>
  );
}

export default GameBoard;
