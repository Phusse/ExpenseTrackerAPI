import { Link, NavLink, useNavigate } from "react-router-dom";
import "./NavBar.css";
import { useAuth } from "../hooks/auth-hook";

const NavBar = () => {
	const { user, logout } = useAuth();
	const navigate = useNavigate();

	const handleLogout = () => {
		logout();
		navigate("/login");
	};

	return (
		<nav className="navbar">
			<div className="navbar__container">
				<div className="navbar__left">
					<div className="navbar__brand">
						<Link to="/dashboard" className="navbar__logo">
							Expense Tracker
						</Link>
					</div>
					<div className="navbar__links">
						<NavLink
							to="/dashboard"
							className={({ isActive }) =>
								isActive ? "navbar__link navbar__link--active" : "navbar__link"
							}
						>
							Dashboard
						</NavLink>
						<NavLink
							to="/expenses"
							className={({ isActive }) =>
								isActive ? "navbar__link navbar__link--active" : "navbar__link"
							}
						>
							Expenses
						</NavLink>
						<NavLink
							to="/savings"
							className={({ isActive }) =>
								isActive ? "navbar__link navbar__link--active" : "navbar__link"
							}
						>
							Savings
						</NavLink>
					</div>
				</div>
				<div className="navbar__right">
					<span className="navbar__user">Welcome, {user?.name}</span>
					<button className="navbar__logout-btn" onClick={handleLogout}>
						Logout
					</button>
				</div>
			</div>
		</nav>
	);
};

export default NavBar;
