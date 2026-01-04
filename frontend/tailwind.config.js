/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Custom premium palette
        background: '#0f172a', // Slate 900
        surface: '#1e293b',    // Slate 800
        primary: '#3b82f6',    // Blue 500
        secondary: '#10b981',  // Emerald 500
        accent: '#8b5cf6',     // Violet 500
        danger: '#ef4444',     // Red 500
        text: {
          main: '#f1f5f9',     // Slate 100
          muted: '#94a3b8',    // Slate 400
        }
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      }
    },
  },
  plugins: [],
}
