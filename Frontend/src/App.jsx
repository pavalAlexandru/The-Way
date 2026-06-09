import { useState } from "react";
import Login from "./components/Login";
import GameBoard from "./components/GameBoard";
import LeaderBoard from "./components/LeaderBoard";
import "./App.css";

function App() {
  const [token, setToken] = useState(null);
  const [username, setUsername] = useState("");

  const handleLogin = (jwtToken, user) => {
    setToken(jwtToken);
    setUsername(user);
  };

  const handleLogout = () => {
    setToken(null);
    setUsername("");
  };

  return (
    <div className="app-container">
      {!token ? (
        <Login onLogin={handleLogin} />
      ) : (
        <>
          <GameBoard
            token={token}
            username={username}
            onLogout={handleLogout}
          />
          <hr style={{ margin: "30px 0", border: "1px solid #eee" }} />
          <LeaderBoard />
        </>
      )}
    </div>
  );
}

export default App;
