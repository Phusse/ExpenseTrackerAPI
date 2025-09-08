import Button from "../components/Button/Button";
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
	return (
		<div className="button-test-page">
			<h1>Button Variants Test</h1>

			<div className="button-test-page__row">
				{variants.map((v) => (
					<Button key={v} variant={v}>
						{v}
					</Button>
				))}
			</div>
		</div>
	);
}
