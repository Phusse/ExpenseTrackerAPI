import "./Button.css"

type ButtonProps = {
	children: React.ReactNode;
	onClick?: () => void;
	variant?:
	| 'primary'
	| 'primary-outline'
	| 'secondary'
	| 'danger'
	| 'danger-secondary'
	| 'confirm'
	| 'confirm-secondary';
	className?: string;
	disabled?: boolean;
};

export default function Button({ children, onClick, variant = 'primary', disabled = false, className = '' }: ButtonProps) {
	return (
		<button className={`button button--${variant} ${className}`} onClick={onClick} disabled={disabled}>
			{children}
		</button>
	);
}
