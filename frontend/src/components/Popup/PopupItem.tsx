import type { PopupType } from "./PopupContainer";
import "./PopupItem.css"

type PopupProps = {
	message: string;
	type: PopupType;
	closing: boolean;
	onClose: () => void;
};

const PopupItem: React.FC<PopupProps> = ({ message, type, closing, onClose }) => {
	return (
		<div
			className={`popup popup--${type} ${closing ? "popup--closing" : ""}`}
			onClick={onClose}
			role="alert"
			aria-live="assertive"
		>
			<small className="popup__message">{message}</small>
		</div>
	);
};

export default PopupItem;
