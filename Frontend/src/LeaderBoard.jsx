import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import "./LeaderBoard.css";

function LeaderBoard() {
  const [data, setData] = useState([]);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5231/leaderboardHub")
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .catch((err) => console.error("SignalR Connection Error: ", err));

    connection.on("UpdateLeaderboard", (newData) => {
      setData(newData);
    });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div className="leaderboard-wrapper">
      <h3>Live Leaderboard (Ranked Descending by Duration)</h3>
      {data.length === 0 ? (
        <p className="no-data">No successful games have been finished yet.</p>
      ) : (
        <table className="leaderboard-table">
          <thead>
            <tr>
              <th>Rank</th>
              <th>Player</th>
              <th>Score</th>
              <th>Duration (seconds)</th>
            </tr>
          </thead>
          <tbody>
            {data.map((l, index) => (
              <tr key={index}>
                <td>#{index + 1}</td>
                <td>{l.playerName}</td>
                <td>{l.score}</td>
                <td>{l.durationInSeconds}s</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default LeaderBoard;
