import Button from "../components/Button/Button";
import { usePopup } from "../hooks/popup-hook";
import "./ComponentTest.css";

const variants = [
	"primary",
	"secondary",
	"primary-outline",
	"danger",
	"danger-secondary",
	"confirm",
	"confirm-secondary",
] as const;

export default function ComponentTest() {
	const { addPopup } = usePopup();

	return (
		<div className="button-test-page">
			<h1>Button Variants Test</h1>

			<div className="button-test-page__row">
				{variants.map((v) => (
					<Button
						key={v}
						variant={v}
						onClick={() => addPopup(`You clicked ${v} button`, 3000)}
					>
						{v}
					</Button>
				))}
			</div>

			<h1>Popup Test</h1>
			<div className="button-test-page__row">
				<Button onClick={() => addPopup("Information message", 3000, "info")}>
					Show Info
				</Button>
				<Button onClick={() => addPopup("Success message", 3000, "success")}>
					Show Success
				</Button>
				<Button onClick={() => addPopup("Warning message", 3000, "warning")}>
					Show Warning
				</Button>
				<Button onClick={() => addPopup("Danger message", 3000, "danger")}>
					Show Danger
				</Button>
			</div>
		</div >
	);
}
