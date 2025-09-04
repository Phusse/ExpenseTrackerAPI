import { useState } from "react";
import { useAuth } from "../context/auth-hook";
import { useNavigate } from "react-router-dom";
import "./RegisterPage.css";

const RegisterPage = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      await register({ name, email, password });
      navigate('/login');
    } catch (err) {
      setError('Failed to register');
    }
  };

  return (
    <div className="register">
      <div className="register__container">
        <h2 className="register__title">Register</h2>
        {error && <p className="register__error">{error}</p>}
        <form onSubmit={handleSubmit} className="register__form">
          <div className="register__field">
            <label htmlFor="name" className="register__label">Name</label>
            <input
              id="name"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              className="register__input"
            />
          </div>
          <div className="register__field">
            <label htmlFor="email" className="register__label">Email</label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="register__input"
            />
          </div>
          <div className="register__field">
            <label htmlFor="password" className="register__label">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="register__input"
            />
          </div>
          <div className="register__actions">
            <button type="submit" className="register__button">Register</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default RegisterPage;
