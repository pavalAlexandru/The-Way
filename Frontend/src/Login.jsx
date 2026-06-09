import { useState } from "react";
import axios from "axios";
import "./Login.css";

function Login({ onLogin }) {
  const [username, setUsername] = useState("");
  const [error, setError] = useState("");

  const submit = async (e) => {
    e.preventDefault();
    try {
      const res = await axios.post("http://localhost:5231/api/game/login", {
        username,
      });
      onLogin(res.data.token, username);
    } catch {
      setError("Nickname must belong to a registered player!");
    }
  };

  return (
    <div className="login-wrapper">
      <h2>Welcome to Joc Traseu MPP</h2>
      <form onSubmit={submit}>
        <input
          type="text"
          placeholder="Enter Player Nickname"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <button type="submit">Login / Play</button>
      </form>
      {error && <p style={{ color: "red" }}>{error}</p>}
    </div>
  );
}

export default Login;
