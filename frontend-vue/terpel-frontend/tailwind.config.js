/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        corporate: {
          dark: '#1a1a1a',
          medium: '#2d2d2d',
          light: '#404040',
          accent: '#666666',
        },
        terpel: {
          red: '#E31E24',
          darkred: '#B71C1C',
          gray: '#4A5568',
        }
      }
    },
  },
  plugins: [],
}
