import { createContext, useCallback, useState, type ReactNode } from "react";
import type { Popup, PopupType } from "../components/Popup/PopupContainer";
import PopupContainer from "../components/Popup/PopupContainer";

const MAX_POPUPS = 3 as const;

export type PopupContextType = {
	addPopup: (message: string, duration?: number, type?: PopupType) => void;
};

export const PopupContext = createContext<PopupContextType | undefined>(undefined);

export const PopupProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
	const [popups, setPopups] = useState<Popup[]>([]);

	const removePopup = (id: number) => {
		setPopups((prev) =>
			prev.map((p) => (p.id === id ? { ...p, closing: true } : p))
		);

		setTimeout(() => {
			setPopups((prev) => prev.filter((p) => p.id !== id));
		}, 300);
	};

	const addPopup = useCallback((message: string, duration: number = 3000, type: PopupType = "info") => {
		const newPopup: Popup = { id: Date.now(), message, type, closing: false };

		setPopups((prev) => {
			let updated = [...prev, newPopup];

			if (updated.length > MAX_POPUPS) {
				updated = updated.slice(1);
			}

			return updated;
		});

		setTimeout(() => removePopup(newPopup.id), duration);
	}, []);

	return (
		<PopupContext.Provider value={{ addPopup }}>
			{children}
			<PopupContainer popups={popups} removePopup={removePopup} />
		</PopupContext.Provider>
	);
};
