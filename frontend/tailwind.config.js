/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Palette personnalisée
        primary: {
          50: '#fdfbf8',
          100: '#faf8f2',
          200: '#f5f0e6',
          300: '#f0e8d8',
          400: '#e8dfca',
          500: '#dfd6b8',
          600: '#d4caa0',
          700: '#c9be88',
          800: '#beb270',
          900: '#b3a658',
        },
        secondary: {
          50: '#f0f4f9',
          100: '#d6e5f5',
          200: '#cbdceb',
          300: '#a8c5dd',
          400: '#85aecf',
          500: '#6d94c5',
          600: '#5a7fb5',
          700: '#476aa5',
          800: '#345595',
          900: '#214085',
        },
        accent: {
          light: '#F5EFE6',
          dark: '#E8DFCA',
          blue: '#6D94C5',
          lightBlue: '#CBDCEB',
        },
      },
      fontFamily: {
        sans: ['"Winky Sans"', 'sans-serif'],
      },
      spacing: {
        section: '5rem',
        gutter: '2rem',
      },
      boxShadow: {
        elegant: '0 10px 30px rgba(109, 148, 197, 0.15)',
        'elegant-sm': '0 4px 12px rgba(109, 148, 197, 0.1)',
        'elegant-lg': '0 20px 60px rgba(109, 148, 197, 0.2)',
      },
    },
  },
  plugins: [],
}
