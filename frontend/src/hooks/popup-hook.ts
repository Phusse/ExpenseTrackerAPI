import { useContext } from "react";
import { type PopupContextType, PopupContext } from "../context/PopupProvider";

export const usePopup = (): PopupContextType => {
	const context = useContext(PopupContext);

	if (!context) {
		throw new Error("usePopup must be used within a PopupProvider");
	}

	return context;
};
