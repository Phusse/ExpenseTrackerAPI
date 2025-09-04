import { useEffect, useState, useRef } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";
import { authService } from "./services/auth-service";

function App() {
  const [count, setCount] = useState(0);
  const [logs, setLogs] = useState<string>("");
  const ranOnce = useRef(false); // <- ensures effect runs once

  const appendLog = (title: string, data: any) => {
    const entry = `${title}:\n${JSON.stringify(data, null, 2)}\n\n`;
    setLogs((prev) => prev + entry);
  };

  useEffect(() => {
    if (ranOnce.current) return; // prevent double run
    ranOnce.current = true;

    async function runTests() {
      try {
        const loginSuccess = await authService.login({
          email: "ggs@dev.com",
          password: "ggsggs",
        });
        appendLog("LOGIN SUCCESS", loginSuccess);

        const loginFail = await authService.login({
          email: "wrong@dev.com",
          password: "wrongpass",
        });
        appendLog("LOGIN FAILURE", loginFail);
      } catch (err) {
        appendLog("UNEXPECTED ERROR", err);
      }
    }

    runTests();
  }, []);

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>

      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>Edit <code>src/App.tsx</code> and save to test HMR</p>
      </div>

      <h2>API Logs (copy me!)</h2>
      <textarea
        value={logs}
        readOnly
        style={{ width: "100%", height: "400px", fontFamily: "monospace" }}
      />
    </>
  );
}

export default App;
