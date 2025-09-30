import React from "react";
import { createPortal } from "react-dom";
import PopupItem from "./PopupItem";
import "./PopupContainer.css";

export type PopupType = "info" | "success" | "warning" | "danger";

export type Popup = {
	id: number;
	message: string;
	type: PopupType;
	closing: boolean;
};

type PopupContainerProps = {
	popups: Popup[];
	removePopup: (id: number) => void;
};

const PopupContainer: React.FC<PopupContainerProps> = ({ popups, removePopup }) => {
	return createPortal(
		<div className="popup-container">
			{popups.map((popup) => (
				<PopupItem
					key={popup.id}
					message={popup.message}
					type={popup.type}
					closing={popup.closing}
					onClose={() => removePopup(popup.id)}
				/>
			))}
		</div>,
		document.body
	);
};

export default PopupContainer;
